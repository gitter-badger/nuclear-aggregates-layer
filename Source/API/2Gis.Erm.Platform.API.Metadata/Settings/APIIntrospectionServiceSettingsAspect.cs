using System;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

namespace DoubleGis.Erm.Platform.API.Metadata.Settings
{
    public sealed class APIIntrospectionServiceSettingsAspect : APIServiceSettingsBase, IAPIIntrospectionServiceSettings
    {
        public override string Name
        {
            get { return "IntrospectionService"; }
        }

        public Uri RestUrl { get; private set; }
        public Uri BaseUrl { get; private set; }

        public override void Initialize(ErmServiceDescription configSettings)
        {
            RestUrl = configSettings.RestUrl;
            BaseUrl = configSettings.BaseUrl;
        }
    }
}