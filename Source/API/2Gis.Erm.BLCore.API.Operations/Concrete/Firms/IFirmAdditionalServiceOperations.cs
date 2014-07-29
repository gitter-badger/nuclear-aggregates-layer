using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms
{
    // FIXME {all, 23.10.2013}: опять набор операций подвязан к одной identity - т.е. профанация операции связаны только сущностью над которой они выполняются, необходимо выделить конкретные операции SRP
    public interface IFirmAdditionalServiceOperations : IOperation<SpecifyAdditionalServicesIdentity>
    {
        void SetFirmServices(long firmId, IEnumerable<AdditionalServicesDto> services);
        void SetFirmAddressServices(long firmAddressId, IEnumerable<AdditionalServicesDto> services);
        IEnumerable<AdditionalServicesDto> GetFirmAdditionalServices(long firmId);
        IEnumerable<AdditionalServicesDto> GetFirmAddressAdditionalServices(long firmAddressId);
    }
}
