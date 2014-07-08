using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

using ProtoBuf.Meta;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf
{
    public static class ProtoBufUtils
    {
        public static MetaType Add<TKey>(this MetaType metaType, int number, Expression<Func<TKey, object>> memberExpression)
        {
            var propertyName = StaticReflection.GetMemberName(memberExpression);
            return metaType.Add(number, propertyName);
        }
    }
}
