using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public partial class BranchOffice : IPartable
    {
        private IEnumerable<IEntityPart> _parts;

        public IEnumerable<IEntityPart> Parts
        {
            get
            {
                return _parts ?? (_parts = Enumerable.Empty<IEntityPart>());
            }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Parts cannot be null");
                }

                _parts = value;
            }
        }
    }
}
