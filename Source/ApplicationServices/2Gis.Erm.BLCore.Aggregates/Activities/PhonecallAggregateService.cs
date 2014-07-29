using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
	public sealed class PhonecallAggregateService : ActivityAggregateService<Phonecall>
	{
		public PhonecallAggregateService(IRepository<Phonecall> activityRepository,
		                                 IIdentityProvider identityProvider,
		                                 IOperationScopeFactory operationScopeFactory)
			: base(activityRepository, identityProvider, operationScopeFactory)
		{
		}
	}
}