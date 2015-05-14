using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class SpecsBundle<TEntityInstance, TEntityPropertyInstance>
        where TEntityInstance : class, IDynamicEntityInstance 
        where TEntityPropertyInstance : class, IDynamicEntityPropertyInstance
    {
        private readonly FindSpecification<TEntityInstance> _findSpec;
        private readonly SelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> _selectSpec;

        public SpecsBundle(FindSpecification<TEntityInstance> findSpec,
                           SelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> selectSpec)
        {
            _findSpec = findSpec;
            _selectSpec = selectSpec;
        }

        public FindSpecification<TEntityInstance> FindSpec
        {
            get { return _findSpec; }
        }

        public SelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> SelectSpec
        {
            get { return _selectSpec; }
        }
    }
}