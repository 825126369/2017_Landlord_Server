using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// ValidatedCode 的摘要说明
    /// </summary>
    public class ValidatedCode : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            UserVerification ver = (UserVerification)context.Session[DefCC.KEY_VERIFICATION];
            if (ver == null)
            {
                //LOGW.Info("生成验证码时没有找到key:{0}", DefCC.KEY_VERIFICATION);
                ver = new UserVerification();
                context.Session[DefCC.KEY_VERIFICATION] = ver;
                //return;
            }

            ValidatedCodeGenerator gen = new ValidatedCodeGenerator();
            ver.m_validatedCode = gen.CreateVerifyCode(6);
            gen.CreateImageOnPage(ver.m_validatedCode, context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}