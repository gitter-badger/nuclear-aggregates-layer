using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLCore.Tests.Unit.Model
{
    public class ConcurrentPeriodCounterSpecs
    {
        [Tags("Model")]
        [Subject(typeof(ConcurrentPeriodCounter))]
        class When_call_Count_with_empty_periods
        {
            static IConcurrentPeriodCounter _concurrentPeriodCounter;
            static int _counter;

            Establish context = () => _concurrentPeriodCounter = new ConcurrentPeriodCounter();

            Because of = () => _counter = _concurrentPeriodCounter.Count(Enumerable.Empty<TimePeriod>(), new DateTime(2012, 01, 01), new DateTime(2012, 01, 31));

            It counter_should_be_equal_to_0 = () => _counter.Should().Be(0);
        }

        [Tags("Model")]
        [Subject(typeof(ConcurrentPeriodCounter))]
        class When_call_Count_with_many_concurrent_but_excludable_periods_from_january_to_november
        {
            static IConcurrentPeriodCounter _concurrentPeriodCounter;
            static IEnumerable<TimePeriod> _periods;
            static Exception _exception;

            Establish context = () =>
                {
                    _periods = new[]
                        {
                            new TimePeriod(new DateTime(2012, 01, 01), new DateTime(2012, 04, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 02, 01), new DateTime(2012, 05, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 03, 01), new DateTime(2012, 06, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 06, 01), new DateTime(2012, 06, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 08, 01), new DateTime(2012, 11, 01).GetEndPeriodOfThisMonth()),
                        };
                    _concurrentPeriodCounter = new ConcurrentPeriodCounter();
                };

            Because of = () => _exception = Catch.Exception(() => _concurrentPeriodCounter.Count(_periods, new DateTime(2012, 01, 01), new DateTime(2012, 01, 31)));

            It exception_of_type_ArgumentException_should_be_thrown = () => _exception.Should().BeOfType<ArgumentException>();
        }

        [Tags("Model")]
        [Subject(typeof(ConcurrentPeriodCounter))]
        class When_call_Count_with_many_concurrent_periods_from_january_to_june_and_with_full_period_from_january_to_july
        {
            static IConcurrentPeriodCounter _concurrentPeriodCounter;
            static IEnumerable<TimePeriod> _periods; 
            static int _counter;

            Establish context = () =>
                {
                    _periods = new[]
                        {
                            new TimePeriod(new DateTime(2012, 01, 01), new DateTime(2012, 04, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 02, 01), new DateTime(2012, 05, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 03, 01), new DateTime(2012, 06, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 06, 01), new DateTime(2012, 06, 01).GetEndPeriodOfThisMonth()),
                        };
                    _concurrentPeriodCounter = new ConcurrentPeriodCounter();
                };

            Because of = () => _counter = _concurrentPeriodCounter.Count(_periods, new DateTime(2012, 01, 01), new DateTime(2012, 07, 01).GetEndPeriodOfThisMonth());

            It counter_should_be_equal_to_3 = () => _counter.Should().Be(3);
        }

        class When_call_Count_with_many_concurrent_periods_from_august_to_november_and_with_full_period_from_july_to_august
        {
            static IConcurrentPeriodCounter _concurrentPeriodCounter;
            static IEnumerable<TimePeriod> _periods;
            static int _counter;

            Establish context = () =>
                {
                    _periods = new[]
                        {
                            new TimePeriod(new DateTime(2012, 08, 01), new DateTime(2012, 11, 01).GetEndPeriodOfThisMonth()),
                        };
                    _concurrentPeriodCounter = new ConcurrentPeriodCounter();
                };

            Because of = () => _counter = _concurrentPeriodCounter.Count(_periods, new DateTime(2012, 07, 01), new DateTime(2012, 08, 01).GetEndPeriodOfThisMonth());

            It counter_should_be_equal_to_1 = () => _counter.Should().Be(1);
        }

        [Tags("Model")]
        [Subject(typeof(ConcurrentPeriodCounter))]
        class When_call_Count_with_many_concurrent_periods_from_february_to_november_and_with_full_period_from_may_to_august
        {
            static IConcurrentPeriodCounter _concurrentPeriodCounter;
            static IEnumerable<TimePeriod> _periods;
            static int _counter;

            Establish context = () =>
                {
                    _periods = new[]
                        {
                            new TimePeriod(new DateTime(2012, 02, 01), new DateTime(2012, 05, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 03, 01), new DateTime(2012, 06, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 08, 01), new DateTime(2012, 11, 01).GetEndPeriodOfThisMonth()),
                        };
                    _concurrentPeriodCounter = new ConcurrentPeriodCounter();
                };

            Because of = () => _counter = _concurrentPeriodCounter.Count(_periods, new DateTime(2012, 05, 01), new DateTime(2012, 08, 01).GetEndPeriodOfThisMonth());

            It counter_should_be_equal_to_2 = () => _counter.Should().Be(2);
        }

        [Tags("Model")]
        [Subject(typeof(ConcurrentPeriodCounter))]
        class When_call_Count_with_many_concurrent_periods_from_january_to_may_and_with_full_period_from_january_to_february
        {
            static IConcurrentPeriodCounter _concurrentPeriodCounter;
            static IEnumerable<TimePeriod> _periods;
            static int _counter;

            Establish context = () =>
                {
                    _periods = new[]
                        {
                            new TimePeriod(new DateTime(2012, 01, 01), new DateTime(2012, 04, 01).GetEndPeriodOfThisMonth()),
                            new TimePeriod(new DateTime(2012, 02, 01), new DateTime(2012, 05, 01).GetEndPeriodOfThisMonth()),
                        };
                    _concurrentPeriodCounter = new ConcurrentPeriodCounter();
                };

            Because of = () => _counter = _concurrentPeriodCounter.Count(_periods, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01).GetEndPeriodOfThisMonth());

            It counter_should_be_equal_to_2 = () => _counter.Should().Be(2);
        }
    }
}