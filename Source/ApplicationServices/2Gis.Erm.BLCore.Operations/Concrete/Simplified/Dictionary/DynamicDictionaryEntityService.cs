using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.DictionaryEntity;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary
{
    public class DynamicDictionaryEntityService : IDynamicDictionaryEntityService
    {
        private readonly IRepository<DictionaryEntityInstance> _dictionaryEntityInstanceGenericRepository;
        private readonly IRepository<DictionaryEntityPropertyInstance> _dictionaryEntityPropertyInstanceGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DynamicDictionaryEntityService(
            IRepository<DictionaryEntityInstance> dictionaryEntityInstanceGenericRepository,
            IRepository<DictionaryEntityPropertyInstance> dictionaryEntityPropertyInstanceGenericRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory)
        {
            _dictionaryEntityInstanceGenericRepository = dictionaryEntityInstanceGenericRepository;
            _dictionaryEntityPropertyInstanceGenericRepository = dictionaryEntityPropertyInstanceGenericRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public long Create(DictionaryEntityInstance dictionaryEntityInstance, IEnumerable<DictionaryEntityPropertyInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DictionaryEntityInstance>())
            {
                _identityProvider.SetFor(dictionaryEntityInstance);
                _dictionaryEntityInstanceGenericRepository.Add(dictionaryEntityInstance);
                operationScope.Added<DictionaryEntityInstance>(dictionaryEntityInstance.Id);

                _dictionaryEntityInstanceGenericRepository.Save();

                foreach (var property in propertyInstances)
                {
                    _identityProvider.SetFor(property);
                    property.EntityInstanceId = dictionaryEntityInstance.Id;
                    _dictionaryEntityPropertyInstanceGenericRepository.Add(property);
                    operationScope.Added<DictionaryEntityPropertyInstance>(property.Id);
                }

                _dictionaryEntityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
                return dictionaryEntityInstance.Id;
            }
        }

        public void Update(DictionaryEntityInstance dictionaryEntityInstance, IEnumerable<DictionaryEntityPropertyInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DictionaryEntityInstance>())
            {
                _dictionaryEntityInstanceGenericRepository.Update(dictionaryEntityInstance);
                operationScope.Updated<DictionaryEntityInstance>(dictionaryEntityInstance.Id);

                _dictionaryEntityInstanceGenericRepository.Save();

                foreach (var property in propertyInstances)
                {
                    if (property.IsNew())
                    {
                        _identityProvider.SetFor(property);
                        property.EntityInstanceId = dictionaryEntityInstance.Id;
                        _dictionaryEntityPropertyInstanceGenericRepository.Add(property);
                        operationScope.Added<DictionaryEntityPropertyInstance>(property.Id);
                    }
                    else
                    {
                        _dictionaryEntityPropertyInstanceGenericRepository.Update(property);
                        operationScope.Updated<DictionaryEntityPropertyInstance>(property.Id);
                    }
                }

                _dictionaryEntityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }

        public void Delete(DictionaryEntityInstance dictionaryEntityInstance, IEnumerable<DictionaryEntityPropertyInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, DictionaryEntityInstance>())
            {
                foreach (var activityPropertyInstance in propertyInstances)
                {
                    _dictionaryEntityPropertyInstanceGenericRepository.Delete(activityPropertyInstance);
                    operationScope.Deleted<DictionaryEntityPropertyInstance>(activityPropertyInstance.Id);
                }

                _dictionaryEntityPropertyInstanceGenericRepository.Save();

                _dictionaryEntityInstanceGenericRepository.Delete(dictionaryEntityInstance);
                operationScope.Deleted<DictionaryEntityInstance>(dictionaryEntityInstance.Id);

                _dictionaryEntityInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}