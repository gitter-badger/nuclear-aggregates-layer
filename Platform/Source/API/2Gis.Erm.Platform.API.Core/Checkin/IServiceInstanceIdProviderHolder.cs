namespace DoubleGis.Erm.Platform.API.Core.Checkin
{
    public interface IServiceInstanceIdProviderHolder
    {
        void SetProvider(IServiceInstanceIdProvider serviceInstanceIdProvider);
        void RemoveProvider(IServiceInstanceIdProvider serviceInstanceIdProvider);
    }
}