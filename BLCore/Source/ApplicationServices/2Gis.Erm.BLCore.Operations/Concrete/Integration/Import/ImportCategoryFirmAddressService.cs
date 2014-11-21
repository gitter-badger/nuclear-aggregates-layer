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
    public class ImportCategoryFirmAddressService : IImportCategoryFirmAddressService
    {
        private readonly IBulkCreateCategoryFirmAddressAggregateService _createAggregateService;
        private readonly IBulkDeleteCategoryFirmAddressAggregateService _deleteAggregateService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBulkUpdateCategoryFirmAddressAggregateService _updateAggregateService;

        public ImportCategoryFirmAddressService(IBulkCreateCategoryFirmAddressAggregateService createAggregateService,
                                                IBulkDeleteCategoryFirmAddressAggregateService deleteAggregateService,
                                                IFirmReadModel firmReadModel,
                                                IOperationScopeFactory scopeFactory,
                                                IBulkUpdateCategoryFirmAddressAggregateService updateAggregateService)
        {
            _createAggregateService = createAggregateService;
            _deleteAggregateService = deleteAggregateService;
            _firmReadModel = firmReadModel;
            _scopeFactory = scopeFactory;
            _updateAggregateService = updateAggregateService;
        }

        public void Import(IEnumerable<CategoryFirmAddress> categoryFirmAddressesToImport, IEnumerable<long> firmAddressCodes)
        {
            var existingCategoryFirmAddresses = _firmReadModel.GetCategoryFirmAddressesByFirmAddresses(firmAddressCodes);

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCategoryFirmAddressIdentity>())
            {
                var relationsToImportDictionary = categoryFirmAddressesToImport.ToDictionary(x => new
                                                                                                      {
                                                                                                          x.FirmAddressId,
                                                                                                          x.CategoryId
                                                                                                      });

                var existingRelationsDictionary = existingCategoryFirmAddresses.ToDictionary(x => new
                                                                                                      {
                                                                                                          x.FirmAddressId,
                                                                                                          x.CategoryId
                                                                                                      });

                var relationIdsToUpdate = relationsToImportDictionary.Keys.Intersect(existingRelationsDictionary.Keys).ToArray();
                var relationIdsToInsert = relationsToImportDictionary.Keys.Except(existingRelationsDictionary.Keys).ToArray();
                var relationIdsToDelete = existingRelationsDictionary.Keys.Except(relationsToImportDictionary.Keys).ToArray();

                if (relationIdsToUpdate.Any())
                {
                    var relationsToUpdate = new List<CategoryFirmAddress>();
                    foreach (var relationId in relationIdsToUpdate)
                    {
                        var dtoToUpdate = relationsToImportDictionary[relationId];
                        var existingRelation = existingRelationsDictionary[relationId];

                        existingRelation.IsPrimary = dtoToUpdate.IsPrimary;
                        existingRelation.SortingPosition = dtoToUpdate.SortingPosition;

                        existingRelation.IsActive = true;
                        existingRelation.IsDeleted = false;

                        relationsToUpdate.Add(existingRelation);
                    }

                    _updateAggregateService.Update(relationsToUpdate);
                }

                if (relationIdsToInsert.Any())
                {
                    var categoryFirmAddresses = relationIdsToInsert.Select(relationId => relationsToImportDictionary[relationId])
                                                                   .Select(dtoToInsert => new CategoryFirmAddress
                                                                                              {
                                                                                                  CategoryId = dtoToInsert.CategoryId,
                                                                                                  FirmAddressId = dtoToInsert.FirmAddressId,
                                                                                                  IsActive = true,
                                                                                                  IsPrimary = dtoToInsert.IsPrimary,
                                                                                                  SortingPosition = dtoToInsert.SortingPosition
                                                                                              })
                                                                   .ToArray();

                    _createAggregateService.Create(categoryFirmAddresses);
                }

                if (relationIdsToDelete.Any())
                {
                    var relationsToDelete = relationIdsToDelete.Select(relationId => existingRelationsDictionary[relationId]).ToArray();
                    _deleteAggregateService.Delete(relationsToDelete);
                }

                scope.Complete();
            }
        }
    }
}