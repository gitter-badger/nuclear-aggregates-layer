﻿using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes
{
    /// <summary>
    /// Fake тип сущности из domain model системы для ErmScope, реализующий интерфейсы IEntitySecure и IEntityKey
    /// </summary>
    public sealed class ErmScopeSecureEntity1 : IEntity, ICuratedEntity, IEntityKey
    {
        public long OwnerCode
        {
            get;
            set;
        }

        public long? OldOwnerCode
        {
            get { throw new NotImplementedException(); }
        }

        public long Id
        {
            get;
            set;
        }
    }
}
