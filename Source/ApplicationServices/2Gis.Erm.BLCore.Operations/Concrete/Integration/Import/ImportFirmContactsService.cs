using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportFirmContactsService : IImportFirmContactsService
    {
        private readonly IBulkCreateFirmContactAggregateService _createAggregateService;
        private readonly IBulkDeleteFirmContactAggregateService _deleteAggregateService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBulkUpdateFirmContactAggregateService _updateAggregateService;

        public ImportFirmContactsService(IBulkCreateFirmContactAggregateService createAggregateService,
                                         IBulkDeleteFirmContactAggregateService deleteAggregateService,
                                         IFirmReadModel firmReadModel,
                                         IOperationScopeFactory scopeFactory,
                                         IBulkUpdateFirmContactAggregateService updateAggregateService)
        {
            _createAggregateService = createAggregateService;
            _deleteAggregateService = deleteAggregateService;
            _firmReadModel = firmReadModel;
            _scopeFactory = scopeFactory;
            _updateAggregateService = updateAggregateService;
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
                    var firmContactsToUpdate = new List<FirmContact>();
                    foreach (var key in keysToUpdate)
                    {
                        var dtoContact = contactsToImportDictionary[key];
                        var existingContact = existingContactsDictionary[key];

                        existingContact.ContactType = dtoContact.ContactType;
                        existingContact.Contact = dtoContact.Contact;

                        firmContactsToUpdate.Add(existingContact);
                    }

                    _updateAggregateService.Update(firmContactsToUpdate);
                }

                if (keysToInsert.Any())
                {
                    var firmContactsToCreate = new List<FirmContact>();
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

                        firmContactsToCreate.Add(contact);
                    }

                    _createAggregateService.Create(firmContactsToCreate);
                }

                if (keysToDelete.Any())
                {
                    var firmContactsToDelete = keysToDelete.Select(key => existingContactsDictionary[key])
                                                           .ToArray();

                    _deleteAggregateService.Delete(firmContactsToDelete);
                }

                scope.Complete();
            }
        }
    }
}