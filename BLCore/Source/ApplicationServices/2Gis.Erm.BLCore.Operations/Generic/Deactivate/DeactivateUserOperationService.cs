using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateUserOperationService : IDeactivateGenericEntityService<User>
    {
        private readonly IUserContext _userContext;
        private readonly ISecureFinder _finder;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IActionLogger _actionLogger;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly IDeactivateUserAggregateService _deactivateUserAggregateService;
        private readonly IAssignGenericEntityService<Appointment> _assignAppointmentOperationService;
        private readonly IAssignGenericEntityService<Letter> _assignLetterOperationService;
        private readonly IAssignGenericEntityService<Phonecall> _assignPhonecallOperationService;
        private readonly IAssignGenericEntityService<Task> _assignTaskOperationService;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;

        public DeactivateUserOperationService(
            ISecureFinder finder,
            IUserReadModel userReadModel,
            IAppointmentReadModel appointmentReadModel,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,
            ITaskReadModel taskReadModel,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IActionLogger actionLogger,
            IDeactivateUserAggregateService deactivateUserAggregateService,
            IAssignGenericEntityService<Appointment> assignAppointmentOperationService,
            IAssignGenericEntityService<Letter> assignLetterOperationService,
            IAssignGenericEntityService<Phonecall> assignPhonecallOperationService,
            IAssignGenericEntityService<Task> assignTaskOperationService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory)
        {
            _userContext = userContext;
            _finder = finder;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _actionLogger = actionLogger;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _deactivateUserAggregateService = deactivateUserAggregateService;
            _assignAppointmentOperationService = assignAppointmentOperationService;
            _assignLetterOperationService = assignLetterOperationService;
            _assignPhonecallOperationService = assignPhonecallOperationService;
            _assignTaskOperationService = assignTaskOperationService;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
        }

        public virtual DeactivateConfirmation Deactivate(long entityId, long targetOwnerCodeForUserRelations)
        {
            if (entityId == _userContext.Identity.Code)
            {
                throw new ArgumentException(BLResources.DeactivateUserCannotDeactivateYourself);
            }

            if (entityId == 0)
            {
                throw new ArgumentException(BLResources.IdentifierNotSet);
            }

            if (entityId == targetOwnerCodeForUserRelations)
            {
                throw new ArgumentException(BLResources.DeactivateCannotPickSameUser);
            }

            try
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, User>())
                {
                    // проверка на задолженности по лицевым счетам
                    var clientIds = _finder.Find(Specs.Select.Id<Client>(), Specs.Find.Owned<Client>(entityId)).ToArray();
                    var checkAggregateForDebtsRepository = _clientRepository as ICheckAggregateForDebtsRepository<Client>;
                    foreach (var clientId in clientIds)
                    {
                        checkAggregateForDebtsRepository.CheckForDebts(clientId, _userContext.Identity.Code, true);
                    }

                    // FIXME {all, 23.12.2014}: два ниже следующих вызова нужно зарефакторить, например, объединив в 1 operation service 
                    _userRepository.AssignUserRelatedEntities(entityId, targetOwnerCodeForUserRelations);
                    AssignRelatedActivities(entityId, targetOwnerCodeForUserRelations);
                    scope.Updated<User>(targetOwnerCodeForUserRelations);

                    var userInfo = _userReadModel.GetUserWithRoleRelations(entityId);
                    var profile = _userReadModel.GetProfileForUser(entityId);

                    _deactivateUserAggregateService.Deactivate(userInfo.User, profile, userInfo.RolesRelations);

                    scope.Updated<User>(userInfo.User.Id)
                         .Complete();
                }
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }

            return null;
        }

        private void AssignRelatedActivities(long previousUserId, long newUserId)
        {
            foreach (var appointment in _appointmentReadModel.LookupOpenAppointmentsOwnedBy(previousUserId))
            {
                _assignAppointmentOperationService.Assign(appointment.Id, newUserId, false, false);
            }

            foreach (var letter in _letterReadModel.LookupOpenLettersOwnedBy(previousUserId))
            {
                _assignLetterOperationService.Assign(letter.Id, newUserId, false, false);
            }

            foreach (var phonecall in _phonecallReadModel.LookupOpenPhonecallsOwnedBy(previousUserId))
            {
                _assignPhonecallOperationService.Assign(phonecall.Id, newUserId, false, false);
            }

            foreach (var task in _taskReadModel.LookupOpenTasksOwnedBy(previousUserId))
            {
                _assignTaskOperationService.Assign(task.Id, newUserId, false, false);
            }
        }
    }
}
