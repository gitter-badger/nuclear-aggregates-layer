using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.List
{
    public sealed class CzechListLegalPersonService : ListEntityDtoServiceBase<LegalPerson, CzechListLegalPersonDto>, ICzechAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public CzechListLegalPersonService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<CzechListLegalPersonDto> GetListData(
            IQueryable<LegalPerson> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
            out int count)
        {
            var restrictForMergeFilter = filterManager.CreateForExtendedProperty<LegalPerson, long>(
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
                .ApplyFilter(restrictForMergeFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                {
                    x.Id,
                    x.LegalName,
                    x.Inn,
                    x.Ic,
                    x.LegalAddress,
                    x.ClientId,
                    ClientName = x.Client.Name,
                    x.OwnerCode,
                })
                .AsEnumerable()
                .Select(x =>
                        new CzechListLegalPersonDto
                        {
                            Id = x.Id,
                            LegalName = x.LegalName,
                            Dic = x.Inn,
                            Ic = x.Ic,
                            LegalAddress = x.LegalAddress,
                            ClientId = x.ClientId,
                            ClientName = x.ClientName,
                            OwnerCode = x.OwnerCode,
                            OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                        });
        }
    }
}