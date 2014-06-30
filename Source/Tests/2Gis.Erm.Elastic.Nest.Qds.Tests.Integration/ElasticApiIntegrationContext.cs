﻿using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Common;

using Machine.Specifications;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    class ElasticApiIntegrationContext
    {
        Establish context = () =>
            {
                NestSettings = ElasticTestConfigurator.CreateSettings("localhost");
                ElasticApi = ElasticTestConfigurator.CreateElasticApi(NestSettings);
                ElasticManagementApi = (IElasticManagementApi)ElasticApi;
            };

        protected static IElasticApi ElasticApi;
        protected static IElasticManagementApi ElasticManagementApi;
        protected static INestSettings NestSettings;
    }
}