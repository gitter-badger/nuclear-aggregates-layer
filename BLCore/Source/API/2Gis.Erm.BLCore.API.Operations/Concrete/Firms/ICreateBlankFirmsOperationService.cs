using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms.Dto;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms
{
    public interface ICreateBlankFirmsOperationService : IOperation<CreateBlankFirmsIdentity>
    {
        void CreateBlankFirms(IEnumerable<BlankFirmDto> firmDtos);
    }
}