using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;
using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBargainService : ListEntityDtoServiceBase<Bargain, ListBargainDto>
    {
        private readonly IUserContext _userContext;
        private readonly IQuery _query;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public ListBargainService(
            IQuery query,
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext,
            ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _query = query;
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Bargain>();

            var myFilter = querySettings.CreateForExtendedProperty<Bargain, bool>(
                "ForMe",
                info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var restrictByLegalPersonFilter = querySettings.CreateForExtendedProperty<Bargain, long>(
                "legalPersonId",
                legalPersonId =>
                    {
                        if (legalPersonId == 0)
                        {
                            return x => true;
                        }

                        return
                            x => x.CustomerLegalPersonId == legalPersonId;
                    });

            var restrictByDealFilter = querySettings.CreateForExtendedProperty<Bargain, long>(
                "dealId",
                dealId =>
                    {
                        if (dealId == 0)
                        {
                            return x => false;
                        }

                        var filterInfo = _finder.Find(Specs.Find.ById<Deal>(dealId))
                            .Select(deal => new
                                {
                                    deal.ClientId,
                                    LegalPersonIds = deal.LegalPersonDeals.Where(link => !link.IsDeleted && link.LegalPerson.IsActive && !link.LegalPerson.IsDeleted)
                                                         .Select(link => link.LegalPersonId)
                                })
                            .Single();

                        IEnumerable<long> legalPersons;
                        if (filterInfo.LegalPersonIds.Any())
                        {
                            legalPersons = filterInfo.LegalPersonIds;
                        }
                        else
                        {
                            var childClients = _finder.Find<DenormalizedClientLink>(link => link.MasterClientId == filterInfo.ClientId)
                                                      .Select(link => link.ChildClientId)
                                                      .ToList();
                            childClients.Add(filterInfo.ClientId);

                            legalPersons = _finder.Find<LegalPerson>(person => childClients.Contains(person.ClientId.Value))
                                                  .Select(person => person.Id)
                                                  .ToArray();
                        }

                        return x => legalPersons.Contains(x.CustomerLegalPersonId);
                    });

            var restrictByBranchOfficeOrganizationUnitFilter = querySettings.CreateForExtendedProperty<Bargain, long>(
                "branchOfficeOrganizationUnitId",
                branchOfficeOrganizationUnitId =>
                {
                    if (branchOfficeOrganizationUnitId == 0)
                    {
                        return x => true;
                    }

                    return
                        x => x.ExecutorBranchOfficeId == branchOfficeOrganizationUnitId;
                });

            var hasAdvertisementAgencyManagementPrivilege =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, _userContext.Identity.Code);
            Expression<Func<Bargain, bool>> agentBargainsFilter = x => hasAdvertisementAgencyManagementPrivilege || x.BargainKind != BargainKind.Agent;

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper, myFilter, restrictByLegalPersonFilter, restrictByDealFilter, restrictByBranchOfficeOrganizationUnitFilter, agentBargainsFilter)
                .Select(x => new ListBargainDto
                {
                    Id = x.Id,
                    Number = x.Number,
                    LegalPersonId = x.CustomerLegalPersonId,
                    LegalPersonLegalName = x.LegalPerson.LegalName,
                    LegalPersonLegalAddress = x.LegalPerson.LegalAddress,
                    BranchOfficeId = x.BranchOfficeOrganizationUnit.BranchOfficeId,
                    BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    CreatedOn = x.CreatedOn,
                    BargainEndDate = x.BargainEndDate,
                    OwnerCode = x.OwnerCode,
                    ClientId = x.LegalPerson.ClientId,
                    ClientName = x.LegalPerson.Client.Name,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ExecutorBranchOfficeId = x.ExecutorBranchOfficeId,
                    BargainKindEnum = x.BargainKind,
                    BargainKind = x.BargainKind.ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}
