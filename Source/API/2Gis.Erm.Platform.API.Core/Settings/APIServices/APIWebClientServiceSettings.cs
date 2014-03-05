using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public sealed class APIWebClientServiceSettings : APIServiceSettingsBase<IAPIWebClientServiceSettings>, IAPIWebClientServiceSettings
    {
        public override string Name
        {
            get
            {
                return "WebClientService";
            }
        }
        
        public Uri Url { get; private set; }

        protected override void Initialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings)
        {
            Url = configSettings.BaseUrl;
        }
    }
}