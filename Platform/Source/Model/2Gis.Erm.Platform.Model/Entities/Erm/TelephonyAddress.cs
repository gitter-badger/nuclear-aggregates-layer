using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
   public class TelephonyAddress : IEntity, IEntityKey
    {
       public TelephonyAddress()
       {
           Departments = new HashSet<Department>();
       }

       public long Id { get; set; }
       public string Name { get; set; }
       public string Address { get; set; }

       public ICollection<Department> Departments { get; set; }
    }
}
