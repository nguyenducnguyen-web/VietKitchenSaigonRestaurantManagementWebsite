using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.Admin
{
    public partial class BangDieuKhien : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    LoadDashboardCounts();
                    ShowWelcomeNotification();
                }
            }
        }

        private void ShowWelcomeNotification()
        {
            string adminName = Session["admin"]?.ToString().Trim() ?? "Admin";
            string script = @"
                Swal.fire({
                    title: 'Chào mừng đến với Viet Kitchen Sài Gòn!',
                    html: 'Quản lý nhà hàng một cách tinh tế, " + adminName + @"!',
                    icon: 'success',
                    confirmButtonText: 'Tuyệt vời!',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    },
                    didOpen: () => {
                        document.getElementById('successSound').play();
                        confetti({
                            particleCount: 150,
                            spread: 90,
                            colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                            origin: { y: 0.6 }
                        });
                    }
                });
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "WelcomeAlert", script, true);
        }

        private void LoadDashboardCounts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    con.Open();

                    // Dictionary to store counts
                    Dictionary<string, string> counts = new Dictionary<string, string>();

                    // Existing counts
                    counts["category"] = GetCount(con, "SELECT COUNT(*) FROM LoaiMonAn");
                    counts["product"] = GetCount(con, "SELECT COUNT(*) FROM MonAn");
                    counts["order"] = GetCount(con, "SELECT COUNT(*) FROM Orders");
                    counts["delivered"] = GetCount(con, "SELECT COUNT(*) FROM Orders WHERE TrangThai = N'Đã giao hàng'");
                    counts["pending"] = GetCount(con, "SELECT COUNT(*) FROM Orders WHERE TrangThai = N'Đang xử lý'");
                    counts["user"] = GetCount(con, "SELECT COUNT(*) FROM NguoiDung");
                    counts["soldAmount"] = GetCount(con, "SELECT ISNULL(SUM(GiaMonAnSauKhuyenMai * SoLuong), 0) FROM Orders WHERE TrangThai = N'Đã giao hàng'");
                    counts["datban"] = GetCount(con, "SELECT COUNT(*) FROM DatBan");
                    counts["contact"] = GetCount(con, "SELECT COUNT(*) FROM LienHe");

                    // New counts for inventory management
                    counts["phieuNhap"] = GetCount(con, "SELECT COUNT(*) FROM PhieuNhap");
                    counts["phieuXuat"] = GetCount(con, "SELECT COUNT(*) FROM PhieuXuat");
                    counts["nguyenLieu"] = GetCount(con, "SELECT COUNT(*) FROM NguyenLieu");
                    counts["nhaCungCap"] = GetCount(con, "SELECT COUNT(*) FROM NhaCungCap");

                    // Assign to Session and format numbers
                    foreach (var count in counts)
                    {
                        if (count.Key == "soldAmount")
                        {
                            Session[count.Key] = string.Format("{0:N0} VND", Convert.ToDecimal(count.Value));
                        }
                        else
                        {
                            Session[count.Key] = string.Format("{0:N0}", Convert.ToInt32(count.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string script = @"
                    Swal.fire({
                        title: 'Lỗi!',
                        html: 'Không thể tải thống kê: " + ex.Message.Replace("'", "\\'") + @"',
                        icon: 'error',
                        confirmButtonText: 'OK',
                        backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            htmlContainer: 'custom-swal-text',
                            confirmButton: 'custom-swal-button'
                        },
                        didOpen: () => {
                            document.getElementById('errorSound').play();
                        }
                    });
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
            }
        }

        private string GetCount(SqlConnection con, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "0";
            }
        }
    }
}