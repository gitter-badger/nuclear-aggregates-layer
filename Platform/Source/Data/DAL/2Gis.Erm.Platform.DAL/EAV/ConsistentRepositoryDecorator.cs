using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    // FIXME {all, 31.07.2014}: в функционале уровня persistence ignorance  не должно быть OperationsScopes и т.п. - данный фнукционал фактически часть DAL, если это нужно чтобы consistencyverifier не ругался, так и нужно доработать verifier для поддержки EAV, а не впиливать сюда филиал слоя applicationservices
    public sealed class ConsistentRepositoryDecorator<TEntity> : IRepository<TEntity>, IDynamicEntityRepository
        where TEntity : class, IEntity, IEntityKey, IPartable
    {
        private readonly IRepository<TEntity> _entityRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly DynamicStorageRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _dynamicStorageRepository;
        private readonly IIdentityProvider _identityProvider;

        public ConsistentRepositoryDecorator(IRepository<TEntity> entityRepository,
                                             IRepository<BusinessEntityInstance> entityInstanceRepository,
                                             IRepository<BusinessEntityPropertyInstance> propertyInstanceRepository,
                                             IOperationScopeFactory operationScopeFactory,
                                             IDynamicPropertiesConverterFactory dynamicPropertiesConvertersFactory,
                                             IIdentityProvider identityProvider)
        {
            _entityRepository = entityRepository;
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
            _dynamicStorageRepository = new DynamicStorageRepository<BusinessEntityInstance, BusinessEntityPropertyInstance>(entityInstanceRepository,
                                                                                                                             propertyInstanceRepository,
                                                                                                                             dynamicPropertiesConvertersFactory);
        }

        public void Add(TEntity entity)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TEntity>())
            {
                _entityRepository.Add(entity);
                foreach (var part in entity.Parts)
                {
                    _identityProvider.SetFor(part);
                    part.EntityId = entity.Id;
                    _dynamicStorageRepository.Add(part);
                    scope.Added<BusinessEntityInstance>(part.Id);
                }

                scope.Added<TEntity>(entity.Id)
                     .Complete();
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TEntity>())
            {
                foreach (var entity in entities)
                {
                    _entityRepository.Add(entity);
                    foreach (var part in entity.Parts)
                    {
                        _identityProvider.SetFor(part);
                        part.EntityId = entity.Id;
                        _dynamicStorageRepository.Add(part);
                        scope.Added<BusinessEntityInstance>(part.Id);
                    }

                    scope.Added<TEntity>(entity.Id);
                }

                scope.Complete();
            }
        }

        public void Update(TEntity entity)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, TEntity>())
            {
                _entityRepository.Update(entity);

                foreach (var part in entity.Parts)
                {
                    _dynamicStorageRepository.Update(part);
                    scope.Updated<BusinessEntityInstance>(part.Id);
                }

                scope.Updated<TEntity>(entity.Id)
                     .Complete();
            }
        }

        public void Delete(TEntity entity)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TEntity>())
            {
                _entityRepository.Delete(entity);

                foreach (var part in entity.Parts)
                {
                    _dynamicStorageRepository.Delete(part);
                    scope.Deleted<BusinessEntityInstance>(part.Id);
                }

                scope.Deleted<TEntity>(entity.Id)
                     .Complete();
            }
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TEntity>())
            {
                foreach (var entity in entities)
                {
                    _entityRepository.Delete(entity);

                    foreach (var part in entity.Parts)
                    {
                        _dynamicStorageRepository.Delete(part);
                        scope.Deleted<BusinessEntityInstance>(part.Id);
                    }

                    scope.Deleted<TEntity>(entity.Id);
                }

                scope.Complete();
            }
        }

        public int Save()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var count = _entityRepository.Save() + _dynamicStorageRepository.Save();
                transaction.Complete();
                
                return count;
            }
        }
    }
}