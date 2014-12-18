using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class MessageTypeMap : EntityConfig<MessageType, ErmContainer>
    {
        public MessageTypeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("MessageTypes", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SenderSystem).HasColumnName("SenderSystem");
            Property(t => t.ReceiverSystem).HasColumnName("ReceiverSystem");
            Property(t => t.IntegrationType).HasColumnName("IntegrationType");
        }
    }
}