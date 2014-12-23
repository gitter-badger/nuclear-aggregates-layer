using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public class ErmSecurityContainer : EntityContainerBase<ErmSecurityContainer>
    {
        public override string Name
        {
            get { return "Model.ErmSecurity"; }
        }
    }
}