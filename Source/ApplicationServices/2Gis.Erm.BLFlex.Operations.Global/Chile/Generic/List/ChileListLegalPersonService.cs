using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.List
{
    public sealed class ChileListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, ChileListLegalPersonDto>, IChileAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ChileListLegalPersonService(
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

        protected override IEnumerable<ChileListLegalPersonDto> List(QuerySettings querySettings,
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
                                    x.Inn == restrictedLegalPerson.Inn &&
                                    x.Kpp == restrictedLegalPerson.Kpp;
                            case LegalPersonType.Businessman:
                                return
                                    x =>
                                    x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                    x.Inn == restrictedLegalPerson.Inn;
                            case LegalPersonType.NaturalPerson:
                                return
                                    x =>
                                    x.Id != restrictForMergeId && x.IsActive && !x.IsDeleted &&
                                    x.PassportNumber == restrictedLegalPerson.PassportNumber &&
                                    x.PassportSeries == restrictedLegalPerson.PassportSeries;
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
                    x.LegalAddress,
                    x.ClientId,
                    ClientName = x.Client.Name,
                    x.OwnerCode,
                    x.Inn,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ChileListLegalPersonDto
                {
                    Id = x.Id,
                    LegalName = x.LegalName,
                    Rut = x.Inn,
                    LegalAddress = x.LegalAddress,
                    ClientId = x.ClientId,
                    ClientName = x.ClientName,
                    OwnerCode = x.OwnerCode,
                    OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                });
        }
    }
}