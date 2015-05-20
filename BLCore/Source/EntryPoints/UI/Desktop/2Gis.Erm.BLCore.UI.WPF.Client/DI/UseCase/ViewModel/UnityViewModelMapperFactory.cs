using System;

using DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityViewModelMapperFactory : IViewModelMapperFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDomainEntityDtoRegistry _domainEntityDtoRegistry;

        public UnityViewModelMapperFactory(IUnityContainer unityContainer, IDomainEntityDtoRegistry domainEntityDtoRegistry)
        {
            _unityContainer = unityContainer;
            _domainEntityDtoRegistry = domainEntityDtoRegistry;
        }

        public IViewModelMapper GetMapper(IEntityType entityName)
        {
            Type domainEntityDtoType;
            if (!_domainEntityDtoRegistry.TryGetDomainEntityDto(entityName, out domainEntityDtoType))
            {
                throw new InvalidOperationException("Can't resolve domain entity dto type for entityname: " + entityName);
            }

            var targetMapperAbstraction = typeof(IViewModelMapper<>).MakeGenericType(domainEntityDtoType);
            var mapper = _unityContainer.Resolve(targetMapperAbstraction) as IViewModelMapper;
            if (mapper == null)
            {
                throw new InvalidOperationException("Can't resolve mapper for entityname: " + entityName);
            }

            return mapper;
        }
    }
}