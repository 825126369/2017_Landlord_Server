define(function (require, exports, module) {

$(function () {

    var cdataJs = require('../cdata.js');
    var g_playerType = 1;
    var g_jsonData = null;

    $('#btnQuery').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/GameTimePlayerFavor.ashx",
            data: { 'time': $('#txtTime').val(), 'playerType':1 },

            success: function (data) {
                process(data);
            }
        });

    });

    $('#playerType li').click(function () {

        g_playerType = $(this).index() + 1;
        $(this).addClass('Active').siblings().removeClass('Active');

        if(g_jsonData != null)
        {
            processData(g_jsonData);
        }
    });


    function process(data) {
        $('#divTemplate').show();
        $('#divTemplate div').hide();
        g_jsonData = JSON.parse(data);
        processData(g_jsonData);
    }

    function processData(jd)
    {
        var result = getAveGameTime(0, g_playerType, jd);
        draw(0, result, '总在线');

        for (var i = 0; i < cdataJs.gameIDs.length; i++) {
            result = getAveGameTime(cdataJs.gameIDs[i], g_playerType, jd);
            var gameId = cdataJs.gameIDs[i];
            draw(gameId, result, cdataJs.games[gameId]);
        }
    }

    function draw(gameId, gameData, chartTitle) {
        $('#game' + gameId).show().highcharts({
            title: {
                text: chartTitle,
                x: -20 //center
            },
            subtitle: {
                text: '',
                x: -20
            },
            xAxis: {
                categories: gameData.timeArr
            },
            yAxis: {
                title: {
                    text: '平均在线'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: '分'
            },
            credits: {
                enabled: false
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0,
                enabled: false
            },
            series:[ { name: 'aaa', data: gameData.dList }]
        });
    }

    /////////////////////////////////////////////////////////////////////
    function getAveGameTime(gameId, playerType, jsonData)
    {
        var result = {timeArr:[], dList:[], playerCount:[]};
        var data = jsonData[playerType];
        if (data != undefined)
        {
            for(var d in data)
            {
                result.timeArr.push(d);

                var tmp = data[d];
                if(tmp['game' + gameId])
                {
                    result.dList.push(tmp['game' + gameId]);
                }
                else
                {
                    result.dList.push(0);
                }

                result.playerCount.push(tmp.playerCount);
            }
        }
        var i = 0;
        for (; i < result.dList.length; i++)
        {
            if (result.playerCount[i] > 0)
            {
                result.dList[i] = result.dList[i] / (result.playerCount[i]*60);
                result.dList[i] = parseFloat(result.dList[i].toFixed(2));
            }
        }

        console.log(result);
        return result;
    }

});
});