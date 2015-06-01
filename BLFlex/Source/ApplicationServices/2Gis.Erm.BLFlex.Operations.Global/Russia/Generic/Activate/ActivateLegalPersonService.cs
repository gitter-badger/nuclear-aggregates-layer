using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Activate
{
    public sealed class ActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public ActivateLegalPersonService(
            IFinder finder,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _finder = finder;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public int Activate(long entityId)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonDeactivationOrActivation, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ActivateIdentity.Instance);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var restoringLegalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(entityId)).One();

                if (restoringLegalPerson.IsActive)
                {
                    throw new ActiveEntityActivationException(typeof(LegalPerson), restoringLegalPerson.LegalName);
                }

                LegalPerson dublicateLegalPerson = null;

                switch (restoringLegalPerson.LegalPersonTypeEnum)
                {
                    case LegalPersonType.LegalPerson:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Kpp) && !string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                        {
                            dublicateLegalPerson = 
                                _finder.Find(
                                        Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                            && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.LegalPerson)
                                            && LegalPersonSpecs.LegalPersons.Find.ByInnAndKpp(restoringLegalPerson.Inn, restoringLegalPerson.Kpp))
                                    .Top();
                        }

                        break;
                    case LegalPersonType.Businessman:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                        {
                            dublicateLegalPerson =
                                _finder.Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                            && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.Businessman)
                                            && LegalPersonSpecs.LegalPersons.Find.ByInn(restoringLegalPerson.Inn))
                                    .Top();
                        }

                        break;
                    case LegalPersonType.NaturalPerson:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.PassportNumber) &&
                            !string.IsNullOrWhiteSpace(restoringLegalPerson.PassportSeries))
                        {
                            dublicateLegalPerson = 
                                _finder.Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                            && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.NaturalPerson)
                                            && LegalPersonSpecs.LegalPersons.Find.ByPassport(restoringLegalPerson.PassportSeries, restoringLegalPerson.PassportNumber))
                                    .Top();
                        }

                        break;
                    default:
                        throw new BusinessLogicException(BLResources.UnknownLegalPersonType);
                }

                if (dublicateLegalPerson != null)
                {
                    throw new NotificationException(string.Format(BLResources.ActivateLegalPersonError, dublicateLegalPerson.LegalName));
                }

                var result = _legalPersonRepository.Activate(entityId);

                operationScope
                    .Updated<LegalPerson>(entityId)
                    .Complete();

                return result;
            }
        }
    }
}
