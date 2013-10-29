using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPICostCalculationServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }

    public class APICostCalculationServiceSettings : APIServiceSettingsBase<IAPICostCalculationServiceSettings>, IAPICostCalculationServiceSettings
    {
        public override string Name
        {
            get
            {
                return "CostCalculationService";
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