using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Text;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyTaiKhoanNguoiDung : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Users";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getUsers();
                    LoadUserStatistics();
                }
            }
            lblMsg.Visible = false;
        }

        private void LoadUserStatistics()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand(@"
                SELECT 
                    (SELECT COUNT(*) FROM NguoiDung) AS TotalUsers,
                    (SELECT COUNT(*) FROM NguoiDung WHERE MONTH(NgayTaoNguoiDung) = MONTH(GETDATE()) AND YEAR(NgayTaoNguoiDung) = YEAR(GETDATE())) AS NewUsers,
                    (SELECT COUNT(*) FROM NguoiDung WHERE MatKhauNguoiDung IS NULL) AS LockedUsers", con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                totalUsersCount.InnerText = dt.Rows[0]["TotalUsers"].ToString();
                newUsersCount.InnerText = dt.Rows[0]["NewUsers"].ToString();
                lockedUsersCount.InnerText = dt.Rows[0]["LockedUsers"].ToString();
            }
        }

        private void getUsers()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("User_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT4ADMIN");

            // Tìm kiếm theo tên người dùng
            if (!string.IsNullOrEmpty(txtSearchName.Text))
            {
                cmd.Parameters.AddWithValue("@Name", txtSearchName.Text.Trim());
            }

            // Tìm kiếm theo số điện thoại
            if (!string.IsNullOrEmpty(txtSearchMobile.Text))
            {
                cmd.Parameters.AddWithValue("@Mobile", txtSearchMobile.Text.Trim());
            }

            // Sắp xếp
            string sortColumn = hdnSortColumn.Value;
            string sortOrder = hdnSortOrder.Value;
            cmd.Parameters.AddWithValue("@SortColumn", sortColumn);
            cmd.Parameters.AddWithValue("@SortOrder", sortOrder);

            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            rUsers.DataSource = dt;
            rUsers.DataBind();
        }

        protected void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            getUsers();
        }

        protected void txtSearchMobile_TextChanged(object sender, EventArgs e)
        {
            getUsers();
        }

        protected void rUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                cmd = new SqlCommand("User_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@UserId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                        "showSuccessAlert('Xoá tài khoản người dùng thành công!');", true);
                    getUsers();
                    LoadUserStatistics();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Error - " + ex.Message.Replace("'", "\\'") + "');", true);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        [WebMethod]
        public static string GetUserDetails(string userId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("User_Crud", con);
                    cmd.Parameters.AddWithValue("@Action", "SELECT4PROFILE");
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    StringBuilder html = new StringBuilder();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        html.Append("<p><strong>Mã người dùng:</strong> ").Append(row["MaNguoiDung"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Họ tên:</strong> ").Append(row["TenNguoiDung"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Username:</strong> ").Append(row["TenDangNhapNguoiDung"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Email:</strong> ").Append(row["EmailNguoiDung"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Số điện thoại:</strong> ").Append(row["SdtNguoiDung"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Địa chỉ:</strong> ").Append(row["DiaChiNguoiDung"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Ngày tham gia:</strong> ").Append(row["NgayTaoNguoiDung"] ?? "N/A").Append("</p>");
                    }
                    else
                    {
                        html.Append("<p>Không tìm thấy thông tin người dùng.</p>");
                    }
                    return html.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi trong GetUserDetails: " + ex.Message);
                return "";
            }
        }
    }
}