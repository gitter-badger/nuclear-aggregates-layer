using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class SecurityAccelerator :
        IEntity
    {
        public long UserId { get; set; }
        public long DepartmentId { get; set; }
        public int? DepartmentLeftBorder { get; set; }
        public int? DepartmentRightBorder { get; set; }
    }
}