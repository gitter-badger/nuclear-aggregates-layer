using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD
{
    public interface IGetUserInfoFromAdSettings : ISettings
    {
        string ADConnectionDomainName { get;  }
        string ADConnectionLogin { get; }
        string ADConnectionPassword { get; }
    }
}
