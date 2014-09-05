using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class OrderProfilesDto
    {
        public EntityReference Profile { get; set; }
        public EntityReference LegalPerson { get; set; }
    }
}