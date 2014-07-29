using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public class ActivityAggregateService : IActivityAggregateService
    {
        private readonly IRepository<ActivityInstance> _activityInstanceGenericRepository;
        private readonly IRepository<ActivityPropertyInstance> _activityPropertyInstanceGenericRepository;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ActivityAggregateService(IRepository<ActivityInstance> activityInstanceGenericRepository,
                                        IRepository<ActivityPropertyInstance> activityPropertyInstanceGenericRepository,
                                        ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                        IIdentityProvider identityProvider,
                                        IOperationScopeFactory operationScopeFactory)
        {
            _activityInstanceGenericRepository = activityInstanceGenericRepository;
            _activityPropertyInstanceGenericRepository = activityPropertyInstanceGenericRepository;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public long CreateActivity(ActivityInstance activityInstance, IEnumerable<ActivityPropertyInstance> activityPropertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, ActivityInstance>())
            {
                _identityProvider.SetFor(activityInstance);
                _activityInstanceGenericRepository.Add(activityInstance);
                operationScope.Added<ActivityInstance>(activityInstance.Id);
                
                _activityInstanceGenericRepository.Save();

                foreach (var property in activityPropertyInstances)
                {
                    _identityProvider.SetFor(property);
                    property.ActivityId = activityInstance.Id;
                    _activityPropertyInstanceGenericRepository.Add(property);
                    operationScope.Added<ActivityPropertyInstance>(property.Id);
                }

                _activityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
                return activityInstance.Id;
            }
        }

        public void UpdateActivity(ActivityInstance activityInstance, IEnumerable<ActivityPropertyInstance> activityPropertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, ActivityInstance>())
            {
                _activityInstanceGenericRepository.Update(activityInstance);
                operationScope.Updated<ActivityInstance>(activityInstance.Id);

                _activityInstanceGenericRepository.Save();

                foreach (var property in activityPropertyInstances)
                {
                    if (property.IsNew())
                    {
                        _identityProvider.SetFor(property);
                        property.ActivityId = activityInstance.Id;
                        _activityPropertyInstanceGenericRepository.Add(property);
                        operationScope.Added<ActivityPropertyInstance>(property.Id);
                    }
                    else
                    {
                        _activityPropertyInstanceGenericRepository.Update(property);
                        operationScope.Updated<ActivityPropertyInstance>(property.Id);
                    }
                }

                _activityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }

        public void DeleteActivity(ActivityInstance activityInstance, IEnumerable<ActivityPropertyInstance> activityPropertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, ActivityInstance>())
            {
                foreach (var activityPropertyInstance in activityPropertyInstances)
                {
                    _activityPropertyInstanceGenericRepository.Delete(activityPropertyInstance);
                    operationScope.Deleted<ActivityPropertyInstance>(activityPropertyInstance.Id);
                }

                _activityPropertyInstanceGenericRepository.Save();

                _activityInstanceGenericRepository.Delete(activityInstance);
                operationScope.Deleted<ActivityPropertyInstance>(activityInstance.Id);

                _activityInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }

        public bool AssignClientRelatedActivities(IEnumerable<ActivityInstance> relatedActivities)
        {
            // FIXME {d.ivanov, 14.11.2013}: Перенести в вызывающий код
            //IEnumerable<ActivityInstance> relatedActivities;
            //if (!_activityReadModel.TryGetRelatedActivities(clientId, out relatedActivities))
            //{
            //    return false;
            //}

            var reserveUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, ActivityInstance>())
            {
                foreach (var activity in relatedActivities)
                {
                    activity.OwnerCode = reserveUserCode;
                    _activityInstanceGenericRepository.Update(activity);
                    operationScope.Updated<ActivityInstance>(activity.Id);
                }

                _activityInstanceGenericRepository.Save();
                operationScope.Complete();
            }

            return true;
        }
    }
}
