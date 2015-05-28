using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel
{
    public class FirmReadModel : IFirmReadModel
    {
        private const long TelesaleCategoryGroupId = 5;
        private const long DefaultCategoryRate = 1;

        private readonly IFinder _finder;
        private readonly ISecureQuery _secureQuery;
        private readonly ISecureFinder _secureFinder;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public FirmReadModel(IFinder finder, ISecureQuery secureQuery, ISecureFinder secureFinder, ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _finder = finder;
            _secureQuery = secureQuery;
            _secureFinder = secureFinder;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public long GetOrderFirmId(long orderId)
        {
            return _secureFinder.FindObsolete(Specs.Find.ById<Order>(orderId)).Select(x => x.FirmId).Single();
        }

        public IReadOnlyDictionary<long, FirmWithAddressesAndProjectDto> GetFirmInfosByIds(IEnumerable<long> ids)
        {
            return _secureFinder.Find(Specs.Find.ByIds<Firm>(ids))
                                .Map(q => q.Select(FirmSpecs.Firms.Select.FirmWithAddressesAndProject()))
                                .Map(dto => dto.Id, dto => dto);
        }

        public IEnumerable<long> GetFirmNonArchivedOrderIds(long firmId)
        {
            return _secureFinder.Find(OrderSpecs.Orders.Find.ActiveOrdersForFirm(firmId)).Map(q => q.Select(x => x.Id)).Many();
        }

        public long GetOrgUnitId(long firmId)
        {
            return _secureFinder.FindObsolete(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();
        }

        public bool HasFirmClient(long firmId)
        {
            return _secureFinder.FindObsolete(Specs.Find.ById<Firm>(firmId)).Select(x => x.ClientId != null).Single();
        }

        public bool IsTelesaleFirmAddress(long firmAddressId)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                            .Map(q => q.Select(address => address.Firm.Territory.OrganizationUnitId))
                                            .One();

            var categoryIds = _finder.FindObsolete(Specs.Find.ById<FirmAddress>(firmAddressId))
                                     .SelectMany(address => address.Firm.FirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                     .SelectMany(address => address.CategoryFirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                     .Select(categoryFirmAddress => categoryFirmAddress.CategoryId)
                                     .Distinct()
                                     .ToArray();

            var mostExpensiveGroupId = _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>()
                                                    && CategorySpecs.CategoryOrganizationUnits.Find.ForOrganizationUnit(organizationUnitId)
                                                    && CategorySpecs.CategoryOrganizationUnits.Find.ForCategories(categoryIds))
                                              .Map(q => q.OrderByDescending(x => x.CategoryGroup != null ? x.CategoryGroup.GroupRate : DefaultCategoryRate)
                                                         .Select(x => x.CategoryGroupId))
                                              .Top();

            return mostExpensiveGroupId == TelesaleCategoryGroupId;
        }

        public bool TryGetFirmAndClientByFirmAddress(long firmAddressCode, out FirmAndClientDto dto)
        {
            dto = null;
            var tmp = _secureFinder.Find(Specs.Find.ById<FirmAddress>(firmAddressCode) && Specs.Find.NotDeleted<FirmAddress>() &&
                                         new FindSpecification<FirmAddress>(x => !x.Firm.IsDeleted))
                                   .Map(q => q.Select(x => new
                                       {
                                           x.Firm,
                                           x.Firm.ClientId
                                       }))
                                   .Top();

            if (tmp == null)
            {
                return false;
            }

            dto = new FirmAndClientDto
            {
                Firm = tmp.Firm,
                Client = tmp.ClientId.HasValue ? _secureFinder.Find(Specs.Find.ById<Client>(tmp.ClientId.Value)).One() : null
            };
            return true;
        }

        public IEnumerable<FirmAddress> GetFirmAddressesByFirm(long firmId)
        {
            return _finder.Find(FirmSpecs.Addresses.Find.ByFirmId(firmId)
                                    && Specs.Find.ActiveAndNotDeleted<FirmAddress>()).Many();
        }

        public IEnumerable<FirmAddress> GetActiveOrWithSalesByFirm(long firmId)
        {
            return _finder.Find(FirmSpecs.Addresses.Find.ByFirmId(firmId) &&
                                    (Specs.Find.ActiveAndNotDeleted<FirmAddress>() || FirmSpecs.Addresses.Find.WithSales())).Many();
        }

        public IEnumerable<FirmContact> GetContacts(long firmAddressId)
        {
            // В данном случае намеренно используется небезопасная версия файндера
            var depCardsQuery = _finder.FindObsolete(new FindSpecification<DepCard>(x => !x.IsHiddenOrArchived));

            // В данном случае намеренно используется небезопасная версия файндера
            var cardRelationsQuery = _finder.FindObsolete(new FindSpecification<CardRelation>(cardRelation => cardRelation.PosCardCode == firmAddressId && !cardRelation.IsDeleted));

            var depCardContacts = (from cardRelation in cardRelationsQuery
                                   join depCard in depCardsQuery on cardRelation.DepCardCode equals depCard.Id
                                   orderby cardRelation.OrderNo
                                   from firmContact in depCard.FirmContacts
                                   orderby firmContact.SortingPosition
                                   select firmContact)
                .AsEnumerable()

                // COMMENT {all, 25.08.2014}: Восстановление ссылки на адрес фирмы из контакта dep-карточки
                .Select(contact =>
                {
                    contact.FirmAddressId = firmAddressId;
                    return contact;
                });

            var firmAddressContacts = _secureQuery.For<FirmContact>()
                                                  .Where(contact => contact.FirmAddressId == firmAddressId)
                                                  .OrderBy(contact => contact.SortingPosition)
                                                  .AsEnumerable();

            return firmAddressContacts.Union(depCardContacts).ToArray();
        }

        public IDictionary<long, IEnumerable<FirmContact>> GetFirmContactsByAddresses(long firmId)
        {
            var firmAddresses = _secureFinder.Find(Specs.Find.ById<Firm>(firmId))
                                             .Map(q => q.SelectMany(firm => firm.FirmAddresses)
                                                        .Select(address => address.Id))
                                             .Many();

            return firmAddresses.ToDictionary(id => id, GetContacts);
        }

        public string GetFirmName(long firmId)
        {
            return _secureFinder.FindObsolete(Specs.Find.ById<Firm>(firmId)).Select(x => x.Name).Single();
        }

        public string GetTerritoryName(long territoryId)
        {
            return _secureFinder.FindObsolete(Specs.Find.ById<Territory>(territoryId)).Select(x => x.Name).Single();
        }

        public bool DoesFirmBelongToClient(long firmId, long clientId)
        {
            return _finder.Find(Specs.Find.ById<Firm>(firmId) && Specs.Find.ActiveAndNotDeleted<Firm>() &&
                                new FindSpecification<Firm>(x => x.ClientId == clientId))
                          .Any();
        }

        public Firm GetFirm(long firmId)
        {
            return _secureFinder.Find(Specs.Find.ById<Firm>(firmId)).One();
        }

        public FirmAddress GetFirmAddress(long firmAddressId)
        {
            return _secureFinder.Find(Specs.Find.ById<FirmAddress>(firmAddressId)).One();
        }

        public CategoryFirmAddress GetCategoryFirmAddress(long categoryFirmAddressId)
        {
            return _secureFinder.Find(Specs.Find.ById<CategoryFirmAddress>(categoryFirmAddressId)).One();
        }

        public IEnumerable<CategoryFirmAddress> GetCategoryFirmAddressesByFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _secureFinder.Find(FirmSpecs.CategoryFirmAddresses.Find.ByAddresses(firmAddressIds)).Many();
        }

        public IEnumerable<FirmContact> GetFirmContactsByFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _secureFinder.Find(FirmSpecs.FirmContacts.Find.ByFirmAddresses(firmAddressIds)).Many();
        }

        public IEnumerable<FirmContact> GetFirmContactsByDepCards(IEnumerable<long> depCardIds)
        {
            return _secureFinder.Find(FirmSpecs.FirmContacts.Find.ByDepCards(depCardIds)).Many();
        }

        public IReadOnlyDictionary<long, DepCard> GetDepCards(IEnumerable<long> depCardIds)
        {
            return _finder.Find(Specs.Find.ByIds<DepCard>(depCardIds)).Map(x => x.Id, x => x);
        }

        public IReadOnlyDictionary<long, FirmAddress> GetFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _finder.Find(Specs.Find.ByIds<FirmAddress>(firmAddressIds)).Map(x => x.Id, x => x);
        }

        public IReadOnlyDictionary<long, Firm> GetFirms(IEnumerable<long> firmIds)
        {
            return _finder.Find(Specs.Find.ByIds<Firm>(firmIds)).Map(x => x.Id, x => x);
        }

        public IEnumerable<Firm> GetFirmsForClientAndLinkedChild(long clientId)
        {
            var clientAndChild = _finder.Find(ClientSpecs.DenormalizedClientLinks.Find.ClientChild(clientId))
                                        .Map(q => q.Select(s => (long?)s.ChildClientId))
                                        .Many()
                                        .Union(new[] { (long?)clientId });
            return _finder.Find(FirmSpecs.Firms.Find.ByClientIds(clientAndChild)).Many();
        }

        public IReadOnlyDictionary<int, RegionalTerritoryDto> GetRegionalTerritoriesByBranchCodes(IEnumerable<int> branchCodes, string regionalTerritoryPhrase)
        {
            var territories = _finder.FindObsolete(OrganizationUnitSpecs.Find.ByDgppIds(branchCodes) && Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                                     .Select(x => new
                                     {
                                         BranchCode = x.DgppId.Value,
                                         Territory = x.Territories.OrderByDescending(t => t.Id).FirstOrDefault(t => t.IsActive && t.Name.Contains(regionalTerritoryPhrase))
                                     })
                                     .Where(x => x.Territory != null)
                                     .ToDictionary(x => x.BranchCode,
                                                   x => new RegionalTerritoryDto
                                                   {
                                                       TerritoryId = x.Territory.Id,
                                                       OrganizationUnitId = x.Territory.OrganizationUnitId
                                                   });

            var branchesWithNoRegionalTerritory = branchCodes.Except(territories.Keys).ToArray();
            if (branchesWithNoRegionalTerritory.Any())
            {
                throw new IntegrationException(string.Format("Can't find regional territories for the following branch codes: {0}.",
                                                             string.Join(", ", branchesWithNoRegionalTerritory)));
            }

            return territories;
        }

        public FirmContact GetFirmContact(long firmContactId)
        {
            return _secureFinder.Find(Specs.Find.ById<FirmContact>(firmContactId)).One();
        }

        public IReadOnlyDictionary<long, string> GetCityPhoneZones(IEnumerable<long> phoneZoneIds)
        {
            return _finder
                .Find(Specs.Find.ByIds<CityPhoneZone>(phoneZoneIds))
                .Map(q => q.Select(x => new { x.Id, x.Name }))
                .Map(x => x.Id, x => x.Name);
        }

        public IReadOnlyDictionary<int, string> GetPhoneFormats(IEnumerable<int> phoneFormatCodes)
        {
            return GetReferenceItems(phoneFormatCodes, "PhoneFormat");
        }

        public IReadOnlyDictionary<int, string> GetPaymentMethods(IEnumerable<int> paymentMethodCodes)
        {
            return GetReferenceItems(paymentMethodCodes, "PaymentMethod");
        }

        public HotClientRequest GetHotClientRequest(long hotClientRequestId)
        {
            return _finder.Find(Specs.Find.ById<HotClientRequest>(hotClientRequestId)).One();
        }

        public IReadOnlyDictionary<long, long> GetFirmTerritories(IEnumerable<long> firmIds, string regionalTerritoryWord)
        {
            // Предполагается, что в FirmAddresses уже проставлен TerritoryId из соответствующего Building
            // Возможно стоит вытаскивать все адреса, а сортировать уже в памяти
            var firmTerritories = _finder.Find(Specs.Find.ByIds<Firm>(firmIds))
                                         .Map(q => q.Select(x => new
                                         {
                                             FirmId = x.Id,
                                             x.OrganizationUnitId,
                                             TerritoryId = x.FirmAddresses
                                                            .Where(y => y.TerritoryId != null && y.Territory.OrganizationUnitId == x.OrganizationUnitId)
                                                            .OrderBy(y => y.IsDeleted)
                                                            .ThenByDescending(y => y.IsActive)
                                                            .ThenBy(y => y.ClosedForAscertainment)
                                                            .ThenBy(y => y.SortingPosition)
                                                            .Select(y => y.TerritoryId)
                                                            .FirstOrDefault()
                                         }))
                                         .Many();

            var firmWithTerritory = firmTerritories.Where(x => x.TerritoryId != null).ToArray();
            var firmsWithoutTerritory = firmTerritories.Where(x => x.TerritoryId == null).ToArray();

            var result = firmWithTerritory.ToDictionary(firm => firm.FirmId, firm => firm.TerritoryId.Value);

            if (firmsWithoutTerritory.Any())
            {
                var regionalTerritories = GetRegionalTerritoriesByOrganizationUnits(firmsWithoutTerritory.Select(x => x.OrganizationUnitId),
                                                                                    regionalTerritoryWord);
                foreach (var firm in firmsWithoutTerritory)
                {
                    result.Add(firm.FirmId, regionalTerritories[firm.OrganizationUnitId]);
                }
            }

            return result;
        }

        public IReadOnlyDictionary<long, CardRelation> GetCardRelationsByIds(IEnumerable<long> cardRelationIds)
        {
            return _finder.Find(Specs.Find.ByIds<CardRelation>(cardRelationIds)).Map(x => x.Id, x => x);
        }

        public bool IsFirmInReserve(long firmId)
        {
            var firmOwner = _finder.FindObsolete(Specs.Find.ById<Firm>(firmId)).Select(firm => firm.OwnerCode).Single();
            return firmOwner == _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }

        public IEnumerable<string> GetAddressesNamesWhichNotBelongToFirm(long firmId, IEnumerable<long> firmAddressIds)
        {
            return _finder.Find(Specs.Find.ByIds<FirmAddress>(firmAddressIds) && FirmSpecs.Addresses.Find.NotBelongToFirm(firmId))
                          .Map(q => q.Select(x => x.Address))
                          .Many();
        }

        public long GetFirmOwnerCodeUnsecure(long firmId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Firm>(firmId)).Select(x => x.OwnerCode).Single();
        }

        private IReadOnlyDictionary<int, string> GetReferenceItems(IEnumerable<int> referenceItemCodes, string referenceCode)
        {
            return _finder
                .Find(Specs.Find.Custom<ReferenceItem>(x => referenceItemCodes.Contains(x.Code) && x.Reference.CodeName == referenceCode))
                .Map(q => q.Select(x => new { x.Code, x.Name }))
                .Map(x => x.Code, x => x.Name);
        }

        private IReadOnlyDictionary<long, long> GetRegionalTerritoriesByOrganizationUnits(IEnumerable<long> organizationUnits, string regionalTerritoryWord)
        {
            return _finder.Find(FirmSpecs.Territories.Find.TerritoriesFromOrganizationUnits(organizationUnits) &&
                                FirmSpecs.Territories.Find.RegionalTerritories(regionalTerritoryWord))
                          .Map(q => q.GroupBy(x => x.OrganizationUnitId,
                                              (orgUnitId, territories) => territories.OrderByDescending(x => x.Id)
                                                                                     .FirstOrDefault()))
                          .Map(x => x.OrganizationUnitId, x => x.Id);
        }
    }
}