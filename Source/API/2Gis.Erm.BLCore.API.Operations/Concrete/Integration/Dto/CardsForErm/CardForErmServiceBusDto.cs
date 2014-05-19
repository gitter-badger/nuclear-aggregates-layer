using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    [ServiceBusObjectDescription("CardForERM")]
    public class CardForErmServiceBusDto : IServiceBusDto<FlowCardsForErm>
    {
        public long Code { get; set; }
        public CardType Type { get; set; }
        public IEnumerable<ContactDto> Contacts { get; set; }
        public IEnumerable<CardRubricDto> Rubrics { get; set; }

        public CardAddressDto Address { get; set; }
        public ScheduleDto Schedule { get; set; }
        public PaymentDto Payment { get; set; }
        public long FirmCode { get; set; }
        public int BranchCode { get; set; }
        public bool IsActive { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsLinked { get; set; }
        public bool IsDeleted { get; set; }

        public ImportFirmAddressDto ToImportFirmAddressDto()
        {
            return new ImportFirmAddressDto
                {
                    Code = Code,
                    BranchCode = BranchCode,
                    Address = Address.Text,
                    TerritoryCode = Address.TerritoryCode,
                    FirmCode = FirmCode,
                    IsActive = IsActive,
                    ClosedForAscertainment = ClosedForAscertainment,
                    IsDeleted = IsDeleted,
                    IsLinked = IsLinked,
                    Payment = Payment.Text,
                    Schedule = Schedule.Text
                };
        }

        public ImportDepCardDto ToImportDepCardDto()
        {
            return new ImportDepCardDto
                {
                    Code = Code,
                    IsHiddenOrArchived = !IsActive || IsDeleted || ClosedForAscertainment
                };
        }
    }

    public class ContactDto
    {
        public ContactType ContactType { private get; set; }
        public string Contact { private get; set; }
        public int SortingPosition { private get; set; }

        public ImportFirmContactDto ToImportFirmContactDto()
        {
            return new ImportFirmContactDto
                {
                    Contact = Contact,
                    ContactType = (int)ContactType,
                    SortingPosition = SortingPosition
                };
        }
    }

    public class CardAddressDto
    {
        public long? TerritoryCode { get; set; }
        public string Text { get; set; }
    }

    public class CardRubricDto
    {
        public int Code { private get; set; }
        public bool IsPrimary { private get; set; }
        public int SortingPosition { private get; set; }

        public ImportCategoryFirmAddressDto ToCategoryFirmAddressDto()
        {
            return new ImportCategoryFirmAddressDto
                {
                    Code = Code,
                    IsPrimary = IsPrimary,
                    SortingPosition = SortingPosition
                };
        }
    }

    public class ScheduleDto
    {
        public string Text { get; set; }
    }

    public class PaymentDto
    {
        public string Text { get; set; }
    }

    public enum CardType
    {
        Pos,
        Dep
    }

    // Should be in sync with FirmAddressContactType
    public enum ContactType
    {
        // ReSharper disable UnusedMember.Local (used implicitly)
        None = 0,
        Phone = 1,
        Fax = 2,
        Email = 3,
        Web = 4,
        Icq = 5,
        Skype = 6,
        Jabber = 7 // == Other
        // ReSharper restore UnusedMember.Local
    }
}