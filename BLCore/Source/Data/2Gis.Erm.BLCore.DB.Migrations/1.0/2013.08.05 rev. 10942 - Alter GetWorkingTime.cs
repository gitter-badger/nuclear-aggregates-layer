using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10942, "Меняем функцию Shared.GetWorkingTime - убираем строки, завязанные на конкретный язык.")]
    public class Migration10942 : TransactedMigration
    {
        private const string Script = @"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [Shared].[GetWorkingTime](@DayLabel nvarchar(3), @DayFrom Time, @DayTo Time, @BreakFrom Time, @BreakTo Time)
RETURNS nvarchar(max)
AS
BEGIN

DECLARE	@dayOff NVARCHAR(max)
DECLARE @roundTheClockTimePattern TIME
DECLARE @WorkHours NVARCHAR(max)
DECLARE @BreakHours NVARCHAR(max)
SET @roundTheClockTimePattern = '00:00:00'

SET @BreakHours = ''

IF @DayFrom IS NULL OR @DayTo IS NULL
	BEGIN
		SET @WorkHours = ''
	END
ELSE
	BEGIN
		SET @WorkHours = CONVERT(VARCHAR(5), @DayFrom, 108) + ' - ' + CONVERT(VARCHAR(5), @DayTo, 108)
		IF @BreakFrom IS NOT NULL AND @BreakTo IS NOT NULL
		BEGIN
			SET @BreakHours = '|'+ CONVERT(VARCHAR(5), @BreakFrom, 108) + ' - ' + CONVERT(VARCHAR(5), @BreakTo, 108)
		END
	END
RETURN '{NF}' +  @DayLabel + '|' + @WorkHours+@BreakHours+';'
END
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Script);
        }
    }
}