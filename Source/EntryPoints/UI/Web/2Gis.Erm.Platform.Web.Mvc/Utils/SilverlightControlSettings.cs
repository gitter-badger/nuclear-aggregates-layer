using System.Collections.Specialized;

namespace DoubleGis.Erm.Platform.Web.Mvc.Utils
{
    public class SilverlightControlSettings
    {
        public const string Silverlight5ReleaseVersionNumberString = "5.0.61118.0";

        public SilverlightControlSettings()
        {
            AutoUpgrade = true;
            MinRuntimeVersion = Silverlight5ReleaseVersionNumberString;
        }

        public string ControlXapFileName { get; set; }
        public string OnErrorJsHandlerName { get; set; }
        public string OnLoadedJsHandlerName { get; set; }
        public string BackgroundColor { get; set; }
        public NameValueCollection InitParams { get; set; }
        public string MinRuntimeVersion { get; set; }
        public bool AutoUpgrade { get; set; }
        public string ControlCulture { get; set; }
    }
}