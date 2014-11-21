using System;

namespace DoubleGis.Erm.Platform.Model
{
    [Obsolete("Атрибут для указания 'пустого' значения в enum, если оно по каким-то причинам не равно 0")]
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class UndefinedEnumValueAttribute : Attribute
    {
        public UndefinedEnumValueAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}