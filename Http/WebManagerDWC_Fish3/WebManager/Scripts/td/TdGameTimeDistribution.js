define(function (require, exports, module) {

$(function () {

    var cdataJs = require('../cdata.js');
    var commonJs = require('../common.js');
    var ID = 1;
    var g_statFields = ['Less10s', 'Less30s', 'Less60s', 'Less5min', 'Less10min', 'Less30min', 'Less60min', 'GT60min'];

    $('.SelCard li').click(function () {

        var param = $(this).attr('data');
        $(this).attr('class', 'Active').siblings().attr('class', '');

        $.ajax({
            type: "POST",
            url: "/ashx/GameTimeDistribution.ashx",
            data: { 'time': $('#txtTime').val(), 'param': param },

            success: function (data) {
                process(data);
                //console.log(data);
            }
        });

    });

    function process(data)
    {
        var jd = JSON.parse(data);
        var content = $('#divContent');
        content.empty();
        var curID = ID;

        for(var d in jd)
        {
            curID = ID;
            var t = genNewHTML();
            content.append(t);

            $('#titleTime' + curID).html(d);

            var games = jd[d];
            for (var i = 0; i < games.length; i++)
            {
                draw(games[i], genDrawID(curID, games[i].gameId));
            }
        }   
    }

    function draw(data, idContainer)
    {
        $('#' + idContainer).css('display', 'block').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: cdataJs.games[data.gameId]
            },
            subtitle: {
            },
            xAxis: {
                categories: ['&lt;=10s', '10s~30s', '30s~60s', '1min~5min', '5min~10min', '10min~30min', '30min~60min', '>60min'],
                title: {
                    text: null
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: '人数百分比',
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
                name: cdataJs.games[data.gameId],
                data: calGameTimePercent(data)
            }, ]
        });
    }

    function genNewHTML()
    {
        var tpl = $('#divTemplate').html();
        var str = tpl.format('titleTime' + ID,
            genDrawID(ID, cdataJs.gameIDs[0]),
            genDrawID(ID, cdataJs.gameIDs[1]),
            genDrawID(ID, cdataJs.gameIDs[2]),
            genDrawID(ID, cdataJs.gameIDs[3]),
            genDrawID(ID, cdataJs.gameIDs[4])
            );
        ID++;
        return $(str);
    }

    function genDrawID(curID, gameId)
    {
        return 'draw' + curID + '_' + gameId;
    }

    function calGameTimePercent(gameData)
    {
        var result = [];
        var sum = 0;
        for (var i = 0; i < g_statFields.length; i++)
        {
            var t = gameData[g_statFields[i]];
            sum += parseInt(t);
        }

        for (i = 0; i < g_statFields.length; i++)
        {
            result[i] = gameData[g_statFields[i]] * 100 / sum;
            result[i] = parseFloat(result[i].toFixed(2));
        }

        console.log(result);
        return result;
    }

});
});