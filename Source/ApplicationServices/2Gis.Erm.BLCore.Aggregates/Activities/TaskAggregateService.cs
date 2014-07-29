using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
	public sealed class TaskAggregateService : ActivityAggregateService<Task>
	{
		public TaskAggregateService(IRepository<Task> activityRepository, 
		                            IIdentityProvider identityProvider, 
		                            IOperationScopeFactory operationScopeFactory) 
			: base(activityRepository, identityProvider, operationScopeFactory)
		{
		}
	}
}