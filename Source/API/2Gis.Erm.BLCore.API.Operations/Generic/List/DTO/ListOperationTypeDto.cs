using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListOperationTypeDto : IListItemEntityDto<OperationType>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string OperationTypeName { get; set; }
    }
}