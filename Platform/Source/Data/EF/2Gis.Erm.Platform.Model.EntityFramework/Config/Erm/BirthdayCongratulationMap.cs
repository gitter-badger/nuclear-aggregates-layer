using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BirthdayCongratulationMap : EntityConfig<BirthdayCongratulation, ErmContainer>
    {
        public BirthdayCongratulationMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("BirthdayCongratulations", "Journal");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CongratulationDate).HasColumnName("CongratulationDate");
        }
    }
}