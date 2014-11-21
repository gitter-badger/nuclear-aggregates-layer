using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.MVVM
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected ViewModelBase()
        {
        }

        protected ViewModelBase(bool throwOnInvalidPropertyName)
        {
            ThrowOnInvalidPropertyName = throwOnInvalidPropertyName;
        }

        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = _propertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    var handler3 = (PropertyChangedEventHandler)Delegate.Combine(handler2, value);
                    propertyChanged = Interlocked.CompareExchange(ref _propertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
            remove
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = _propertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    var handler3 = (PropertyChangedEventHandler)Delegate.Remove(handler2, value);
                    propertyChanged = Interlocked.CompareExchange(ref _propertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);

            }
        }

        protected bool ThrowOnInvalidPropertyName { get; private set; }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                {
                    throw new Exception(msg);
                }
                
                Debug.Fail(msg);
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            FirePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            FirePropertyChanged(StaticReflection.GetMemberName(propertyExpression));
        }
        
        protected void RaiseAllPropertiesChanged()
        {
            FirePropertyChanged(null);
        }

        private void FirePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = _propertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}