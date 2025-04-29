using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DangKyTaiKhoan : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    getUserDetails();
                    if (Request.QueryString["mode"] == "password")
                    {
                        pnlPassword.Visible = true; // Hiển thị panel mật khẩu
                        lblHeaderMsg.Text = "<h2>ĐỔI MẬT KHẨU</h2>";
                        btnRegister.Text = "Cập nhật mật khẩu";
                        lblAlreadyUser.Text = "";
                    }
                }
                else if (Session["userId"] != null)
                {
                    Response.Redirect("TrangChu.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;

            int userId = Convert.ToInt32(Request.QueryString["id"] ?? "0");
            con = new SqlConnection(ConnectionSQL.GetConnectionString());

            if (userId == 0) // Đăng ký mới
            {
                cmd = new SqlCommand("ThemNguoiDung", con);
                cmd.Parameters.AddWithValue("@TenNguoiDung", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@TenDangNhapNguoiDung", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@SdtNguoiDung", txtDt.Text.Trim());
                cmd.Parameters.AddWithValue("@EmailNguoiDung", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@DiaChiNguoiDung", txtDiaChi.Text.Trim());
                cmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim());
                cmd.Parameters.AddWithValue("@MatKhauNguoiDung", txtPassword.Text.Trim());
                cmd.Parameters.AddWithValue("@NgayTaoNguoiDung", DateTime.Now);
                actionName = "Đã tạo tài khoản thành công!";
            }
            else // Cập nhật thông tin
            {
                cmd = new SqlCommand("User_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@Mobile", txtDt.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Address", txtDiaChi.Text.Trim());
                cmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim());

                // Nếu ở chế độ cập nhật mật khẩu
                if (Request.QueryString["mode"] == "password")
                {
                    if (txtNewPassword.Text.Trim() == txtConfirmPassword.Text.Trim())
                    {
                        cmd.Parameters.AddWithValue("@Password", txtNewPassword.Text.Trim());
                        actionName = "Đổi mật khẩu thành công!";
                    }
                    else
                    {
                        ShowSweetAlert("Lỗi", "Mật khẩu xác nhận không khớp!", "error");
                        return;
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Password", DBNull.Value); // Không cập nhật mật khẩu nếu không ở chế độ đổi mật khẩu
                    actionName = "Thông tin người dùng đã được cập nhật thành công!";
                }
            }

            if (fuUserImage.HasFile)
            {
                if (Utils.IsValidExtension(fuUserImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExtension = Path.GetExtension(fuUserImage.FileName);
                    imagePath = "Images/User/" + obj.ToString() + fileExtension;
                    fuUserImage.PostedFile.SaveAs(Server.MapPath("~/Images/User/") + obj.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue(userId == 0 ? "@DiaChiAnhAvatar" : "@ImageUrl", imagePath);
                    isValidToExecute = true;
                }
                else
                {
                    ShowSweetAlert("Lỗi", "Vui lòng chọn ảnh định dạng .jpg, .jpeg hoặc .png!", "error");
                    return;
                }
            }
            else
            {
                cmd.Parameters.AddWithValue(userId == 0 ? "@DiaChiAnhAvatar" : "@ImageUrl", DBNull.Value);
                isValidToExecute = true;
            }

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    ShowSweetAlert("Thành công", actionName, "success");

                    string redirectScript = userId == 0
                        ? "setTimeout(function() { window.location.href = 'DangNhap.aspx'; }, 2000);"
                        : "setTimeout(function() { window.location.href = 'TrangThongTinNguoiDung.aspx?updated=true'; }, 2000);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirectScript", redirectScript, true);

                    clear();
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                    {
                        ShowSweetAlert("Lỗi", "Username hoặc email đã tồn tại, vui lòng chọn tên khác!", "error");
                    }
                    else
                    {
                        ShowSweetAlert("Lỗi", $"Lỗi cơ sở dữ liệu: {ex.Message}", "error");
                    }
                }
                catch (Exception ex)
                {
                    ShowSweetAlert("Lỗi", $"Lỗi hệ thống: {ex.Message}", "error");
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void ShowSweetAlert(string title, string text, string type)
        {
            string script = $@"
                Swal.fire({{
                    title: '{title}',
                    html: '{text}',
                    icon: '{type}',
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        {(type == "success" ? "document.getElementById('successSound').play(); confetti();" : "document.getElementById('errorSound').play();")}
                    }}
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
        }

        void getUserDetails()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("User_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT4PROFILE");
            cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                txtName.Text = dt.Rows[0]["TenNguoiDung"].ToString();
                txtUsername.Text = dt.Rows[0]["TenDangNhapNguoiDung"].ToString();
                txtDt.Text = dt.Rows[0]["SdtNguoiDung"].ToString();
                txtEmail.Text = dt.Rows[0]["EmailNguoiDung"].ToString();
                txtDiaChi.Text = dt.Rows[0]["DiaChiNguoiDung"].ToString();
                txtPostCode.Text = dt.Rows[0]["PostCode"].ToString();
                imgUser.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["DiaChiAnhAvatar"].ToString())
                    ? "../Images/No_image.png"
                    : "../" + dt.Rows[0]["DiaChiAnhAvatar"].ToString();
                imgUser.Height = 200;
                imgUser.Width = 200;
                txtPassword.Visible = false; // Ẩn txtPassword trong chế độ chỉnh sửa
            }
            lblHeaderMsg.Text = "<h2>CHỈNH SỬA THÔNG TIN NGƯỜI DÙNG</h2>";
            btnRegister.Text = "Cập nhật";
            lblAlreadyUser.Text = "";
        }

        private void clear()
        {
            txtName.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtDt.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtDiaChi.Text = string.Empty;
            txtPostCode.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
        }
    }
}