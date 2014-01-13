using System.Diagnostics;
using System.Web;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI
{
    public class UnityPerWebRequestLifetimeManager : LifetimeManager
    {
        public UnityPerWebRequestLifetimeManager()
        {
        }

        private HttpContextBase Context
        {
            get
            {
                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        private object Value
        {
            [DebuggerStepThrough]
            get { return UnityPerWebRequestLifetimeModule.GetValue(Context, this); }

            [DebuggerStepThrough]
            set { UnityPerWebRequestLifetimeModule.SetValue(Context, this, value);}
        }

        [DebuggerStepThrough]
        public override object GetValue()
        {
            return Value;
        }

        [DebuggerStepThrough]
        public override void SetValue(object newValue)
        {
            Value = newValue;
        }

        [DebuggerStepThrough]
        public override void RemoveValue()
        {
            Value = null;
        }
    }
}