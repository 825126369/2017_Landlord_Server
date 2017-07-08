define(function (require, exports, module) {

// isAdd是否上分 toWho操作目标 gm or player
function confirmScore(isAdd, toWho, dstAcc)
{
    var isAdmin = parseInt($("#MainContent_m_isAdmin").val());

    var str = "目标账号: " + getAccount(toWho, dstAcc) + "\n";

    var curMoney = parseInt($("#MainContent_m_curMoney").val());
    var score = getScore(toWho, dstAcc);

    return confirmScoreImp(isAdd, isAdmin, curMoney, score, dstAcc);
}

function isUserAdmin(flag)
{
    if(flag == 0 || flag==7)
        return true;
    return false;
}

function getAccount(toWho, dstAcc)
{
    if (toWho == "player") // 给player上分下分
    {
        return dstAcc;
    }

    // 给GM上分下分
    return $("#MainContent_m_accList").val();
}

// 取得加减的分数
function getScore(toWho, dstAcc)
{
    if (toWho == "player") // 给player上分下分
    {
        return parseInt($("#MainContent_txt" + dstAcc).val());
    }

    // 给GM上分下分
    return parseInt($("#MainContent_m_score").val());
}

function isScoreValid(score)
{
    if (isNaN(score))
    {
        return false;
    }
    if (score < 0)
    {
        return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////
/* 
        isAdd       是否上分 
        isAdmin     是否管理员操作
        curMoney    操作者的当前分数
        score       上下分的金额
        dstAcc      操作的目标账号
*/
function confirmScoreImp(isAdd, isAdmin, curMoney, score, dstAcc) {
    //var isAdmin = parseInt($("#MainContent_m_isAdmin").val());

    var str = "目标账号: " + dstAcc + "\n";

    if (!isScoreValid(score)) {
        alert("分数需要填写正确");
        return false;
    }

    if (!isUserAdmin(isAdmin)) {
        str += "当前余额: " + curMoney + "\n";
    }

    if (isAdd) {
        str += "操作类型: 上分\n";

        if (!isUserAdmin(isAdmin)) {
            if (curMoney < score) {
                alert('你当前余额不足');
                return false;
            }
        }
    }
    else {
        str += "操作类型: 下分\n";
    }
    str += "操作金额: " + score + "\n";

    if (!isUserAdmin(isAdmin)) {
        var remain = 0;
        if (isAdd) {
            remain = curMoney - score;
        }
        else {
            remain = curMoney + score;
        }
        str += "操作后余额:  " + remain + "\n";
    }

    str += "是否确定操作";

    return confirm(str);
}

exports.confirmScore = confirmScore;
});
