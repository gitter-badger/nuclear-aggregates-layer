using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateUserService : IActivateGenericEntityService<User>
    {
        private readonly IUserReadModel _readModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IActivateUserAggregateService _aggregateService;

        public ActivateUserService(IUserReadModel readModel, IActivateUserAggregateService aggregateService, IOperationScopeFactory operationScopeFactory)
        {
            _readModel = readModel;
            _aggregateService = aggregateService;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Activate(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, User>())
            {
                var user = _readModel.GetUser(entityId);
                var profile = _readModel.GetProfileForUser(entityId);

                _aggregateService.Activate(user, profile);

                scope.Updated<User>(user.Id)
                     .Complete();
            }

            return 0;
        }
    }
}