using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public sealed class APIOperationsServiceSettings : APIServiceSettingsBase<IAPIOperationsServiceSettings>, IAPIOperationsServiceSettings
    {
        public override string Name
        {
            get { return "BasicOperationsService"; }
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