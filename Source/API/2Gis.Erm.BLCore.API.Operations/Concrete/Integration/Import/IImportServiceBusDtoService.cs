using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    public interface IImportServiceBusDtoService
    {
        void Import(IEnumerable<IServiceBusDto> dtos);
    }

    // ReSharper disable once UnusedTypeParameter
    public interface IImportServiceBusDtoService<in TServiceBusDto> : IImportServiceBusDtoService
    {
    }
}