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
    public partial class ThanhToan : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr, drl;
        DataTable dt;
        SqlTransaction transaction = null;
        string _name = string.Empty;
        string _cardNo = string.Empty;
        string _expiryDate = string.Empty;
        string _cvv = string.Empty;
        string _address = string.Empty;
        string _paymentMode = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["MaNguoiDung"] == null)
                {
                    Response.Redirect("DangNhap.aspx");
                }
                // Calculate total price including VAT
                if (Session["grandTotalPrice"] != null && decimal.TryParse(Session["grandTotalPrice"].ToString(), out decimal grandTotal))
                {
                    Session["grandTotalPriceWithVAT"] = grandTotal * 1.08m; // Include 8% VAT
                }
                else
                {
                    Session["grandTotalPrice"] = 0m;
                    Session["grandTotalPriceWithVAT"] = 0m;
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "showErrorAlert('Không tìm thấy thông tin giỏ hàng. Vui lòng quay lại giỏ hàng.');", true);
                }
                // Generate and store OrderNo in Session for QR payment
                Session["OrderNo"] = WebDatDoAnOnline.Utils.GetUniqueId();
            }
        }

        protected void lbCardSubmit_Click(object sender, EventArgs e)
        {
            _name = txtName.Text.Trim();
            _cardNo = txtCardNo.Text.Trim();
            _cardNo = string.Format("************{0}", txtCardNo.Text.Trim().Substring(12, 4));
            _expiryDate = txtExpMonth.Text.Trim() + "/" + txtExpYear.Text.Trim();
            _cvv = txtCvv.Text.Trim();
            _address = txtAddress.Text.Trim();
            _paymentMode = "card";
            if (Session["MaNguoiDung"] != null)
            {
                OrderPayment(_name, _cardNo, _expiryDate, _cvv, _address, _paymentMode);
            }
            else
            {
                Response.Redirect("DangNhap.aspx");
            }
        }

        protected void lbCodSubmit_Click(object sender, EventArgs e)
        {
            _address = txtCODAddress.Text.Trim();
            _paymentMode = "cod";
            if (Session["MaNguoiDung"] != null)
            {
                _name = null;
                _cardNo = null;
                _expiryDate = null;
                _cvv = null;
                OrderPayment(_name, _cardNo, _expiryDate, _cvv, _address, _paymentMode);
            }
            else
            {
                Response.Redirect("DangNhap.aspx");
            }
        }

        protected void lbQRSubmit_Click(object sender, EventArgs e)
        {
            _address = txtQRAddress.Text.Trim();
            _paymentMode = "qr";
            if (Session["MaNguoiDung"] != null)
            {
                _name = null;
                _cardNo = null;
                _expiryDate = null;
                _cvv = null;
                OrderPayment(_name, _cardNo, _expiryDate, _cvv, _address, _paymentMode);
            }
            else
            {
                Response.Redirect("DangNhap.aspx");
            }
        }

        void OrderPayment(string name, string cardNo, string expiryDate, string cvv, string address, string paymentMode)
        {
            int paymentId = -1;
            dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[8] {
                new DataColumn("OrderNo", typeof(string)),
                new DataColumn("MaMonAn", typeof(int)),
                new DataColumn("SoLuong", typeof(int)),
                new DataColumn("MaNguoiDung", typeof(int)),
                new DataColumn("TrangThai", typeof(string)),
                new DataColumn("MaThanhToan", typeof(int)),
                new DataColumn("NgayTaoOrder", typeof(DateTime)),
                new DataColumn("GiaMonAnSauKhuyenMai", typeof(decimal))
            });

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            con.Open();
            transaction = con.BeginTransaction();
            cmd = new SqlCommand("Save_Payment", con, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", (object)name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CardNo", (object)cardNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate", (object)expiryDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cvv", (object)cvv ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", address);
            cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);

            SqlParameter insertedIdParam = new SqlParameter("@InsertedId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(insertedIdParam);

            try
            {
                cmd.ExecuteNonQuery();
                paymentId = Convert.ToInt32(cmd.Parameters["@InsertedId"].Value);

                cmd = new SqlCommand("Cart_Crud", con, transaction);
                cmd.Parameters.AddWithValue("@Action", "SELECT");
                cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader();
                int rowCount = 0;
                while (dr.Read())
                {
                    int productId = (int)dr["MaMonAn"];
                    int quantity = (int)dr["SoLuong"];
                    decimal giaMonAnSauKhuyenMai = Convert.ToDecimal(dr["GiaMonAnSauKhuyenMai"]);
                    UpdateQuantity(productId, quantity, transaction, con);
                    DeleteCartItem(productId, transaction, con);
                    dt.Rows.Add(Session["OrderNo"].ToString(), productId, quantity, (int)Session["MaNguoiDung"], "Đang xử lý", paymentId, DateTime.Now, giaMonAnSauKhuyenMai);
                    rowCount++;
                }
                dr.Close();

                if (dt.Rows.Count > 0)
                {
                    cmd = new SqlCommand("Save_Orders", con, transaction);
                    cmd.Parameters.AddWithValue("@tblOrders", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                // Hiển thị thông báo thành công cho tất cả các phương thức thanh toán
                string successMessage = paymentMode == "card" ? "Thanh toán bằng thẻ thành công!" :
                                       paymentMode == "cod" ? "Đặt hàng COD thành công!" :
                                       "Thanh toán bằng QR thành công!";
                ScriptManager.RegisterStartupScript(this, GetType(), "success", $"showSuccessAlert('{successMessage}', 'HoaDon.aspx?id={paymentId}');", true);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showErrorAlert('Lỗi: {ex.Message}');", true);
                }
                catch (Exception rollbackEx)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showErrorAlert('Rollback lỗi: {rollbackEx.Message}');", true);
                }
            }
            finally
            {
                con.Close();
            }
        }

        void UpdateQuantity(int _productId, int _quantity, SqlTransaction sqlTransaction, SqlConnection sqlConnection)
        {
            int dbQuantity;
            cmd = new SqlCommand("Product_Crud", sqlConnection, sqlTransaction);
            cmd.Parameters.AddWithValue("@Action", "GETBYID");
            cmd.Parameters.AddWithValue("@ProductId", _productId);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                drl = cmd.ExecuteReader();
                while (drl.Read())
                {
                    dbQuantity = (int)drl["SoLuong"];
                    if (dbQuantity > _quantity && dbQuantity > 2)
                    {
                        dbQuantity = dbQuantity - _quantity;
                        cmd = new SqlCommand("Product_Crud", sqlConnection, sqlTransaction);
                        cmd.Parameters.AddWithValue("@Action", "QTYUPDATE");
                        cmd.Parameters.AddWithValue("@Quantity", dbQuantity);
                        cmd.Parameters.AddWithValue("@ProductId", _productId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
                drl.Close();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showErrorAlert('Lỗi UpdateQuantity: {ex.Message}');", true);
            }
        }

        void DeleteCartItem(int _productId, SqlTransaction sqlTransaction, SqlConnection sqlConnection)
        {
            cmd = new SqlCommand("Cart_Crud", sqlConnection, sqlTransaction);
            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@ProductId", _productId);
            cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showErrorAlert('Lỗi DeleteCartItem: {ex.Message}');", true);
            }
        }
    }
}