Ext.ns('Ext.DoubleGis');
Ext.DoubleGis.DependencyHandler = Ext.extend(Ext.util.Observable, {
    dependencyList: {},
    HIDDEN: 0,
    READONLY: 1,
    DISABLE: 2,
    REQUIRED: 3,
    DISABLE_AND_HIDE: 4,
    NOT_REQUIRED_DISABLE_HIDE: 5,
    TRANSFER: 6,
    constructor: function (config)
    {
        window.Ext.DoubleGis.DependencyHandler.superclass.constructor.call(this, config);
        this.addEvents('beforeregister', 'afterregister', 'beforeunregister', 'afterunregister', 'beforehandle', 'afterhandle');
    },
    register: function (dependencyList)
    {
        if (this.fireEvent('beforeregister') === false)
        {
            return;
        }
        this.dependencyList = dependencyList;
        this.initFields(this.dependencyList);
        this.fireEvent('afterregister');
    },
    unregister: function ()
    {
        if (this.fireEvent('beforeunregister') === false)
        {
            return;
        }
        window.Ext.each(this.dependencyList, function (item)
        {
            if (item.TargetFieldId)
            {
                var cmp = window.Ext.getCmp(item.TargetFieldId);
                if (cmp)
                {
                    if (cmp.events.change)
                    {
                        cmp.un("change", this.cmpHandleChange, this);
                    }
                }
                else
                {
                    var el = window.Ext.get(item.TargetFieldId);
                    if (el)
                    {
                        el.un(el.dom.type == "radio" || el.dom.type == "checkbox" ? "click" : "change", this.handleChange, this);
                    }
                }
            }
        }, this);

        this.fireEvent('afterunregister');
        this.dependencyList = {};

    },
    initFields: function (dependencyList)
    {
        window.Ext.each(dependencyList, function (item)
        {
            if (item.TargetFieldId)
            {
                var cmp = window.Ext.getCmp(item.TargetFieldId);
                if (cmp)
                {
                    if (cmp.events.change)
                    {
                        cmp.on("change", this.cmpHandleChange, this);
                        this.calculateDependencies(cmp, true);
                    }
                    else
                    {
                        throw 'there is no change event on component ' + item.TargetFieldId;
                    }
                }
                else
                {
                    var el = window.Ext.get(item.TargetFieldId);
                    if (el)
                    {
                        el.on(el.dom.type == "radio" || el.dom.type == "checkbox" ? "click" : "change", this.handleChange, this);
                        this.calculateDependencies(el.dom, true);
                    }
                }
            }
        }, this);
    },
    handleChange: function (eventObj, elRef) { this.calculateDependencies(elRef, true); },
    cmpHandleChange: function (cmp) { this.calculateDependencies(cmp, true); },
    evaluateExpression: function (expression, initially)
    {
        return eval(expression);
    },
    handleDependency: function (dependency, field, initially)
    {
        if (Ext.isEmpty(Ext.getDom(dependency.Id)))
            return;

        if (dependency.Type == this.TRANSFER)
        {
            this.transferValue(dependency, field, initially);
            return;
        }
        var sign = this.evaluateExpression.createDelegate(field)(dependency.Expression, initially) === true;
        switch (dependency.Type)
        {
            case this.HIDDEN:
                sign ? this.hideField(dependency, initially) : this.showField(dependency, initially);
                break;
            case this.READONLY:
                this.setReadOnly(dependency, sign);
                break;
            case this.DISABLE:
                sign ? this.disableField(dependency) : this.enableField(dependency);
                break;
            case this.DISABLE_AND_HIDE:
                sign ? this.disableField(dependency) : this.enableField(dependency);
                sign ? this.hideField(dependency, initially) : this.showField(dependency, initially);
                break;
            case this.REQUIRED:
                sign ? this.addRequiredVRule(dependency) : this.removeRequiredVRule(dependency);
                break;
            case this.NOT_REQUIRED_DISABLE_HIDE:
                sign ? this.disableField(dependency) : this.enableField(dependency);
                sign ? this.hideField(dependency, initially) : this.showField(dependency, initially);
                sign ? this.removeRequiredVRule(dependency) : this.addRequiredVRule(dependency);
                break;
        }
    },
    calculateDependencies: function (field, initially)
    {
        window.Ext.each(this.dependencyList,
            function (item)
            {
                if (item.TargetFieldId == field.id)
                {
                    window.Ext.each(item.DependentFields, function (dependentField)
                    {
                        if (dependentField.isCmp === undefined)
                        {
                            dependentField.isCmp = !Ext.isEmpty(Ext.getCmp(dependentField.Id));
                        }
                        this.handleDependency(dependentField, field, initially);
                    }, this);
                }
            }, this);
    },
    calculateRowLayout: function (elem)
    {
        var parent = elem.parent(".row-wrapper");
        if (parent)
        {
            var display = parent.query("div.display-wrapper");

            var cls = "hidden-wrapper";
            switch (display.length)
            {
                case 1:
                    cls = "lone";
                    break;
                case 2:
                    cls = "twins";
                    break;
                case 3:
                    cls = "triplet";
                    break;
                case 4:
                    cls = "quadruplet";
                    break;
            }
            for (var i = 0; i < display.length; i++)
            {
                window.Ext.fly(display[i]).removeClass(["lone", "twins", "triplet", "quadruplet"]).addClass(cls);
            }
        }
    },
    hideField: function (dependency, initially)
    {
        var field = window.Ext.get(dependency.Id);
        if (field.dom.tagName == "INPUT" || field.dom.tagName == "SELECT" || field.dom.tagName == "TEXTAREA")
        {
            var wrapper = window.Ext.get(field.dom.id + "-wrapper");
            if (wrapper && wrapper.hasClass("display-wrapper"))
            {
                wrapper.replaceClass("display-wrapper", "hidden-wrapper");
                this.calculateRowLayout(wrapper);
                initially ? wrapper.setVisibilityMode(Ext.Element.DISPLAY).hide() : wrapper.fadeOut({ useDisplay: true, duration: 0.75 });
            }
        }
        else
        {
            initially ? field.setVisibilityMode(Ext.Element.DISPLAY).hide() : field.fadeOut({ useDisplay: true, duration: 0.75 });
        }
    },
    showField: function (dependency, initially)
    {
        var field = window.Ext.get(dependency.Id);
        if (field.dom.nodeName == "INPUT" || field.dom.nodeName == "SELECT")
        {
            var wrapper = window.Ext.get(field.dom.id + "-wrapper");
            if (wrapper && wrapper.hasClass("hidden-wrapper"))
            {
                wrapper.replaceClass("hidden-wrapper", "display-wrapper");
                this.calculateRowLayout(wrapper);
                initially ? wrapper.show() : wrapper.fadeIn({ useDisplay: true, duration: 0.75 });
            }
        }
        else
        {
            initially ? field.show() : field.fadeIn({ useDisplay: true, duration: 0.75 });
        }
    },
    setReadOnly: function (dependency, readonly)
    {
        if (dependency.isCmp === true)
        {
            Ext.getCmp(dependency.Id).setReadOnly(readonly);
        }
        else
        {
            Ext.get(dependency.Id).setReadOnly(readonly);
        }
    },
    disableField: function (dependency)
    {
        if (dependency.isCmp === true)
        {
            Ext.getCmp(dependency.Id).disable();
        }
        else
        {
            Ext.get(dependency.Id).disable();
        }
        var wrapper = window.Ext.getDom(dependency.Id + "-caption");
        if (wrapper)
            wrapper.disabled = "disabled";
    },
    enableField: function (dependency)
    {
        if (dependency.isCmp === true)
        {
            Ext.getCmp(dependency.Id).enable();
        }
        else
        {
            Ext.get(dependency.Id).enable();
        }
        var wrapper = window.Ext.getDom(dependency.Id + "-caption");
        if (wrapper)
            wrapper.disabled = false;
    },
    transferValue: function (dependency, field, initially)
    {
        var result = this.evaluateExpression.createDelegate(field)(dependency.Expression, initially);
        if (result !== undefined)
        {
            if (dependency.isCmp === true)
            {
                Ext.getCmp(dependency.Id).setValue(result);
            }
            else
            {
                window.Ext.getDom(dependency.Id).value = result;
            }
        }
    },
    addRequiredVRule: function (dependency)
    {
        if (!Ext.DoubleGis.FormValidator)
        {
            Ext.MessageBox.show({
                title: '',
                msg: "Не удается найти валидатор для обработки зависимостей.",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.ERROR
            });
            return;
        }
        var frm = Ext.DoubleGis.FormValidator.forms[Ext.getDom(dependency.Id).form.id];
        if (!frm)
        {
            Ext.MessageBox.show({
                title: '',
                msg: "Не удается найти форму для обработки зависимостей.",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.ERROR
            });
            return;
        }
        var field, rule;
        Ext.each(frm.Fields, function (frmField)
        {
            if (frmField.FieldName == dependency.Id)
            {
                field = frmField;
                return false;
            }
            return true;
        });
        if (!field)
        {
            field = {
                FieldName: dependency.Id,
                ReplaceValidationMessageContents: true,
                ValidationMessageId: dependency.Id + "_validationMessage",
                validators: {},
                ValidationRules: []
            };
            frm.Fields.push(field);
        }
        Ext.each(field.ValidationRules, function (fieldRule)
        {
            if (fieldRule.ValidationType == "required")
            {
                rule = fieldRule;
                return false;
            }
            return true;
        });

        if (!rule)
        {
            var localizedLabel = window.Ext.select('label[for=' + dependency.Id + ']', true, frm.form.id);
            var localizedText = (localizedLabel && localizedLabel.elements && localizedLabel.elements[0]) ? localizedLabel.elements[0].dom.innerText : dependency.Id;
            rule = {
                ValidationType: "required",
                ValidationParameters: {},
                ErrorMessage: String.format(Ext.LocalizedResources.RequiredFieldMessage, localizedText)
            };
            field.ValidationRules.push(rule);
        }
        Ext.DoubleGis.FormValidator.attachValidator(frm, field, rule);
    },
    removeRequiredVRule: function (dependency)
    {
        if (!Ext.DoubleGis.FormValidator)
        {
            Ext.MessageBox.show({
                title: '',
                msg: "Не удается найти валидатор для обработки зависимостей.",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.ERROR
            });
            return;
        }
        var frm = Ext.DoubleGis.FormValidator.forms[Ext.getDom(dependency.Id).form.id];
        if (!frm)
        {
            Ext.MessageBox.show({
                title: '',
                msg: "Не удается найти форму для обработки зависимостей.",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.ERROR
            });
            return;
        }
        var field;
        Ext.each(frm.Fields, function (frmField)
        {
            if (frmField.FieldName == dependency.Id)
            {
                field = frmField;
                return false;
            }
            return true;
        });
        if (field)
        {
            Ext.DoubleGis.FormValidator.detachValidator(frm, field, "required");
        }
    }
});
