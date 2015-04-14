using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts
{
    public static class AccountingMethodExtensions
    {
        public static AccountingMethod ToAccountingMethod(this SalesModel salesModel)
        {
            switch (salesModel)
            {
                case SalesModel.GuaranteedProvision:
                    return AccountingMethod.GuaranteedProvision;
                case SalesModel.PlannedProvision:
                    return AccountingMethod.PlannedProvision;
                case SalesModel.MultiPlannedProvision:
                    return AccountingMethod.PlannedProvision;
                default:
                    throw new ArgumentOutOfRangeException("salesModel");
            }
        }

        public static IEnumerable<SalesModel> ToSalesModels(this AccountingMethod accountingMethod)
        {
            switch (accountingMethod)
            {
                case AccountingMethod.GuaranteedProvision:
                    return new[] { SalesModel.GuaranteedProvision };
                case AccountingMethod.PlannedProvision:
                    return new[]
                               {
                                   SalesModel.PlannedProvision,
                                   SalesModel.MultiPlannedProvision,
                               };
                default:
                    throw new ArgumentOutOfRangeException("accountingMethod");
            }
        }
    }
}
