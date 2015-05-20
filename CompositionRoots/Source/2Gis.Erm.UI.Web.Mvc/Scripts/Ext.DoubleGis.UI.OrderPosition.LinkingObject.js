Ext.ns("Ext.DoubleGis.UI.OrderPosition");

Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes = {
    Firm: 'Firm',
    AddressCategorySingle: 'AddressCategorySingle',
    AddressFirstLevelCategorySingle: 'AddressFirstLevelCategorySingle',
    AddressFirstLevelCategoryMultiple: 'AddressFirstLevelCategoryMultiple',
    CategorySingle: 'CategorySingle',
    AddressSingle: 'AddressSingle',
    AddressMultiple: 'AddressMultiple',
    CategoryMultiple: 'CategoryMultiple',
    CategoryMultipleAsterix: 'CategoryMultipleAsterix',
    AddressCategoryMultiple: 'AddressCategoryMultiple',
    ThemeMultiple: 'ThemeMultiple'
};

Ext.DoubleGis.UI.OrderPosition.LinkingObject = Ext.extend(Ext.util.Observable, {
    linkingObjectTypes: Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes,

    //#region private fields

    key: null,
    type: null,
    position: null,
    categoryId: null,
    firmAddressId: null,
    selectedCount: null,

    advertisementLookup: null,

    checkBox: null,
    dumbCheckbox: null,

    controller: null,

    pendingCreationFinalizers: null,

    isInitialized: false,

    //#endregion

    //#region public methods

    /// @param {Ext.DoubleGis.UI.OrderPosition.AdvertisementController} controller
    /// @param {} position
    /// @param {Integer} categoryId
    /// @param {Integer} firmAddressId
    /// @param {Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes} type
    constructor: function (controller, position, categoryId, firmAddressId, themeId, type) {

        // эту функцию можно использовать с 5-ю или 6-ю параметрами, themeId - необязательный параметр.
        if (!type) {
            type = themeId;
            themeId = null;
        }

        this.key = controller.makeLinkingObjectKey(position.Id, categoryId, firmAddressId, themeId);

        for (var i = 0; i < position.LinkingObjects.length; i++) {
            if (position.LinkingObjects[i].key == this.key) {
                throw "AlreadyExists";
            }
        }

        this.type = type;
        this.position = position;
        this.categoryId = categoryId;
        this.firmAddressId = firmAddressId;
        this.themeId = themeId;
        this.checkBox = null;
        this.isDummyCheckBox = null;
        this.controller = controller;

        this.pendingCreationFinalizers = [];
        this.addEvents('checkedChanged');

        position.LinkingObjects.push(this);
        controller.registerLinkingObject(this);
    },

    beginCheckboxCreation: function () {
        var isComposite = window.Ext.getDom('IsComposite').value.toLowerCase();
        var outerDivId = 'checkboxDiv-' + this.key;
        var div = document.createElement('div');
        div.id = outerDivId;
        div.style.width = '100%';
        var self = this;

        this.pendingCreationFinalizers.push(function () {
            var checkbox = document.createElement('input');
            checkbox.id = 'checkbox-' + this.key;
            checkbox.type = 'checkbox';
            self.checkbox = checkbox;
            window.Ext.getDom(outerDivId).appendChild(checkbox);
            checkbox.checked = (self.type == window.Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes.Firm && isComposite == 'false') || self.getAdvertisement() != null;
            self.originalValue = checkbox.checked;
        });

        return div;
    },

    beginDummyCheckboxCreation: function () {
        var outerDivId = 'isDummyCheckboxDiv-' + this.key;
        var div = document.createElement('div');
        div.id = outerDivId;
        div.style.width = '100%';
        var self = this;

        this.pendingCreationFinalizers.push(function () {
            var checkbox = document.createElement('input');
            checkbox.id = 'isDummyCheckbox-' + this.key;
            checkbox.type = 'checkbox';
            self.isDummyCheckBox = checkbox;
            window.Ext.getDom(outerDivId).appendChild(checkbox);
            checkbox.checked = self.getAdvertisement() != null && self.getAdvertisement().AdvertisementId == self.position.DummyAdvertisementId;
            self.originalValue = checkbox.checked;
        });

        return div;
    },

    // Метод создаёт чекбокс отражающий не изменённое пользователем состояние заказа в базе данных и не доступный для изменения.
    beginDisabledCheckboxCreation: function () {
        var isComposite = window.Ext.getDom('IsComposite').value.toLowerCase();
        var key = this.key + '-disabled';
        var outerDivId = 'checkboxDiv-' + key;
        var div = document.createElement('div');
        div.id = outerDivId;
        div.style.width = '100%';
        var self = this;

        this.pendingCreationFinalizers.push(function () {
            var checkbox = document.createElement('input');
            checkbox.id = 'checkbox-' + key;
            checkbox.type = 'checkbox';
            window.Ext.getDom(outerDivId).appendChild(checkbox);
            checkbox.disabled = true;
            checkbox.checked = (self.type == window.Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes.Firm && isComposite == 'false') || self.getAdvertisement() != null;
        });

        return div;
    },

    beginAdvertisementLookupCreation: function () {
        if (!this.supportsAdvertisement()) {
            throw "Invalid operation";
        }

        var batch = this.createLookup('Advertisement');
        var self = this;
        this.pendingCreationFinalizers.push(function () {
            self.advertisementLookup = batch.createLookup();
        });
        return batch.outerDiv;
    },

    finalizeInitalization: function () {
        if (this.isInitialized) {
            throw "Invalid operation";
        }
        for (var i = 0; i < this.pendingCreationFinalizers.length; i++) {
            this.pendingCreationFinalizers[i]();
        }
        this.registerEvents();
        this.setupControlsAvailability();
        this.isInitialized = true;
    },

    supportsAdvertisement: function () {
        return typeof this.position.AdvertisementTemplateId == 'string' && this.position.AdvertisementTemplateId != '0';
    },


    registerEvents: function () {
        var self = this;

        if (this.supportsAdvertisement() && this.advertisementLookup) {
            this.advertisementLookup.on("change", function () { self.onAdvertisementChanged(); });
        }

        this.checkbox.onclick = function () { self.onCheckboxClick(); };

        if (this.supportsAdvertisement() && this.isDummyCheckBox) {
            this.isDummyCheckBox.onclick = function () { self.onIsDummyCheckboxClick(); };
        }
    },

    setupControlsAvailability: function () {

        var isComposite = window.Ext.getDom('IsComposite').value.toLowerCase();

        var checkboxDisabled = this.controller.localData.readOnly;
        var isDummyCheckboxDisabled = this.controller.localData.readOnly;

        if (this.controller.localData.areLinkingObjectParametersLocked) {
            checkboxDisabled = true;
            isDummyCheckboxDisabled = true;
        }
        else if (this.position.isAdvertisementLimitReached && !this.checkbox.checked) {
            checkboxDisabled = true;
            isDummyCheckboxDisabled = true;
        }
        else {
            if (this.checkbox.checked) {
                if (this.type == window.Ext.DoubleGis.UI.OrderPosition.LinkingObjectTypes.Firm && isComposite == 'false') {
                    checkboxDisabled = true;
                    isDummyCheckboxDisabled = false;
                }
            } else {
                if (this.isDeleted()) {
                    checkboxDisabled = true;
                    isDummyCheckboxDisabled = true;
                }
            }
        }

        this.checkbox.disabled = checkboxDisabled;

        var disableAdvertisementLookup = this.controller.localData.readOnly;
        if (this.supportsAdvertisement() && this.advertisementLookup) {

            if (!this.checkbox.checked) {
                if (this.controller.localData.areLinkingObjectParametersLocked ||
                    this.position.isAdvertisementLimitReached) {
                    disableAdvertisementLookup = true;
                }
            }
            if (disableAdvertisementLookup)
                this.advertisementLookup.disable();
            else
                this.advertisementLookup.enable();
        }

        if (this.supportsAdvertisement() && this.isDummyCheckBox) {
            this.isDummyCheckBox.disabled = disableAdvertisementLookup && isDummyCheckboxDisabled;
        }
    },

    isDeleted: function () {
        return this.node.disabled;
    },

    isSelected: function () {
        return this.checkbox.checked;
    },

    isChanged: function () {
        return this.checkbox.checked != this.originalValue;
    },

    isAdvertisementSelected: function () {
        if (!this.supportsAdvertisement()) {
            throw "Invalid operation";
        }
        if (!this.advertisementLookup)
            return false;
        return this.advertisementLookup.getValue() != null;
    },

    getSelectedAdvertisement: function () {
        if (!this.supportsAdvertisement() || !this.advertisementLookup) {
            throw "Invalid operation";
        }
        return this.advertisementLookup.getValue();
    },

    clearAdvertisement: function () {
        if (this.supportsAdvertisement() && this.isDummyCheckBox) {
            this.isDummyCheckBox.checked = false;
        }
        if (this.supportsAdvertisement() && this.advertisementLookup) {
            this.advertisementLookup.clearValue();
        }
    },

    destroy: function () {
        //Place lookup release code here  
    },

    //#endregion

    //#region private event handlers

    onAdvertisementChanged: function () {
        if (this.advertisementLookup.getValue()) {
            if (this.position.DummyAdvertisementId != this.advertisementLookup.getValue().id) {
                this.isDummyCheckBox.checked = false;
            }
            if (!this.checkbox.checked) {
                this.checkbox.checked = true;
                this.controller.notifySelectedCountChanged(this);
            }
        } else {
            this.isDummyCheckBox.checked = false;
        }
    },

    changeExtendedInfoParameter: function (originalString, parameterName, parameterValue) {
        var newString = '';
        var found = false;
        if (originalString && originalString.length > 0) {
            var split = originalString.split('&');
            for (var i = 0; i < split.length; i++) {
                if (i > 0) {
                    newString += '&';
                }
                var parts = split[i].split('=');
                if (parts.length != 2) {
                    newString += split[i];
                } else {
                    if (parts[0] == parameterName) {
                        parts[1] = parameterValue.toString();
                        found = true;
                    }
                    newString += parts[0] + "=" + parts[1];
                }
            }
        }
        if (!found) {
            if (newString.length > 0)
                newString += '&';
            newString += parameterName + '=' + parameterValue.toString();
        }
        return newString;
    },

    onCheckboxClick: function () {
        if (!this.checkbox.checked) {
            this.clearAdvertisement();
        }
        this.controller.notifySelectedCountChanged(this);
    },

    onIsDummyCheckboxClick: function () {
        if (!this.checkbox.checked) {
            if (this.checkbox.disabled) {
                return;
            }
        }

        if (!this.isDummyCheckBox.checked) {
            if (this.advertisementLookup) {
                this.advertisementLookup.clearValue();
            }
        }
        else {
            if (this.supportsAdvertisement() && this.advertisementLookup) {
                Ext.MessageBox.show({
                    title: Ext.LocalizedResources.Alert,
                    msg: Ext.LocalizedResources.DummyValueWillBeApplied,
                    width: 300,
                    buttons: window.Ext.MessageBox.ContinueCANCEL,
                    linkingObjectNode: this,
                    fn: function (buttonId, value, opt) {
                        if (buttonId == 'Continue') {
                            opt.linkingObjectNode.advertisementLookup.setValue({ id: opt.linkingObjectNode.position.DummyAdvertisementId, name: Ext.LocalizedResources.DummyValue }, true);
                            if (!opt.linkingObjectNode.checkbox.checked && !opt.linkingObjectNode.checkbox.disabled) {
                                opt.linkingObjectNode.checkbox.click();

                                // этот WTF является workaround'ом для бага ERM-6593
                                // Суть бага - в редких случаях checked остается false.
                                opt.linkingObjectNode.checkbox.checked = true;
                            }
                        } else {
                            opt.linkingObjectNode.isDummyCheckBox.checked = false;
                        }
                    },
                    icon: window.Ext.MessageBox.QUESTION
                });
            }
        }
    },

    //#endregion

    //#region private methods

    getAdvertisement: function () {
        return this.controller.advertisements.byKey[this.key];
    },

    createLookup: function (entityName) {
        var outerDiv = document.createElement('div');
        outerDiv.setAttribute('class', entityName.toLowerCase() + 'LookupOuter');

        var inputIdentity = this.key + '-' + entityName + '-input';
        var hiddenField = document.createElement('input');
        hiddenField.type = 'hidden';
        hiddenField.setAttribute('id', inputIdentity);
        hiddenField.setAttribute('name', inputIdentity);

        var keyValue = {};
        var settingsItem = {};

        var advertisement = this.getAdvertisement();

        if (advertisement) {
            switch (entityName) {
                case 'Advertisement':
                    keyValue.Key = advertisement.AdvertisementId;
                    keyValue.Value = advertisement.AdvertisementName;
                    break;
                case 'Category':
                    if (advertisement.CategoryId) {
                        keyValue.Key = advertisement.CategoryId;
                        keyValue.Value = advertisement.CategoryName;
                    }
                    break;
                case 'FirmAddress':
                    if (advertisement.FirmAddressId) {
                        keyValue.Key = advertisement.FirmAddressId;
                        keyValue.Value = advertisement.FirmAddress;
                    }
                    break;
            }

            if (keyValue.Key) {
                settingsItem = {
                    Id: keyValue.Key,
                    Name: keyValue.Value
                };
            }
        }

        hiddenField.value = window.Ext.encode(keyValue);

        outerDiv.appendChild(hiddenField);

        var extendedInfo = 'FirmId=' + this.controller.localData.firmId;

        switch (entityName) {
            case 'Advertisement':
                {
                    extendedInfo += '&AdvertisementTemplateId=' + this.position.AdvertisementTemplateId;
                }
                break;
            case 'FirmAddress':
                {
                }
                break;
            case 'Category':
                {
                    var desiredLevel;
                    if (this.type == this.linkingObjectTypes.AddressFirstLevelCategorySingle) {
                        desiredLevel = 1;
                    }
                    else {
                        desiredLevel = 3;
                    }

                    extendedInfo += '&Level=' + desiredLevel;

                    if (this.type == this.linkingObjectTypes.AddressCategorySingle || this.type == this.linkingObjectTypes.AddressFirstLevelCategorySingle) {
                        if (advertisement && advertisement.FirmAddressId) {
                            extendedInfo += '&FirmAddressId=' + advertisement.FirmAddressId;
                        }
                    }

                    extendedInfo += "&OrganizationUnitId=" + (this.controller.localData.organizationUnitId || -1);
                }
                break;
        }


        var settings = {
            isCrm: false,
            crmCode: 0,
            disabled: false,
            locked: false,
            showReadOnlyCard: (entityName != 'Advertisement'),
            name: inputIdentity,
            applyTo: inputIdentity,
            crmRoot: "../../..",
            entityName: entityName,
            entityIcon: "en_ico_16_Default.gif",
            extendedInfo: extendedInfo,
            item: settingsItem
        };

        var result = {
            outerDiv: outerDiv,
            createLookup: function () {
                return new window.Ext.ux.LookupField(settings);
            }
        };

        return result;
    }

    //#endregion
});
