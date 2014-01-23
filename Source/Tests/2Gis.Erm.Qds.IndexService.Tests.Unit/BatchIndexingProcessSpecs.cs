using System;
using System.Threading;

using DoubleGis.Erm.Qds.Etl.Flow;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.IndexService.Tests.Unit
{
    class BatchIndexingProcessSpecs
    {
        [Subject(typeof(BatchIndexingProcess))]
        class When_start : BatchIndexingProcessContext
        {
            Because of = () => Target.Start();

            It should_init_elt_flow = () => Flow.Verify(f => f.Init());
        }

        // TODO В теории не надежный тест, возможны ложные срабатывания, стоит еще раз подумать о нем.
        [Subject(typeof(BatchIndexingProcess))]
        class When_start_stop_scenario_executed : BatchIndexingProcessContext
        {
            Establish context = () => Flow.Setup(f => f.Execute()).Callback(() => ExecitedCount++);

            Because of = () =>
                {
                    Target.Start();
                    Thread.Sleep(50);

                    Target.Stop();
                    TotalExecutions = ExecitedCount;
                    Thread.Sleep(50);
                };

            It should_execute_etl_flow_continuously = () => Flow.Verify(f => f.Execute(), Times.AtLeast(2));
            It should_stop_execution_after_stop = () => TotalExecutions.Should().Be(ExecitedCount);

            static int ExecitedCount;
            static int TotalExecutions;
        }

        class BatchIndexingProcessContext
        {
            Establish context = () =>
                {
                    Flow = new Mock<IEtlFlow>();
                    Target = new BatchIndexingProcess(Flow.Object, new BatchIndexingSettings(0, 10));
                };

            protected static Mock<IEtlFlow> Flow { get; private set; }

            protected static BatchIndexingProcess Target { get; private set; }
        }
    }
}
