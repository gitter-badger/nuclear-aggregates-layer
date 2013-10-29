using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public class ApiMoDiServiceSettings : APIServiceSettingsBase<IAPIMoDiServiceSettings>, IAPIMoDiServiceSettings
    {
        public override string Name
        {
            get { return "MoDiService"; }
        }

        public Uri RestUrl { get; private set; }
        public Uri BaseUrl { get; private set; }
        public string SoapEndpointName { get; private set; }

        protected override void Initialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings)
        {
            RestUrl = configSettings.RestUrl;
            BaseUrl = configSettings.BaseUrl;
            SoapEndpointName = configSettings.SoapEndpointName;
        }
    }
}