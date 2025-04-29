using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace WebDatDoAnOnline.Admin
{
    public partial class CapNhatMatKhau : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Cập nhật mật khẩu";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
            }
            lblMsg.Visible = false;
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            string username = Session["admin"].ToString();

            // Verify current password
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT MatKhauNhanVien FROM NhanVien WHERE TenDangNhapNhanVien = @Username", con);
            cmd.Parameters.AddWithValue("@Username", username);

            try
            {
                con.Open();
                byte[] storedPasswordHash = (byte[])cmd.ExecuteScalar();
                byte[] inputPasswordHash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    inputPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(txtCurrentPassword.Text.Trim()));
                }

                if (!storedPasswordHash.SequenceEqual(inputPasswordHash))
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Mật khẩu hiện tại không đúng!";
                    lblMsg.CssClass = "alert alert-danger";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Mật khẩu hiện tại không đúng!');", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi khi kiểm tra mật khẩu: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi khi kiểm tra mật khẩu: {ex.Message}');", true);
                return;
            }
            finally
            {
                con.Close();
            }

            // Update password
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NhanVien_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "UPDATE_PASSWORD");
            cmd.Parameters.AddWithValue("@EmployeeId", username);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] newPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(txtNewPassword.Text.Trim()));
                cmd.Parameters.AddWithValue("@Password", newPasswordHash);
            }
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                lblMsg.Visible = true;
                lblMsg.Text = "Đổi mật khẩu thành công!";
                lblMsg.CssClass = "alert alert-success";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Đổi mật khẩu thành công!');", true);
                ClearPasswordFields();
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi: {ex.Message}');", true);
            }
            finally
            {
                con.Close();
            }
        }

        protected void btnClearPassword_Click(object sender, EventArgs e)
        {
            ClearPasswordFields();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("TrangThongTinTaiKhoan.aspx");
        }

        private void ClearPasswordFields()
        {
            txtCurrentPassword.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
        }
    }
}