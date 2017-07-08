$(document).ready(function () {

    var g_reqData = {};

    $("#divBetLimitOp li").each(function (i, elem) {

        this.gameIndex = i;
        
        this.onclick = function () {

            $("#divBetLimitOp li").each(function () {
                this.className = '';
            });

            this.className = 'active';

            $("#divBetLimitOp div").hide();
            $("#divBetLimitOp div").eq(this.gameIndex).show();

            var id = ($(this).attr("gameId"));
            reqData(id);
        };
    });


    $("#divBetLimitOp input[type=button]").click(function () {

        var gameId = $(this).parent().parent().parent().parent().parent().attr("gameId");
        var areaId = $(this).attr("area");
        var newValue = $(this).parent().prev().find("input").val();
        //console.log(gameId + "  " + areaId + "  " + newValue);
        if (newValue == '') {
            alert("请输入新的限制!");
            return;
        }
        if (gameId != 9)
        {
            if (isNaN(newValue) || newValue <= 0) {
                alert("非法输入");
                return;
            }
        }
       
        $.ajax({
            type: "POST",
            url: "/ashx/AccountMaxBetLimit.ashx",
            data: { "gameId":gameId,"areaId":areaId,"newValue":newValue,"op":1 },

            success: function (data) {
                processRet(data);
            }
        });


    });

    $("#divBetLimitOp a").click(function () {

        var gameId = $(this).parent().attr("gameId");
        
        $.ajax({
            type: "POST",
            url: "/ashx/AccountMaxBetLimit.ashx",
            data: { "gameId": gameId, "op": 2 },

            success: function (data) {
                alert(data);
            }
        });


    });

    function reqData(id)
    {
        if (g_reqData[id + ''] == undefined) {

            $.ajax({
                type: "POST",
                url: "/ashx/AccountMaxBetLimit.ashx",
                data: { "gameId": id, "op": 0 },

                success: function (data) {

                    processQueryResult(data);
                    g_reqData[id + ''] = true;

                }
            });

        }
        else {
           // console.log("aaaaaaaaaaaaaaaa");
        }
    }

    function processRet(data)
    {
        var arr = data.split('@');
        var code = arr[0];
        var res = arr[1];
        if (code == 0)
        {
            var gameId = arr[2];
            var areaId = arr[3];
            var newValue = arr[4];

           // console.log(gameId + "  " + areaId + "  " + newValue);

            setLimitValue(gameId, areaId, newValue);
        }
     
        alert(res);
    }

    function processQueryResult(data)
    {
        // console.log(data);
        if (data == '')
            return;

        var ret = JSON.parse(data);

        for (var attr in ret) {
            if (attr.indexOf('area') >= 0) {

                if (ret.gameId == 9)
                {
                    var rateList = ret[attr].split(",");
                    var resStr = [];
                    $.each(rateList, function (i, elem) {

                        // console.log(i + '  ' + elem);
                        
                        resStr.push(elem / ret.base);
                    });

                    setLimitValue(ret.gameId, attr.substring(4), resStr.join(','));
                }
                else
                {
                    setLimitValue(ret.gameId, attr.substring(4), ret[attr] / ret.base);
                }
            }
        }
    }

    function setLimitValue(gameId, areaId, newValue)
    {
       // console.log(gameId + "  " + areaId + "  " + newValue);

        var td = $("div[gameId=" + gameId + "]").find("input[area=" + areaId + "]").eq(0).parent().prev().prev();
        td.text(newValue);
    }

    reqData(3);
});
