define(function (require, exports, module) {
$(function () {

    var OK = 'ok';
    var CANCEL = 'cancel';

    var oConfirmDlg = $('#_scoreOpConfirmDlg');
    var oTitle = $('#_scoreOpConfirmDlgTitle');
    var oContent = $('#_scoreOpConfirmDlgContent');

    var oCancel = oConfirmDlg.find('.Cancel');
    var oOk = oConfirmDlg.find('.Ok');

    var g_sel = '';

    oCancel.click(function () {
        g_sel = CANCEL;
    });

    oOk.click(function () {

        g_sel = OK;
    });

    function doModal(title, content, callBack)
    {
        g_sel = '';
        oTitle.html(title);
        oContent.html(content);

        oConfirmDlg.off();

        oConfirmDlg.on('hidden.bs.modal', function () {

            if (callBack != null)
            {
                callBack(g_sel);
            }
        });

        oConfirmDlg.modal();
    }

    // param {isAdd, score, dstAcc, title, fun}
    function confirmScore(param) {

        var html = '';

        var str = cell('目标账号:') + cell(param.dstAcc);

        html += trow(str);
        
        if (!isScoreValid(param.score)) {
            doModal('错误', '分数需要填写正确', null);
            return;
        }

        var accType = parseInt($("#accType").val());
        var curMoney = convertToFloat($("#tdRemainMoney").text());

        if (param.isAdd)
        {
            str = cell('操作类型:') + cell('上分');
            html += trow(str);

            if (!isUserAdmin(accType))
            {
                if (curMoney < param.score)
                {
                    doModal('错误', '你当前余额不足', null);
                    return;
                }
            }
        }
        else
        {
            str = cell('操作类型:') + cell('下分');
            html += trow(str);
        }

        str = cell('操作金额:') + cell(param.score);
        html += trow(str);

        if (!isUserAdmin(accType)) {
            var remain = 0;
            if (param.isAdd) {
                remain = curMoney - param.score;
            }
            else {
                remain = curMoney + param.score;
            }

            str = cell('我的余额:') + cell(curMoney);
            html += trow(str);

            str = cell('操作后我的余额:') + cell(remain);
            html += trow(str);
        }

        doModal(param.title, table(html), param.fun);
    }

    function cell(txt)
    {
        var str = '<td>';
        str += txt;
        str += '</td>';
        return str;
    }

    function trow(txt)
    {
        var str = '<tr>';
        str += txt;
        str += '</tr>';
        return str;
    }

    function table(txt)
    {
        var str = '<table class="table">';
        str += txt;
        str += '</table>';
        return str;
    }

    function isUserAdmin(flag)
    {
        if(flag == 0 || flag==7)
            return true;
        return false;
    }

    function isScoreValid(score)
    {
        if (isNaN(score)) {
            return false;
        }
        if (score < 0) {
            return false;
        }
        return true;
    }

    function convertToFloat(str)
    {
        var tmp = str.replace(/,/g, '');
        return Number(tmp);
    }

    exports.OK = OK;
    exports.CANCEL = CANCEL;
    exports.confirmScore = confirmScore;

});
});
