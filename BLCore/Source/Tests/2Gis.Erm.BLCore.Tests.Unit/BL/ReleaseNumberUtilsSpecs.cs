using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL
{
    public class ReleaseNumberUtilsSpecs
    {
        [Tags("BL")]
        [Subject(typeof(ReleaseNumberUtils))]
        class When_call_AbsoluteReleaseNumberToMonth_with_150_as_argument
        {
            static DateTime _date;

            Because of = () => _date = ReleaseNumberUtils.AbsoluteReleaseNumberToMonth(150);

            It should_be_2011_03_01_date_as_result = () => _date.Should().Be(new DateTime(2011, 03, 01));
        }

        [Tags("BL")]
        [Subject(typeof(ReleaseNumberUtils))]
        class When_call_ToAbsoluteReleaseNumber_with_2011_03_01_date_as_argument
        {
            static int _releaseNumber;

            Because of = () => _releaseNumber = new DateTime(2011, 03, 01).ToAbsoluteReleaseNumber();

            It should_be_150_as_result = () => _releaseNumber.Should().Be(150);
        }
    }
}