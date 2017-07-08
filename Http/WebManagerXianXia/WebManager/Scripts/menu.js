$(document).ready(function () {

    initState();
    $(".cdyMenu").click(function () {
        var display = $(this).next().css("display");
        var elem = $(this).next();
        var id = elem.attr("id");

        if (display == "none") {
            elem.show(500);
            addCookie(id, "opened");
            $(this).css("border", "none");
        }
        else {
            elem.hide(500);
            addCookie(id, "closed");
            $(this).css("border", "1px solid white");
        }
    });
});

function initState()
{
    showOrHide("divScore");
    showOrHide("divAccMgr");
    showOrHide("divReport");
    showOrHide("divAccView");
    showOrHide("divAdmFun");

    /*var t = parseInt($("#accType").val());
    if (t != 0 && t != 7)
    {
        $("#divStat").hide();
        $(".cMenuAdmin").hide();
    }
    else*/
    {
        showOrHide("divStat");
    }
}

function showOrHide(id)
{
    var s = $.cookie(id);

    if (s == "opened") {
        $("#" + id).show();

        $("#" + id).css("border", "none");
    }
    else {
        $("#" + id).hide();

        $("#" + id).prev().css("border", "1px solid white");
    }
}

function addCookie(id, val)
{
    $.cookie(id, val,
             { path: "/", expiress: 7 }
            );
}


