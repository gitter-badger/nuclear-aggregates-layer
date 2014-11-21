using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    [DataContract]
    public enum IntegrationSystem
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Erm = 1,
        [EnumMember]
        Dgpp = 2,
        [EnumMember]
        Billing = 3,
        //// can't use _1C because on code inspections
        [EnumMember]
        OneC = 4,
        [EnumMember]
        Export = 5,
        [EnumMember]
        AutoMailer = 6,
    }
}