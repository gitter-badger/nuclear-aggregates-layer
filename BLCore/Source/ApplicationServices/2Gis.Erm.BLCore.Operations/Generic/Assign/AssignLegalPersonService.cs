using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignLegalPersonService : IAssignGenericEntityService<LegalPerson>
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IPublicService _publicService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly ITracer _tracer;

        public AssignLegalPersonService(
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory,
            IPublicService publicService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext, 
            ITracer tracer)
        {
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _publicService = publicService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _tracer = tracer;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            try
            {
                using (var operationScope  = _scopeFactory.CreateSpecificFor<AssignIdentity, LegalPerson>())
                {
                    _publicService.Handle(new ValidateOwnerIsNotReserveRequest<LegalPerson> { Id = entityId });
                    var checkAggregateForDebtsRepository = _legalPersonRepository as ICheckAggregateForDebtsRepository<LegalPerson>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, bypassValidation);
                    _legalPersonRepository.AssignWithRelatedEntities(entityId, ownerCode, isPartialAssign);

                    operationScope
                        .Updated<LegalPerson>(entityId)
                        .Complete();
                }
                
                _tracer.InfoFormat("Куратором юр.лица с id={0} назначен пользователь {1}, isPartialAssign={2}", entityId, ownerCode, isPartialAssign);
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(
                        FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, ex.Message), ex);
                }

                return new AssignResult
                           {
                               EntityId = entityId,
                               OwnerCode = ownerCode,
                               CanProceed = true,
                               Message = ex.Message
                           };
            }
            
            return null;
        }
    }
}
