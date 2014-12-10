using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class SecurityAccelerator :
        IEntity
    {
        public long UserId { get; set; }
        public long DepartmentId { get; set; }
        public int? DepartmentLeftBorder { get; set; }
        public int? DepartmentRightBorder { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}