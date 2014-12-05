using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.Clients
{
    public sealed class EmiratesContactSalutationsProvider : IContactSalutationsProvider
    {
        public string[] GetMaleSalutations()
        {
            return new[] { string.Empty, "Mr." };
        }

        public string[] GetFemaleSalutations()
        {
            return new[] { string.Empty, "Ms." };
        }
    }
}