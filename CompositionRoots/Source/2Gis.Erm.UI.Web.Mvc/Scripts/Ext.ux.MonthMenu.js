
Ext.ux.MonthMenu = Ext.extend(Ext.menu.Menu, {
	enableScrolling: false,
	hideOnClick: true,
	pickerId: null,
	cls: 'x-date-menu',

	initComponent: function () {
		if (this.strict = (Ext.isIE7 && Ext.isStrict)) {
			this.on('show', this.onShow, this, { single: true, delay: 20 });
		}
		Ext.apply(this, {
			plain: true,
			showSeparator: false,
			items: this.picker = new Ext.ux.MonthPicker(Ext.applyIf({
				ctCls: 'x-menu-date-item',
				id: this.pickerId
			}, this.initialConfig))
		});
		this.picker.purgeListeners();
		Ext.ux.MonthMenu.superclass.initComponent.call(this);
		/**
         * @event select
         * Fires when a date is selected from the {@link #picker Ext.DatePicker}
         * @param {DatePicker} picker The {@link #picker Ext.DatePicker}
         * @param {Date} date The selected date
         */
		this.relayEvents(this.picker, ['select', 'cancel']);
		this.on('show', this.picker.focus, this.picker);
		this.on('select', this.menuHide, this);
		this.on('cancel', this.menuHide, this);
	},

	menuHide: function () {
		if (this.hideOnClick) {
			this.hide(true);
		}
	},

	onShow: function () {
		var el = this.picker.getEl();
		el.setWidth(el.getWidth()); //nasty hack for IE7 strict mode
	}
});
Ext.reg('monthmenu', Ext.ux.MonthMenu);