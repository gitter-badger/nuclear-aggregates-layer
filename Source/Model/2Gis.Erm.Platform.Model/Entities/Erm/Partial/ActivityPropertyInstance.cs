using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.Erm
// ReSharper restore CheckNamespace
{
    public partial class ActivityPropertyInstance : IDynamicEntityPropertyInstance
    {
        // TODO {all, 30.04.2014}: Изменить в будущем структуру БД
        public long EntityInstanceId 
        {
            get { return ActivityId; }
            set { ActivityId = value; }
        }
    }
}