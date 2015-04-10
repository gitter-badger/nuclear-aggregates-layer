(function (factory) {
    factory(moment);
}(function (moment) {
    return moment.defineLocale('kk-kz', {
        months: ["қаңтар", "ақпан", "наурыз", "сәуір", "мамыр", "маусым", "шілде", "тамыз", "қыркүйек", "қазан", "қараша", "желтоқсан"],
        monthsShort: ["қаң", "ақп", "нау", "сәу", "мам", "мау", "шіл", "там", "қыр", "қаз", "қар", "жел"],
        weekdays: ["Жексенбі", "Дүйсенбі", "Сейсенбі", "Сәрсенбі", "Бейсенбі", "Жұма", "Сенбі"],
        weekdaysShort: ["Жк", "Дс", "Сс", "Ср", "Бс", "Жм", "Сн"],
        weekdaysMin: ["Жк", "Дс", "Сс", "Ср", "Бс", "Жм", "Сн"],
        week: {
            dow: 1, // Monday is the first day of the week.
            doy: 7  // The week that contains Jan 1st is the first week of the year.
        }
    });
}));
