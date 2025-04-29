using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyNhanVien : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Nhân viên";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getEmployees();
                    UpdateStats();
                    pnlPassword.Visible = true; // Default: show password field for adding
                }
            }
            lblMsg.Visible = false;
        }

        private bool CheckDuplicate(string username, string email, string currentUsername = "")
        {
            bool isDuplicate = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE (TenDangNhapNhanVien = @Username OR EmailNhanVien = @Email) AND TenDangNhapNhanVien != @CurrentUsername", con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@CurrentUsername", currentUsername);

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
                lblMsg.Text = "Lỗi khi kiểm tra trùng lặp: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi khi kiểm tra trùng lặp: {ex.Message}');", true);
            }
            finally
            {
                con.Close();
            }
            return isDuplicate;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty;
            bool isValidToExecute = true;
            string employeeId = hdnId.Value != "" ? hdnId.Value : txtUsername.Text.Trim();

            // Check for duplicates
            if (CheckDuplicate(txtUsername.Text.Trim(), txtEmail.Text.Trim(), hdnId.Value))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Tên đăng nhập hoặc email đã tồn tại!";
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Tên đăng nhập hoặc email đã tồn tại!');", true);
                return;
            }

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NhanVien_Crud", con);

            cmd.Parameters.AddWithValue("@Action", string.IsNullOrEmpty(hdnId.Value) ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@Role", ddlRoles.SelectedValue);
            cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked ? 1 : 0);

            // Handle password
            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Password", hashedBytes);
                }
            }
            else if (string.IsNullOrEmpty(hdnId.Value))
            {
                // Password is required only for INSERT
                lblMsg.Visible = true;
                lblMsg.Text = "Mật khẩu là bắt buộc khi thêm nhân viên mới!";
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Mật khẩu là bắt buộc khi thêm nhân viên mới!');", true);
                isValidToExecute = false;
            }
            // For UPDATE, if password is empty, @Password will be NULL, and stored procedure keeps existing password

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = string.IsNullOrEmpty(hdnId.Value) ? "thêm" : "cập nhật";
                    lblMsg.Visible = true;
                    lblMsg.Text = $"Nhân viên được {actionName} thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", $"showSuccessAlert('Nhân viên được {actionName} thành công!');", true);
                    getEmployees();
                    UpdateStats();
                    clear();
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
        }

        private void getEmployees()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NhanVien_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.Parameters.AddWithValue("@SearchText", txtSearch.Text.Trim());
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            // Áp dụng sắp xếp
            string sortOption = ddlSort.SelectedValue;
            if (!string.IsNullOrEmpty(sortOption))
            {
                switch (sortOption)
                {
                    case "name_asc":
                        dt = dt.AsEnumerable().OrderBy(row => row.Field<string>("TenNhanVien")).CopyToDataTable();
                        break;
                    case "name_desc":
                        dt = dt.AsEnumerable().OrderByDescending(row => row.Field<string>("TenNhanVien")).CopyToDataTable();
                        break;
                    case "newest":
                        dt = dt.AsEnumerable().OrderByDescending(row => row.Field<DateTime>("NgayTaoNhanVien")).CopyToDataTable();
                        break;
                    case "oldest":
                        dt = dt.AsEnumerable().OrderBy(row => row.Field<DateTime>("NgayTaoNhanVien")).CopyToDataTable();
                        break;
                }
            }

            rEmployee.DataSource = dt;
            rEmployee.DataBind();
        }

        private void UpdateStats()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) AS Total, SUM(CASE WHEN TrangThai = 1 THEN 1 ELSE 0 END) AS Active, SUM(CASE WHEN Quyen = 'Nhân viên bán hàng' THEN 1 ELSE 0 END) AS Sales FROM NhanVien", con);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalEmployees.InnerText = reader["Total"].ToString();
                        activeEmployees.InnerText = reader["Active"].ToString();
                        salesEmployees.InnerText = reader["Sales"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi khi cập nhật thống kê: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
            finally
            {
                con.Close();
            }
        }

        private void clear()
        {
            txtName.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPassword.Text = string.Empty;
            ddlRoles.ClearSelection();
            cbIsActive.Checked = false;
            hdnId.Value = "";
            btnAddOrUpdate.Text = "Thêm";
            txtSearch.Text = string.Empty;
            ddlSort.ClearSelection();
            pnlPassword.Visible = true; // Reset to show password field for adding
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            getEmployees();
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            clear();
            getEmployees();
        }

        protected void rEmployee_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());

            if (e.CommandName == "edit")
            {
                cmd = new SqlCommand("NhanVien_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@EmployeeId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                txtName.Text = dt.Rows[0]["TenNhanVien"].ToString();
                txtUsername.Text = dt.Rows[0]["TenDangNhapNhanVien"].ToString();
                txtPhone.Text = dt.Rows[0]["SdtNhanVien"].ToString();
                txtEmail.Text = dt.Rows[0]["EmailNhanVien"].ToString();
                txtAddress.Text = dt.Rows[0]["DiaChiNhanVien"].ToString();
                ddlRoles.SelectedValue = dt.Rows[0]["Quyen"].ToString();
                cbIsActive.Checked = Convert.ToInt32(dt.Rows[0]["TrangThai"]) == 1;
                hdnId.Value = dt.Rows[0]["TenDangNhapNhanVien"].ToString();
                txtPassword.Text = string.Empty; // Clear password field for update
                btnAddOrUpdate.Text = "Cập nhật";
                pnlPassword.Visible = false; // Hide password field for update
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-primary";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new SqlCommand("NhanVien_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@EmployeeId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Nhân viên đã được xóa thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Nhân viên đã được xóa thành công!');", true);
                    getEmployees();
                    UpdateStats();
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
        }

        protected void rEmployee_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblIsActive = e.Item.FindControl("lblIsActive") as Label;

                if (Convert.ToInt32(lblIsActive.Text) == 1)
                {
                    lblIsActive.Text = "Hoạt động";
                    lblIsActive.CssClass = "badge badge-success";
                }
                else
                {
                    lblIsActive.Text = "Không hoạt động";
                    lblIsActive.CssClass = "badge badge-danger";
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getEmployees();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            getEmployees();
        }
    }
}