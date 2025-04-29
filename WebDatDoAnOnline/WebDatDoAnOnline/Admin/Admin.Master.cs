using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null || Session["Quyen"] == null)
            {
                // Redirect to login if not authenticated
                Response.Redirect("../NguoiDung/DangNhap.aspx");
            }
            else
            {
                string quyen = Session["Quyen"].ToString();
                string currentPage = Request.Path.Split('/').Last().ToLower();

                // Define allowed pages for each role
                List<string> nhanVienBanHangPages = new List<string>
                {
                    
                    "quanlytrangthaidonhang.aspx",
                    "quanlydatban.aspx",
                    "quanlylienhenguoidung.aspx",
                    "trangthongtintaikhoan.aspx"
                };

                List<string> nhanVienKhoPages = new List<string>
                {
                   
                    "taophieunhap.aspx",
                    "taophieuxuat.aspx",
                    "trangthongtintaikhoan.aspx"
                };

                // Check access based on role
                if (quyen == "Nhân viên bán hàng")
                {
                    if (!nhanVienBanHangPages.Contains(currentPage))
                    {
                        // Hiển thị thông báo lỗi và chuyển hướng
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            "showErrorAlert('Bạn không có quyền truy cập trang này!'); setTimeout(function(){ window.location.href = 'QuanLyTrangThaiDonHang.aspx'; }, 2000);", true);
                    }
                }
                else if (quyen == "Nhân viên kho")
                {
                    if (!nhanVienKhoPages.Contains(currentPage))
                    {
                        // Hiển thị thông báo lỗi và chuyển hướng
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            "showErrorAlert('Bạn không có quyền truy cập trang này!'); setTimeout(function(){ window.location.href = 'TaoPhieuNhap.aspx'; }, 2000);", true);
                    }
                }
                else if (quyen != "Admin")
                {
                    // Redirect to login if role is invalid
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
            }
        }

        protected void lbLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("../NguoiDung/DangNhap.aspx");
        }
    }
}