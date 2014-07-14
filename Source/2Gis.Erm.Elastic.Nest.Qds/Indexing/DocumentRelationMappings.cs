using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public static class DocumentRelationMappings
    {
        public static void InsertUserDocToClientGridDoc(ClientGridDoc document, UserDoc documentPart)
        {
            document.OwnerName = documentPart.Name;
            InsertTags(document, documentPart);
        }
        public static void InsertUserDocToFirmGridDoc(FirmGridDoc document, UserDoc documentPart)
        {
            document.OwnerName = documentPart.Name;
            InsertTags(document, documentPart);
        }

        public static void InsertTerritoryDocToClientGridDoc(ClientGridDoc document, TerritoryDoc documentPart)
        {
            document.TerritoryName = documentPart.Name;
        }
        public static void InsertTerritoryDocToFirmGridDoc(FirmGridDoc document, TerritoryDoc documentPart)
        {
            document.TerritoryName = documentPart.Name;
        }

        public static void InsertClientGridDocToFirmGridDoc(FirmGridDoc document, ClientGridDoc documentPart)
        {
            document.ClientName = documentPart.Name;
        }

        public static void InsertFirmGridDocToClientGridDoc(ClientGridDoc document, FirmGridDoc documentPart)
        {
            document.MainFirmName = documentPart.Name;
        }

        private static void InsertTags(IAuthorizationDoc authorizationDoc, UserDoc userDoc)
        {
            var operation = "List/" + authorizationDoc.GetType().Name;

            authorizationDoc.Authorization = new DocumentAuthorization
            {
                Operations = new[] { operation },
                Tags = userDoc.Tags,
            };
        }
    }
}