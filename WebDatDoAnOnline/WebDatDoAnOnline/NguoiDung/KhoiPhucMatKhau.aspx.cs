/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class KhoiPhucMatKhau : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ResetEmail"] = null;
                Session["OTP"] = null;
                Session["OTPTime"] = null;
            }
        }

        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = txtEmail.Text.Trim();
                if (CheckEmailExists(email))
                {
                    try
                    {
                        string otpCode = GenerateOTP();
                        Session["OTP"] = otpCode;
                        Session["ResetEmail"] = email;
                        Session["OTPTime"] = DateTime.Now;
                        SendOTPEmail(email, otpCode);
                        ShowSuccessAlert("Mã OTP đã được gửi đến email của bạn!");
                        Response.Redirect("XacNhanMaOTP.aspx");
                    }
                    catch (Exception ex)
                    {
                        ShowErrorAlert("Lỗi khi gửi email: " + ex.Message);
                    }
                }
                else
                {
                    ShowErrorAlert("Chưa có người dùng nào đăng ký bằng email này!");
                }
            }
            else
            {
                ShowErrorAlert("Vui lòng nhập email hợp lệ!");
            }
        }

        private bool CheckEmailExists(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE EmailNguoiDung = @Email", con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert("Lỗi khi kiểm tra email: " + ex.Message);
                return false;
            }
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private void SendOTPEmail(string toEmail, string otp)
        {
            string fromEmail = "nhahangvietkitchensaigon@gmail.com";
            string appPassword = "ktcb hsii xcho czpn";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = "Đặt lại mật khẩu tài khoản";

            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<!DOCTYPE html>");
            htmlContent.Append("<html lang='vi'>");
            htmlContent.Append("<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            htmlContent.Append("<title>Đặt lại mật khẩu</title></head>");
            htmlContent.Append("<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f6f6f6;'>");
            htmlContent.Append("<table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: #ffffff; margin: 40px auto; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>");
            htmlContent.Append("<tr><td style='background: linear-gradient(45deg, #ff0000, #ff6f61); text-align: center; padding: 20px; border-top-left-radius: 8px; border-top-right-radius: 8px;'>");
            htmlContent.Append("<h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: bold;'>Website đặt món ăn Online nhà hàng Việt Kitchen Sài Gòn</h1></td></tr>");
            htmlContent.Append("<tr><td style='padding: 40px 30px;'>");
            htmlContent.Append("<h2 style='color: #1c2526; font-size: 20px; margin: 0 0 10px 0;'>Yêu cầu đặt lại mật khẩu</h2>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Xin chào,</p>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Mình là Nguyễn Đức Nguyên (admin của chương trình), mình nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã xác nhận dưới đây nhé:</p>");
            htmlContent.Append("<div style='text-align: center; margin: 30px 0;'>");
            htmlContent.Append("<span style='display: inline-block; background-color: #e7f3ff; color: #ff6f61; font-size: 24px; font-weight: bold; padding: 10px 20px; border-radius: 4px; letter-spacing: 2px;'>" + otp + "</span>");
            htmlContent.Append("</div>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Vui lòng nhập mã này vào giao diện khôi phục mật khẩu. Mã này có hiệu lực trong 10 phút.</p>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0;'>Trân trọng,<br>Website Đặt Đồ Ăn Online nhà hàng Việt Kitchen Sài Gòn</p>");
            htmlContent.Append("</td></tr>");
            htmlContent.Append("<tr><td style='background-color: #f6f6f6; text-align: center; padding: 20px; border-bottom-left-radius: 8px; border-bottom-right-radius: 8px;'>");
            htmlContent.Append("<p style='color: #90949c; font-size: 12px; margin: 0;'>© 2025 Website Đặt Đồ Ăn Online. All rights reserved.</p>");
            htmlContent.Append("<p style='color: #90949c; font-size: 12px; margin: 0;'>Nếu có thắc mắc, vui lòng liên hệ Nguyễn Đức Nguyên: <a href='mailto:nhahangvietkitchensaigon@gmail.com'>nhahangvietkitchensaigon@gmail.com</a></p>");
            htmlContent.Append("</td></tr></table></body></html>");

            mail.Body = htmlContent.ToString();
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
            smtp.EnableSsl = true;
            smtp.Send(mail);
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
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class KhoiPhucMatKhau : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ResetEmail"] = null;
                Session["OTP"] = null;
                Session["OTPTime"] = null;
                Session["UserType"] = null;
            }
        }

        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = txtEmail.Text.Trim();
                string userType;
                if (CheckEmailExists(email, out userType))
                {
                    try
                    {
                        string otpCode = GenerateOTP();
                        Session["OTP"] = otpCode;
                        Session["ResetEmail"] = email;
                        Session["OTPTime"] = DateTime.Now;
                        Session["UserType"] = userType;
                        SendOTPEmail(email, otpCode);
                        ShowSuccessAlert("Mã OTP đã được gửi đến email của bạn!");
                        Response.Redirect("XacNhanMaOTP.aspx");
                    }
                    catch (Exception ex)
                    {
                        ShowErrorAlert("Lỗi khi gửi email: " + ex.Message);
                    }
                }
                else
                {
                    ShowErrorAlert("Chưa có người dùng hoặc nhân viên nào đăng ký bằng email này!");
                }
            }
            else
            {
                ShowErrorAlert("Vui lòng nhập email hợp lệ!");
            }
        }

        private bool CheckEmailExists(string email, out string userType)
        {
            userType = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    con.Open();
                    // Check NguoiDung
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE EmailNguoiDung = @Email", con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            userType = "NguoiDung";
                            return true;
                        }
                    }
                    // Check NhanVien
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE EmailNhanVien = @Email", con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            userType = "NhanVien";
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert("Lỗi khi kiểm tra email: " + ex.Message);
                return false;
            }
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private void SendOTPEmail(string toEmail, string otp)
        {
            string fromEmail = "nhahangvietkitchensaigon@gmail.com";
            string appPassword = "ktcb hsii xcho czpn";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = "Đặt lại mật khẩu tài khoản";

            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<!DOCTYPE html>");
            htmlContent.Append("<html lang='vi'>");
            htmlContent.Append("<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            htmlContent.Append("<title>Đặt lại mật khẩu</title></head>");
            htmlContent.Append("<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f6f6f6;'>");
            htmlContent.Append("<table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: #ffffff; margin: 40px auto; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>");
            htmlContent.Append("<tr><td style='background: linear-gradient(45deg, #ff0000, #ff6f61); text-align: center; padding: 20px; border-top-left-radius: 8px; border-top-right-radius: 8px;'>");
            htmlContent.Append("<h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: bold;'>Website đặt món ăn Online nhà hàng Việt Kitchen Sài Gòn</h1></td></tr>");
            htmlContent.Append("<tr><td style='padding: 40px 30px;'>");
            htmlContent.Append("<h2 style='color: #1c2526; font-size: 20px; margin: 0 0 10px 0;'>Yêu cầu đặt lại mật khẩu</h2>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Xin chào,</p>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Mình là Nguyễn Đức Nguyên (admin của chương trình), mình nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã xác nhận dưới đây nhé:</p>");
            htmlContent.Append("<div style='text-align: center; margin: 30px 0;'>");
            htmlContent.Append("<span style='display: inline-block; background-color: #e7f3ff; color: #ff6f61; font-size: 24px; font-weight: bold; padding: 10px 20px; border-radius: 4px; letter-spacing: 2px;'>" + otp + "</span>");
            htmlContent.Append("</div>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Vui lòng nhập mã này vào giao diện khôi phục mật khẩu. Mã này có hiệu lực trong 10 phút.</p>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>");
            htmlContent.Append("<p style='color: #606770; font-size: 16px; line-height: 24px; margin: 0;'>Trân trọng,<br>Website Đặt Đồ Ăn Online nhà hàng Việt Kitchen Sài Gòn</p>");
            htmlContent.Append("</td></tr>");
            htmlContent.Append("<tr><td style='background-color: #f6f6f6; text-align: center; padding: 20px; border-bottom-left-radius: 8px; border-bottom-right-radius: 8px;'>");
            htmlContent.Append("<p style='color: #90949c; font-size: 12px; margin: 0;'>© 2025 Website Đặt Đồ Ăn Online. All rights reserved.</p>");
            htmlContent.Append("<p style='color: #90949c; font-size: 12px; margin: 0;'>Nếu có thắc mắc, vui lòng liên hệ Nguyễn Đức Nguyên: <a href='mailto:nhahangvietkitchensaigon@gmail.com'>nhahangvietkitchensaigon@gmail.com</a></p>");
            htmlContent.Append("</td></tr></table></body></html>");

            mail.Body = htmlContent.ToString();
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
            smtp.EnableSsl = true;
            smtp.Send(mail);
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