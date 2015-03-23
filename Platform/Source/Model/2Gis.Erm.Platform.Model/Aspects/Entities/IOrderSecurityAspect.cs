namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IOrderSecurityAspect : IAspect
    {
        long CurrenctUserCode { get; set; }
        bool CanEditOrderType { get; set; }
    }
}