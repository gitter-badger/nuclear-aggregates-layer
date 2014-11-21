namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists.WebClientSpecific
{
    public class JsActionFeature : IWebClientMetadataFeature
    {
        private readonly string _action;

        public JsActionFeature(string action)
        {
            _action = action;
        }

        public string Action
        {
            get { return _action; }
        }
    }
}