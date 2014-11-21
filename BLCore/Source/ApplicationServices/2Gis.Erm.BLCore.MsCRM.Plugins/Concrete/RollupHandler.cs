using System;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins.Concrete
{
    public sealed  class RollupHandler : BasePlugin, IPlugin
    {
        #region IPlugin Members

        public void Execute(IPluginExecutionContext context)
        {
            if (context.Depth != 1) //To calculate count of pages and records another one fetch will be executed
                return;//so to avoid infinite loops i need to check the depth of request - if it more then 2 - return

            if (context.MessageName == "Rollup")
            {
                var entityName = context.PrimaryEntityName;
                if (Activities.Contains(entityName))
                {
                    return;
                }
                var contextTarget = (object[])context.InputParameters["Target"];

                var target = new TargetRollupDynamic
                                 {
                                     EntityId = ((Moniker) contextTarget[1]).Id,
                                     EntityName = ((Moniker) contextTarget[1]).Name
                                 };

                var query = (QueryExpression)contextTarget[2];

                int pageRecordsCount = query.PageInfo.Count;
                if (pageRecordsCount == 0)
                    return;

                query.PageInfo = null;
                FilterExpression filter = query.Criteria;
                TransformCrmDateTimeFields(ref filter);
                query.Criteria = filter;

                target.Query = query;

                var request = new RollupRequest
                                  {
                                      ReturnDynamicEntities = true,
                                      RollupType = (RollupType) context.InputParameters["RollupType"],
                                      Target = target
                                  };

                ICrmService crmservice = context.CreateCrmService(true);
                var response = (RollupResponse)crmservice.Execute(request);
                crmservice.Dispose();

                int totalRecordsCount = response.BusinessEntityCollection.BusinessEntities.Count;

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
