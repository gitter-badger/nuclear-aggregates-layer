using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class vwUsersDescendantMap : EntityConfig<UsersDescendant, ErmContainer>
    {
        public vwUsersDescendantMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("vwUsersDescendants", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AncestorId).HasColumnName("AncestorId");
            Property(t => t.DescendantId).HasColumnName("DescendantId");
            Property(t => t.Level).HasColumnName("Level");
        }
    }
}