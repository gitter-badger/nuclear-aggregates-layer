using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10245, "Добавляем сервисы обработки операций над РМ")]
    public sealed class Migration10245 : TransactedMigration
    {
        public const int AdvertisementElementTemplateDescriptor = 906062798;
        public const int AdvertisementTemplateDescriptor = 591087170;
        public const int AdsTemplatesAdsElementTemplateDescriptor = 879749287;
        public const int AdvertisementService = 4;
        public const int CreateOrUpdateOperation = 1;
        public const int DeleteOperation = 2;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})", AdvertisementTemplateDescriptor, DeleteOperation, AdvertisementService));
            context.Connection.ExecuteNonQuery(string.Format("IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})", AdsTemplatesAdsElementTemplateDescriptor, DeleteOperation, AdvertisementService));
            context.Connection.ExecuteNonQuery(string.Format("IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})", AdsTemplatesAdsElementTemplateDescriptor, CreateOrUpdateOperation, AdvertisementService));
            context.Connection.ExecuteNonQuery(string.Format("IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})", AdvertisementElementTemplateDescriptor, CreateOrUpdateOperation, AdvertisementService));
        }
    }
}
