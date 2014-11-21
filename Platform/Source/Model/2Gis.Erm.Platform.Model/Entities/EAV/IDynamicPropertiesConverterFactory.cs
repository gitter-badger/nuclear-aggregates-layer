using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.EAV
{
    public interface IDynamicPropertiesConverterFactory
    {
        IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TPropertyInstace> Create<TEntity, TEntityInstance, TPropertyInstace>()
            where TEntity : IEntity
            where TEntityInstance : IDynamicEntityInstance
            where TPropertyInstace : IDynamicEntityPropertyInstance;
    }
}
