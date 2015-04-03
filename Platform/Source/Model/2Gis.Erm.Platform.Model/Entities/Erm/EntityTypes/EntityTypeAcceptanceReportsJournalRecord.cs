using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAcceptanceReportsJournalRecord : EntityTypeBase<EntityTypeAcceptanceReportsJournalRecord>
    {
        public override int Id
        {
            get { return EntityTypeIds.AcceptanceReportsJournalRecord; }
        }

        public override string Description
        {
            get { return "AcceptanceReportsJournalRecord"; }
        }
    }
}