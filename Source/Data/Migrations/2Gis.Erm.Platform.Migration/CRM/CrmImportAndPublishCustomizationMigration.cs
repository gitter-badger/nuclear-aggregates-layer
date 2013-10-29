using System;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Migration.Base;

using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client.Services;

namespace DoubleGis.Erm.Platform.Migration.CRM
{
    public abstract class CrmImportAndPublishCustomizationMigration : IContextedMigration<ICrmMigrationContext>, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmWebService; }
        }

        public void Revert(ICrmMigrationContext context)
        {
            throw new NotImplementedException();
        }

        public void Apply(ICrmMigrationContext context)
        {
            using (IOrganizationService service = context.CrmContext.CreateService())
            {
                var importRequest = new ImportAllXmlRequest();

                var customizationsXml = GetCustomizationXml(context);
                if (customizationsXml == null)
                {
                    throw new ArgumentException("CustomizationXml cannot be null");
                }

                importRequest.CustomizationXml = customizationsXml.ToString();
                var importResponse = (ImportAllXmlResponse)service.Execute(importRequest);

                var request = new PublishAllXmlRequest();

                var response = (PublishAllXmlResponse)service.Execute(request);
            }
        }

        /// <summary>
        /// С помощью context (в частности, св-ва context.Options.EnvironmentSuffix) 
        /// можно настроить xml под тестовую среду 
        /// ВНИМАНИЕ!!! Использовать для подстановки placeholder вида {0} - нужно очень аккуратно, 
        /// т.к. они используются не только у нас в Url isvconfig раздела customization.xml, но и в настройках самого mscrm dynamics crm - 
        /// обычно в настройках представлений savedquery - внутри подразделов fetchxml.
        /// Т.о. нужно дествовать аккуратнее. Варианты:
        /// 1). Использовать placeholder который не будучи замененным делает не валидным XML => его не возможно накатить на crm, 
        ///     это защитит от наката не корректно обработанных шаблоновс customizations. Большой минус этого варианта - студия в процессе build solution
        ///     выводит такие ошибки в xml документах (добавленных как файлы в solution) в окно errors list, хотя на компилируемость проекта это не влияет
        /// 2). Разделять различные части customizations части xml содержащие savedqueries отдельно, а части содержащие isvcongig отдельно.
        ///     Сделать это можно или растащив их по разным миграциям, или растащив по разным ресурсам используемым в одной миграции.
        /// </summary>
        protected abstract XElement GetCustomizationXml(ICrmMigrationContext context);
    }
}
