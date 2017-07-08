define(function (require, exports, module) {
$(function () {

    var scoreConfirmM = require('../score_confirm.js');

    $('#MainContent_m_result input[type=button]').click(function () {

        var $this = $(this);
        var op = $this.attr('op');
        var targetType = $this.attr('targettype');

        if (op == 'add')
        {
            addScore($this.attr('acc'), op, targetType);
        }
        else
        {
            desScore($this.attr('acc'), op, targetType);
        }
    });

    function addScore(playerAcc, op, targetType)
    {
        var opScore = parseInt($("#MainContent_txt" + playerAcc).val());
        scoreConfirmM.confirmScore({

            isAdd: true,
            score: opScore,
            dstAcc: playerAcc,
            title: '确认',
            fun: function (sel) {

                if(sel == scoreConfirmM.OK)
                {
                    doScore(playerAcc, opScore, 0, targetType)
                }
            }
        });
    }

    function desScore(playerAcc, op, targetType)
    {
        var opScore = parseInt($("#MainContent_txt" + playerAcc).val());
        scoreConfirmM.confirmScore({

            isAdd: false,
            score: opScore,
            dstAcc: playerAcc,
            title: '确认',
            fun: function (sel) {

                if (sel == scoreConfirmM.OK) {
                    doScore(playerAcc, opScore, 1, targetType)
                }
            }
        });
    }

    function doScore(playerAcc, score, op, targetType)
    {
        setTipInfo('操作中,请稍候...', playerAcc);
        $.ajax({
            type: "POST",
            url: "/ashx/AccountScoreOp.ashx",
            data: { 'acc': playerAcc, "op": op, "param": score, 'targetType': targetType },

            success: function (data) {
                var d = JSON.parse(data);
                if (d.result == 0)
                {
                    $('#tdRemainMoney').html(d.remainScoreStr);
                    $("#MainContent_RemainMoney_" + playerAcc).html(d.remainScoreFixBug);
                    var element = document.getElementById("MainContent_txt" + playerAcc);
                    if (element) {
                        element.value = "";
                    }
                }
                setTipInfo(d.resultInfo, playerAcc);
            }
        });
    }

    function setTipInfo(info, playerAcc)
    {
        var rid = '#MainContent_Label' + playerAcc;
        $(rid).css('color', 'red');
        $(rid).html(info);
    }

});
});
