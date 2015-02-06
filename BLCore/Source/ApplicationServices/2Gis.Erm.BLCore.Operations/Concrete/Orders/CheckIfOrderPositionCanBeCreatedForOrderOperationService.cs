using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public sealed class CheckIfOrderPositionCanBeCreatedForOrderOperationService : ICheckIfOrderPositionCanBeCreatedForOrderOperationService
    {
        private readonly IOrderReadModel _orderReadModel;

        public CheckIfOrderPositionCanBeCreatedForOrderOperationService(
            IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        public bool Check(long orderId, OrderType orderType, out string report)
        {
            if (!AreLegalPersonsSpecified(orderId, out report))
            {
                return false;
            }

            if (!IsOrderTypeCorrect(orderId, orderType, out report))
            {
                return false;
            }

            return true;
        }

        #region Проверки
        private bool AreLegalPersonsSpecified(long orderId,
                                              out string report)
        {
            report = null;
            var completionState = _orderReadModel.GetOrderCompletionState(orderId);
            if (!completionState.BranchOfficeOrganizationUnit || !completionState.LegalPerson)
            {
                var fields = new List<string>();

                if (!completionState.LegalPerson)
                {
                    fields.Add(BLResources.LegalPersonOrderFieldName);
                }

                if (!completionState.BranchOfficeOrganizationUnit)
                {
                    fields.Add(BLResources.BranchOfficeOrganizationUnitOrderFieldName);
                }

                report = string.Format(BLResources.OrderFieldsMustBeSpecified, string.Join(", ", fields));
                return false;
            }

            return true;
        }


        private bool IsOrderTypeCorrect(long orderId,
                                        OrderType orderType,
                                        out string report)
        {
            report = null;
            var currentOrderType = _orderReadModel.GetOrderType(orderId);
            if (orderType != currentOrderType)
            {
                report = BLResources.OrderTypeHasBeenChanged;
                return false;
            }

            return true;
        }

        #endregion
    }
}
