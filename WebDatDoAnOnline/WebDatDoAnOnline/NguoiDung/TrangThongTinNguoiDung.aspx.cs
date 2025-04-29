using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class TrangThongTinNguoiDung : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["MaNguoiDung"] == null)
                {
                    Response.Redirect("DangNhap.aspx");
                }
                else
                {
                    getUserDetails();
                    getPurchaseHistory();

                    // Kiểm tra nếu vừa cập nhật xong thì redirect về trang chủ
                    if (Request.QueryString["updated"] == "true")
                    {
                        Response.Redirect("TrangChu.aspx"); // Redirect về trang chủ ngay lập tức
                    }
                }
            }
        }

        void getUserDetails()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("User_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT4PROFILE");
            cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rUserProfile.DataSource = dt;
            rUserProfile.DataBind();
            if (dt.Rows.Count == 1)
            {
                Session["TenNguoiDung"] = dt.Rows[0]["TenNguoiDung"].ToString();
                Session["EmailNguoiDung"] = dt.Rows[0]["EmailNguoiDung"].ToString();
                Session["DiaChiAnhAvatar"] = dt.Rows[0]["DiaChiAnhAvatar"].ToString();
                Session["NgayTaoNguoiDung"] = dt.Rows[0]["NgayTaoNguoiDung"].ToString();
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect($"DangKyTaiKhoan.aspx?id={Session["MaNguoiDung"]}&mode=password");
        }

        void getPurchaseHistory()
        {
            int sr = 1;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Invoice", con);
            cmd.Parameters.AddWithValue("@Action", "ODRHISTORY");
            cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            if (!dt.Columns.Contains("MaThanhToan"))
            {
                return;
            }

            dt.Columns.Add("SrNo", typeof(Int32));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    dataRow["SrNo"] = sr;
                    sr++;
                }
            }

            if (dt.Rows.Count == 0)
            {
                rPurchaseHistory.FooterTemplate = null;
                rPurchaseHistory.FooterTemplate = new CustomTemplate(ListItemType.Footer);
            }
            rPurchaseHistory.DataSource = dt;
            rPurchaseHistory.DataBind();
        }

        protected void rPurchaseHistory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                double grandTotal = 0;
                HiddenField paymentId = e.Item.FindControl("hdnPaymentId") as HiddenField;
                Repeater repOrders = e.Item.FindControl("rOrders") as Repeater;

                if (paymentId != null && !string.IsNullOrEmpty(paymentId.Value))
                {
                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    cmd = new SqlCommand("Invoice", con);
                    cmd.Parameters.AddWithValue("@Action", "INVOICBYID");
                    cmd.Parameters.AddWithValue("@PaymentId", Convert.ToInt32(paymentId.Value));
                    cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
                    cmd.CommandType = CommandType.StoredProcedure;
                    sda = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dt.Rows)
                        {
                            // Kiểm tra giá trị TotalPrice trước khi ép kiểu
                            object totalPriceObj = dataRow["TotalPrice"];
                            double totalPrice = totalPriceObj != DBNull.Value ? Convert.ToDouble(totalPriceObj) : 0;
                            grandTotal += totalPrice;
                        }
                    }

                    DataRow dr = dt.NewRow();
                    dr["TotalPrice"] = grandTotal;
                    dt.Rows.Add(dr);
                    repOrders.DataSource = dt;
                    repOrders.DataBind();
                }
                else
                {
                    repOrders.DataSource = null;
                    repOrders.DataBind();
                }
            }
        }

        private sealed class CustomTemplate : ITemplate
        {
            private ListItemType ListItemType { get; set; }

            public CustomTemplate(ListItemType type)
            {
                ListItemType = type;
            }

            public void InstantiateIn(Control container)
            {
                if (ListItemType == ListItemType.Footer)
                {
                    var footer = new LiteralControl("<tr><td colspan='5'><b>Đặt đồ ăn gì đi người đẹp!!!! Người đẹp chưa order lần nào hết ó!!!" +
                        ".</b><a href='DanhSachMonAn.aspx' class='badge badge-info ml-2'>Click vào đây để đặt hàng nào!!!</a></td></tr></tbody></table>");
                    container.Controls.Add(footer);
                }
            }
        }
    }
}