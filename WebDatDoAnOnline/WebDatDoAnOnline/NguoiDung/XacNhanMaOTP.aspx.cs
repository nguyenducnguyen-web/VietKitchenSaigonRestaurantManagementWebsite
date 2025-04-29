using System;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class XacNhanMaOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ResetEmail"] == null || Session["OTP"] == null)
            {
                ShowErrorAlert("Phiên làm việc không hợp lệ. Vui lòng thử lại!");
                Response.Redirect("KhoiPhucMatKhau.aspx");
            }
        }

        protected void btnVerifyOtp_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Kiểm tra thời hạn OTP
                if (Session["OTPTime"] != null)
                {
                    DateTime otpTime = (DateTime)Session["OTPTime"];
                    if ((DateTime.Now - otpTime).TotalMinutes > 10)
                    {
                        ShowErrorAlert("Mã OTP đã hết hạn! Vui lòng yêu cầu mã mới.");
                        Session["ResetEmail"] = null;
                        Session["OTP"] = null;
                        Session["OTPTime"] = null;
                        Response.Redirect("KhoiPhucMatKhau.aspx");
                        return;
                    }
                }

                string inputOtp = txtOtp.Text.Trim();
                if (inputOtp == Session["OTP"]?.ToString())
                {
                    ShowSuccessAlert("Xác nhận mã OTP thành công!");
                    Response.Redirect("DoiMatKhau.aspx");
                }
                else
                {
                    ShowErrorAlert("Mã OTP không chính xác! Vui lòng nhập lại.");
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

