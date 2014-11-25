namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared
{
    // TODO {all, 10.11.2014}: может быть вообще избавиться от данного типа использовать в импорте напрямую FirmAddressContactType, при необходимости изменив их строковые представления, либо схему
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