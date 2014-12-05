using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Clients
{
    public sealed class CzechContactSalutationsProvider : IContactSalutationsProvider
    {
        public string[] GetMaleSalutations()
        {
            return new[] { string.Empty, "Vážený", "Vážený pane" };
        }

        public string[] GetFemaleSalutations()
        {
            return new[] { string.Empty, "Vážená", "Vážená paní" };
        }
    }
}