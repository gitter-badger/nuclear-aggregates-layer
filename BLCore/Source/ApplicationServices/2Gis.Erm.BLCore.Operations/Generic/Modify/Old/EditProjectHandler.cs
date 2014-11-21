﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditProjectHandler : RequestHandler<EditRequest<Project>, EmptyResponse>
    {
        private readonly IProjectService _projectService;

        public EditProjectHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }

        protected override EmptyResponse Handle(EditRequest<Project> request)
        {
            var project = request.Entity;

            if (project.IsNew())
            {
                throw new NotSupportedException("Project creation is not supported");
            }

            if (project.OrganizationUnitId.HasValue)
            {
                var projects = _projectService.GetProjectsByOrganizationUnit(project.OrganizationUnitId.Value)
                                              .Where(x => x.Id != project.Id)
                                              .ToArray();
                if (projects.Length > 0)
                {
                    throw new NotificationException(
                        string.Format("На данный момент нельзя связать точку продаж с несколькими проектами. Выбранная точка продаж уже связана с проектом {0}",
                                      projects[0].DisplayName));
                }
            }

            _projectService.Update(project);
            return Response.Empty;
        }
    }
}