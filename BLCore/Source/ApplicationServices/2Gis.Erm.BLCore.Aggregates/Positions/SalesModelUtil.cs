using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions
{
    public static class SalesModelUtil
    {
        public static bool IsPlannedProvisionSalesModel(this SalesModel salesModel)
        {
            var newSalesModels = new[]
                                     {
                                         SalesModel.PlannedProvision,
                                         SalesModel.MultiPlannedProvision
                                     };

            return newSalesModels.Contains(salesModel);
        }
    }
}
