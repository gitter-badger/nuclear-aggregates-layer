using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Clients
{
    public sealed class RussiaContactSalutationsProvider : IContactSalutationsProvider
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