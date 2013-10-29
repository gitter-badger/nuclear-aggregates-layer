using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel
{
    public interface IViewModelStructure :
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