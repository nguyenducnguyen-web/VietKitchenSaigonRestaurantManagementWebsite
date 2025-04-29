using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.Admin
{
    public partial class TrangThongTinTaiKhoan : System.Web.UI.Page
    {
        SqlConnection con; SqlCommand cmd; SqlDataAdapter sda; DataTable dt;
    protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Thông tin tài khoản";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    LoadProfile();
                }
            }
            lblMsg.Visible = false;
        }

        private void LoadProfile()
        {
            string username = Session["admin"].ToString();
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("TaiKhoanNhanVien", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT_PROFILE");
            cmd.Parameters.AddWithValue("@TenDangNhapNhanVien", username);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtName.Text = dt.Rows[0]["TenNhanVien"].ToString();
                txtUsername.Text = dt.Rows[0]["TenDangNhapNhanVien"].ToString();
                txtPhone.Text = dt.Rows[0]["SdtNhanVien"].ToString();
                txtEmail.Text = dt.Rows[0]["EmailNhanVien"].ToString();
                txtAddress.Text = dt.Rows[0]["DiaChiNhanVien"].ToString();
                txtRole.Text = dt.Rows[0]["Quyen"].ToString();
                txtStatus.Text = Convert.ToInt32(dt.Rows[0]["TrangThai"]) == 1 ? "Hoạt động" : "Không hoạt động";
                txtCreatedDate.Text = Convert.ToDateTime(dt.Rows[0]["NgayTaoNhanVien"]).ToString("dd/MM/yyyy");
            }
            else
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Không tìm thấy thông tin tài khoản!";
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Không tìm thấy thông tin tài khoản!');", true);
            }
        }

        private bool CheckDuplicateEmail(string email, string username)
        {
            bool isDuplicate = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE EmailNhanVien = @Email AND TenDangNhapNhanVien != @Username", con);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Username", username);

            try
            {
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    isDuplicate = true;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi khi kiểm tra email trùng lặp: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi khi kiểm tra email trùng lặp: {ex.Message}');", true);
            }
            finally
            {
                con.Close();
            }
            return isDuplicate;
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            string username = Session["admin"].ToString();

            // Check for duplicate email
            if (CheckDuplicateEmail(txtEmail.Text.Trim(), username))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Email đã được sử dụng bởi tài khoản khác!";
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Email đã được sử dụng bởi tài khoản khác!');", true);
                return;
            }

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NhanVien_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "UPDATE_PROFILE");
            cmd.Parameters.AddWithValue("@EmployeeId", username);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                lblMsg.Visible = true;
                lblMsg.Text = "Cập nhật thông tin thành công!";
                lblMsg.CssClass = "alert alert-success";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Cập nhật thông tin thành công!');", true);
                LoadProfile();
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

        protected void btnClearProfile_Click(object sender, EventArgs e)
        {
            LoadProfile();
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("CapNhatMatKhau.aspx");
        }
    }

}