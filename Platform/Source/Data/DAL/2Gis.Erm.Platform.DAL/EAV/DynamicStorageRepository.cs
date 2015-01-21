using System;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal sealed class DynamicStorageRepository<TEntityInstance, TEntityPropertyInstance>
        where TEntityInstance : class, IDynamicEntityInstance
        where TEntityPropertyInstance : class, IDynamicEntityPropertyInstance
    {
        private readonly IRepository<TEntityInstance> _entityInstanceRepository;
        private readonly IRepository<TEntityPropertyInstance> _propertyInstanceRepository;
        private readonly IDynamicPropertiesConverterFactory _dynamicPropertiesConvertersFactory;

        public DynamicStorageRepository(IRepository<TEntityInstance> entityInstanceRepository,
                                        IRepository<TEntityPropertyInstance> propertyInstanceRepository,
                                        IDynamicPropertiesConverterFactory dynamicPropertiesConvertersFactory)
        {
            _entityInstanceRepository = entityInstanceRepository;
            _propertyInstanceRepository = propertyInstanceRepository;
            _dynamicPropertiesConvertersFactory = dynamicPropertiesConvertersFactory;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity, IEntityKey
        {
            var dynamicEntityInstanceInfo = GetEntityInstanceDto(entity);

            var dynamicEntityInstance = dynamicEntityInstanceInfo.EntityInstance;

            _entityInstanceRepository.Add(dynamicEntityInstance);

            foreach (var dynamicEntityPropertyInstance in dynamicEntityInstanceInfo.PropertyInstances)
            {
                dynamicEntityPropertyInstance.EntityInstanceId = dynamicEntityInstance.Id;
                _propertyInstanceRepository.Add(dynamicEntityPropertyInstance);
            }
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity, IEntityKey
        {
            var dto = GetEntityInstanceDto(entity);

            _entityInstanceRepository.Update(dto.EntityInstance);

            foreach (var dynamicEntityPropertyInstance in dto.PropertyInstances)
            {
                _propertyInstanceRepository.Update(dynamicEntityPropertyInstance);
            }
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity, IEntityKey
        {
            var dto = GetEntityInstanceDto(entity);

            _entityInstanceRepository.Delete(dto.EntityInstance);

            foreach (var dynamicEntityPropertyInstance in dto.PropertyInstances)
            {
                _propertyInstanceRepository.Delete(dynamicEntityPropertyInstance);
            }
        }

        public DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance> GetEntityInstanceDto(IEntity entity)
        {
            var typeArguments = new[]
                {
                    entity.GetType(),
                    typeof(TEntityInstance),
                    typeof(TEntityPropertyInstance)
                };

            var type = typeof(EntityPartConverterHost<,,>).MakeGenericType(typeArguments);
            var instance = (IEntityPartConverterHost<TEntityInstance, TEntityPropertyInstance>)Activator.CreateInstance(type, new object[] { _dynamicPropertiesConvertersFactory });
            return instance.ConvertBack(entity);
        }

        public int Save()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var count = _entityInstanceRepository.Save() + _propertyInstanceRepository.Save();
                transaction.Complete();

                return count;
            }
        }
    }
}
