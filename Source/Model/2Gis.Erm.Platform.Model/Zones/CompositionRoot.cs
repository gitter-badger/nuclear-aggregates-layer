using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DoubleGis.Erm.Platform.Model.Zones
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class ZoneDescriptor
    {
        private readonly Type _zoneType;
        private readonly IEnumerable<Assembly> _assemblies;

        internal ZoneDescriptor(Type zoneType, IEnumerable<Assembly> assemblies)
        {
            _zoneType = zoneType;
            _assemblies = assemblies;
        }

        public IEnumerable<Assembly> Assemblies
        {
            get { return _assemblies; }
        }

        public Type ZoneType
        {
            get { return _zoneType; }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CompositionRoot
    {
        private readonly IEnumerable<ZoneDescriptor> _zoneDescriptors;

        internal CompositionRoot(IEnumerable<ZoneDescriptor> zoneDescriptors)
        {
            _zoneDescriptors = zoneDescriptors;
        }

        public static CompositionRootBuilder Config
        {
            get { return new CompositionRootBuilder(); }
        }

        public IEnumerable<ZoneDescriptor> ZoneDescriptors
        {
            get { return _zoneDescriptors; }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CompositionRootBuilder
    {
        private readonly IDictionary<Type, HashSet<Assembly>> _zonesToAssembliesMap = new Dictionary<Type, HashSet<Assembly>>();

        internal CompositionRootBuilder()
        {
        }

        public CompositionRootAnchorsBuilder<TZone> RequireZone<TZone>() where TZone : IZone
        {
            _zonesToAssembliesMap.Add(typeof(TZone), new HashSet<Assembly>());
            return new CompositionRootAnchorsBuilder<TZone>(this);
        }

        internal void AddToZonesToAssembliesMap(Type type, Assembly assembly)
        {
            _zonesToAssembliesMap[type].Add(assembly);
        }

        internal CompositionRoot AsCompositionRoot()
        {
            var zonesWithoutAnchors = _zonesToAssembliesMap.Where(x => !x.Value.Any()).Select(x => x.Key).ToArray();
            if (zonesWithoutAnchors.Any())
            {
                throw new ApplicationException(string.Format("At least one assembly anchor must be specified for zones: {0}",
                                                             string.Join(", ", zonesWithoutAnchors.Select(x => x.Name))));
            }

            return new CompositionRoot(_zonesToAssembliesMap.Select(x => new ZoneDescriptor(x.Key, x.Value)).ToArray());
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CompositionRootAnchorsBuilder<T> where T : IZone
    {
        private readonly CompositionRootBuilder _rootBuilder;

        internal CompositionRootAnchorsBuilder(CompositionRootBuilder rootBuilder)
        {
            _rootBuilder = rootBuilder;
        }

        public static implicit operator CompositionRoot(CompositionRootAnchorsBuilder<T> builder)
        {
            return builder.AsCompositionRoot();
        }

        public CompositionRootAnchorsBuilder<T> UseAnchor<TZoneAnchor>() where TZoneAnchor : IZoneAnchor<T>
        {
            _rootBuilder.AddToZonesToAssembliesMap(typeof(T), typeof(TZoneAnchor).Assembly);
            return this;
        }

        public CompositionRootAnchorsBuilder<TZone> RequireZone<TZone>() where TZone : IZone
        {
            return _rootBuilder.RequireZone<TZone>();
        }

        internal CompositionRoot AsCompositionRoot()
        {
            return _rootBuilder.AsCompositionRoot();
        }
    }
}