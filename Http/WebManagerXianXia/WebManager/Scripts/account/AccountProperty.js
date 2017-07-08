define(function (require, exports, module) {
$(function () {

    var alertJs = require('../alert.js');
    var oResultTable = $('#MainContent_m_result');

    (function () {

        var oModifyName = $('#divModifyName');
        var oTextAccount = $('#txtAccount');
        var oTextNewName = $('#txtNewName');

        $(window).resize(function () {
            initModifyDialog();
        });
        $(window).scroll(function () {
            initModifyDialog();
        });

        $('.close').click(function () {

            oModifyName.hide();
        });

        $('#btnModifyName').click(function () {

            reqOp({
                    op: 1, acc: oTextAccount.val(), param: oTextNewName.val()
                  },
                function (data) {

                    $('#opRes').html(data);

                });
        });

        function initModifyDialog()
        {
            oModifyName.css('width', $(window).width()).css('height', $(window).height());
            oModifyName.css('top', $(document).scrollTop()).css('left', $(document).scrollLeft());
        }

        initModifyDialog();

        oResultTable.find('a[id^=aname_]').click(function () {

            var acc = $(this).attr('acc');
            oTextAccount.val(acc);
            oTextNewName.val('');
            $('#opRes').html('');
            oModifyName.show();
        });

        
    })();

    (function () {

        oResultTable.find('input[id^=astate_]').click(function () {

            var $this = $(this);
            var acc = $this.attr('acc');
            var param = $this.attr('op');

            reqOp({ 'op': 4, 'acc': acc, 'param': param },

                function (data) {

                    alertJs.doModal('', data);

                });
        });

    })();

    function reqOp(param, callBack) {
        $.ajax({
            type: "POST",
            url: "/ashx/ModifyGmProperty.ashx",
            data: param,

            success: function (data) {
                callBack(data);
            }
        });
    }


});
});
