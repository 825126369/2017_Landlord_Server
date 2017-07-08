define(function (require, exports, module) {

    $(function () {

        var cdata = require('../cdata.js');

        $.ajax({
            type: "POST",
            url: "/ashx/RechargePerHour.ashx",
            success: function (data) {
                drawCurve(data);
            }
        });

        function drawCurve(data)
        {
            //console.log(data);
            var jd = JSON.parse(data);
            var dataArr = [];
            var field = 0;
            for (var d in jd)
            {
                var t1 = cdata.dateName[field];
                field++;
                var t2 = jd[d].split(',');
               // console.log(t1);
               // console.log(t2);

                var t3 = [];
                for (var k = 0; k < t2.length; k++)
                {
                    t3[k] = parseInt(t2[k]);
                }
                dataArr.push({ name: t1, "data": t3 });
            }
            //console.log(arr);

            var credits = Highcharts.getOptions().credits;
            credits.enabled = false;

            $('#container').highcharts({
                title: {
                    text: '实时付费',
                    x: -20 //center
                },
                subtitle: {
                    text: '',
                    x: -20
                },
                xAxis: {
                    categories: cdata.timePointName
                },
                yAxis: {
                    title: {
                        text: '付费金额'
                    },
                    plotLines: [{
                        value: 0,
                        width: 1,
                        color: '#808080'
                    }]
                },
                tooltip: {
                    valueSuffix: '元'
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle',
                    borderWidth: 0
                },
                series: dataArr
            });
        }


    });
});