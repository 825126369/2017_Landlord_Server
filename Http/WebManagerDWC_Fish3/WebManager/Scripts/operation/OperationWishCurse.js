$(document).ready(function () {
    var opId = "#MainContent_operation_common_m_opType";

    var sel = $(opId).get(0).selectedIndex;
    show(sel);

    $(opId).change(function () {
        var v = $(this).get(0).selectedIndex;
        show(v);
    });

    var OP_VIEW_BUFF = 2;
    var OP_REMOVE_BUFF = 1;
    var BTN_ID = 0;

    // 刷新玩家列表
    $('#btnRefresh').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/WishCurse.ashx",
            data: { op: OP_VIEW_BUFF },

            success: function (data) {
                processRet(data);
            }
        });

    });

    function processRet(data)
    {
        var obj = JSON.parse(data);
        var op = obj.op;

        switch(op)
        {
            case OP_VIEW_BUFF:
                {
                    viewBuff(obj);
                }
                break;
            case OP_REMOVE_BUFF:
                {
                    removeBuff(obj);
                }
                break;
        }
    }

    function viewBuff(jsonObj) {

        $('#bufferPlayerList').html('');
        var head = '<tr><td>玩家ID</td><td>buff类型</td><td>数值</td><td>添加时间</td><td>操作</td></tr>';
        var $bufferPlayerList = $('#bufferPlayerList');
        $(head).appendTo($bufferPlayerList);
        var bList = JSON.parse(jsonObj.buffList);

       // console.log(bList);
        for (var i = 0; i < bList.length; i++)
        {
            var obj = bList[i];
            console.log(obj);

            var btnId = "RemovePlayerBuff" + BTN_ID;
            BTN_ID++;

            var tstr = '<tr id="' + obj.playerId + '">' + '<td>' + obj.playerId + '</td>' + '<td>' + getBuffType(obj) + '</td>' +
           '<td>' + obj.value + '</td>' + '<td>' + obj.genTime + '</td>' +
           '<td>' + '<input type="button" id="' + btnId + '" value="移除BUFF"/>' + '</td>' + '</tr>';

            var t = $(tstr);
            $bufferPlayerList.append(t);

            $('#' + btnId).click(function () {
               
                $.ajax({
                    type: "POST",
                    url: "/ashx/WishCurse.ashx",
                    data: { op: OP_REMOVE_BUFF, "playerId":$(this).parent().parent().find('td').eq(0).html()
                    },

                    success: function (data) {
                        processRet(data);
                    }
                });

            });

        }
    }

    function removeBuff(obj) {
       
        if(obj.result == 0)
        {
            $('#bufferPlayerList').find('tr[id='+obj.playerId+']').remove();
        }
    }

    function getBuffType(obj) {
        if (obj.value < 0)
            return "诅咒";

        return "祝福";
    }
});

function show(show) {
    if (show == 0) {
        $(".tdAlt").show();
    }
    else {
        $(".tdAlt").hide();
    }
}

