using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    // NOTE: Юра, незачем оборачивать в region артефакты классов.
    //       Вообще, region отлично подходят для генерированного кода, стоит его использовать только при наличии реальной необходимости
    //       В остальных случаях стоит стараться, чтобы код был "самодокументируемым"
    internal sealed class FakeStringSecureEntity : IEntity, ICuratedEntity, IEntityKey
    {
        #region Поля
        private int fakeOwnerCode;

        private long? fakeOldOwnerCode = 2;

        private long fakeId;
        #endregion

        #region Свойства
        public String Value
        {
            get;
            set;
        }

        public long OwnerCode
        {
            get { return fakeOwnerCode; }
            set { fakeOldOwnerCode = value; }
        }

        public long? OldOwnerCode
        {
            get { return fakeOldOwnerCode; }
        }

        public long Id
        {
            get { return fakeId; }
            set { fakeId = value; }
        }
        #endregion

        #region Конструкторы
        public FakeStringSecureEntity(String value, int id, int ownerCode)
        {
            Value = value;
            fakeId = id;
            fakeOwnerCode = ownerCode;
        }
        #endregion

        #region Методы
        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            FakeStringSecureEntity p = (FakeStringSecureEntity)obj;
            return (Value == p.Value) && (Id == p.Id);
        }
        #endregion
    }
}
