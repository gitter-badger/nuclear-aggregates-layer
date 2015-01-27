using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IContactSalutationsAspect : IAspect
    {
        IDictionary<string, string[]> AvailableSalutations { get; set; }
    }
}