using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersonProfiles;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPersonProfile;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LegalPersonProfiles
{
    public sealed class MakeLegalPersonProfileMainHandler : RequestHandler<MakeLegalPersonProfileMainRequest, EmptyResponse>
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;

        public MakeLegalPersonProfileMainHandler(
            ILegalPersonRepository legalPersonRepository, 
            ISecurityServiceEntityAccess securityServiceEntityAccess,
            IUserContext userContext, 
            IOperationScopeFactory scopeFactory)
        {
            _legalPersonRepository = legalPersonRepository;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(MakeLegalPersonProfileMainRequest request)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<SetAsMainLegalPersonProfileIdentity>())
            {
                var legalPerson = _legalPersonRepository.FindLegalPersonByProfile(request.LegalPersonProfileId);
                var hasAccess = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                             EntityName.LegalPerson,
                                                                             _userContext.Identity.Code,
                                                                             legalPerson.Id,
                                                                             legalPerson.OwnerCode,
                                                                             null);

                if (!hasAccess)
                {
                    throw new NotificationException(BLResources.AccessDenied);
                }

                _legalPersonRepository.SetProfileAsMain(request.LegalPersonProfileId);

                operationScope
                    .Updated<LegalPersonProfile>(request.LegalPersonProfileId)
                    .Complete();
            }
            
            return Response.Empty;
        }
    }
}
