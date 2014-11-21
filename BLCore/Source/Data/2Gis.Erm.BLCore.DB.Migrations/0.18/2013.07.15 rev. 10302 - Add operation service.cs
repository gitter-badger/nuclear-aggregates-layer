using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10302, "Добавляем сервисы обработки операций над РМ")]
    public sealed class Migration10302 : TransactedMigration
    {
        public const int AdvertisementTemplateDescriptor = 591087170;
        public const int AdvertisementService = 4;
        public const int CreateOperation = 12;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})", AdvertisementTemplateDescriptor, CreateOperation, AdvertisementService));
        }
    }
}
