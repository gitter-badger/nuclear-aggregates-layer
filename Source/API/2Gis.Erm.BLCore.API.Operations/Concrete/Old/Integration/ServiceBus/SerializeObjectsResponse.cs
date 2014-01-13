using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class SerializeObjectsResponse : Response
    {
        public IEnumerable<string> SerializedObjects { get; set; }
        public IEnumerable<IExportableEntityDto> FailedObjects { get; set; }
        public IEnumerable<IExportableEntityDto> SuccessObjects { get; set; }
    }
}