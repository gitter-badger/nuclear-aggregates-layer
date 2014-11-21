using System;
using System.Windows;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.FocusableBinding
{
    public class FocusableBinding : BindingDecoratorBase
    {
        public FocusableBinding(string path) 
        {
            Path = new PropertyPath(path);
        }

        public override object ProvideValue(IServiceProvider provider)
        {
           
            DependencyObject elem;
            string prop;
            if (TryGetTargetItems(provider, out elem, out prop))
            {
                FocusController.SetFocusableProperty(elem, prop);
            }

            return base.ProvideValue(provider);
        }
    }
}