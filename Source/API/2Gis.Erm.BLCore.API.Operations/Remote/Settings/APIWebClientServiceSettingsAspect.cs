using System;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Settings
{
    public sealed class APIWebClientServiceSettingsAspect : APIServiceSettingsBase, IAPIWebClientServiceSettings
    {
        public override string Name
        {
            get
            {
                return "WebClientService";
            }
        }
        
        public Uri Url { get; private set; }

        public override void Initialize(ErmServiceDescription configSettings)
        {
            Url = configSettings.BaseUrl;
        }
    }
}