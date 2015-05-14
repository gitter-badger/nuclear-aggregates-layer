using DoubleGis.Erm.Platform.Common.Prerequisites;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Kinds;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;

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
