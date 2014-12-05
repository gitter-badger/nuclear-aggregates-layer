using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Kazakhstan.Clients
{
    public sealed class KazakhstanContactSalutationsProvider : IContactSalutationsProvider
    {
        public string[] GetMaleSalutations()
        {
            return new[] { string.Empty, "Уважаемый" };
        }

        public string[] GetFemaleSalutations()
        {
            return new[] { string.Empty, "Уважаемая" };
        }
    }
}