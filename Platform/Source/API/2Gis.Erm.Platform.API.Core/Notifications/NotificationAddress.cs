namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public sealed class NotificationAddress
    {
        private readonly string _address;
        private readonly string _displayName;
        private readonly string _displayNameEncoding;

        public NotificationAddress(string address)
            : this(address, null, null)
        {
        }

        public NotificationAddress(string address, string displayName)
            : this(address, displayName, null)
        {
        }

        public NotificationAddress(string address, string displayName, string displayNameEncoding)
        {
            _address = address;
            _displayName = displayName;
            _displayNameEncoding = displayNameEncoding;
        }

        public string DisplayNameEncoding
        {
            get { return _displayNameEncoding; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public string Address
        {
            get { return _address; }
        }
    }
}
