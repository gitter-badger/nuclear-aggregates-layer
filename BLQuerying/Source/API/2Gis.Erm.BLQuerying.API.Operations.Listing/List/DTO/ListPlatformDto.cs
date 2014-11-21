using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPlatformDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name{ get; set; }
    }
}