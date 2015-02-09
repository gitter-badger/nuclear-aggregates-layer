Ext.namespace('Ext.DoubleGis.UI');
Ext.DoubleGis.UI.GroupProcessor = Ext.extend(Ext.util.Observable, {
    Config: {
        Entities: [],
        OperationName: {},
        CloseButtonText: {},
        NeedToSelectOneOrMoreItemsMsg: {},
        ResultMessageTitle: {},
        ResultMessageTemplate: {}
    },
    SuccessStatus: {
        Approved: 0,
        Rejected: 1,
        ReprocessingRequired: 2
    },
    IsCallFromCrm: false,
    ProcessingQueue: [],
    EntitiesCount: -1,
    EntitiesLeft: -1,
    IsSingleEntityProcessing: false,
    SuccessProcessed: 0,
    ErrorProcessed: 0,
    QuantProgress: 0,
    OperationProgressBar: {},
    ResponseMessages: [],
    constructor: function (config) {
        // пример конфига, показывает какие элементы должен содержать конфиг, в коде не используется
        var sampleconfig = {
            Entities: [0, 1, 2], // массив id сущностей
            OperationName: "Qualify", // тип операции - Qualify, Assign, ChangeTerritory
            CloseButtonText: '', // локализованная надпись для кнопки закрыть
            NeedToSelectOneOrMoreItemsMsg: '', // локализованная надпись о том что нужно выбрать один или несколько элементов
            ResultMessageTitle: '', // локализованная надпись - заголовок для результатов операции
            ResultMessageTemplate: '' // локализованная надпись - шаблон строки для результатов операции
        };

        this.addEvents('configspecificcontrols', 'applyusersettings', 'entityprocessingsuccess', 'entityprocessingfail', 'processingfinished');
        Ext.apply(this.Config, config);
        Ext.DoubleGis.UI.GroupProcessor.superclass.constructor.call(this, config);
        this.EntitiesCount = this.Config.Entities.length;
        this.EntitiesLeft = this.EntitiesCount;
        this.QuantProgress = 1 / this.EntitiesCount;
        this.IsSingleEntityProcessing = this.EntitiesCount === 1;

        this.IsCallFromCrm = !(isNumber(this.Config.Entities[0]));

        if (this.Config.listeners) {
            var p, l = this.Config.listeners;
            for (p in l) {
                if (Ext.isFunction(l[p])) {
                    this.on(p, l[p], this);
                }
            }
        }
        if (this.IsCallFromCrm) {
            if (!this.ConvertEntityIds()) {
                return;
            }
        }
    },
    CheckProcessingPossibility: function () {
        if (!this.Config || !this.Config.Entities || !this.Config.Entities.length) {
            Ext.MessageBox.show({
                title: '',
                msg: this.Config.NeedToSelectOneOrMoreItemsMsg,
                buttons: window.Ext.MessageBox.OK,
                fn: function () { window.close(); },
                width: 300,
                icon: window.Ext.MessageBox.ERROR
            });
            return false;
        }

        return true;
    },
    Process: function () {
        
        var topMessage = Ext.get("TopBarMessage").dom;
        topMessage.innerHTML = topMessage.innerHTML.replace(/%0%/g, this.EntitiesCount);

        Ext.getDom("pbDiv").style.visibility = "hidden";

        // write eventhandlers for buttons
        Ext.get("Cancel").on("click", function () { window.close(); });
        Ext.get("OK").on("click", this.SubmitForm, this);

        this.fireEvent('configspecificcontrols');
    },
    EvaluateOperationUrlTemplate: function () {
        return String.format("{0}{1}.svc/Rest/", Ext.BasicOperationsServiceRestUrl, this.Config.OperationName) + "{0}";
    },
    EvaluateOperationUrl: function () {
        return String.format(this.EvaluateOperationUrlTemplate(), this.ResolveEntityName());
    },
    EvaluateConvertIdsUrl: function () {
        return String.format("/{0}/{1}", "GroupOperation", "ConvertToEntityIds");
    },
    SubmitForm: function () {
        if (!this.IsUserSettingsValid()) {
            return;
        };

        this.fireEvent('applyusersettings');

        Ext.getDom("OK").disabled = "disabled";
        Ext.getDom("Cancel").disabled = "disabled";
        Ext.get("Notifications").removeClass("Notifications");

        if (!this.IsSingleEntityProcessing) {
            this.OperationProgressBar = new Ext.ProgressBar({ width: '100%', cls: "custom", id: "pbDiv", applyTo: "pbInner", value: 0 });
            Ext.getDom("pbDiv").style.visibility = "";
        }

        this.ProcessEntities();
    },    
    IsUserSettingsValid: function () { /*переопределить в потомке*/ },
    PreProcessEntities: function () { /*реализация по-умолчанию*/ return true; },
    ConvertEntityIds: function () {
        var entityNamesArray = new Array(this.Config.Entities.length);
        var entityReplicationCodesArray = new Array(this.Config.Entities.length);
        for (var i = 0; i < this.Config.Entities.length; i++) {
            entityNamesArray[i] = this.Config.Entities[i].EntityName;
            entityReplicationCodesArray[i] = this.Config.Entities[i].EntityId;
        }
        var response = window.Ext.Ajax.syncRequest({
            timeout: 1200000,
            url: this.EvaluateConvertIdsUrl(),
            method: 'POST',
            params:
                {
                    entityTypeNames: entityNamesArray,
                    replicationCodes: entityReplicationCodesArray
                }
        });
        if ((response.conn.status >= 200 && response.conn.status < 300) || (Ext.isIE && response.conn.status == 1223)) {
            this.Config.Entities = Ext.decode(response.conn.responseText);
            return true;
        }
        else {
            this.FinishProcessing(response.conn.responseText);
            return false;
        }
    },
    CreateParamsForControllerCall: function (entityId) { /*переопределить в потомке*/
        // что-то похожее должен возвращать код в потомке 
        // return { id: entityId, ownerCode: argOwnerCode }
    },
    ProcessEntities: function () {
        if (this.PreProcessEntities() == true) {
            this.ProcessingQueue = Ext.apply([], this.Config.Entities);
            this.ProcessNextEntityInQueue();
        }
        else {
            this.FinishProcessing();
        }
    },
    ProcessNextEntityInQueue: function () {
        if (this.ProcessingQueue.length != 0) {
            var nextEntity = this.ProcessingQueue.shift();
            var entityName, entityId;
            if (!isNumber(nextEntity))
            {
                entityName = nextEntity.EntityName;
                entityId = nextEntity.Id;
            } else 
            {
                entityId = nextEntity;
                entityName = this.ResolveEntityName(nextEntity);
            }
            var operationUrl = String.format(this.EvaluateOperationUrlTemplate(), entityName);
            var params = this.CreateParamsForControllerCall(entityId);

            this.ProcessSingleEntity(operationUrl, params);
        }
    },
    ProcessSingleEntity: function (operationUrl, params) {
        // Параметры передаем в REST стиле
        for (var index in params) {
            var param = params[index];
            if (param == undefined || param === '') {
                operationUrl += "/null";
            }
            else {
                operationUrl += "/" + param;
            }
        }
        Ext.Ajax.request({
            timeout: 1200000,
            url: operationUrl,
            method: 'POST',
            scope: this,
            callback: this.ProcessNextEntityInQueue,
            success: this.OnSuccessProcessed,
            failure: this.OnFail
        });
    },
	ResolveEntityName: function(entityId) {
		return Ext.getDom("EntityTypeName").value;
	},
    OnSuccessProcessed: function (response) {
        var validatedProcessingStatus = this.ValidateEntryProcessingSuccessStatus(response.responseText);
        if (validatedProcessingStatus == this.SuccessStatus.ReprocessingRequired) {
            // данный экземпляр требуется повторно обработать
            return;
        }

        if (validatedProcessingStatus == this.SuccessStatus.Approved) {   // успешность обработки экземпляра подтверждена 
            if (!this.IsSingleEntityProcessing) {
                this.OperationProgressBar.updateProgress(this.OperationProgressBar.value + this.QuantProgress);
            }

            this.fireEvent('entityprocessingsuccess', response.responseText);

            var result = Ext.decode(response.responseText);
            if (result.Message) {
                this.ResponseMessages.push(result.Message);
            }

            ++this.SuccessProcessed;
            if (--this.EntitiesLeft == 0) {
                this.FinishProcessing();
            }
        }
        else if (validatedProcessingStatus == this.SuccessStatus.Rejected) {   // успешность обработки экземпляра НЕ подтверждена - считаем, что обработка завершилась ошибкой
            this.OnFail(this.AdaptResponseForRejectedSuccessProcessing(response));
        }
    },
    FinishProcessing: function (errorMessage) {
        var isFullSuccessProcessing = this.EntitiesCount === this.SuccessProcessed;
        if (this.IsSingleEntityProcessing) {
            if (errorMessage) {
                this.SetNotification(errorMessage);
            }
        }
        else {
            // FinishProcessing может вызываться когда this.OperationProgressBar не инициирован.
            if (this.OperationProgressBar && this.OperationProgressBar.destroy) {
                this.OperationProgressBar.destroy();
            }

            if (!isFullSuccessProcessing) {
                this.SetNotification(String.format(' ' + this.Config.ResultMessageTitle + '. ' + this.Config.ResultMessageTemplate, this.SuccessProcessed, this.ErrorProcessed));
            }
        }

        var domCancel = Ext.getDom("Cancel");
        domCancel.disabled = '';
        domCancel.value = this.Config.CloseButtonText;
        Ext.get("OK").setVisible(false);

        this.fireEvent('processingfinished');

        window.returnValue = true;
        if (isFullSuccessProcessing) {
            // если все сущности обработаны успешно - тогда закрывем окно,
            // иначе пусть пользователь смотрит или notification area или статус групповой операции и вручную закрывает окно
            // окно закрываем в самом конце при успешной обработке, т.к. есть событие processingfinished в обработчике которого могут делаться разные вещи

            if (this.ResponseMessages.length > 0) {
                this.SetNotification(this.ResponseMessages.join("<p/>"));
            }
            else
            window.close();
        }
    },
    ValidateEntryProcessingSuccessStatus: function (message) {   // реализация по-умолчанию - можно переопределить в потомке для custom проверок
        // для данной проверки не использован event entityprocessingsuccess, т.к. результат проверки нужен не просто true или false, а расширенное значение
        return this.SuccessStatus.Approved;
    },
    AdaptResponseForRejectedSuccessProcessing: function (response) {   // адаптирует response полученный от сервера для якобы успешно обработанного экземпляра
        // к передаче в OnFail - т.к. фактически обработка экземпляра завершилась неудачей
        // для данной проверки не использован event entityprocessingsuccess, т.к. результат проверки нужен не просто true или false, а расширенное значение
        return response;
    },
    OnFail: function (response) {
        var errorDescription = Ext.decode(response.responseText);
        if (!this.IsSingleEntityProcessing) {
            this.OperationProgressBar.updateProgress(this.OperationProgressBar.value + this.QuantProgress);
        }

        this.fireEvent('entityprocessingfail', errorDescription.Message);

        ++this.ErrorProcessed;
        if (--this.EntitiesLeft == 0) {
            this.FinishProcessing(errorDescription.Message);
        }
    },

    SetNotification: function(innerhtml) {
        Ext.getDom("Notifications").innerHTML = innerhtml;
        Ext.get("Notifications").addClass("Notifications");
        Ext.getDom("Notifications").style.display = "block";
    }
});
