using System;
using System.Data.Entity;

using DoubleGis.Erm.Platform.DAL.EntityFramework;

namespace DoubleGis.Erm.Platform.Tests.Unit
{
    public class StubEntityEntry : IDbEntityEntry
    {
        private readonly object _entity;
        private readonly EntityState _state;

        public StubEntityEntry(object entity, EntityState state)
        {
            _entity = entity;
            _state = state;
        }

        public object Entity
        {
            get { return _entity; }
        }

        public EntityState State
        {
            get { return _state; }
        }

        public void SetCurrentValues(object entity)
        {
            throw new NotSupportedException();
        }

        public void SetAsModified()
        {
            throw new NotSupportedException();
        }
    }
}