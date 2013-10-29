using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings
{
    public interface IWebApplicationUriProvider
    {
        Uri WebApplicationRoot { get; }
    }
}
