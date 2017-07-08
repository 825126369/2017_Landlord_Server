define(function (require, exports, module) {

$(function () {

    var OP_MODIFY_CAHNNEL_VIEW = 1;
    var OP_MODIFY_GM_TYPE = 2;
    var OP_DELETE = 3;

    var commonJS = require('./common.js');

    $('select[id^=channel_').multiselect({

        allSelectedText: '全部',
        nonSelectedText: '空',
        selectAllText: '全部',
        numberDisplayed: 2,
        includeSelectAllOption: true
    });
    
    $('input[id^=btnChannel_]').click(function () {

        var acc = $(this).attr('id').substring(11);
        console.log(acc);

        var channelId = '#channel_' + acc;
        var viewChannelArr = $(channelId).val();
        var viewChannel = '';
        if (viewChannelArr != null)
        {
            viewChannel = viewChannelArr.join(',');
        }
        
        console.log(viewChannel);

        reqOp(OP_MODIFY_CAHNNEL_VIEW, { 'viewChannel': viewChannel, 'acc': acc },
            function (data) {

                alert(data);

            });

    });
    
    $('input[id^=btnType_]').click(function () {

        var acc = $(this).attr('id').substring(8);
        //console.log(acc);

        var typeId = '#gmType_' + acc;
        var typeVal = $(typeId).val();
        //console.log(typeVal);

        reqOp(OP_MODIFY_GM_TYPE, { 'type': typeVal, 'acc': acc },
            function (data) {

                alert(data);

            });

    });

    $('input[id^=btnDel_]').click(function () {

        var acc = $(this).attr('id').substring(7);
        //console.log(acc);

        reqOp(OP_DELETE, { 'acc': acc },
            function (data) {

                alert(data);

            });

    });

    function reqOp(op, param, callBack) {
        param.op = op;

        $.ajax({
            type: "POST",
            url: "/ashx/AddAccount.ashx",
            data: param,

            success: function (data) {
                var res = commonJS.checkAjaxScript(data);
                if (res)
                    return;

                callBack(data);
            }
        });
    }


});

});