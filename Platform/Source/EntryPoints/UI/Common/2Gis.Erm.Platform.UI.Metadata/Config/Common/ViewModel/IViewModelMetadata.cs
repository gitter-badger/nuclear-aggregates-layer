﻿using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;

using NuClear.Metamodeling.Elements.Aspects.Features.Operations;
using NuClear.Metamodeling.Elements.Aspects.Features.Resources.Titles;

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