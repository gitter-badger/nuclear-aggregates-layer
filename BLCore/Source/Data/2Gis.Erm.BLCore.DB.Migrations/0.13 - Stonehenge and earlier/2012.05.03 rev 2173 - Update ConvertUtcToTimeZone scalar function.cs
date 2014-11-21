using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2173, "Обновление функции [ConvertUtcToTimeZone] для поддержки отчетов OLAP." +
                           "Теперь функция будет принимать целочисленый timezoneid и конвертировать его в строковый через справочную таблицу TimeZones. " +
                           "После чего вызывается SQLCLR функция которая и выполняет конвертацию времени")]
    public class Migration2173 : TransactedMigration
    {

        private readonly SchemaQualifiedObjectName _convertUtcToTimeZoneClrProxy =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "ConvertUtcToTimeZoneClrProxy");

        private readonly SchemaQualifiedObjectName _convertUtcToTimeZoneClr =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "ConvertUtcToTimeZone");
        
        private const String ConvertUtcToTimeZoneClrTemplate =
@"BEGIN
	DECLARE @timzeZoneStr nvarchar(256) 
	SET @timzeZoneStr = (select tz.TimeZoneId from {0} tz where tz.Id = @timeZoneId)
	return {1}(@utcDateTime, @timzeZoneStr)
END";

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateConvertUtcToTimeZoneClrProxy(context);
            CreateConvertUtcToTimeZoneClr(context);
        }

        private const String AssemblyName = "DoubleGis.Databases.Erm.SqlClr";
        private const String ClassName = "DoubleGis.Databases.Erm.SqlClr.ScalarFunctions";
        private const String MethodName = "ConvertUtcToTimeZone";

        private void CreateConvertUtcToTimeZoneClrProxy(IMigrationContext context)
        {
            var function =
                context.Database.UserDefinedFunctions[_convertUtcToTimeZoneClrProxy.Name, _convertUtcToTimeZoneClrProxy.Schema];
            if (function != null)
            {
                function.Drop();
            }

            function = new UserDefinedFunction(context.Database, _convertUtcToTimeZoneClrProxy.Name, _convertUtcToTimeZoneClrProxy.Schema);
            function.TextMode = false;
            function.FunctionType = UserDefinedFunctionType.Scalar;
            function.ExecutionContext = ExecutionContext.Caller;
            function.DataType = DataType.DateTime2(7);
            function.ImplementationType = ImplementationType.SqlClr;
            function.AssemblyName = AssemblyName;
            function.ClassName = ClassName;
            function.MethodName = MethodName;
            var param = new UserDefinedFunctionParameter(function, "@utcDateTime", DataType.DateTime2(7));
            function.Parameters.Add(param);
            param = new UserDefinedFunctionParameter(function, "@timeZoneId", DataType.NVarChar(256));
            function.Parameters.Add(param);
            function.Create();
        }

        private void CreateConvertUtcToTimeZoneClr(IMigrationContext context)
        {
            var function =
                context.Database.UserDefinedFunctions[_convertUtcToTimeZoneClr.Name, _convertUtcToTimeZoneClr.Schema];
            if (function != null)
            {
                function.Drop();
            }

            function = new UserDefinedFunction(context.Database, _convertUtcToTimeZoneClr.Name, _convertUtcToTimeZoneClr.Schema);
            function.TextMode = false;
            function.FunctionType = UserDefinedFunctionType.Scalar;
            function.ExecutionContext = ExecutionContext.Caller;
            function.TextBody = String.Format(ConvertUtcToTimeZoneClrTemplate, ErmTableNames.TimeZones, _convertUtcToTimeZoneClrProxy);
            function.DataType = DataType.DateTime2(7);
            function.ImplementationType = ImplementationType.TransactSql;
            var param = new UserDefinedFunctionParameter(function, "@utcDateTime", DataType.DateTime2(7));
            function.Parameters.Add(param);
            param = new UserDefinedFunctionParameter(function, "@timeZoneId", DataType.Int);
            function.Parameters.Add(param);
            function.Create();
        }
    }
}
