using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Specs;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.List
{
    public sealed class ChileListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, ChileListLegalPersonDto>, IChileAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ChileListLegalPersonService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper,
            IDebtProcessingSettings debtProcessingSettings,
            IUserContext userContext)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
            _debtProcessingSettings = debtProcessingSettings;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LegalPerson>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            long clientId;
            if (querySettings.TryGetExtendedProperty("ClientAndItsDescendants", out clientId))
            {
                query = _filterHelper.ForClientAndItsDescendants(query, clientId);
            }

            if (querySettings.ParentEntityName == EntityName.Deal && querySettings.ParentEntityId.HasValue)
            {
                clientId = _finder.Find(Specs.Find.ById<Deal>(querySettings.ParentEntityId.Value)).Select(x => x.ClientId).Single();
                query = _filterHelper.ForClientAndItsDescendants(query, clientId);
            }

            var debtFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("WithDebt",
                                                                                        info => LegalPersonListSpecs.Filter.WithDebt(_debtProcessingSettings.MinDebtAmount));

            var hasMyOrdersFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("HasMyOrders",
                                                                                               info => LegalPersonListSpecs.Filter.WithUserOrders(_userContext.Identity.Code));
            

            var myBranchFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("MyBranch",
                                                                                            info => LegalPersonListSpecs.Filter.ByMyBranch(_userContext.Identity.Code));

            var myFilter = querySettings.CreateForExtendedProperty<LegalPerson, bool>("ForMe",
                                                                                      forMe => LegalPersonListSpecs.Filter.ByOwner(forMe, _userContext.Identity.Code));

            // FIXME {y.baranihin, 03.09.2014}: Довольно большой кусок логики скопирован во много мест.
            // почему бы не создать реестр. аналогичный IFindSpecification?
            // есть зависимость от IFinder - его можно передавать параметром.
            // Нужно обговорить с Максимом, может, у него есть другое решение.
            // COMMENT {y.baranihin;a.rechkalov, 03.09.2014}: Кроме того, это временно решение - до полного внедрения рекламных кампаний вместо привычных сделок. 
            // Важно держать этот кусок логики в отдельном, хорошо отсвечивающем месте, чтобы можно было его легко выпилить
            // DONE {a.rechkalov, 05.09.2014}: done
            var dealFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>("dealId", dealId => LegalPersonListSpecs.Filter.ByDeal(dealId, _finder));
                    
            return query
                .Filter(_filterHelper, dealFilter, debtFilter, hasMyOrdersFilter, myBranchFilter, myFilter)
                .Select(x => new ChileListLegalPersonDto
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    LegalAddress = x.LegalAddress,
                    ClientId = x.ClientId,
                    ClientName = x.Client.Name,
                    OwnerCode = x.OwnerCode,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Rut = x.Inn,
                    OwnerName = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ChileListLegalPersonDto dto)
                {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}