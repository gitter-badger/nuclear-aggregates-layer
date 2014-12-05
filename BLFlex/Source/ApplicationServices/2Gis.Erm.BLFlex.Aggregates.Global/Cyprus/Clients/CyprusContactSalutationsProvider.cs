using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Clients
{
    public sealed class CyprusContactSalutationsProvider : IContactSalutationsProvider
    {
        public string[] GetMaleSalutations()
        {
            return new[] { string.Empty, "Dear" };
        }

        public string[] GetFemaleSalutations()
        {
            return new[] { string.Empty, "Dear" };
        }
    }
}