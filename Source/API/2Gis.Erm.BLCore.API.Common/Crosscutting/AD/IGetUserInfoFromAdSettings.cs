namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD
{
    public interface IGetUserInfoFromAdSettings
    {
        string ADConnectionDomainName { get;  }
        string ADConnectionLogin { get; }
        string ADConnectionPassword { get; }
    }
}
