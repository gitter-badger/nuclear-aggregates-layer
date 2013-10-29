using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public sealed class APIIdentityServiceSettings : APIServiceSettingsBase<IAPIIdentityServiceSettings>, IAPIIdentityServiceSettings
    {
        public override string Name
        {
            get { return "IdentityService"; }
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