using System;
using System.Linq.Expressions;
using System.Windows;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils
{
    public static class ResourcesUtils
    {
        public static ComponentResourceKey CreateKey<TKey>(Expression<Func<TKey>> keyMemberExpression)
        {
            string keyMemberName = StaticReflection.GetMemberName(keyMemberExpression);
            Type keyMemberContainer = StaticReflection.GetMemberDeclaringType(keyMemberExpression);
            return new ComponentResourceKey(keyMemberContainer, keyMemberName);
        }
    }
}
