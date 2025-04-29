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
    public partial class QuanLyLienHeNguoiDung : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Contact Users";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getContacts();
                    LoadContactStatistics();
                }
            }
            lblMsg.Visible = false;
        }

        private void LoadContactStatistics()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("ContactSp", con);
            cmd.Parameters.AddWithValue("@Action", "STATS");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            int total = 0, today = 0, thisWeek = 0;
            if (dt.Rows.Count > 0)
            {
                total = Convert.ToInt32(dt.Rows[0]["TotalContacts"]);
                today = Convert.ToInt32(dt.Rows[0]["TodayContacts"]);
                thisWeek = Convert.ToInt32(dt.Rows[0]["ThisWeekContacts"]);
            }

            totalContactsCount.InnerText = total.ToString();
            todayContactsCount.InnerText = today.ToString();
            thisWeekContactsCount.InnerText = thisWeek.ToString();
        }

        private void getContacts()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("ContactSp", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                cmd.Parameters.AddWithValue("@Search", txtSearch.Text.Trim());
            }

            if (ddlSort.SelectedValue != "0")
            {
                cmd.Parameters.AddWithValue("@Sort", ddlSort.SelectedValue);
            }

            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            rContacts.DataSource = dt;
            rContacts.DataBind();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getContacts();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            getContacts();
        }

        protected void rContacts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                cmd = new SqlCommand("ContactSp", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ContactId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                        "showSuccessAlert('Xoá thông tin liên hệ thành công!');", true);
                    getContacts();
                    LoadContactStatistics();
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
        public static string GetContactDetails(string contactId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("ContactSp", con);
                    cmd.Parameters.AddWithValue("@Action", "GETBYID");
                    cmd.Parameters.AddWithValue("@ContactId", contactId);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    StringBuilder html = new StringBuilder();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        html.Append("<p><strong>Mã liên hệ:</strong> ").Append(row["MaLienHe"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Tên khách hàng:</strong> ").Append(row["TenLienHe"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Email:</strong> ").Append(row["EmailLienHe"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Chủ đề:</strong> ").Append(row["ChuDe"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Thông tin liên hệ:</strong> ").Append(row["TinNhan"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Ngày tạo:</strong> ").Append(row["NgayTaoLienHe"] != DBNull.Value ? Convert.ToDateTime(row["NgayTaoLienHe"]).ToString("dd/MM/yyyy HH:mm") : "N/A").Append("</p>");
                    }
                    else
                    {
                        html.Append("<p>Không tìm thấy thông tin liên hệ.</p>");
                    }
                    return html.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi trong GetContactDetails: " + ex.Message);
                return "";
            }
        }
    }
}