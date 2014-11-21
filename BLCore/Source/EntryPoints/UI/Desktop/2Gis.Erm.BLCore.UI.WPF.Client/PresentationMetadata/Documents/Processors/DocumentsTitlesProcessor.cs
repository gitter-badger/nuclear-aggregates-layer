using System.Linq;

using DoubleGis.Erm.Platform.Common.Prerequisites;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Processors
{
    [Prerequisites(typeof(ReferencesEvaluatorProcessor))]
    public sealed class DocumentsTitlesProcessor : MetadataProcessorBase<MetadataDocumentsIdentity, DocumentMetadata>
    {
        protected override void Process(
            IMetadataKindIdentity metadataKind,
            MetadataSet flattenedMetadata,
            MetadataSet concreteKindMetadata,
            DocumentMetadata metadata)
        {
            if (metadata.Elements == null || !metadata.Elements.Any())
            {
                return;
            }

            foreach (var childElement in metadata.Elements)
            {
                if (childElement.References.Contains(metadata))
                {
                    var titleFeature = childElement.Features<TitleFeature>().SingleOrDefault();
                    if (titleFeature == null)
                    {
                        continue;
                    }

                    var updater = (IMetadataElementUpdater)metadata;
                    updater.AddFeature(titleFeature);
                    break;
                }

                var attachedElement = childElement as AttachedMetadata;
                if (childElement is AttachedMetadata)
                {
                    var titleFeature = attachedElement.Features<TitleFeature>().SingleOrDefault();
                    if (titleFeature == null)
                    {
                        continue;
                    }

                    var updater = (IMetadataElementUpdater)metadata;
                    updater.AddFeature(titleFeature);
                }
            }
        }
    }
}