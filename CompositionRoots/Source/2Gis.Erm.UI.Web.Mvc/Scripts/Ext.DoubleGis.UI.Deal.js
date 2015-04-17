Ext.DoubleGis.CustomValidatorRegistry["validateAgencyFeePercent"] = function () {
    var discountPercent = Ext.getDom('AgencyFee');
    if (discountPercent) {
        var formatedPercent = Number.parseFromLocal(discountPercent.value);
        return (isNaN(formatedPercent) || (formatedPercent >= 0 && formatedPercent <= 100));
    };
    return true;
};

window.InitPage = function () {
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
            // "Горячий клиент", то, не спрашивая, выставляем
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

                        //"Выставлена причина сделки \"Горячий клиент\", т.к. для данного клиента существует задача \"NNN\", закрытая \"\" с типом \"Горячий клиент\"";
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

    Ext.apply(this, {
        BuildLegalPersonsList: function () {
            if (this.form.Id.value != 0) {
                var cnt = Ext.getCmp('ContentTab_holder');
                var tp = Ext.getCmp('TabWrapper');

                tp.anchor = "100%, 60%";
                delete tp.anchorSpec;
                cnt.add(new Ext.Panel({
                    id: 'legalPersonsFrame_holder',
                    anchor: '100%, 40%',
                    html: '<iframe id="legalPersonsFrame_frame"></iframe>'
                }));
                cnt.doLayout();
                var mask = new window.Ext.LoadMask(window.Ext.get("legalPersonsFrame_holder"));
                mask.show();
                var iframe = Ext.get('legalPersonsFrame_frame');

                iframe.dom.src = '/Grid/View/LegalPersonDeal/Deal/{0}/{1}/LegalPerson?extendedInfo=filterToParent%3Dtrue'.replace(/\{0\}/g, this.form.Id.value).replace(/\{1\}/g, this.ReadOnly ? 'Inactive' : 'Active');
                iframe.on('load', function (evt, el) {
                    el.height = Ext.get(el.parentElement).getComputedHeight();
                    el.width = Ext.get(el.parentElement).getComputedWidth();
                    el.style.height = "100%";
                    el.style.width = "100%";
                    el.contentWindow.Ext.onReady(function () {
                        el.contentWindow.IsBottomLegalPersonDataList = true;
                    });
                    this.hide();
                }, mask);
                cnt.doLayout();
            }
        }
    });

    this.on("afterbuild", this.BuildLegalPersonsList, this);

    this.on("afterrelatedlistready", function (card, details) {
        var dataListName = details.dataList.currentSettings.Name;

        if (dataListName === 'LegalPersonDeal') {
            var dataListWindow = details.dataList.ContentContainer.container.dom.document.parentWindow;
            if (dataListWindow.IsBottomLegalPersonDataList) {
                dataListWindow.Ext.getDom('Toolbar').style.display = 'none';
                details.dataList.Items.Grid.getBottomToolbar().hide();
                details.dataList.ContentContainer.doLayout();
            }
        }
    }, this);
}
