window.InitPage = function () {
    Card.on("afterbuild", function () {
        Ext.get("Gender").on("change", function (s, e) {
            Card.refillSalutationComboBox(e.value);
        });

        // После загрузки страницы, выставляем значение поля "Обращение".
        var initialSalutation = Ext.getDom("InitialSalutation").value;
        Card.refillSalutationComboBox(Ext.get("Gender").getValue(), initialSalutation);
    });

    Ext.apply(this, {
        ChangeOwner: function () {
            if (Ext.getDom("Id").value == 0) {
                return;
            }

            var businessModelArea = Ext.getDom("BusinessModelArea").value;
            var params = "dialogHeight:270px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ";
            var sUrl = "/" + businessModelArea + "/GroupOperation/Assign/Contact";
            var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
            if (result === true) {
                this.refresh(true);
            }
        },

        refillSalutationComboBox: function (gender, selectedValue) {
            var genderSalutations = this.getSalutations(gender);
            var salutationDom = Ext.getDom("Salutation");
            salutationDom.length = 0;

            if (!genderSalutations || genderSalutations.length == 0) {
                return;
            }
            
            if (!selectedValue) {
                selectedValue = genderSalutations[1]; // значение по умолчанию во всех культурах
            }

            for (var i = 0; i < genderSalutations.length; i++) {
                var salutation = genderSalutations[i];
                var isSelected = selectedValue && salutation == selectedValue;
                Card.addOption(salutationDom, salutation, salutation, isSelected);
            }
        },

        getSalutations: function (gender) {
            var salutations = Ext.decode(Ext.getDom("Salutations").value);
            return salutations[gender];
        },

        addOption: function (oListbox, text, value, isSelected) {
            var oOption = document.createElement("option");
            oOption.appendChild(document.createTextNode(text));
            oOption.setAttribute("value", value);
            if (isSelected)
                oOption.selected = true;
            oListbox.appendChild(oOption);
        }
    });
}
