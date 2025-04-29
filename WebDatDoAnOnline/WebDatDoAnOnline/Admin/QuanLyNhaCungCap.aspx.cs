using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyNhaCungCap : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Nhà cung cấp";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getSuppliers();
                    UpdateStats();
                    if (string.IsNullOrEmpty(hdnId.Value))
                    {
                        txtMaNhaCungCap.Text = GenerateMaNhaCungCap();
                    }
                }
            }
            lblMsg.Visible = false;
        }

        private string GenerateMaNhaCungCap()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT MaNhaCungCap FROM NhaCungCap WITH (NOLOCK)", con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            int id = dt.Rows.Count + 1;
            string newId = "NCC" + id.ToString("D3");
            bool exists = dt.AsEnumerable().Any(row => row.Field<string>("MaNhaCungCap") == newId);

            while (exists)
            {
                id++;
                newId = "NCC" + id.ToString("D3");
                exists = dt.AsEnumerable().Any(row => row.Field<string>("MaNhaCungCap") == newId);
            }

            return newId;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty;
            bool isValidToExecute = true;

            string maNhaCungCap = txtMaNhaCungCap.Text.Trim();

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NhaCungCap_Crud", con);

            cmd.Parameters.AddWithValue("@Action", string.IsNullOrEmpty(hdnId.Value) ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
            cmd.Parameters.AddWithValue("@TenNhaCungCap", txtTenNhaCungCap.Text.Trim());
            cmd.Parameters.AddWithValue("@Sdt", txtSdt.Text.Trim());
            cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = string.IsNullOrEmpty(hdnId.Value) ? "thêm" : "cập nhật";
                    lblMsg.Visible = true;
                    lblMsg.Text = $"Nhà cung cấp được {actionName} thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", $"showSuccessAlert('Nhà cung cấp được {actionName} thành công!');", true);
                    getSuppliers();
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

        private void getSuppliers()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("NhaCungCap_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            var filteredRows = dt.AsEnumerable();
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                filteredRows = filteredRows
                    .Where(row => row.Field<string>("TenNhaCungCap").ToLower().Contains(txtSearch.Text.Trim().ToLower()) ||
                                  row.Field<string>("Sdt").Contains(txtSearch.Text.Trim()));
            }

            string sortOption = ddlSort.SelectedValue;
            if (!string.IsNullOrEmpty(sortOption))
            {
                switch (sortOption)
                {
                    case "name_asc":
                        filteredRows = filteredRows.OrderBy(row => row.Field<string>("TenNhaCungCap"));
                        break;
                    case "name_desc":
                        filteredRows = filteredRows.OrderByDescending(row => row.Field<string>("TenNhaCungCap"));
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

            rSupplier.DataSource = dt;
            rSupplier.DataBind();
        }

        private void UpdateStats()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) AS Total, SUM(CASE WHEN Sdt IS NOT NULL AND Sdt != '' THEN 1 ELSE 0 END) AS Active FROM NhaCungCap", con);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalSuppliers.InnerText = reader["Total"].ToString();
                        activeSuppliers.InnerText = reader["Active"].ToString();
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
            txtMaNhaCungCap.Text = GenerateMaNhaCungCap();
            txtTenNhaCungCap.Text = string.Empty;
            txtSdt.Text = string.Empty;
            txtDiaChi.Text = string.Empty;
            hdnId.Value = string.Empty;
            btnAddOrUpdate.Text = "Thêm";
            txtSearch.Text = string.Empty;
            ddlSort.ClearSelection();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            getSuppliers();
        }

        protected void rSupplier_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());

            if (e.CommandName == "edit")
            {
                cmd = new SqlCommand("NhaCungCap_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@MaNhaCungCap", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                txtMaNhaCungCap.Text = dt.Rows[0]["MaNhaCungCap"].ToString();
                txtTenNhaCungCap.Text = dt.Rows[0]["TenNhaCungCap"].ToString();
                txtSdt.Text = dt.Rows[0]["Sdt"].ToString();
                txtDiaChi.Text = dt.Rows[0]["DiaChi"].ToString();
                hdnId.Value = dt.Rows[0]["MaNhaCungCap"].ToString();
                btnAddOrUpdate.Text = "Cập nhật";
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-primary";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new SqlCommand("NhaCungCap_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaNhaCungCap", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Nhà cung cấp đã được xóa thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Nhà cung cấp đã được xóa thành công!');", true);
                    getSuppliers();
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

        protected void rSupplier_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Optional: Add logic for styling rows based on conditions, e.g., highlight suppliers with no Sdt
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getSuppliers();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            getSuppliers();
        }
    }
}