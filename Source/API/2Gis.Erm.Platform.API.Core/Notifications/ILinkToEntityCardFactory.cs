using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface ILinkToEntityCardFactory
    {
        string CreateLink(EntityName entity, long entityId);
        string CreateLinkTag(EntityName entity, long entityId, string linkText);
    }
}
