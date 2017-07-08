define(function (require, exports, module) {
$(function () {

    var oResultTable = $('#MainContent_m_result');

    (function () {

        var oModifyName = $('#divModifyName');
        var oTextAccount = $('#txtAccount');
        var oTextNewName = $('#txtNewName');
        var oInfoTitle = $('#infoTitle');
        var oInfoContent = $('#infoContent');

        var g_opName = '';

        $(window).resize(function () {
            initDialog();
        });
        $(window).scroll(function () {
            initDialog();
        });

        $('.close').click(function () {

            oModifyName.hide();
        });

        $('#btnModifyName').click(function () {

            reqOp({
                op: g_opName, acc: oTextAccount.val(), param: oTextNewName.val()
            },
                function (data) {

                    var jd = $.parseJSON(data);
                    $('#opRes').html(jd.resultMsg);

                });
        });

        function initDialog() {
            oModifyName.css('width', $(window).width()).css('height', $(window).height());
            oModifyName.css('top', $(document).scrollTop()).css('left', $(document).scrollLeft());
        }

        function initModify()
        {
            g_opName = 'modifyName';
            oInfoTitle.text('修改别名');
            oInfoContent.text('别名:');
        }

        function initResetPwd()
        {
            g_opName = 'resetPwd';
            oInfoTitle.text('重置密码');
            oInfoContent.text('新密码:');
        }

        oResultTable.find('a[id^=aname_]').click(function () {

            var acc = $(this).attr('acc');
            oTextAccount.val(acc);
            oTextNewName.val('');
            $('#opRes').html('');
            initModify();
            oModifyName.show();
        });

        oResultTable.find('a[id^=apwd_]').click(function () {

            var acc = $(this).attr('acc');
            oTextAccount.val(acc);
            oTextNewName.val('');
            $('#opRes').html('');
            initResetPwd();
            oModifyName.show();
        });

        initDialog();

    })();

    (function () {

        oResultTable.find('a[id^=arate_]').click(function () {

            var $this = $(this);
            var acc = $this.attr('acc');
            var param = $this.attr('param');

            reqOp({ op: 'affectRate', 'acc': acc, 'param': param },
                function (data) {
                    var jd = $.parseJSON(data);
                    alert(jd.resultMsg);
                });
        });

    })();

    (function () {

        oResultTable.find('a[id^=astate_]').click(function () {

            var $this = $(this);
            var acc = $this.attr('acc');
            var op = $this.attr('op');
            var param = $this.attr('param');

            reqOp({ 'op': op, 'acc': acc, 'param': param },
                function (data) {
                    var jd = $.parseJSON(data);
                    alert(jd.resultMsg);
                });
        });

    })();

    function reqOp(param, callBack) {
        $.ajax({
            type: "POST",
            url: "/ashx/PlayerOp.ashx",
            data: param,

            success: function (data) {
                callBack(data);
            }
        });
    }


});
});
