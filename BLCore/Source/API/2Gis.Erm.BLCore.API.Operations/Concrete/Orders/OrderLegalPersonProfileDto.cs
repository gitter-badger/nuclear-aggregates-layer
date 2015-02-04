using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class OrderLegalPersonProfileDto
    {
        public EntityReference LegalPerson { get; set; }
        public EntityReference LegalPersonProfile { get; set; }
    }
}