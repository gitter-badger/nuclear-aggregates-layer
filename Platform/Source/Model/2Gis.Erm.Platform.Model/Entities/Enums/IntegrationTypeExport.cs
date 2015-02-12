using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // see also IntegrationSystem enum
    [DataContract]
    public enum IntegrationTypeExport
    {
        [EnumMember]
        None = 0,

        // ERM <-> DGPP (range 1 - 10)
        [EnumMember]
        FirmsWithActiveOrdersToDgpp = 4,

        // ERM <-> 1C (range 21 - 30)
        [EnumMember]
        LegalPersonsTo1C = 22,


        // AccountSaldosTo1C = 25, - not implemented yet
        [EnumMember]
        AccountDetailsToServiceBus = 26,

       // ERM <-> AutoMailer (range 41 - 50)
        [EnumMember]
        DataForAutoMailer = 41
    }
}