using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider
{
    internal class DynamicPropertyInfo : PropertyInfo
    {
        private readonly string _name;
        private readonly Type _type;
        private readonly object _defaultValue;
        private readonly IEnumerable<Attribute> _attributes;
        private readonly MethodInfo _setMethodInfo;

        internal DynamicPropertyInfo(string name, Type type)
        {
            _name = name;
            _type = type;

            var setMethodName = StaticReflection.GetMemberName(() => SetValueAction(null, null));
            _setMethodInfo = GetType().GetMethod(setMethodName);
        }

        internal DynamicPropertyInfo(string name, Type type, object defaultValue, IEnumerable<Attribute> attributes)
            : this(name, type)
        {
            _defaultValue = defaultValue;
            _attributes = attributes;
        }

        public override PropertyAttributes Attributes
        {
            get { return PropertyAttributes.None; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _type; }
        }

        public override Type DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { return _name; }
        }

        public override Type ReflectedType
        {
            get { throw new NotImplementedException(); }
        }

        public virtual object GetDefaultValue(IDynamicPropertiesContainer entity)
        {
            return _defaultValue;
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            Action<IDynamicPropertiesContainer> getMethod = obj => obj.GetDynamicPropertyValue(_name);
            return getMethod.Method;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return new ParameterInfo[0];
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return _setMethodInfo;
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            return ((IDynamicPropertiesContainer)obj).GetDynamicPropertyValue(_name);
        }

        public void SetValueAction(object obj, object value)
        {
            // данный метод должен быть именно public - инфраструктура WPF binding проверяет возможена ли запись в этой свойство (при TwoWay, OneWayToSource и т.п. bindings)
            // проверка смотрит, на результат CanWrite, GetSetMethod (не null, public или нет)
            // в зависимости от сочетания этих факторов принимается решение бросить exception или нет: InvalidOperationException A TwoWay or OneWayToSource binding cannot work on the read-only property
            // Поведение изменилось при переходе .NET 4.0->4.5
            // Подробности см:
            //  - PropertyPathWorker.IsPropertyReadOnly
            //  - FrameworkCompatibilityPreferences.HandleTwoWayBindingToPropertyWithNonPublicSetter
            ((IDynamicPropertiesContainer)obj).SetDynamicPropertyValue(_name, value);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            SetValueAction(obj, value);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _attributes != null ? _attributes.Where(a => a.GetType() == attributeType).Cast<object>().ToArray() : new object[0];
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _attributes != null ? _attributes.Cast<object>().ToArray() : new object[0];
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}