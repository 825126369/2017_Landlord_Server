using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace HttpLogin
{
    public partial class LogService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string rootPath = HttpContext.Current.Server.MapPath("./gameLog");
            Directory.CreateDirectory(rootPath);

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string fileName = Guid.NewGuid().ToString() + ".log";
                string path = Path.Combine(rootPath, fileName);
                file.SaveAs(path);
            }
        }
    }
}