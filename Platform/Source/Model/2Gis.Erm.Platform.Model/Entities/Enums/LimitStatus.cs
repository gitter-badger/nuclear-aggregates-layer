namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // TODO {all, 30.10.2014}: В базе поле имеет тип smallint => EF может смапить его в enum только если в качестве underlying type указан short
    public enum LimitStatus : short
    {
        None = 0,
        Opened = 1,
        Approved = 2,
        Rejected = 3,
    }
}