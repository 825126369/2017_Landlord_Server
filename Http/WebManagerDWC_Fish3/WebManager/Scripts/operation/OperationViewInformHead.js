define(function (require, exports, module) {
$(function () {

    var headTable = $('#MainContent_operation_common_m_result');
    var idPrefix = '#MainContent_operation_common_dstPlayer';

    $('#btnSelAll').click(function () {
        
        headTable.find('input[type=checkbox]').prop('checked', true);
    });

    $('#btnCancelSelAll').click(function () {

        headTable.find('input[type=checkbox]').prop('checked', false);
    });

    $('#btnPass').click(function () {

        headTable.find('input[type=checkbox]').each(function () {

            if($(this).is(':checked'))
            {
                reqDel(0, $(this).attr('value'), function (data) {

                    var d = JSON.parse(data);
                    if(d.result == 0)
                    {
                        var id = idPrefix + d.playerId;
                        $(id).remove();
                    }

                });
            }
        });
    });

    $('input[id^=btnFreeze_]').click(function () {

        var id = $(this).attr('dstid');
        var info = $(this).parent().next().next();
        info.text('正在进行操作');

        reqDel(1, id, function (data) {

            info.text(data);
        });
    });

    function reqDel(opCode, playerList, callBack) {
        $.ajax({
            type: "POST",
            url: "/ashx/OperationViewInformHead.ashx",
            data: { 'playerList': playerList, op: opCode },

            success: function (data) {
                callBack(data);
            }
        });
    }

});
});


