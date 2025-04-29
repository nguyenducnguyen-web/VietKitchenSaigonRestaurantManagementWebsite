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
    public partial class GioHang : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        decimal grandTotal = 0;

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
                    getCartItems();
                }
            }
        }

        void getCartItems()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Cart_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            rCartItem.DataSource = dt;
            if (dt.Rows.Count == 0)
            {
                rCartItem.FooterTemplate = null;
                rCartItem.FooterTemplate = new CustomTemplate(ListItemType.Footer);
            }
            rCartItem.DataBind();
        }

        protected void rCartItem_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Utils utils = new Utils();
            if (e.CommandName == "remove")
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                cmd = new SqlCommand("Cart_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    getCartItems();
                    Session["cartCount"] = utils.cartCount(Convert.ToInt32(Session["MaNguoiDung"]));
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", "showSuccessAlert('Đã xóa món ăn khỏi giỏ hàng!');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"showErrorAlert('Lỗi: {ex.Message}');", true);
                }
                finally
                {
                    con.Close();
                }
            }
            if (e.CommandName == "updateCart")
            {
                bool isCartUpdated = false;
                for (int item = 0; item < rCartItem.Items.Count; item++)
                {
                    if (rCartItem.Items[item].ItemType == ListItemType.Item || rCartItem.Items[item].ItemType == ListItemType.AlternatingItem)
                    {
                        TextBox quantity = rCartItem.Items[item].FindControl("txtQuantity") as TextBox;
                        HiddenField _productId = rCartItem.Items[item].FindControl("hdnProductId") as HiddenField;
                        HiddenField _quantity = rCartItem.Items[item].FindControl("hdnQuantity") as HiddenField;
                        HiddenField _productQuantity = rCartItem.Items[item].FindControl("hdnPrdQuantity") as HiddenField;

                        if (!int.TryParse(quantity.Text, out int quantityFromCart) || quantityFromCart < 1)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "Error", "showErrorAlert('Số lượng phải là số nguyên dương!');", true);
                            return;
                        }

                        int productId = Convert.ToInt32(_productId.Value);
                        int quantityFromDB = Convert.ToInt32(_quantity.Value);
                        int productQuantity = Convert.ToInt32(_productQuantity.Value);

                        if (quantityFromCart > productQuantity)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"showErrorAlert('Số lượng vượt quá tồn kho cho món ăn (Tối đa: {productQuantity})!');", true);
                            return;
                        }

                        if (quantityFromCart != quantityFromDB)
                        {
                            isCartUpdated = utils.updateCartQuantity(quantityFromCart, productId, Convert.ToInt32(Session["MaNguoiDung"])) || isCartUpdated;
                        }
                    }
                }
                getCartItems();
                if (isCartUpdated)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", "showSuccessAlert('Cập nhật giỏ hàng thành công!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Info", "showSuccessAlert('Không có thay đổi nào được áp dụng.');", true);
                }
            }
            if (e.CommandName == "checkout")
            {
                bool isTrue = false;
                string pName = string.Empty;

                for (int item = 0; item < rCartItem.Items.Count; item++)
                {
                    if (rCartItem.Items[item].ItemType == ListItemType.Item || rCartItem.Items[item].ItemType == ListItemType.AlternatingItem)
                    {
                        HiddenField _productId = rCartItem.Items[item].FindControl("hdnProductId") as HiddenField;
                        HiddenField _cartQuantity = rCartItem.Items[item].FindControl("hdnQuantity") as HiddenField;
                        HiddenField _productQuantity = rCartItem.Items[item].FindControl("hdnPrdQuantity") as HiddenField;
                        Label productName = rCartItem.Items[item].FindControl("lblName") as Label;

                        int productId = Convert.ToInt32(_productId.Value);
                        int cartQuantity = Convert.ToInt32(_cartQuantity.Value);
                        int productQuantity = Convert.ToInt32(_productQuantity.Value);

                        if (productQuantity >= cartQuantity && productQuantity > 0)
                        {
                            isTrue = true;
                        }
                        else
                        {
                            isTrue = false;
                            pName = productName.Text.ToString();
                            break;
                        }
                    }
                }

                if (isTrue)
                {
                    Response.Redirect("ThanhToan.aspx");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"showErrorAlert('Món ăn \\'{pName}\\' đã hết hàng hoặc số lượng không hợp lệ!');", true);
                }
            }
        }

        protected void rCartItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label totalPrice = e.Item.FindControl("lblTotalPrice") as Label;
                Label productPrice = e.Item.FindControl("lblPrice") as Label;
                TextBox quantity = e.Item.FindControl("txtQuantity") as TextBox;

                // Kiểm tra giá trị GiaMonAnSauKhuyenMai trước khi ép kiểu
                object discountedPriceObj = DataBinder.Eval(e.Item.DataItem, "GiaMonAnSauKhuyenMai");
                decimal discountedPrice = discountedPriceObj != DBNull.Value ? Convert.ToDecimal(discountedPriceObj) : 0;

                // Nếu GiaMonAnSauKhuyenMai là NULL, lấy giá gốc từ GiaMonAn
                if (discountedPrice == 0)
                {
                    object originalPriceObj = DataBinder.Eval(e.Item.DataItem, "GiaMonAn");
                    discountedPrice = originalPriceObj != DBNull.Value ? Convert.ToDecimal(originalPriceObj) : 0;
                }

                if (!decimal.TryParse(quantity.Text, out decimal qty) || qty < 1)
                {
                    qty = 1; // Fallback to 1 if invalid
                }

                decimal calTotalPrice = discountedPrice * qty;
                totalPrice.Text = calTotalPrice.ToString("N0") + " VNĐ";
                productPrice.Text = discountedPrice.ToString("N0");
                grandTotal += calTotalPrice;
            }
            Session["grandTotalPrice"] = grandTotal; // Store as decimal
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
                    var footer = new LiteralControl("<tr><td colspan='6' class='text-center'><b style='color: #ff0000;'>Giỏ hàng của bạn đang trống rỗng.</b><a href='DanhSachMonAn.aspx' class='btn ml-2'><i class='fa fa-arrow-circle-left mr-2'></i>Tiếp tục chọn món</a></td></tr></tbody></table>");
                    container.Controls.Add(footer);
                }
            }
        }
    }
}