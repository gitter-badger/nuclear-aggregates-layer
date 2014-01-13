using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9001, "Заполнение BusnessOperationServices уже не сущностями, а описателями")]
    public class Migration9001 : TransactedMigration
    {
        private const string InsertStatements = @"
delete from [Shared].[BusinessOperationServices]
go

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (653315513, 101, 1, N'flowcardextensions.cardcommercial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1198296840, 101, 1, N'flowcardextensions.cardcommercial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 1, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 2, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 6, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 7, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 103, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (1905845000, 1, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 1, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 100, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 105, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 106, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-57837337, 1, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-57837337, 2, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-57837337, 104, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (237181970, 1, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (237181970, 102, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-579290438, 107, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1586871658, 1, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1586871658, 2, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (2093513043, 1, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (2093513043, 2, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1586871658, 102, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1904075578, 1, 1, N'floworders.resource')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1904075578, 2, 1, N'floworders.resource')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-1904075578, 102, 1, N'floworders.resource')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (18711035, 1, 1, N'floworders.themebranch')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (18711035, 2, 1, N'floworders.themebranch')
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
