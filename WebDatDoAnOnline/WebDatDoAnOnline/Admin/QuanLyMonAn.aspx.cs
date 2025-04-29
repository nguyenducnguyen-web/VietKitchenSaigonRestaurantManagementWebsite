using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyMonAn : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        // references
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Món ăn";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getProducts();
                    SqlDataSource1.DataBind();
                    ddlCategories.DataBind();
                    UpdateStats();
                }
            }
            lblMsg.Visible = false;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;

            int productId = Convert.ToInt32(hdnId.Value);

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Product_Crud", con);

            cmd.Parameters.AddWithValue("@Action", productId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
            cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text.Trim());
            cmd.Parameters.AddWithValue("@CategoryId", ddlCategories.SelectedValue);
            cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked);
            cmd.Parameters.AddWithValue("@KhuyenMai", string.IsNullOrEmpty(txtKhuyenMai.Text.Trim()) ? DBNull.Value : (object)Convert.ToDouble(txtKhuyenMai.Text.Trim()));

            if (fuProductImage.HasFile)
            {
                if (Utils.IsValidExtension(fuProductImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExtension = Path.GetExtension(fuProductImage.FileName);
                    imagePath = "Images/Product/" + obj.ToString() + fileExtension;
                    fuProductImage.PostedFile.SaveAs(Server.MapPath("~/Images/Product/") + obj.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@ImageUrl", imagePath);
                    isValidToExecute = true;
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Vui lòng chọn hình ảnh .jpg, .jpeg hoặc .png";
                    lblMsg.CssClass = "alert alert-danger";
                    isValidToExecute = false;
                }
            }
            else
            {
                isValidToExecute = true;
            }

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = productId == 0 ? "thêm" : "cập nhật";
                    lblMsg.Visible = true;
                    lblMsg.Text = $"Món ăn được {actionName} thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", $"showSuccessAlert('Món ăn được {actionName} thành công!');", true);
                    getProducts();
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

        private void getProducts()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Product_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            // Áp dụng tìm kiếm
            var filteredRows = dt.AsEnumerable();
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                filteredRows = filteredRows
                    .Where(row => row.Field<string>("TenMonAn").ToLower().Contains(txtSearch.Text.Trim().ToLower()));
            }

            // Áp dụng sắp xếp
            string sortOption = ddlSort.SelectedValue;
            if (!string.IsNullOrEmpty(sortOption))
            {
                switch (sortOption)
                {
                    case "price_desc":
                        filteredRows = filteredRows.OrderByDescending(row => row.Field<double>("GiaMonAn"));
                        break;
                    case "price_asc":
                        filteredRows = filteredRows.OrderBy(row => row.Field<double>("GiaMonAn"));
                        break;
                    case "newest":
                        filteredRows = filteredRows.OrderByDescending(row => row.Field<DateTime>("NgayTaoMonAn"));
                        break;
                    case "discount_desc":
                        filteredRows = filteredRows.OrderByDescending(row => row.Field<double?>("KhuyenMai") ?? 0);
                        break;
                }
            }

            // Kiểm tra nếu có dữ liệu để tránh lỗi CopyToDataTable
            if (filteredRows.Any())
            {
                dt = filteredRows.CopyToDataTable();
            }
            else
            {
                dt.Clear(); // Nếu không có dữ liệu, trả về bảng rỗng
            }

            rProduct.DataSource = dt;
            rProduct.DataBind();
        }

        private void UpdateStats()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) AS Total, SUM(CASE WHEN CoHoatDongMonAn = 1 THEN 1 ELSE 0 END) AS Active, SUM(CASE WHEN SoLuong <= 5 THEN 1 ELSE 0 END) AS LowStock FROM MonAn", con);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalProducts.InnerText = reader["Total"].ToString();
                        activeProducts.InnerText = reader["Active"].ToString();
                        lowStockProducts.InnerText = reader["LowStock"].ToString();
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
            txtDescription.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtKhuyenMai.Text = string.Empty;
            ddlCategories.ClearSelection();
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            btnAddOrUpdate.Text = "Thêm";
            imgProduct.ImageUrl = string.Empty;
            txtSearch.Text = string.Empty;
            ddlSort.ClearSelection();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            getProducts();
        }

        protected void rProduct_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());

            if (e.CommandName == "edit")
            {
                cmd = new SqlCommand("Product_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                txtName.Text = dt.Rows[0]["TenMonAn"].ToString();
                txtDescription.Text = dt.Rows[0]["MoTaMonAn"].ToString();
                txtPrice.Text = dt.Rows[0]["GiaMonAn"].ToString();
                txtQuantity.Text = dt.Rows[0]["SoLuong"].ToString();
                txtKhuyenMai.Text = dt.Rows[0]["KhuyenMai"] != DBNull.Value ? dt.Rows[0]["KhuyenMai"].ToString() : string.Empty;
                ddlCategories.SelectedValue = dt.Rows[0]["MaLoaiMonAn"].ToString();
                cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["CoHoatDongMonAn"]);
                imgProduct.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["DiaChiAnhMonAn"].ToString()) ?
                    "../Images/No_image.png" : "../" + dt.Rows[0]["DiaChiAnhMonAn"].ToString();
                imgProduct.Height = 200;
                imgProduct.Width = 200;
                hdnId.Value = dt.Rows[0]["MaMonAn"].ToString();
                btnAddOrUpdate.Text = "Cập nhật";
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-primary";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new SqlCommand("Product_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Món ăn đã được xóa thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Món ăn đã được xóa thành công!');", true);
                    getProducts();
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

        protected void rProduct_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblIsActive = e.Item.FindControl("lblIsActive") as Label;
                Label lblQuantity = e.Item.FindControl("lblQuantity") as Label;

                if (lblIsActive.Text == "True")
                {
                    lblIsActive.Text = "Hoạt động";
                    lblIsActive.CssClass = "badge badge-success";
                }
                else
                {
                    lblIsActive.Text = "Không hoạt động";
                    lblIsActive.CssClass = "badge badge-danger";
                }

                if (Convert.ToInt32(lblQuantity.Text) <= 5)
                {
                    lblQuantity.CssClass = "badge badge-danger";
                    lblQuantity.ToolTip = "Món ăn sắp hết hàng!";
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getProducts();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            getProducts();
        }
    }
}
