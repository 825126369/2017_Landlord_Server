$(document).ready(function ()
{
    var sel = $("#MainContent_stat_common_m_statWay").get(0).selectedIndex;
    showTime(sel);

    $("#MainContent_stat_common_m_statWay").change(function ()
    {
        var v = $(this).get(0).selectedIndex;
        showTime(v);
    });
});

function showTime(show)
{
    if(show == 0)
    {
        $("#spanTime").show();
    }
    else
    {
        $("#spanTime").hide();
    }
}
