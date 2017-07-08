using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ParamModifyGmRight
{
    public int m_op; // 1分配权限, 0默认的
    public string m_acc;
    public string m_right;
}

// 修改后台管理员权限
public class DyOpModifyGmRight : DyOpBase
{
    public delegate bool isDirectlyChild();

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamModifyGmRight p = (ParamModifyGmRight)param;
        if (p.m_op == 1)
            return dispatch(p, user);

        DestGmUser dst = new DestGmUser(p.m_acc, user);
        if (!dst.m_isExists)
            return OpRes.op_res_no_right;

        if (!dst.isDerivedFrom(user))
            return OpRes.op_res_no_right;

        if (!dst.isAccType(AccType.ACC_AGENCY) && 
            !dst.isAccType(AccType.ACC_GENERAL_AGENCY) &&
            !dst.isAccType(AccType.ACC_SUPER_ADMIN_SUB))
            return OpRes.op_res_no_right;

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("gmRight", p.m_right, FieldType.TypeString);
        string sql = gen.getResultSql(TableName.GM_ACCOUNT,
            string.Format("acc='{0}'", p.m_acc));
        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    /*
     *      user 可否给 toAccType分配权限
     *      child toAccType是否user的直接子结点
     */
    public static bool canDispatchRight(GMUser user, int toAccType, isDirectlyChild child)
    {
        if (!(user.isAdmin() || user.isGeneralAgency() || user.isAdminSub() ||
              user.isAgency()))
        {
            return false;
        }

        if (!child())
            return false;

        if (toAccType != AccType.ACC_GENERAL_AGENCY &&
            toAccType != AccType.ACC_AGENCY &&
            toAccType != AccType.ACC_SUPER_ADMIN_SUB)
            return false;

        return true;
    }

    public OpRes dispatch(ParamModifyGmRight p, GMUser user)
    {
        DestGmUser dst = new DestGmUser(p.m_acc, user);
        if (!dst.m_isExists)
            return OpRes.op_res_no_right;

        bool res = DyOpModifyGmRight.canDispatchRight(user, dst.m_accType,
            () => { return dst.m_owner == user.m_user; });

        if (!res)
            return OpRes.op_res_no_right;

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("gmRight", p.m_right, FieldType.TypeString);
        string sql = gen.getResultSql(TableName.GM_ACCOUNT,
            string.Format("acc='{0}'", p.m_acc));
        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamStartStopGmAcc
{
    public string m_acc;
    public int m_opType; // 0启用 1停用

    public bool isStart()
    {
        return m_opType == 0;
    }
}

// 启用停用GM账号
public class DyOpStartStopGmAcc : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamStartStopGmAcc p = (ParamStartStopGmAcc)param;
        DestGmUser dst = new DestGmUser(p.m_acc, user);
        if (!dst.m_isExists)
            return OpRes.op_res_no_right;

        if (!dst.isDerivedFrom(user))
            return OpRes.op_res_no_right;

        if (p.isStart())
        {
            return startAcc(dst, user, p);
        }

        return stopAcc(dst, user, p);
    }

    // 停封账号dst及其之下所有的代理，代理下的玩家
    private OpRes stopAcc(DestGmUser dst, GMUser user, ParamStartStopGmAcc p)
    {
        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("state", GmState.STATE_BLOCK, FieldType.TypeNumber);

        if(dst.m_accType == AccType.ACC_GENERAL_AGENCY ||
            dst.m_accType == AccType.ACC_AGENCY ||
            dst.m_accType == AccType.ACC_API)
        {
            string sql = gen.getResultSql(TableName.GM_ACCOUNT,
            string.Format(" createCode like '{0}%' ", dst.m_createCode));
            int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

            // 停封账号之下的玩家
            gen.reset();
            gen.addField("enable", 0, FieldType.TypeNumber);
            sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                string.Format(" createCode like '{0}%' ", dst.m_createCode));
            user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }
        else
        {
            string sql = gen.getResultSql(TableName.GM_ACCOUNT,
            string.Format(" acc= '{0}' ", p.m_acc));
            int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }
    }

    // 解封账号dst
    private OpRes startAcc(DestGmUser dst, GMUser user, ParamStartStopGmAcc p)
    {
        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("state", GmState.STATE_NORMAL, FieldType.TypeNumber);

        if (dst.m_accType == AccType.ACC_GENERAL_AGENCY ||
           dst.m_accType == AccType.ACC_AGENCY ||
           dst.m_accType == AccType.ACC_API)
        {
            string sql = gen.getResultSql(TableName.GM_ACCOUNT,
            string.Format(" createCode like '{0}%' ", dst.m_createCode));
            int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

            // 解封账号之下的玩家
            gen.reset();
            gen.addField("enable", 1, FieldType.TypeNumber);
            sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                string.Format(" createCode like '{0}%' ", dst.m_createCode));
            user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }
        else
        {
            string sql = gen.getResultSql(TableName.GM_ACCOUNT,
                string.Format(" acc= '{0}' ", p.m_acc));
            int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamScore
{
    // 0上分 1下分
    public int m_op;
    public string m_toAcc = "";
    public string m_score;

    // 来源，OrderGenerator中的定义
    public int m_orderFrom;
    public string m_orderId = "";

    // 目标账号 0管理 1玩家
    protected int m_dest;

    public ParamScore()
    {
        m_orderFrom = OrderGenerator.ORDER_FROM_BG_OP;
    }

    public bool isAddScore()
    {
        return m_op == 0;
    }

    public bool isToPlayer()
    {
        return m_dest == 1;
    }

    public void scoreToPlayer()
    {
        m_dest = 1;
    }

    public void scoreToMgr()
    {
        m_dest = 0;
    }
}

// 上分，下分
public class DyOpScore : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamScore p = (ParamScore)param;
        long score = 0;
        if (!long.TryParse(p.m_score, out score))
            return OpRes.op_res_param_not_valid;

        if (score < 0)
            return OpRes.op_res_param_not_valid;

        if (!RightMap.hasRight(user.m_accType, RIGHT.SCORE, user.m_right))
            return OpRes.op_res_no_right;

        score = ItemHelp.saveMoneyValue(score);

        OpRes res = OpRes.op_res_failed;

        try
        {
            HttpContext.Current.Application.Lock();
            if (user.m_accType == AccType.ACC_SUPER_ADMIN ||
                user.m_accType == AccType.ACC_SUPER_ADMIN_SUB)
            {
                res = doScoreSuperAdmin(p, score, user);
            }
            else
            {
                if (p.isToPlayer()) // 给玩家上分下分
                {
                    res = doScorePlayer(p, score, user);
                }
                else // 给管理，上分下分
                {
                    res = doScore(p, score, user);
                }
            }
        }
        catch (System.Exception ex)
        {
            CLOG.Info("DyOpScore.doDyop, 出现异常:{0}", ex.ToString());
            res = OpRes.op_res_happend_exception;
        }
        finally
        {
            HttpContext.Current.Application.UnLock();
        }

        // 后续处理 对于来自订单的操作，由于订单Id不会变化，采取轮循会有问题
        if (res == OpRes.op_res_player_in_game &&
            p.m_orderFrom == OrderGenerator.ORDER_FROM_BG_OP)
        {
            QueryOrderState qos = new QueryOrderState();
            int state = qos.queryOrderState(p.m_orderId,
                 user.sqlDb.getServer(user.getMySqlServerID()).queryOne, 10, 1000);
            if (state != PlayerReqOrderState.STATE_PROCESSING)
            {
                res = (state == PlayerReqOrderState.STATE_FINISH) ? OpRes.opres_success : OpRes.op_res_failed;
            }
        }

        return res;
    }

    // 超级管理员操作
    private OpRes doScoreSuperAdmin(ParamScore p, long score, GMUser user)
    {
        OpRes res = OpRes.op_res_failed;
        if (p.isAddScore()) // 加分
        {
            if (p.isToPlayer())
            {
                res = addScorePlayer(p, score, user);
            }
            else
            {
                res = addScore(p.m_toAcc, score, user);
            }
        }
        else
        {
            if (p.isToPlayer())
            {
                res = decScorePlayer(p, score, user);
            }
            else
            {
                res = decScore(p.m_toAcc, score, user);
            }
        }
        if (res == OpRes.opres_success)
        {
            addScoreToOnlineGM(p, score);
            scoreLog(TableName.GM_SCORE, p, score, user);
        }
        return res;
    }

    private OpRes doScore(ParamScore p, long score, GMUser user)
    {
        OpRes res = OpRes.op_res_failed;
        if (p.isAddScore()) // 加分
        {
            if (user.m_money < score)
                return OpRes.op_res_money_not_enough;

            res = addScore(p.m_toAcc, score, user);
            if (res == OpRes.opres_success)
            {
                addScoreToOnlineGM(p, score);
                user.m_money -= score;
                decScoreDirect(user.m_user, score, user);
            }
        }
        else
        {
            res = decScore(p.m_toAcc, score, user);
            if (res == OpRes.opres_success)
            {
                addScoreToOnlineGM(p, score);
                user.m_money += score;
                addScoreDirect(user.m_user, score, user);
            }
        }

        if (res == OpRes.opres_success)
        {
            scoreLog(TableName.GM_SCORE, p, score, user);
        }

        return res;
    }

    private OpRes doScorePlayer(ParamScore p, long score, GMUser user)
    {
        OpRes res = OpRes.op_res_failed;
        if (p.isAddScore()) // 加分
        {
            if (user.m_money < score)
                return OpRes.op_res_money_not_enough;

            res = addScorePlayer(p, score, user);
            if (res == OpRes.opres_success ||
                res == OpRes.op_res_player_in_game) // 玩家在线时，先扣gm的钱，若实时给玩家加分失败，订单服务器会返还这部分钱
            {
                user.m_money -= score;
                decScoreDirect(user.m_user, score, user);
            }
        }
        else
        {
            res = decScorePlayer(p, score, user);
            if (res == OpRes.opres_success) // 玩家在线时，先实时给扣玩家分数，成功后，订单服务器会返还这部分钱
            {
                user.m_money += score;
                addScoreDirect(user.m_user, score, user);
            }
        }

        if (res == OpRes.opres_success)
        {
            scoreLog(TableName.GM_SCORE, p, score, user);
        }

        return res;
    }

    private OpRes addScore(string acc, long score, GMUser user)
    {
        OpRes res = canDoScoreToGM(acc, score, true, user);
        if (res != OpRes.opres_success)
            return res;

        string cmd = string.Format(SqlStrCMD.SQL_ADD_SCORE_TO_MGR,
                                    TableName.GM_ACCOUNT,
                                    score,
                                    acc);
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    private OpRes addScoreDirect(string acc, long score, GMUser user)
    {
        if (score < 0)
            return OpRes.op_res_failed;

        string cmd = string.Format(SqlStrCMD.SQL_ADD_SCORE_TO_MGR_DIRECT,
                                    TableName.GM_ACCOUNT,
                                    score,
                                    acc);
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    // score传正数
    private OpRes decScore(string acc, long score, GMUser user)
    {
        OpRes res = canDoScoreToGM(acc, score, false, user);
        if (res != OpRes.opres_success)
            return res;

        string cmd = string.Format(SqlStrCMD.SQL_DEC_SCORE_TO_MGR,
                                      TableName.GM_ACCOUNT,
                                      score,
                                      acc,
                                      score);
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    private OpRes decScoreDirect(string acc, long score, GMUser user)
    {
        if (score < 0)
            return OpRes.op_res_failed;

        string cmd = string.Format(SqlStrCMD.SQL_DEC_SCORE_TO_MGR_DIRECT,
                                      TableName.GM_ACCOUNT,
                                      score,
                                      acc,
                                      score);
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    private OpRes addScorePlayer(ParamScore p, long score, GMUser user)
    {
        OpRes res = canDoScoreToPlayer(p.m_toAcc, score, true, user);

        if (res == OpRes.op_res_player_in_game) // 玩家在游戏内，提交订单请求
        {
            bool code = submitPlayerOnlineOrder(p, score, user);

            return code ? OpRes.op_res_player_in_game : OpRes.op_res_db_failed;
        }
        else if (res == OpRes.opres_success)
        {
            string cmd = string.Format(SqlStrCMD.SQL_ADD_SCORE_TO_PLAYER_BYPASS,
               TableName.PLAYER_ACCOUNT_XIANXIA,
               score,
               p.m_toAcc,
               PlayerState.STATE_IDLE);
           // int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            int count = user.sqlDb.executeOpTran(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }

        return res;
    }

    // 提交玩家在线时订单
    private bool submitPlayerOnlineOrder(ParamScore p, long score, GMUser user)
    {
        OrderGenerator or = new OrderGenerator();
        Dictionary<string, object> orData = or.genOrder(user.m_user, p.m_toAcc, score, p.m_op,
            AccType.ACC_PLAYER,
            p.m_orderFrom, p.m_orderId);

        if (string.IsNullOrEmpty(p.m_orderId))
        {
            p.m_orderId = Convert.ToString(orData["orderId"]);
        }

        bool code = DBMgr.getInstance().insertData(TableName.PLAYER_ORDER_REQ, orData, user.getDbServerID(),
            DbName.DB_PLAYER);
        return code;
    }

    // score传正数
    private OpRes decScorePlayer(ParamScore p, long score, GMUser user)
    {
        OpRes res = canDoScoreToPlayer(p.m_toAcc, score, false, user);

        if (res == OpRes.op_res_player_in_game) // 玩家在游戏内，提交订单请求
        {
           // bool code = submitPlayerOnlineOrder(p, score, user);
           // return code ? OpRes.op_res_player_in_game : OpRes.op_res_db_failed;
            return OpRes.op_res_player_in_game_cannot_subscore;
        }
        else if (res == OpRes.opres_success)
        {
            string cmd = string.Format(SqlStrCMD.SQL_DEC_SCORE_TO_PLAYER_BYPASS,
              TableName.PLAYER_ACCOUNT_XIANXIA,
              score,
              p.m_toAcc,
              PlayerState.STATE_IDLE,
              score);
            int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }

        return res;
    }

    private void addScoreToOnlineGM(ParamScore p, long score)
    {
        GMUser dstUser = AccountSys.getInstance().getUser(p.m_toAcc);
        if (dstUser != null)
        {
            if (p.isAddScore())
            {
                dstUser.m_money += score;
            }
            else
            {
                dstUser.m_money -= score;
            }
        }
    }

    private void scoreLog(string tableName, ParamScore p, long score, GMUser user)
    {
        long remainMoney = ItemHelp.getRemainMoney(p.m_toAcc, p.isToPlayer(), user);

        // 操作账号余额
        long opSrcRemainMoney = ItemHelp.getRemainMoney(user.m_user, false, user);

       /*string cmd = string.Format(SqlStrCMD.SQL_CMD_PLAYER_SCORE,
                                  tableName,
                                  DateTime.Now.ToString(ConstDef.DATE_TIME24),
                                  user.m_user,
                                  p.m_toAcc,
                                  p.m_op,
                                  score,
                                  user.m_moneyType,
                                  user.m_depth,
                                  user.m_createCode,
                                  p.isToPlayer() ? AccType.ACC_PLAYER : 0,
                                  remainMoney);

      user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
      */
        // 生成上下分记录
        OrderInfo oinfo =
                   OrderGenerator.genOfflineSuccessOrder(user.m_user, p.m_toAcc, score,
                   p.m_op,
                   p.isToPlayer() ? AccType.ACC_PLAYER : 0,
                   remainMoney, p.m_orderFrom);
        // 生成上下分记录
        string cmd = OrderGenerator.genSqlForLogScore(oinfo, user.m_createCode, opSrcRemainMoney);
        user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }

    // 是否可以对玩家账号进行上分下分操作
    private OpRes canDoScoreToPlayer(string acc, long score, bool isAddScore, GMUser user)
    {
        Player player = new Player(acc, user);
        if (!player.m_isExists)
            return OpRes.op_res_player_not_exist;

        if (player.isAccStop())
            return OpRes.op_res_acc_block;

        if (!player.isDerivedFrom(user))
            return OpRes.op_res_no_right;

        if (!isAddScore)
        {
            if (!player.isMoneyEnough(score))
                return OpRes.op_res_money_not_enough;
        }

        Dictionary<string, object> data = QueryBase.getPlayerPropertyByAcc(acc, user, new string[] { "SyncLock" });
        if (data != null)
        {
            if (data.ContainsKey("SyncLock"))
            {
                int state = Convert.ToInt32(data["SyncLock"]);
                if (state == 2)
                {
                    return OpRes.op_res_player_locked;
                }
            }
        }

        if (player.isInGame())
            return OpRes.op_res_player_in_game;

        return OpRes.opres_success;
    }

    // 是否可以对GM账号进行上分下分操作
    private OpRes canDoScoreToGM(string acc, long score, bool isAddScore, GMUser user)
    {
        DestGmUser dstUser = new DestGmUser(acc, user);
        if (!dstUser.m_isExists)
            return OpRes.op_res_no_right;

        if (!dstUser.isDerivedFrom(user))
            return OpRes.op_res_no_right;

        if (dstUser.isAccStop())
            return OpRes.op_res_acc_block;

        if (!isAddScore)
        {
            if (dstUser.m_money < score)
                return OpRes.op_res_money_not_enough;
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamApiApprove
{
    // 索引
    public int m_index;

    // 是否通过
    public bool m_isPass;
    public string m_apiAcc;

    /////////////////输出参数//////////////////////////
    public string m_resultAcc;
    public string m_validatedCode;
}

// API审批
public class DyOpApiApprove : DyOpBase
{
    const string SQL_REFUSE = "delete from {0} where apiAcc='{1}' ";

    const string QUERY_SQL = " SELECT depth,createCode,childNodeNumber,generalAgency " +
                             " FROM {0} where acc='{1}' ";

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamApiApprove p = (ParamApiApprove)param;

        ResultAPIItem item = getItem(p, user);
        if (item == null)
            return OpRes.op_res_failed;

        if (p.m_isPass)
        {
            OpRes res = OpRes.op_res_failed;
            try
            {
                HttpContext.Current.Application.Lock();
                res = createAPI(user, item, p); //agree(item, user);
                if (res == OpRes.opres_success)
                {
                    refuse(item, user);
                }
            }
            catch (System.Exception ex)
            {
            }
            finally
            {
                HttpContext.Current.Application.UnLock();
            }
            return res;
        }
        return refuse(item, user);
    }

//     OpRes agree(ResultAPIItem item, GMUser user)
//     {
//         return createAPI(user, item);
//     }

    OpRes refuse(ResultAPIItem item, GMUser user)
    {
        string cmd = string.Format(SQL_REFUSE, TableName.API_APPROVE, item.m_apiAcc);
        user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return OpRes.opres_success;
    }

    ResultAPIItem getItem(ParamApiApprove p, GMUser user)
    {
        OpRes res = user.doQuery(p.m_apiAcc, QueryType.queryTypeQueryApiApprove);
        if (res != OpRes.opres_success)
            return null;

        List<ResultAPIItem> qresult =
            (List<ResultAPIItem>)user.getQueryResult(QueryType.queryTypeQueryApiApprove);

        return qresult[0];
    }

    private OpRes createAPI(GMUser user, ResultAPIItem item, ParamApiApprove resultParam)
    {
        bool res = user.sqlDb.keyStrExists(TableName.GM_ACCOUNT, "acc", item.m_apiAcc,
                                                user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (res)
            return OpRes.op_res_account_has_exists; // 账号重复

        CreateInfo info = new CreateInfo();
        res = getCreatorInfo(user, item.m_apiCreator, info);
        if (!res)
            return OpRes.op_res_failed;

        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("acc", item.m_apiAcc, FieldType.TypeString);
        gen.addField("pwd", item.m_apiPwd, FieldType.TypeString);
        gen.addField("accType", AccType.ACC_API, FieldType.TypeNumber);
        gen.addField("createTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("owner", item.m_apiCreator, FieldType.TypeString);
        
        gen.addField("generalAgency", info.m_generalAgency, FieldType.TypeString);
        
        gen.addField("postfix", item.m_apiPrefix, FieldType.TypeString);
        gen.addField("money", 0, FieldType.TypeNumber);
        gen.addField("moneyType", 0, FieldType.TypeNumber);

        string key = Guid.NewGuid().ToString().Replace("-", "");
        gen.addField("devSecretKey", key, FieldType.TypeString);
       
        gen.addField("gmRight", "", FieldType.TypeString);
        
        gen.addField("depth", info.m_depth + 1, FieldType.TypeNumber);
       
        string ccode = ItemHelp.genCreateCode(info.m_childCount, info.m_createCode);
        gen.addField("createCode", ccode, FieldType.TypeString);
        gen.addField("aliasName", item.m_apiAliasName, FieldType.TypeString);

        ValidatedCodeGenerator vg = new ValidatedCodeGenerator();
        vg.CodeSerial = DefCC.CODE_SERIAL;
        string validatedCode = vg.CreateVerifyCode(4);
        gen.addField("validatedCode", validatedCode, FieldType.TypeString);
        
        gen.addField("agentRatio", item.m_apiAgentRatio, FieldType.TypeNumber);
        gen.addField("washRatio", item.m_apiWashRatio, FieldType.TypeNumber);

        res = updateChildNodeNumber(user, info);
        if (res)
        {
            string sqlCmd = gen.getResultSql(TableName.GM_ACCOUNT);
            int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            if (count > 0)
            {
                resultParam.m_resultAcc = item.m_apiAcc;
                resultParam.m_validatedCode = validatedCode;

                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_API_APPROVE,
                        new LogApiApprove(item.m_apiAcc, item.m_apiAliasName), user);

                return OpRes.opres_success;
            }
        }

        return OpRes.op_res_db_failed;
    }

    bool getCreatorInfo(GMUser user, string creatorAcc, CreateInfo info)
    {
        string cmd = string.Format(QUERY_SQL, TableName.GM_ACCOUNT, creatorAcc);
        Dictionary<string, object> data = user.sqlDb.queryOne(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (data == null)
            return false;

        int count1 = Convert.ToInt32(data["childNodeNumber"]);
        int count2 = (int)user.sqlDb.getRecordCount(TableName.GM_ACCOUNT,
            string.Format("owner='{0}' ", creatorAcc), user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        info.m_acc = creatorAcc;

        info.m_childCount = Math.Max(count1, count2);
        info.m_createCode = Convert.ToString(data["createCode"]);
        info.m_depth = Convert.ToInt32(data["depth"]);
        info.m_generalAgency = Convert.ToString(data["generalAgency"]);
        return true;
    }

    bool updateChildNodeNumber(GMUser user, CreateInfo info)
    {
        GMUser dstUser = AccountSys.getInstance().getUser(info.m_acc);
        if (dstUser != null)
        {
            dstUser.m_childCount++;
        }

        info.m_childCount++;
        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("childNodeNumber", info.m_childCount, FieldType.TypeNumber);
        string sqlCmd = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", info.m_acc));
        int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0;
    }

    public class CreateInfo
    {
        public string m_acc;

        public int m_childCount;
        public string m_createCode;
        public int m_depth;
        public string m_generalAgency;
    }
}


//////////////////////////////////////////////////////////////////////////
public struct OrderOp
{
    // 执行订单
    public const int OP_EXEC = 0;

    // 取消订单
    public const int OP_CANCEL = 1;

    // 转发订单到上级
    public const int OP_FORWARD_TO_SUP = 2;

    // 自动转发订单到上级，客户端通过勾选来确定是否需要自动转发
    public const int OP_AUTO_FORWARD_TO_SUP = 3;
}

public class ParamPlayerOrder
{
    public string m_orderId;
    public string m_playerAcc;

    public int m_index;

    // 操作类型， 0 执行订单， 1 取消订单
    public int m_op;

    public int m_isAutoForward;
}

// 处理玩家订单号
public class DyOpPlayerOrder : DyOpBase
{
    const string SQL_CANCEL = "delete from {0} where orderId='{1}' and playerAcc='{2}' ";

    const string QUERY_SQL = " SELECT depth,createCode,childNodeNumber,generalAgency " +
                             " FROM {0} where acc='{1}' ";

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamPlayerOrder p = (ParamPlayerOrder)param;

        OpRes res = OpRes.op_res_failed;
        switch (p.m_op)
        {
            case OrderOp.OP_EXEC:
                {
                    ResultPlayerOrderItem item = getItem(p, user);
                    if (item == null)
                        return OpRes.op_res_failed;

                    res = execOrder(item, user);
                }
                break;
            case OrderOp.OP_CANCEL:
                {
                    ResultPlayerOrderItem item = getItem(p, user);
                    if (item == null)
                        return OpRes.op_res_failed;

                    res = cancel(item, user);
                }
                break;
            case OrderOp.OP_FORWARD_TO_SUP:
                {

                }
                break;
            case OrderOp.OP_AUTO_FORWARD_TO_SUP:
                {
                    res = autoForwardOrder(p, user);
                }
                break;
        }

        return res;
    }

    OpRes execOrder(ResultPlayerOrderItem item, GMUser user)
    {
        if (item.m_orderState != OrderState.STATE_WAIT)
            return OpRes.opres_success;

        ParamScore param = new ParamScore();
        param.m_op = item.m_orderType;
        param.m_toAcc = item.m_playerAcc;
        param.m_score = item.m_orderMoney.ToString();
        param.m_orderFrom = OrderGenerator.ORDER_FROM_PLAYER_ORDER;
        param.m_orderId = item.m_orderId;
        param.scoreToPlayer();

        OpRes res = user.doDyop(param, DyOpType.opTypeDyOpScore);
        if (res == OpRes.opres_success /*||
            res == OpRes.op_res_player_in_game*/)
        {
            OpRes code = removeWait(item, user);
            if (code == OpRes.opres_success)
            {
                if (res == OpRes.opres_success)
                {
                    addFinish(item, user, OrderState.STATE_FINISH);
                }
                else
                {
                    addFinish(item, user, OrderState.STATE_HAS_SUB);
                }
            }
        }
        else if (res == OpRes.op_res_player_in_game)
        {
            string cmd = CPlayerOrder.genUpdateSql(item.m_orderId, OrderState.STATE_HAS_SUB);
            int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            // 返回成功，界面可以重刷订单
            res = OpRes.opres_success;
        }
        return res;
    }

    OpRes cancel(ResultPlayerOrderItem item, GMUser user)
    {
        OpRes res = removeWait(item, user);
        if (res == OpRes.opres_success)
        {
            addFinish(item, user, OrderState.STATE_CANCEL);
        }
        return res;
    }

    OpRes removeWait(ResultPlayerOrderItem item, GMUser user)
    {
        string cmd = string.Format(SQL_CANCEL, TableName.PLAYER_ORDER_WAIT,
            item.m_orderId, item.m_playerAcc);
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    void addFinish(ResultPlayerOrderItem item, GMUser user, int orderState)
    {
        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("orderId", item.m_orderId, FieldType.TypeString);
        gen.addField("orderTime", item.m_orderTime, FieldType.TypeString);
        gen.addField("playerAcc", item.m_playerAcc, FieldType.TypeString);
        gen.addField("playerOwner", item.m_playerOwner, FieldType.TypeString);
        gen.addField("curOpAcc", user.m_user, FieldType.TypeString);
        gen.addField("orderState", orderState, FieldType.TypeNumber);
        gen.addField("playerOwnerCreator", item.m_playerOwnerCreator, FieldType.TypeString);
        gen.addField("orderMoney", item.m_orderMoney, FieldType.TypeNumber);
        gen.addField("orderType", item.m_orderType, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.PLAYER_ORDER_FINISH);
        int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }

    ResultPlayerOrderItem getItem(ParamPlayerOrder p, GMUser user)
    {
        ParamPlayerOrderQuery param = new ParamPlayerOrderQuery();
        param.m_orderState = OrderState.STATE_WAIT;
        param.m_orderId = p.m_orderId;
        param.m_playerAcc = p.m_playerAcc;
        param.m_op = 1;
        OpRes res = user.doQuery(param, QueryType.queryTypeQueryPlayerOrder);

        List<ResultPlayerOrderItem> items =
            (List<ResultPlayerOrderItem>)user.getQueryResult(QueryType.queryTypeQueryPlayerOrder);
        if (items.Count > 0)
            return items[0];
        return null;
    }

    OpRes autoForwardOrder(ParamPlayerOrder p, GMUser user)
    {
        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("forwardOrder", p.m_isAutoForward, FieldType.TypeNumber);
        string cond = string.Format(" acc = '{0}' ", user.m_user);
        string sqlCmd = gen.getResultSql(TableName.GM_ACCOUNT, cond);
        int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamModifyGmProperty
{
    // 修改别名
    public const int MODIFY_ALIASNAME = 1;
    public const int MODIFY_AGENT_RATION = 2;
    public const int MODIFY_WASH_RATION = 3;
    // 启用停用账号
    public const int MODIFY_ACC_STATE = 4;

    // 修改哪个属性
    public int m_whichProperty;
    public string m_acc = "";
    // 修改参数
    public string m_param = "";
}

// 修改GM属性
public class DyOpModifyGmProperty : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamModifyGmProperty p = (ParamModifyGmProperty)param;
        DestGmUser dst = new DestGmUser(p.m_acc, user);
        if (!dst.m_isExists)
            return OpRes.op_res_no_right;

        OpRes res = OpRes.op_res_failed;
        int count = 0;
        switch (p.m_whichProperty)
        {
            case ParamModifyGmProperty.MODIFY_ALIASNAME:
                {
                    if (!dst.isDerivedFrom(user))
                        return OpRes.op_res_no_right;

                    SqlUpdateGenerator gen = new SqlUpdateGenerator();
                    gen.addField("aliasName", p.m_param, FieldType.TypeString);
                    string sql = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", p.m_acc));
                    count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
                    res = count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
                }
                break;
            case ParamModifyGmProperty.MODIFY_AGENT_RATION:
                {
                    res = updateAgentRatio(p, dst, user);
                }
                break;
            case ParamModifyGmProperty.MODIFY_WASH_RATION:
                {
                    res = updateWashRatio(p, dst, user);
                }
                break;
            case ParamModifyGmProperty.MODIFY_ACC_STATE:
                {
                    res = modifyAccState(p, user);
                }
                break;
        }
        return res;
    }

    private OpRes updateAgentRatio(ParamModifyGmProperty p, DestGmUser dst, GMUser user)
    {
        if(dst.m_owner != user.m_user)
            return OpRes.op_res_no_right;

        ParamCreateGmAccount param = new ParamCreateGmAccount();
        param.m_agentRatio = p.m_param;
        double agentRatio = 0;
        if (!ItemHelp.isValidAgentRatio(param, user, ref agentRatio))
            return OpRes.op_res_param_not_valid;

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("agentRatio", agentRatio, FieldType.TypeNumber);

        string sql = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", p.m_acc));
        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    private OpRes updateWashRatio(ParamModifyGmProperty p, DestGmUser dst, GMUser user)
    {
        if (dst.m_owner != user.m_user)
            return OpRes.op_res_no_right;

        ParamCreateGmAccount param = new ParamCreateGmAccount();
        param.m_washRatio = p.m_param;
        double washRatio = 0;
        if (!ItemHelp.isValidWashRatio(param, user, ref washRatio))
            return OpRes.op_res_param_not_valid;

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("washRatio", washRatio, FieldType.TypeNumber);

        string sql = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", p.m_acc));
        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    OpRes modifyAccState(ParamModifyGmProperty p, GMUser user)
    {
        ParamStartStopGmAcc param = new ParamStartStopGmAcc();
        param.m_acc = p.m_acc;
        param.m_opType = Convert.ToInt32(p.m_param);
        OpRes res = user.doDyop(param, DyOpType.opTypeDyOpStartStopGmAcc);
        return res;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamDelData
{
    public string m_tableName = "";

    // 时间区间
    public string m_timeRange = "";
}

// 删除数据
public class DyOpDelData : DyOpBase
{
    private Dictionary<string, DyOpBase> m_items = new Dictionary<string, DyOpBase>();

    public DyOpDelData()
    {
        m_items.Add(TableName.GM_SCORE, new DyOpDelScoreLog());
    }

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamDelData p = (ParamDelData)param;
        if (!m_items.ContainsKey(p.m_tableName))
            return OpRes.op_res_failed;

        return m_items[p.m_tableName].doDyop(param, user);
    }
}

public class ParamDelDataScoreLog : ParamDelData
{
    // 目标账号
    public string m_playerAcc = "";

    // 操作者账号
    public string m_opAcc = "";
}

// 删除上下分记录。操作者只能删除自己及所有下级代理的操作记录
public class DyOpDelScoreLog : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        if (user.isSubAcc()) // 子账号没有操作权限
            return OpRes.op_res_no_right;

        ParamDelDataScoreLog p = (ParamDelDataScoreLog)param;
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_timeRange, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        string cond = getCondition(p, mint, user);

        SqlDeleteGenerator gen = new SqlDeleteGenerator();
        string cmd = gen.getResultSql(TableName.GM_SCORE, cond);
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        return count >= 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    string getCondition(ParamDelDataScoreLog p, DateTime mint, GMUser user)
    {
        QueryCondGenerator gen = new QueryCondGenerator();
        if (!string.IsNullOrEmpty(p.m_opAcc))
        {
            gen.addCondition(string.Format("opSrc='{0}'", p.m_opAcc));
        }
        if (!string.IsNullOrEmpty(p.m_playerAcc))
        {
            gen.addCondition(string.Format("opDst='{0}'", p.m_playerAcc));
        }

        gen.addCondition(string.Format(" opTime<'{0}' ", mint.ToString(ConstDef.DATE_TIME24)));
        gen.addCondition(string.Format(" opSrcCreateCode like '{0}%' ", user.m_createCode));

        return gen.and(false);
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamDelAcc
{
    public string m_acc;

    // 0删除代理号 1删除玩家
    public int m_op;
}

// 删除账号
public class DyOpDelAccount : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamDelAcc p = (ParamDelAcc)param;
        OpRes res = OpRes.op_res_failed;
        switch (p.m_op)
        {
            case 0:
                {
                    res = delGmAcc(p, user);
                }
                break;
            case 1:
                {
                    res = delPlayer(p, user);
                }
                break;
            case 2:
                {
                    res = delGmOpLog(p, user);
                }
                break;
        }

        return res;
    }

    private OpRes delGmAcc(ParamDelAcc p, GMUser user)
    {
        DestGmUser dst = new DestGmUser(p.m_acc, user);
        if(dst.m_isSelf)
            return OpRes.op_res_no_right;

        if (!dst.m_isExists)
            return OpRes.op_res_no_right;

        if (!dst.isDerivedFrom(user))
            return OpRes.op_res_no_right;

        int childCount = (int)user.sqlDb.getRecordCount(TableName.GM_ACCOUNT,
            string.Format("owner='{0}' ", dst.m_owner), 0, MySqlDbName.DB_XIANXIA);
        SqlUpdateGenerator up = new SqlUpdateGenerator();
        up.addField("childNodeNumber", childCount, FieldType.TypeNumber);
        string sql = up.getResultSql(TableName.GM_ACCOUNT, string.Format("acc='{0}' and childNodeNumber=0 ", dst.m_owner));
        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (count >= 0)
        {
            SqlDeleteGenerator gen = new SqlDeleteGenerator();
            string cmd1 = gen.getResultSql(TableName.GM_ACCOUNT,
                 string.Format(" createCode like '{0}%' ", dst.m_createCode));
            user.sqlDb.executeOp(cmd1, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

            string cmd2 = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                string.Format(" createCode like '{0}%' ", dst.m_createCode));
            count = user.sqlDb.executeOp(cmd2, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

            return count >= 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }

        return OpRes.op_res_db_failed;
    }

    private OpRes delPlayer(ParamDelAcc p, GMUser user)
    {
        Player player = new Player(p.m_acc, user);
        if (!player.m_isExists)
            return OpRes.op_res_player_not_exist;

        if (!player.isDerivedFrom(user))
            return OpRes.op_res_no_right;

        SqlDeleteGenerator gen = new SqlDeleteGenerator();
        string cmd = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA, string.Format("acc='{0}'", p.m_acc));
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    // 删除操作日志
    private OpRes delGmOpLog(ParamDelAcc p, GMUser user)
    {
        if (!user.isAdmin())
            return OpRes.op_res_no_right;

        SqlDeleteGenerator gen = new SqlDeleteGenerator();
        string cmd = gen.getResultSql(TableName.OPLOG, "1=1");
        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count >= 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
// 开关游戏
public class DyOpOpenGame : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        if (!user.isAPIAcc())
            return OpRes.op_res_no_right;

        string gameCloseList = (string)param;
        string gameList = ItemHelp.getReverseGameList(gameCloseList);

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("gameClose", gameList, FieldType.TypeString);
        string sql = gen.getResultSql(TableName.GM_ACCOUNT,
            string.Format("acc='{0}'", user.m_user));
        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家相关操作
public class DyOpPlayerOp : DyOpPlayerBase
{
    // 修改玩家别名
    public OpRes modifyPlayerAliasName(string acc, string newName, GMUser user)
    {
        if (!canOpPlayer(acc, user))
            return OpRes.op_res_no_right;

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("aliasName", newName, FieldType.TypeString);
        string cmd = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
            string.Format("acc='{0}'", acc));

        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    // 重置玩家登录密码
    public OpRes resetPlayerPwd(string acc, string newPwd, GMUser user)
    {
        ParamModifyPwd p = new ParamModifyPwd();
        p.m_account = acc;
        p.m_newPwd = newPwd;
        p.m_pwdType = 0; 
        OpRes res = user.doDyop(p, DyOpType.opTypeModifyPwd);
        return res;
    }

    // 设置是否影响盈利率
    public OpRes playerAffectEarnRate(string acc, bool isAffect, GMUser user)
    {
        ParamPlayerSpecialFlag param = new ParamPlayerSpecialFlag();
        param.m_acc = acc;
        param.m_isAffectEarning = isAffect;
        OpRes res = user.doDyop(param, DyOpType.opTypeSetPlayerSpecialFlag);
        return res;
    }

    // 停封/解封玩家账号
    public OpRes blockPlayerAcc(string acc, bool block, GMUser user)
    {
        ParamBlock param = new ParamBlock();
        param.m_param = acc;
        param.m_isBlock = block;
        OpRes res = user.doDyop(param, DyOpType.opTypeBlockAcc);
        return res;
    }

    // 踢出玩家
    public OpRes kickPlayerAcc(string acc, GMUser user)
    {
        ParamPlayerOp param = new ParamPlayerOp();
        param.m_acc = acc;
        OpRes res = user.doDyop(param, DyOpType.opTypeKickPlayer);
        return res;
    }

    // 清理登录失败次数
    public OpRes clearPlayerFailCount(string acc, GMUser user)
    {
        ParamPlayerOp param = new ParamPlayerOp();
        param.m_acc = acc;
        OpRes res = user.doDyop(param, DyOpType.opTypeClearLoginFailed);
        return res;
    }

    // 解锁玩家
    public OpRes unlockPlayer(string acc, GMUser user)
    {
        ParamPlayerOp param = new ParamPlayerOp();
        param.m_acc = acc;
        OpRes res = user.doDyop(param, DyOpType.opTypeUnlockPlayer);
        return res;
    }
}












