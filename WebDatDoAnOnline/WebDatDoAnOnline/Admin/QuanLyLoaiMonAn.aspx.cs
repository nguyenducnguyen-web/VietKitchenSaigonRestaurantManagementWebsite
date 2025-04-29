using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyLoaiMonAn : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Loại món ăn";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    getCategories();
                    UpdateStats();
                }
            }
            lblMsg.Visible = false;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;

            int categoryId = Convert.ToInt32(hdnId.Value);
            con = new SqlConnection(ConnectionSQL.GetConnectionString());

            cmd = new SqlCommand("Category_Crud", con);
            cmd.Parameters.AddWithValue("@Action", categoryId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked);
            cmd.CommandTimeout = 60; // Increase timeout to 60 seconds

            if (fuCategoryImage.HasFile)
            {
                if (Utils.IsValidExtension(fuCategoryImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExtension = Path.GetExtension(fuCategoryImage.FileName);
                    imagePath = "Images/LoaiMonAn/" + obj.ToString() + fileExtension;
                    fuCategoryImage.PostedFile.SaveAs(Server.MapPath("~/Images/LoaiMonAn/") + obj.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@ImageUrl", imagePath);
                    isValidToExecute = true;
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Vui lòng chọn hình ảnh .jpg, .jpeg hoặc .png";
                    lblMsg.CssClass = "alert alert-danger";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Vui lòng chọn hình ảnh .jpg, .jpeg hoặc .png');", true);
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
                    actionName = categoryId == 0 ? "thêm" : "cập nhật";
                    lblMsg.Visible = true;
                    lblMsg.Text = $"Loại món ăn được {actionName} thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", $"showSuccessAlert('Loại món ăn được {actionName} thành công!');", true);
                    getCategories();
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

        private void getCategories(string searchText = "")
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Category_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.Parameters.AddWithValue("@SearchText", searchText);
            cmd.Parameters.AddWithValue("@SortOption", ddlSort.SelectedValue);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 60; // Increase timeout
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            try
            {
                con.Open();
                var startTime = DateTime.Now;
                sda.Fill(dt);
                var duration = (DateTime.Now - startTime).TotalSeconds;
                Debug.WriteLine($"getCategories took {duration} seconds");

                // Áp dụng tìm kiếm và sắp xếp
                var filteredRows = dt.AsEnumerable();
                if (!string.IsNullOrEmpty(searchText))
                {
                    filteredRows = filteredRows
                        .Where(row => row.Field<string>("TenLoaiMonAn").ToLower().Contains(searchText.ToLower()));
                }

                string sortOption = ddlSort.SelectedValue;
                if (!string.IsNullOrEmpty(sortOption))
                {
                    switch (sortOption)
                    {
                        case "newest":
                            filteredRows = filteredRows.OrderByDescending(row => row.Field<DateTime>("NgayTaoLoaiMonAn"));
                            break;
                        case "name_asc":
                            filteredRows = filteredRows.OrderBy(row => row.Field<string>("TenLoaiMonAn"));
                            break;
                        case "name_desc":
                            filteredRows = filteredRows.OrderByDescending(row => row.Field<string>("TenLoaiMonAn"));
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

                rCategory.DataSource = dt;
                rCategory.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi khi tải danh sách: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi khi tải danh sách: {ex.Message}');", true);
            }
            finally
            {
                con.Close();
            }
        }

        private void UpdateStats()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) AS Total, SUM(CASE WHEN CoHoatDongLoaiMonAn = 1 THEN 1 ELSE 0 END) AS Active, SUM(CASE WHEN CoHoatDongLoaiMonAn = 0 THEN 1 ELSE 0 END) AS Inactive FROM LoaiMonAn", con);
            cmd.CommandTimeout = 60; // Increase timeout
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalCategories.InnerText = reader["Total"].ToString();
                        activeCategories.InnerText = reader["Active"].ToString();
                        inactiveCategories.InnerText = reader["Inactive"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi khi cập nhật thống kê: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi khi cập nhật thống kê: {ex.Message}');", true);
            }
            finally
            {
                con.Close();
            }
        }

        private void clear()
        {
            txtName.Text = string.Empty;
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            btnAddOrUpdate.Text = "Thêm";
            imgCategory.ImageUrl = string.Empty;
            txtSearch.Text = string.Empty;
            ddlSort.ClearSelection();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            getCategories();
        }

        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            if (e.CommandName == "edit")
            {
                cmd = new SqlCommand("Category_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@CategoryId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60; // Increase timeout
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                try
                {
                    con.Open();
                    var startTime = DateTime.Now;
                    sda.Fill(dt);
                    var duration = (DateTime.Now - startTime).TotalSeconds;
                    Debug.WriteLine($"GETBYID for CategoryId {e.CommandArgument} took {duration} seconds");

                    if (dt.Rows.Count > 0)
                    {
                        txtName.Text = dt.Rows[0]["TenLoaiMonAn"].ToString();
                        cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["CoHoatDongLoaiMonAn"]);
                        imgCategory.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["DiaChiAnhLoaiMonAn"].ToString()) ? "../Images/No_image.png" : "../" + dt.Rows[0]["DiaChiAnhLoaiMonAn"].ToString();
                        imgCategory.Height = 200;
                        imgCategory.Width = 200;
                        hdnId.Value = dt.Rows[0]["MaLoaiMonAn"].ToString();
                        btnAddOrUpdate.Text = "Cập nhật";
                        LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                        btn.CssClass = "badge badge-primary";
                    }
                    else
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Không tìm thấy loại món ăn!";
                        lblMsg.CssClass = "alert alert-danger";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Không tìm thấy loại món ăn!');", true);
                    }
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
            else if (e.CommandName == "delete")
            {
                cmd = new SqlCommand("Category_Crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@CategoryId", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60; // Increase timeout
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Loại món ăn đã được xóa thành công!";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Loại món ăn đã được xóa thành công!');", true);
                    getCategories();
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

        protected void rCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lbl = e.Item.FindControl("lblIsActive") as Label;
                if (lbl.Text == "True")
                {
                    lbl.Text = "Hoạt động";
                    lbl.CssClass = "badge badge-success";
                }
                else
                {
                    lbl.Text = "Không hoạt động";
                    lbl.CssClass = "badge badge-danger";
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getCategories(txtSearch.Text.Trim());
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            getCategories(txtSearch.Text.Trim());
        }
    }
}