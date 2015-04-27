Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.ActivityBase = Ext.extend(Ext.DoubleGis.UI.Card, {
    contactField: null,
    contactComp: null,
    contactRelationController: null,
    reagrdingObjectController: null,
    changeStatusOperation:undefined,
    autoHeader: {
        prefix: null,
        suffix: null,
        build: function () {
            return (this.suffix ? this.prefix + ' - ' + this.suffix : this.prefix) || "";
        }
    },

    constructor: function (config) {
        Ext.DoubleGis.UI.ActivityBase.superclass.constructor.call(this, config);

        this.contactField = config.contactField;
        this.contactComp = config.contactComponent;

        var scope = this;

        function evaluateOperationUrlTemplate(operationName) {
            return String.format("{0}{1}.svc/Rest/", Ext.BasicOperationsServiceRestUrl, operationName) + "{0}";
        }
        function evaluateOperationUrl(operationName) {
            return String.format(evaluateOperationUrlTemplate(operationName), scope.EntityName);
        }
        function postOperation(operationName) {
            scope.Items.Toolbar.disable();
            scope.submitMode = scope.submitModes.SAVE;
            var operationUrl = evaluateOperationUrl(operationName) + "/" + scope.form.Id.value;
            window.Ext.Ajax.syncRequest({
                timeout: 1200000,
                url: operationUrl,
                method: 'POST',
                scope: scope,
                success: scope.refresh,
                failure: scope.postFormFailure
            });
        }       
        function checkDirty() {
            if (scope.form.Id.value == 0) {
            Ext.Msg.alert('', Ext.LocalizedResources.CardIsNewAlert);
            return false;
        }
            if (scope.isDirty) {
            Ext.Msg.alert('', Ext.LocalizedResources.CardIsDirtyAlert);
            return false;
        }
        return true;
        }
        function сhangeStatus(operation)
        {
            var currentIsDirty = scope.isDirty;
            if (currentIsDirty)
            {
                scope.Items.Toolbar.disable();
                scope.submitMode = scope.submitModes.SAVE;
                if (scope.fireEvent('beforepost', scope) && scope.normalizeForm()) {
                    scope.changeStatusOperation = operation;
                    scope.postForm();
                }
                else {
                    scope.recalcDisabling();
                    scope.isDirty = currentIsDirty;
                }
            }
            else
            {
                postOperation(operation);
            }
        }

        this.saveFormSuccess = function() {
            if (scope.changeStatusOperation) {
                postOperation(scope.changeStatusOperation);
                scope.changeStatusOperation = null;
            }
        }

        this.saveFormFailure= function() {
            scope.changeStatusOperation = null;
        }

        this.getComboboxText = function (name) {
            var element = Ext.get(name);
            if (element) {
                var dom = element.dom;
                return dom.options[dom.selectedIndex].text;
            }
            return null;
        }                
        this.CompleteActivity =  function () {        
            сhangeStatus("Complete");
        }
        this.CancelActivity = function () {        
            сhangeStatus("Cancel");
        }
        this.ReopenActivity = function() {        
            сhangeStatus("Reopen");
        }
        this.Assign = function () {
            if (!checkDirty()) return;
        var result = window.showModalDialog("/GroupOperation/Assign/" + this.EntityName, [this.form.Id.value],
            "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no; resizable:no;");
        if (result) {
            this.refresh(true);
        }
        }
       
        
    },
    Build : function() {
        Ext.DoubleGis.UI.ActivityBase.superclass.Build.call(this);

        Ext.getCmp("Client").on("change",this.autocompleteHeader, this);

        if (this.contactField && this.contactComp) {
            this.contactRelationController = new Ext.DoubleGis.UI.ContactRelationController({ contactField: this.contactField, contactComponent: this.contactComp });
        }
        this.reagrdingObjectController = new Ext.DoubleGis.UI.RegardingObjectController(this);

            this.on('postformsuccess', this.saveFormSuccess);
            this.on('postformfailure', this.saveFormFailure);

        this.autocompleteHeader();
    },
    autocompleteHeader: function() {
        var prefix = this.getTitlePrefix();
        var suffix = this.getTitleSuffix();

        var headerElement = Ext.get("Title");
        var header = headerElement.getValue() || "";

        // Автозаполнение срабатывает если поле "Заголовок" - пустое или ранее было автоматически заполнено (т.е. после автозаполнения не редактировалось пользователем).
        var shouldAutoCompleteHeader = prefix && (!header || header.trim() == this.autoHeader.build().trim());
        this.autoHeader.prefix = prefix;
        this.autoHeader.suffix = suffix;
        if (shouldAutoCompleteHeader) {
            headerElement.setValue(this.autoHeader.build());
        }
    },
    getTitlePrefix: function() {
        return Ext.get("ClientName").getValue();
    },
    getTitleSuffix: function() {
            return null;
    }
    
    
});
