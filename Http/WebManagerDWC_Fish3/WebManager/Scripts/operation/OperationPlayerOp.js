$(document).ready(function () {

    $("table").eq(0).find("input[type=button]").each(function () {

        $(this).click(function () {
            
            var _this = $(this);
            var pid = $("#" + _this.attr("player")).val();
            var val = $("#" + _this.attr("val")).val();
            var opType = _this.attr("opType");
            var property = _this.attr("playerProp");

            $.ajax({
                type: "POST",
                url: "/ashx/PlayerOp.ashx",
                data: { op: opType, playerId: pid, prop: property, value: val },

                success: function (data) {
                    alert(data);
                }
            });
            
        });
    });

    $('#logFishRefresh').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/PlayerOp.ashx",
            data: { op: 'getLogFishList'},

            success: function (data) {
                
                var ul = $('#logFishRefresh').next();
                ul.empty();

                fillList(data, ul);
            }
        });
    });

    $('#logLimitDbRefresh').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/PlayerOp.ashx",
            data: { op: 'getLimitDbSendPlayer' },

            success: function (data) {

                var ul = $('#logLimitDbRefresh').next();
                ul.empty();

                fillList(data, ul);
            }
        });
    });

    function fillList(data, ul)
    {
        var arr = data.split(',');
        for (var i = 0; i < arr.length; i++) {
            var li = $('<li>');
            li.html(arr[i]).appendTo(ul);
        }
    }

});