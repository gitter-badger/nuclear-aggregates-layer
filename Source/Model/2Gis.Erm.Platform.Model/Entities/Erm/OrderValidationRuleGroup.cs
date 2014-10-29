using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrderValidationRuleGroup :
        IEntity,
        IEntityKey
    {
        public OrderValidationRuleGroup()
        {
            OrderValidationRuleGroupDetails = new HashSet<OrderValidationRuleGroupDetail>();
        }

        public long Id { get; set; }
        public int Code { get; set; }

        public ICollection<OrderValidationRuleGroupDetail> OrderValidationRuleGroupDetails { get; set; }

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