using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9342, "Заполнение BusnessOperationServices")]
    public class Migration9342 : TransactedMigration
    {
        private const string InsertStatements = @"
delete from [Shared].[BusinessOperationServices]
go

-- flowcardextensions.cardcommercial
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1198296840, 101, 2)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (653315513, 101, 2)

-- flowfinancialdata.legalentity
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-566764810, 10, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-394024283, 10, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (936439653, 1, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (936439653, 2, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (936439653, 4, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 1, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 2, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 4, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 6, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 7, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 11, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 103, 3)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (1186303282, 4, 3)

-- floworders.advmaterial
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-57837337, 1, 4)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-57837337, 2, 4)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-57837337, 104, 4)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (237181970, 1, 4)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (237181970, 102, 4)

-- floworders.order
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 1, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 4, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 9, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 100, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 105, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 106, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-2081716307, 109, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-894528218, 4, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-579290438, 107, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-566764810, 10, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-394024283, 10, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 4, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 11, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (1186303282, 3, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (1186303282, 4, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (1186303282, 8, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (1905845000, 1, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (1905845000, 2, 5)

-- floworders.resource
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1904075578, 1, 6)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1904075578, 2, 6)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1904075578, 102, 6)

-- floworders.theme
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1586871658, 1, 7)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1586871658, 2, 7)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1586871658, 102, 7)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (2093513043, 1, 7)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (2093513043, 2, 7)

-- floworders.themebranch
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (18711035, 1, 8)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (18711035, 2, 8)

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
