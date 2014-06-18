using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Russia.LegalPersons.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.Russia.LegalPersons.ReadModel
{
    public interface IRussiaLegalPersonReadModel : IAggregateReadModel<LegalPerson>, IRussiaAdapted
    {
        LegalPersonForMergeDto GetInfoForMerge(long legalPersonId);
    }
}