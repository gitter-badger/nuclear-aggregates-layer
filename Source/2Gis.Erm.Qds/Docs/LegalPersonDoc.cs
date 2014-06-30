namespace DoubleGis.Erm.Qds.Docs
{
    public class LegalPersonDoc
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public AccountDoc[] Accounts { get; set; }
    }
}