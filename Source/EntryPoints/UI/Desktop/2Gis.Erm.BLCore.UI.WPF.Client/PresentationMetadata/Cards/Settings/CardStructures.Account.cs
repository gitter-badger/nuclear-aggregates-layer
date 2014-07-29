﻿using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards.Generated;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public readonly static CardMetadata Account =
            CardMetadata.Config
                .For<Account>()
                .Title.Resource(() => ErmConfigLocalization.EnAccounts)
                .MVVM.Bind<DynamicCardViewModel, AccountView>()
                .WithDynamicProperties()
                .Validator.Dynamic<DynamicViewModelValidator<DynamicViewModel>, DynamicViewModel>()
                //.Validator.Static<ValidationTestValidator, ValidationTestViewModel>()
                .Localizator(typeof(MetadataResources), typeof(BLResources), typeof(EnumResources), typeof(ErmConfigLocalization));

    }
}
