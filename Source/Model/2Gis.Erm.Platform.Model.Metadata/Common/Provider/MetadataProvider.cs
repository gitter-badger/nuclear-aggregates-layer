using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Prerequisites;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Provider
{
    public sealed class MetadataProvider : IMetadataProvider
    {
        private readonly MetadataSet _flattenMetadata;
        private readonly Dictionary<IMetadataKindIdentity, MetadataSet> _kindMetadataMap;

        public MetadataProvider(IMetadataSource[] sources, IMetadataProcessor[] processors)
        {
            string mergeReport;
            if (!sources.TryMerge(out _flattenMetadata, out _kindMetadataMap, out mergeReport))
            {
                throw new InvalidOperationException("Metadata merge failed. " + mergeReport);
            }

            foreach (var metadataEntry in _kindMetadataMap)
            {
                var appropriateProcessors =
                        processors
                            .Where(p => !p.TargetMetadataConstraints.Any() || p.TargetMetadataConstraints.Any(c => c.Id == metadataEntry.Key.Id))
                            .OrderBy(p => p, new СonsideringPrerequisitesComparer<IMetadataProcessor>())
                            .ToArray();

                foreach (var metadataElement in metadataEntry.Value.Metadata.Values.ToArray())
                {
                    foreach (var processor in appropriateProcessors)
                    {
                        processor.Process(metadataEntry.Key, _flattenMetadata, metadataEntry.Value, metadataElement);
                    }
                }
            }
        }

        public MetadataSet Metadata 
        {
            get { return _flattenMetadata; }
        }

        public bool TryGetMetadata(Uri uri, out IMetadataElement element)
        {
            return _flattenMetadata.Metadata.TryGetValue(uri, out element);
        }

        public bool TryGetMetadata<TMetadataElement>(Uri uri, out TMetadataElement element) 
            where TMetadataElement : class, IMetadataElement
        {
            element = null;

            IMetadataElement rawMetadataElement;
            if (!_flattenMetadata.Metadata.TryGetValue(uri, out rawMetadataElement))
            {
                return false;
            }

            element = rawMetadataElement as TMetadataElement;
            return element != null;
        }

        public bool TryGetMetadata<TMetadataKindIdentity>(out MetadataSet metadata)
            where TMetadataKindIdentity : class, IMetadataKindIdentity, new()
        {
            var metadataKindIdentity = new TMetadataKindIdentity();
            return _kindMetadataMap.TryGetValue(metadataKindIdentity, out metadata);
        }
    }
}