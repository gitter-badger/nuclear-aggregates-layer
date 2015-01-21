using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class ReleaseValidationResult :
        IEntity,
        IEntityKey
    {
        public long Id { get; set; }
        public long ReleaseInfoId { get; set; }
        public long? OrderId { get; set; }
        public bool IsBlocking { get; set; }
        public string RuleCode { get; set; }
        public string Message { get; set; }

        public ReleaseInfo ReleaseInfo { get; set; }
        public Order Order { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}