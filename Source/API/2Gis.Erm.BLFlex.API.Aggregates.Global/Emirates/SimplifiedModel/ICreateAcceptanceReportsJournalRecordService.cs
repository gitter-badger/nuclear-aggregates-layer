using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.SimplifiedModel
{
    public interface ICreateAcceptanceReportsJournalRecordService : ISimplifiedModelConsumer, IEmiratesAdapted
    {
        int Create(AcceptanceReportsJournalRecord acceptanceReportsJournalRecord);
    }
}