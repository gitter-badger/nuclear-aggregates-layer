using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class ModifyBargainService : IModifyBusinessModelEntityService<Bargain>
    {
        private readonly ICreateAggregateRepository<Bargain> _createAggregateService;
        private readonly IUpdateAggregateRepository<Bargain> _updateAggregateService;
        private readonly IBusinessModelEntityObtainer<Bargain> _bargainObtainer;
        private readonly IEvaluateBargainNumberService _evaluateBargainNumberService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IBargainPersistenceService _bargainPersistenceService;
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public ModifyBargainService(ICreateAggregateRepository<Bargain> createAggregateService,
                                    IUpdateAggregateRepository<Bargain> updateAggregateService,
                                    IBusinessModelEntityObtainer<Bargain> bargainObtainer,
                                    IEvaluateBargainNumberService evaluateBargainNumberService,
                                    ILegalPersonReadModel legalPersonReadModel,
                                    IBranchOfficeReadModel branchOfficeReadModel,
                                    IBargainPersistenceService bargainPersistenceService,
                                    IOrderReadModel orderReadModel,
                                    ISecurityServiceFunctionalAccess functionalAccessService,
                                    IUserContext userContext)
        {
            _createAggregateService = createAggregateService;
            _updateAggregateService = updateAggregateService;
            _bargainObtainer = bargainObtainer;
            _evaluateBargainNumberService = evaluateBargainNumberService;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _bargainPersistenceService = bargainPersistenceService;
            _orderReadModel = orderReadModel;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _bargainObtainer.ObtainBusinessModelEntity(domainEntityDto);

            // Сейчас временно запрещаем создавать агентские договоры
            if (entity.BargainKind == BargainKind.Agent)
            {
                throw new AccessToAgentBargainCreationIsDeniedException(BLResources.ItIsNotAllowedToCreateAgentBargains);
            }

            if (entity.BargainKind == BargainKind.Agent && entity.BargainEndDate == null)
            {
                throw new AgentBargainEndDateIsNotSpecifiedException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.BargainEndDate));
            }

            if (entity.BargainEndDate != null)
            {
                entity.BargainEndDate = entity.BargainEndDate.Value.GetEndOfTheDay();

                if (entity.BargainEndDate < entity.SignedOn)
                {
                    throw new BargainEndDateIsLessThanSignedOnDateException(BLResources.BargainEndDateCannotBeLessThanSignedOnDate);
                }
            }

            if (entity.IsNew())
            {
                Create(entity);
            }
            else
            {
                Update(entity);
            }

            return entity.Id;
        }

        private void Create(Bargain bargain)
        {
            if (bargain.BargainKind == BargainKind.Agent)
            {
                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement,
                                                                            _userContext.Identity.Code))
                {
                    throw new AccessToAgentBargainCreationIsDeniedException(BLResources.PrivilegeToCreateAgentBargainIsMissing);
                }

                if (_branchOfficeReadModel.GetBranchOfficeOrganizationUnitContributionType(bargain.ExecutorBranchOfficeId) == ContributionTypeEnum.Franchisees)
                {
                    throw new AgentBargainCanBeForBranchOnlyException(BLResources.AgentBargainCanBeForBranchOnly);
                }

                var duplicateNumber = _orderReadModel.GetDuplicateAgentBargainNumber(bargain.Id,
                                                                                     bargain.CustomerLegalPersonId,
                                                                                     bargain.ExecutorBranchOfficeId,
                                                                                     bargain.SignedOn,
                                                                                     bargain.BargainEndDate.Value);
                if (duplicateNumber != null)
                {
                    throw new AgentBargainIsNotUniqueException(string.Format(BLResources.AgentBargainIsNotUnique, duplicateNumber));
                }
            }

            var legalPersonOrganizationUnitCode = _legalPersonReadModel.GetLegalPersonOrganizationDgppid(bargain.CustomerLegalPersonId);
            if (legalPersonOrganizationUnitCode == null)
            {
                throw new LegalPersonOrganizationUnitCodeIsUndefinedException(BLResources.LegalPersonOrganizationUnitCodeIsUndefined);
            }

            var branchOfficeOrganizationUnitCode = _branchOfficeReadModel.GetBranchOfficeOrganizationDgppid(bargain.ExecutorBranchOfficeId);
            if (branchOfficeOrganizationUnitCode == null)
            {
                throw new BranchOfficeOrganizationUnitCodeIsUndefinedException(BLResources.BranchOfficeOrganizationUnitCodeIsUndefined);
            }

            var bargainUniqueIndex = _bargainPersistenceService.GenerateNextBargainUniqueNumber();

            bargain.Number = _evaluateBargainNumberService.Evaluate(bargain.BargainKind,
                                                                    legalPersonOrganizationUnitCode.ToString(),
                                                                    branchOfficeOrganizationUnitCode.ToString(),
                                                                    bargainUniqueIndex);

            bargain.BargainTypeId = _branchOfficeReadModel.GetBargainTypeId(bargain.ExecutorBranchOfficeId);

            _createAggregateService.Create(bargain);
        }

        private void Update(Bargain bargain)
        {
            var currentBargain = _orderReadModel.GetBargain(bargain.Id);
            if (currentBargain.BargainKind != bargain.BargainKind)
            {
                throw new BargainKindChangingException(BLResources.BargainKindChangingIsNotAllowed);
            }

            if (bargain.BargainEndDate.HasValue)
            {
                var ordersForBargain = _orderReadModel.GetBargainUsage(bargain.Id);
                var ordersForBargainWithEndDistributionDateGreaterThanBargainEndDate =
                    ordersForBargain.Where(x => x.Value > bargain.BargainEndDate.Value).Select(x => x.Key).ToArray();
                if (ordersForBargainWithEndDistributionDateGreaterThanBargainEndDate.Any())
                {
                    throw new BargainEndDateIsLessThanOrderEndDistributionDateException(
                        string.Format(BLResources.EndDistributionDateOfSomeOrdersIsGreaterThanBargainEndDate,
                                      string.Join(", ", ordersForBargainWithEndDistributionDateGreaterThanBargainEndDate)));
                }
            }

            _updateAggregateService.Update(bargain);
        }
    }
}