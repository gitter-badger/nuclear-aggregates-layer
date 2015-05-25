using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Exceptions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendUserToBranchOfficeOperationService : IAppendGenericEntityService<User, BranchOffice>
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAppendUserToBranchOfficeAggregateService _appendUserToBranchOfficeAggregateService;
        private readonly IUserReadModel _userReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public AppendUserToBranchOfficeOperationService(IOperationScopeFactory scopeFactory,
                                                        IAppendUserToBranchOfficeAggregateService appendUserToBranchOfficeAggregateService,
                                                        IUserReadModel userReadModel,
                                                        IBranchOfficeReadModel branchOfficeReadModel)
        {
            _scopeFactory = scopeFactory;
            _appendUserToBranchOfficeAggregateService = appendUserToBranchOfficeAggregateService;
            _userReadModel = userReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        public void Append(AppendParams appendParams)
        {
            var branchOfficeId = appendParams.ParentId.Value;
            var userId = appendParams.AppendedId.Value;

            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, User, BranchOffice>())
            {
                if (!_userReadModel.DoesUserAndBranchOfficeHaveCommonOrganizationUnit(userId, branchOfficeId))
                {
                    var userName = _userReadModel.GetUserName(userId);
                    var branchOfficeName = _branchOfficeReadModel.GetBranchOfficeName(branchOfficeId);                    
                    throw new UserIsNotLinkedWithOrganizationUnitException(string.Format(BLResources.UserDoesntHaveSharedWithBranchOfficeOrganizationUnits, userName, branchOfficeName));
                }

                if (_userReadModel.IsUserLinkedToBranchOffice(userId, branchOfficeId))
                {
                    var userName = _userReadModel.GetUserName(userId);
                    var branchOfficeName = _branchOfficeReadModel.GetBranchOfficeName(branchOfficeId);
                    throw new EntityIsNotUniqueException(typeof(UserBranchOffice), string.Format(BLResources.UserIsAlreadyLinkedWithBranchOffice, userName, branchOfficeName));
                }

                _appendUserToBranchOfficeAggregateService.AppendUser(userId, branchOfficeId);
                scope.Complete();
            }
        }
    }
}