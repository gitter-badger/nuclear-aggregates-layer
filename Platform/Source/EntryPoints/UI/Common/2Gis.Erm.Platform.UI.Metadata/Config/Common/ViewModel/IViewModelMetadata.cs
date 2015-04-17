using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel
{
    public interface IViewModelMetadata :
        IBoundViewModelView,
        IPartsContainerElement,
        IRelatedItemsHost,
        IValidationTarget,
        ITitledElement,
        IOperationsBoundElement,
        IActionsContained
    {
    }
}