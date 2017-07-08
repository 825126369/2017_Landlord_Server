$(function () {

    $("#btnGameOn").click(function () {

        var gameList = "";
        $("#MainContent_gameList").children("input[type=checkbox]").each(function () {
            if ($(this).is(':checked')) {
                if (gameList.length <= 0)
                {
                    gameList += $(this).val();
                }
                else
                {
                    gameList += "," + $(this).val();
                }
            }
        });

        $.ajax({
            type: "POST",
            url: "/ashx/GameOpen.ashx",
            data: { gameList: gameList},

            success: function (data) {
                alert(data);
            }
        });

    });

    /////////////////////////////////////////////////////////

    // 提交分配的权限
    $("#btnRight").click(function () {

        var accGm = $("#MainContent_m_acc").text();
        var rightStr = "";

        $("#MainContent_rightGroup").children("input[type=checkbox]").each(function () {
            if ($(this).is(':checked')) {
                rightStr += $(this).val() + ",";
            }
        });

        $.ajax({
            type: "POST",
            url: "/ashx/AccountRightAssign.ashx",
            data: { acc: accGm, rstr: rightStr },

            success: function (data) {
                alert(data);
            }
        });

    });

    /////////////////////////////////////////////////////////
    $('#apiLimit input[type=button]').click(function () {

        var newValue = $(this).parent().prev().find('input').val();
        var gameId = $(this).attr('gameId');
        var roomId = $(this).attr('roomId');
        var acc = $("#MainContent_m_acc").text();

        //console.log(gameId + '..' + roomId + '..' + newValue + ".." + acc);

        $.ajax({
            type: "POST",
            url: "/ashx/AccountAPISetLimit.ashx",
            data: { "gameId": gameId, "roomId": roomId, "value": newValue, 'acc': acc, "op": 0 },

            success: function (data) {

                var json = JSON.parse(data);
                if (json.result == 0)
                {
                    setAPISettingLimit(json.gameId, json.roomId, json.value);
                }
                alert(json.resStr);
            }
        });

    });

    $('#apiLimit span').click(function () {

        var acc = $("#MainContent_m_acc").text();
        $.ajax({
            type: "POST",
            url: "/ashx/AccountAPISetLimit.ashx",
            data: { "op": 1, 'acc': acc },

            success: function (data) {

                var json = JSON.parse(data);
                //console.log(json);
                var base = json.base;
                var data = json.limitList;
                for (var i = 0; i < data.length; i++)
                {
                    var gameId = data[i].gameId;
                    
                    for(var j = 1; j < 5; j++)
                    {
                        var value = data[i]['room' + j];
                        if (value) {
                           
                            setAPISettingLimit(gameId, j, value / base);
                        }
                    }
                }

            }
        });

    });

    function setAPISettingLimit(gameId, roomId, value)
    {
        $('input[gameId=' + gameId + ']').filter('input[roomId=' + roomId + ']').parent().prev().prev().html(value);
    }

    /////////////////////////////////////////////////////////
    $('#awModify input[type=button]').click(function () {

        Page_ClientValidate();
        if (!WebForm_OnSubmit())
            return;

        var id = $(this).attr('vid');
        var v = $(id).val();
        var op = $(this).attr('op');
        var acc = $("#MainContent_m_acc").text();
        //console.log(v + '.............' + op + '..........' + acc);

        $.ajax({
            type: "POST",
            url: "/ashx/ModifyGmProperty.ashx",
            data: {'acc': acc, "op": op, "param":v },

            success: function (data) {
                alert(data);
            }
        });
    });
});