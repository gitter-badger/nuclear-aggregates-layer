using System;

using DoubleGis.Erm.DB.Migration.Base;
using DoubleGis.Erm.DB.Migration.Impl.Shared;
using DoubleGis.Erm.DB.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2420, "Добавление системного представления 'Клиенты в резерве для моей территории'.")]
    public class Migration2420 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var customViewField = CrmDbHelper.GetFieldsForCustomView(context);

            const String name = "Клиенты в резерве на моей территории [New]";
            const String layoutXml = 
@"
<grid name=""resultset"" object=""1"" jump=""name"" select=""1"" icon=""1"" preview=""1"">
    <row name=""result"" id=""accountid"">
        <cell name=""name"" width=""150""/>
        <cell name=""dg_mainfirm"" width=""150""/>
        <cell name=""ownerid"" width=""150""/>
        <cell name=""dg_territory"" width=""125""/>
        <cell name=""telephone1"" width=""125""/>
        <cell name=""dg_mainaddress"" width=""150""/>
        <cell name=""dg_promisingscore"" width=""150""/>
        <cell name=""dg_lastdisqualifytime"" width=""125""/>
    </row>
</grid>";

            const String fetchXmlFormat =
@"
<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
    <entity name=""account"">
        <attribute name=""name""/>
        <attribute name=""telephone1""/>
        <attribute name=""dg_territory""/>
        <attribute name=""dg_promisingscore""/>
        <attribute name=""dg_mainaddress""/>
        <attribute name=""dg_mainfirm""/>
        <attribute name=""ownerid""/>
        <attribute name=""dg_lastdisqualifytime""/>
        <attribute name=""accountid""/>
        <order attribute=""name"" descending=""false""/>
        <filter type=""and"">
            <condition attribute=""statecode"" operator=""eq"" value=""0""/>
            <condition attribute=""ownerid"" operator=""eq"" uiname=""&#1056;&#1072;&#1081;&#1086;&#1085;&#1072;, &#1056;&#1077;&#1079;&#1077;&#1088;&#1074;"" uitype=""systemuser"" value=""{{{0}}}""/>
            <filter type=""or"">
                <condition attribute=""dg_lastdisqualifytime"" operator=""null""/>
                <condition attribute=""dg_lastdisqualifytime"" operator=""olderthan-x-months"" value=""2""/>
            </filter>
        </filter>
        <link-entity name=""dg_territory"" from=""dg_territoryid"" to=""dg_territory"">
            <link-entity name=""dg_systemuser_dg_territory"" from=""dg_territoryid"" to=""dg_territoryid"">
                <filter>
                    <condition attribute=""systemuserid"" operator=""eq-userid"" />
                </filter>
            </link-entity>
        </link-entity>
    </entity>
</fetch>
"
;
            // В fetchxml присутствует условие что текущий куратор -  резерв района, 
            // подставляем туда guid пользователя "резерв района" для текущей базы.
            String fetchXml = String.Format(fetchXmlFormat, customViewField.ReserveUserId);
            var expression = CrmDbHelper.GenerateNewCustomView(name, fetchXml, layoutXml, customViewField.CrmAdministratorId, customViewField.CrmAdministratorOrganizationId, 1);
            context.InsertData(expression);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Crm; }
        }

    }
}