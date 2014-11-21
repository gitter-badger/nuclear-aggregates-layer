using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    // TODO {all, 20.05.2014}: По факту это generic operation service, параметризуемый типом dto. 
    //                         Тем же типом должна закрываться его identity, что на текущий момент не поддерживается
    public interface IImportServiceBusDtoService
    {
        void Import(IEnumerable<IServiceBusDto> dtos);
    }

    // ReSharper disable once UnusedTypeParameter
    public interface IImportServiceBusDtoService<in TServiceBusDto> : IImportServiceBusDtoService
        where TServiceBusDto : IServiceBusDto
    {
    }
}