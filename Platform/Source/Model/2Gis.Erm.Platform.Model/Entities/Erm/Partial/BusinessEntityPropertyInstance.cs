using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.Erm
// ReSharper restore CheckNamespace
{
    public partial class BusinessEntityPropertyInstance : INonActivityDynamicEntityPropertyInstance
    {
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

            var propertyInstance = (BusinessEntityPropertyInstance)obj;

            return this.EntityInstanceId == propertyInstance.EntityInstanceId && this.PropertyId == propertyInstance.PropertyId;
        }
    }
}