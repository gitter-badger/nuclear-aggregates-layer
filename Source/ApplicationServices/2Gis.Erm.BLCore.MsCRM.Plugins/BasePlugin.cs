using System.Collections.Generic;
using System.Threading;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Metadata;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy.Metadata;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins
{
    public abstract class BasePlugin
    {
        protected readonly static List<string> Activities = 
            new List<string>(new[]
                { Microsoft.Crm.SdkTypeProxy.EntityName.activitypointer.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.task.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.email.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.fax.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.letter.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.campaignactivity.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.appointment.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.serviceappointment.ToString(),
                  Microsoft.Crm.SdkTypeProxy.EntityName.bulkoperation.ToString(),
                Microsoft.Crm.SdkTypeProxy.EntityName.phonecall.ToString(),
                Microsoft.Crm.SdkTypeProxy.EntityName.campaignresponse.ToString()});

        private static Dictionary<string, string> _primaryFieldNames = new Dictionary<string, string>(20);
        private readonly static object _primaryFieldNamesWriteLock = new object();

        #region Methods

        protected static string GetPrimaryFieldName(IPluginExecutionContext context, string entityName)
        {
            string result;

            if(!_primaryFieldNames.TryGetValue(entityName, out result))
            {
                lock (_primaryFieldNamesWriteLock)
                {
                    if(!_primaryFieldNames.TryGetValue(entityName, out result))
                    {
                        Dictionary<string, string> newValue = new Dictionary<string, string>(_primaryFieldNames);

                        IMetadataService mservice = context.CreateMetadataService(false);

                        var request = new RetrieveEntityRequest
                        {
                            RetrieveAsIfPublished = false,
                            LogicalName = entityName,
                            EntityItems = EntityItems.EntityOnly
                        };
                        result = ((RetrieveEntityResponse)mservice.Execute(request)).EntityMetadata.PrimaryField;
                        mservice.Dispose();
                        newValue[entityName] = result;
                        Interlocked.Exchange(ref _primaryFieldNames, newValue);
                    }
                }
            }

            return result;
        }

        protected void TransformCrmDateTimeFields(ref FilterExpression filter)
        {
            foreach (ConditionExpression condition in filter.Conditions)
                for (int i = 0; i < condition.Values.Length; i++)
                    if (condition.Values[i].GetType().Name == "CrmDateTime" &&
                        !(condition.Values[i] is CrmDateTime))
                    {
                        condition.Values[i] = condition.Values[i].GetType().GetProperty("Value").GetValue(condition.Values[i], null); // ("Value", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags., null, condition.Values[i], new object[] {});
                    }

            foreach (object t in filter.Filters)
            {
                var subfilter = (FilterExpression)t;
                TransformCrmDateTimeFields(ref subfilter);
            }
        }
        #endregion Methods
    }
}
