using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DI.Common.Config;
using NuClear.Assembling.TypeProcessing;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing
{
    public sealed class MetadataSourcesMassProcessor : IMassProcessor
    {
        private static readonly Type MetadataSourceIndicator = typeof(IMetadataSource);

        private readonly IUnityContainer _container;
        private readonly List<Type> _metadataSourceTypes = new List<Type>();

        public MetadataSourcesMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { MetadataSourceIndicator };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            _metadataSourceTypes.AddRange(types.Where(type => !type.IsInterface && !type.IsAbstract));
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            foreach (var implementation in _metadataSourceTypes)
            {
                _container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), implementation, Lifetime.Singleton);
            }
        }
    }
}