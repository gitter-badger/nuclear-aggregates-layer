using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportCardForErmAggregateService : IAggregatePartRepository<Firm>
    {
        void ImportCategoryFirmAddresses(long firmAddressId, IEnumerable<ImportCategoryFirmAddressDto> categoryFirmAddresses);
        void ImportFirmAddresses(IEnumerable<ImportFirmAddressDto> firmAddresses, string regionalTerritoryName);
        void ImportDepCards(IEnumerable<ImportDepCardDto> importDepCardDtos);
        void ImportFirmContacts(long firmAddressId, IEnumerable<ImportFirmContactDto> firmContacts, bool isDepCard);
    }
}