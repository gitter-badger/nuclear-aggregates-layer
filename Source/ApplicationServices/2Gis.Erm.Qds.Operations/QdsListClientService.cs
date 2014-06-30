using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations.Infrastructure;
using DoubleGis.Erm.Qds.Operations.Metadata;

namespace DoubleGis.Erm.Qds.Operations
{
    public sealed class QdsListClientService : ListEntityDtoServiceBase<Client, ClientGridDoc>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly FilterHelper<ClientGridDoc> _filterHelper;
        private readonly IDebtProcessingSettings _debtProcessingSettings;
        private readonly IListGenericEntityDtoService<Client, ListClientDto> _legacyService;

        public QdsListClientService(IUserContext userContext, ISecurityServiceUserIdentifier userIdentifierService, FilterHelper<ClientGridDoc> filterHelper, IDebtProcessingSettings debtProcessingSettings, IListGenericEntityDtoService<Client, ListClientDto> legacyService)
        {
            _userContext = userContext;
            _userIdentifierService = userIdentifierService;
            _filterHelper = filterHelper;
            _debtProcessingSettings = debtProcessingSettings;
            _legacyService = legacyService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            if (!QdsDefaultFilterMetadata.ContainsFilter<ClientGridDoc>(querySettings.FilterName))
            {
                return _legacyService.List(querySettings.SearchListModel);
            }

            bool forMe;
            if (querySettings.TryGetExtendedProperty("ForMe", out forMe))
            {
                var userId = _userContext.Identity.Code.ToString();
                _filterHelper.AddFilter(x => x.Term(y => y.OwnerCode, userId));
            }

            bool forReserve;
            if (querySettings.TryGetExtendedProperty("ForReserve", out forReserve))
            {
                var reserveId = _userIdentifierService.GetReserveUserIdentity().Code.ToString();
                _filterHelper.AddFilter(x => x.Term(y => y.OwnerCode, reserveId));
            }

            bool forToday;
            if (querySettings.TryGetExtendedProperty("ForToday", out forToday))
            {
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                _filterHelper.AddFilter(x => x.Range(y => y.OnField(z => z.CreatedOn).GreaterOrEquals(userDateTimeTodayUtc).Lower(userDateTimeTomorrowUtc)));
            }

            bool withDebt;
            if (querySettings.TryGetExtendedProperty("WithDebt", out withDebt))
            {
                var minDebtAmount = (double)_debtProcessingSettings.MinDebtAmount;

                _filterHelper.AddFilter(x => x
                        .And(acc =>
                            acc.Nested(n => n.Path(c => c.LegalPersons.First().Accounts)
                                .Query(nq => nq.Bool(b => b.Must(
                                        accBalance => accBalance.Range(r => r.OnField(c => c.LegalPersons[0].Accounts[0].Balance).Lower(minDebtAmount)),
                                        accIsActive => accIsActive.Term(c => c.LegalPersons[0].Accounts[0].IsActive, true),
                                        accIsDeleted => accIsDeleted.Term(c => c.LegalPersons[0].Accounts[0].IsDeleted, false)
                                    )))),
                            lp => lp.Nested(n => n.Path(c => c.LegalPersons)
                                .Query(nq => nq.Bool(b => b.Must(
                                        accIsActive => accIsActive.Term(c => c.LegalPersons[0].IsActive, true),
                                        accIsDeleted => accIsDeleted.Term(c => c.LegalPersons[0].IsDeleted, false)
                                    )))),
                            cl => cl.Query(nq => nq.Bool(b => b.Must(
                                        accIsActive => accIsActive.Term(c => c.IsActive, true),
                                        accIsDeleted => accIsDeleted.Term(c => c.IsDeleted, false)
                                    )))
                        )
                    );
            }

            return _filterHelper.Search(querySettings);
        }
    }
}