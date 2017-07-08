define(function (require, exports, module) {
$(function () {

    var OP_PASS='pass';

    var oResultTable = $('#MainContent_m_result');

    oResultTable.find('input[type=button]').click(function () {

        var op = $(this).attr('op');
        var acc = $(this).attr('acc');

        var res = ask(op, acc);
        if (!res)
            return;

        process(op, acc);
    });

    function process(op, acc)
    {
        var param = { 'op': op, 'acc': acc };
        reqOp(param, function (data) {

            var jd = $.parseJSON(data);
            $('#td_' + jd.acc).text(jd.resultMsg);
        });
    }

    function reqOp(param, callBack)
    {
        $.ajax({
            type: "POST",
            url: "/ashx/AccountApiApprove.ashx",
            data: param,

            success: function (data) {
                callBack(data);
            }
        });
    }

    function ask(op, acc)
    {
        var str = acc + '提交申请,';
        if(op == OP_PASS)
        {
            str += '确定同意?';
        }
        else
        {
            str += '确定拒绝?';
        }
        var res = confirm(str);
        return res;
    }

});
});
