using System;
using System.Windows;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util
{
    public static class ResourcesUtils
    {
        public static TResource GetResource<TResource>(object key, Uri resourceUri)
        {
            var dict = new ResourceDictionary { Source = resourceUri };
            return (TResource)dict[key];
        }
    }
}
