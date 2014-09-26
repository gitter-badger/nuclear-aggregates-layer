﻿window.InitPage = function () {
    window.Card.on("beforebuild", function (card) {
        this.ChangeOwner = function () {
            if (Ext.getDom("Id").value != 0) {
                var params = "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/GroupOperation/Assign/Deal";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            }
        };

        this.ReopenDeal = function () {
            if (Ext.getDom("Id").value != 0) {
                var params = "dialogWidth:450px; dialogHeight:200px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/Deal/ReopenDeal";
                var result = window.showModalDialog(sUrl, Ext.getDom("Id").value, params);
                if (result === true) {
                    this.refresh(true);
                }
            }

        };

        this.CloseDeal = function () {
            if (Ext.getDom("Id").value != 0) {
                var params = "dialogWidth:450px; dialogHeight:450px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/Deal/Close";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            }
        };

        this.ChangeDealClient = function () {
            if (Ext.getDom("Id").value != 0) {
                var params = "dialogWidth:450px; dialogHeight:200px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/GroupOperation/ChangeClient/Deal";
                var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
                if (result === true) {
                    this.refresh(true);
                }
            }
        };

        this.AutoFillDealName = function () {
            if (Ext.getDom("Name").value.length == 0 && Ext.getCmp("Client").item && Ext.getCmp("MainFirm").item) {
                var clientName = Ext.getCmp("Client").item.name;
                var mainFirmName = Ext.getCmp("MainFirm").item.name;

                var response = window.Ext.Ajax.syncRequest({
                    method: 'POST',
                    url: '/Deal/GenerateDealName',
                    params: { clientName: clientName, mainFirmName: mainFirmName }
                });

                if ((response.conn.status >= 200 && response.conn.status < 300)
                    || (Ext.isIE && response.conn.status == 1223)) {
                    var responseDecoded = window.Ext.decode(response.conn.responseText);
                    if (responseDecoded) {
                        Ext.getDom("Name").value = responseDecoded;
                    }
                }
                else {
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: response.conn.responseText,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    return false;
                }
            }
        };
    });

    window.Card.on("afterbuild", function (card) {
        if (Ext.getDom("Id").value == 0 && Ext.getDom("Message").innerHTML.trim() == "") {

            var isReasonSet = false;

            // Если по данному клиенту существует задача с признаком 
            // "Теплый клиент", то, не спрашивая, выставляем 
            // соответствующую причину сделки.
            var clientLookup = Ext.getCmp('Client');
            if (clientLookup.item) {
                var clientId = clientLookup.item.id;

                var response = window.Ext.Ajax.syncRequest({
                    method: 'POST',
                    url: '/Deal/CheckIsWarmClient',
                    params: { clientId: clientId }
                });

                if ((response.conn.status >= 200 && response.conn.status < 300)
                    || (Ext.isIE && response.conn.status == 1223)) {
                    var responseDecoded = window.Ext.decode(response.conn.responseText);
                    if (responseDecoded && responseDecoded.IsWarmClient == true) {
                        isReasonSet = true;
                        // Hardcode for ReasonForNewDeal.WarmClient:
                        Ext.getDom("StartReason").value = "WarmClient";
                        var dateFormatted = responseDecoded.TaskActualEnd ?
                            responseDecoded.TaskActualEnd.format(Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern) : "";

                        //"Выставлена причина сделки \"Теплый клиент\", т.к. для данного клиента существует задача \"NNN\", закрытая \"\" с типом \"Теплый клиент\"";
                        var alertText = String.format(Ext.LocalizedResources.IsWarmClientWarningFormat, responseDecoded.TaskDescription, dateFormatted);
                        alert(alertText);
                    }
                }
                else {
                    alert(response.conn.responseText);
                    return false;
                }
            }

            if (isReasonSet == false) {
                var params = "dialogWidth:450px; dialogHeight:200px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/Deal/PickCreateReason";
                var result = window.showModalDialog(sUrl, null, params);
                if (result) {
                    Ext.getDom("StartReason").value = result.ReasonForNewDeal;
                    this.AutoFillDealName();
                } else {
                    window.close();
                }
            }
        }

        if (window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {
            this.Items.TabPanel.add(
            {
                xtype: "actionshistorytab",
                pCardInfo:
                {
                    pTypeName: this.Settings.EntityName,
                    pId: window.Ext.getDom("ViewConfig_Id").value
                }
            });
        }
    });

    // if client have changed, change the main firm
    window.Card.on("afterbuild", function (card) {
        Ext.getCmp("Client").on("change", function () {
            var cmpFrm = Ext.getCmp("MainFirm");
            cmpFrm.clearValue();
            if (this.getValue() != undefined) {
                cmpFrm.getDataFromServer();
            }

            window.Card.AutoFillDealName();
        });

        Ext.getCmp("MainFirm").on("change", function () {
            window.Card.AutoFillDealName();
        });
    });

}