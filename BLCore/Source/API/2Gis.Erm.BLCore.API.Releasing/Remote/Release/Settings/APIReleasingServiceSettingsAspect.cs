using System;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Settings
{
    public sealed class APIReleasingServiceSettingsAspect : APIServiceSettingsBase, IAPIReleasingServiceSettings
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

        public override void Initialize(ErmServiceDescription configSettings)
        {
            RestUrl = configSettings.RestUrl;
            BaseUrl = configSettings.BaseUrl;
            SoapEndpointName = configSettings.SoapEndpointName;
        }
    }
}