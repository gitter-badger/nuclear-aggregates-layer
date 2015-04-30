using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class AssignedEntityProvider : IDependentEntityProvider
    {
        private readonly IFinder _finder;
        private readonly Dictionary<IEntityType, IEnumerable<long>> _entityNamesToIdsMap = new Dictionary<IEntityType, IEnumerable<long>>();

        public AssignedEntityProvider(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<IEntityType> GetDependentEntityNames(IEntityType entityName)
        {
            return new IEntityType[]
                {
                    EntityType.Instance.LegalPerson(),
                    EntityType.Instance.Account(),
                    EntityType.Instance.AccountDetail(),
                    EntityType.Instance.Client(),
                    EntityType.Instance.Firm(),
                    EntityType.Instance.Deal(),
                    EntityType.Instance.Order(),
                    EntityType.Instance.AdvertisementElement(),
                    EntityType.Instance.Bargain()
                };
        }

        public IEnumerable<IEntityKey> GetDependentEntities(IEntityType parentEntityName, IEntityType targetEntityName, long parentId)
        {
            return GetDependentEntities(parentEntityName, targetEntityName, parentId, false);
        }

        public IEnumerable<IEntityKey> GetDependentEntities(IEntityType parentEntityName, IEntityType targetEntityName, long parentId, bool forceCaching)
        {
            if (targetEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return GetDependentLegalPersons(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.Bargain()))
            {
                return GetDependentBargains(parentEntityName, parentId, forceCaching);

            }

            if (targetEntityName.Equals(EntityType.Instance.Account()))
            {
                return GetDependentAccounts(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.AccountDetail()))
            {
                return GetDependentAccountDetails(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.Client()))
            {
                return GetDependentClients(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.Firm()))
            {
                return GetDependentFirms(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.Order()))
            {
                 return GetDependentOrders(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.Deal()))
            {
                return GetDependentDeals(parentEntityName, parentId, forceCaching);
            }

            if (targetEntityName.Equals(EntityType.Instance.AdvertisementElement()))
            {
                return GetDependentAdvertisementElements(parentEntityName, parentId, forceCaching);
            }
            
            if (targetEntityName.Equals(EntityType.Instance.Advertisement()))
            {
                return GetDependentAdvertisements(parentEntityName, parentId, forceCaching);
            }

            return new IEntityKey[0];
        }

        private IEnumerable<Advertisement> GetDependentAdvertisements(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.AdvertisementElement()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Advertisement>(parentId)) };
            }

            return Enumerable.Empty<Advertisement>();
        }

        private IEnumerable<AdvertisementElement> GetDependentAdvertisementElements(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.AdvertisementElement()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<AdvertisementElement>(parentId)) };
            }

            return Enumerable.Empty<AdvertisementElement>();
        }

        private IEnumerable<Deal> GetDependentDeals(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.Deal()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Deal>(parentId)) };
            }
            
            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                IEnumerable<long> dealIds;
                if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityType.Instance.Deal(), out dealIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Deal>(dealIds));
                }

                var result = _finder.Find(Specs.Find.ById<Client>(parentId)).SelectMany(x => x.Firms.SelectMany(y => y.Deals)).ToArray();

                if (forceCaching)
                {
                    _entityNamesToIdsMap.Add(EntityType.Instance.Deal(), result.Select(x => x.Id).ToArray());
                }

                return result;
            }

            if (parentEntityName.Equals(EntityType.Instance.User()))
            {
                IEnumerable<long> dealIds;
                if (_entityNamesToIdsMap.TryGetValue(EntityType.Instance.Deal(), out dealIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Deal>(dealIds));
                }

                var result = _finder.FindMany(Specs.Find.Owned<Deal>(parentId));

                _entityNamesToIdsMap.Add(EntityType.Instance.Deal(), result.Select(x => x.Id).ToArray());

                return result;
            }

            return Enumerable.Empty<Deal>();
        }

        private IEnumerable<Order> GetDependentOrders(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.Order()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Order>(parentId)) };
            }

            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                IEnumerable<long> orderIds;

                if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityType.Instance.Order(), out orderIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Order>(orderIds));
                }

                var result = _finder.Find(Specs.Find.ById<Client>(parentId)).SelectMany(x => x.Firms.SelectMany(y => y.Orders)).ToArray();

                if (forceCaching)
                {
                    _entityNamesToIdsMap.Add(EntityType.Instance.Order(), result.Select(x => x.Id).ToArray());
                }

                return result;
            }

            if (parentEntityName.Equals(EntityType.Instance.User()))
            {
                IEnumerable<long> orderIds;

                if (_entityNamesToIdsMap.TryGetValue(EntityType.Instance.Order(), out orderIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Order>(orderIds));
                }

                var result = _finder.FindMany(Specs.Find.Owned<Order>(parentId));

                _entityNamesToIdsMap.Add(EntityType.Instance.Order(), result.Select(x => x.Id).ToArray());

                return result;
            }

            if (parentEntityName.Equals(EntityType.Instance.Deal()))
            {
                return _finder.FindMany(OrderSpecs.Orders.Find.ForDeal(parentId)).ToArray();
            }

            return Enumerable.Empty<Order>();
        }

        private IEnumerable<Firm> GetDependentFirms(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.Firm()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Firm>(parentId)) };
            }

            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                IEnumerable<long> firmIds;
                if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityType.Instance.Firm(), out firmIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Firm>(firmIds));
                }

                var result = _finder.Find(Specs.Find.ById<Client>(parentId)).SelectMany(x => x.Firms).ToArray();

                if (forceCaching)
                {
                    _entityNamesToIdsMap.Add(EntityType.Instance.Firm(), result.Select(x => x.Id).ToArray());
                }

                return result;
            }

            if (parentEntityName.Equals(EntityType.Instance.User()))
            {
                IEnumerable<long> firmIds;
                if (_entityNamesToIdsMap.TryGetValue(EntityType.Instance.Firm(), out firmIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Firm>(firmIds));
                }

                var result = _finder.FindMany(Specs.Find.Owned<Firm>(parentId));

                _entityNamesToIdsMap.Add(EntityType.Instance.Firm(), result.Select(x => x.Id).ToArray());

                return result;
            }

            return Enumerable.Empty<Firm>();
        }

        private IEnumerable<LegalPerson> GetDependentLegalPersons(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<LegalPerson>(parentId)) };
            }

            return Enumerable.Empty<LegalPerson>();
        }

        private IEnumerable<Bargain> GetDependentBargains(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.Bargain()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Bargain>(parentId)) };
            }

            return Enumerable.Empty<Bargain>();
        }

        private IEnumerable<Client> GetDependentClients(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Client>(parentId)) };
            }

            if (parentEntityName.Equals(EntityType.Instance.User()))
            {
                IEnumerable<long> clientIds;
                if (_entityNamesToIdsMap.TryGetValue(EntityType.Instance.Client(), out clientIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Client>(clientIds));
                }

                var result = _finder.FindMany(Specs.Find.Owned<Client>(parentId));

                _entityNamesToIdsMap.Add(EntityType.Instance.Client(), result.Select(x => x.Id).ToArray());

                return result;
            }

            return Enumerable.Empty<Client>();
        }

        private IEnumerable<Account> GetDependentAccounts(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.Account()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<Account>(parentId)) };
            }

            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                IEnumerable<long> accountIds;
                if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityType.Instance.Account(), out accountIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Account>(accountIds));
                }

                var result = _finder.Find(Specs.Find.ById<Client>(parentId)).SelectMany(x => x.LegalPersons.SelectMany(y => y.Accounts)).ToArray();

                if (forceCaching)
                {
                    _entityNamesToIdsMap.Add(EntityType.Instance.Account(), result.Select(x => x.Id).ToArray());
                }

                return result;
            }

            if (parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return _finder.FindMany(Specs.Find.ById<LegalPerson>(parentId)).SelectMany(lp => lp.Accounts).ToArray();
            }

            if (parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                IEnumerable<long> accountIds;
                if (_entityNamesToIdsMap.TryGetValue(EntityType.Instance.Account(), out accountIds))
                {
                    return _finder.FindMany(Specs.Find.ByIds<Account>(accountIds));
                }

                var result = _finder.FindMany(Specs.Find.Owned<Account>(parentId));

                _entityNamesToIdsMap.Add(EntityType.Instance.Account(), result.Select(x => x.Id).ToArray());

                return result;
            }

            return Enumerable.Empty<Account>();
        }

        private IEnumerable<AccountDetail> GetDependentAccountDetails(IEntityType parentEntityName, long parentId, bool forceCaching)
        {
            if (parentEntityName.Equals(EntityType.Instance.AccountDetail()))
            {
                return new[] { _finder.FindOne(Specs.Find.ById<AccountDetail>(parentId)) };
            }

            return Enumerable.Empty<AccountDetail>();
        }
    }
}