define(function (require, exports, module) {

$(function () {

    var commonJs = require('../common.js');
    var g_playerType = 3;
    var g_jsonData = null;
    var g_id = 1;

    $('#btnQuery').click(function () {

        $.ajax({
            type: "POST",
            url: "/ashx/FirstRechargeGameTimeDistribution.ashx",
            data: { 'time': $('#txtTime').val(), 'op': 1 },

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

    function processData() {
        if (g_jsonData == null)
            return;

        var content = $('#divContent');
        content.empty();

        var data = g_jsonData[g_playerType];
        if (data == null) {
            return;
        }

        var timeList = arrangeTimeDyDesc(data);

        var tpl = $('#divTemplate').html();
        var curId = g_id;
        for (var i = 0; i < timeList.length; i++) {
            curId = g_id;

            var t = timeList[i];
            var str = tpl.format('titleTime' + curId, 'chart' + curId);
            content.append($(str));
            $('#titleTime' + curId).html(t);

            var payDataInfo = data[t];
            draw(payDataInfo, 'chart' + curId);

            g_id++;
        }
    }

    function draw(data, idContainer) {
        var result = calPayPointPercent(data);

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
                categories: result.fieldName,
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
                data: result.percentData
            }, ]
        });
    }

    function calPayPointPercent(payDataInfo) {
        var result = {fieldName:[], percentData:[]};
        var sum = 0;
        var payPoints = g_jsonData.payPoint;

        for (var p in payDataInfo) {
            var t = payDataInfo[p];
            sum += parseInt(t);
            result.fieldName.push(payPoints[p]);
        }

        for (var p in payDataInfo) {
            var pd = payDataInfo[p] * 100 / sum;
            pd = parseFloat(pd.toFixed(2));
            result.percentData.push(pd);
        }

        return result;
    }

    function arrangeTimeDyDesc(data)
    {
        var result = [];
        for(var d in data)
        {
            result.push(d);
        }

        result.sort(function (a, b) {

            var t1 = new Date(a);
            var t2 = new Date(b);
            return t1 < t2;
        });
            
        return result;
    }

});
});