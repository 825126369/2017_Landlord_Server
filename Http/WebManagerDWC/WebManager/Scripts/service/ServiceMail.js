$(document).ready(function ()
{
    var sel = $("#MainContent_service_common_m_target").get(0).selectedIndex;
    showFullSendCond(sel);

    $("#MainContent_service_common_m_target").change(function ()
    {
        var v = $(this).get(0).selectedIndex;
        showFullSendCond(v);
    });
});

function showFullSendCond(show)
{
    if(show == 1)
    {
        $("#divFullCond").show();
    }
    else
    {
        $("#divFullCond").hide();
    }
}
