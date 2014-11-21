using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // see also IntegrationSystem enum
    [DataContract]
    public enum IntegrationTypeImport
    {
        [EnumMember]
        None = 0,

        // ERM <-> DGPP (range 1 - 10)
        [EnumMember]
        FirmsFromDgpp = 1,

        // CategoriesFromDgpp = 2, - устарело, больше не обрабатывается
        [EnumMember]
        TerritoriesFromDgpp = 3,

        // ERM <-> Billing (range 11 - 20)
        // WithdrawalsFromBilling = 11, - устарело, больше не обрабатывается

        // ERM <-> 1C (range 21 - 30)
        [EnumMember]
        AccountDetailsFrom1C = 21
    }
}