using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
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
				operationScope.Updated<Bank>(activity.Id);

				_activityRepository.Save();

				operationScope.Complete();
			}
		}

	    public void Delete(TActivity activity)
	    {
			using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TActivity>())
			{
				_activityRepository.Delete(activity);
				operationScope.Deleted<Bank>(activity.Id);

				_activityRepository.Save();

				operationScope.Complete();
			}
		}
    }
}
