using System.Collections.Generic;
using System.Transactions;

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
    public sealed class DynamicStorageRepositoryDecorator<TEntity> : IRepository<TEntity>, IDynamicEntityRepository
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly DynamicStorageRepository<DictionaryEntityInstance, DictionaryEntityPropertyInstance> _dynamicStorageRepository;

        public DynamicStorageRepositoryDecorator(IRepository<DictionaryEntityInstance> entityInstanceRepository,
                                                 IRepository<DictionaryEntityPropertyInstance> propertyInstanceRepository,
                                                 IOperationScopeFactory operationScopeFactory,
                                                 IDynamicPropertiesConverterFactory dynamicPropertiesConvertersFactory)
        {
            _operationScopeFactory = operationScopeFactory;
            _dynamicStorageRepository = new DynamicStorageRepository<DictionaryEntityInstance, DictionaryEntityPropertyInstance>(entityInstanceRepository,
                                                                                                                                 propertyInstanceRepository,
                                                                                                                                 dynamicPropertiesConvertersFactory);
        }

        public void Add(TEntity entity)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TEntity>())
            {
                _dynamicStorageRepository.Add(entity);
                scope.Added<TEntity>(entity.Id)
                     .Added<DictionaryEntityInstance>(entity.Id)
                     .Complete();
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TEntity>())
            {
                foreach (var entity in entities)
                {
                    _dynamicStorageRepository.Add(entity);
                    scope.Added<TEntity>(entity.Id)
                         .Added<DictionaryEntityInstance>(entity.Id);

                }

                scope.Complete();
            }
        }

        public void Update(TEntity entity)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, TEntity>())
            {
                _dynamicStorageRepository.Update(entity);
                scope.Updated<TEntity>(entity.Id)
                     .Updated<DictionaryEntityInstance>(entity.Id)
                     .Complete();
            }
        }

        public void Delete(TEntity entity)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TEntity>())
            {
                _dynamicStorageRepository.Delete(entity);
                scope.Deleted<TEntity>(entity.Id)
                     .Deleted<DictionaryEntityInstance>(entity.Id)
                     .Complete();
            }
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public int Save()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var result = _dynamicStorageRepository.Save();
                transaction.Complete();
                return result;
            }
        }
    }
}