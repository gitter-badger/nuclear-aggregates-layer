window.Ext.ns("Ext.DoubleGis.Global");
Ext.IS_DEBUG = Ext.fly("meta_IsDebug").dom.getAttribute("content") == 'True';

// static resources
Ext.APP_REVISION = Ext.fly("meta_Revision").dom.getAttribute("content");
Ext.APP_IMAGES_RES_PATH = "/Content/images/";
Ext.APP_ENTITY_RES_PATH = Ext.APP_IMAGES_RES_PATH + "Configuration/Entity/";

//Параметры глобализации/локализации

function RecalculateCardSize(rate) {
    Ext.DoubleGis.Global.UISettings = {};
    Ext.DoubleGis.Global.UISettings.ActualCardWidth = window.screen.width * rate;
    Ext.DoubleGis.Global.UISettings.ActualCardHeight = window.screen.height * rate;
    Ext.DoubleGis.Global.UISettings.ScreenCenterLeft = (window.screen.width - window.screen.width * rate) / 2;
    Ext.DoubleGis.Global.UISettings.ScreenCenterTop = (window.screen.height - window.screen.height * rate) / 2;
}

RecalculateCardSize(0.7);

//Статические методы, в основном для отрисовки UI
Ext.DoubleGis.Global.Helpers = {

    GetStaticImagePath: function (imgRelativePath) {
        return (imgRelativePath) ? Ext.APP_IMAGES_RES_PATH + imgRelativePath + "?" + Ext.APP_REVISION : undefined;
    },
    GetEntityIconPath: function (imgName) {
        // В базе название иконки для сущности хранится в виде "en_ico_16_Default-gif".
        return (imgName) ? Ext.APP_ENTITY_RES_PATH + imgName + "?" + Ext.APP_REVISION : undefined;
    },
    EvaluateCreateEntityUrl: function (entityName, queryString) {
        return '/CreateOrUpdate/' + entityName + queryString;
    },
    EvaluateUpdateEntityUrl: function (entityName, entityId, queryString) {
        return '/CreateOrUpdate/' + entityName + '/' + entityId + queryString;
    },
    EvaluateUpdateEntityUrlByQueryString: function (entityName, queryString) {
        return '/CreateOrUpdate/' + entityName + queryString;
    },
    OpenEntity: function (entityName, tmpEntityId) {
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", Ext.DoubleGis.Global.UISettings.ActualCardWidth, Ext.DoubleGis.Global.UISettings.ActualCardHeight, Ext.DoubleGis.Global.UISettings.ScreenCenterTop, Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl(entityName, tmpEntityId, '');
        window.open(sUrl, "_blank", params);
    },
    GridColumnHelper: {
        GetColumnRenderer: function (field) {
            var renderFn = undefined;
            if (field.RenderFn) {
                // Если указана пользовательская функция рендеринга полей - создаём пользовательский тип поля, что позволяет отдать в неё данные, 
                // прилетевшие с сервера без предварительного приведения к типу field.Type (который может быть String, Int, и т.д.)
                // В настоящее время сортировка записей проводится на стороне сервера, поэтому нет необходимости в реализованной функции сортитровки (sortType)
                var jscode = 'field.Type = {';
                jscode += 'convert: ' + field.RenderFn + ', ';
                jscode += 'sortType: function (v) { return ""; }, type: "UserDefined"'; 
                jscode += '};';
                eval(jscode);
            }
            else if (field.ReferenceTo && field.ReferenceKeyField) {
                renderFn = window.Ext.DoubleGis.Global.Helpers.GridColumnHelper.RenderReferenceColumn;
            }
            else if (field.FieldType == Ext.FieldType.DateTime) {
                renderFn = Ext.util.Format.dateWOffsetRenderer(Ext.CultureInfo.DateTimeFormatInfo);
            }
            else if (field.FieldType == Ext.FieldType.Period) {
                renderFn = Ext.util.Format.dateRenderer(Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern);
            }
            else if (field.FieldType == Ext.FieldType.Float) {
                field.align = 'right';
                renderFn = Ext.util.Format.exNumberRenderer(Ext.CultureInfo.NumberFormatInfo);
            }
            else if (field.FieldType == Ext.FieldType.SmallInt || field.FieldType == Ext.FieldType.BigInt || field.FieldType == Ext.FieldType.Int) {
                field.align = 'right';
            }
            else if (field.FieldType == Ext.FieldType.Money) {
                field.align = 'right';
                renderFn = Ext.util.Format.moneyRenderer(Ext.CultureInfo.NumberFormatInfo);
            }
            else if (field.FieldType == Ext.FieldType.Boolean) {
                renderFn = Ext.util.Format.booleanRenderer();
            }
            return renderFn;
        },
        RenderDefaultIcon: function () {
            return this.Icon ? "<img style='height: 16px; width: 16px' src='" + window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(this.Icon) + "'></img>" : "";
        },
        RenderEntityIcon: function (entityName) {
            return "<img style='height: 16px; width: 16px' src='" + window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(entityName) + "'></img>";
        },
        RenderReferenceColumn: function (value, metaData, record) {
            if (record.data[this.ReferenceKeyField]) {
                return String.format('<span class="x-entity-link" style="text-decoration:underline; cursor: hand;">{0}</span>', value);
            }
            return value;
        }
    },
    BuildDataListFilter: function (entitySettings, relatedFilter) {
        if (relatedFilter) {
            return relatedFilter;
        }
        return "";
    },
    //Реукрсивный построитель меню
    ToolbarHelper: {
        BuildToolbar: function (items, showtips, scope) {
            if (showtips === true) {
                window.Ext.QuickTips.init();
            }

            var toolbarItems = [];

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.ParentName) {
                    continue;
                }

                if (item.ControlType == "Menu") {
                    toolbarItems.push({
                        xtype: "tbbutton",
                        text: item.LocalizedName,
                        tooltip: item.LocalizedName,
                        id: item.Name,
                        menu: window.Ext.DoubleGis.Global.Helpers.ToolbarHelper.AddChildMenuItems(items, item.Name, scope)
                    });
                }
                else if (item.ControlType == "Splitter") {
                    toolbarItems.push({ xtype: "tbseparator" });
                }
                else if (item.ControlType == "TextButton") {
                    toolbarItems.push({
                        xtype: "tbbutton",
                        text: item.LocalizedName,
                        tooltip: item.LocalizedName,
                        id: item.Name,
                        cls: "x-btn-text",
                        handler: eval(item.Action),
                        scope: scope,
                        disabled: item.Disabled,
                        disabledInitially: item.Disabled,
                        disableOnEmpty: item.DisableOnEmpty
                    });
                }
                else if (item.ControlType == "ImageButton") {
                    toolbarItems.push({
                        xtype: "tbbutton",
                        text: '',
                        tooltip: item.LocalizedName,
                        id: item.Name,
                        icon: window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(item.Icon),
                        cls: "x-btn-icon",
                        handler: eval(item.Action),
                        scope: scope,
                        disabled: item.Disabled,
                        disabledInitially: item.Disabled,
                        disableOnEmpty: item.DisableOnEmpty
                    });
                }
                else if (item.ControlType == "TextImageButton") {
                    toolbarItems.push({
                        xtype: "tbbutton",
                        text: item.LocalizedName,
                        tooltip: item.LocalizedName,
                        id: item.Name,
                        icon: window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(item.Icon),
                        cls: "x-btn-text-icon",
                        handler: eval(item.Action),
                        scope: scope,
                        disabled: item.Disabled,
                        disabledInitially: item.Disabled,
                       disableOnEmpty: item.DisableOnEmpty
                    });
                }
            }
            return toolbarItems;
        },
        AddChildMenuItems: function (items, parentName, scope) {
            var childItems = [];
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.ParentName != parentName) {
                    continue;
                }
                if (item.ControlType == "Menu") {
                    childItems.push({
                        text: item.LocalizedName,
                        tooltip: item.LocalizedName,
                        id: item.Name,
                        icon: item.Icon ? window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(item.Icon) : undefined,
                        handler: eval(item.Action),
                        scope: scope,
                        menu: this.AddChildMenuItems(items, item.Name, scope)
                    });
                }
                else {
                    childItems.push({
                        text: item.LocalizedName,
                        tooltip: item.LocalizedName,
                        id: item.Name,
                        handler: eval(item.Action),
                        scope: scope,
                        disabled: item.Disabled,
                        disabledInitially: item.Disabled,
                        disableOnEmpty: item.DisableOnEmpty
                    });
                }
            }
            return childItems;
        }
    },
    //Реукрсивный построитель дерева
    NavBarHelper: {
        BuildTree: function (root, textLengthLimit) {
            var accItems = [];
            if (!root.Items)
            { return undefined; }
            for (var i = 0; i < root.Items.length; i++) {
                var leaf = root.Items[i];
                accItems.push(new Object({
                    id: leaf.Name,
                    text: textLengthLimit && leaf.LocalizedName.length > textLengthLimit ? leaf.LocalizedName.substring(0, textLengthLimit - 2) + '...' : leaf.LocalizedName,
                    qtip: leaf.LocalizedName,
                    leaf: !(leaf.Items && leaf.Items.length),
                    disabledExpression: leaf.DisabledExpression,
                    expanded: true,
                    requestUrl: leaf.RequestUrl,
                    extendedInfo: leaf.ExtendedInfo,
                    appendableEntity: leaf.AppendableEntity,
                    disabled: leaf.Disabled || eval(leaf.DisabledExpression),
                    children: window.Ext.DoubleGis.Global.Helpers.NavBarHelper.BuildTree(leaf, textLengthLimit),
                    icon: leaf.Icon ? window.Ext.DoubleGis.Global.Helpers.GetEntityIconPath(leaf.Icon) : undefined
                }));
            }
            return accItems;
        }
    },
    ShowEntityLink: function (config) {
        config = Ext.apply({}, config);

        var linkId = config.entityName + '-' + config.entityId + '-link';
        config.msg = config.msg.replace("{entity}", String.format('<span class="x-lookup-item" id="{0}">{1}</span>', linkId, config.entityDescription));

        Ext.Msg.show(config);
        Ext.get(linkId).on('click', function () {
            Ext.DoubleGis.Global.Helpers.OpenEntity(config.entityName, config.entityId);
        });
    },
    HideComboBoxItemsByValues : function(comboBox, valuesToHide) {
        for (var i = 0; i < comboBox.options.length; i++) {
            if (valuesToHide.indexOf(comboBox.options[i].value) != -1) {
                comboBox.options[i] = null;
                i--;
            }
        }
    },
    HideComboBoxItemsExceptValues: function (comboBox, valuesToDisplay) {
        for (var i = 0; i < comboBox.options.length; i++) {
            if (valuesToDisplay.indexOf(comboBox.options[i].value) == -1) {
                comboBox.options[i] = null;
                i--;
            }
        }
    }
};

Ext.Notification = {
    Icon: {
        None: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("default/blank-image.gif"),
        CriticalError: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/Error/critical.png"),
        Info: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/Error/notification.png"),
        Warning: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/Error/warning.png")
    }
};
Ext.FieldType =
    {
        String: 'String',
        SmallInt: 'SmallInt',
        Int: 'Int',
        BigInt: 'BigInt',
        Float: 'Float',
        Money: 'Money',
        Boolean: 'Boolean',
        DateTime: 'DateTime',
        Object: 'Object',
        Guid: 'Guid',
        ByteArray: 'ByteArray',
        Lookupfield: 'Lookupfield',
        Period: 'Period'
    };

Ext.CurrencyFormat = {
    Positive: {KZT:'n $'},
    Negative: {KZT:'-n $'}
};

//Небольшое расширение модели Ext
var extendExt = function () {
    Ext.BLANK_IMAGE_URL = Ext.DoubleGis.Global.Helpers.GetStaticImagePath("default/blank-image.gif");
    if (Ext.isNull) {
        alert("Функция IsNull уже определена.");
    } else {
        Ext.isNull = function (o) {
            return ("undefined" == typeof (o) || "unknown" == typeof (o) || null == o);
        };
    }

    if (Ext.isNullOrDefault) {
        alert("Функция isNullOrDefault уже определена.");
    } else {
        Ext.isNullOrDefault = function (o) {
            return ("undefined" == typeof (o) || "unknown" == typeof (o) || null == o || o === false || o === 0 || o === "");
        };
    }

    Ext.override(Ext.Element,
    {
        setValue: function (value) {
            var el = this.dom;
            if (el.value === undefined) {
                throw new Error("Элемент [" + el.id + "] не поддерживает свойство value.");
            }
            el.value = Ext.isNull(value) ? "" : value;
            var evObj;
            if (document.createEvent) {
                evObj = document.createEvent('Events');
                evObj.initEvent('change', true, true);
                el.dispatchEvent(evObj);
            } else if (document.createEventObject) {
                evObj = document.createEventObject();
                el.fireEvent('onchange', evObj);
            }
        },
        disable: function () {
            this.dom.disabled = "disabled";
            return this.addClass("ReadOnly");
        },
        enable: function () {
            this.dom.disabled = null;
            return this.removeClass("ReadOnly");
        },
        setReadOnly: function (readonly) {
            var el = this.dom;
            if (el.readOnly === undefined) {
                throw new Error("Элемент [" + el.id + "] не поддерживает свойство readonly.");
            }
            el.readOnly = readonly;
            readonly ? this.addClass("ReadOnly") : this.removeClass("ReadOnly");
            return this;
        },
        focusTo: function (position, selectFrom) {
            if (this.dom.createTextRange) {
                var range = this.dom.createTextRange();
                if (selectFrom !== undefined) {
                    range.moveStart("character", selectFrom);
                    range.moveEnd("character", position);
                } else {
                    range.move('character', position);
                }
                range.select();
                return;
            }
            
            if (Ext.isChrome) {
                var self = this;
                window.setTimeout(function () { self.dom.setSelectionRange(selectFrom != undefined ? selectFrom : position, position); }, 0);
                return;
            }

            if (this.dom.selectionStart) {
                this.focus();
                this.dom.setSelectionRange(selectFrom != undefined ? selectFrom : position, position);
                return;
            }
            this.focus();
        }
    });
    Ext.applyIf(Number, {
        formatToLocal: function (value) {
            // work with nullable number
            if (value == null)
                return null;

            return value.toString().replace('.', Ext.CultureInfo.NumberFormatInfo.NumberDecimalSeparator);
            return Ext.util.Format.exNumber(value, Ext.CultureInfo.NumberFormatInfo, false);
        },
        parseFromLocal: function (text) {
            if (text == Ext.CultureInfo.NumberFormatInfo.PositiveInfinitySymbol || text == Ext.CultureInfo.NumberFormatInfo.NegativeInfinitySymbol) {
                return NaN;
            }
            var regex = new RegExp("[" + (Ext.CultureInfo.NumberFormatInfo.NumberGroupSeparator ? Ext.CultureInfo.NumberFormatInfo.NumberGroupSeparator : " ") + "]", 'g');
            var clText = text
            .replace(/\s/g, '')
            .replace(regex, '')
            .replace(Ext.CultureInfo.NumberFormatInfo.NumberDecimalSeparator, '.');
            return Ext.num(clText, NaN);
        },
        truncateFloat: function (number, decimalPlaces) {
            var base = Math.pow(10, decimalPlaces);
            return Math[number < 0 ? 'ceil' : 'floor'](number * base) / base;
        }
    });

    Ext.apply(Date.prototype, {
        shiftOffset: function (offsetInMinutes) {
            return this.add(Date.MINUTE, offsetInMinutes || Ext.CultureInfo.DateTimeFormatInfo.TimeOffsetInMinutes);

        },
        clearOffset: function (offsetInMinutes) {
            return this.add(Date.MINUTE, (offsetInMinutes || Ext.CultureInfo.DateTimeFormatInfo.TimeOffsetInMinutes) * -1);
        },
        fixDateTime: function () {
            // // FIXME {all, 30.10.2014}: Откатить после декабрьского обновления поясов
            // Одинаковая дата сейчас и через час сигнализирует о проблемах перевода стрелок часов.
            // Наиболее вероятно, что при десериализации даты поимели эту проблему.
            // Поэтому принудительно добавляем час.
            // Добавляя один час к внутреннему представлению, меняем внешнее представление на два часа.
            if (this.add(Date.HOUR, 1).getTime() == this.getTime()) {
                return new Date(this.getTime() + 60 * 60 * 1000);
            }

            return this;
        }
    });

    if (Ext.util.Format) {
        Ext.apply(Ext.util.Format, {
            cacheBuster: function () {
                return (new Date().getTime());
            },
            dateWOffset: function (v, formatInfo) {
                formatInfo = formatInfo || Ext.CultureInfo.DateTimeFormatInfo;
                if (!v) {
                    return "";
                }
                if (!Ext.isDate(v)) {
                    v = new Date(Date.parse(v));
                }
                v = v.fixDateTime();
                return v.shiftOffset().dateFormat(formatInfo.PhpFullDateTimePattern || Ext.CultureInfo.DateTimeFormatInfo.PhpFullDateTimePattern);
            },
            reformatDateFromUserLocaleToInvariant: function (v, formatInfo) {
                formatInfo = formatInfo || Ext.CultureInfo.DateTimeFormatInfo;
                if (!v) {
                    return "";
                }
                if (!Ext.isDate(v)) {
                    v = new Date(Date.parseDate(v, Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern));
                }
                return v.dateFormat(formatInfo.PhpInvariantDateTimePattern);
            },
            dateWOffsetRenderer: function (formatInfo) {
                return function (v) {
                    return Ext.util.Format.dateWOffset(v, formatInfo || Ext.CultureInfo.DateTimeFormatInfo);
                };
            },
            date: function (v, format) {
                if (!v) {
                    return "";
                }
                if (!Ext.isDate(v)) {
                    v = new Date(Date.parse(v));
                }
                v = v.fixDateTime(v);
                return v.dateFormat(format || Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern);
            },
            moneyRenderer: function (formatInfo) {
                return function (v) {
                    return Ext.util.Format.money(v, formatInfo);
                };
            },
            money: function (v, formatInfo, roundNumber) {
                if (!formatInfo) {
                    return v;
                }
                v = Ext.num(v, NaN);
                if (isNaN(v)) {
                    return '';
                }
                var neg = v < 0;

                var fnum = this.getNumberLiteral(v, formatInfo, roundNumber);

                // COMMENT {all, 13.11.2014}: Костыль, связанный с тем, что есть потребность трёхбуквенные коды располагать не так, как указано в системных настройках культуры.
                var format = neg
                    ? (Ext.CurrencyFormat.Negative[formatInfo.CurrencySymbol] || formatInfo.CurrencyNegativePattern)
                    : (Ext.CurrencyFormat.Positive[formatInfo.CurrencySymbol] || formatInfo.CurrencyPositivePattern);

                return format.replace('-', formatInfo.NegativeSign)
                             .replace('n', fnum)
                             .replace('$', formatInfo.CurrencySymbol);
            },

            exNumberRenderer: function (formatInfo) {
                return function (v) {
                    return Ext.util.Format.exNumber(v, formatInfo);
                };
            },
            //Форматирование чисел в соответствии с форматами .NET
            exNumber: function (v, formatInfo, roundNumber) {
                if (!formatInfo) {
                    return v;
                }
                v = Ext.num(v, NaN);
                if (isNaN(v)) {
                    return '';
                }
                var neg = v < 0;

                var fnum = this.getNumberLiteral(v, formatInfo, roundNumber);

                return neg ? formatInfo.NumberNegativePattern.replace('-', formatInfo.NegativeSign).replace('n', fnum) : fnum;
            },
            getNumberLiteral: function (v, formatInfo, roundNumber) {
                v = Math.abs(v);
                var round = roundNumber === undefined ? true : roundNumber;

                v = round ? v.toFixed(formatInfo.NumberDecimalDigits) : v.toString();

                var fnum = v.toString();

                var psplit = fnum.split('.');
                var groupSize = formatInfo.NumberGroupSizes[0];

                var cnum = psplit[0],
                parr = [],
                j = cnum.length,
                m = Math.floor(j / groupSize),
                n = cnum.length % groupSize || groupSize,
                i;

                for (i = 0; i < j; i += n) {
                    if (i != 0) {
                        n = groupSize;
                    }

                    parr[parr.length] = cnum.substr(i, n);
                    m -= 1;
                }
                fnum = parr.join(formatInfo.NumberGroupSeparator);

                if (psplit[1]) {
                    fnum += formatInfo.NumberDecimalSeparator + psplit[1];
                }
                return fnum;
            },
            
            booleanRenderer: function () {
                return function (v) {
                    if (v)
                        return Ext.LocalizedResources.Yes;
                    else
                        return Ext.LocalizedResources.No;
                };
            }
        });
    }
}();
//Локализация Ext.js
var localize = function () {

    Ext.UpdateManager.defaults.indicatorText = '<div class="loading-indicator">' + Ext.LocalizedResources.IndicatorText + '</div>';

    if (Ext.View) {
        Ext.View.prototype.emptyText = "";
    }
    if (Ext.grid.Grid) {
        Ext.grid.Grid.prototype.ddText = Ext.LocalizedResources.GridDDText;
    }
    if (Ext.TabPanelItem) {
        Ext.TabPanelItem.prototype.closeText = Ext.LocalizedResources.TabPanelItemCloseText;
    }
    if (Ext.form.Field) {
        Ext.form.Field.prototype.invalidText = Ext.LocalizedResources.FieldInvalidText;
    }
    if (Ext.LoadMask) {
        Ext.LoadMask.prototype.msg = Ext.LocalizedResources.LoadMask;
    }
    Date.monthNames = Ext.CultureInfo.DateTimeFormatInfo.MonthNames;
    Date.monthNumbers = {

    };
    for (var i = 0; i < 12; i++) {
        var shorMonthName = Date.getShortMonthName(i);
        var firstLetter = shorMonthName.substring(0, 1);
        var remainingLetters = shorMonthName.substring(1, shorMonthName.length);
        Date.monthNumbers[firstLetter.toUpperCase() + remainingLetters.toLowerCase()] = i;
    }
    Date.dayNames = Ext.CultureInfo.DateTimeFormatInfo.DayNames;
    if (Ext.MessageBox) {
        Ext.MessageBox.buttonText = {
            ok: Ext.LocalizedResources.OK,
            Continue: Ext.LocalizedResources.Continue,
            cancel: Ext.LocalizedResources.Cancel,
            yes: Ext.LocalizedResources.Yes,
            no: Ext.LocalizedResources.No
        };
    }

    if (Ext.DatePicker) {
        Ext.apply(Ext.DatePicker.prototype, {
            todayText: Ext.LocalizedResources.DatePickerToday,
            minText: Ext.LocalizedResources.DatePickerMinText,
            maxText: Ext.LocalizedResources.DatePickerMaxText,
            disabledDaysText: Ext.LocalizedResources.DisabledDaysText,
            disabledDatesText: Ext.LocalizedResources.DisabledDatesText,
            monthNames: Date.monthNames,
            dayNames: Date.dayNames,
            nextText: Ext.LocalizedResources.DatePickerNextText,
            prevText: Ext.LocalizedResources.DatePickerPrevText,
            monthYearText: Ext.LocalizedResources.DatePickerMonthYearText,
            todayTip: Ext.LocalizedResources.DatePickerTodayTip,
            format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
            okText: "&#160;OK&#160;",
            cancelText: Ext.LocalizedResources.Cancel,
            startDay: Ext.CultureInfo.DateTimeFormatInfo.FirstDayOfWeek
        });
    }
    if (Ext.form.DateField) {
        Ext.apply(Ext.form.DateField.prototype, {
            disabledDaysText: Ext.LocalizedResources.DisabledDaysText,
            disabledDatesText: Ext.LocalizedResources.DisabledDatesText,
            minText: Ext.LocalizedResources.MinDateText,
            maxText: Ext.LocalizedResources.MaxDateText,
            invalidText: Ext.LocalizedResources.InvalidDateText,
            format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern,
            altFormats: Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern,
            startDay: Ext.CultureInfo.DateTimeFormatInfo.FirstDayOfWeek
        });
    }
    if (Ext.ux.Calendar) {
        Ext.apply(Ext.ux.Calendar.prototype,
            {
                monthlyUpperBoundText: Ext.LocalizedResources.MonthlyUpperBoundText,
                monthlyLowerBoundText: Ext.LocalizedResources.MonthlyLowerBoundText,
                startDay: Ext.CultureInfo.DateTimeFormatInfo.FirstDayOfWeek
            }
        );
    }
    if (Ext.PagingToolbar) {
        Ext.apply(Ext.PagingToolbar.prototype, {
            beforePageText: Ext.LocalizedResources.BeforePageText,
            afterPageText: Ext.LocalizedResources.AfterPageText,
            firstText: Ext.LocalizedResources.FirstPageText,
            prevText: Ext.LocalizedResources.PrevPageText,
            nextText: Ext.LocalizedResources.NextPageText,
            lastText: Ext.LocalizedResources.LastPageText,
            refreshText: Ext.LocalizedResources.RefreshText,
            displayMsg: Ext.LocalizedResources.DisplayMsg,
            emptyMsg: Ext.LocalizedResources.EmptyMsg
        });
    }
    if (Ext.form.HtmlEditor) {
        Ext.apply(Ext.form.HtmlEditor.prototype, {
            createLinkText: Ext.LocalizedResources.CreateLinkText,
            buttonTips: {
                bold: {
                    title: Ext.LocalizedResources.BoldTitle,
                    text: Ext.LocalizedResources.BoldText,
                    cls: 'x-html-editor-tip'
                },
                italic: {
                    title: Ext.LocalizedResources.ItalicTitle,
                    text: Ext.LocalizedResources.ItalicText,
                    cls: 'x-html-editor-tip'
                },
                underline: {
                    title: Ext.LocalizedResources.UnderlineTitle,
                    text: Ext.LocalizedResources.UnderlineText,
                    cls: 'x-html-editor-tip'
                },
                increasefontsize: {
                    title: Ext.LocalizedResources.IncreaseFontSizeTitle,
                    text: Ext.LocalizedResources.IncreaseFontSizeText,
                    cls: 'x-html-editor-tip'
                },
                decreasefontsize: {
                    title: Ext.LocalizedResources.DecreaseFontSizeTitle,
                    text: Ext.LocalizedResources.DecreaseFontSizeText,
                    cls: 'x-html-editor-tip'
                },
                backcolor: {
                    title: Ext.LocalizedResources.BackColorTitle,
                    text: Ext.LocalizedResources.BackColorText,
                    cls: 'x-html-editor-tip'
                },

                forecolor: {
                    title: Ext.LocalizedResources.ForeColorText,
                    text: Ext.LocalizedResources.ForeColorText,
                    cls: 'x-html-editor-tip'
                },
                justifyleft: {
                    title: Ext.LocalizedResources.JustifyLeftTitle,
                    text: Ext.LocalizedResources.JustifyLeftText,
                    cls: 'x-html-editor-tip'
                },
                justifycenter: {
                    title: Ext.LocalizedResources.JustifyCenterTitle,
                    text: Ext.LocalizedResources.JustifyCenterText,
                    cls: 'x-html-editor-tip'
                },
                justifyright: {
                    title: Ext.LocalizedResources.JustifyRightTitle,
                    text: Ext.LocalizedResources.JustifyRightText,
                    cls: 'x-html-editor-tip'
                },
                insertunorderedlist: {
                    title: Ext.LocalizedResources.InsertUnorderedListTitle,
                    text: Ext.LocalizedResources.InsertUnorderedListText,
                    cls: 'x-html-editor-tip'
                },
                insertorderedlist: {
                    title: Ext.LocalizedResources.InsertOrderedListTitle,
                    text: Ext.LocalizedResources.InsertOrderedListText,
                    cls: 'x-html-editor-tip'
                },
                createlink: {
                    title: Ext.LocalizedResources.CreateLinkTitle,
                    text: Ext.LocalizedResources.CreateLinkText,
                    cls: 'x-html-editor-tip'
                },
                sourceedit: {
                    title: Ext.LocalizedResources.SourceEditTitle,
                    text: Ext.LocalizedResources.SourceEditText,
                    cls: 'x-html-editor-tip'
                }
            }
        });
    }

    /*if (Ext.form.TextField) {
    Ext.apply(Ext.form.TextField.prototype, {
    minLengthText: "Минимальная длина этого поля {0}",
    maxLengthText: "Максимальная длина этого поля {0}",
    blankText: "Это поле обязательно для заполнения",
    regexText: "",
    emptyText: null
    });
    }

    if (Ext.form.NumberField) {
    Ext.apply(Ext.form.NumberField.prototype, {
    minText: "Значение этого поля не может быть меньше {0}",
    maxText: "Значение этого поля не может быть больше {0}",
    nanText: "{0} не является числом"
    });
    }

    if (Ext.form.ComboBox) {
    Ext.apply(Ext.form.ComboBox.prototype, {
    loadingText: "Загрузка...",
    valueNotFoundText: undefined
    });
    }

    if (Ext.form.VTypes) {
    Ext.apply(Ext.form.VTypes, {
    emailText: 'Это поле должно содержать адрес электронной почты в формате "user@domain.com"',
    urlText: 'Это поле должно содержать URL в формате "http:/' + '/www.domain.com"',
    alphaText: 'Это поле должно содержать только латинские буквы и символ подчеркивания "_"',
    alphanumText: 'Это поле должно содержать только латинские буквы, цифры и символ подчеркивания "_"'
    });
    }

    if (Ext.grid.GridView) {
    Ext.apply(Ext.grid.GridView.prototype, {
    sortAscText: "Сортировать по возрастанию",
    sortDescText: "Сортировать по убыванию",
    lockText: "Закрепить столбец",
    unlockText: "Снять закрепление столбца",
    columnsText: "Столбцы"
    });
    }

    if (Ext.grid.PropertyColumnModel) {
    Ext.apply(Ext.grid.PropertyColumnModel.prototype, {
    nameText: "Название",
    valueText: "Значение",
    dateFormat: "j.m.Y"
    });
    }

    if (Ext.layout.BorderLayout.SplitRegion) {
    Ext.apply(Ext.layout.BorderLayout.SplitRegion.prototype, {
    splitTip: "Тяните для изменения размера.",
    collapsibleSplitTip: "Тяните для изменения размера. Двойной щелчок спрячет панель."
    });
    }
    */
}();

// Полезные расширения стандартных объектов.

// Возвращает из массива первый подходящий под критерий поиска объект или null, если такого нет.
// callback: Функция, принимающая объект из массива, возвращаяющая результат проверки по критерию.
Array.prototype.findOne = function(callback) {
    for (var i = 0; i < this.length; i++) {
        var item = this[i];
        if (callback(item)) return item;
    }
    return null;
};

// Удаляет из массива первый подходящий под критерий поиска объект, и возвращает true, если такой объект найден, иначе - false. 
// predicate: Функция, принимающая объект из массива, возвращаяющая результат проверки по критерию.
Array.prototype.removeFirstWhere = function(predicate) {
    for (var i = 0; i < this.length; i++) {
        if (predicate(this[i]) === true) {
            this.splice(i, 1);
            return true;
        }
    }
    return false;
};

// https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Array/forEach
if (!Array.prototype.forEach) {
    Array.prototype.forEach = function(fn, scope) {
        for (var i = 0, len = this.length; i < len; ++i) {
            fn.call(scope, this[i], i, this);
        }
    };
}

function isNumber (value) {
    return parseFloat(value) == value;
}