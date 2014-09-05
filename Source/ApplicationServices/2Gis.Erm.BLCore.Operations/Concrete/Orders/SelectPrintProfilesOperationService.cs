using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class SelectPrintProfilesOperationService : ISelectPrintProfilesOperationService
    {
        private readonly IOrderReadModel _orderReadModel;

        public SelectPrintProfilesOperationService(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        public OrderProfilesDto SelectProfilesByOrder(long orderId)
        {
            var dto = _orderReadModel.GetOrderProfiles(orderId);

            if (!dto.Profile.Id.HasValue)
            {
                throw new EntityNotLinkedException(BLResources.LegalPersonFieldsMustBeFilled);
            }

            return dto;
        }
    }
}
