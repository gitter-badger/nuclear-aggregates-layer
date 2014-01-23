namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public interface IEntityLinkFilter
    {
        bool IsSupported(EntityLink link);
    }
}