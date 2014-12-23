using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class CardRelationMap : EntityConfig<CardRelation, ErmContainer>
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