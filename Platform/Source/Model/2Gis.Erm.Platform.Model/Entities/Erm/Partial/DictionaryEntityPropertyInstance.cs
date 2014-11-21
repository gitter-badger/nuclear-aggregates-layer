using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
// ReSharper restore CheckNamespace
{
    public partial class DictionaryEntityPropertyInstance : INonActivityDynamicEntityPropertyInstance
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

            var propertyInstance = (DictionaryEntityPropertyInstance)obj;

            return this.EntityInstanceId == propertyInstance.EntityInstanceId && this.PropertyId == propertyInstance.PropertyId;
        }
    }
}