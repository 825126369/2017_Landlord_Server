define(function(require, exports, module) {
$(function () {

    function setStartInfo(id, info) {
        $(id).text(info);
    }

    var idExpRate = '#MainContent_stat_common_StatCommonShcd_Content_txtExpRate';
    var idExpTable = '#MainContent_stat_common_StatCommonShcd_Content_m_expRateTable';
    var idRes = '#MainContent_stat_common_StatCommonShcd_Content_m_res';
    var expCheat = /^\s*(\d+)\s+(\d+)\s*$/;

    var commonJs = require('../common.js');

    $("#btnModifyLevel").click(function () {
        var roomList = getRoomList();
        if (!roomList)
        {
            alert('请选择房间!');
            return;
        }

        $.ajax({
            type: "POST",
            url: "/ashx/ShcdControl.ashx",
            data: { roomId: roomList, level: $('#level').val(), op: 1 },

            success: function (data) {
                commonJs.checkAjaxScript(data);

                var arr = data.split('#');
                $(idRes).text(arr[1]);

                //var tbody = $(idExpTable).find("tbody").eq(0);
                //tbody.children("tr").eq(1).children("td").eq(6).text(arr[1]);
                if(arr[0]==0)
                {
                    setSuccessData(arr[3], 6, arr[2]);
                }
            }
        });
    });

    // 修改大小王个数
    $("#btnModifyJokerCount").click(function () {
        var roomList = getRoomList();
        if (!roomList) {
            alert('请选择房间!');
            return;
        }

        var count = parseInt($(idExpRate).val());
        if (isNaN(count)) {
            alert("输入非法");
            return;
        }

        if (count < 0 || count > 8) {
            alert('请输入0-8之间的整数')
            return;
        }

        $.ajax({
            type: "POST",
            url: "/ashx/ShcdControl.ashx",
            data: { roomId: roomList, level: count, op: 2 },

            success: function (data) {
                commonJs.checkAjaxScript(data);

                var arr = data.split('#');
                if (arr[0] == 0) {
                    //var td = $(idExpTable).find("tr").eq(1).find('td').eq(7);
                    //td.text(arr[2]);
                    setSuccessData(arr[3], 7, arr[2]);
                }
                $(idRes).text(arr[1]);
            }
        });
    });

    // 修改作弊开始结束局数
    $("#btnCheat").click(function () {

        var roomList = getRoomList();
        if (!roomList) {
            alert('请选择房间!');
            return;
        }

        var cheatVal = $(idExpRate).val();
        if (!expCheat.test(cheatVal))
        {
            alert('格式非法，应为 起始局数 + 空格 + 结束局数');
            return;
        }
        var arr = cheatVal.match(expCheat);
        var start = Math.min(arr[1], arr[2]);
        var end = Math.max(arr[1], arr[2]);
        if (start < 0 || start > 99 || end < 0 || end > 99)
        {
            alert('应在 0--99范围内');
            return;
        }
        //console.log(start + '--' + end);

        $.ajax({
            type: "POST",
            url: "/ashx/ShcdControl.ashx",
            data: { roomId: roomList, level: start + '-' + end, op: 3 },

            success: function (data) {
                commonJs.checkAjaxScript(data);

                var arr = data.split('#');
                if (arr[0] == 0) {
                    //var td = $(idExpTable).find("tr").eq(1).find('td').eq(8);
                    //td.text(arr[2]);

                    setSuccessData(arr[3], 8, arr[2]);
                }
                $(idRes).text(arr[1]);
            }
        });
        
    });

    $("#level").change(function () {
        var v = $(this).val();

        if (v === "0")
        {
            $("#MainContent_stat_common_StatCommonShcd_Content_Button2").attr("disabled", false);
        }
        else
        {
            $("#MainContent_stat_common_StatCommonShcd_Content_Button2").attr("disabled", true);
        }
    });

    function getRoomList()
    {
        var str = '';
        var first = true;
        $(idExpTable + ' input[type=checkbox]').each(function (i, elem) {

            if ($(elem).is(':checked'))
            {
                if (first)
                {
                    str += $(elem).val();
                    first = false;
                }
                else
                {
                    str += ',' + $(elem).val();
                }
            }
        });
        return str;
    }

    function setSuccessData(roomList, tdIndex, newValue)
    {
        var aroom = roomList.split(',');
        console.log(aroom + '...........' + tdIndex);
        for(var i = 0; i < aroom.length; i++)
        {
            var td = $(idExpTable).find("tr").eq(parseInt(aroom[i])).find('td').eq(tdIndex);
            td.text(newValue);
        }
    }

});


});