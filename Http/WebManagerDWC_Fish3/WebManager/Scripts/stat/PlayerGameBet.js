define(function (require, exports, module) {
$(function () {

    var timerInput = $('#txtTime');
    var oPlayerId = $('#txtPlayerId');
    var gameList = $('#MainContent_stat_common_m_gameList');
    var resultTable = $('#resultTable');

    $('#btnQuery').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/PlayerGameBet.ashx",
            data: { time: timerInput.val(), playerId: oPlayerId.val(), gameId:gameList.val() },

            success: function (data) {
               // commonJs.checkAjaxScript(data);
                resultTable.html(data);
            }
        });

    });

});

});