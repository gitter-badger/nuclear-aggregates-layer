using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages.DTO
{
    public sealed class LocalMessageDto
    {
        public LocalMessage LocalMessage { get; set; }
        public int IntegrationType { get; set; }
        public string FileName { get; set; }
    }
}