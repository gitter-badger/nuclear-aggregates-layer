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
    public class ImportCategoryFirmAddressService : IImportCategoryFirmAddressService
    {
        private readonly ICreateAggregateRepository<CategoryFirmAddress> _createAggregateService;
        private readonly IUpdateAggregateRepository<CategoryFirmAddress> _updateAggregateService;
        private readonly IDeleteAggregateRepository<CategoryFirmAddress> _deleteAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFirmReadModel _firmReadModel;

        public ImportCategoryFirmAddressService(ICreateAggregateRepository<CategoryFirmAddress> createAggregateService,
                                                IUpdateAggregateRepository<CategoryFirmAddress> updateAggregateService,
                                                IDeleteAggregateRepository<CategoryFirmAddress> deleteAggregateService,
                                                IOperationScopeFactory scopeFactory,
                                                IFirmReadModel firmReadModel)
        {
            _createAggregateService = createAggregateService;
            _updateAggregateService = updateAggregateService;
            _deleteAggregateService = deleteAggregateService;
            _scopeFactory = scopeFactory;
            _firmReadModel = firmReadModel;
        }

        public void Import(IEnumerable<CategoryFirmAddress> categoryFirmAddressesToImport, IEnumerable<long> firmAddressCodes)
        {
            var existingCategoryFirmAddresses = _firmReadModel.GetCategoryFirmAddressesByFirmAddresses(firmAddressCodes);

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCategoryFirmAddressIdentity>())
            {
                scope.Complete();

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
                    foreach (var relationId in relationIdsToUpdate)
                    {
                        var dtoToUpdate = relationsToImportDictionary[relationId];
                        var existingRelation = existingRelationsDictionary[relationId];

                        existingRelation.IsPrimary = dtoToUpdate.IsPrimary;
                        existingRelation.SortingPosition = dtoToUpdate.SortingPosition;

                        existingRelation.IsActive = true;
                        existingRelation.IsDeleted = false;

                        _updateAggregateService.Update(existingRelation);
                        scope.Updated<CategoryFirmAddress>(existingRelation.Id);
                    }
                }

                if (relationIdsToInsert.Any())
                {
                    foreach (var relationId in relationIdsToInsert)
                    {
                        var dtoToInsert = relationsToImportDictionary[relationId];
                        var category = new CategoryFirmAddress
                            {
                                CategoryId = dtoToInsert.CategoryId,
                                FirmAddressId = dtoToInsert.FirmAddressId,
                                IsActive = true,
                                IsPrimary = dtoToInsert.IsPrimary,
                                SortingPosition = dtoToInsert.SortingPosition
                            };

                        _createAggregateService.Create(category);
                        scope.Added<CategoryFirmAddress>(category.Id);
                    }
                }

                if (relationIdsToDelete.Any())
                {
                    foreach (var relationId in relationIdsToDelete)
                    {
                        var relationToDelete = existingRelationsDictionary[relationId];
                        _deleteAggregateService.Delete(relationToDelete.Id);
                        scope.Deleted<CategoryFirmAddress>(relationToDelete.Id);
                    }
                }
            }
        }
    }
}