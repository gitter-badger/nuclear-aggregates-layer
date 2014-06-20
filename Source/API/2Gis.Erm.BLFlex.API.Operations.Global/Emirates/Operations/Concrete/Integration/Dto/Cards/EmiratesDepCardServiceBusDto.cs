using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards
{
    public sealed class EmiratesDepCardServiceBusDto : CardServiceBusDto, IEmiratesAdapted
    {
        public long Code { get; set; }
        public bool IsHiddenOrArchived { get; set; }

        public IEnumerable<ContactDto> Contacts { get; set; }
    }
}