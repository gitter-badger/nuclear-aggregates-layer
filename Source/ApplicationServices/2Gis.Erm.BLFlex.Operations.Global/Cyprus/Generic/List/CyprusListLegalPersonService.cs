﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List;
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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.List
{
    public class CyprusListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, CyprusListLegalPersonDto>, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public CyprusListLegalPersonService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext,
            IDebtProcessingSettings debtProcessingSettings)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
            _debtProcessingSettings = debtProcessingSettings;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LegalPerson>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            if (querySettings.ParentEntityName == EntityName.Deal && querySettings.ParentEntityId.HasValue)
            {
                var clientId = _finder.Find(Specs.Find.ById<Deal>(querySettings.ParentEntityId.Value)).Select(x => x.ClientId).Single();
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

            var dealFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>("dealId", dealId => LegalPersonListSpecs.Filter.ByDeal(dealId, _finder));

            return query
                .Filter(_filterHelper, dealFilter, debtFilter, hasMyOrdersFilter, myBranchFilter, myFilter)
                .Select(x => new CyprusListLegalPersonDto
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    ShortName = x.ShortName,
                    Tic = x.Inn,
                    Vat = x.VAT,
                    LegalAddress = x.LegalAddress,
                    PassportNumber = x.PassportNumber,
                    ClientId = x.ClientId,
                    ClientName = x.Client.Name,
                    FirmId = x.Client.MainFirmId,
                    OwnerCode = x.OwnerCode,
                    CreatedOn = x.CreatedOn,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    OwnerName = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(CyprusListLegalPersonDto dto)
                {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}