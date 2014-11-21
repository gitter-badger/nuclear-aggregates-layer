using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Provider
{
    public static class MetadataProviderUtils
    {
        public static bool TryMerge(
            this IMetadataSource[] sources,
            out MetadataSet flattenMetadata,
            out Dictionary<IMetadataKindIdentity, MetadataSet> byKindMetadata,
            out string report)
        {
            flattenMetadata = new MetadataSet();
            byKindMetadata = new Dictionary<IMetadataKindIdentity, MetadataSet>();

            var aggregatedReport = new StringBuilder();

            foreach (var sourceInfo in sources.GroupBy(s => s.Kind, (kind, kindSources) => new { Kind = kind, Sources = kindSources }))
            {
                var metadataSet = new MetadataSet();
                byKindMetadata.Add(sourceInfo.Kind, metadataSet);

                foreach (var source in sourceInfo.Sources)
                {
                    string internalReport;
                    if (!TryProcessElements(source.Kind, source.Metadata.Values, flattenMetadata, metadataSet, out internalReport))
                    {
                        aggregatedReport.AppendLine(internalReport);
                    }
                }
            }

            report = aggregatedReport.Length > 0 ? aggregatedReport.ToString() : null;
            return report == null;
        }

        private static bool TryProcessElements(
            IMetadataKindIdentity metadataKindIdentity, 
            IEnumerable<IMetadataElement> metadataElements,
            MetadataSet flattenMetadata,
            MetadataSet byKindMetadata,
            out string report)
        {
            var aggregatedReport = new StringBuilder();

            foreach (var metadataElement in metadataElements)
            {
                metadataElement.EnsureCorrectness(metadataKindIdentity);

                string internalReport;
                IMetadataElement mergeTargetElement;
                if (!flattenMetadata.Metadata.TryGetValue(metadataElement.Identity.Id, out mergeTargetElement))
                {
                    flattenMetadata.Metadata.Add(metadataElement.Identity.Id, metadataElement);
                    byKindMetadata.Metadata.Add(metadataElement.Identity.Id, metadataElement);
                }

                if (metadataElement.Elements != null &&
                    !TryProcessElements(metadataKindIdentity, metadataElement.Elements, flattenMetadata, byKindMetadata, out internalReport))
                {
                    aggregatedReport.AppendLine(internalReport);
                }

                if (mergeTargetElement != null)
                {
                    if (!TryMerge(mergeTargetElement, metadataElement, out internalReport))
                    {
                        aggregatedReport.AppendLine(internalReport);
                    }
                }
            }

            report = aggregatedReport.Length > 0 ? aggregatedReport.ToString() : null;
            return report == null;
        }

        private static void EnsureCorrectness(this IMetadataElement metadataElement, IMetadataKindIdentity metadataKindIdentity)
        {
            var elementUpdater = (IMetadataElementUpdater)metadataElement;
            elementUpdater.ActualizeKind(metadataKindIdentity);

            if (metadataElement.Identity == null || metadataElement.Identity.Id.IsStub())
            {
                elementUpdater.ActualizeId(new MetadataElementIdentity(metadataElement.Parent.Identity.Id.DynamicChild()));
                return;
            }

            if (!metadataElement.Identity.Id.IsAbsoluteUri)
            {
                elementUpdater.ActualizeId(new MetadataElementIdentity(metadataElement.Parent.Identity.Id.WithRelative(metadataElement.Identity.Id)));
            }
        }

        private static bool TryMerge(IMetadataElement target, IMetadataElement source, out string report)
        {
            var targetUpdater = target as IMetadataElementUpdater;
            if (targetUpdater == null || !(source is IMetadataElementUpdater))
            {
                report = "Can't merge elements. Elements have to be updatalbe. Target: " + target.Identity.Id + ". Source: " + source.Identity.Id;
                return false;
            }

            if (!TryMergeFeatures(targetUpdater, target.Features, source.Features, out report))
            {
                report = string.Format("Can't merge features of target {0} and source {1}. {2}", target.Identity.Id, source.Identity.Id, report ?? string.Empty);
                return false;
            }

            if (!TryMergeChilds(target, target.Elements, source.Elements, out report))
            {
                report = string.Format("Can't merge childs of target {0} and source {1}. {2}", target.Identity.Id, source.Identity.Id, report ?? string.Empty);
                return false;
            }

            return true;
        }

        private static bool TryMergeFeatures(IMetadataElementUpdater targetUpdater,
                                             IEnumerable<IMetadataFeature> targetFeatures,
                                             IEnumerable<IMetadataFeature> sourceFeatures,
                                             out string report)
        {
            var targetFeaturesRegistry = targetFeatures.Aggregate(new HashSet<Type>(),
                                                    (set, feature) =>
                                                        {
                                                            set.Add(feature.GetType());
                                                            return set;
                                                        });
            var aggregatedReport = new StringBuilder();

            foreach (var sourceFeature in sourceFeatures)
            {
                var sourceFeatureType = sourceFeature.GetType();
                if (targetFeaturesRegistry.Contains(sourceFeatureType) && sourceFeature is IUniqueMetadataFeature)
                {
                    aggregatedReport.AppendLine("Feature of type " + sourceFeatureType.FullName + " and unique constraint already exists in master element. Unique feature duplicates are prohibited.");
                    continue;
                }

                targetUpdater.AddFeature(sourceFeature);
            }
            
            report = aggregatedReport.Length > 0 
                ? aggregatedReport.ToString() 
                : null;

            return report == null;
        }

        private static bool TryMergeChilds(IMetadataElement targetElement,
                                           IEnumerable<IMetadataElement> targetChilds,
                                           IEnumerable<IMetadataElement> sourceChilds,
                                           out string report)
        {
            var aggregatedReport = new StringBuilder();
            var targetUpdater = (IMetadataElementUpdater)targetElement;

            var targetChildsRegistry = targetChilds.Aggregate(new Dictionary<Uri, IMetadataElement>(),
                                                    (set, element) =>
                                                    {
                                                        set.Add(element.Identity.Id, element);
                                                        return set;
                                                    });

            foreach (var sourceChild in sourceChilds)
            {
                IMetadataElement conflictingTargetChild;
                if (targetChildsRegistry.TryGetValue(sourceChild.Identity.Id, out conflictingTargetChild))
                {
                    var conflictingTargetChildType = conflictingTargetChild.GetType();
                    var sourceChildType = sourceChild.GetType();
                    if (conflictingTargetChildType != sourceChildType)
                    {
                        aggregatedReport
                            .AppendFormat(
                                "Can't execute safe auto merge for elements with same id {0} and different types target {1} source {2}",
                                sourceChild.Identity.Id,
                                conflictingTargetChildType.FullName,
                                sourceChildType.FullName)
                            .Append(Environment.NewLine);
                    }

                    continue;
                }

                targetUpdater.AddChilds(new[] { sourceChild });
            }

            report = aggregatedReport.Length > 0
                ? aggregatedReport.ToString()
                : null;

            return report == null;
        }
    }
}