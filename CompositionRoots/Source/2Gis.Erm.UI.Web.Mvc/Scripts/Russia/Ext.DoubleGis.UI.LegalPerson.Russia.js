function GetChangeLegalPersonRequisitesUrl(legalPersonId) {
    return '/Russia/LegalPerson/ChangeLegalPersonRequisites/' + legalPersonId;
}

function CultureSpecificBeforeBuildActions(object) {

    Ext.apply(object, {
        Merge: function () {
            var params = "dialogWidth:" + 700 + "px; dialogHeight:" + 350 + "px; status:yes; scroll:yes;resizable:yes;";
            var url = '/Russia/LegalPerson/Merge?masterId={0}';
            window.showModalDialog(String.format(url, Ext.getDom("Id").value), null, params);
            this.refresh(true);
        }
    });
}