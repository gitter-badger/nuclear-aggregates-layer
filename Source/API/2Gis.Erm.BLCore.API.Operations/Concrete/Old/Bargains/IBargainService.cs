using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains
{
    // FIXME {all, 21.10.2013}: фактически набор OperationServices зачем-то слепленных в один интерфейс
    public interface IBargainService
    {
        long GenerateNextBargainUniqueNumber();
        Bargain GetById(long id);
        GetOrderBargainResult GetBargain(long? branchOfficeOrganizationUnitId, long? legalPersonId, DateTime orderSignupDate);
        CloseBargainsResult CloseBargains(DateTime closeDate);
        EntityIdAndNumber CreateBargainForOrder(long orderId);
    }
}
