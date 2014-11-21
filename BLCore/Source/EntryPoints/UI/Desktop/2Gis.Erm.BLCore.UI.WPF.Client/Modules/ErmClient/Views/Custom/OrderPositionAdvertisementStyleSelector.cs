using System.Windows;
using System.Windows.Controls;

using DoubleGis.Erm.Core.Dto.DomainEntity.Custom;
using DoubleGis.Erm.UI.WPF.Client.Common.ViewModel.Card.OrderPosition;

namespace DoubleGis.Erm.UI.WPF.Client.Modules.ErmClient.Views.Custom
{
    public class OrderPositionAdvertisementStyleSelector : DataTemplateSelector
    {
        public DataTemplate PricePosition { get; set; }
        public DataTemplate Address { get; set; }
        public DataTemplate Category { get; set; }
        public DataTemplate Theme { get; set; }
        public DataTemplate CategoryAdder { get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var model = item as OrderPositionAdvertisementViewModel;
            if (model == null)
            {
                return null;
            }

            switch (model.LinkingObjectType)
            {
                case LinkingObjectType.Address:
                    return Address;
                case LinkingObjectType.Theme:
                    return Theme;
                case LinkingObjectType.PricePosition:
                    return PricePosition;
                case LinkingObjectType.Category:
                    return Category;
                case LinkingObjectType.CategoryAdder:
                    return CategoryAdder;
                default:
                    return null;
            }
        }
    }
}
