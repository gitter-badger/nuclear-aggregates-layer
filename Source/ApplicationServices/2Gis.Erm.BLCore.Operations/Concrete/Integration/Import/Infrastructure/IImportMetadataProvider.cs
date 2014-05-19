using System;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure
{
    public interface IImportMetadataProvider
    {
        Type GetObjectType(string flowName, string busObjectTypeName);
        bool IsSupported(string flowName);
        bool IsSupported(string flowName, string busObjectTypeName);
        int GetProcessingOrder(string flowName, string busObjectTypeName);
    }
}