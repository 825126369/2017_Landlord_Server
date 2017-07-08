define(function (require, exports, module) {

$(function () {

    var commonJs = require('../common.js');
    var g_statFields = ['Less1min', 'Less10min', 'Less30min', 'Less60min', 'Less3h', 'Less5h', 'Less12h', 'Less24h', 'GT24h'];
    var g_playerType = 3;
    var g_jsonData = null;
    var g_id = 1;

    $('#btnQuery').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/FirstRechargeGameTimeDistribution.ashx",
            data: { 'time': $('#txtTime').val(), 'op':0 },

            success: function (data) {
                //console.log(data);
                process(data);
            }
        });

    });

    $('#playerType li').click(function () {

        g_playerType = $(this).attr('pType');
        $(this).addClass('Active').siblings().removeClass('Active');

        if (g_jsonData != null) {
            processData();
        }
    });

    function process(data) {
        g_jsonData = JSON.parse(data);
        processData();
    }

    function processData()
    {
        if (g_jsonData == null)
            return;

        var content = $('#divContent');
        content.empty();

        var data = g_jsonData[g_playerType];
        if (data == null)
        {
            return;
        }

        var timeList = arrangeTimeDyDesc(data);

        var tpl = $('#divTemplate').html();
        var curId = g_id;
        for (var i = 0; i < timeList.length; i++)
        {
            curId = g_id;
            var t = timeList[i];
            var tmp = data[t];

            var str = tpl.format('titleTime' + curId, 'chart' + curId);
            content.append($(str));

            $('#titleTime' + curId).html(t);
            draw(tmp, 'chart' + curId);

            g_id++;
        }
    }

    function draw(data, idContainer) {
        $('#' + idContainer).css('display', 'block').highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: ''
            },
            subtitle: {
            },
            xAxis: {
                categories: ['&lt;=1min', '1min~10min', '10min~30min', '30min~60min',
                             '1h~3h', '3h~5h', '5h~12h', '12h~24h', '>24h'],
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
                name: '',
                data: calGameTimePercent(data)
            }, ]
        });
    }

    function calGameTimePercent(gameData) {
        var result = [];
        var sum = 0;
        for (var i = 0; i < g_statFields.length; i++) {
            var t = gameData[g_statFields[i]];
            sum += parseInt(t);
        }

        for (i = 0; i < g_statFields.length; i++) {
            result[i] = gameData[g_statFields[i]] * 100 / sum;
            result[i] = parseFloat(result[i].toFixed(2));
        }

       // console.log(result);
        return result;
    }

    function arrangeTimeDyDesc(data) {
        var result = [];
        for (var d in data) {
            result.push(d);
        }

        result.sort(function (a, b) {

            var t1 = new Date(a);
            var t2 = new Date(b);
            return t1 < t2;
        });
       // console.log(result);
        return result;
    }

});
});