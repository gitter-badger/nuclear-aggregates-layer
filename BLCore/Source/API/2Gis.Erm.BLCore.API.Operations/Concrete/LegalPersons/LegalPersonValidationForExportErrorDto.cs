namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons
{
    public sealed class LegalPersonValidationForExportErrorDto
    {
        public long LegalPersonId { get; set; }
        public string SyncCode1C { get; set; }
        public bool IsBlockingError { get; set; }

        public string ErrorMessage { get; set; }
    }
}