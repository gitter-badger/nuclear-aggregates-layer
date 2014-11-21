Ext.layout.MSOfficeAccordionLayout = Ext.extend(Ext.layout.FitLayout, {
    fill: true,
    autoWidth: true,
    titleCollapse: true,
    hideCollapseTool: false,
    collapseFirst: false,
    animate: false,
    sequence: false,
    activeOnTop: false,
    type: 'msofficeaccordion',
    icon: undefined,
    renderItem: function (c)
    {
        if (this.animate === false)
        {
            c.animCollapse = false;
        }
        c.collapsible = true;
        if (this.autoWidth)
        {
            c.autoWidth = true;
        }
        if (this.titleCollapse)
        {
            c.titleCollapse = true;
        }
        if (this.hideCollapseTool)
        {
            c.hideCollapseTool = true;
        }
        if (this.collapseFirst !== undefined)
        {
            c.collapseFirst = this.collapseFirst;
        }
        if (!this.activeItem && !c.collapsed)
        {
            this.setActiveItem(c, true);
        } else if (this.activeItem && this.activeItem != c)
        {
            c.collapsed = true;
        }
        c.on('render', this.onRender, c);

        Ext.layout.MSOfficeAccordionLayout.superclass.renderItem.apply(this, arguments);
        c.header.addClass('x-office-accordion-hd');
        c.on('beforeexpand', this.beforeExpand, this);
        c.header.on('mouseover', this.mouseOver, c.header);
        c.header.on('mouseout', this.mouseOut, c.header);

        this.activeItem.header.addClass("x-office-accordion-hd-selected");
    },
    onRender: function ()
    {
        this.header.dom.innerHTML = '<table class="x-office-accordion-hd-table"><tbody><tr><td style="width:24px; height: 24px;">' +
        (this.icon?'<img class="x-office-accordion-hd-icon" src="'+this.icon+'"/>':'')+
        '<td>' + this.header.dom.innerHTML + '</td></tr></tbody></table>';
    },
    onRemove: function (c)
    {
        Ext.layout.AccordionLayout.superclass.onRemove.call(this, c);
        if (c.rendered)
        {
            c.header.removeClass('x-office-accordion-hd');
        }
        c.un('beforeexpand', this.beforeExpand, this);
        c.header.un('mouseover', this.mouseOver, c.header);
        c.header.un('mouseout', this.mouseOut, c.header);
    },

    mouseOver: function (e, c)
    {
        var header = this;
        if (!header.hasClass("x-office-accordion-hd-selected")) { header.addClass("x-office-accordion-hd-over"); }
        header = null;
    },
    mouseOut: function (e, c)
    {
        var header = this;
        header.removeClass("x-office-accordion-hd-over");
        header = null;
    },
    // private
    beforeExpand: function (p, anim)
    {

        var ai = this.activeItem;
        if (ai)
        {
            ai.header.removeClass("x-office-accordion-hd-selected");
            if (this.sequence)
            {
                delete this.activeItem;
                if (!ai.collapsed)
                {
                    ai.collapse({ callback: function ()
                    {
                        p.expand(anim || true);
                    }, scope: this
                    });
                    return false;
                }
            } else
            {
                ai.collapse(this.animate);
            }
        }

        p.header.removeClass("x-office-accordion-hd-over");
        p.header.addClass("x-office-accordion-hd-selected");

        this.setActive(p);
        if (this.activeOnTop)
        {
            p.el.dom.parentNode.insertBefore(p.el.dom, p.el.dom.parentNode.firstChild);
        }
        // Items have been hidden an possibly rearranged, we need to get the container size again.
        this.layout();
    },

    // private
    setItemSize: function (item, size)
    {
        if (this.fill && item)
        {
            var hh = 0, i, ct = this.getRenderedItems(this.container), len = ct.length, p;
            // Add up all the header heights
            for (i = 0; i < len; i++)
            {
                if ((p = ct[i]) != item && !p.hidden)
                {
                    hh += p.header.getHeight();
                }
            };
            // Subtract the header heights from the container size
            size.height -= hh;
            // Call setSize on the container to set the correct height.  For Panels, deferedHeight
            // will simply store this size for when the expansion is done.
            item.setSize(size);
        }
    },

    setActiveItem: function (item)
    {
        this.setActive(item, true);
    },

    // private
    setActive: function (item, expand)
    {
        var ai = this.activeItem;
        item = this.container.getComponent(item);
        if (ai != item)
        {
            if (item.rendered && item.collapsed && expand)
            {
                item.expand();
            } else
            {
                if (ai)
                {
                    ai.fireEvent('deactivate', ai);
                }
                this.activeItem = item;
                item.fireEvent('activate', item);
            }
        }
    }
});
Ext.Container.LAYOUTS.msofficeaccordion = Ext.layout.MSOfficeAccordionLayout;
//backwards compat
Ext.layout.msofficeaccordion = Ext.layout.MSOfficeAccordionLayout;