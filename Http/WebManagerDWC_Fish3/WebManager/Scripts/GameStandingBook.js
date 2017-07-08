define(function (require, exports, module) {
$(function () {

    var PLAYER_INCOME = 0;
    var PLAYER_OUTLAY = 1;
    var PLAYER_START = 2;

    var oTime = $('#MainContent_m_time');
    var oItem = $('#MainContent_m_property');
    var oResultTable = $('#MainContent_m_result');
    var oDetail = $('#divDetail');
    var oTableHead = $('#tableHead');
    var oTableContent = $('#tableContent');
    var cdataJs = require('./cdata.js');
    var g_jsonData = null;
    var g_reason = null;

    function req(op, time, itemId, callBack) {
        $.ajax({
            type: "POST",
            url: "/ashx/GameStandingBook.ashx",
            data: { "time": time, "itemId": itemId, 'op':op },

            success: function (data) {
                callBack(data);
            }
        });
    }

    function init()
    {
        req(1, '', 0, function (data) {

            g_reason = JSON.parse(data);

            afterInit();
        });
    }

    function afterInit()
    {
        $(window).resize(function () {
            oDetail.css('width', $(document).width()).css('height', $(document).height());
        });

        oDetail.find('a').click(function () {

            oDetail.hide();
        });

        $('#btnQuery').click(function () {

            req(0, oTime.val(), oItem.val(), function (data) {

                g_jsonData = JSON.parse(data);
                //console.log(g_jsonData);

                genOverviewTable();
            });

        });
    }

    function genOverviewTable()
    {
        oResultTable.empty();
        if (g_jsonData == null)
            return;

        var str = '<tr>';
        str += cell('日期'); str += cell('前日库存'); str += cell('今日系统减少'); str += cell('今日系统增加'); str += cell('今日实际库存'); str += cell('今日记录差值比');
        str += '</tr>';

        for (var time in g_jsonData)
        {
            var result = getOverview(time);
            str += '<tr>';
            str += cell(time);
            str += cell(result.start.toLocaleString() + detailBtn('详情', time, PLAYER_START));
            str += cell(result.income.toLocaleString() + detailBtn('详情', time, PLAYER_INCOME));
            str += cell(result.outlay.toLocaleString() + detailBtn('详情', time, PLAYER_OUTLAY));
            str += cell(result.remain.toLocaleString());
            str += cell(calRelativeError(result.start + result.income - result.outlay, result.remain));
            str += '</tr>';
        }

        oResultTable.html(str);

        oResultTable.find('a').click(function () {
            
            var obj = $(this);
            var op = obj.attr('op');
            if (op == PLAYER_START)
            {
                showStartDetailInfo(obj.attr('time'));
            }
            else
            {
                showDetail(obj.attr('time'), op);
            }

        });
    }

    function detailBtn(text, time, op)
    {
        var str = '<br/><br/><a time="' + time + '"' + 'op="' + op + '">';
        str += text;
        str += '</a>';
        return str;
    }

    function cell(text)
    {
        var str = '<td>' + text + '</td>';
        return str;
    }

    function getOverview(time) {
        if (g_jsonData == null)
            return null;

        var result = { income: 0, outlay: 0, start: 0, remain: 0 };

        var data = g_jsonData[time];
        for (var gameId in data)
        {
            if (isNaN(gameId))
                continue;

            var ret = calSum(data[gameId]);
            result.income += ret.income;
            result.outlay += ret.outlay;
        }

        result.start = data.start;
        result.remain = data.remain;

        return result;
    }

    function calSum(inObj)
    {
        var result = { income: 0, outlay: 0 };
        for (var key in inObj)
        {
            var obj = inObj[key];
            result.income += obj.income;
            result.outlay += obj.outlay;
        }
        return result;
    }

    function calRelativeError(cur, accuracy)
    {
        if (accuracy == 0.0)
            return "";

        var delta = Math.abs(cur - accuracy);
        var e = delta * 100 / accuracy;
        return e.toFixed(2) + '%';
    }

    function showStartDetailInfo(time) {
        oTableHead.empty();
        oTableContent.empty();
        if (g_jsonData == null)
            return;

        oDetail.css('width', $(document).width()).css('height', $(document).height()).show();

        $('#dateTitle').html(time + '前日库存详情');
        var data = g_jsonData[time];
        var count = data['playerCount'];
        var start = data.start;

        var str = '<tr>';
        str += cell('统计账号数');
        str += cell(count);
        str += '</tr>';

        str += cell('平均每账号货币数');
        str += cell((start / count).toFixed(2));
        str += '</tr>';

        oTableHead.html(str);
    }

    function showDetail(time, op)
    {
        oDetail.css('width', $(document).width()).css('height', $(document).height());
        $('#dateTitle').html(time + (op == PLAYER_INCOME ? '今日系统减少详情' : '今日系统增加详情'));
        oDetail.show();

        var fieldName = '';
        var classOwner = {};
        if (op == PLAYER_INCOME) {
            fieldName = 'income';
            classOwner = g_classIncome;
        }
        else {
            fieldName = 'outlay';
            classOwner = g_classOutlay;
        }

        var arrHTML = genContentTable(time, op, fieldName, classOwner);
        genClassContentTable(arrHTML, classOwner);
    }

    function genContentTable(time, op, fieldName, classOwner)
    {
        oTableContent.empty();

        var str = '<tr>';
        str += cell('所属类别'); str += cell('项目细则'); str += cell('数量'); str += cell('占比');
        str += '</tr>';

        var result = sumDetailForReason(time, fieldName);
        var percent = calPercent(result);

        var arrHTML = [];
        for (var i = 0; i < classOwner.length; i++)
        {
            arrHTML.push({html:[], count:0, total:0});
        }

        var tmpHtml = '';
        for (var key in result)
        {
            tmpHtml = '';
            var classId = getClass(classOwner, key);

            if (key == 11)
            {
                var dd = result[key];
                for (var gameId in dd)
                {
                    tmpHtml = '';
                    tmpHtml += cell(cdataJs.games[gameId]);
                    tmpHtml += cell(dd[gameId].toLocaleString());
                    tmpHtml += cell(percent[key][gameId]);
                    arrHTML[classId].count++;
                    arrHTML[classId].html.push(tmpHtml);

                    arrHTML[classId].total += dd[gameId];
                }
            }
            else
            {
                var reasonStr = g_reason[key];
                if (reasonStr == undefined) {
                    reasonStr = '';
                }

                tmpHtml += cell(reasonStr);
                tmpHtml += cell(result[key].toLocaleString());
                tmpHtml += cell(percent[key]);
                arrHTML[classId].count++;

                arrHTML[classId].html.push(tmpHtml);

                arrHTML[classId].total += result[key];
            }
        }

        for (var i = 0; i < arrHTML.length; i++)
        {
            if (arrHTML[i].count == 0)
                continue;

            for (var k = 0; k < arrHTML[i].html.length; k++)
            {
                str += '<tr>';
                if (k == 0)
                {
                    str += '<td rowspan="' + arrHTML[i].count + '">' + classOwner[i].name + '</td>';
                }

                str += arrHTML[i].html[k];
                str += '</tr>';
            }
        }

        oTableContent.html(str);

        return arrHTML;
    }

    function genClassContentTable(arrHTML, classOwner)
    {
        oTableHead.empty();

        var str = '<tr>';
        str += cell('项目大类'); str += cell('数量'); str += cell('占比');
        str += '</tr>';

        var percent = calClassPercent(arrHTML);
        for (var i = 0; i < arrHTML.length; i++)
        {
            if (arrHTML[i].count == 0)
                continue;

            str += '<tr>';
            str += cell(classOwner[i].name);
            str += cell(arrHTML[i].total.toLocaleString());
            str += cell(percent[i]);
            str += '</tr>';
        }

        oTableHead.html(str);
    }

    function sumDetailForReason(time, fieldName)
    {
        var result = {};
        var data = g_jsonData[time];
        for (var gameId in data) {
            if (isNaN(gameId))
                continue;

            var tmpData = data[gameId];

            for (var reason in tmpData) {
                var obj = tmpData[reason];

                addUp(gameId, reason, obj[fieldName], result);
            }
        }
        console.log(result);

        return result;
    }

    function addUp(gameId, reason, value, result)
    {
        if (value == 0)
            return;

        var key = reason;

        var d = result[key];
        if(d == undefined)
        {
            if (key == 11)
            {
                result[key] = {};
                result[key][gameId] = value;
            }
            else
            {
                result[key] = value;
            }
        }
        else
        {
            if (key == 11)
            {
                if (result[key][gameId] == undefined)
                {
                    result[key][gameId] = value;
                }
                else
                {
                    result[key][gameId] += value;
                }
            }
            else
            {
                result[key] += value;
            }
        }
    }

    function calPercent(detailInfo)
    {
        var result = {};

        var sum = 0;
        for (var key in detailInfo)
        {
            if(key == 11)
            {
                var dd = detailInfo[key];
                for(var game in dd)
                {
                    sum += dd[game];
                }
            }
            else
            {
                sum += detailInfo[key];
            }
        }
        
        for (var key in detailInfo)
        {
            if (key == 11)
            {
                var dd = detailInfo[key];
                result[key] = {};
                for (var game in dd) {
                    var tmp = dd[game] * 100 / sum;
                    result[key][game] = tmp.toFixed(4) + '%';
                }
            }
            else
            {
                var tmp = detailInfo[key] * 100 / sum;
                result[key] = tmp.toFixed(4) + '%';
            }  
        }

        return result;
    }

    function calClassPercent(info)
    {
        var result = {};
        var sum = 0;
        for (var i = 0; i < info.length; i++) {
            sum += info[i].total;
        }

        for(var i = 0; i <info.length;i++)
        {
            var tmp = info[i].total * 100 / sum;
            result[i] = tmp.toFixed(4) + '%';
        }

        return result;
    }

    var g_classIncome = {

        0: {
            name: '玩家游戏下注获得',
            include:[11]
        },
        1: {
            name: '系统赠送',
            include: [1,6,8,10,15,25,40,30,37,38,39,42,46,48,43]
        },
        2: {
            name: '充值获得',
            include: [18,20,23,45,44]
        },
        3: {
            name: 'GM增加',
            include: [21,22,50]
        },
        4: {
            name: '其他',
            include: []
        },
        length:5
    };

    var g_classOutlay = {

        0: {
            name: '玩家游戏下注损失',
            include: [11]
        },
        1: {
            name: '系统回收',
            include: [5,7,8,12,13,14,17,19,28,49,33,34]
        },
        2: {
            name: 'GM扣除',
            include: [50]
        },
        3: {
            name: '其他',
            include: []
        },
        length: 4
    };

    function getClass(json, reason)
    {
        var i = 0;
        for (var i = 0; i < json.length; i++)
        {
            var arr = json[i];
            for (var k = 0; k < arr.include.length; k++)
            {
                if (reason == arr.include[k])
                    return i;
            }
        }

        return i - 1;
    }


    init();


});
});