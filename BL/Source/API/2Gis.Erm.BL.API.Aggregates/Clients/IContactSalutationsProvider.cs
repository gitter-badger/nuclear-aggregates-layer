namespace DoubleGis.Erm.BL.API.Aggregates.Clients
{
    public interface IContactSalutationsProvider
    {
        string[] GetMaleSalutations();
        string[] GetFemaleSalutations();
    }
}
