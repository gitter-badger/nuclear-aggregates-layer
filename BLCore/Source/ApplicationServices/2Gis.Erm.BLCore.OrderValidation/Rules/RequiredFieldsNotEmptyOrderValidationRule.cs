using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить на заполненность всех ОДЗ полей
    /// </summary>
    public sealed class RequiredFieldsNotEmptyOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public RequiredFieldsNotEmptyOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        /// <summary>
        /// See http://confluence.dvlp.2gis.local/pages/viewpage.action?pageId=51577357 (Вспомогательная информация)
        /// </summary>
        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var orderDetails = _finder.Find(ruleContext.OrdersFilterPredicate)
                    .Where(o =>
                            o.BeginDistributionDate.Day != 1 ||
                            o.LegalPersonId == null ||
                            o.BranchOfficeOrganizationUnitId == null ||
                            o.InspectorCode == null ||
                            o.OwnerCode <= 0 ||
                            ((o.DiscountPercent > 0M || o.DiscountSum > 0M) && o.DiscountReasonEnum == OrderDiscountReason.None) ||
                            o.CurrencyId == null ||
                            o.ReleaseCountPlan == 0)
                    .Select(o => new
                                     {
                                         o.Id, 
                                         o.Number, 
                                         o.FirmId, 
                                         o.BeginDistributionDate, 
                                         o.CurrencyId, 
                                         o.ReleaseCountPlan, 
                                         o.LegalPersonId, 
                                         o.BranchOfficeOrganizationUnitId, 
                                         o.OwnerCode, 
                                         o.InspectorCode, 
                                         o.DiscountSum, 
                                         o.DiscountPercent,
                                         o.DiscountReasonEnum
                                     })
                    .ToArray();


            var results = new List<OrderValidationMessage>();

                foreach (var orderDetail in orderDetails)
                {
                    var sb = new StringBuilder(50);

                    Action<string> addFieldErrorAction = txt =>
                                                             {
                                                                 if (sb.Length > 0)
                                                                 {
                                                                     sb.Append(BLResources.ListSeparator);
                                                                 }
                                                                 sb.Append(txt);
                                                             };


                    if (orderDetail.BeginDistributionDate.Day != 1)
                    {
                        addFieldErrorAction(MetadataResources.BeginDistributionDate);
                    }

                    if (orderDetail.LegalPersonId == null)
                    {
                        addFieldErrorAction(MetadataResources.LegalPerson);
                    }

                    if (orderDetail.BranchOfficeOrganizationUnitId == null)
                    {
                        addFieldErrorAction(MetadataResources.BranchOfficeOrganizationUnitName);
                    }

                    if (orderDetail.OwnerCode < 1)
                    {
                        addFieldErrorAction(MetadataResources.Owner);
                    }

                    if (orderDetail.InspectorCode == null)
                    {
                        addFieldErrorAction(MetadataResources.Inspector);
                    }

                    if ((orderDetail.DiscountPercent > 0M || orderDetail.DiscountSum > 0) && orderDetail.DiscountReasonEnum == OrderDiscountReason.None)
                    {
                        addFieldErrorAction(MetadataResources.DiscountSum);
                    }

                    if (orderDetail.ReleaseCountPlan == 0)
                    {
                        addFieldErrorAction(MetadataResources.PlanReleaseCount);
                    }

                    if (orderDetail.CurrencyId == null)
                    {
                        addFieldErrorAction(MetadataResources.Currency);
                    }

                results.Add(new OrderValidationMessage
                                     {
                                         Type = MessageType.Error,
                                         MessageText = string.Format(BLResources.OrderCheckOrderHasUnspecifiedFields, sb),
                                         OrderId = orderDetail.Id,
                                         OrderNumber = orderDetail.Number
                                     });
                }

            return results;
        }
    }
}