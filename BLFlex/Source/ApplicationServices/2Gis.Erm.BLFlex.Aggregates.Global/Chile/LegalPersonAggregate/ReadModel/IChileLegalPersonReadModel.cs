using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel
{
    public interface IChileLegalPersonReadModel : IAggregateReadModel<LegalPerson>, IChileAdapted
    {
        EntityReference GetCommuneReference(long legalPersonId);
    }
}