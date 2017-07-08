<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonDateSpan.ascx.cs" Inherits="WebManager.ascx.CommonDateSpan" %>

<div>
    <script type="text/javascript">
        $(function () {
            //$.datetimepicker.setLocale('ch');
            $('#datetimepickerleft').datetimepicker({ format: 'Y/m/d H:i' });
            $('#datetimepickerleft_open').click(function () {
                $('#datetimepickerleft').datetimepicker('show');
            });

            $('#datetimepickerright').datetimepicker({ format: 'Y/m/d H:i' });
            $('#datetimepickerright_open').click(function () {
                $('#datetimepickerright').datetimepicker('show');
            });
        });
    </script>

    <asp:TextBox type="text" ID="datetimepickerleft" runat="server" ClientIDMode="Static" ></asp:TextBox>
    <input type="button" value="日历" id="datetimepickerleft_open" />
    到  <asp:TextBox ID="datetimepickerright" runat="server" ClientIDMode="Static" ></asp:TextBox>
    <input type="button" value="日历" id="datetimepickerright_open" />
</div>