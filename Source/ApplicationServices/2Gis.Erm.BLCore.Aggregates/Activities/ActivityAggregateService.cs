using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    // TODO {s.pomadin, 31.07.2014}: CRUD стиль в агрегате, стоит продумать положение действий в domain model, а также их разбиение на aggregates, entitites и т.п. пока видиться, мало общего между разными подвидами действий, в любом случае для SRP лучше разделить данный тип как и соответсвующую ему абстракцию
	public abstract class ActivityAggregateService<TActivity> : IActivityAggregateService<TActivity> where TActivity : ActivityBase
    {
	    private readonly IRepository<TActivity> _activityRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        protected ActivityAggregateService(IRepository<TActivity> activityRepository,
                                        IIdentityProvider identityProvider,
                                        IOperationScopeFactory operationScopeFactory)
        {
	        _activityRepository = activityRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

	    public long Create(TActivity activity)
	    {
			using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TActivity>())
			{
				_identityProvider.SetFor(activity);
				_activityRepository.Add(activity);
				operationScope.Added<TActivity>(activity.Id);

				_activityRepository.Save();
				operationScope.Complete();

				return activity.Id;
			}
		}

	    public void Update(TActivity activity)
	    {
			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, TActivity>())
			{
                _activityRepository.Update(activity);
				operationScope.Updated<TActivity>(activity.Id);

				_activityRepository.Save();
				operationScope.Complete();
			}
		}

	    public void Delete(TActivity activity)
	    {
			using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TActivity>())
			{
				_activityRepository.Delete(activity);
				operationScope.Deleted<TActivity>(activity.Id);

				_activityRepository.Save();
				operationScope.Complete();
			}
		}
    }
}
