using System;

using DoubleGis.Erm.DB.Migration.Base;
using DoubleGis.Erm.DB.Migration.Impl.Shared;
using DoubleGis.Erm.DB.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2082, "Добавление системных представлений для пользователей по городам.")]
    public class Migration2082 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var customViewField = CrmDbHelper.GetFieldsForCustomView(context);

            const String name = "Фирмы в резерве на моей территории [New]";
            const String layoutXml = 
@"<grid name=""resultset"" object=""10013"" jump=""dg_name"" select=""1"" icon=""1"" preview=""1"">
    <row name=""result"" id=""dg_firmid"">
        <cell name=""dg_account"" width=""150"" />
        <cell name=""dg_name"" width=""150"" />
        <cell name=""ownerid"" width=""150"" />
        <cell name=""dg_promisingscore"" width=""150""/>
        <cell name=""dg_territory"" width=""125"" />
        <cell name=""dg_organizationunit"" width=""150"" />
        <cell name=""dg_lastqualifytime"" width=""100"" />
        <cell name=""dg_lastdisqualifytime"" width=""150"" />
    </row>
</grid>";
            const String fetchXmlFormat = 
@"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
  <entity name=""dg_firm"">
    <attribute name=""ownerid""/>
    <attribute name=""dg_lastdisqualifytime""/>
    <attribute name=""dg_territory""/>
    <attribute name=""dg_lastqualifytime""/>
    <attribute name=""dg_promisingscore""/>
    <attribute name=""dg_organizationunit""/>
    <attribute name=""dg_account""/>
    <attribute name=""dg_firmid""/>
    <attribute name=""dg_name""/>
    <order attribute=""dg_promisingscore"" descending=""true""/>
    <filter type=""and"">
      <condition attribute=""statecode"" operator=""eq"" value=""0""/>
      <condition attribute=""ownerid"" operator=""eq"" uiname=""&#1056;&#1072;&#1081;&#1086;&#1085;&#1072;, &#1056;&#1077;&#1079;&#1077;&#1088;&#1074;"" uitype=""systemuser"" value=""{{{0}}}""/>
      <filter type=""or"">
        <condition attribute=""dg_lastdisqualifytime"" operator=""olderthan-x-months"" value=""2""/>
        <condition attribute=""dg_lastdisqualifytime"" operator=""null""/>
      </filter>
      <condition attribute=""dg_closedforascertainment"" operator=""ne"" value=""1""/>
    </filter>
    <link-entity name=""dg_territory"" from=""dg_territoryid"" to=""dg_territory"">
      <link-entity name=""dg_systemuser_dg_territory"" from=""dg_territoryid"" to=""dg_territoryid"">
        <filter>
          <condition attribute=""systemuserid"" operator=""eq-userid"" />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>"
;

            // В fetchxml присутствует условие что текущий куратор -  резерв района, 
            // подставляем туда guid пользователя "резерв района" для текущей базы.
            String fetchXml = String.Format(fetchXmlFormat, customViewField.ReserveUserId);
            var expression = CrmDbHelper.GenerateNewCustomView(name, fetchXml, layoutXml, customViewField.CrmAdministratorId, customViewField.CrmAdministratorOrganizationId, 10013);
            context.InsertData(expression);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Crm; }
        }
    }
}
