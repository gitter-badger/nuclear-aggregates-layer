using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Phonecall : ActivityBase
    {
        public ActivityPurpose Purpose { get; set; }
        public byte AfterSaleServiceType { get; set; }
    }
}
