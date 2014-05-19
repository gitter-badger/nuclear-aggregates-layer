using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportCardForErmAggregateService : IImportCardForErmAggregateService
    {
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressGenericRepository;
        private readonly IRepository<DepCard> _depCardRepository;
        private readonly IFinder _finder;
        private readonly IRepository<FirmAddress> _firmAddressGenericRepository;
        private readonly IRepository<FirmContact> _firmContactGenericRepository;
        private readonly IRepository<Firm> _firmGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public ImportCardForErmAggregateService(IFinder finder,
                                                IOperationScopeFactory scopeFactory,
                                                IRepository<CategoryFirmAddress> categoryFirmAddressGenericRepository,
                                                IIdentityProvider identityProvider,
                                                IRepository<FirmAddress> firmAddressGenericRepository,
                                                ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                                IRepository<Firm> firmGenericRepository,
                                                IRepository<FirmContact> firmContactGenericRepository,
                                                IRepository<DepCard> depCardRepository)
        {
            _finder = finder;
            _scopeFactory = scopeFactory;
            _categoryFirmAddressGenericRepository = categoryFirmAddressGenericRepository;
            _identityProvider = identityProvider;
            _firmAddressGenericRepository = firmAddressGenericRepository;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _firmGenericRepository = firmGenericRepository;
            _firmContactGenericRepository = firmContactGenericRepository;
            _depCardRepository = depCardRepository;
        }

        public void ImportCategoryFirmAddresses(long firmAddressId, IEnumerable<ImportCategoryFirmAddressDto> categoryFirmAddresses)
        {
            var dtoCategories = categoryFirmAddresses.ToDictionary(x => (long)x.Code);
            var existingCategories = _finder.Find<CategoryFirmAddress>(x => x.FirmAddressId == firmAddressId).ToDictionary(x => x.CategoryId);

            // COMMENT {a.tukaev, 26.11.2013}: cids - имелось ввиду categoryIds? Может так и написать? 
            // DONE {d.ivanov, 04.12.2013}: Сделано
            var categoryIdsToUpdate = dtoCategories.Keys.Intersect(existingCategories.Keys).ToArray();
            var categoryIdsToInsert = dtoCategories.Keys.Except(existingCategories.Keys).ToArray();
            var categoryIdsToDelete = existingCategories.Keys.Except(dtoCategories.Keys).ToArray();

            if (categoryIdsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, CategoryFirmAddress>())
                {
                    foreach (var categoryId in categoryIdsToUpdate)
                    {
                        var dtoCategory = dtoCategories[categoryId];
                        var existingCategory = existingCategories[categoryId];

                        existingCategory.IsPrimary = dtoCategory.IsPrimary;
                        existingCategory.SortingPosition = dtoCategory.SortingPosition;

                        existingCategory.IsActive = true;
                        existingCategory.IsDeleted = false;

                        _categoryFirmAddressGenericRepository.Update(existingCategory);
                        scope.Updated<CategoryFirmAddress>(existingCategory.Id);
                    }

                    scope.Complete();
                }
            }

            if (categoryIdsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, CategoryFirmAddress>())
                {
                    foreach (var categoryId in categoryIdsToInsert)
                    {
                        var dtoCategory = dtoCategories[categoryId];
                        var category = new CategoryFirmAddress
                            {
                                CategoryId = dtoCategory.Code,
                                FirmAddressId = firmAddressId,
                                IsActive = true,
                                IsPrimary = dtoCategory.IsPrimary,
                                SortingPosition = dtoCategory.SortingPosition
                            };

                        _identityProvider.SetFor(category);
                        _categoryFirmAddressGenericRepository.Add(category);
                        scope.Added<CategoryFirmAddress>(category.Id);
                    }

                    scope.Complete();
                }
            }

            if (categoryIdsToDelete.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, CategoryFirmAddress>())
                {
                    foreach (var categoryId in categoryIdsToDelete)
                    {
                        var existingCategory = existingCategories[categoryId];
                        _categoryFirmAddressGenericRepository.Delete(existingCategory);
                        scope.Deleted<CategoryFirmAddress>(existingCategory.Id);
                    }

                    scope.Complete();
                }
            }

            _categoryFirmAddressGenericRepository.Save();
        }

        public void ImportFirmAddresses(IEnumerable<ImportFirmAddressDto> firmAddresses, string regionalTerritoryName)
        {
            var dtoAddresses = firmAddresses.ToDictionary(x => x.Code);

            var ids = dtoAddresses.Keys;
            var existingAddresses = _finder.Find<FirmAddress>(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);

            var idsToUpdate = dtoAddresses.Keys.Intersect(existingAddresses.Keys).ToArray();
            var idsToInsert = dtoAddresses.Keys.Except(existingAddresses.Keys).ToArray();

            // Создаем пустые фирмы, если нужно
            CreateBlankFirms(dtoAddresses.Values, regionalTerritoryName);

            if (idsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
                {
                    foreach (var id in idsToUpdate)
                    {
                        var dtoAddress = dtoAddresses[id];
                        var existingAddress = existingAddresses[id];

                        existingAddress.FirmId = dtoAddress.FirmCode;
                        existingAddress.TerritoryId = dtoAddress.TerritoryCode;
                        existingAddress.IsActive = dtoAddress.IsActive;
                        existingAddress.ClosedForAscertainment = dtoAddress.ClosedForAscertainment;
                        existingAddress.IsLocatedOnTheMap = dtoAddress.IsLinked;
                        existingAddress.IsDeleted = dtoAddress.IsDeleted;
                        existingAddress.Address = dtoAddress.Address;
                        existingAddress.ReferencePoint = null;
                        existingAddress.WorkingTime = dtoAddress.Schedule;
                        existingAddress.PaymentMethods = dtoAddress.Payment;

                        _firmAddressGenericRepository.Update(existingAddress);
                        scope.Updated<FirmAddress>(existingAddress.Id);
                    }

                    scope.Complete();
                }
            }

            if (idsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, FirmAddress>())
                {
                    foreach (var id in idsToInsert)
                    {
                        var dtoAddress = dtoAddresses[id];
                        var address = new FirmAddress
                            {
                                Id = dtoAddress.Code,
                                FirmId = dtoAddress.FirmCode,
                                Address = dtoAddress.Address,
                                WorkingTime = dtoAddress.Schedule,
                                PaymentMethods = dtoAddress.Payment,
                                IsLocatedOnTheMap = dtoAddress.IsLinked,
                                TerritoryId = dtoAddress.TerritoryCode,
                                ClosedForAscertainment = dtoAddress.ClosedForAscertainment,
                                IsActive = dtoAddress.IsActive,
                                IsDeleted = dtoAddress.IsDeleted
                            };

                        _firmAddressGenericRepository.Add(address);
                        scope.Added<FirmAddress>(address.Id);
                    }

                    scope.Complete();
                }
            }

            _firmAddressGenericRepository.Save();
        }

        public void ImportDepCards(IEnumerable<ImportDepCardDto> importDepCardDtos)
        {
            var dtoCards = importDepCardDtos.ToDictionary(x => x.Code);

            var ids = dtoCards.Keys;
            var existingCards = _finder.Find<DepCard>(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);

            var idsToUpdate = dtoCards.Keys.Intersect(existingCards.Keys).ToArray();
            var idsToInsert = dtoCards.Keys.Except(existingCards.Keys).ToArray();

            if (idsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, DepCard>())
                {
                    foreach (var id in idsToUpdate)
                    {
                        var existingCard = existingCards[id];
                        existingCard.IsHiddenOrArchived = dtoCards[id].IsHiddenOrArchived;
                        _depCardRepository.Update(existingCard);
                        scope.Updated<DepCard>(id);
                    }

                    scope.Complete();
                }
            }

            if (idsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, DepCard>())
                {
                    foreach (var id in idsToInsert)
                    {
                        var newCard = new DepCard { Id = id, IsHiddenOrArchived = dtoCards[id].IsHiddenOrArchived };
                        _depCardRepository.Add(newCard);
                        scope.Added<DepCard>(id);
                    }

                    scope.Complete();
                }
            }

            _depCardRepository.Save();
        }

        public void ImportFirmContacts(long firmAddressId, IEnumerable<ImportFirmContactDto> firmContacts, bool isDepCard)
        {
            var dtoContacts = firmContacts.ToDictionary(x => x.SortingPosition);
            var existingContacts = isDepCard
                                       ? _finder.Find<FirmContact>(x => x.CardId == firmAddressId).ToDictionary(x => x.SortingPosition)
                                       : _finder.Find<FirmContact>(x => x.FirmAddressId == firmAddressId).ToDictionary(x => x.SortingPosition);

            // TODO {a.tukaev, 26.11.2013}: Не забывай про StyleCop
            // DONE {d.ivanov, 04.12.2013}: Ок
            var sortingPositionsToUpdate = dtoContacts.Keys.Intersect(existingContacts.Keys).ToArray();
            var sortingPositionsToInsert = dtoContacts.Keys.Except(existingContacts.Keys).ToArray();
            var sortingPositionsToDelete = existingContacts.Keys.Except(dtoContacts.Keys).ToArray();

            if (sortingPositionsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmContact>())
                {
                    foreach (var sortingPosition in sortingPositionsToUpdate)
                    {
                        var dtoContact = dtoContacts[sortingPosition];
                        var existingContact = existingContacts[sortingPosition];

                        existingContact.ContactType = dtoContact.ContactType;
                        existingContact.Contact = dtoContact.Contact;

                        _firmContactGenericRepository.Update(existingContact);
                        scope.Updated<FirmContact>(existingContact.Id);
                    }

                    scope.Complete();
                }
            }

            if (sortingPositionsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, FirmContact>())
                {
                    foreach (var sortingPosition in sortingPositionsToInsert)
                    {
                        var dtoContact = dtoContacts[sortingPosition];
                        var contact = new FirmContact
                            {
                                ContactType = dtoContact.ContactType,
                                Contact = dtoContact.Contact,
                                SortingPosition = dtoContact.SortingPosition
                            };

                        if (isDepCard)
                        {
                            contact.CardId = firmAddressId;
                        }
                        else
                        {
                            contact.FirmAddressId = firmAddressId;
                        }

                        _identityProvider.SetFor(contact);
                        _firmContactGenericRepository.Add(contact);
                        scope.Added<FirmContact>(contact.Id);
                    }

                    scope.Complete();
                }
            }

            if (sortingPositionsToDelete.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, FirmContact>())
                {
                    foreach (var sortingPosition in sortingPositionsToDelete)
                    {
                        var existingContact = existingContacts[sortingPosition];
                        _firmContactGenericRepository.Delete(existingContact);
                        scope.Deleted<FirmContact>(existingContact.Id);
                    }

                    scope.Complete();
                }
            }

            _firmContactGenericRepository.Save();
        }

        private void CreateBlankFirms(IEnumerable<ImportFirmAddressDto> addresses, string regionalTerritoryName)
        {
            var addressesByFirmCode = addresses.ToLookup(x => x.FirmCode);
            var firmIds = addressesByFirmCode.Select(x => x.Key);
            var existingFirmIds = _finder.Find<Firm>(x => firmIds.Contains(x.Id)).Select(x => x.Id).ToArray();

            var firmIdsToInsert = firmIds.Except(existingFirmIds).ToArray();
            if (!firmIdsToInsert.Any())
            {
                return;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Firm>())
            {
                foreach (var firmId in firmIdsToInsert)
                {
                    var organizationUnitDgppId = addressesByFirmCode[firmId].Select(x => x.BranchCode).First();

                    var territory = _finder.Find<OrganizationUnit>(x => x.IsActive && !x.IsDeleted && x.DgppId == organizationUnitDgppId)
                                           .SelectMany(x => x.Territories.Where(t => t.IsActive && t.Name.Contains(regionalTerritoryName)))
                                           .Select(x => new { x.Id, x.OrganizationUnitId })
                                           .SingleOrDefault();

                    if (territory == null)
                    {
                        throw new NotificationException(string.Format("Cannot find either organization unit with DgppId = {0} or its regional territory",
                                                                      organizationUnitDgppId));
                    }

                    var firm = new Firm
                        {
                            Id = firmId,
                            Name = string.Format("Пустая фирма #{0}", firmId),
                            ReplicationCode = Guid.NewGuid(),
                            TerritoryId = territory.Id,
                            ClosedForAscertainment = true,
                            OwnerCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code,
                            IsActive = false,
                            OrganizationUnitId = territory.OrganizationUnitId,
                        };

                    _firmGenericRepository.Add(firm);
                    scope.Added<Firm>(firm.Id);
                }

                scope.Complete();
            }

            _firmGenericRepository.Save();
        }
    }
}