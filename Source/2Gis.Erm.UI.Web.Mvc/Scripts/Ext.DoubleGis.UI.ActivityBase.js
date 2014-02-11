window.InitPage = function() {
    this.renderHeader = false;
    Ext.apply(this,
    {
        initEventListeners: function() {
            var renderTarget = document.createElement('div');
            renderTarget.style.visibility = 'hidden';
            document.body.appendChild(renderTarget);

            this.timepickerFormat = "H:i";

            Ext.getCmp("ScheduledStart").on("change", this.scheduledStartChanged, this);
            Ext.getCmp("ScheduledEnd").on("change", this.scheduledEndChanged, this);

            var purposeElement = Ext.get("Purpose");
            if (purposeElement) {
                purposeElement.on("change", this.purposeChanged, this);
                var purposeCombobox = purposeElement.dom;
                this.PurposeBeforeChanged = purposeCombobox.options[purposeCombobox.selectedIndex].text;
            }

            this.ClientNameBeforeChanged = Ext.fly("ClientName").getValue();

            Ext.getCmp("Client").on("change", this.onClientChanged, this);
            var lookup = Ext.getCmp("Firm");
            lookup.on("afterselect", this.onFirmChanged, this);
            lookup.supressMatchesErrors = true;

            lookup = Ext.getCmp("Contact");
            lookup.on("afterselect", this.onContactChanged, this);
            lookup.supressMatchesErrors = true;

            var timeField = new Ext.ux.TimeComboBox({
                id: "ScheduledStartTime",
                allowBlank: false,
                format: this.timepickerFormat,
                applyTo: "ScheduledStartTime"
            });
            timeField.on("change", this.scheduledStartChanged, this);

            timeField = new Ext.ux.TimeComboBox({
                id: "ScheduledEndTime",
                allowBlank: false,
                format: this.timepickerFormat,
                applyTo: "ScheduledEndTime"
            });

            timeField.on("change", this.scheduledEndChanged, this);

            this.durationComboBox = new Ext.form.ComboBox({
                id: "Duration",
                triggerAction: 'all',
                store: new Ext.data.ArrayStore({
                    id: 1,
                    fields: [
                        'Name',
                        'DurationInMinutes'
                    ],
                    data: this.getDurationComboBoxItems()
                }),
                mode: 'local',
                displayField: 'Name',
                valueField: 'DurationInMinutes',
                editable: false,
                applyTo: "Duration",
                width: 200
            });

            this.durationComboBox.on("change", this.durationChanged, this);

            this.recalculateDuration();

            if (!Ext.fly("Id").getValue() || Ext.fly("Id").getValue() == 0) {
                this.autoCompleteRelatedItems();
            }
            
            {
                var actualEndTimeComboBox = new Ext.ux.TimeComboBox({
                    id: "ActualEndTime",
                    allowBlank: false,
                    format: this.timepickerFormat,
                    applyTo: "ActualEndTime"
                });
                actualEndTimeComboBox.setReadOnly(true);
            }

            Ext.fly("Status").dom.disabled = true;
            Ext.fly("Type").dom.disabled = true;
            if (this.ReadOnly) {
                Ext.getCmp("ScheduledStartTime").setReadOnly(true);
                Ext.getCmp("ScheduledEndTime").setReadOnly(true);
                Ext.getCmp("Duration").setReadOnly(true);
            }
        },
        autoCompleteRelatedItems: function () {
            // Логика для заполнения незаполненных полей "В отношении" для новой сущности.
            if (Ext.fly("ClientId").getValue()) {
                this.onClientChanged();
            }
            else if (Ext.fly("FirmId").getValue()) {
                this.onFirmChanged();
            }
            else if (Ext.fly("ContactId").getValue()) {
                this.onContactChanged();
            }
        },
        scheduledStartChanged: function() {
            this.recalculateEndDate();
        },
        scheduledEndChanged: function() {
            if (!this.recalculateDuration()) {
                var startDate = this.getStartDate();
                if (!startDate) {
                    return;
                }
                this.stopProcessingTimeFieldsEvents = true;
                this.setEndDate(startDate);
                delete(this.stopProcessingTimeFieldsEvents);
            }
        },
        durationChanged: function() {
            this.recalculateEndDate();
        },
        purposeChanged: function() {
            this.autocompleteHeader();
            var purposeCombobox = Ext.get("Purpose").dom;
            this.PurposeBeforeChanged = purposeCombobox.options[purposeCombobox.selectedIndex].text;
        },
        autocompleteHeader: function() {
            if (!Ext.fly("ClientName").getValue()) {
                return;
            }

            // Автозаполнение срабатывает если поле "Заголовок" - пустое или ранее было автоматически заполнено 
            // (т.е. после автозаполнения не редактировалось пользователем).
            var currentHeader = Ext.get("Header").getValue();
            var shouldAutoCompleteHeader = currentHeader.length == 0;

            if (!shouldAutoCompleteHeader) {
                var purpose = this.PurposeBeforeChanged;
                if (!purpose) {
                    var purposeElement = Ext.get("Purpose");
                    var purposeCombobox = purposeElement ? purposeElement.dom : null;
                    purpose = purposeCombobox ? purposeCombobox.options[purposeCombobox.selectedIndex].text : Ext.LocalizedResources.Task;
                }

                var clientName = this.ClientNameBeforeChanged;

                if (!clientName || clientName == "") {
                    clientName = Ext.fly("ClientName").getValue();
                }

                var previousAutocompleteHeader = clientName + " - " + purpose;
                shouldAutoCompleteHeader = currentHeader.trim() == previousAutocompleteHeader.trim();
            }

            if (shouldAutoCompleteHeader) {
                purposeElement = Ext.get("Purpose");
                purposeCombobox = purposeElement ? Ext.get("Purpose").dom : null;
                var currentSelectedPurposeItemText = purposeCombobox ? purposeCombobox.options[purposeCombobox.selectedIndex].text : Ext.LocalizedResources.Task;
                Ext.get("Header").setValue(Ext.fly("ClientName").getValue() + " - " + currentSelectedPurposeItemText);
            }

        },
        checkDirty: function() {
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
        recalculateEndDate: function() {
            if (this.stopProcessingTimeFieldsEvents) {
                return;
            }
            var startDate = this.getStartDate();
            if (!startDate) {
                return;
            }
            var durationInMinutes = this.durationComboBox.getValue();
            this.stopProcessingTimeFieldsEvents = true;
            this.setEndDate(startDate.add("mi", parseInt(durationInMinutes)));
            delete(this.stopProcessingTimeFieldsEvents);
        },
        recalculateDuration: function() {
            if (this.stopProcessingTimeFieldsEvents) {
                return 1;
            }
            var startDate = this.getStartDate();
            var endDate = this.getEndDate();
            
            if (!startDate || !endDate) {
                return 0;
            }
            var diff = endDate.getTime() - startDate.getTime();

            if (diff < 0) {
                diff = 0;
            }

            diff = diff / 60000;
            this.stopProcessingTimeFieldsEvents = true;
            this.durationComboBox.setValue(diff);
            this.durationComboBox.setRawValue(this.durationToString(diff));
            delete (this.stopProcessingTimeFieldsEvents);
            return diff;
        },
        getStartDate: function() {
            var startTime = Date.parseDate(Ext.getCmp("ScheduledStartTime").getValue(), "H:i");
            if (!startTime) {
                return null;
            }
            var startDate = Ext.getCmp("ScheduledStart").getValue();
            startDate.setHours(startTime.getHours());
            startDate.setMinutes(startTime.getMinutes());
            return startDate;
        },
        getEndDate: function() {
            var endTime = Date.parseDate(Ext.getCmp("ScheduledEndTime").getValue(), "H:i");
            if (!endTime) {
                return null;
            }
            var endDate = Ext.getCmp("ScheduledEnd").getValue();
            endDate.setHours(endTime.getHours());
            endDate.setMinutes(endTime.getMinutes());
            return endDate;
        },
        setStartDate: function(startDate) {
            Ext.getCmp("ScheduledStart").setValue(startDate);
            Ext.getCmp("ScheduledStartTime").setValue(startDate.format("H:i"));
        },
        setEndDate: function(endDate) {
            Ext.getCmp("ScheduledEnd").setValue(endDate);
            Ext.getCmp("ScheduledEndTime").setValue(endDate.format("H:i"));
        },
        durationToString: function (durationInMinutes) {
            if (durationInMinutes < 1440) {
                var hoursCount = Math.floor(durationInMinutes / 60);
                var minutesCount = durationInMinutes % 60;
                return (hoursCount < 10 ? ("0" + hoursCount.toString()) : hoursCount.toString()) + ":" + (minutesCount < 10 ? "0" + minutesCount.toString() : minutesCount.toString());
            } else {
                hoursCount = Math.floor((durationInMinutes % 1440) / 60);
                var daysCount = Math.floor(durationInMinutes / 1440);

                var result;
                if (hoursCount) {
                    result = String.format(Ext.LocalizedResources.ActivityDaysAndHoursFormat, daysCount, hoursCount);
                } else {
                    result = String.format(Ext.LocalizedResources.ActivityDaysFormat, daysCount, hoursCount);
                }
                
                return result;
            }
        },
        getDurationComboBoxItems: function() {
            var durations = [5, 10, 15, 30, 45, 60, 90, 120, 150, 180, 240, 360, 1440, 2880, 4320];
            var result = [];
            Ext.each(durations, function(n) {
                result.push([this.durationToString(n), n]);
            }, this);
            return result;
        },
        onClientChanged: function () {
            this.autocompleteHeader();
            this.ClientNameBeforeChanged = Ext.fly("ClientName").getValue();

            // Если контакт 1, то выставляем его.
            // Если фирма 1, то выбираем её.
            // Если сделка 1, то выбираем её.
            // при этом проверяем, чем было вызвано изменение поля Client
            
            if (Ext.fly("Client").getValue()) {
                this.suppressClientUpdate = true;

                if (this.invoker !== "Firm") {
                    Ext.getCmp("Firm").forceGetData();
                }

                if (this.invoker !== "Contact") {
                    Ext.getCmp("Contact").forceGetData();
                }

                delete(this.suppressClientUpdate);
                delete(this.invoker);
            } 
        },
        onContactChanged: function () {
            if (!this.suppressClientUpdate && !Ext.fly("ClientId").getValue() && Ext.fly("ContactId").getValue()) {
                // Заполнить поле "Клиент" если оно ещё не заполнено
                this.invoker = "Contact";
                var clientCmp = Ext.getCmp("Client");
                clientCmp.forceGetData({
                    extendedInfo: "ContactId={ContactId}"
                });
            }
        },
        onFirmChanged: function () {
            if (!this.suppressClientUpdate && !Ext.fly("ClientId").getValue() && Ext.fly("FirmId").getValue()) {
                // Заполнить поле "Клиент" если оно ещё не заполнено
                this.invoker = "Firm";
                var clientCmp = Ext.getCmp("Client");
                clientCmp.forceGetData({
                    extendedInfo: "FirmId={FirmId}"
                });
            }
        },
        CancelActivity: function() {
            this.changeState("Cancelled");
        },
        CompleteActivity: function () {
            this.changeState("Completed");
        },
        RevertActivity: function() {
            this.changeState("InProgress");
        },
        Assign: function () {
            if (!this.checkDirty()) return;
            var params = "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no; resizable:no; ";
            var sUrl = "/GroupOperation/Assign/ActivityInstance";
            var result = window.showModalDialog(sUrl, [this.form.Id.value], params);
            if (result === true) {
                this.refresh(true);
            }
        },
        changeState: function (newStatus) {
            var currentStatus = Ext.fly("Status").getValue();
            var currentActualEnd = Ext.fly("ActualEnd").getValue();
            var currentActualEndTime = Ext.fly("ActualEndTime").getValue();

            if (newStatus == 'InProgress') {
                Ext.fly("ActualEnd").setValue(null);
            }
            else if (newStatus == 'Completed' || newStatus == 'Cancelled') {
                var currentDate = new Date();
                Ext.fly("ActualEnd").setValue(currentDate);
                Ext.getCmp("ActualEnd").setRawValue(currentDate);
                Ext.getCmp("ActualEndTime").setValue(currentDate);
            }
            
            var wasNotDirty = !this.isDirty;
            Ext.fly("Status").setValue(newStatus);
            
            this.Items.Toolbar.disable();
            this.submitMode = this.submitModes.SAVE;
            if (this.fireEvent('beforepost', this) === false) {
                this.recalcDisabling();
                Ext.fly("Status").setValue(currentStatus);
                Ext.fly("ActualEnd").setValue(currentActualEnd);
                Ext.fly("ActualEndTime").setValue(currentActualEndTime);
                
                if (wasNotDirty) {
                    this.isDirty = false;
                }
                    
                return;
            }
            if (this.normalizeForm() !== false) {
                this.postForm();
                this.on('afterpost', this.refresh, this, { single: true });
            }
            else {
                this.recalcDisabling();
                Ext.fly("Status").setValue(currentStatus);
                Ext.fly("ActualEnd").setValue(currentActualEnd);
                if (wasNotDirty) {
                    this.isDirty = false;
                }
            }
        },
        formbind: function() {
            var startTimeCmp = Ext.getCmp("ScheduledStartTime");
            startTimeCmp.setValue(startTimeCmp.getValue());
            
            var endTimeCmp = Ext.getCmp("ScheduledEndTime");
            endTimeCmp.setValue(endTimeCmp.getValue());
            this.recalculateDuration();
        }
        
    });

    this.on("afterbuild", this.initEventListeners, this);
    this.on("formbind", this.formbind, this);
};
