var l = 0, t = 0;
document.onmousemove = getMouseXY;
var ns_tt = document.createElement("div");
ns_tt.className = "tooltip";

function getMouseXY(e) {
    if (Ext.isIE) {
        l = event.clientX + document.body.scrollLeft;
        t = event.clientY + document.body.scrollTop;
    }
    else {
        l = e.pageX;
        t = e.pageY;
    }
    ns_tt.style.left = l + "px";
    ns_tt.style.top = t + "px";
    return true;
}

function AddTooltip(tt_text) {
    if (tt_text && tt_text.length > 0) {
        document.body.appendChild(ns_tt);
        ns_tt.id = "ns_tt";
        ns_tt.innerHTML = tt_text;
    }
}

function RemoveTooltip() {
    var ttElement = document.getElementById("ns_tt");
    if (ttElement) {
        document.body.removeChild(ttElement);
    }
}