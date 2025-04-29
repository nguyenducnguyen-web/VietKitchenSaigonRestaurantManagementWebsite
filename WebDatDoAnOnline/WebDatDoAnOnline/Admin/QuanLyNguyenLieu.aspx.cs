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
    public partial class QuanLyNguyenLieu : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Nguyên liệu";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getIngredients();
                    UpdateStats();
                    // Generate MaNguyenLieu for new entry
                    if (string.IsNullOrEmpty(hdnId.Value))
                    {
                        txtMaNguyenLieu.Text = GenerateMaNguyenLieu();
                    }
                }
            }
            lblMsg.Visible = false;
        }

        private string GenerateMaNguyenLieu()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT MaNguyenLieu FROM NguyenLieu WITH (NOLOCK)", con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            int id = dt.Rows.Count + 1;
            string newId = "NL" + id.ToString("D3");
            bool exists = dt.AsEnumerable().Any(row => row.Field<string>("MaNguyenLieu") == newId);

            while (exists)
            {
                id++;
                newId = "NL" + id.ToString("D3");
                exists = dt.AsEnumerable().Any(row => row.Field<string>("MaNguyenLieu") == newId);
            }

            return newId;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty;
            bool isValidToExecute = true;

            string maNguyenLieu = txtMaNguyenLieu.Text.Trim();

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NguyenLieu_Crud", con);

            cmd.Parameters.AddWithValue("@Action", string.IsNullOrEmpty(hdnId.Value) ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@MaNguyenLieu", maNguyenLieu);
            cmd.Parameters.AddWithValue("@TenNguyenLieu", txtTenNguyenLieu.Text.Trim());
            cmd.Parameters.AddWithValue("@DonViTinh", txtDonViTinh.Text.Trim());
            cmd.Parameters.AddWithValue("@SoLuongTon", Convert.ToInt32(txtSoLuongTon.Text.Trim()));

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = string.IsNullOrEmpty(hdnId.Value) ? "thêm" : "cập nhật";
                    lblMsg.Visible = true;
                    lblMsg.Text = $"Nguyên liệu được {actionName} thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", $"showSuccessAlert('Nguyên liệu được {actionName} thành công!');", true);
                    getIngredients();
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

        private void getIngredients()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NguyenLieu_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            var filteredRows = dt.AsEnumerable();
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                filteredRows = filteredRows
                    .Where(row => row.Field<string>("TenNguyenLieu").ToLower().Contains(txtSearch.Text.Trim().ToLower()));
            }

            string sortOption = ddlSort.SelectedValue;
            if (!string.IsNullOrEmpty(sortOption))
            {
                switch (sortOption)
                {
                    case "stock_desc":
                        filteredRows = filteredRows.OrderByDescending(row => row.Field<int>("SoLuongTon"));
                        break;
                    case "stock_asc":
                        filteredRows = filteredRows.OrderBy(row => row.Field<int>("SoLuongTon"));
                        break;
                }
            }

            if (filteredRows.Any())
            {
                dt = filteredRows.CopyToDataTable();
            }
            else
            {
                dt.Clear();
            }

            rIngredient.DataSource = dt;
            rIngredient.DataBind();
        }

        private void UpdateStats()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) AS Total, SUM(CASE WHEN SoLuongTon <= 5 AND SoLuongTon > 0 THEN 1 ELSE 0 END) AS LowStock, SUM(CASE WHEN SoLuongTon = 0 THEN 1 ELSE 0 END) AS ZeroStock FROM NguyenLieu", con);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalIngredients.InnerText = reader["Total"].ToString();
                        lowStockIngredients.InnerText = reader["LowStock"].ToString();
                        zeroStockIngredients.InnerText = reader["ZeroStock"].ToString();
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
            txtMaNguyenLieu.Text = GenerateMaNguyenLieu();
            txtTenNguyenLieu.Text = string.Empty;
            txtDonViTinh.Text = string.Empty;
            txtSoLuongTon.Text = string.Empty;
            hdnId.Value = string.Empty;
            btnAddOrUpdate.Text = "Thêm";
            txtSearch.Text = string.Empty;
            ddlSort.ClearSelection();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            getIngredients();
        }

        protected void rIngredient_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());

            if (e.CommandName == "edit")
            {
                cmd = new SqlCommand("NguyenLieu_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@MaNguyenLieu", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                txtMaNguyenLieu.Text = dt.Rows[0]["MaNguyenLieu"].ToString();
                txtTenNguyenLieu.Text = dt.Rows[0]["TenNguyenLieu"].ToString();
                txtDonViTinh.Text = dt.Rows[0]["DonViTinh"].ToString();
                txtSoLuongTon.Text = dt.Rows[0]["SoLuongTon"].ToString();
                hdnId.Value = dt.Rows[0]["MaNguyenLieu"].ToString();
                btnAddOrUpdate.Text = "Cập nhật";
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-primary";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new SqlCommand("NguyenLieu_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaNguyenLieu", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Nguyên liệu đã được xóa thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Nguyên liệu đã được xóa thành công!');", true);
                    getIngredients();
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

        protected void rIngredient_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblSoLuongTon = e.Item.FindControl("lblSoLuongTon") as Label;

                if (Convert.ToInt32(lblSoLuongTon.Text) <= 5)
                {
                    lblSoLuongTon.CssClass = "badge badge-danger";
                    lblSoLuongTon.ToolTip = "Nguyên liệu sắp hết hoặc đã hết!";
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getIngredients();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            getIngredients();
        }
    }
}