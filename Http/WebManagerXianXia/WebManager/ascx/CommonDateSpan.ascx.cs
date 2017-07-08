using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.ascx {
    public partial class CommonDateSpan : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {

        }

        public string getDateTimeSpanLeft() {
            return datetimepickerleft.Text;
        }

        public string getDateTimeSpanRight() {
            return datetimepickerright.Text;
        }
    }
}