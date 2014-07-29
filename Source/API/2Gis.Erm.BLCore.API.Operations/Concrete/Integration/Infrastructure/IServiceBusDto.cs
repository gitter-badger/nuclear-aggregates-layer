namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure
{
    public interface IServiceBusDto
    {
    }

    public interface IServiceBusDto<TServiceBusFlow> : IServiceBusDto where TServiceBusFlow : IServiceBusFlow
    {
    }
}