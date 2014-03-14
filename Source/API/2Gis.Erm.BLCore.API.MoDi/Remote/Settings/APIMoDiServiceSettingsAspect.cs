using System;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings
{
    public sealed class APIMoDiServiceSettingsAspect : APIServiceSettingsBase, IAPIMoDiServiceSettings
    {
        public override string Name
        {
            get { return "MoDiService"; }
        }

        public Uri RestUrl { get; private set; }
        public Uri BaseUrl { get; private set; }
        public string SoapEndpointName { get; private set; }

        public override void Initialize(ErmServiceDescription configSettings)
        {
            RestUrl = configSettings.RestUrl;
            BaseUrl = configSettings.BaseUrl;
            SoapEndpointName = configSettings.SoapEndpointName;
        }
    }
}