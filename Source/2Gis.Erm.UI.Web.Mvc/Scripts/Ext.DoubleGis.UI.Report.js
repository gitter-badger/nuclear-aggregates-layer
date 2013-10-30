function disable() {
    Ext.getDom("Preview").disabled = "disabled";
    Ext.getDom("Download").disabled = "disabled";
}

function enable() {
    Ext.getDom("Preview").disabled = "";
    Ext.getDom("Download").disabled = "";
}

function submit() {
    if (Ext.DoubleGis.FormValidator.validate(window.EntityForm)) {
        disable();

        window.Ext.each(window.Ext.query("input.x-calendar", window.EntityForm), function (node) {
            var value = window.Ext.getCmp(node.id).getValue();
            node.value = value
                ? new Date(value).format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern)
                : "";
        });

        window.EntityForm.submit();
        enable();
    }
}

// Используется для сокрытия полей отчёта по отсутствию прав доступа
function isHiddenField(name) {
    var wrapper = window.Ext.get(name + '-wrapper');
    var isHidden = wrapper.hasClass('hidden-wrapper');
    var hiddens = Ext.decode(window.EntityForm.HiddenField.value);
    return isHidden || hiddens.indexOf(name) > -1;
}

Ext.onReady(function () {
    Ext.each(Ext.CardLookupSettings, function (item, i) {
        new window.Ext.ux.LookupField(item);
    }, this);

    Ext.get("Preview").on("click", function () {
        window.EntityForm['Format'].value = 'preview';
        submit();
    });

    Ext.get("Download").on("click", function () {
        window.EntityForm['Format'].value = 'download';
        submit();
    });

    var depList = window.Ext.getDom("ViewConfig_DependencyList");
    if (depList.value) {
        this.DependencyHandler = new window.Ext.DoubleGis.DependencyHandler();
        this.DependencyHandler.register(window.Ext.decode(depList.value), window.EntityForm);
    }

    if (Ext.getDom("Notifications").innerHTML.trim() != "") {
        Ext.getDom("Notifications").style.display = "block";
        disable();
    }
});