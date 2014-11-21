using System.Globalization;

using DoubleGis.Erm.BLCore.Operations.Crosscutting;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL
{
    public class FirmWorkingTimeLocalizerSpecs
    {
        [Tags("BL")]
        [Subject(typeof(FirmWorkingTimeLocalizer))]
        class When_call_LocalizeWorkingTime_for_old_way_formatted_string
        {
            const string StringToFormat = "Понедельник: 09:00 - 18:00; " +
                                          "Вторник: 09:00 - 18:00; " +
                                          "Среда: 09:00 - 18:00; " +
                                          "Четверг: 09:00 - 18:00; " +
                                          "Пятница: 09:00 - 18:00; " +
                                          "Суббота: Не работает; " +
                                          "Воскресенье: Не работает; ";

            static string _localizedString;

            Because of = () => _localizedString = FirmWorkingTimeLocalizer.LocalizeWorkingTime(StringToFormat, new CultureInfo("ru-RU"));

            It formatted_string_should_be_unchanged = () => _localizedString.Should().Be(StringToFormat);
        }

        [Tags("BL")]
        [Subject(typeof(FirmWorkingTimeLocalizer))]
        class When_call_LocalizeWorkingTime_for_new_way_formatted_string
        {
            const string StringToFormat = "{NF}Mon|00:00 - 00:00|13:00 - 14:00;" +
                                          "{NF}Tue|09:00 - 18:00|13:00 - 14:00;" +
                                          "{NF}Wed|00:00 - 00:00|13:00 - 14:00;" +
                                          "{NF}Thu|09:00 - 18:00|13:00 - 14:00;" +
                                          "{NF}Fri|09:00 - 18:00|13:00 - 14:00;" +
                                          "{NF}Sat|;" +
                                          "{NF}Sun|; ";
            const string ExpectedLocalizedString = "Понедельник: Круглосуточно, обед 13:00 - 14:00; " +
                                                   "Вторник: 09:00 - 18:00, обед 13:00 - 14:00; " +
                                                   "Среда: Круглосуточно, обед 13:00 - 14:00; " +
                                                   "Четверг: 09:00 - 18:00, обед 13:00 - 14:00; " +
                                                   "Пятница: 09:00 - 18:00, обед 13:00 - 14:00; " +
                                                   "Суббота: Не работает; Воскресенье: " +
                                                   "Не работает; ";

            static string _localizedString;

            Because of = () => _localizedString = FirmWorkingTimeLocalizer.LocalizeWorkingTime(StringToFormat, new CultureInfo("ru-RU"));

            It formatted_string_should_be_unchanged = () => _localizedString.Should().Be(ExpectedLocalizedString);
        } 
    }
}