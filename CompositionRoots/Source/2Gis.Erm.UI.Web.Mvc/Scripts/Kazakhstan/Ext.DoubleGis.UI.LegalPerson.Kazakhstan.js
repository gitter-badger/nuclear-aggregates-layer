function GetChangeLegalPersonRequisitesUrl(legalPersonId) {
    // FIXME {all, 01.10.2014}: Подумать: мне кажется, клиент не должен знать, какая модель используется - это должен знать сервер и соответственно подстраивать роутинг.
    return '/Kazakhstan/LegalPerson/ChangeLegalPersonRequisites/' + legalPersonId; 
}

function CultureSpecificBeforeBuildActions(object) {

}