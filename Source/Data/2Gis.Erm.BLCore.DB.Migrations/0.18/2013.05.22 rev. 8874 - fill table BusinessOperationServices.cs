using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8874, "Наполнение таблицы BusinessOperationServices")]
    public class Migration8874 : TransactedMigration
    {
        private const string InsertStatements = @"
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (146, 101, 1, N'flowcardextensions.cardcommercial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (164, 101, 1, N'flowcardextensions.cardcommercial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (147, 1, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (147, 2, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (147, 6, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (147, 7, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (147, 103, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (150, 1, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (151, 1, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (151, 100, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (151, 105, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (151, 106, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (186, 1, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (186, 2, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (186, 104, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (187, 1, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (187, 102, 1, N'floworders.advmaterial')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (198, 107, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (221, 1, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (221, 2, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (223, 1, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (223, 2, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (221, 102, 1, N'floworders.theme')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (222, 1, 1, N'floworders.resource')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (222, 2, 1, N'floworders.resource')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (222, 102, 1, N'floworders.resource')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (224, 1, 1, N'floworders.themebranch')
INSERT INTO [Shared].[BusinessOperationServices] ([EntityName], [OperationName], [Service], [ServiceParameter]) VALUES (224, 2, 1, N'floworders.themebranch')
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
