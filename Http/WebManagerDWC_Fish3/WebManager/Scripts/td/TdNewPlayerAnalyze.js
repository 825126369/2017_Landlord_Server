define(function (require, exports, module) {
 $(function () {

     var g_fishLevelCFG = null;
     var g_callBack = {};
     var oResultTable = $('#m_result');
     var g_fishFireCountHead = ['<=50炮', '51炮~700炮', '701炮~1400炮', '1401炮~2700炮', '2701炮~5400炮', '5401炮~8100炮', '>=8101炮'];
     var g_outlayHead = ['<=1000', '1001~2000', '2001~5000', '5001~1万', '1万~2万', '2万~5万', '5万~10万', '10万~100万', '100万~1千万', '1千万~1亿', '>1亿'];

     function init()
     {
         g_callBack[1] = processLevelDistribution;
         g_callBack[2] = processGoldOutlayDistribution;
         g_callBack[3] = processFishActivityDistribution;
         g_callBack[5] = processEnterRoom;

         g_callBack[4] = processLevelCFG;

         req(4, g_callBack[4]);
     }

     function processLevelCFG(data)
     {
         g_fishLevelCFG = $.parseJSON(data);

         ////////////////////////////////////////////
         $('.SelCard li').click(function () {

             $(this).attr('class', 'Active').siblings().attr('class', '');

             var data = $(this).attr('data');
             req(data, g_callBack[data]);
         });
     }

     function processEnterRoom(data)
     {
         //console.log(data);
         oResultTable.empty();
         var dat = $.parseJSON(data);
         
         var str = '<tr>';
         str += cell('时间'); str += cell('登录大厅'); str += cell('进初级场'); str += cell('进中级场'); str += cell('进高级场');
         str += '</tr>';

         var dataList = dat['data'];
         for (var i = 0; i < dataList.length; i++) {
             var obj = dataList[i];

             str += '<tr>';
             str += cell(obj.time);

             for (var j = 0; j < 4; j++) {
                 if (obj[j]) {
                     str += cell(obj[j]);
                 }
                 else {
                     str += cell('');
                 }
             }
             str += '</tr>';
         }

         oResultTable.html(str);
     }

     function processGoldOutlayDistribution(data) {
         oResultTable.empty();
         var str = '<tr>';
         str += cell('时间');
         for (var i = 0; i < g_outlayHead.length; i++)
         {
             str += cell(g_outlayHead[i]);
         }
         str += '</tr>';

         var dat = $.parseJSON(data);
         var dataList = dat['data'];
         for (var i = 0; i < dataList.length; i++) {
             var obj = dataList[i];
             var result = calFishActivityPercent(obj);

             str += '<tr>';
             str += cell(obj.time);
             for (var k = 0; k < g_outlayHead.length; k++) {
                 str += cell(result[k]);
             }
             str += '</tr>';
         }
         oResultTable.html(str);
     }

     function processFishActivityDistribution(data) {
         //console.log(data);
         oResultTable.empty();
         var dat = $.parseJSON(data);

         var str = '<tr>';
         str += cell('时间');
         for (var i = 0; i < g_fishFireCountHead.length; i++) {
             str += cell(g_fishFireCountHead[i]);
         }
         str += '</tr>';

         var dataList = dat['data'];
         for (var i = 0; i < dataList.length; i++) {
             var obj = dataList[i];
             var result = calFishActivityPercent(obj);
            
             str += '<tr>';
             str += cell(obj.time);
             for(var k = 0; k < g_fishFireCountHead.length; k++)
             {
                 str += cell(result[k]);
             }
             str += '</tr>';
         }

         oResultTable.html(str);
     }

     function processOnlineTimeDistribution() {

     }

     function processLevelDistribution(data)
     {
         //console.log(data);
         oResultTable.empty();
         var dat = $.parseJSON(data);

         var tableHead = {};
         var resultArr = [];

         var dataList = dat['data'];
         for (var i = 0; i < dataList.length; i++)
         {
             var obj = dataList[i];
             var res = calFishLevelPercent(obj, tableHead)
             resultArr.push(res);
         }

         var str = '<tr>';
         str += cell('时间');
         for (var key in tableHead) {

             if (g_fishLevelCFG)
             {
                 if(g_fishLevelCFG[key])
                 {
                     str += cell(g_fishLevelCFG[key] + '炮');
                 }
                 else
                 {
                     str += cell(key + '级');
                 }
             }
             else
             {
                 str += cell(key + '级');
             }
         }
         str += '</tr>';

         for (var i = 0; i < dataList.length; i++)
         {
             str += '<tr>';
             var obj = resultArr[i];
             str += cell(dataList[i].time);

             for(var key in tableHead)
             {
                 if(obj[key] != undefined)
                 {
                     str += cell(obj[key]);
                 }
                 else
                 {
                     str += cell('');
                 }
             }
             str += '</tr>';
         }

         oResultTable.html(str);
     }

     function req(data, callBack)
     {
         $.ajax({
             type: "POST",
             url: "/ashx/TdNewPlayerAnalyze.ashx",
             data: { 'time': $('#txtTime').val(), 'data': data },

             success: function (data) {
                 callBack(data);
             }
         });
     }

     function cell(text) {
         var str = '<td>' + text + '</td>';
         return str;
     }

     function calFishLevelPercent(gameData, tableHead) {
         var result = {};
         var sum = 0;
         for (var key in gameData)
         {
             if(!isNaN(key))
             {
                 sum += gameData[key];
             }
         }

         for (var key in gameData)
         {
             if (!isNaN(key))
             {
                 if (gameData[key] > 0 && sum > 0)
                 {
                     result[key] = gameData[key] * 100 / sum;
                     result[key] = result[key].toFixed(2) + '%';
                     tableHead[key] = true;
                 }
                 else
                 {
                     result[key] = '0%';
                 }
             }
         }
         //console.log(result);
         return result;
     }

     function calFishActivityPercent(gameData) {
         var result = {};
         var sum = 0;
         for (var key in gameData) {
             if (!isNaN(key)) {
                 sum += gameData[key];
             }
         }

         for (var key in gameData) {
             if (!isNaN(key)) {
                 if (gameData[key] > 0 && sum > 0) {
                     result[key] = gameData[key] * 100 / sum;
                     result[key] = result[key].toFixed(2) + '%';
                 }
                 else {
                     result[key] = '0%';
                 }
             }
         }
         //console.log(result);
         return result;
     }

     init();

});
});