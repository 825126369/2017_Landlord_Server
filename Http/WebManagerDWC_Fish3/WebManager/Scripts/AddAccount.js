define(function (require, exports, module) {
$(function () {

    $('input[id^=btnm]').click(function () {

        var acc = $(this).attr('acc');
        var id = $(this).attr('id').substr(4);
        //alert(id);
        var cid = '#MainContent_newTypeList' + id;
        
        
        req(0, acc, $(cid).val(), function (data) {

            alert(data);
        })

    });

    $('input[id^=btnd]').click(function () {

        var res = confirm('确定删除该账号?');
        if (!res)
            return;

        var acc = $(this).attr('acc');

        req(1, acc, '', function (data) {

            alert(data);
        })

    });

    function req(op, acc, gmType, callBack)
    {
        $.ajax({
            type: "POST",
            url: "/ashx/AddAccount.ashx",
            data: { 'acc': acc, 'gmType': gmType, 'op': op },

            success: function (data) {
                callBack(data);
            }
        });
    }
});

});
