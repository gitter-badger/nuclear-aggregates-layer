using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10305, "Удаляем сервисы обработки операций над РМ")]
    public sealed class Migration10305 : TransactedMigration
    {
        public const int AdvertisementElementTemplateDescriptor = 906062798;
        public const int CreateOrUpdateOperation = 1;
        public const int AdvertisementService = 4;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("IF EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) DELETE FROM [Shared].[BusinessOperationServices] WHERE [Descriptor] = {0} AND [Operation] = {1} AND [Service] = {2}", AdvertisementElementTemplateDescriptor, CreateOrUpdateOperation, AdvertisementService));
        }
    }
}
