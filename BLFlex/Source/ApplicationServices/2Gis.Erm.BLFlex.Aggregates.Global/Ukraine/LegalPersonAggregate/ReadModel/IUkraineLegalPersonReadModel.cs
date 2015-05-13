using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.LegalPersonAggregate.ReadModel
{
    public interface IUkraineLegalPersonReadModel : IAggregateReadModel<LegalPerson>, IUkraineAdapted
    {
        bool AreThereAnyActiveEgrpouDuplicates(long legalPersonId, string egrpou);
    }
}