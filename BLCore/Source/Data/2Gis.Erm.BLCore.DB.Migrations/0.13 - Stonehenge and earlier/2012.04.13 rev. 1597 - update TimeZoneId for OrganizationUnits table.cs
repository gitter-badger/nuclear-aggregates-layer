using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // ReSharper disable InconsistentNaming
    [Migration(1597, "Апдейт часовых зон для городов присутствия 2ГИС")]
    public sealed class Migration_1597 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var commandText =
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Смоленск'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Yakutsk Standard Time' WHERE [Name]=N'Чита'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Иваново'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Калуга'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Владимир'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Старый Оскол'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'North Asia Standard Time' WHERE [Name]=N'Абакан'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Йошкар-Ола'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Орёл'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Сыктывкар'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Курск'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'Russian Standard Time' WHERE [Name]=N'Новороссийск'" + Environment.NewLine +
            "UPDATE [Billing].[OrganizationUnits] SET [TimeZoneId]=N'W. Europe Standard Time' WHERE [Name]=N'Milan'" + Environment.NewLine;

            context.Connection.ExecuteNonQuery(commandText);
        }
    }
    // ReSharper restore InconsistentNaming
}
