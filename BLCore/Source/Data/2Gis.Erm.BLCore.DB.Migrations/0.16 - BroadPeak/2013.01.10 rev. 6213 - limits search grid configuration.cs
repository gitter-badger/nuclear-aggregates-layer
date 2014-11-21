using System.IO;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6213, "Добавление колонок в CRM-грид лимитов при отображение результатов поиска")]
    public class Migration6213 : CrmImportAndPublishCustomizationMigration
    {
        protected override XElement GetCustomizationXml(ICrmMigrationContext context)
        {
            var attachedResource = ReplicationHelper.GetAttachedResource(this, null);

            #region ВНИМАНИЕ!!! Использовать для подстановки placeholder вида {0} - нужно очень аккуратно
            // ВНИМАНИЕ!!! Использовать для подстановки placeholder вида {0} - нужно очень аккуратно, 
            // т.к. они используются не только у нас в Url isvconfig раздела customization.xml, но и в настройках самого mscrm dynamics crm - 
            // обычно в настройках представлений savedquery - внутри подразделов fetchxml.
            // Т.о. нужно дествовать аккуратнее. Варианты:
            // 1). Использовать placeholder который не будучи замененным делает не валидным XML => его не возможно накатить на crm, 
            //     это защитит от наката не корректно обработанных шаблоновс customizations. Большой минус этого варианта - студия в процессе build solution
            //     выводит такие ошибки в xml документах (добавленных как файлы в solution) в окно errors list, хотя на компилируемость проекта это не влияет
            // 2). Разделять различные части customizations части xml содержащие savedqueries отдельно, а части содержащие isvcongig отдельно.
            //     Сделать это можно или растащив их по разным миграциям, или растащив по разным ресурсам используемым в одной миграции.
            #endregion
            attachedResource = attachedResource.Replace("{0}", "Erm");

            return XElement.Load(new StringReader(attachedResource));
        }
    }
}
