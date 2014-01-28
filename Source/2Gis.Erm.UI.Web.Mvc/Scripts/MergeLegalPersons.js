Ext.namespace('Ext.ux');
Ext.ux.MergeLegalPersonsUtility = Ext.extend(Object,
            {
                addNotification: function (message, level, messageId) {
                    var nopt = { message: message, level: window.Ext.Notification.Icon[level], messageId: messageId };
                    var nc = Ext.get("Notifications");
                    if (!this.NotificationTemplate) {
                        this.NotificationTemplate = new Ext.XTemplate(
                        '<div id="{messageId}" class="Notification">',
                        '<table cellspacing="0" cellpadding="0"><tbody><tr><td valign="top">' +
                            '<img class="ms-crm-Lookup-Item" alt="" src="{level}"/>',
                        '</td><td width="5px"></td><td><span id="NotificationText">{message}</span>',
                        '</td></tr></tbody></table></div>');
                    }
                    nc.show(true).dom.innerHTML = "";
                    this.NotificationTemplate.append(nc.dom, nopt);
                },
                getLegalPersonsData: function () {
                    if (Ext.getCmp("LegalPerson1").getValue() && Ext.getCmp("LegalPerson2").getValue()) {
                        Ext.Ajax.request(
                            {
                                url: "/Russia/LegalPerson/MergeLegalPersonsGetData",
                                scope: this,
                                params: { masterId: Ext.getCmp("LegalPerson1").getValue().id, subordinateId: Ext.getCmp("LegalPerson2").getValue().id },
                                success: this.getLegalPersonsDataSuccess,
                                failure: this.getLegalPersonsDataFailure
                            });
                    }
                    else {
                        Ext.get("dataRegion").update('');
                    }
                },
                getLegalPersonsDataSuccess: function (jsonResponse) {
                    Ext.get("dataRegion").update(jsonResponse.responseText);
                    this.onMainRadioClick(null, this.leftGroup.legalPersonRad.dom.checked ? this.leftGroup.legalPersonRad.dom : this.rightGroup.legalPersonRad.dom);
                },
                getLegalPersonsDataFailure: function (xhr) {
                    Ext.get("dataRegion").update('');
                    Ext.MessageBox.show({
                        title: Ext.LocalizedResources.Error,
                        msg: xhr.responseText,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });
                },
                close: function () {
                    window.close();
                },
                submitForm: function () {
                    if (Ext.DoubleGis.FormValidator.validate(window.EntityForm)) {
                        
                        Ext.MessageBox.show({
                            title: Ext.LocalizedResources.Alert,
                            msg: Ext.LocalizedResources.MergeLegalPersonsAlert,
                            width: 300,
                            buttons: window.Ext.MessageBox.ContinueCANCEL,
                            fn: ContinueProccess,
                            icon: window.Ext.MessageBox.QUESTION
                        });
                    }
                },
                onMainRadioClick: function (evt, node) {
                    if (Ext.getCmp("LegalPerson2").getValue() != null && Ext.getCmp("LegalPerson1").getValue() != null) {
                        var left = Ext.getDom('legalPerson_1').checked;
                        Ext.getDom('AppendedLegalPersonId').value = left ? Ext.getCmp("LegalPerson2").getValue().id : Ext.getCmp("LegalPerson1").getValue().id;
                        Ext.getDom('MainLegalPersonId').value = left ? Ext.getCmp("LegalPerson1").getValue().id : Ext.getCmp("LegalPerson2").getValue().id;
                        Ext.getDom("OK").disabled = false;
                    } else {
                        Ext.getDom("OK").disabled = true;
                    }
                },
                init: function () {
                    
                    Ext.getDom("OK").disabled = true;
                    
                    if (Ext.getDom("Message").innerHTML.trim() == "OK") {
                        Ext.MessageBox.alert(Ext.LocalizedResources.MergeLegalPersonsSucceded);
                        window.close();
                        return;
                    }
                    else if (Ext.getDom("Message").innerHTML.trim() != "") {
                        if (window.Ext.getDom("MessageType").innerHTML.trim() == "CriticalError") {
                            Ext.MessageBox.show({
                                title: window.Ext.getDom("MessageType").innerHTML.trim(),
                                msg: window.Ext.getDom("Message").innerHTML.trim(),
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        }
                            
                    }

                    window.Ext.each(window.Ext.CardLookupSettings, function (item) {
                        new window.Ext.ux.LookupField(item);
                    }, this);

                    Ext.get("Cancel").on("click", this.close);
                    Ext.get("OK").on("click", this.submitForm);
                   
                    Ext.getCmp("LegalPerson1").on('change', this.getLegalPersonsData, this);
                    Ext.getCmp("LegalPerson2").on('change', this.getLegalPersonsData, this);

                    Ext.getCmp("LegalPerson1").on('change', this.onMainRadioClick, this);
                    Ext.getCmp("LegalPerson2").on('change', this.onMainRadioClick, this);

                    this.leftGroup = {
                        legalPersonRad: Ext.get("legalPerson_1"),
                        sections: []
                    };
                    this.rightGroup = {
                        legalPersonRad: Ext.get("legalPerson_2"),
                        sections: []
                    };

                    this.leftGroup.legalPersonRad.on('click', this.onMainRadioClick, this);
                    this.rightGroup.legalPersonRad.on('click', this.onMainRadioClick, this);
                    
                    if (Ext.getCmp("LegalPerson2").getValue() != null && Ext.getCmp("LegalPerson1").getValue() != null) {
                        var left = Ext.getDom('legalPerson_1').checked;
                        Ext.getDom('AppendedLegalPersonId').value = left ? Ext.getCmp("LegalPerson2").getValue().id : Ext.getCmp("LegalPerson1").getValue().id;
                        Ext.getDom('MainLegalPersonId').value = left ? Ext.getCmp("LegalPerson1").getValue().id : Ext.getCmp("LegalPerson2").getValue().id;
                        Ext.getDom("OK").disabled = false;
                    }

                    this.getLegalPersonsData();
                }
                
            });

Ext.onReady(function () {
    new Ext.ux.MergeLegalPersonsUtility().init();
});

function ContinueProccess(buttonId, dir_values) {
    if (buttonId == 'Continue') {
        Ext.getDom("OK").disabled = true;
        Ext.getDom("Cancel").disabled = true;
        EntityForm.submit();
    }

}