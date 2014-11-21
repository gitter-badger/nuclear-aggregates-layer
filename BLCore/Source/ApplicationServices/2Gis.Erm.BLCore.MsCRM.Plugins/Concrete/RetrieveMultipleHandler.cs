using System;
using System.Web;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins.Concrete
{
    public sealed class RetrieveMultipleHandler : BasePlugin, IPlugin
    {

        #region IPlugin Members

        public void Execute(IPluginExecutionContext context)
        {
            if (context.Depth != 1) //To calculate count of pages and records another one fetch will be executed
                return;//so to avoid infinite loops i need to check the depth of request - if it more then 2 - return

            if (context.MessageName == MessageName.RetrieveMultiple &&
                context.PrimaryEntityName != EntityName.annotation.ToString() &&
                context.PrimaryEntityName != EntityName.userquery.ToString() &&
                context.PrimaryEntityName != EntityName.savedquery.ToString())
            {
                HttpContext webContext = HttpContext.Current;

                if (webContext.Request.Path.Contains("GanttControlFrame.aspx"))
                    return;

                var entityName = context.PrimaryEntityName;
                if (Activities.Contains(entityName))
                {
                    return;
                }

                var query = (QueryExpression)context.InputParameters["query"];

                if (query.PageInfo == null || query.PageInfo.Count == 0)
                {
                    return;
                }

                int pageRecordsCount = query.PageInfo.Count;

                ICrmService crmservice = context.CreateCrmService(true);
                query.PageInfo = null;

                FilterExpression filter = query.Criteria;
                TransformCrmDateTimeFields(ref filter);
                query.Criteria = filter;

                var request = new RetrieveMultipleRequest {Query = query, ReturnDynamicEntities = true};

                int totalRecordsCount = ((RetrieveMultipleResponse)crmservice.Execute(request)).BusinessEntityCollection.BusinessEntities.Count;
                crmservice.Dispose();

                string primaryFieldName = GetPrimaryFieldName(context, entityName);
                int totalPageCount = (totalRecordsCount / pageRecordsCount) + ((totalRecordsCount % pageRecordsCount) == 0 ? 0 : 1);
                string result = string.Format("var tInfo={{tRec: {0}, tPages: {1}}}", totalRecordsCount, totalPageCount);

                var entities = (BusinessEntityCollection)context.OutputParameters["BusinessEntityCollection"];

                var counterentity = new DynamicEntity(entityName);
                counterentity[entityName + "id"] = new Key(new Guid("{FF11FA35-7D4C-4AD1-8D1C-B52F44EBD12A}"));
                counterentity[primaryFieldName] = result;

                entities.BusinessEntities.Insert(0, counterentity);
                context.OutputParameters["BusinessEntityCollection"] = entities;
            }
        }

        #endregion IPlugin Members
    }
}
