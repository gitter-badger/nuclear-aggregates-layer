﻿using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing
{
    public sealed class PerformedOperationsTransportSettingsAspect : ISettingsAspect, IPerformedOperationsTransportSettings
    {
        private readonly EnumSetting<PerformedOperationsTransport> 
            _performedOperationsTransport = ConfigFileSetting.Enum.Optional("PerformedOperationsTransport", PerformedOperationsTransport.DBOnline);

        public PerformedOperationsTransport OperationsTransport
        {
            get { return _performedOperationsTransport.Value; }
        }
    }
}