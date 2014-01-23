using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Authorization;
using DoubleGis.Erm.Qds.API.Operations.Documents;

using Nest;

namespace DoubleGis.Erm.Qds.Common.Extensions
{
    public class MustDescriptor<TDocument>
        where TDocument : class, IDocumentAuthorization
    {
        protected readonly List<BaseQuery> QueryList = new List<BaseQuery>();

        public BaseQuery[] Queries
        {
            get { return QueryList.ToArray(); }
        }

        public MustDescriptor<TDocument> ApplyQuerySettings(QuerySettings querySettings)
        {
            // user input
            if (!string.IsNullOrEmpty(querySettings.UserInputFilter))
            {
                var userInputproperty = DocumentMetadata.GetUserInputPropertyFor<TDocument>();

                var userInputFilterLower = querySettings.UserInputFilter.ToLowerInvariant();
                userInputFilterLower = (userInputFilterLower.Contains('*') || userInputFilterLower.Contains('?'))
                                           ? userInputFilterLower
                                           : string.Concat("*", userInputFilterLower, "*");

                var queryDescriptor = new QueryDescriptor<TDocument>().Wildcard(userInputproperty, userInputFilterLower);
                QueryList.Add(queryDescriptor);
            }

            return this;
        }

        public MustDescriptor<TDocument> ApplyUserPermissions(UserDoc user)
        {
            if (!user.Tags.Contains("organization"))
            {
                var queryDescriptor = new QueryDescriptor<TDocument>().Terms(y => y.Authorization.Tags, user.Tags.ToArray());
                QueryList.Add(queryDescriptor);
            }

            return this;
        }
    }
}