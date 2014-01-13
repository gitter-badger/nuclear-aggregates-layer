﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates;
using DoubleGis.Erm.BLCore.Aggregates.Clients;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.UserOperations;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateUserService : IDeactivateGenericEntityService<User>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IUserContext _userContext;
        private readonly ISecureFinder _finder;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeactivateUserService(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ISecureFinder finder,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IPublicService publicService, 
            IOperationScopeFactory scopeFactory)
        {
            _msCrmSettings = msCrmSettings;
            _userContext = userContext;
            _finder = finder;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
        }

        public virtual DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            if (entityId == _userContext.Identity.Code)
            {
                throw new ArgumentException(BLResources.DeactivateUserCannotDeactivateYourself);
            }

            if (entityId == 0)
            {
                throw new ArgumentException(BLResources.IdentifierNotSet);
            }

            if (entityId == ownerCode)
            {
                throw new ArgumentException(BLResources.DeactivateCannotPickSameUser);
            }

            try
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, User>())
                {
                    // проверка на задолженности по лицевым счетам
                    var clientIds = _finder.Find(Specs.Select.Id<Client>(), Specs.Find.Owned<Client>(entityId)).ToArray();
                    var checkAggregateForDebtsRepository = _clientRepository as ICheckAggregateForDebtsRepository<Client>;
                    foreach (var clientId in clientIds)
                    {
                        checkAggregateForDebtsRepository.CheckForDebts(clientId, _userContext.Identity.Code, true);
                    }

                    _userRepository.AssignUserRelatedEntites(entityId, ownerCode);

                    var deactivateAggregateRepository = _userRepository as IDeactivateAggregateRepository<User>;
                    deactivateAggregateRepository.Deactivate(entityId);

                    operationScope
                        .Updated<User>(entityId, ownerCode)
                        .Complete();
                }

                if (_msCrmSettings.EnableReplication)
                {
                    _publicService.Handle(new UpdateUserCrmRelatedEntitiesRequest { AssignedUserCode = ownerCode, DeactivatedUserCode = entityId });
                }
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }

            return null;
        }
    }
}
