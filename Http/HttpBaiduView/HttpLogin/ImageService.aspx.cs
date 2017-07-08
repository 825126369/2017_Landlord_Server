using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Drawing;
using System.Web.Script.Serialization;

namespace HttpLogin
{
    public partial class ImageService : System.Web.UI.Page
    {
        protected JavaScriptSerializer jser = new JavaScriptSerializer();
        protected void Page_Load(object sender, EventArgs e)
        {
            string player_id = Request.Params["playerId"];
            string extensions = Request.Params["extensions"];

            if (string.IsNullOrEmpty(player_id) || string.IsNullOrEmpty(extensions) )
            {
                return;
            }

            string directory = HttpContext.Current.Server.MapPath("./Picture");

            Dictionary<string, string> result = new Dictionary<string, string>();
            result["result"] = "0";
            
            CreateDirectoryIfNotExist(directory);
            string player_photo_dir = Path.Combine(directory, GetSimpleHash(player_id));
            CreateDirectoryIfNotExist(player_photo_dir);

            try
            {
                if (Request.Files.Count == 1)
                {
                    // 写入新文件
                    var ft = Request.Files[0];
                    string hash_string = GetStreamHash(ft.InputStream);                   
                    string realName = hash_string + "_100." + extensions;
                    string path = Path.Combine(player_photo_dir, realName);                   
                    ft.SaveAs(path);      

                    result["filename"] = realName;
                    result["result"] = "1";
                }
                else
                {
                    result["info"] = "can't find file!";
                }
            }
            catch (Exception error)
            {
                //扔给用户错误
                result["info"] = error.Message;
            }

            Response.Write(Server.UrlEncode(jser.Serialize(result)));
        }

        protected string GetStreamHash(Stream stream)
        {
            var md5_maker = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var hash = md5_maker.ComputeHash(stream);
            return string.Join("", hash.Select(t => t.ToString("x2")).ToArray());
        }

        protected void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))            
                Directory.CreateDirectory(path);           
        }

        protected string GetSimpleHash(string player_id)
        {
            uint temp = Convert.ToUInt32(player_id) % 10000;
            return temp.ToString();           
        }
    }
}