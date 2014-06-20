using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportFirmContactsService : IImportFirmContactsService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ICreateAggregateRepository<FirmContact> _createAggregateService;
        private readonly IUpdateAggregateRepository<FirmContact> _updateAggregateService;
        private readonly IDeleteAggregateRepository<FirmContact> _deleteAggregateService;

        public ImportFirmContactsService(IOperationScopeFactory scopeFactory,
                                         IFirmReadModel firmReadModel,
                                         ICreateAggregateRepository<FirmContact> createAggregateService,
                                         IUpdateAggregateRepository<FirmContact> updateAggregateService,
                                         IDeleteAggregateRepository<FirmContact> deleteAggregateService)
        {
            _scopeFactory = scopeFactory;
            _firmReadModel = firmReadModel;
            _createAggregateService = createAggregateService;
            _updateAggregateService = updateAggregateService;
            _deleteAggregateService = deleteAggregateService;
        }

        public void Import(IEnumerable<FirmContact> firmContactsToImport, IEnumerable<long> firmAddressIds, IEnumerable<long> depCardIds)
        {
            var existingPosCardContacts = _firmReadModel.GetFirmContactsByFirmAddresses(firmAddressIds);
            var existingDepCardContacts = _firmReadModel.GetFirmContactsByDepCards(depCardIds);

            var existingFirmContacts = existingDepCardContacts.Union(existingPosCardContacts);

            var contactsToImportDictionary = firmContactsToImport.ToDictionary(x => new { x.CardId, x.FirmAddressId, x.SortingPosition });
            var existingContactsDictionary = existingFirmContacts.ToDictionary(x => new { x.CardId, x.FirmAddressId, x.SortingPosition });

            var keysToUpdate = contactsToImportDictionary.Keys.Intersect(existingContactsDictionary.Keys).ToArray();
            var keysToInsert = contactsToImportDictionary.Keys.Except(existingContactsDictionary.Keys).ToArray();
            var keysToDelete = existingContactsDictionary.Keys.Except(contactsToImportDictionary.Keys).ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportFirmContactIdentity>())
            {
                if (keysToUpdate.Any())
                {
                    foreach (var key in keysToUpdate)
                    {
                        var dtoContact = contactsToImportDictionary[key];
                        var existingContact = existingContactsDictionary[key];

                        existingContact.ContactType = dtoContact.ContactType;
                        existingContact.Contact = dtoContact.Contact;

                        _updateAggregateService.Update(existingContact);
                        scope.Updated<FirmContact>(existingContact.Id);
                    }
                }

                if (keysToInsert.Any())
                {
                    foreach (var key in keysToInsert)
                    {
                        var dtoContact = contactsToImportDictionary[key];
                        var contact = new FirmContact
                            {
                                ContactType = dtoContact.ContactType,
                                Contact = dtoContact.Contact,
                                SortingPosition = dtoContact.SortingPosition,
                                CardId = dtoContact.CardId,
                                FirmAddressId = dtoContact.FirmAddressId
                            };

                        _createAggregateService.Create(contact);
                        scope.Added<FirmContact>(contact.Id);
                    }
                }

                if (keysToDelete.Any())
                {
                    foreach (var key in keysToDelete)
                    {
                        var existingContact = existingContactsDictionary[key];
                        _deleteAggregateService.Delete(existingContact.Id);
                        scope.Deleted<FirmContact>(existingContact.Id);
                    }
                }

                scope.Complete();
            }
        }
    }
}