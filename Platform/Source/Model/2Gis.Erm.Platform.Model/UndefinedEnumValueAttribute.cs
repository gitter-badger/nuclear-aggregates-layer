using System;

namespace DoubleGis.Erm.Platform.Model
{
    [Obsolete("������� ��� �������� '�������' �������� � enum, ���� ��� �� �����-�� �������� �� ����� 0")]
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