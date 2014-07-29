using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4592, "Изменение хранимой процедуры ConvertUtcToTimeZone")]
    public sealed class Migration4592 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            RefreshAssemblies(context);
        }

        private void RefreshAssemblies(IMigrationContext context)
        {
            #region Текст запроса
            const string ScriptText = @"
DROP Function [Shared].[ConvertUtcToTimeZone]
GO

DROP Function [Shared].[FormatPhoneAndFax]
GO

DROP ASSEMBLY [DoubleGis.Erm.SqlClr]
GO

Create ASSEMBLY [DoubleGis.Erm.SqlClr] FROM {0}
GO

CREATE FUNCTION [Shared].[ConvertUtcToTimeZone](@utcDateTime [datetime] = NULL, @timeZoneId [nvarchar](256)) 
RETURNS [datetime] WITH EXECUTE AS CALLER 
AS 
EXTERNAL NAME [DoubleGis.Erm.SqlClr].[DoubleGis.Erm.SqlClr.ScalarFunctions].[ConvertUtcToTimeZone]
GO

GRANT EXECUTE ON OBJECT::[Shared].[ConvertUtcToTimeZone]
    TO [DVLP-2GIS\a.gutorov];
GO 

GRANT EXECUTE ON OBJECT::[Shared].[ConvertUtcToTimeZone]
    TO [2GIS\CRMReportingService];
GO 

GRANT EXECUTE ON OBJECT::[Shared].[ConvertUtcToTimeZone]
    TO [2GIS\a.pozolotin];
GO 

CREATE FUNCTION [Shared].[FormatPhoneAndFax](@phone [nvarchar](50), @format [nvarchar](50), @zone [nvarchar](50))
RETURNS nvarchar(50) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [DoubleGis.Erm.SqlClr].[DoubleGis.Erm.SqlClr.ScalarFunctions].[FormatPhoneAndFax]
GO
";
            #endregion
            byte[] dllAsArray = Resources.DoubleGis_Erm_SqlClr;
            StringBuilder sb = new StringBuilder();

            sb.Append("0x");

            foreach (byte b in dllAsArray)
            {
                sb.Append(b.ToString("X2"));
            }

            string assemblyString = sb.ToString();
            context.Connection.ExecuteNonQuery(string.Format(ScriptText, assemblyString));
        }
    }
}
