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
    public partial class QuanLyTrangThaiDonHang : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Order Status";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getOrderStatus();
                    LoadOrderStatistics();
                }
            }
            lblMsg.Visible = false;
            pUpdateOrderStatus.Visible = false;
        }

        private void LoadOrderStatistics()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT TrangThai, COUNT(*) as Count FROM Orders GROUP BY TrangThai", con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            int pending = 0, shipping = 0, delivered = 0;
            foreach (DataRow row in dt.Rows)
            {
                string status = row["TrangThai"].ToString().Trim();
                int count = Convert.ToInt32(row["Count"]);
                if (status == "Đang xử lý") pending = count;
                else if (status == "Đang trên đường vận chuyển") shipping = count;
                else if (status == "Đã giao hàng") delivered = count;
            }

            pendingOrdersCount.InnerText = pending.ToString();
            shippingOrdersCount.InnerText = shipping.ToString();
            deliveredOrdersCount.InnerText = delivered.ToString();
        }

        private void getOrderStatus()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Invoice", con);
            cmd.Parameters.AddWithValue("@Action", "GETSTATUS");

            if (ddlFilterStatus.SelectedValue != "0")
            {
                cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue);
            }

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                cmd.Parameters.AddWithValue("@Search", txtSearch.Text.Trim());
            }

            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            rOrderStatus.DataSource = dt;
            rOrderStatus.DataBind();
        }

        protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            getOrderStatus();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getOrderStatus();
        }

        protected void rOrderStatus_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;

            if (e.CommandName == "edit")
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                cmd = new SqlCommand("Invoice", con);

                cmd.Parameters.AddWithValue("@Action", "STATUSBYID");
                cmd.Parameters.AddWithValue("@OrderDetailsId", e.CommandArgument);

                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);

                dt = new DataTable();
                sda.Fill(dt);

                string trangThai = dt.Rows[0]["TrangThai"].ToString().Trim();

                if (ddlOrderStatus.Items.FindByValue(trangThai) != null)
                {
                    ddlOrderStatus.SelectedValue = trangThai;
                }
                else
                {
                    ddlOrderStatus.Items.Add(new ListItem(trangThai, trangThai));
                    ddlOrderStatus.SelectedValue = trangThai;
                }

                hdnId.Value = dt.Rows[0]["MaOrders"].ToString();
                pUpdateOrderStatus.Visible = true;

                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-warning";
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            int orderDetailsId = Convert.ToInt32(hdnId.Value);
            string selectedStatus = ddlOrderStatus.SelectedValue;

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Invoice", con);

            cmd.Parameters.AddWithValue("@Action", "UPDTSTATUS");
            cmd.Parameters.AddWithValue("@OrderDetailsId", orderDetailsId);
            cmd.Parameters.AddWithValue("@Status", selectedStatus);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.CommandText = "Invoice";
                cmd.Parameters.AddWithValue("@Action", "STATUSBYID");
                cmd.Parameters.AddWithValue("@OrderDetailsId", orderDetailsId);

                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                        "showSuccessAlert('Cập nhật trạng thái đơn hàng thành công!');", true);
                    getOrderStatus();
                    LoadOrderStatistics();
                    pUpdateOrderStatus.Visible = false;
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Không tìm thấy đơn hàng để cập nhật!";
                    lblMsg.CssClass = "alert alert-warning";
                }
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pUpdateOrderStatus.Visible = false;
        }

        [WebMethod]
        public static string GetOrderDetails(string orderId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("Invoice", con);
                    cmd.Parameters.AddWithValue("@Action", "STATUSBYID");
                    cmd.Parameters.AddWithValue("@OrderDetailsId", orderId);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    StringBuilder html = new StringBuilder();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        html.Append("<p><strong>Mã đơn hàng:</strong> ").Append(row["OrdersNo"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Ngày đặt hàng:</strong> ").Append(row["NgayTaoOrders"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Trạng thái:</strong> ").Append(row["TrangThai"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Tên món ăn:</strong> ").Append(row["TenMonAn"] ?? "N/A").Append("</p>");
                        html.Append("<p><strong>Số tiền:</strong> ").Append(row["TotalPrice"] != DBNull.Value ? String.Format("{0:N0} VND", Convert.ToDecimal(row["TotalPrice"])) : "0 VND").Append("</p>");
                        html.Append("<p><strong>Phương thức thanh toán:</strong> ").Append(row["PhuongThucThanhToan"] ?? "N/A").Append("</p>");
                    }
                    else
                    {
                        html.Append("<p>Không tìm thấy thông tin đơn hàng.</p>");
                    }
                    return html.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi trong GetOrderDetails: " + ex.Message);
                return "";
            }
        }
    }
}
