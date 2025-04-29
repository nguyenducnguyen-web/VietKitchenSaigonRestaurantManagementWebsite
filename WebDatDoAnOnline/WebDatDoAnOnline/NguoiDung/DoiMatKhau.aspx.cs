/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DoiMatKhau : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ResetEmail"] == null)
            {
                ShowErrorAlert("Phiên làm việc không hợp lệ. Vui lòng thử lại!");
                Response.Redirect("KhoiPhucMatKhau.aspx");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = Session["ResetEmail"]?.ToString();
                string newPassword = txtNewPassword.Text.Trim();
                if (!string.IsNullOrEmpty(email))
                {
                    try
                    {
                        UpdatePassword(email, newPassword);
                        ShowSuccessAlert("Đổi mật khẩu thành công! Bạn sẽ được chuyển về trang đăng nhập.");
                        Session["ResetEmail"] = null;
                        Session["OTP"] = null;
                        Session["OTPTime"] = null;
                        string redirectScript = "setTimeout(function() { window.location.href = 'DangNhap.aspx'; }, 3000);"; // Đồng bộ với timer
                        ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", redirectScript, true);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorAlert("Lỗi khi cập nhật mật khẩu: " + ex.Message);
                    }
                }
                else
                {
                    ShowErrorAlert("Phiên làm việc đã hết hạn. Vui lòng thử lại!");
                }
            }
        }

        private void UpdatePassword(string email, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE NguoiDung SET MatKhauNguoiDung = HASHBYTES('SHA2_256', @Password) WHERE EmailNguoiDung = @Email", con))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ShowSuccessAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Thành công!',
                    html: '{message}',
                    icon: 'success',
                    confirmButtonText: 'Tuyệt vời!',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('successSound').play();
                        confetti({{
                            particleCount: 150,
                            spread: 90,
                            colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                            origin: {{ y: 0.6 }}
                        }});
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert", script, true);
        }

        private void ShowErrorAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Lỗi!',
                    html: '{message}',
                    icon: 'error',
                    confirmButtonText: 'Thử lại',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('errorSound').play();
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
        }
    }
}


/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DoiMatKhau : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ResetEmail"] == null || Session["AccountType"] == null)
            {
                ShowErrorAlert("Phiên làm việc không hợp lệ. Vui lòng thử lại!");
                Response.Redirect("KhoiPhucMatKhau.aspx");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = Session["ResetEmail"]?.ToString();
                string accountType = Session["AccountType"]?.ToString();
                string newPassword = txtNewPassword.Text.Trim();

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(accountType))
                {
                    try
                    {
                        UpdatePassword(email, newPassword, accountType);
                        ShowSuccessAlert("Đổi mật khẩu thành công! Bạn sẽ được chuyển về trang đăng nhập.");
                        Session["ResetEmail"] = null;
                        Session["OTP"] = null;
                        Session["OTPTime"] = null;
                        Session["AccountType"] = null;
                        string redirectScript = "setTimeout(function() { window.location.href = 'DangNhap.aspx'; }, 3000);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", redirectScript, true);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorAlert("Lỗi khi cập nhật mật khẩu: " + ex.Message);
                    }
                }
                else
                {
                    ShowErrorAlert("Phiên làm việc đã hết hạn hoặc thông tin tài khoản không hợp lệ. Vui lòng thử lại!");
                }
            }
        }

        private void UpdatePassword(string email, string newPassword, string accountType)
        {
            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                string query;
                if (accountType == "NguoiDung")
                {
                    query = "UPDATE NguoiDung SET MatKhauNguoiDung = HASHBYTES('SHA2_256', @Password) WHERE EmailNguoiDung = @Email";
                }
                else if (accountType == "NhanVien")
                {
                    query = "UPDATE NhanVien SET MatKhauNhanVien = HASHBYTES('SHA2_256', @Password) WHERE EmailNhanVien = @Email";
                }
                else
                {
                    throw new Exception("Loại tài khoản không hợp lệ.");
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("Không tìm thấy tài khoản với email này.");
                    }
                }
            }
        }

        private void ShowSuccessAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Thành công!',
                    html: '{message}',
                    icon: 'success',
                    confirmButtonText: 'Tuyệt vời!',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('successSound').play();
                        confetti({{
                            particleCount: 150,
                            spread: 90,
                            colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                            origin: {{ y: 0.6 }}
                        }});
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert", script, true);
        }

        private void ShowErrorAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Lỗi!',
                    html: '{message}',
                    icon: 'error',
                    confirmButtonText: 'Thử lại',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('errorSound').play();
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
        }
    }
}*/



using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DoiMatKhau : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ResetEmail"] == null || Session["UserType"] == null)
            {
                ShowErrorAlert("Phiên làm việc không hợp lệ. Vui lòng thử lại!");
                Response.Redirect("KhoiPhucMatKhau.aspx");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = Session["ResetEmail"]?.ToString();
                string userType = Session["UserType"]?.ToString();
                string newPassword = txtNewPassword.Text.Trim();
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(userType))
                {
                    try
                    {
                        UpdatePassword(email, newPassword, userType);
                        ShowSuccessAlert("Đổi mật khẩu thành công! Bạn sẽ được chuyển về trang đăng nhập.");
                        Session["ResetEmail"] = null;
                        Session["OTP"] = null;
                        Session["OTPTime"] = null;
                        Session["UserType"] = null;
                        string redirectScript = "setTimeout(function() { window.location.href = 'DangNhap.aspx'; }, 3000);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", redirectScript, true);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorAlert("Lỗi khi cập nhật mật khẩu: " + ex.Message);
                    }
                }
                else
                {
                    ShowErrorAlert("Phiên làm việc đã hết hạn. Vui lòng thử lại!");
                }
            }
        }

        private void UpdatePassword(string email, string newPassword, string userType)
        {
            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                string query = userType == "NguoiDung"
                    ? "UPDATE NguoiDung SET MatKhauNguoiDung = HASHBYTES('SHA2_256', @Password) WHERE EmailNguoiDung = @Email"
                    : "UPDATE NhanVien SET MatKhauNhanVien = HASHBYTES('SHA2_256', @Password) WHERE EmailNhanVien = @Email";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ShowSuccessAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Thành công!',
                    html: '{message}',
                    icon: 'success',
                    confirmButtonText: 'Tuyệt vời!',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('successSound').play();
                        confetti({{
                            particleCount: 150,
                            spread: 90,
                            colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                            origin: {{ y: 0.6 }}
                        }});
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert", script, true);
        }

        private void ShowErrorAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Lỗi!',
                    html: '{message}',
                    icon: 'error',
                    confirmButtonText: 'Thử lại',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('errorSound').play();
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
        }
    }
}