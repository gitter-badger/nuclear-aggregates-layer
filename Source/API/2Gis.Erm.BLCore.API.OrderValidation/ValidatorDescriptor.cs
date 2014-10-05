using System;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class ValidatorDescriptor : IEquatable<ValidatorDescriptor>
    {
        public ValidatorDescriptor(OrderValidationRuleGroup ruleGroup, int ruleCode)
        {
            RuleGroup = ruleGroup;
            RuleCode = ruleCode;
        }

        public OrderValidationRuleGroup RuleGroup { get; private set; }
        public int RuleCode { get; private set; }

        #region Implementation of IEquatable<EntitiesDescriptor>

        bool IEquatable<ValidatorDescriptor>.Equals(ValidatorDescriptor other)
        {
            return Equals(other);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(ValidatorDescriptor) != obj.GetType())
            {
                return false;
            }

            var other = (ValidatorDescriptor)obj;
            return RuleGroup.Equals(other.RuleGroup) && RuleCode.Equals(other.RuleCode);
        }

        public override int GetHashCode()
        {
            return RuleGroup.GetHashCode() ^ RuleCode.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Group={0}. RuleCode={1}", RuleGroup, RuleCode);
        }
    }
}