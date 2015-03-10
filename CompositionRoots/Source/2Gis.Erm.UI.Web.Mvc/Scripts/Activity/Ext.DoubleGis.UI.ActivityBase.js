Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.ActivityBase = Ext.extend(Ext.DoubleGis.UI.Card, {
    contactField: null,
    contactComp: null,
    contactRelationController: null,
    reagrdingObjectController: null,
    autoHeader: {
        prefix: null,
        suffix: null,
        build: function () {
            return (this.suffix ? this.prefix + ' - ' + this.suffix : this.prefix) || "";
        }
    },
    getComboboxText: function (name) {
        var element = Ext.get(name);
        if (element) {
            var dom = element.dom;
            return dom.options[dom.selectedIndex].text;
        }
        return null;
    },
    getTitlePrefix : function() {
        return Ext.get("ClientName").getValue();
    },
    getTitleSuffix : function() {
        return null;
    },

    constructor: function (config) {
        Ext.DoubleGis.UI.ActivityBase.superclass.constructor.call(this, config);

        this.contactField = config.contactField;
        this.contactComp = config.contactComponent;
    },
    autocompleteHeader: function () {
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
    postOperation: function (operationName) {
        this.Items.Toolbar.disable();
        this.submitMode = this.submitModes.SAVE;
        var operationUrl = this.EvaluateOperationUrl(operationName) + "/" + this.form.Id.value;
        window.Ext.Ajax.syncRequest({
            timeout: 1200000,
            url: operationUrl,
            method: 'POST',
            scope: this,
            success: this.refresh,
            failure: this.postFormFailure
        });
    },
    ChangeStatus: function (operation)
    {
        var currentIsDirty = this.isDirty;
        if (currentIsDirty)
        {
            this.Items.Toolbar.disable();
            this.submitMode = this.submitModes.SAVE;
            if (this.fireEvent('beforepost', this) && this.normalizeForm()) {
                this.postForm();                
                this.on('postformsuccess', function () { this.postOperation(operation); });
            }
            else {
                this.recalcDisabling();                
                this.isDirty = currentIsDirty;
            }
        }
        else
        {
            this.postOperation(operation);
        }
    },
    EvaluateOperationUrlTemplate: function (operationName) {
        return String.format("{0}{1}.svc/Rest/", Ext.BasicOperationsServiceRestUrl, operationName) + "{0}";
    },
    EvaluateOperationUrl: function (operationName) {
        return String.format(this.EvaluateOperationUrlTemplate(operationName), this.EntityName);
    },    
    CompleteActivity: function () {        
        this.ChangeStatus("Complete");
    },
    CancelActivity: function () {        
        this.ChangeStatus("Cancel");
    },
    ReopenActivity: function() {        
        this.ChangeStatus("Reopen");
    },
    Assign: function () {
        if (!this.checkDirty()) return;
        var result = window.showModalDialog("/GroupOperation/Assign/" + this.EntityName, [this.form.Id.value],
            "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no; resizable:no;");
        if (result) {
            this.refresh(true);
        }
    },
    Build : function() {
        Ext.DoubleGis.UI.ActivityBase.superclass.Build.call(this);

        Ext.getCmp("Client").on("change", this.autocompleteHeader, this);

        if (this.contactField && this.contactComp) {
            this.contactRelationController = new Ext.DoubleGis.UI.ContactRelationController({ contactField: this.contactField, contactComponent: this.contactComp });
        }
        this.reagrdingObjectController = new Ext.DoubleGis.UI.RegardingObjectController(this);

        this.autocompleteHeader();
    }
});
