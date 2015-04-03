using System.Web.Mvc;
using System.Web.Routing;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.App_Start
{
    public static class RouteConfig
    {
        internal const string CreateOrUpdateRoute = "CreateOrUpdateRoute";

        private const string GetEntityNotesRoute = "GetEntityNotesRoute";
        private const string ViewAndSearchRoute = "ViewAndSearchRoute";
        private const string EditRoute = "EditRoute";
        private const string UploadFileRoute = "UploadFileRoute";
        private const string CrmCreateOrUpdateRoute = "CrmCreateOrUpdateRoute";
        private const string GroupOperationRoute = "GroupOperationRoute";
        private const string GroupOperationConvertToEntityIdsRoute = "GroupOperationConvertToEntityIdsRoute";
        private const string SupportRoute = "SupportRoute";
        private const string ReportRoute = "ReportRoute";

        // Dynamics CRM
        private const string CrmRedirectRoute = "CrmRedirectRoute";

        public static RouteCollection Configure(this RouteCollection routes)
        {
            // default page
            routes.MapRoute(null,
                            string.Empty,
                            new { controller = "Main", action = "Index" });

            // main route
            routes.MapRoute(null,
                            "Main/{action}",
                            new { controller = "Main", action = "Index" });

            routes.MapRoute(SupportRoute,
                            "Support/{action}",
                            new { controller = "Support", action = "Index" });

            routes.MapRoute(ReportRoute,
                            "Report/{action}/{id}",
                            new { controller = "Report", action = "Edit", id = UrlParameter.Optional, });

            routes.MapRoute(GetEntityNotesRoute,
                            "Note/GetEntityNotes/{entityType}/{entityId}",
                            new
                                {
                                    controller = "Note",
                                    action = "GetEntityNotes",
                                    entityType = EntityName.None,
                                });

            routes.MapRoute(ViewAndSearchRoute,
                            "Grid/{action}/{entityTypeName}/{parentEntityType}/{parentEntityId}/{parentEntityState}/{appendedEntityType}",
                            new
                                {
                                    controller = "Grid",
                                    parentEntityType = EntityName.None,
                                    parentEntityId = UrlParameter.Optional,
                                    parentEntityState = UrlParameter.Optional,
                                    appendedEntityType = EntityName.None
                                });

            routes.MapRoute(EditRoute,
                            "Edit/{action}/{entityTypeName}/{entityId}/{entityState}",
                            new
                                {
                                    controller = "Edit",
                                    parentEntityType = EntityName.None,
                                    entityId = UrlParameter.Optional,
                                    entityState = UrlParameter.Optional
                                });

            routes.MapRoute(CreateOrUpdateRoute,
                            "CreateOrUpdate/{entityTypeName}/{entityId}",
                            new
                                {
                                    controller = "CreateOrUpdate",
                                    action = "Entity",
                                    entityId = UrlParameter.Optional
                                });

            routes.MapRoute(CrmCreateOrUpdateRoute,
                            "Crm/CreateOrUpdate/{entityTypeName}",
                            new
                                {
                                    controller = "CrmCreateOrUpdate",
                                    action = "Redirect"
                                });

            routes.MapRoute(CrmRedirectRoute,
                            "Redirect/Crm/{operationName}/{entityTypeName}/{replicationCode}",
                            new
                                {
                                    controller = "CrmRedirectToAction",
                                    action = "Execute",
                                    entityTypeName = EntityName.None,
                                    replicationCode = UrlParameter.Optional
                                });

            routes.MapRoute(GroupOperationConvertToEntityIdsRoute,
                            "GroupOperation/ConvertToEntityIds",
                            new
                                {
                                    controller = "GroupOperation",
                                    action = "ConvertToEntityIds",
                                });

            routes.MapRoute(GroupOperationRoute,
                            "GroupOperation/{operation}/{entityTypeName}",
                            new
                                {
                                    controller = "GroupOperation",
                                    action = "Execute",
                                    entityTypeName = EntityName.None,
                                });

            routes.MapRoute(UploadFileRoute,
                            "Upload/{entityTypeName}/{fileId}",
                            new
                                {
                                    controller = "Upload",
                                    action = "Upload",
                                    fileId = UrlParameter.Optional
                                });

            // this should be last route
            routes.MapRoute(null,
                            "{controller}/{action}/{id}",
                            new
                                {
                                    action = "Index",
                                    id = UrlParameter.Optional,
                                });

            return routes;
        }
    }
}