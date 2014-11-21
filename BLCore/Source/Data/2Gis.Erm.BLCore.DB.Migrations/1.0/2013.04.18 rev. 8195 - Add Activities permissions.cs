using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8195, "Добавлены разрешения на действия")]
    public sealed class Migration8195 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string CommandTemplate = @"INSERT INTO Security.Privileges (Id, EntityType, Operation) VALUES (78067531851497728, {0}, 1), (78067927718298112, {0}, 2), (78068824150115072, {0}, 32), (78068947043222528, {0}, 65536)";
            context.Connection.ExecuteNonQuery(string.Format(CommandTemplate, 500));
        }
    }
}
