using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    /// <summary>
    /// �������� ���� ��� ����� ����������� � ���������� ������
    /// </summary>
    public static class DefaultConnectionStringNameExtension
    {
        private static readonly IDictionary<ConnectionStringName, string> DefaultName2ConnectionStringMap = new Dictionary<ConnectionStringName, string>
        {
            { ConnectionStringName.Logging, "ErmLogging" },
            { ConnectionStringName.Erm, "Erm" },
            { ConnectionStringName.ErmValidation, "ErmValidation" },
            { ConnectionStringName.CrmConnection, "CrmConnection" },
            { ConnectionStringName.ErmRabbitMq, "ErmRabbitMq" },
            { ConnectionStringName.ErmReports, "ErmReports" },
            { ConnectionStringName.ErmSearch, "ErmSearch" },
            { ConnectionStringName.ErmPerformedOperationsServiceBus, "ErmPerformedOperationsServiceBus" },
        };

        internal static string ToDefaultConnectionStringName(this ConnectionStringName connectionStringName)
        {
            return DefaultName2ConnectionStringMap[connectionStringName];
        }
    }
}