using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class UserDocMapper : IDocMapper<UserDoc>
    {
        public void UpdateDocByEntity(IEnumerable<UserDoc> docs, IEntityKey entity)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var user = entity as User;

            if (user == null)
                throw new NotSupportedException(entity.GetType().FullName);

            foreach (var doc in docs)
            {
                doc.Id = user.Id;
                doc.Name = user.DisplayName;
            }
        }
    }
}