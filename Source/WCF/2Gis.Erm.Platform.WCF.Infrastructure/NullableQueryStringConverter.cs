using System;
using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure
{
    public class NullableQueryStringConverter : QueryStringConverter
    {
        public override bool CanConvert(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            return (underlyingType != null && base.CanConvert(underlyingType)) || base.CanConvert(type);
        }

        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            var underlyingType = Nullable.GetUnderlyingType(parameterType);

            if (underlyingType != null)
            {
                return string.IsNullOrEmpty(parameter) ? null : base.ConvertStringToValue(parameter, underlyingType);
            }

            return base.ConvertStringToValue(parameter, parameterType);
        }
    }
}
