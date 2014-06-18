using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel
{
    public class FirmReadModel : IFirmReadModel
    {
        private readonly IFinder _unsecureFinder;
        private readonly ISecureFinder _secureFinder;
        
        public FirmReadModel(IFinder unsecureFinder, ISecureFinder secureFinder)
        {
            _unsecureFinder = unsecureFinder;
            _secureFinder = secureFinder;
        }

        public long GetOrderFirmId(long orderId)
        {
            return _secureFinder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.FirmId).Single();
        }

        public IReadOnlyDictionary<Guid, FirmWithAddressesAndProjectDto> GetFirmInfosByCrmIds(IEnumerable<Guid> crmIds)
        {
            return _secureFinder.Find(FirmSpecs.Firms.Find.ByReplicationCodes(crmIds))
                          .Select(f => new
                              {
                                  CrmId = f.ReplicationCode,
                                  Dto = new FirmWithAddressesAndProjectDto
                                      {
                                          Id = f.Id,
                                          Name = f.Name,
                                          FirmAddresses = f.FirmAddresses
                                                           .Where(fa => fa.IsActive && !fa.IsDeleted && !fa.ClosedForAscertainment)
                                                           .Select(fa => new FirmAddressWithCategoriesDto
                                                               {
                                                                   Id = fa.Id,
                                                                   Address = fa.Address,
                                                                   Categories = fa.CategoryFirmAddresses
                                                                                  .Where(cfa => cfa.IsActive && !cfa.IsDeleted)
                                                                                  .Select(cfa => new CategoryDto
                                                                                      {
                                                                                          Id = cfa.CategoryId,
                                                                                          Name = cfa.Category.Name
                                                                                      })
                                                               }),
                                          Project = f.OrganizationUnit.Projects
                                                     .Where(p => p.IsActive)
                                                     .Select(p => new ProjectDto
                                                         {
                                                             Code = p.Id,
                                                             Name = p.DisplayName
                                                         }).FirstOrDefault()

                                      }
                              })
                          .ToDictionary(x => x.CrmId, x => x.Dto);
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

        public IEnumerable<CategoryGroup> GetFirmAddressCategoryGroups(long firmAddressId)
        {
            var organizationUnitId = _secureFinder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                            .Select(address => address.Firm.Territory.OrganizationUnitId)
                                            .SingleOrDefault();

            var categoryIds = _secureFinder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                     .SelectMany(address => address.Firm.FirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                     .SelectMany(address => address.CategoryFirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                     .Select(categoryFirmAddress => categoryFirmAddress.CategoryId)
                                     .Distinct()
                                     .ToArray();

            var groups = _secureFinder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                .Where(link => link.OrganizationUnitId == organizationUnitId && categoryIds.Contains(link.CategoryId))
                                .Select(link => link.CategoryGroup)
                                .Distinct()
                                .ToArray();

            return groups;
        }

        public FirmAndClientDto GetFirmAndClientByFirmAddress(long firmAddressCode)
        {
            var tmp = _secureFinder.Find(Specs.Find.ById<FirmAddress>(firmAddressCode) && Specs.Find.NotDeleted<FirmAddress>())
                             .Select(x => new
                                 {
                                     Firm = x.Firm != null && !x.Firm.IsDeleted ? x.Firm : (Firm)null,
                                     ClientId = x.Firm != null && !x.Firm.IsDeleted ? (!x.Firm.Client.IsDeleted ? x.Firm.Client.Id : (long?)null) : (long?)null
                                 })
                             .FirstOrDefault();
            return tmp == null
                       ? null
                       : new FirmAndClientDto
                           {
                               Firm = tmp.Firm,
                               Client = tmp.ClientId.HasValue ? _secureFinder.FindOne(Specs.Find.ById<Client>(tmp.ClientId.Value)) : null
                           };
        }

        public IEnumerable<FirmAddress> GetFirmAddressesByFirm(long firmId)
        {
            return _unsecureFinder.FindMany(FirmSpecs.Addresses.Find.ActiveAddresses(firmId)).ToArray();
        }

        public IEnumerable<FirmContact> GetContacts(long firmAddressId)
        {
            // В данном случае намеренно используется небезопасная версия файндера
            var depCardsQuery = _unsecureFinder.Find<DepCard>(x => !x.IsHiddenOrArchived);

            // В данном случае намеренно используется небезопасная версия файндера
            var cardRelations = _unsecureFinder.FindAll<CardRelation>()
                                               .Where(cardRelation => cardRelation.PosCardCode == firmAddressId && !cardRelation.IsDeleted)
                                               .OrderBy(cardRelation => cardRelation.OrderNo)
                                               .Join(depCardsQuery,
                                                     cardRelation => cardRelation.DepCardCode,
                                                     depCard => depCard.Id,
                                                     (cardRelation, depCard) => depCard)
                                               .SelectMany(depCard => depCard.FirmContacts)
                                               .OrderBy(contact => contact.SortingPosition)
                                               .ToArray();

            var firmAddressContacts = _secureFinder.FindAll<FirmContact>()
                                             .Where(contact => contact.FirmAddressId == firmAddressId)
                                             .OrderBy(contact => contact.SortingPosition)
                                             .ToArray();

            return firmAddressContacts.Union(cardRelations).ToArray();
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
            return _unsecureFinder.Find(Specs.Find.ById<Firm>(firmId) && Specs.Find.ActiveAndNotDeleted<Firm>())
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
            return _unsecureFinder.FindMany(Specs.Find.ByIds<DepCard>(depCardIds)).ToDictionary(x => x.Id);
        }

        public Dictionary<long, FirmAddress> GetFirmAddresses(IEnumerable<long> firmAddressIds)
        {
            return _unsecureFinder.FindMany(Specs.Find.ByIds<FirmAddress>(firmAddressIds)).ToDictionary(x => x.Id);
        }

        public Dictionary<long, Firm> GetFirms(IEnumerable<long> firmIds)
        {
            return _unsecureFinder.FindMany(Specs.Find.ByIds<Firm>(firmIds)).ToDictionary(x => x.Id);
        }

        public IEnumerable<RegionalTerritoryDto> GetRegionalTerritoriesByBranchCodes(IEnumerable<int> branchCodes, string regionalTerritoryPhrase)
        {
            return _unsecureFinder.Find(OrganizationUnitSpecs.Find.ByDgppIds(branchCodes) && Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                                  .SelectMany(x =>
                                              x.Territories.Where(t => t.IsActive && t.Name.Contains(regionalTerritoryPhrase)))
                                  .Select(
                                      x =>
                                      new RegionalTerritoryDto
                                          {
                                              TerritoryId = x.Id,
                                              OrganizationUnitId = x.OrganizationUnitId,
                                              OrganizationUnitDgppId = x.OrganizationUnit.DgppId.Value
                                          })
                                  .ToArray();
        }

        public FirmContact GetFirmContact(long firmContactId)
        {
            return _secureFinder.FindOne(Specs.Find.ById<FirmContact>(firmContactId));
        }

        public Dictionary<long, string> GetCityPhoneZones(IEnumerable<long> phoneZoneIds)
        {
            return _unsecureFinder
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

        private Dictionary<int, string> GetReferenceItems(IEnumerable<int> referenceItemCodes, string referenceCode)
        {
            return _unsecureFinder
                .Find(Specs.Find.Custom<ReferenceItem>(x => referenceItemCodes.Contains(x.Code) && x.Reference.CodeName == referenceCode))
                .Select(x => new { x.Code, x.Name })
                .ToDictionary(x => x.Code, x => x.Name);
        }
    }
}