define(function (require, exports, module) {
$(function () {

    var GROWTH = 0;
    var NET_GROWTH = 1;
    var oTime = $('#time');
    var oRankSel = $('#rankSel');
    var oRankWay = $('#rankWay');
    var resultDiv = $('#resultDiv');

    $('#btnQuery').click(function () {

        reqData(oTime.val(), oRankWay.val(), oRankSel.val(), function (data) {

            resultDiv.empty();
            var jd = JSON.parse(data);
            for(var d in jd)
            {
                $('<h2/>').appendTo(resultDiv).html(d).css({
                    "text-align": "center", "background": "#ccc",
                    "padding": "6px", "font-size": "30px"
                });
                $(jd[d]).attr('class', 'cTable').appendTo(resultDiv);
            }

        });

    });

    function reqData(time, rankWay, rankId, callBack)
    {
        $.ajax({
            type: "POST",
            url: "/ashx/OperationRank.ashx",
            data: { 'time': time, 'rankWay': rankWay, 'rankId': rankId },

            success: function (data) {
                callBack(data);
            }
        });
    }


});
});
