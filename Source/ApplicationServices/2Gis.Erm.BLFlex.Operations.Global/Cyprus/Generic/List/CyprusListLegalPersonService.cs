using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.List
{
    public class CyprusListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, CyprusListLegalPersonDto>, ICyprusAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public CyprusListLegalPersonService(
            IQuerySettingsProvider querySettingsProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<CyprusListLegalPersonDto> List(QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<LegalPerson>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var restrictForMergeFilter = querySettings.CreateForExtendedProperty<LegalPerson, long>(
                "restrictForMergeId",
                restrictForMergeId =>
                {
                    var restrictedLegalPerson =
                        query.SingleOrDefault(x => x.Id == restrictForMergeId);
                    if (restrictedLegalPerson != null)
                    {
                        var legalPersonType =
                            (LegalPersonType)restrictedLegalPerson.LegalPersonTypeEnum;
                        switch (legalPersonType)
                        {
                            case LegalPersonType.LegalPerson:
                                return
                                    x =>
                                    x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                    x.Inn == restrictedLegalPerson.Inn;
                            case LegalPersonType.Businessman:
                                return
                                    x =>
                                    x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                    x.Inn == restrictedLegalPerson.Inn;
                            case LegalPersonType.NaturalPerson:
                                return
                                    x =>
                                    x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                    x.PassportNumber == restrictedLegalPerson.PassportNumber;
                        }
                    }

                    return x => x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted;
                });

            return query
                .Filter(_filterHelper, restrictForMergeFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                {
                    x.Id,
                    x.LegalName,
                    x.ShortName,
                    x.Inn,
                    x.VAT,
                    x.LegalAddress,
                    x.PassportNumber,
                    x.ClientId,
                    ClientName = x.Client.Name,
                    FirmId = x.Client.MainFirmId,
                    x.OwnerCode,
                    x.CreatedOn,
                    x.IsDeleted,
                    x.IsActive,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                        new CyprusListLegalPersonDto
                        {
                            Id = x.Id,
                            LegalName = x.LegalName,
                            ShortName = x.ShortName,
                            Tic = x.Inn,
                            Vat = x.VAT,
                            LegalAddress = x.LegalAddress,
                            PassportNumber = x.PassportNumber,
                            ClientId = x.ClientId,
                            ClientName = x.ClientName,
                            FirmId = x.FirmId,
                            OwnerCode = x.OwnerCode,
                            CreatedOn = x.CreatedOn,
                            OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                            IsDeleted = x.IsDeleted,
                            IsActive = x.IsActive
                        });
        }
    }
}