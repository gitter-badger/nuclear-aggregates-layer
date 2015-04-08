using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Microsoft.Practices.Unity;

using Nest;

namespace DoubleGis.Erm.BLQuerying.WCF.Operations.Listing.DI
{
    public sealed class UnityQdsExtendedInfoFilterMetadata : IQdsExtendedInfoFilterMetadata
    {
        private readonly IUnityContainer _unityContainer;
        private readonly Dictionary<Tuple<Type, string>, Delegate> _filtersMap = new Dictionary<Tuple<Type, string>, Delegate>();

        public UnityQdsExtendedInfoFilterMetadata(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            RegisterAll();
        }

        private void RegisterAll()
        {
            RegisterExtendedInfoFilter<OrderGridDoc, bool>("NotDeleted", value => x => x.Term(t => t.IsDeleted, false));

            RegisterExtendedInfoFilter<DepartmentGridDoc, bool>("ActiveAndNotDeleted", value => x => x.Bool(b => b.Must(f => f.Term(t => t.IsActive, true), f => f.Term(t => t.IsDeleted, false))));
            RegisterExtendedInfoFilter<DepartmentGridDoc, bool>("NotActiveAndNotDeleted", value => x => x.Bool(b => b.Must(f => f.Term(t => t.IsActive, false), f => f.Term(t => t.IsDeleted, false))));
            RegisterExtendedInfoFilter<DepartmentGridDoc, string>("excludeId", value => x => x.Not(n => n.Term(t => t.Id, value)));

            RegisterExtendedInfoFilter<FirmGridDoc, bool>("NotDeleted", value => x => x.Term(t => t.IsDeleted, false));
            RegisterExtendedInfoFilter<FirmGridDoc, bool>("ActiveAndNotDeleted", value => x => x.Bool(b => b.Must(f => f.Term(t => t.IsActive, true), f => f.Term(t => t.IsDeleted, false))));
            RegisterExtendedInfoFilter<FirmGridDoc, bool>("ActiveBusinessMeaning", value => x => x.Bool(b => b.Must(f => f.Term(t => t.IsActive, true), f => f.Term(t => t.IsDeleted, false), f => f.Term(t => t.ClosedForAscertainment, false))));
            RegisterExtendedInfoFilter<FirmGridDoc, bool>("InactiveBusinessMeaning", value => x => x.Bool(b => b.Must(f => f.Term(t => t.IsDeleted, false)).Should(f => f.Term(t => t.IsActive, false), f => f.Term(t => t.ClosedForAscertainment, true))));
            RegisterExtendedInfoFilter<FirmGridDoc, string>("organizationUnitId", value => x => x.Term(t => t.OrganizationUnitId, value));
            RegisterExtendedInfoFilter<FirmGridDoc, bool>("ForReserve", value =>
            {
                var userIdentifierService = _unityContainer.Resolve<ISecurityServiceUserIdentifier>();
                var reserveId = userIdentifierService.GetReserveUserIdentity().Code.ToString();
                if (value)
                {
                    return x => x.Term(t => t.OwnerCode, reserveId);
                }
                return x => x.Not(n => n.Term(t => t.OwnerCode, reserveId));
            });
            RegisterExtendedInfoFilter<FirmGridDoc, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code.ToString();
                return x => x.Term(y => y.OwnerCode, userId);
            });

            RegisterExtendedInfoFilter<ClientGridDoc, bool>("ActiveAndNotDeleted", value => x => x.Bool(b => b.Must(f => f.Term(t => t.IsActive, true), f => f.Term(t => t.IsDeleted, false))));
            RegisterExtendedInfoFilter<ClientGridDoc, bool>("Warm", value => x => x.Term(t => t.InformationSourceEnum, InformationSource.WarmClient));
            RegisterExtendedInfoFilter<ClientGridDoc, bool>("ForReserve", value =>
            {
                var userIdentifierService = _unityContainer.Resolve<ISecurityServiceUserIdentifier>();
                var reserveId = userIdentifierService.GetReserveUserIdentity().Code.ToString();
                if (value)
                {
                    return x => x.Term(t => t.OwnerCode, reserveId);
                }
                return x => x.Not(n => n.Term(t => t.OwnerCode, reserveId));
            });
            RegisterExtendedInfoFilter<ClientGridDoc, bool>("ForMe", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userId = userContext.Identity.Code.ToString();
                return x => x.Term(y => y.OwnerCode, userId);
            });
            RegisterExtendedInfoFilter<ClientGridDoc, bool>("ForToday", value =>
            {
                var userContext = _unityContainer.Resolve<IUserContext>();
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                return x => x.Range(y => y.OnField(z => z.CreatedOn).GreaterOrEquals(userDateTimeTodayUtc).Lower(userDateTimeTomorrowUtc));
            });
        }

        public void RegisterExtendedInfoFilter<TDocument, TInfoType>(string filterName, Func<TInfoType, Func<FilterDescriptor<TDocument>, FilterContainer>> func)
            where TDocument : class
        {
            var key = Tuple.Create(typeof(TDocument), filterName.ToLowerInvariant());
            Func<string, Func<FilterDescriptor<TDocument>, FilterContainer>> value = x =>
            {
                var argument = (TInfoType)Convert.ChangeType(x, typeof(TInfoType), CultureInfo.InvariantCulture);
                return func(argument);
            };
            _filtersMap.Add(key, value);
        }

        public IReadOnlyCollection<Func<FilterDescriptor<TDocument>, FilterContainer>> GetExtendedInfoFilters<TDocument>(IReadOnlyDictionary<string, string> extendedInfoMap)
            where TDocument : class
        {
            var filters = extendedInfoMap.Select(x =>
            {
                var key = Tuple.Create(typeof(TDocument), x.Key);

                Delegate func;
                if (!_filtersMap.TryGetValue(key, out func))
                {
                    return null;
                }

                var filter = ((Func<string, Func<FilterDescriptor<TDocument>, FilterContainer>>)func)(x.Value);
                return filter;
            })
            .Where(x => x != null)
            .ToList();

            return filters;
        }

        public bool ContainsExtendedInfo<TDocument>(IReadOnlyDictionary<string, string> extendedInfoMap)
        {
            var documentType = typeof(TDocument);
            var registeredExtendedInfos = _filtersMap.Where(x => x.Key.Item1 == documentType).Select(x => x.Key.Item2).ToList();

            // исключение для filterToParent, потом зарефакторится
            return extendedInfoMap.Keys.Where(x => !string.Equals(x, "filterToParent", StringComparison.OrdinalIgnoreCase)).All(registeredExtendedInfos.Contains);
        }
    }
}