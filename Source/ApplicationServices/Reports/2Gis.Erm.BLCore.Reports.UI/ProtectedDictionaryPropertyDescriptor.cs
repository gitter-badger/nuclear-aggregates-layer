using System;
using System.ComponentModel;

namespace DoubleGis.Erm.BLCore.Reports.UI
{
    class ProtectedDictionaryPropertyDescriptor : PropertyDescriptor
    {
        private ProtectedDictionary _dictionary;
        string _key;

        internal ProtectedDictionaryPropertyDescriptor(ProtectedDictionary dictionary, string key)
            : base(key.ToString(), null)
        {
            _dictionary = dictionary;
            _key = key;
        }

        public override Type PropertyType
        {
            get
            {
                if (_dictionary[_key] != null)
                {
                    return _dictionary[_key].GetType();
                }
                else
                {
                    return typeof(object);
                }
            }
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
