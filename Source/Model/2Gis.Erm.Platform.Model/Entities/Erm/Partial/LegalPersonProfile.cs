using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public partial class LegalPersonProfile : IPartable
    {
        public IEnumerable<IEntityPart> Parts { get; set; }
    }
}