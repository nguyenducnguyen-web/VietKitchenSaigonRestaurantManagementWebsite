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
    public partial class QuanLyDatBan : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Quản lý đặt bàn";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getBookings();
                    LoadBookingStatistics();
                }
            }
            lblMsg.Visible = false;
        }

        private void LoadBookingStatistics()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) as TotalBookings, " +
                                 "COUNT(CASE WHEN CAST(NgayDatBan AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) as TodayBookings, " +
                                 "AVG(CAST(SoLuongKhach AS FLOAT)) as AvgGuests " +
                                 "FROM DatBan", con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                todayBookingsCount.InnerText = dt.Rows[0]["TodayBookings"].ToString();
                totalBookingsCount.InnerText = dt.Rows[0]["TotalBookings"].ToString();
                averageGuestsCount.InnerText = String.Format("{0:F1}", dt.Rows[0]["AvgGuests"]);
            }
            else
            {
                todayBookingsCount.InnerText = "0";
                totalBookingsCount.InnerText = "0";
                averageGuestsCount.InnerText = "0";
            }
        }

        private void getBookings()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("DatBanSp", con);

            cmd.Parameters.AddWithValue("@Action", "SELECT");

            if (!string.IsNullOrEmpty(txtSearchName.Text))
            {
                cmd.Parameters.AddWithValue("@TenKhachHang", txtSearchName.Text.Trim());
            }
            if (!string.IsNullOrEmpty(txtSearchPhone.Text))
            {
                cmd.Parameters.AddWithValue("@SdtDatBan", txtSearchPhone.Text.Trim());
            }
            if (!string.IsNullOrEmpty(txtSearchEmail.Text))
            {
                cmd.Parameters.AddWithValue("@EmailDatBan", txtSearchEmail.Text.Trim());
            }

            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            rBookings.DataSource = dt;
            rBookings.DataBind();
        }

        protected void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            getBookings();
        }

        protected void txtSearchPhone_TextChanged(object sender, EventArgs e)
        {
            getBookings();
        }

        protected void txtSearchEmail_TextChanged(object sender, EventArgs e)
        {
            getBookings();
        }

        protected void rBookings_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                cmd = new SqlCommand("DatBanSp", con);

                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaDatBan", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                        "showSuccessAlert('Xóa thông tin đặt bàn thành công!');", true);
                    getBookings();
                    LoadBookingStatistics();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Lỗi: " + ex.Message.Replace("'", "\\'") + "');", true);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        [WebMethod]
        public static string GetBookingDetails(string bookingId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("DatBanSp", con);
                    cmd.Parameters.AddWithValue("@Action", "SELECTBYID");
                    cmd.Parameters.AddWithValue("@MaDatBan", bookingId);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    StringBuilder html = new StringBuilder();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        html.Append("<p><strong>Mã đặt bàn:</strong> ").Append(row["MaDatBan"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Tên khách hàng:</strong> ").Append(row["TenKhachHang"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Số điện thoại:</strong> ").Append(row["SdtDatBan"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Email:</strong> ").Append(row["EmailDatBan"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Số lượng khách:</strong> ").Append(row["SoLuongKhach"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Ngày đặt bàn:</strong> ").Append(row["NgayDatBan"] ?? "N/A").Append("</p>");
                    }
                    else
                    {
                        html.Append("<p>Không tìm thấy thông tin đặt bàn.</p>");
                    }
                    return html.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi trong GetBookingDetails: " + ex.Message);
                return "";
            }
        }
    }
}