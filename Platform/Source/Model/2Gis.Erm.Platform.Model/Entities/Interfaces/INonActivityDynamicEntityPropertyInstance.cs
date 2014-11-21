namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    public interface INonActivityDynamicEntityPropertyInstance : IDynamicEntityPropertyInstance
    {
        long EntityInstanceId { get; set; }
    }
}