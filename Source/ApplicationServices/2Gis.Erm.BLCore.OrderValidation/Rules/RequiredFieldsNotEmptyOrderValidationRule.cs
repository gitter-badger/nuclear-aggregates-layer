using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить на заполненность всех ОДЗ полей
    /// </summary>
    public sealed class RequiredFieldsNotEmptyOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public RequiredFieldsNotEmptyOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        /// <summary>
        /// See http://confluence.dvlp.2gis.local/pages/viewpage.action?pageId=51577357 (Вспомогательная информация)
        /// </summary>
        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var orderDetails = _finder.Find(filterPredicate)
                    .Where(o =>
                            o.BeginDistributionDate.Day != 1 ||
                            o.LegalPersonId == null ||
                            o.BranchOfficeOrganizationUnitId == null ||
                            o.InspectorCode == null ||
                            o.OwnerCode <= 0 ||
                            ((o.DiscountPercent > 0M || o.DiscountSum > 0M) && o.DiscountReasonEnum == (int)OrderDiscountReason.None) ||
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
                    .ToList();

            if (orderDetails.Count > 0)
            {
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

                    if ((orderDetail.DiscountPercent > 0M || orderDetail.DiscountSum > 0) && orderDetail.DiscountReasonEnum == (int)OrderDiscountReason.None)
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

                    messages.Add(new OrderValidationMessage
                                     {
                                         Type = MessageType.Error,
                                         MessageText = string.Format(BLResources.OrderCheckOrderHasUnspecifiedFields, sb),
                                         OrderId = orderDetail.Id,
                                         OrderNumber = orderDetail.Number
                                     });
                }
            }
        }
    }
}