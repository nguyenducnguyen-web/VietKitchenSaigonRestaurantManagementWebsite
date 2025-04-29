using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class NguoiDung : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.Url.AbsoluteUri.ToString().Contains("TrangChu.aspx"))
            {
                form1.Attributes.Add("class", "sub_page");
            }
            else
            {
                form1.Attributes.Remove("class");
                Control SliderUserControl = (Control)Page.LoadControl("SliderUserControl.ascx");
                pnlSliderUC.Controls.Add(SliderUserControl);
            }

            // Khởi tạo và cập nhật Session["cartCount"]
            if (Session["MaNguoiDung"] != null)
            {
                lbLoginOrLogout.Text = "Đăng xuất";
                Utils utils = new Utils();
                Session["cartCount"] = utils.cartCount(Convert.ToInt32(Session["MaNguoiDung"]));
            }
            else
            {
                lbLoginOrLogout.Text = "Đăng nhập";
                Session["cartCount"] = "0"; // Hiển thị 0 nếu chưa đăng nhập
            }
        }

        protected void lbLoginOrLogout_Click(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] == null)
            {
                Response.Redirect("DangNhap.aspx");
            }
            else
            {
                Session.Abandon();
                Response.Redirect("DangNhap.aspx");
            }
        }

        protected void lbRegisterOrProfile_Click(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] != null)
            {
                lbRegisterOrProfile.ToolTip = "User Profile";
                Response.Redirect("TrangThongTinNguoiDung.aspx");
            }
            else
            {
                lbRegisterOrProfile.ToolTip = "User Registration";
                Response.Redirect("DangKyTaiKhoan.aspx");
            }
        }
    }
}