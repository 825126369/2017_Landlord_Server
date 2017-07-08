define(function (require, exports, module) {
$(function () {

    var headTable = $('#MainContent_LogTable');

    $('#btnSelAll').click(function () {
        
        headTable.find('input[type=checkbox]').attr('checked', true);
    });

    $('#btnCancelSelAll').click(function () {

        headTable.find('input[type=checkbox]').attr('checked', false);
    });

    $('#btnDelLog').click(function () {

        var ok = confirm("确定删除所选日志?");
        if (!ok)
            return;

        headTable.find('input[type=checkbox]').each(function () {

            if($(this).is(':checked'))
            {
                reqDel($(this).attr('value'), function (data) {

                    var d = JSON.parse(data);
                    if(d.result == 0)
                    {
                        $('#MainContent_' + d.id).remove();
                    }

                });
            }
        });
    });

    function reqDel(param, callBack) {
        $.ajax({
            type: "POST",
            url: "/ashx/LogViewer.ashx",
            data: { 'param': param },

            success: function (data) {
                callBack(data);
            }
        });
    }

});
});


