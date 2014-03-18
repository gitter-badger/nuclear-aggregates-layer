using System;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Settings
{
    public sealed class APIOperationsServiceSettingsAspect : APIServiceSettingsBase, IAPIOperationsServiceSettings
    {
        public override string Name
        {
            get { return "BasicOperationsService"; }
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