using System;
using System.Collections.Generic;
using System.Globalization;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Settings
{
    public class ApplicationInitParamsAppSettings : IAppSettings
    {
        private const string UserCodeParamName = "UserCode";
        private const string ControlCultureParamName = "ControlCulture";

        private readonly CultureInfo _targetCulture;
        private readonly long _userCode;

        public ApplicationInitParamsAppSettings(IDictionary<string, string> applicationInitParams)
        {
            _userCode = GetInitParam<long>(applicationInitParams, UserCodeParamName, false);
            var controlCultureName = GetInitParam<string>(applicationInitParams, ControlCultureParamName, true);

            try
            {
                _targetCulture = new CultureInfo(controlCultureName);
            }
            catch (Exception ex)
            {   // default culture
                _targetCulture = new CultureInfo("ru-RU");
            }
        }

        public CultureInfo TargetCulture
        {
            get
            {
                return _targetCulture;
            }
        }

        public long UserCode
        {
            get
            {
                return _userCode;
            }
        }

        private static TParam GetInitParam<TParam>(IDictionary<string, string> initParams, string paramName, bool isOptional)
        {
            string paramValueString;
            if (!initParams.TryGetValue(paramName, out paramValueString))
            {
                if (isOptional)
                {
                    return default(TParam);
                }

                throw new InvalidOperationException("Required silverlight app init param is not found. Param name:" + paramName);
            }

            var nullableUnderlayingType = Nullable.GetUnderlyingType(typeof(TParam));
            if (nullableUnderlayingType != null)
            {
                return (TParam)Convert.ChangeType(paramValueString, nullableUnderlayingType, CultureInfo.InvariantCulture);
            }

            return (TParam)Convert.ChangeType(paramValueString, typeof(TParam), CultureInfo.InvariantCulture);
        }
    }
}
