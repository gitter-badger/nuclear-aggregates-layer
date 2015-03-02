using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Shared
{
    public enum ServiceBusSalesModel
    {
        None = 0,
        CPS = 10,
        FH = 11,
        MFH = 12,
        MAR = 13
    }

    public static class ServiceBusSalesModelExtensions
    {
        public static ServiceBusSalesModel ConvertToServiceBusSalesModel(this SalesModel model)
        {
            switch (model)
            {
                case SalesModel.None:
                    return ServiceBusSalesModel.None;
                case SalesModel.GuaranteedProvision:
                    return ServiceBusSalesModel.CPS;
                case SalesModel.PlannedProvision:
                    return ServiceBusSalesModel.FH;
                case SalesModel.MultiPlannedProvision:
                    return ServiceBusSalesModel.MFH;
                case SalesModel.Media:
                    return ServiceBusSalesModel.MAR;
                default:
                    throw new ArgumentOutOfRangeException("model");
            }
        }

        public static SalesModel ConvertToSalesModel(this ServiceBusSalesModel model)
        {
            switch (model)
            {
                case ServiceBusSalesModel.CPS:
                    return SalesModel.GuaranteedProvision;
                case ServiceBusSalesModel.FH:
                    return SalesModel.PlannedProvision;
                case ServiceBusSalesModel.MFH:
                    return SalesModel.MultiPlannedProvision;
                case ServiceBusSalesModel.MAR:
                    return SalesModel.Media;
                default:
                    throw new ArgumentOutOfRangeException("model");
            }
        }
    }
}
