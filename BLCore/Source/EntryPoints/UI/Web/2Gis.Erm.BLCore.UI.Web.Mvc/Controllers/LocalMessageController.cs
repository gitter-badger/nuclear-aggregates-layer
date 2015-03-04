using System;
using System.IO;
using System.Net.Mime;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class LocalMessageController : ControllerBase
    {
        public static readonly ResourceManager IntegrationTypeImportResourceManager = new IntegrationTypeResourceManager<IntegrationTypeImport>();
        public static readonly ResourceManager IntegrationTypeExportResourceManager = new IntegrationTypeResourceManager<IntegrationTypeExport>();

        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly ILocalMessageRepository _localMessageRepository;

        public LocalMessageController(IMsCrmSettings msCrmSettings,
                                      IAPIOperationsServiceSettings operationsServiceSettings,
                                      IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                      IAPIIdentityServiceSettings identityServiceSettings,
                                      IUserContext userContext,
                                      ICommonLog logger,
                                      IGetBaseCurrencyService getBaseCurrencyService,
                                      ISecurityServiceFunctionalAccess functionalAccessService,
                                      IPublicService publicService,
                                      ILocalMessageRepository localMessageRepository)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _localMessageRepository = localMessageRepository;
        }

        #region import from file


        [HttpGet, UseDependencyFields]
        public ActionResult ImportFromFile()
        {
            return View(new LocalMessageImportFromFileViewModel());
        }

        [HttpPost]
        public ActionResult ImportFromFile(LocalMessageImportFromFileViewModel viewModel, HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    viewModel.SetCriticalError(BLResources.FileRequired);
                    return View(viewModel);
                }

                var isValid = true;
                if(viewModel.IntegrationType == IntegrationTypeImport.AccountDetailsFrom1C)
                {
                    if (!viewModel.BranchOfficeOrganizationUnit.Key.HasValue)
                    {
                        viewModel.SetCriticalError("Необходимо указать юр. лицо отделения организации");
                        return View(viewModel);
                    }

                    var ms = new MemoryStream(file.ContentLength);
                    file.InputStream.CopyTo(ms);
                    file.InputStream.Position = 0;
                    ms.Position = 0;
                    var request = new ValidateAccountDetailsFrom1CRequest
                        {
                            FileName = Path.GetFileName(file.FileName), 
                            InputStream = ms,
                            BranchOfficeOrganizationUnitId = viewModel.BranchOfficeOrganizationUnit.Key.Value
                        };
                    var response =
                        (ValidateAccountDetailsFrom1CResponse)_publicService.Handle(request);

                    // TODO : переделать на использование OperationController.
                    if (response.Errors.Count > 0)
                    {
                        isValid = false;
                        viewModel.SetInfo(BLResources.FileContainsErrors);
                        viewModel.MessageErrors = String.Join(Environment.NewLine, response.Errors);
                    }
                }

                if (isValid)
                {
                    _publicService.Handle(new CreateLocalMessageRequest
                        {
                            FileName = Path.GetFileName(file.FileName),
                            ContentType = file.ContentType,
                            Content = file.InputStream,
                            IntegrationType = (int)viewModel.IntegrationType,
                            Entity = new LocalMessage
                                {
                                    EventDate = DateTime.UtcNow,
                                    Status = LocalMessageStatus.NotProcessed,
                                },
                        });

                    viewModel.SetInfo(BLResources.MessageImported);
                }
            }
            catch (BusinessLogicException ex)
            {
                viewModel.Message = ex.Message;
            }

            return View(viewModel);
        }

        public ActionResult GetValidationResults(string errors)
        {
            if (errors == null)
            {
                return new EmptyResult();
            }

            return File(Encoding.UTF8.GetBytes(errors), MediaTypeNames.Text.Plain, "validation_errors.txt");
        }

        #endregion

        #region export

        [HttpGet, UseDependencyFields]
        public ActionResult Export()
        {
            // На клиенте контрол календаря настроен с constraint:
            // 1). Указан диапазон в котором должна находиться дата и в него не попадает default(DateTime)
            // 2). Не допускается любая дата не являющаяся первым число месяца
            // Если дата не удовлетворяет вышеуказанным constraint, то генеряться alert с сообщениями об ошибке
            // На клиенте же в контроле календаря указан диапазон допустимых значений даты и туда не попадает default(DateTime) - генеряться alert с сообщениями об ошибке
            // Чтобы этого избежать проблем с constraint контрола календаря отправляем заведомо корректное значение - дату начала текущего месяца 
            var date = DateTime.UtcNow.GetFirstDateOfMonth();
            var model = new LocalMessageExportViewModel
                {
                    PeriodStart = date,
                    PeriodStartFor1C = date,
                    MailSendingType = MailSendingType.Statistic
                };
            return View(model);
        }

        [HttpPost, UseDependencyFields]
        public ActionResult Export(LocalMessageExportViewModel viewModel)
        {
            try
            {
                var request = new ExportLocalMessageRequest
                    {
                        IntegrationType = viewModel.IntegrationType,
                        PeriodStart = viewModel.IntegrationType == IntegrationTypeExport.LegalPersonsTo1C
                                          ? viewModel.PeriodStartFor1C
                                          : viewModel.PeriodStart,
                        SendingType = viewModel.MailSendingType,
                        IncludeRegionalAdvertisement = viewModel.IncludeRegionalAdvertisement
                    };

                if(viewModel.OrganizationUnit != null && viewModel.OrganizationUnit.Key != null)
                {
                    request.OrganizationUnitId = viewModel.OrganizationUnit.Key.Value;
                }

                Response response = _publicService.Handle(request);

                var processResponse = response as IntegrationResponse;
                if (processResponse != null && !processResponse.DoNotDisplayProcessingAmount)
                {
                    viewModel.SetInfo(
                        string.Format(
                            BLResources.LocalMessageProcessingResultTemplate,
                            processResponse.ProcessedWithoutErrors,
                            processResponse.NonBlockingErrorsAmount,
                            processResponse.BlockingErrorsAmount));
                }
                else
                {
                    viewModel.SetInfo(BLResources.MessageExported);
                }
            }
            catch (BusinessLogicException ex)
            {
                viewModel.Message = ex.Message;
            }
            
            if (viewModel.PeriodStart.Equals(default(DateTime)) || viewModel.PeriodStartFor1C.Equals(default(DateTime)))
            {   // данный код, нужен, т.к. на клиенте контрол календаря при некоторых значениях model.IntegrationType становится disable => не постится на сервер
                // т.о. на сервер приходит в поле model.PeriodStart значение default(DateTime)
                // На клиенте контрол календаря настроен с constraint:
                // 1). Указан диапазон в котором должна находиться дата и в него не попадает default(DateTime)
                // 2). Не допускается любая дата не являющаяся первым число месяца
                // Если дата не удовлетворяет вышеуказанным constraint, то генеряться alert с сообщениями об ошибке
                // На клиенте же в контроле календаря указан диапазон допустимых значений даты и туда не попадает default(DateTime) - генеряться alert с сообщениями об ошибке
                // Чтобы этого избежать проблем с constraint контрола календаря отправляем заведомо корректное значение - дату начала текущего месяца 
                var dateTime = DateTime.UtcNow;
                viewModel.PeriodStart = new DateTime(dateTime.Year, dateTime.Month, 1);
                viewModel.PeriodStartFor1C = new DateTime(dateTime.Year, dateTime.Month, 1);
            }
            else
            {
                viewModel.PeriodStart = viewModel.PeriodStart;
                viewModel.PeriodStartFor1C = viewModel.PeriodStartFor1C;
            }

            return View(viewModel);
        }

        #endregion

        #region save as

        [HttpPost]
        public ActionResult SaveAs(long[] ids)
        {
            if (ids == null)
            {
                return new EmptyResult();
            }

            var response = (StreamResponse)_publicService.Handle(new SaveLocalMessageRequest { Ids = ids });

            return File(response.Stream, response.ContentType, response.FileName);
        }

        #endregion

        #region process message

        [HttpGet]
        public ActionResult ProcessMessages()
        {
            return View();
        }

        [HttpPost]
        public JsonNetResult ProcessMessages(long id)
        {
            _localMessageRepository.SetWaitForProcessState(id);

            return new JsonNetResult();
        }

        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CorporateQueueAccess, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            base.OnActionExecuting(filterContext);
        }

        #region nested types

        // mega-hack to use custom resource manager for integration type, see also IntegrationType enum
        private sealed class IntegrationTypeResourceManager<TEnum> : ResourceManager where TEnum : struct
        {
            public override string GetString(string name)
            {
                var nonParsedIntegrationType = name.Replace(typeof(TEnum).Name, null);

                TEnum integrationType;
                if (!Enum.TryParse(nonParsedIntegrationType, true, out integrationType))
                {
                    return nonParsedIntegrationType;
                }

                var integrationTypeInt = ((IConvertible)integrationType).ToInt32(null);
                var integrationTypeLocalizedName = (integrationType as Enum).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture);

                IntegrationSystem integrationSystem;
                if (1 <= integrationTypeInt && integrationTypeInt <= 10)
                {
                    integrationSystem = IntegrationSystem.Dgpp;
                }
                else if (11 <= integrationTypeInt && integrationTypeInt <= 20)
                {
                    integrationSystem = IntegrationSystem.Billing;
                }
                else if (21 <= integrationTypeInt && integrationTypeInt <= 30)
                {
                    integrationSystem = IntegrationSystem.OneC;
                }
                else if (31 <= integrationTypeInt && integrationTypeInt <= 40)
                {
                    integrationSystem = IntegrationSystem.Export;
                }
                else if (41 <= integrationTypeInt && integrationTypeInt <= 50)
                {
                    integrationSystem = IntegrationSystem.AutoMailer;
                }
                else
                {
                    return integrationTypeLocalizedName;
                }

                return string.Concat(integrationSystem.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture), " - ", integrationTypeLocalizedName);
            }
        }

        #endregion
    }
}
