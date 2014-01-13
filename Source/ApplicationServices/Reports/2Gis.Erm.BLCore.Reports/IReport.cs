using System.IO;

namespace DoubleGis.Erm.BLCore.Reports
{
    public interface IReport
    {
        string ReportName { get; }

        void SaveAs(string filePath);

        Stream ExecuteStream();

        ProtectedDictionary Parameters { get; }

        ProtectedDictionary GetReformattedParameters();

        ProtectedDictionary Connections { get; }
    }
}
