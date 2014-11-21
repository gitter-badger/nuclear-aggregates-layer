using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public class ErrorDescription
    {
        public ErrorDescription(string messageResourceKey, Type messageResourceManagerType, string propertyNameResourceKey, Type propertyNameResourceManagerType, params object[] formatArgs)
        {
            MessageResourceKey = messageResourceKey;
            MessageResourceManagerType = messageResourceManagerType;
            PropertyNameResourceKey = propertyNameResourceKey;
            PropertyNameResourceManagerType = propertyNameResourceManagerType;
            FormatArgs = formatArgs;
        }

        public ErrorDescription(Expression<Func<string>> messageResourceAccessExpression, Expression<Func<string>> propertyNameResourceAccessExpression, params object[] formatArgs)
        {
            MessageResourceManagerType = StaticReflection.GetMemberDeclaringType(messageResourceAccessExpression);
            MessageResourceKey = StaticReflection.GetMemberName(messageResourceAccessExpression);
            PropertyNameResourceManagerType = StaticReflection.GetMemberDeclaringType(propertyNameResourceAccessExpression);
            PropertyNameResourceKey = StaticReflection.GetMemberName(propertyNameResourceAccessExpression);
            FormatArgs = formatArgs;
        }

        public string MessageResourceKey { get; private set; }
        public Type MessageResourceManagerType { get; private set; }
        public string PropertyNameResourceKey { get; private set; }
        public Type PropertyNameResourceManagerType { get; private set; }
        public object[] FormatArgs { get; private set; }
    }
}