using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel
{
    public class FirmReadModel : IFirmReadModel
    {
        private const long TelesaleCategoryGroupId = 1;
        private const long DefaultCategoryRate = 1;

        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public FirmReadModel(IFinder finder, ISecureFinder secureFinder, ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public long GetOrderFirmId(long orderId)
        {
            return _secureFinder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.FirmId).Single();
        }

        public IReadOnlyDictionary<long, FirmWithAddressesAndProjectDto> GetFirmInfosByIds(IEnumerable<long> ids)
        {
            var firms = _secureFinder.Find(FirmSpecs.Firms.Select.FirmWithAddressesAndProject(), Specs.Find.ByIds<Firm>(ids))
                                .ToDictionary(dto => dto.Id, dto => dto);

            var users = _finder.Find(Specs.Find.ByIds<User>(firms.Values.Select(f => f.OwnerCode)))
                               .ToDictionary(user => user.Id, user => user.DisplayName);

            foreach (var dto in firms.Values)
            {
                dto.Owner = users[dto.OwnerCode];
            }

            return firms;
        }

        public IEnumerable<long> GetFirmNonArchivedOrderIds(long firmId)
        {
            return _secureFinder.Find(OrderSpecs.Orders.Find.ActiveOrdersForFirm(firmId)).Select(x => x.Id).ToArray();
        }

        public long GetOrgUnitId(long firmId)
        {
            return _secureFinder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();
        }

        public bool HasFirmClient(long firmId)
        {
            return _secureFinder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.ClientId != null).Single();
        }

        public bool IsTelesaleFirmAddress(long firmAddressId)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                            .Select(address => address.Firm.Territory.OrganizationUnitId)
                                            .SingleOrDefault();

            var categoryIds = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                     .SelectMany(address => address.Firm.FirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                     .SelectMany(address => address.CategoryFirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                     .Select(categoryFirmAddress => categoryFirmAddress.CategoryId)
                                     .Distinct()
                                     .ToArray();

            var mostExpensiveGroupId = _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>() &&
                                                    new FindSpecification<CategoryOrganizationUnit>(
                                                        link => link.OrganizationUnitId == organizationUnitId &&
                                                                categoryIds.Contains(link.CategoryId)))
                // ReSharper disable once ConstantNullCoalescingCondition
                                              .OrderByDescending(x => (decimal?)x.CategoryGroup.GroupRate ?? DefaultCategoryRate)
                                              .Select(x => x.CategoryGroupId)
                                              .FirstOrDefault();

            return mostExpensiveGroupId == TelesaleCategoryGroupId;
        }

        public bool TryGetFirmAndClientByFirmAddress(long firmAddressCode, out FirmAndClientDto dto)
        {
            dto = null;
            var tmp = _secureFinder.Find(Specs.Find.ById<FirmAddress>(firmAddressCode) && Specs.Find.NotDeleted<FirmAddress>())
                                   .Where(x => !x.Firm.IsDeleted)
                                   .Select(x => new
                                   {
                                       x.Firm,
                                       x.Firm.ClientId
                                   })
                                   .FirstOrDefault();

            if (tmp == null)
            {
                return false;
            }

            dto = new FirmAndClientDto
            {
                Firm = tmp.Firm,
                Client = tmp.ClientId.HasValue ? _secureFinder.FindOne(Specs.Find.ById<Client>(tmp.ClientId.Value)) : null
            };
            return true;
        }

        public IEnumerable<FirmAddress> GetFirmAddressesByFirm(long firmId)
        {
            return _finder.FindMany(FirmSpecs.Addresses.Find.ByFirmId(firmId)
                                    && Specs.Find.ActiveAndNotDeleted<FirmAddress>()).ToArray();
        }

        public IEnumerable<FirmAddress> GetActiveOrWithSalesByFirm(long firmId)
        {
            return _finder.FindMany(FirmSpecs.Addresses.Find.ByFirmId(firmId) &&
                                    (Specs.Find.ActiveAndNotDeleted<FirmAddress>() || FirmSpecs.Addresses.Find.WithSales())).ToArray();
        }

        public IEnumerable<FirmContact> GetContacts(long firmAddressId)
        {
            // В данном случае намеренно используется небезопасная версия файндера
            var depCardsQuery = _finder.Find<DepCard>(x => !x.IsHiddenOrArchived);

            // В данном случае намеренно используется небезопасная версия файндера
            var cardRelationsQuery = _finder.Find<CardRelation>(cardRelation => cardRelation.PosCardCode == firmAddressId && !cardRelation.IsDeleted);

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

            var firmAddressContacts = _secureFinder.FindAll<FirmContact>()
                                                   .Where(contact => contact.FirmAddressId == firmAddressId)
                                                   .OrderBy(contact => contact.SortingPosition)
                                                   .AsEnumerable();

            return firmAddressContacts.Union(depCardContacts).ToArray();
        }

        public IDictionary<long, IEnumerable<FirmContact>> GetFirmContactsByAddresses(long firmId)
        {
            var firmAddresses = _secureFinder.Find(Specs.Find.ById<Firm>(firmId))
                                             .SelectMany(firm => firm.FirmAddresses)
                                             .Select(address => address.Id)
                                             .ToArray();

            return firmAddresses.ToDictionary(id => id, GetContacts);
        }

        public string GetFirmName(long firmId)
        {
            return _secureFinder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.Name).Single();
        }

        public string GetTerritoryName(long territoryId)
        {
            return _secureFinder.Find(Specs.Find.ById<Territory>(territoryId)).Select(x => x.Name).Single();
        }

        public bool DoesFirmBelongToClient(long firmId, long clientId)
        {
            return _finder.Find(Specs.Find.ById<Firm>(firmId) && Specs.Find.ActiveAndNotDeleted<Firm>())
                                  .Any(x => x.ClientId == clientId);
        }

        public Firm GetFirm(long firmId)
        {
            return _secureFinder.FindOne(Specs.Find.ById<Firm>(firmId));
        }

        public FirmAddress GetFirmAddress(long firmAddressId)
        {
            return _secureFinder.FindOne(Specs.Find.ById<FirmAddress>(firmAddressId));
        }

        public CategoryFirmAddress GetCategoryFirmAddress(long categoryFirmAddressId)
        {
            return _secureFinder.FindOne(Specs.Find.ById<CategoryFirmAddress>(categoryFirmAddressId));
        }

        public IEnumerable<CategoryFirmAddress> GetCategoryFirmAddressesByFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _secureFinder.FindMany(FirmSpecs.CategoryFirmAddresses.Find.ByAddresses(firmAddressIds));
        }

        public IEnumerable<FirmContact> GetFirmContactsByFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _secureFinder.FindMany(FirmSpecs.FirmContacts.Find.ByFirmAddresses(firmAddressIds));
        }

        public IEnumerable<FirmContact> GetFirmContactsByDepCards(IEnumerable<long> depCardIds)
        {
            return _secureFinder.FindMany(FirmSpecs.FirmContacts.Find.ByDepCards(depCardIds));
        }

        public Dictionary<long, DepCard> GetDepCards(IEnumerable<long> depCardIds)
        {
            return _finder.FindMany(Specs.Find.ByIds<DepCard>(depCardIds)).ToDictionary(x => x.Id);
        }

        public Dictionary<long, FirmAddress> GetFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _finder.FindMany(Specs.Find.ByIds<FirmAddress>(firmAddressIds)).ToDictionary(x => x.Id);
        }

        public Dictionary<long, Firm> GetFirms(IEnumerable<long> firmIds)
        {
            return _finder.FindMany(Specs.Find.ByIds<Firm>(firmIds)).ToDictionary(x => x.Id);
        }

        public IReadOnlyDictionary<int, RegionalTerritoryDto> GetRegionalTerritoriesByBranchCodes(IEnumerable<int> branchCodes, string regionalTerritoryPhrase)
        {
            var territories = _finder.Find(OrganizationUnitSpecs.Find.ByDgppIds(branchCodes) && Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
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
            return _secureFinder.FindOne(Specs.Find.ById<FirmContact>(firmContactId));
        }

        public Dictionary<long, string> GetCityPhoneZones(IEnumerable<long> phoneZoneIds)
        {
            return _finder
                .Find(Specs.Find.ByIds<CityPhoneZone>(phoneZoneIds))
                .Select(x => new { x.Id, x.Name })
                .ToDictionary(x => x.Id, x => x.Name);
        }

        public Dictionary<int, string> GetPhoneFormats(IEnumerable<int> phoneFormatCodes)
        {
            return GetReferenceItems(phoneFormatCodes, "PhoneFormat");
        }

        public Dictionary<int, string> GetPaymentMethods(IEnumerable<int> paymentMethodCodes)
        {
            return GetReferenceItems(paymentMethodCodes, "PaymentMethod");
        }

        public HotClientRequest GetHotClientRequest(long hotClientRequestId)
        {
            return _finder.FindOne(Specs.Find.ById<HotClientRequest>(hotClientRequestId));
        }

        public IReadOnlyDictionary<long, long> GetFirmTerritories(IEnumerable<long> firmIds, string regionalTerritoryWord)
        {
            // Предполагается, что в FirmAddresses уже проставлен TerritoryId из соответствующего Building
            // Возможно стоит вытаскивать все адреса, а сортировать уже в памяти
            var firmTerritories = _finder.Find(Specs.Find.ByIds<Firm>(firmIds))
                                         .Select(x => new
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
                                         })
                                         .ToArray();

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
            return _finder.Find(Specs.Find.ByIds<CardRelation>(cardRelationIds)).ToDictionary(x => x.Id);
        }

        public bool IsFirmInReserve(long firmId)
        {
            var firmOwner = _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(firm => firm.OwnerCode).Single();
            return firmOwner == _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
        }

        public IEnumerable<string> GetAddressesNamesWhichNotBelongToFirm(long firmId, IEnumerable<long> firmAddressIds)
        {
            return _finder.Find(Specs.Find.ByIds<FirmAddress>(firmAddressIds) && FirmSpecs.Addresses.Find.NotBelongToFirm(firmId))
                          .Select(x => x.Address)
                          .ToArray();
        }

        private Dictionary<int, string> GetReferenceItems(IEnumerable<int> referenceItemCodes, string referenceCode)
        {
            return _finder
                .Find(Specs.Find.Custom<ReferenceItem>(x => referenceItemCodes.Contains(x.Code) && x.Reference.CodeName == referenceCode))
                .Select(x => new { x.Code, x.Name })
                .ToDictionary(x => x.Code, x => x.Name);
        }

        private IReadOnlyDictionary<long, long> GetRegionalTerritoriesByOrganizationUnits(IEnumerable<long> organizationUnits, string regionalTerritoryWord)
        {
            return _finder.Find(FirmSpecs.Territories.Find.TerritoriesFromOrganizationUnits(organizationUnits) &&
                                FirmSpecs.Territories.Find.RegionalTerritories(regionalTerritoryWord))
                          .GroupBy(x => x.OrganizationUnitId, (orgUnitId, territories) => territories.OrderByDescending(x => x.Id).FirstOrDefault())
                          .ToDictionary(x => x.OrganizationUnitId, x => x.Id);
        }
    }
}