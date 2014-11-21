using DoubleGis.Erm.Qds.API.Operations.Docs;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static class DocumentRelationMappings
    {
        private static void InsertTags(IAuthorizationDoc authorizationDoc, UserAuthorizationDoc userAuthorizationDoc)
        {
            var operation = "List/" + authorizationDoc.GetType().Name;

            authorizationDoc.Authorization = new DocumentAuthorization
            {
                Operations = new[] { operation },
                Tags = userAuthorizationDoc.Tags,
            };
        }
    }
}