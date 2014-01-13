using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class AssignedEntityProvider : IDependentEntityProvider
    {
        private readonly IFinder _finder;
        private readonly Dictionary<EntityName, IEnumerable<long>> _entityNamesToIdsMap = new Dictionary<EntityName, IEnumerable<long>>();

        public AssignedEntityProvider(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<EntityName> GetDependentEntityNames(EntityName entityName)
        {
            return new[] { EntityName.LegalPerson, EntityName.Account, EntityName.Client, EntityName.Firm, EntityName.Deal, EntityName.Order, EntityName.AdvertisementElement };
        }

        public IEnumerable<IEntityKey> GetDependentEntities(EntityName parentEntityName, EntityName targetEntityName, long parentId)
        {
            return GetDependentEntities(parentEntityName, targetEntityName, parentId, false);
        }

        public IEnumerable<IEntityKey> GetDependentEntities(EntityName parentEntityName, EntityName targetEntityName, long parentId, bool forceCaching)
        {
            switch (targetEntityName)
            {
                case EntityName.LegalPerson:
                    return GetDependentLegalPersons(parentEntityName, parentId, forceCaching);

                case EntityName.Account:
                    return GetDependentAccounts(parentEntityName, parentId, forceCaching);

                case EntityName.Client:
                    return GetDependentClients(parentEntityName, parentId, forceCaching);

                case EntityName.Firm:
                    return GetDependentFirms(parentEntityName, parentId, forceCaching);

                case EntityName.Order:
                    return GetDependentOrders(parentEntityName, parentId, forceCaching);

                case EntityName.Deal:
                    return GetDependentDeals(parentEntityName, parentId, forceCaching);

                case EntityName.AdvertisementElement:
                    return GetDependentAdvertisementElements(parentEntityName, parentId, forceCaching);

                case EntityName.Advertisement:
                    return GetDependentAdvertisements(parentEntityName, parentId, forceCaching);

                default:
                    return new IEntityKey[0];
            }
        }

        private IEnumerable<Advertisement> GetDependentAdvertisements(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.AdvertisementElement:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Advertisement>(parentId)).Single() };
                default:
                    return Enumerable.Empty<Advertisement>();
            }
        }

        private IEnumerable<AdvertisementElement> GetDependentAdvertisementElements(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.AdvertisementElement:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<AdvertisementElement>(parentId)).Single() };
                default:
                    return Enumerable.Empty<AdvertisementElement>();
            }
        }

        private IEnumerable<Deal> GetDependentDeals(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.Deal:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Deal>(parentId)).Single() };

                case EntityName.Client:
                    {
                        IEnumerable<long> dealIds;
                        if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityName.Deal, out dealIds))
                        {
                            return _finder.Find<Deal>(x => dealIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Client>(parentId)).SelectMany(x => x.Firms.SelectMany(y => y.Deals)).ToArray();

                        if (forceCaching)
                        {
                            _entityNamesToIdsMap.Add(EntityName.Deal, result.Select(x => x.Id).ToArray());
                        }

                        return result;
                    }
                case EntityName.User:
                    {
                        IEnumerable<long> dealIds;
                        if (_entityNamesToIdsMap.TryGetValue(EntityName.Deal, out dealIds))
                        {
                            return _finder.Find<Deal>(x => dealIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find<Deal>(x => x.OwnerCode == parentId).ToArray();

                        _entityNamesToIdsMap.Add(EntityName.Deal, result.Select(x => x.Id).ToArray());

                        return result;
                    }
                default:
                    return new Deal[0];
            }
        }

        private IEnumerable<Order> GetDependentOrders(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.Order:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Order>(parentId)).Single() };

                case EntityName.Client:
                    {
                        IEnumerable<long> orderIds;

                        if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityName.Order, out orderIds))
                        {
                            return _finder.Find<Order>(x => orderIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Client>(parentId)).SelectMany(x => x.Firms.SelectMany(y => y.Orders)).ToArray();

                        if (forceCaching)
                        {
                            _entityNamesToIdsMap.Add(EntityName.Order, result.Select(x => x.Id).ToArray());
                        }

                        return result;
                    }

                case EntityName.User:
                    {
                        IEnumerable<long> orderIds;

                        if (_entityNamesToIdsMap.TryGetValue(EntityName.Order, out orderIds))
                        {
                            return _finder.Find<Order>(x => orderIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find<Order>(x => x.OwnerCode == parentId).ToArray();

                        _entityNamesToIdsMap.Add(EntityName.Order, result.Select(x => x.Id).ToArray());

                        return result;
                    }
                case EntityName.Deal:
                    return _finder.Find<Order>(x => x.DealId == parentId).ToArray();

                default:
                    return new Order[0];
            }
        }

        private IEnumerable<Firm> GetDependentFirms(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.Firm:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Firm>(parentId)).Single() };

                case EntityName.Client:
                    {
                        IEnumerable<long> firmIds;
                        if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityName.Firm, out firmIds))
                        {
                            return _finder.Find<Firm>(x => firmIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Client>(parentId)).SelectMany(x => x.Firms).ToArray();

                        if (forceCaching)
                        {
                            _entityNamesToIdsMap.Add(EntityName.Firm, result.Select(x => x.Id).ToArray());
                        }

                        return result;
                    }
                case EntityName.User:
                    {
                        IEnumerable<long> firmIds;
                        if (_entityNamesToIdsMap.TryGetValue(EntityName.Firm, out firmIds))
                        {
                            return _finder.Find<Firm>(x => firmIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find<Firm>(x => x.OwnerCode == parentId).ToArray();

                        _entityNamesToIdsMap.Add(EntityName.Firm, result.Select(x => x.Id).ToArray());

                        return result;
                    }

                default:
                    return new Firm[0];
            }
        }

        private IEnumerable<LegalPerson> GetDependentLegalPersons(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.LegalPerson:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<LegalPerson>(parentId)).Single() };

                default:
                    return new LegalPerson[0];
            }
        }

        private IEnumerable<Client> GetDependentClients(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.Client:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Client>(parentId)).Single() };

                case EntityName.User:
                    IEnumerable<long> clientIds;
                    if (_entityNamesToIdsMap.TryGetValue(EntityName.Client, out clientIds))
                    {
                        return _finder.Find<Client>(x => clientIds.Contains(x.Id)).ToArray();
                    }

                    var result = _finder.Find<Client>(x => x.OwnerCode == parentId).ToArray();

                    _entityNamesToIdsMap.Add(EntityName.Client, result.Select(x => x.Id).ToArray());

                    return result;

                default:
                    return new Client[0];
            }
        }

        private IEnumerable<Account> GetDependentAccounts(EntityName parentEntityName, long parentId, bool forceCaching)
        {
            switch (parentEntityName)
            {
                case EntityName.Account:
                    return new[] { _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Account>(parentId)).Single() };

                case EntityName.Client:
                    {
                        IEnumerable<long> accountIds;
                        if (forceCaching && _entityNamesToIdsMap.TryGetValue(EntityName.Account, out accountIds))
                        {
                            return _finder.Find<Account>(x => accountIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<Client>(parentId)).SelectMany(x => x.LegalPersons.SelectMany(y => y.Accounts)).ToArray();

                        if (forceCaching)
                        {
                            _entityNamesToIdsMap.Add(EntityName.Account, result.Select(x => x.Id).ToArray());
                        }

                        return result;
                    }
                case EntityName.LegalPerson:
                    return _finder.Find(Platform.DAL.Specifications.Specs.Find.ById<LegalPerson>(parentId)).SelectMany(lp => lp.Accounts).ToArray();

                case EntityName.User:
                    {
                        IEnumerable<long> accountIds;
                        if (_entityNamesToIdsMap.TryGetValue(EntityName.Account, out accountIds))
                        {
                            return _finder.Find<Account>(x => accountIds.Contains(x.Id)).ToArray();
                        }

                        var result = _finder.Find<Account>(x => x.OwnerCode == parentId).ToArray();

                        _entityNamesToIdsMap.Add(EntityName.Account, result.Select(x => x.Id).ToArray());

                        return result;
                    }
                default:
                    return new Account[0];
            }
        }
    }
}