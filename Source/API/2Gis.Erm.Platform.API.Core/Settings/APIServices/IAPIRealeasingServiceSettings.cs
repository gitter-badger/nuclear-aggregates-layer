using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIRealeasingServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }

    public class ApiRealeasingServiceSettings : APIServiceSettingsBase<IAPIRealeasingServiceSettings>, IAPIRealeasingServiceSettings
    {
        public override string Name
        {
            get
            {
                return "RealeasingService";
            }
        }

        public Uri RestUrl { get; private set; }

        public string SoapEndpointName { get; private set; }

        public Uri BaseUrl { get; private set; }

        protected override void Initialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings)
        {
            RestUrl = configSettings.RestUrl;
            BaseUrl = configSettings.BaseUrl;
            SoapEndpointName = configSettings.SoapEndpointName;
        }
    }
}