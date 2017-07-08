define(function (require, exports, module) {
$(function () {

    var ROW_EARN_VALUE = 3, ROW_ACTIVE_ROW = 5;
    var gameIds = [
        {gameId:1,  col:1}, 
        {gameId:2,  col:2}, 
        {gameId:4,  col:3},
        {gameId:6,  col:4},
        {gameId:10, col:5}];

    var commonJs = require('../common.js');
    var commonData = require('../cdata.js');
    var gameSel = $('#MainContent_stat_common_m_game');
    var timeInput = $('#MainContent_stat_common_m_time');
    var statBnt = $('#statGame');
    var resultTable = $('#MainContent_stat_common_m_result');

    statBnt.click(function () {

        if (gameSel.get(0).selectedIndex == 0)
        {
            req(function (data) {
                resultTable.html(data);

                var gameData = collectData();
                drawChartActive(gameData);
                drawChartEarn(gameData);
            });
        }
        else
        {
            console.log('22222222222222222');
            $('#Form1').submit();
        }
    });

    function req(callBack)
    {
        $.ajax({
            type: "POST",
            url: "/ashx/StatServerEarnings.ashx",
            data: { gameId: gameSel.get(0).selectedIndex, time: timeInput.val()},

            success: function (data) {
                commonJs.checkAjaxScript(data);

                callBack(data);
            }
        });
    }


    function getGameData(row, col)
    {
        var val = resultTable.find('tr').eq(row).find('td').eq(col).text();
        console.log(row + '.........' + col + "..........." + val);
        if (val == '')
            return 0;
        return parseInt(val);
    }

    function collectData()
    {
        data = {};
        var tmp = {};

        for(var i = 0; i < gameIds.length; i++)
        {
            tmp = {};
            tmp.earn = getGameData(ROW_EARN_VALUE, gameIds[i].col);
            tmp.active = getGameData(ROW_ACTIVE_ROW, gameIds[i].col);
            tmp.gameId = gameIds[i].gameId;
            data[gameIds[i].gameId] = tmp;
        }

        console.log(data);
        return data;
    }

    function drawChartActive(data) {
        var result = calActivePercent(data);

        $('#chartActive').css('display', 'block').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: '游戏活跃百分比'
            },
            subtitle: {
            },
            xAxis: {
                categories: result.fieldName,
                title: {
                    text: null
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: '活跃百分比',
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                }
            },
            tooltip: {
                valueSuffix: ' %'
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            legend: {
                layout: 'vertical',
                align: 'left',
                verticalAlign: 'top',
                x: -40,
                y: 80,
                floating: true,
                borderWidth: 1,
                backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                shadow: true,
                enabled: false
            },
            credits: {
                enabled: false
            },
            series: [{
                name: '',
                data: result.percentData
            }, ]
        });
    }

    function drawChartEarn(data)
    {
        var result = calEarn(data);
        $('#chartEarnValue').show().highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: '各游戏盈利值'
            },
            xAxis: {
                categories: result.fieldName
            },
            credits: {
                enabled: false
            },
            legend: {
                enabled: false
            },
            series: [{
                name: '',
                data: result.earnVal
            }]
        });
    }

    function calActivePercent(dataActive) {
        var result = { fieldName: [], percentData: [] };
        var sum = 0;

        for (var p in dataActive) {
            var d = dataActive[p];
            var t = d.active;
            sum += t;
            result.fieldName.push(commonData.games[d.gameId]);
        }

        for (var p in dataActive) {
            var pd = dataActive[p].active * 100 / sum;
            pd = parseFloat(pd.toFixed(2));
            result.percentData.push(pd);
        }

        return result;
    }

    function calEarn(dataActive) {
        var result = { fieldName: [], earnVal: [] };

        for (var p in dataActive) {
            var d = dataActive[p];
            result.fieldName.push(commonData.games[d.gameId]);
            result.earnVal.push(d.earn);
        }

        return result;
    }
});

});
