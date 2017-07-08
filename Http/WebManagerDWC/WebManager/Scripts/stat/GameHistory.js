$(document).ready(function () {
    var sel = $("#m_whichGame").val();
    showXue(sel);

    $("#m_whichGame").change(function () {
        var v = $(this).val();
        showXue(v);
    });
});

function showXue(show) {
    if (show == 5) {
        $(".cXueBound").show();
    }
    else {
        $(".cXueBound").hide();
    }
}
