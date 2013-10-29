using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public abstract class APIServiceSettingsBase<TConcreteServiceSettingsContract> : IAPIServiceSettingsInitializer
        where TConcreteServiceSettingsContract : IAPIServiceSettings
    {
        private readonly Type _concreteSettingsInterface = typeof(TConcreteServiceSettingsContract);
        
        public abstract string Name { get; }

        public Type ConcreteSettingsInterface 
        {
            get { return _concreteSettingsInterface; }
        }

        public bool TryInitialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings)
        {
            if (string.CompareOrdinal(configSettings.ServiceName, Name) != 0)
            {
                return false;
            }

            Initialize(configSettings);
            return true;
        }

        protected abstract void Initialize(ErmServiceDescriptionsConfiguration.ErmServiceDescription configSettings);
    }
}