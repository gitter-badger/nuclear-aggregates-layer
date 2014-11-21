using DoubleGis.Erm.Platform.Common.Prerequisites;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Processors
{
    [Prerequisites(typeof(ReferencesEvaluatorProcessor))]
    public sealed class NavigationRestrictionsProcessor : MetadataProcessorBase<MetadataNavigationIdentity, HierarchyMetadata>
    {
        protected override void Process(
            IMetadataKindIdentity metadataKind,
            MetadataSet flattenedMetadata,
            MetadataSet concreteKindMetadata,
            HierarchyMetadata metadata)
        {
            // TODO {all, 24.03.2014}: реализовать ограничение доступных элементов в navigation pane, через обработку метаданных по операциям
        }
    }
}
