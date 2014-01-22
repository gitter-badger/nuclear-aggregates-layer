using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards.Generated;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public readonly static CardStructure LegalPerson =
            CardStructure.Config
                .For(LegalPersonAggregate.LegalPerson.AsEntityName())
                .Title.Resource(() => ErmConfigLocalization.EnLegalPerson)
                .Parts.Use(() => BLResources.AdministrationTabTitle)
                .MVVM.Bind<DynamicCardViewModel, LegalPersonView>()
                .WithDynamicProperties()
                .Validator.Dynamic<DynamicViewModelValidator<DynamicViewModel>, DynamicViewModel>()
                .Localizator(typeof(MetadataResources), typeof(BLResources), typeof(EnumResources));
    }
}
