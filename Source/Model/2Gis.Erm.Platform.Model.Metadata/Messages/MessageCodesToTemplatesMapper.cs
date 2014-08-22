using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.Platform.Model.Metadata.Messages
{
    public static class MessageCodesToTemplatesMapper
    {
        public static readonly IReadOnlyDictionary<int, Func<string>> Map = new Dictionary<int, Func<string>>
            {
                { MessageCodes.GeneralMessage, () => ResPlatform.GeneralMessage }
            };
    }
}
