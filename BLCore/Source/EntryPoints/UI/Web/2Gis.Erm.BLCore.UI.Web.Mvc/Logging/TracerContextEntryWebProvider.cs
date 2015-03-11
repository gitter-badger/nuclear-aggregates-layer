using System;
using System.Web;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    public class TracerContextEntryWebProvider : TracerContextEntryProvider
    {
        public TracerContextEntryWebProvider(string tracerContextKey) 
            : base(tracerContextKey)
        {
        }

        private bool IsHttpContextAvailable
        {
            get
            {
                try
                {
                    return HttpContext.Current != null;
                }
                catch
                {
                    return false;
                }                
            }
        }

        public override string Value
        {
            get
            {
                if (!IsHttpContextAvailable)
                {
                    return String.Empty;
                }

                return HttpContext.Current.Items[Key] as String;
            }
            set
            {
                if (!IsHttpContextAvailable)
                {
                    return;
                }

                HttpContext.Current.Items[Key] = value;
            }
        }
    }
}