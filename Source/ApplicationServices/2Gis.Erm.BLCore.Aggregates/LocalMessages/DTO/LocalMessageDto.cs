using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LocalMessages.DTO
{
    public sealed class LocalMessageDto
    {
        public LocalMessage LocalMessage { get; set; }
        public int IntegrationType { get; set; }
        public string FileName { get; set; }
    }
}