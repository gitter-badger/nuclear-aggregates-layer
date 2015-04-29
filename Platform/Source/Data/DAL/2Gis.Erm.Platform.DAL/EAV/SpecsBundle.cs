using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class SpecsBundle<TEntityInstance, TEntityPropertyInstance>
        where TEntityInstance : IDynamicEntityInstance where TEntityPropertyInstance : IDynamicEntityPropertyInstance
    {
        private readonly IFindSpecification<TEntityInstance> _findSpec;
        private readonly ISelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> _selectSpec;

        public SpecsBundle(IFindSpecification<TEntityInstance> findSpec,
                           ISelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> selectSpec)
        {
            _findSpec = findSpec;
            _selectSpec = selectSpec;
        }

        public IFindSpecification<TEntityInstance> FindSpec
        {
            get { return _findSpec; }
        }

        public ISelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> SelectSpec
        {
            get { return _selectSpec; }
        }
    }
}