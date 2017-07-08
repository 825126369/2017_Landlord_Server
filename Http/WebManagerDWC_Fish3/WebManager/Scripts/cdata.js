define(function (require, exports, module) {

    var dateName = ['今日', '昨日', '7日', '30日'];

    var timePointName = ['0点', '1点', '2点', '3点', '4点', '5点',
                     '6点', '7点', '8点', '9点', '10点', '11点', '12点', '13点', '14点',
                     '15点', '16点', '17点', '18点', '19点', '20点', '21点', '22点', '23点']

    var opTypeDef = {
        OP_ADD: 0,
        OP_REMOVE: 1,
        OP_MODIFY: 2,
        OP_VIEW: 3
    };

    var games = {
        1: '捕鱼',
        2: '鳄鱼大亨',
        4: '牛牛',
        6: '五龙',
        10: '黑红梅方',
    };

    var gameIDs = [1, 2, 4, 6, 10];

    var gameRoom = ['总体', '初级房', '中级房', '高级房'];

    exports.dateName = dateName;
    exports.timePointName = timePointName;
    exports.opTypeDef = opTypeDef;
    exports.games = games;
    exports.gameIDs = gameIDs;
    exports.gameRoom = gameRoom;
});
