namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains
{
    public class EntityIdAndNumber
    {
        public EntityIdAndNumber(long id, string number)
        {
            Id = id;
            Number = number;
        }

        public long Id { get; private set; }
        public string Number { get; private set; }
    }
}
