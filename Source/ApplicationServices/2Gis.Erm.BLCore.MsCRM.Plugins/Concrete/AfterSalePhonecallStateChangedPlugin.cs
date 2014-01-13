using System;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.BLCore.MsCRM.Plugins.ErmServiceReference;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins.Concrete
{
    public class AfterSalePhonecallStateChangedPlugin : IPlugin 
    {
        private static readonly int[] AfterSalePurposes = new[]
            {
                (int)PhonecallPurposeExcerpt.Service, 
                (int)PhonecallPurposeExcerpt.Prolongation
            };

        private static readonly int[] AfterSaleTypesPurposes = new[]
            {
                (int)MsCrmAfterSaleServiceType.ASS1, 
                (int)MsCrmAfterSaleServiceType.ASS2, 
                (int)MsCrmAfterSaleServiceType.ASS3, 
                (int)MsCrmAfterSaleServiceType.ASS4
            };

        private readonly string _ermServiceAddress;

        /// <summary>
        /// Конструктор для получения настроек плагина.
        /// </summary>
        public AfterSalePhonecallStateChangedPlugin(string unsecure, string secure)
        {
            _ermServiceAddress = unsecure;
        }

        public void Execute(IPluginExecutionContext context)
        {
            if (_ermServiceAddress == null)
            {
                throw new Exception("ERM-service address is not specified");
            }

            bool isDelete = context.MessageName == MessageName.Delete;

            var referencePropertyName = (context.MessageName == MessageName.SetState || context.MessageName == MessageName.SetStateDynamicEntity) ? "EntityMoniker" : "Target";

            if (context.InputParameters.Properties.Contains(referencePropertyName) &&
                context.InputParameters.Properties[referencePropertyName] is Moniker)
            {
                var entityMoniker = (Moniker)context.InputParameters.Properties[referencePropertyName];
                if (entityMoniker.Name == EntityName.phonecall.ToString())
                {
                    DynamicEntity phonecall;
                    using (var service = context.CreateCrmService(true))
                    {
                        phonecall = GetPhonecall(service, entityMoniker.Id);
                    }

                    if (!phonecall.Properties.Contains("dg_aftersaletype"))
                    {
                        if (Debugger.IsAttached)
                        {
                            Debugger.Log(1, "Message", "afterSaleTypeProperty is absent");
                        }

                        return;
                    }

                    var afterSaleTypeProperty = phonecall.Properties["dg_aftersaletype"] as Picklist;
                    if (afterSaleTypeProperty == null || !AfterSaleTypesPurposes.Contains(afterSaleTypeProperty.Value))
                    {
                        if (Debugger.IsAttached)
                        {
                            Debugger.Log(
                                1, 
                                "Message", 
                                string.Format("afterSaleTypeProperty : {0}", afterSaleTypeProperty != null ? afterSaleTypeProperty.Value.ToString() : "null"));
                        }
                            
                        return;
                    }

                    int newActivityPurpose = ((Picklist)phonecall.Properties["dg_purpose"]).Value;
                    if (!AfterSalePurposes.Contains(newActivityPurpose))
                    {
                        return;
                    }

                    if (!isDelete && (string)phonecall.Properties["statecode"] == PhoneCallState.Open.ToString())
                    {
                        return;
                    }

                    var regardingObject = (CrmReference)phonecall.Properties["regardingobjectid"];
                    if (regardingObject.type != EntityName.opportunity.ToString())
                    {
                        return;
                    }

                    DateTime newActivityDate = ((CrmDateTime)phonecall.Properties["scheduledstart"]).UniversalTime;
                    Guid opportunityId = regardingObject.Value;

                    ErmWcfServiceHelper.SendRequest(
                        _ermServiceAddress, 
                        x => x.UpdateAfterSaleActivity(opportunityId, newActivityDate, (MsCrmAfterSaleServiceType)afterSaleTypeProperty.Value));
                }
            }
        }

        private static DynamicEntity GetPhonecall(ICrmService service, Guid phonecallId)
        {
            var targetRet = new TargetRetrieveDynamic { EntityId = phonecallId, EntityName = EntityName.phonecall.ToString() };

            var retrieveReq = new RetrieveRequest
                {
                    ColumnSet = new ColumnSet(new[] { "regardingobjectid", "dg_purpose", "scheduledstart", "statecode" }),
                    Target = targetRet,
                    ReturnDynamicEntities = true
                };

            var retrieveRes = service.Execute(retrieveReq) as RetrieveResponse;
            return (DynamicEntity)retrieveRes.BusinessEntity;
        }

        /// <summary>
        /// Synchronized with phonecall.dg_purpose picklist.
        /// </summary>
        private enum PhonecallPurposeExcerpt
        {
            Prolongation = 7,
            Service = 8
        }
    }
}
