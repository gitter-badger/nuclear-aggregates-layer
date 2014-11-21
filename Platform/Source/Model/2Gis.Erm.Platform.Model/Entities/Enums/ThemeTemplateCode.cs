using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    [DataContract]
    public enum ThemeTemplateCode
    {
        [EnumMember]
        Default = 0,
        [EnumMember]
        Red = 1,
        [EnumMember]
        Green = 2,
        [EnumMember]
        Blue = 3,
        [EnumMember]
        Skyscraper = 4,
    }
}
