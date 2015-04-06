using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions
{
    public static class SalesModelUtil
    {
        public static SalesModel[] PlannedProvisionSalesModels =
            {
                SalesModel.PlannedProvision,
                SalesModel.MultiPlannedProvision
            };

        public static bool IsPlannedProvisionSalesModel(this SalesModel salesModel)
        {
            return PlannedProvisionSalesModels.Contains(salesModel);
        }
    }
}