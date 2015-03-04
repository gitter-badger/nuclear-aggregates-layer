using System;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class UploadController : ControllerBase
    {
        #region Почему вернулись к контроллеру, при наличии Upload сервиса в basicoperations
        /// от использования upload wcf сервиса из веб клиента отказались из-за проблем с cross domain scripting, и геморности обхода этих проблем
        /// в итоге web клиент использует для upload данный контроллер (контроллер InProc загружает файлы, т.е. basic operations не используется), 
        /// а остальные клиенты (ЛК, WPF) - wcf сервис
        /// Кратко суть проблем была в том, что страничку с формой и скриптами для загрузки любых файлов отдает web приложение, а непосредственно upload происходил, через
        /// basicoperations, результат возвращаемый basicoperations было не возможно прочитать, из скрипта основной формы, т.к. домены поставщики контента были разные - 
        /// итого -> обычый access denied для cross domain scripting.
        /// Если детальнее, то используется наш JS "класс" для upload file построенный на базе ExtJS - т.е. фактически динамически создается скрытая форма с 
        /// input для файла и post этой, а после выбора файла, происходит post этой скрытой формы, с перенаправленим релузтатов в также динамически создаваемый, скрытый iframe.
        /// Вот в момент когда script из страницы загрузки пытался получить доступ к результам загрузки - генерился exception access denied в методе doFormUpload ExtJS строка doc = frame.contentWindow.document || frame.contentDocument || WINDOW.frames[id].document; )
        /// Т.е. если upload происходил на другой домен, не тот с которого отдали view для странички загрузки (и скрипты для неё) - не можем прочитать результаты upload.
        /// Варианты обхода известны, см., например, http://stackoverflow.com/questions/7680776/how-to-circumvent-same-origin-policy-for-a-3rd-party-https-site
        /// Основной варианты используемый у нас - это Cross-Origin Resource Sharing - т.е. выставление нужных заголовков на серверной стороне, не сработал именно, 
        /// для случая с загрузкой файла через post формы, для обычных AJAX запросов это рабочее решение. 
        /// Был опробован вариант с подменой домена через скрипт (document.domain method), тут проблемы:
        ///  - нужно менять output сервера - чтобы этой был корректный html, с нужным embedded script для помены - это просто вопрос вкуса
        ///  - собственно значение domain которое сервер должен был проставить в embedded script, возвращаемого html, нужно было передавать с клиента (т.к. только клиент значет с какого домена отдали страницу загрузки файла)
        ///     можно было передавать через headers запроса - вариант отпал, т.к. extjs игнорирует headers при file upload form.submit
        ///     передавать как содержимое формы (т.е. params в Ext.Ajax.request) - тут жопа уже на серверной стороне, т.к. смешиавет содержимое файла, и доп.настройки в одном Stream (в котором раньше был просто файл)
        /// В итоге от всех таких вариантов (upload через wcf сервис basicoperation) отказались, востановив, uploadcontroller - т.е. фактически метод похожий на The Reverse Proxy method 
        #endregion
        private readonly IOperationServicesManager _operationServicesManager;

        public UploadController(IMsCrmSettings msCrmSettings,
                                IAPIOperationsServiceSettings operationsServiceSettings,
                                IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                IAPIIdentityServiceSettings identityServiceSettings,
                                IUserContext userContext,
                                ICommonLog logger,
                                IGetBaseCurrencyService getBaseCurrencyService,
                                IOperationServicesManager operationServicesManager)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
        }

        [HttpPost]
        public JsonNetResult Upload(EntityName entityTypeName, long? fileId, long? entityId, HttpPostedFileBase file)
        {
            IUploadFileService uploadFileService;
            try
            {
                uploadFileService = _operationServicesManager.GetUploadFileService(entityTypeName);
            }
            catch (Exception ex)
            {
                throw new NotificationException(BLResources.EntityDoesNotSupportFileOperations, ex);
            }

            var uploadFileParams = new UploadFileParams
            {
                EntityId = entityId ?? 0,
                FileId = fileId ?? 0,
                FileName = file.FileName,
                Content = file.InputStream,
                ContentType = file.ContentType,
                ContentLength = file.ContentLength
            };

            try
            {
                var uploadFileResult = uploadFileService.UploadFile(uploadFileParams);

                var result = new
                {
                    uploadFileResult.FileId,
                    uploadFileResult.FileName,
                    uploadFileResult.ContentType,
                    uploadFileResult.ContentLength,
                    IsFileContentChanged = true,
                };

                return new JsonNetResult(result) { ContentType = MediaTypeNames.Text.Html };
            }
            catch (Exception ex)
            {
                // todo: не работает на клиенте
                Logger.Error(ex, BLResources.ErrorDuringOperation);
                var errorText = ExceptionFilter.HandleException(ex, Response);
                Response.StatusCode = 200;
                var result = new { Message = errorText };

                return new JsonNetResult(result) { ContentType = MediaTypeNames.Text.Html };
            }
        }
    }
}
