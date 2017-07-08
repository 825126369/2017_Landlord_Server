$(document).ready(function ()
{
    return;

    $("#btnOpenClose1").click(function ()
    {
        var display = $(".cDiv1").css("display");

        if (display == "none")
        {
            $(".cDiv1").show(500);
        }
        else
        {
            $(".cDiv1").hide(500);
        }
    });

    $("#btnOpenClose2").click(function () {
        var display = $(".cDiv2").css("display");

        if (display == "none") {
            $(".cDiv2").show(500);
        }
        else {
            $(".cDiv2").hide(500);
        }
    });
});

// 上下分提示
function confirmScoreInfo(isAdd, toWho)
{
    var isAdmin = parseInt($("#MainContent_m_isAdmin").val());
    var curMoney = parseInt($("#MainContent_m_curMoney").val());
    var score = parseInt($("#MainContent_m_score").val());
    var dstAcc = $("#MainContent_m_acc").val();
    if (dstAcc == "")
    {
        alert("需要填写目标账号");
        return false;
    }

    return confirmScoreImp(isAdd, isAdmin, curMoney, score, dstAcc);
}


