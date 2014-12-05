using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.Clients
{
    public sealed class UkraineContactSalutationsProvider : IContactSalutationsProvider
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