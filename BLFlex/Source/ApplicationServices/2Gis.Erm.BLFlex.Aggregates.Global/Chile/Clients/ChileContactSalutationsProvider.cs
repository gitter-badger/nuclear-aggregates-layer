using DoubleGis.Erm.BL.API.Aggregates.Clients;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Clients
{
    public sealed class ChileContactSalutationsProvider : IContactSalutationsProvider
    {
        public string[] GetMaleSalutations()
        {
            return new[] { string.Empty, "Estimado" };
        }

        public string[] GetFemaleSalutations()
        {
            return new[] { string.Empty, "Estimada" };
        }
    }
}