$(document).ready(function () {
    var opId = "#MainContent_operation_common_m_opType";

    var sel = $(opId).get(0).selectedIndex;
    show(sel);

    $(opId).change(function () {
        var v = $(this).get(0).selectedIndex;
        show(v);
    });
});

function show(show) {
    if (show == 0) {
        $(".tdAlt").show();
    }
    else {
        $(".tdAlt").hide();
    }
}

