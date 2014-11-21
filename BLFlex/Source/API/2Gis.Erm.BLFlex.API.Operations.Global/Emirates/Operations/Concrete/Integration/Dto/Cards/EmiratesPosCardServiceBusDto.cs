using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards
{
    public sealed class EmiratesPosCardServiceBusDto : CardServiceBusDto, IEmiratesAdapted
    {
        public long Code { get; set; }
        public long FirmCode { get; set; }
        public int BranchCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool ClosedForAscertainment { get; set; }

        public long? BuildingCode { get; set; }
        public long? AddressCode { get; set; }

        public bool IsLinkedToTheMap { get; set; }
        public string Address { get; set; }
        public string ReferencePoint { get; set; }

        public string PoBox { get; set; }

        public string Schedule { get; set; }

        public IEnumerable<int> PaymentMethodCodes { get; set; }
        public IEnumerable<ContactDto> Contacts { get; set; }
        public IEnumerable<ImportCategoryFirmAddressDto> Rubrics { get; set; }
    }
}