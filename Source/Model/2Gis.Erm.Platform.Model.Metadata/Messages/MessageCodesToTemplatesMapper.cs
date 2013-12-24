using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;

namespace DoubleGis.Erm.Model.Metadata.Messages
{
    public static class MessageCodesToTemplatesMapper
    {
        public static readonly IReadOnlyDictionary<int, Func<string>> Map = new Dictionary<int, Func<string>>
            {
                { MessageCodes.GeneralMessage, () => BLResources.GeneralMessage }
            };
    }
}
