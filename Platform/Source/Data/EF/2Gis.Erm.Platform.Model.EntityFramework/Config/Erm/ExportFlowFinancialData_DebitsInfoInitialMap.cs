using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ExportFlowFinancialData_DebitsInfoInitialMap : EntityConfig<ExportFlowFinancialDataDebitsInfoInitial, ErmContainer>
    {
        public ExportFlowFinancialData_DebitsInfoInitialMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ExportFlowFinancialData_DebitsInfoInitial", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Date).HasColumnName("Date");
        }
    }
}