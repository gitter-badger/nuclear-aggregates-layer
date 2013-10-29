using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public sealed class APIIntrospectionServiceSettings : APIServiceSettingsBase<IAPIIntrospectionServiceSettings>, IAPIIntrospectionServiceSettings
    {
        public override string Name
        {
            get { return "IntrospectionService"; }
        }

        public Uri RestUrl { get; private set; }
        public Uri BaseUrl { get; private set; }

        protected override void Initialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings)
        {
            RestUrl = configSettings.RestUrl;
            BaseUrl = configSettings.BaseUrl;
        }
    }
}