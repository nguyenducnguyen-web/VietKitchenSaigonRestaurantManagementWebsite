/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DangNhap : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] != null || Session["admin"] != null)
            {
                Response.Redirect("TrangChu.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());

                // Step 1: Check NhanVien table
                cmd = new SqlCommand("DangNhapNhanVien_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "SELECT4LOGIN");
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    // NhanVien login successful
                    Session["admin"] = txtUsername.Text.Trim();
                    Session["Quyen"] = dt.Rows[0]["Quyen"].ToString(); // Store the role for potential role-based access
                    string script = @"
                        Swal.fire({
                            title: 'Đăng nhập thành công!',
                            html: 'Chào mừng " + txtUsername.Text.Trim() + @" đến với bảng điều khiển!',
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
                        }).then(() => { window.location.href = '../Admin/BangDieuKhien.aspx'; });
                    ";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", script, true);
                    return;
                }

                // Step 2: Check NguoiDung table
                cmd = new SqlCommand("User_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "SELECT4LOGIN");
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    // NguoiDung login successful
                    Session["username"] = txtUsername.Text.Trim();
                    Session["MaNguoiDung"] = dt.Rows[0]["MaNguoiDung"];
                    string script = @"
                        Swal.fire({
                            title: 'Đăng nhập thành công!',
                            html: 'Chào mừng bạn trở lại, " + txtUsername.Text.Trim() + @"!',
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
                        }).then(() => { window.location.href = 'TrangChu.aspx'; });
                    ";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", script, true);
                }
                else
                {
                    // Login failed
                    string errorScript = @"
                        Swal.fire({
                            title: 'Đăng nhập thất bại!',
                            html: 'Thông tin đăng nhập không hợp lệ!',
                            icon: 'error',
                            confirmButtonText: 'Thử lại',
                            backdrop: `rgba(0, 0, 0, 0.7)`,
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
                }
            }
            catch (Exception ex)
            {
                string errorScript = @"
                    Swal.fire({
                        title: 'Lỗi hệ thống!',
                        html: 'Đã xảy ra lỗi: " + ex.Message.Replace("'", "\\'") + @"',
                        icon: 'error',
                        confirmButtonText: 'OK',
                        backdrop: `rgba(0, 0, 0, 0.7)`,
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
            }
        }
    }
}*/



using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DangNhap : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] != null || Session["admin"] != null)
            {
                Response.Redirect("TrangChu.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());

                // Step 1: Check NhanVien table
                cmd = new SqlCommand("DangNhapNhanVien_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "SELECT4LOGIN");
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    // NhanVien login successful
                    Session["admin"] = txtUsername.Text.Trim();
                    Session["Quyen"] = dt.Rows[0]["Quyen"].ToString(); // Store the role for potential role-based access
                    string quyen = dt.Rows[0]["Quyen"].ToString();
                    string redirectUrl = "../Admin/BangDieuKhien.aspx"; // Default redirect
                    if (quyen == "Nhân viên kho")
                    {
                        redirectUrl = "../Admin/TaoPhieuNhap.aspx";
                    }
                    else if (quyen == "Nhân viên bán hàng")
                    {
                        redirectUrl = "../Admin/QuanLyTrangThaiDonHang.aspx";
                    }

                    string script = @"
                        Swal.fire({
                            title: 'Đăng nhập thành công!',
                            html: 'Chào mừng " + txtUsername.Text.Trim() + @" đến với bảng điều khiển!',
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
                        }).then(() => { window.location.href = '" + redirectUrl + @"'; });
                    ";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", script, true);
                    return;
                }

                // Step 2: Check NguoiDung table
                cmd = new SqlCommand("User_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "SELECT4LOGIN");
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    // NguoiDung login successful
                    Session["username"] = txtUsername.Text.Trim();
                    Session["MaNguoiDung"] = dt.Rows[0]["MaNguoiDung"];
                    string script = @"
                        Swal.fire({
                            title: 'Đăng nhập thành công!',
                            html: 'Chào mừng bạn trở lại, " + txtUsername.Text.Trim() + @"!',
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
                        }).then(() => { window.location.href = 'TrangChu.aspx'; });
                    ";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", script, true);
                }
                else
                {
                    // Login failed
                    string errorScript = @"
                        Swal.fire({
                            title: 'Đăng nhập thất bại!',
                            html: 'Thông tin đăng nhập không hợp lệ!',
                            icon: 'error',
                            confirmButtonText: 'Thử lại',
                            backdrop: `rgba(0, 0, 0, 0.7)`,
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
                }
            }
            catch (Exception ex)
            {
                string errorScript = @"
                    Swal.fire({
                        title: 'Lỗi hệ thống!',
                        html: 'Đã xảy ra lỗi: " + ex.Message.Replace("'", "\\'") + @"',
                        icon: 'error',
                        confirmButtonText: 'OK',
                        backdrop: `rgba(0, 0, 0, 0.7)`,
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
            }
        }
    }
}