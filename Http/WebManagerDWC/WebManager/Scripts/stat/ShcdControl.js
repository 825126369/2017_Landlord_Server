define(function(require, exports, module) {
$(document).ready(function () {

    function setStartInfo(id, info) {
        $(id).text(info);
    }

    var idExpRate = '#MainContent_stat_common_StatCommonShcd_Content_txtExpRate';
    var idExpTable = '#MainContent_stat_common_StatCommonShcd_Content_m_expRateTable';
    var idRes = '#MainContent_stat_common_StatCommonShcd_Content_m_res';
    var expCheat = /^\s*(\d+)\s+(\d+)\s*$/;

    var commonJs = require('../common.js');

    $("#btnModifyLevel").click(function () {
        var res = true;
        if (res) {
           // setStartInfo("#txtAgencyMsg", "正在进行操作，请稍候...");
            $.ajax({
                type: "POST",
                url: "/ashx/ShcdControl.ashx",
                data: { roomId:1, level:$('#level').val(), op: 1 },

                success: function (data) {
                    commonJs.checkAjaxScript(data);

                    var arr = data.split('#');
                    $(idRes).text(arr[0]);

                    var tbody = $(idExpTable).find("tbody").eq(0);

                    tbody.children("tr").eq(1).children("td").eq(6).text(arr[1]);
                }
            });
        }
    });

    // 修改大小王个数
    $("#btnModifyJokerCount").click(function () {
        var res = true;
        if (res) {
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
                data: { roomId: 1, level: count, op: 2 },

                success: function (data) {
                    commonJs.checkAjaxScript(data);

                    var arr = data.split('#');
                    if (arr[0] == 0) {
                        var td = $(idExpTable).find("tr").eq(1).find('td').eq(7);
                        td.text(arr[2]);
                    }
                    $(idRes).text(arr[1]);
                }
            });
        }
    });

    // 修改作弊开始结束局数
    $("#btnCheat").click(function () {

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
            data: { roomId: 1, level: start + '-' + end, op: 3 },

            success: function (data) {
                commonJs.checkAjaxScript(data);

                var arr = data.split('#');
                if (arr[0] == 0) {
                    var td = $(idExpTable).find("tr").eq(1).find('td').eq(8);
                    td.text(arr[2]);
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

   
});


});