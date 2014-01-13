using System.Windows;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition.DTOs;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards.Custom.OrderPosition
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
