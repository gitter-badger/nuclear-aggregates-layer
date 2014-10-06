using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class CardRelationMap : EntityTypeConfiguration<CardRelation>
    {
        public CardRelationMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("CardRelations", "Integration");
            Property(t => t.DepCardCode).HasColumnName("DepCardCode");
            Property(t => t.PosCardCode).HasColumnName("PosCardCode");
            Property(t => t.OrderNo).HasColumnName("OrderNo");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}