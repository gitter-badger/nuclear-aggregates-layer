using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO
{
    public class AssignAccountDto
    {
        public Account Account { get; set; }
        public IEnumerable<Limit> Limits { get; set; } 
    }
}