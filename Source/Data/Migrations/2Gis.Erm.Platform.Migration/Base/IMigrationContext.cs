using System;
using System.IO;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    /// <summary>
    /// ��������� ��� ��������� ��������, ����������� � ��.
    /// </summary>
    public interface IMigrationContext : IMigrationContextBase, IDisposable
    {
        Database Database { get; }
        ServerConnection Connection { get; }
        MigrationOptions Options { get; }
        TextWriter Output { get; }
    }
}