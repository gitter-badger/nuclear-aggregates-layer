﻿Ext.DoubleGis.CustomValidatorRegistry["validateBeginDistributionDate"] = function () {
    var orderId = Ext.getDom("Id").value;
    var cl = Ext.getCmp("BeginDistributionDate");
    var sourceOrganizationUnitId = Ext.getDom('SourceOrganizationUnitId').value;
    var destinationOrganizationUnitId = Ext.getDom('DestinationOrganizationUnitId').value;
    var beginDistributionDate = cl.getValue() ? cl.getValue().format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern) : "";

    var result = true;
    var messageId = "validateBeginDistributionDateError";

    if (!Ext.isEmpty(beginDistributionDate)) {
        var response = window.Ext.Ajax.syncRequest({
            method: 'POST',
            url: '/Order/CheckBeginDistributionDate',
            params: {
                orderId: orderId,
                beginDistributionDate: beginDistributionDate,
                sourceOrganizationUnitId: Ext.isEmpty(sourceOrganizationUnitId) ? 0 : sourceOrganizationUnitId,
                destinationOrganizationUnitId: Ext.isEmpty(destinationOrganizationUnitId) ? 0 : destinationOrganizationUnitId
            }
        });
        if ((response.conn.status >= 200 && response.conn.status < 300) || (Ext.isIE && response.conn.status == 1223)) {
            window.Card.RemoveNotification(messageId);
        } else {
            window.Card.AddNotification(response.conn.responseText, 'CriticalError', messageId);
            result = response.conn.responseText;
        }
    }
    else {
        var validationText = Ext.LocalizedResources.ValueCantBeEmpty;
        window.Card.AddNotification(validationText, 'CriticalError', messageId);
        result = validationText;
    }
    return result;
};
Ext.DoubleGis.CustomValidatorRegistry["validateDiscountSum"] = function () {
    var discountSum = Ext.getDom('DiscountSum');
    if (discountSum) {
        var formatedSum = Number.parseFromLocal(discountSum.value);
        return (isNaN(formatedSum) || (formatedSum >= 0));
    };
    return true;
};
Ext.DoubleGis.CustomValidatorRegistry["validateDiscountPercent"] = function () {
    var discountPercent = Ext.getDom('DiscountPercent');
    if (discountPercent) {
        var formatedPercent = Number.parseFromLocal(discountPercent.value);
        return (isNaN(formatedPercent) || (formatedPercent >= 0 && formatedPercent <= 100));
    };
    return true;
};
window.InitPage = function () {
    this.renderHeader = false;
    
    Ext.apply(this, CultureSpecificOrder);
    
    Ext.apply(this,
            {
                checkDirty: function () {
                    if (this.form.Id.value == 0) {
                        Ext.Msg.alert('', Ext.LocalizedResources.CardIsNewAlert);
                        return false;
                    }
                    if (this.isDirty) {
                        Ext.Msg.alert('', Ext.LocalizedResources.CardIsDirtyAlert);
                        return false;
                    }
                    return true;
                },
                appendToolbarItems: function () {
                    var toolbar = this.Items.Toolbar;
                    toolbar.addField('->');
                    toolbar.addText(Ext.LocalizedResources.OrderState + ': ');
                    toolbar.addField({
                        xtype: 'combo',
                        id: "orderWorkflow",
                        valueField: 'Value',
                        displayField: 'Text',
                        hiddenName: 'tmpWorkflowStepId',
                        triggerAction: 'all',
                        allowBlank: false,
                        editable: false,
                        forceSelection: true,
                        mode: 'local',
                        disabled: (Ext.getDom('IsWorkflowLocked').value.toLowerCase() == 'true'),
                        listeners: {
                            change: function (field, newValue, oldValue) {
                                if ((newValue != oldValue) && (newValue != Ext.getDom('PreviousWorkflowStepId').value)) {
                                    this.previousOrderState = oldValue;
                                    this.changeOrderState(newValue);
                                }
                            },
                            beforerender: function (cmp) {
                                if (cmp.store.data.length > 0) {
                                    cmp.setValue(cmp.store.getAt(0).data.Value);
                                }
                                if ((this.form.Id.value == 0) || (cmp.store.data.length < 2)) {
                                    cmp.readOnly = true;
                                }
                            },
                            scope: this
                        },

                        store: new Ext.data.JsonStore({
                            fields: [{ name: 'Value', type: 'int' }, 'Text'],
                            data: Ext.decode(Ext.getDom('AvailableSteps').value)
                        })
                    });

                    toolbar.doLayout();
                },
                buildOrderPositionsList: function () {
                    if (this.form.Id.value != 0) {
                        var cnt = Ext.getCmp('ContentTab_holder');
                        var tp = Ext.getCmp('TabWrapper');

                        tp.anchor = "100%, 60%";
                        delete tp.anchorSpec;
                        cnt.add(new Ext.Panel({
                            id: 'positionFrame_holder',
                            anchor: '100%, 40%',
                            html: '<iframe id="positionFrame_frame"></iframe>'
                        }));
                        cnt.doLayout();
                        var mask = new window.Ext.LoadMask(window.Ext.get("positionFrame_holder"));
                        mask.show();
                        var iframe = Ext.get('positionFrame_frame');

                        iframe.dom.src = '/Grid/View/OrderPosition/Order/{0}/{1}?extendedInfo=filterToParent%3Dtrue'.replace(/\{0\}/g, this.form.Id.value).replace(/\{1\}/g, this.ReadOnly ? 'Inactive' : 'Active');
                        iframe.on('load', function (evt, el) {
                            el.height = Ext.get(el.parentElement).getComputedHeight();
                            el.width = Ext.get(el.parentElement).getComputedWidth();
                            el.style.height = "100%";
                            el.style.width = "100%";
                            el.contentWindow.Ext.onReady(function () {
                                el.contentWindow.IsBottomOrderPositionDataList = true;
                            });
                            this.hide();
                        }, mask);
                        cnt.doLayout();
                    }
                },
                setReadonly: function (field, readonly) {
                    if (readonly) {
                        field.addClass('readonly');
                        field.dom.readOnly = 'readonly';
                    } else {
                        field.removeClass('readonly');
                        field.dom.readOnly = '';
                    }
                },
                Request: function (settings) {
                    this.Items.Toolbar.disable(true);
                    Ext.Ajax.request({
                        timeout: 1200000,
                        method: settings.method,
                        url: settings.url,
                        params: settings.params,
                        scope: this,
                        success: settings.success,
                        failure: settings.failure,
                        callback: function () {
                            if (settings.callback) {
                                settings.callback();
                            }
                            this.Items.Toolbar.enable();
                        }
                    });
                },
                Print: function (methodName) {
                    var entityId = Ext.getDom('Id').value;
                    var legalPersonId = Ext.getDom("LegalPersonId").value;

                    if (this.isDirty) {
                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: Ext.LocalizedResources.YouHaveToSaveOrderToPrint,
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                        return;
                    }

                    if (legalPersonId == '') {
                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: Ext.LocalizedResources.YouHaveToChooseLegalPersonToPrintOrder,
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    }

                    if (entityId != '' && entityId != 0 && legalPersonId != '') {
                        var url = '/Order/IsChooseProfileNeeded/' + entityId + '/?printOrderType=' + methodName + '&__dc=' + Ext.util.Format.cacheBuster();
                        Ext.Ajax.request(
                            {
                                url: url,
                                method: 'POST',
                                extinstance: this,
                                success: function (result, opts) {
                                    var jsonData = Ext.decode(result.responseText);
                                    if (!jsonData.IsChooseProfileNeeded) {
                                        opts.extinstance.PrintWithoutProfileChoosing(methodName, jsonData.LegalPersonProfileId);
                                    } else {
                                        opts.extinstance.PrintWithProfileChoosing(methodName);
                                    }
                                },
                                params: { orderId: entityId }
                            });
                    }
                },
                PrintWithoutProfileChoosing: function (methodName, profileId) {
                    var entityId = Ext.getDom('Id').value;
                    url = '/Order/' + methodName + '/' + entityId + '?profileId=' + profileId + '&__dc=' + Ext.util.Format.cacheBuster();
                    this.Items.Toolbar.disable();

                    var iframe;
                    iframe = document.getElementById("hiddenDownloader");
                    if (iframe === null) {
                        iframe = document.createElement('iframe');
                        iframe.id = "hiddenDownloader";
                        iframe.style.visibility = 'hidden';

                        var iframeEl = new Ext.Element(iframe);
                        iframeEl.on("load", function () {
                            var iframeContent = iframe.contentWindow.document.documentElement.innerText;
                            if (iframeContent != "") {
                                alert(iframeContent);
                            }
                        });
                        document.body.appendChild(iframe);
                    }

                    iframe.src = url;
                    this.Items.Toolbar.enable();
                },
                PrintWithProfileChoosing: function (methodName) {
                    var entityId = Ext.getDom('Id').value;
                    url = '/Order/Print/' + methodName + '/' + entityId + '?__dc=' + Ext.util.Format.cacheBuster();
                    var params = "dialogWidth:500px; dialogHeight:250px; status:yes; scroll:no;resizable:no;";
                    window.showModalDialog(url, null, params);
                },
                PrintOrder: function () {
                    this.Print('PrintOrder');
                },
                PrintRegionalOrder: function () {
                    this.Print('PrintRegionalOrder');
                },
                PrintBargain: function () {
                    this.Print('PrintBargain');
                },
                PrintBill: function () {
                    this.Print('PrintBill');
                },
                PrepareJointBill: function () {
                    var entityId = Ext.getDom('Id').value;
                    var url = '/Order/IsChooseProfileNeeded/' + entityId + '/?printOrderType=PrepareJointBill&__dc=' + Ext.util.Format.cacheBuster();
                    Ext.Ajax.request(
                        {
                            url: url,
                            method: 'POST',
                            extinstance: this,
                            success: function (result, opts) {
                                var jsonData = Ext.decode(result.responseText);
                                if (!jsonData.IsChooseProfileNeeded) {
                                    opts.extinstance.PrepareJointBillWithoutProfileChoosing(jsonData.LegalPersonProfileId);
                                } else {
                                    alert('Пожалуйста, распечатайте бланк заказа для определения профиля пользователя.');
                                }
                            },
                            params: { orderId: entityId }
                        });
                },
                PrepareJointBillWithoutProfileChoosing: function (profileId) {
                    var url = "/Order/PrepareJointBill/?id=" + Ext.getDom('Id').value + '&profileId=' + profileId;
                    var params = "dialogWidth:780px; dialogHeight:350px; status:yes; scroll:no;resizable:no;";
                    window.showModalDialog(url, null, params);
                    this.refresh();
                },
                PrintTerminationNotice: function () {
                    this.Print('PrintTerminationNotice');
                },
                PrintRegionalTerminationNotice: function () {
                    this.Print('PrintRegionalTerminationNotice');
                },
                PrintAdditionalAgreement: function () {
                    this.Print('PrintAdditionalAgreement');
                },
                PrintReferenceInformation: function () {
                    this.Print('PrintReferenceInformation');
                },
                PrintLetterOfGuarantee: function () {
                    this.Print('PrintLetterOfGuarantee');
                },
                CreateBargain: function () {
                    if (!this.checkDirty()) return;
                    this.updateBargain(false); //Попыта получить договор на случай если он уже есть
                    var bargain = Ext.getCmp('Bargain').getValue();
                    var self = this;
                    if (!bargain) {
                        var progressWindow = Ext.MessageBox.wait(Ext.LocalizedResources.BargainCreationIsInProgress, Ext.LocalizedResources.BargainCreation);

                        Ext.Ajax.request({
                            method: 'POST',
                            url: '/Bargain/CreateBargainForOrder',
                            params: { orderId: this.form.Id.value },
                            success: function (xhr) {
                                progressWindow.hide();
                                var response = Ext.decode(xhr.responseText);

                                Ext.DoubleGis.Global.Helpers.ShowEntityLink({
                                    title: Ext.LocalizedResources.BargainCreation,
                                    msg: Ext.LocalizedResources.BargainIsCreated,
                                    buttons: Ext.Msg.OK,
                                    icon: Ext.MessageBox.INFO,
                                    entityName: 'Bargain',
                                    entityId: response.BargainId,
                                    entityDescription: response.BargainNumber,
                                    fn: function () { self.refresh(true); }
                                });
                            },
                            failure: function () {
                                progressWindow.hide();
                                Ext.Msg.show({
                                    title: Ext.LocalizedResources.Error,
                                    msg: Ext.LocalizedResources.ApplicationError,
                                    buttons: Ext.Msg.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                        });
                    }
                },
                RemoveBargain: function () {
                    if (!this.checkDirty()) return;
                    this.Request({
                        method: 'POST',
                        url: '/Order/GetBargainRemovalConfirmation',
                        params: { orderId: this.form.Id.value },
                        scope: this,
                        success: function (xhr) {
                            var message = Ext.decode(xhr.responseText);
                            Ext.MessageBox.confirm(Ext.LocalizedResources.AreYouSureWantToDeleteBargain, message, function (btn) {
                                if (btn == 'yes') {
                                    this.Request({
                                        method: 'POST',
                                        url: '/Order/RemoveBargain',
                                        params: { orderId: this.form.Id.value },
                                        success: function () {
                                            this.refresh();
                                        },
                                        failure: function (xhr) {
                                            alert(xhr.responseText);
                                        }
                                    });
                                }
                            }, this);
                        },
                        failure: function (xhr) {
                            alert(xhr.responseText);
                        }
                    });
                },
                ChangeDeal: function () {
                    if (!this.checkDirty()) return;
                    var params = "dialogWidth:450px; dialogHeight:200px; status:yes; scroll:no; resizable:no; ";
                    var sUrl = "/Order/ChangeOrderDeal?orderId=" + this.form.Id.value;
                    var result = window.showModalDialog(sUrl, this.form.Id.value, params);
                    if (result === true) {
                        this.refresh(true);
                    }
                },
                ChangeOwner: function () {
                    if (!this.checkDirty()) return;
                    var params = "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no; resizable:no; ";
                    var sUrl = "/GroupOperation/Assign/Order";
                    var result = window.showModalDialog(sUrl, [this.form.Id.value], params);
                    if (result === true) {
                        this.refresh(true);
                    }
                },
                SwitchToAccount: function () {
                    var accountId = Ext.getDom('AccountId').value;
                    if (!accountId) {
                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: Ext.LocalizedResources.OrderIsNotRelatedToAccount,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR
                        });
                        return;
                    }

                    Ext.DoubleGis.Global.Helpers.OpenEntity("Account", accountId);
                },
                CheckOrder: function () {
                    if (!this.checkDirty()) return;
                    this.CheckManager.performManualCheck();
                },
                CopyOrder: function () {
                    var div = document.createElement('div');
                    document.body.appendChild(div);
                    var workflowStep = Ext.getDom('WorkflowStepId').value;
                    var win = new Ext.DoubleGis.Order.CopyOrderDialog({
                        target: Ext.get(div),
                        isTerminationAllowed: workflowStep == 4, // only OnTermination
                        orderId: this.form.Id.value
                    });
                    win.show();
                },

                acquireAndProcessOrderAggregate: function (aggregateProcessDelegate) {
                    Ext.Ajax.request({
                        timeout: 1200000,
                        method: 'GET',
                        url: '/Order/GetOrderAggregateInCurrentState/' + this.form.Id.value,
                        params: { id: this.form.Id.value },
                        scope: this,
                        success: function (xhr) {
                            var response = Ext.decode(xhr.responseText);
                            aggregateProcessDelegate(response, this);
                        },
                        failure: function () {
                            Ext.Msg.show({
                                title: Ext.LocalizedResources.Error,
                                msg: Ext.LocalizedResources.AcquireOrderExtensionFailed,
                                buttons: Ext.Msg.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        }
                    });
                },
                fillOrderAggregateFields: function (orderAggregate, card) {
                    //Do NOT use SetValue in this method, because the card shouldn't become dirty

                    // updating EntityStateToken
                    card.form.EntityStateToken.value = orderAggregate.EntityStateToken,

                    card.form.BudgetType.value = orderAggregate.BudgetType;
                    card.form.DiscountReason.value = orderAggregate.DiscountReason;
                    // prevent setting of "null"
                    card.form.OrderNumber.value = (orderAggregate.Order.Number == null) ? "" : orderAggregate.Order.Number;;
                    card.form.RegionalNumber.value = (orderAggregate.Order.RegionalNumber == null) ? "" : orderAggregate.Order.RegionalNumber;
                    card.form.DiscountComment.value = (orderAggregate.Order.DiscountComment == null) ? "" : orderAggregate.Order.DiscountComment;
                    card.form.Platform.value = (orderAggregate.Platform == null) ? "" : orderAggregate.Platform;

                    // numbers
                    card.form.PayablePrice.value = Number.formatToLocal(orderAggregate.Order.PayablePrice);
                    card.form.PayablePlan.value = Number.formatToLocal(orderAggregate.Order.PayablePlan);
                    card.form.PayableFact.value = Number.formatToLocal(orderAggregate.Order.PayableFact);
                    card.form.VatPlan.value = Number.formatToLocal(orderAggregate.Order.VatPlan);
                    card.form.AmountToWithdraw.value = Number.formatToLocal(orderAggregate.Order.AmountToWithdraw);
                    card.form.AmountWithdrawn.value = Number.formatToLocal(orderAggregate.Order.AmountWithdrawn);
                    card.form.DiscountPercent.value = Number.formatToLocal(orderAggregate.Order.DiscountPercent);
                    card.form.DiscountSum.value = Number.formatToLocal(orderAggregate.Order.DiscountSum);
                    card.form.PlatformId.value = (orderAggregate.Platform == null) ? "" : orderAggregate.Order.PlatformId;

                    //For the DiscountPercent and DiscountSum fields things are as follows:
                    //1. When a user changes one of them, the card should become dirty;
                    //2. When they are changed by the code below, the card shouldn't become dirty.
                    //Hence, we need to place a hack here:
                    var cardWasDirty = card.isDirty;
                    Ext.get('DiscountPercent').setValue(Number.formatToLocal(orderAggregate.Order.DiscountPercent));
                    Ext.get('DiscountSum').setValue(Number.formatToLocal(orderAggregate.Order.DiscountSum));

                    // percents checked if all positions checked
                    if (orderAggregate.DiscountInPercents)
                        Ext.getDom('DiscountPercentChecked').click();
                    else
                        Ext.getDom('DiscountSumChecked').click();

                    this.refreshDiscountRelatedAvailability();

                    card.isDirty &= cardWasDirty;
                },
                updateBargain: function (showAlert) {
                    // Обновление отделения организации юр лица исполнителя
                    var legalPerson = Ext.getCmp('LegalPerson').getValue();
                    var branchOfficeOrganizationUnit = Ext.getCmp('BranchOfficeOrganizationUnit').getValue();
                    var bargain = Ext.getCmp('Bargain');

                    if (bargain.item) {
                        bargain.clearValue();
                    }

                    var currentOrderSignupDate = Ext.getCmp("SignupDate").getValue();
                    var currentOrderSignupDateText = currentOrderSignupDate ? currentOrderSignupDate.format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern) : currentOrderSignupDate;

                    // Для юр. лица исполнителя и юр. лица клиента производим поиск 
                    // договора, действующего на дату подписания заказа.
                    if (legalPerson && branchOfficeOrganizationUnit) {
                        this.Items.Toolbar.disable();
                        var bargainInfoResponse = window.Ext.Ajax.syncRequest({
                            method: 'POST',
                            url: '/Bargain/GetBargain',
                            params: { branchOfficeOrganizationUnitId: branchOfficeOrganizationUnit.id, legalPersonId: legalPerson.id, orderSignupDate: currentOrderSignupDateText }
                        });

                        if ((bargainInfoResponse.conn.status >= 200 && bargainInfoResponse.conn.status < 300) || (Ext.isIE && bargainInfoResponse.conn.status == 1223)) {
                            var bargainInfo = Ext.decode(bargainInfoResponse.conn.responseText);

                            if (bargainInfo) {
                                var bargainClosedOn = bargainInfo.BargainClosedOn;

                                if (!bargainClosedOn || bargainClosedOn >= currentOrderSignupDate) {
                                    bargain.setValue({ id: bargainInfo.Id, name: bargainInfo.BargainNumber });
                                } else {
                                    if (showAlert)
                                        alert(Ext.LocalizedResources.CloseBargains_CurrentBargainIsObsolete);
                                }
                            }
                            this.Items.Toolbar.enable();
                        }
                        else {
                            alert(bargainInfoResponse.conn.responseText);
                        }
                    }

                    this.refreshBargainButtons();
                },
                getMenuItem: function () {
                    var menu = this.Items.Toolbar;
                    var item = null;
                    for (var i = 0; i < arguments.length; i++) {
                        item = menu.items.map[arguments[i]];
                        if (!item) {
                            throw 'Элемент меню ' + arguments[i] + ' не найден';
                        }
                        menu = item.menu;
                    }

                    return item;
                },
                CloseWithDenial: function () {
                    if (!this.checkDirty()) {
                        return;
                    }
                    var url = "/" + this.EntityName + "/CloseWithDenial/" + this.form.Id.value;
                    var result = window.showModalDialog(url, {}, "dialogWidth:500px; dialogHeight:203px; scroll:no;resizable:no;");
                    if (result === true) {
                        this.refresh();
                    }
                },
                changeOrderState: function (newState) {
                    if (!this.checkDirty()) {
                        this.SetPreviousState(this.previousOrderState);
                        return;
                    }

                    this.Items.Toolbar.disable();
                    this.Items.Toolbar.findById("orderWorkflow").enable();
                    Ext.getDom('WorkflowStepId').value = newState;

                    var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 250 + "px; status:yes; scroll:no;resizable:no;";
                    var url = '/Order/ChangeState'
                        + '?orderId=' + this.form.Id.value
                            + '&oldState=' + this.previousOrderState
                                + '&newState=' + newState
                                    + '&inspectorId=' + Ext.getDom("InspectorId").value
                                        + '&sourceOrgUnitId=' + Ext.getDom('SourceOrganizationUnitId').value;

                    var result = window.showModalDialog(url, null, params); //окно запроса дополнительных параметров (куратора, причины расторжения...)

                    //если все успешно обновилось, то запрашиваю окно проверки заказа
                    if (result) {
                        switch (newState) {
                            case 2:
                                //На утверждении
                                Ext.getCmp('Inspector').setValue({ id: result.InspectorId, name: result.InspectorName });
                                break;
                            case 4:
                                //На расторжении
                                Ext.get('TerminationReason').setValue(result.TerminationReason);
                                Ext.get('Comment').setValue(result.TerminationReasonComment);
                                break;
                            default:
                                break;
                        }
                        this.CheckManager.validateStateChangeAsync(newState);
                    }
                    else {
                        this.SetPreviousState(this.previousOrderState); //если окно упало, то откатываю статус назад
                        this.Items.Toolbar.enable();
                    }
                },
                onCheckManagerValidationCompleted: function (proceed) {
                    this.Items.Toolbar.enable();
                    if (proceed) {
                        Ext.fly('ViewConfig_ReadOnly').setValue(false);
                        var newState = Ext.getDom('WorkflowStepId').value;

                        this.on('postformsuccess', function (card, form) {
                            if (!form.Message || form.MessageType == "Warning" || form.MessageType == "Info") {
                                this.isDirty = false;
                                this.refresh(true);
                                return false;
                            }
                        }, this, { single: true });

                        if (newState == 2) {
                            this.acquireAndProcessOrderAggregate(function (orderAggregate, card) {
                                // Костыль для подпирания другого костыля: подтягиваем обновленный EntityStateToken из базы, т.к. 
                                // в случае смене статуса "на утверждении" мы перед валидацией обновили в базе поле InspectorCode
                                // чтобы правильно отрабатывала проверка заказа (((.
                                card.form.EntityStateToken.value = orderAggregate.EntityStateToken;
                                card.Save();
                            });
                        }
                        else {
                            this.Save();
                        }
                    }
                    else {
                        // При откате к предыдущему состоянию может образоваться рассинхронизация между данными в БД и данными формы.
                        // Например, при смене статуса в "На одобрении" в базе у заказа выставляется InspectorCode и при 
                        // непрохождении валидации не откатывается назад (костыль же).
                        this.SetPreviousState(this.previousOrderState);
                        this.isDirty = false;
                        this.refresh(true);
                    }
                },
                onCheckManagerError: function (errorText) {
                    this.Items.Toolbar.enable();
                    this.AddNotification(errorText, 'CriticalError', "ServerError");
                },
                SetPreviousState: function (prevState) {
                    this.Items.Toolbar.findById("orderWorkflow").setValue(prevState);
                    Ext.getDom('WorkflowStepId').value = prevState;
                },
                refreshReleaseDistributionInfo: function () {
                    //Обновление номеров выпусков и дат размещения
                    if (!Ext.getCmp('DestinationOrganizationUnit').item)
                        return;

                    var releaseCountPlan = parseInt(Ext.getDom("ReleaseCountPlan").value);
                    var destOrgUnitId = Ext.getCmp('DestinationOrganizationUnit').item.id;
                    var beginDistrDate = Ext.getCmp("BeginDistributionDate").getValue();

                    if (releaseCountPlan < 1 || releaseCountPlan > 99)
                        return;

                    this.Items.Toolbar.disable();
                    var orderInfoResponse = window.Ext.Ajax.syncRequest({
                        method: 'POST',
                        url: '/Order/GetReleasesNumbers',
                        params: { organizationUnitid: destOrgUnitId, beginDistributionDate: beginDistrDate, releaseCountPlan: releaseCountPlan }
                    });

                    if ((orderInfoResponse.conn.status >= 200 && orderInfoResponse.conn.status < 300) || (Ext.isIE && orderInfoResponse.conn.status == 1223)) {
                        var orderInfo = window.Ext.decode(orderInfoResponse.conn.responseText);
                        if (orderInfo) {
                            Ext.get("BeginReleaseNumber").setValue(orderInfo.BeginReleaseNumber);
                            Ext.get("EndReleaseNumberPlan").setValue(orderInfo.EndReleaseNumberPlan);
                            Ext.get("EndReleaseNumberFact").setValue(orderInfo.EndReleaseNumberFact);

                            Ext.getCmp("BeginDistributionDate").setRawValue(new Date(orderInfo.BeginDistributionDate));
                            Ext.getCmp("EndDistributionDatePlan").setRawValue(new Date(orderInfo.EndDistributionDatePlan));
                            Ext.getCmp("EndDistributionDateFact").setRawValue(new Date(orderInfo.EndDistributionDateFact));
                        }
                        this.Items.Toolbar.enable();
                    }
                    else {
                        alert(orderInfoResponse.conn.responseText);
                        return;
                    }

                },
                toFixedWithoutRounding: function (figure, decimals) {
                    if (!decimals) decimals = 2;
                    var d = Math.pow(10, decimals);
                    return (parseInt(figure * d) / d).toFixed(decimals);
                },
                discountRecalc: function () {
                    var discountPercent = Ext.get('DiscountPercent');
                    var discountSum = Ext.get('DiscountSum');
                    var orderType = Ext.get('OrderType');

                    discountPercent.dom.disabled = 'disabled';
                    discountSum.dom.disabled = 'disabled';
                    this.Items.Toolbar.disable();
                    var newDiscountInfoResponse = window.Ext.Ajax.syncRequest({
                        method: 'POST',
                        url: '/Order/DiscountRecalc',
                        params: {
                            orderId: this.form.Id.value,
                            releaseCountFact: Ext.getDom('ReleaseCountFact').value,
                            inPercents: Ext.getDom('DiscountPercentChecked').checked,
                            discountPercent: discountPercent.dom.value,
                            discountSum: discountSum.dom.value,
                            orderType: orderType.dom.value
                        },
                        scope: this
                    });

                    if ((newDiscountInfoResponse.conn.status >= 200 && newDiscountInfoResponse.conn.status < 300) || (Ext.isIE && newDiscountInfoResponse.conn.status == 1223)) {
                        this.Items.Toolbar.enable();
                        discountPercent.dom.disabled = null;
                        discountSum.dom.disabled = null;

                        var newDiscountInfo = window.Ext.decode(newDiscountInfoResponse.conn.responseText);
                        if (newDiscountInfo) {
                            var discountPercentDecimalDigits = 4;
                            var newDiscountPercent = Number.formatToLocal(this.toFixedWithoutRounding(newDiscountInfo.CorrectedDiscountPercent, discountPercentDecimalDigits));
                            var newDiscountSum = Number.formatToLocal(newDiscountInfo.CorrectedDiscountSum.toFixed(this.Settings.DecimalDigits));
                            discountPercent.setValue(newDiscountPercent);
                            discountSum.setValue(newDiscountSum);

                            var discountReason = Ext.get('DiscountReason');
                            var discountComment = Ext.get('DiscountComment');



                            if (newDiscountInfo.CorrectedDiscountSum == 0) {
                                discountReason.setValue('None');
                                discountComment.setValue(null);

                                discountReason.dom.disabled = true;
                                discountComment.dom.disabled = true;
                                discountComment.addClass('readonly');
                            } else if (!this.disabled) {
                                discountReason.dom.disabled = false;
                                discountComment.dom.disabled = false;
                                discountComment.removeClass('readonly');
                            }
                        }
                    }
                    else {
                        var response = Ext.decode(newDiscountInfoResponse.conn.responseText);
                        if (response) {
                            this.AddNotification(response.Message, response.MessageType, "ServerError");
                            this.Items.Toolbar.findById("Refresh").enable();
                            this.Items.Toolbar.findById("Close").enable();
                        }
                    }
                },
                discountChecker: function () {
                    var cbp = Ext.getDom('DiscountPercentChecked');
                    if (cbp.checked && !this.ReadOnly) {
                        cbp.click();
                    }
                    var cbs = Ext.getDom('DiscountSumChecked');
                    if (cbs.checked && !this.ReadOnly) {
                        cbs.click();
                    }
                }
            });
    Ext.apply(this, {
        initEventListeners: function () {
            var renderTarget = document.createElement('div');
            renderTarget.style.visibility = 'hidden';
            document.body.appendChild(renderTarget);

            var orderValidationServiceUrl = Ext.getDom('OrderValidationServiceUrl').value;
            this.CheckManager = new Ext.DoubleGis.UI.Order.CheckManager({
                renderTarget: renderTarget,
                orderId: this.form.Id.value,
                orderValidationServiceUrl: orderValidationServiceUrl
            });

            this.appendToolbarItems();
            // Задолженность по документам
            Ext.getCmp("Firm").on("change", this.onFirmChanged, this);
            Ext.getCmp('SourceOrganizationUnit').on("change", this.onSourceOrganizationUnitChanged, this);
            Ext.getCmp('LegalPerson').on("change", function () { this.updateBargain(true); }, this);
            Ext.getCmp('BranchOfficeOrganizationUnit').on("change", function () { this.updateBargain(true); }, this);
            Ext.getCmp("BeginDistributionDate").on("change", function () { this.refreshReleaseDistributionInfo(); }, this);

            // Если для текущей бизнес-модели должны быть заданы дополнительные обработчики событий, задаём их
            if (this.setupCultureSpecificEventListeners) {
                this.setupCultureSpecificEventListeners();
            }

            // Обновление фирмы/юр.лица клиента/договора
            Ext.getCmp('DestinationOrganizationUnit').on("beforechange", function (cmp) {
                if (cmp.getValue()) {
                    this.oldDestOrgUnitId = cmp.getValue().id;
                }
            }, this);
            Ext.getCmp('DestinationOrganizationUnit').on("change", this.onDestinationOrganizationUnit, this);

            this.refreshDiscountRelatedAvailability();

            Ext.fly("ReleaseCountPlan").on("blur", this.onReleaseCountPlanChange, this);
            Ext.fly('ReleaseCountPlan').on('keyup', function () {
                var releaseCountPlan = parseInt(this.form.ReleaseCountPlan.value);
                if (releaseCountPlan > 12) {
                    Ext.get("ReleaseCountPlan").setValue(12);
                } else if (releaseCountPlan < 1) {
                    Ext.get("ReleaseCountPlan").setValue(1);
                } else {
                    Ext.get("ReleaseCountPlan").setValue(isNaN(releaseCountPlan) ? null : releaseCountPlan);
                }
            }, this);

            // #region Discount
            Ext.fly('DiscountPercent').on("focus", function () {
                var cb = Ext.getDom('DiscountPercentChecked');
                if (!cb.checked) {
                    this.oldDiscountPercent = this.form.DiscountPercent.value;
                    cb.click();
                }
            }, this);
            Ext.fly('DiscountPercent').on("blur", function () {
                var newDiscountPercent = this.form.DiscountPercent.value;
                if (!this.ReadOnly && this.oldDiscountPercent !== newDiscountPercent) {
                    this.discountRecalc();
                }
            }, this);
            Ext.fly('DiscountPercentChecked').on("click", function () {
                this.setReadonly(Ext.get('DiscountPercent'), false);
                this.setReadonly(Ext.get('DiscountSum'), true);
            }, this);

            Ext.fly('DiscountSum').on("focus", function () {
                var cb = Ext.getDom('DiscountSumChecked');
                if (!cb.checked) {
                    this.oldDiscountSum = this.form.DiscountSum.value;
                    cb.click();
                }
            }, this);
            Ext.fly('DiscountSum').on("blur", function () {
                var newDiscountSum = this.form.DiscountSum.value;
                if (!this.ReadOnly && this.oldDiscountSum !== newDiscountSum) {
                    this.discountRecalc();
                }
            }, this);

            Ext.fly('DiscountSumChecked').on("click", function () {
                this.setReadonly(Ext.get('DiscountPercent'), true);
                this.setReadonly(Ext.get('DiscountSum'), false);
            }, this);

            this.CheckManager.on("validationCompleted", this.onCheckManagerValidationCompleted, this);
            this.CheckManager.on("error", this.onCheckManagerError, this);
            var self = this;
            this.CheckManager.on("repairOutdatedOrderPositionsCompleted", function () {
                self.refresh();
            });

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
        },

        refreshDiscountRelatedAvailability: function () {
            // Блокируем поля "причина скидки", "комментарий по скидке", если скидка не задана
            var discountReason = Ext.get('DiscountReason');
            var discountComment = Ext.get('DiscountComment');
            var discountPercent = parseFloat(Ext.fly('DiscountPercent').getValue().replace(',', '.'));
            var disabled = window.Ext.getDom("ViewConfig_ReadOnly").checked;

            if (discountPercent == 0 || isNaN(discountPercent)) {
                discountReason.dom.disabled = true;
                discountComment.dom.disabled = true;
                discountComment.addClass('readonly');
            }
            else if (discountPercent > 0 && !disabled) {
                discountReason.dom.disabled = false;
                discountComment.dom.disabled = false;
                discountComment.removeClass('readonly');
            }
        },

        // Обновление Отделения организации юр лица исполнителя/Валюты
        onSourceOrganizationUnitChanged: function (cmp) {

            Ext.getCmp('BranchOfficeOrganizationUnit').clearValue();
            Ext.getCmp('Currency').clearValue();

            if (cmp.getValue()) {
                var sourceOrgUnitId = cmp.getValue().id;
                this.Request({
                    method: 'POST',
                    url: '/Order/GetBranchOfficeOrganizationUnit',
                    params: { organizationUnitid: sourceOrgUnitId },
                    success: function (xhr) {
                        var branchOfficeOrganizationUnitInfo = Ext.decode(xhr.responseText);
                        if (branchOfficeOrganizationUnitInfo) {
                            Ext.getCmp('BranchOfficeOrganizationUnit').setValue({ id: branchOfficeOrganizationUnitInfo.Id, name: branchOfficeOrganizationUnitInfo.Name });
                        }
                    },
                    failure: function (xhr) {
                        alert(xhr.responseText);
                    }
                });

                this.Request({
                    method: 'POST',
                    url: '/Order/GetCurrency',
                    params: { organizationUnitid: sourceOrgUnitId },
                    success: function (xhr) {
                        var currencyInfo = Ext.decode(xhr.responseText);
                        if (currencyInfo) {
                            Ext.getCmp('Currency').setValue({ id: currencyInfo.Id, name: currencyInfo.Name });
                        }
                    },
                    failure: function (xhr) {
                        alert(xhr.responseText);
                    }
                });
            }
        },

        // Проверка значения введенного в поле "планируемое число выпусков"
        onReleaseCountPlanChange: function () {
            if (!this.form.ReleaseCountPlan.readOnly) {
                var releaseCountPlan = parseInt(this.form.ReleaseCountPlan.value);
                if (isNaN(releaseCountPlan) || releaseCountPlan > 12 || releaseCountPlan < 1) {
                    Ext.Msg.alert('', Ext.LocalizedResources.ReleaseCountPlanRangeMessage);
                    return;
                }

                Ext.get("ReleaseCountFact").setValue(releaseCountPlan);

                this.refreshReleaseDistributionInfo();
                this.discountRecalc();
            }
        }
    });


    this.on("afterbuild", this.initEventListeners, this);

    this.on("afterrelatedlistready", function (card, details) {
        var dataListName = details.dataList.currentSettings.Name;
        if (dataListName == "OrderPosition") {
            details.dataList.on("beforecreate", function () {
                var hasPrice = window.Ext.get("HasDestOrganizationUnitPublishedPrice").getValue();
                var destOrgUnitItem = window.Ext.getCmp('DestinationOrganizationUnit').getValue();
                if (destOrgUnitItem && hasPrice && hasPrice.toLowerCase() == "false") {
                    window.Ext.Msg.alert('', Ext.LocalizedResources.PriceForOrganizationUnitNotExists.replace("{0}", destOrgUnitItem.name));
                    return false;
                }

                var orderType = Ext.get("OrderType").getValue();
                var checkResponse = window.Ext.Ajax.syncRequest({
                    method: 'GET',
                    url: '/Order/CanCreateOrderPositionsForOrder',
                    params: { orderId: this.form.Id.value, orderTypeValue: orderType }
                });
                if ((checkResponse.conn.status >= 200 && checkResponse.conn.status < 300) || (Ext.isIE && checkResponse.conn.status == 1223)) {
                    checkResponse = window.Ext.decode(checkResponse.conn.responseText);

                    if (!checkResponse.CanCreate) {
                        window.Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: checkResponse.Message,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR
                        });
                        return false;
                    }
                    return true;
                }
                else {
                    alert(checkResponse.conn.responseText);
                    return false;
                }
            }, this);

            var self = this;
            details.dataList.on("beforerefresh", function () {
                this.acquireAndProcessOrderAggregate(function (aggregate, cardParam) {
                    card.fillOrderAggregateFields(aggregate, cardParam);
                    self.refreshDiscountRelatedAvailability();
                });
            }, this);

            Ext.get("HasAnyOrderPosition").un("change", this.onFieldChange, this);
            Ext.get("MakeReadOnly").un("change", this.onFieldChange, this);

            Ext.get("HasAnyOrderPosition").on("change", function (evt, el) {
                var notActive = Ext.getDom("IsActive").value.toLowerCase() == "false";
                var hasAnyOrderPosition = el.value.toLowerCase() == "true";
                Ext.get("MakeReadOnly").setValue(notActive || hasAnyOrderPosition);
            }, this);

            details.dataList.on("afterrefresh", function () {
                window.Ext.fly("HasAnyOrderPosition").setValue((this.Items.Store.getTotalCount() > 0) ? "true" : "false");

                var records = this.Items.Store.query().items;
                records.sort(function (x, y) { return x.data.PayablePlan - y.data.PayablePlan; });

                var total = 0;
                Ext.each(records, function (rec) {
                    // todo: запилить банковское округление вместо обычного
                    total += parseFloat(rec.data.PayablePlan.toFixed(2));
                });

                var totalText = Ext.util.Format.money(total, Ext.CultureInfo.NumberFormatInfo);
                totalLabel.setText(totalText);
            });

            var dataListWindow = details.dataList.ContentContainer.container.dom.document.parentWindow;
            if (dataListWindow.IsBottomOrderPositionDataList) {
                dataListWindow.Ext.getDom('Toolbar').style.display = 'none';
                details.dataList.Items.Grid.getBottomToolbar().hide();
                details.dataList.ContentContainer.doLayout();
            }


            var tbar = details.dataList.Items.Grid.getTopToolbar();
            Ext.each(tbar.items.items, function (item) {
                item.disabled = window.Ext.getDom("ViewConfig_ReadOnly").checked;
            });
            tbar.addField('->');
            tbar.addText('<strong>' + Ext.LocalizedResources.Total + ': </strong>');
            var totalLabel = tbar.add({ xtype: 'tbtext', id: "totalLabel" });
            tbar.doLayout();
        }
    }, this);
    
    this.on("afterbuild", this.buildOrderPositionsList, this);
    this.on("formbind", this.buildOrderPositionsList, this);
    this.on("afterbuild", this.refreshBargainButtons, this);
    this.on("formbind", this.refreshBargainButtons, this);

    this.on("afterbuild", this.discountChecker, this);
    this.on("formbind", this.discountChecker, this);
    this.on("afterbuild", this.setupMenuAvailability, this);
    this.on("formbind", this.setupMenuAvailability, this);
};
