namespace DoubleGis.Erm.Platform.Common.Utils
{
    public sealed class PropertyChangeDescriptor
    {
        private readonly object _originalValue;
        private readonly object _modifiedValue;

        public PropertyChangeDescriptor(object originalValue, object modifiedValue)
        {
            _originalValue = originalValue;
            _modifiedValue = modifiedValue;
        }

        public object OriginalValue
        {
            get { return _originalValue; }
        }

        public object ModifiedValue
        {
            get { return _modifiedValue; }
        }
    }
}