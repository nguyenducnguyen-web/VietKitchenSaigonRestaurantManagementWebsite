using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.Admin
{
    public partial class CapNhatPhieuXuat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Cập Nhật Phiếu Xuất Hàng";

                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else if (string.IsNullOrEmpty(Request.QueryString["MaPhieu"]))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Mã phiếu xuất không hợp lệ!');", true);
                    Response.Redirect("QuanLyPhieuXuatHang.aspx");
                }
                else
                {
                    LoadPhieuXuat();
                    LoadNguyenLieu();
                    LoadChiTietPhieuXuat();
                }
            }

            lblMsg.Visible = false;
        }

        private void LoadPhieuXuat()
        {
            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM PhieuXuat WHERE MaPhieu = @MaPhieu", con))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                txtMaPhieu.Text = dt.Rows[0]["MaPhieu"].ToString();
                                txtThoiGianTao.Text = Convert.ToDateTime(dt.Rows[0]["ThoiGianTao"]).ToString("yyyy-MM-ddTHH:mm");
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                    "showErrorAlert('Phiếu xuất không tồn tại!');", true);
                                Response.Redirect("QuanLyPhieuXuatHang.aspx");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi khi tải thông tin phiếu xuất: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        private void LoadNguyenLieu()
        {
            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MaNguyenLieu, TenNguyenLieu FROM NguyenLieu", con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            ddlNguyenLieu.DataSource = dt;
                            ddlNguyenLieu.DataTextField = "TenNguyenLieu";
                            ddlNguyenLieu.DataValueField = "MaNguyenLieu";
                            ddlNguyenLieu.DataBind();
                            ddlNguyenLieu.Items.Insert(0, new ListItem("Chọn nguyên liệu", ""));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi khi tải danh sách nguyên liệu: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        private void LoadChiTietPhieuXuat()
        {
            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT c.*, n.TenNguyenLieu, " +
                        "CAST(COALESCE(c.SoLuong * c.DonGia, 0) AS DECIMAL(18,2)) AS ThanhTien " +
                        "FROM ChiTietPhieuXuat c " +
                        "JOIN NguyenLieu n ON c.MaNguyenLieu = n.MaNguyenLieu " +
                        "WHERE c.MaPhieu = @MaPhieu", con))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            rChiTietPhieuXuat.DataSource = dt;
                            rChiTietPhieuXuat.DataBind();

                            decimal tongTien = dt.AsEnumerable()
                                .Sum(row => row.IsNull("ThanhTien") ? 0 : row.Field<decimal>("ThanhTien"));
                            lblTongTien.Text = String.Format("{0:N0} VND", tongTien);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi tải chi tiết phiếu xuất: {ex.Message}\n{ex.StackTrace}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi khi tải chi tiết phiếu xuất: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        protected void rChiTietPhieuXuat_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "update")
            {
                string maNguyenLieu = e.CommandArgument.ToString();
                TextBox txtSoLuong = (TextBox)e.Item.FindControl("txtSoLuong");
                if (string.IsNullOrEmpty(txtSoLuong.Text) || !int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Số lượng không hợp lệ!');", true);
                    return;
                }

                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    try
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE ChiTietPhieuXuat SET SoLuong = @SoLuong WHERE MaPhieu = @MaPhieu AND MaNguyenLieu = @MaNguyenLieu", con))
                        {
                            cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                            cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                            cmd.Parameters.AddWithValue("@MaNguyenLieu", maNguyenLieu);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                                    "showSuccessAlert('Cập nhật chi tiết thành công!');", true);
                                LoadChiTietPhieuXuat();
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                    "showErrorAlert('Không thể cập nhật chi tiết!');", true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            $"showErrorAlert('Lỗi khi cập nhật chi tiết: {ex.Message.Replace("'", "\\'")}');", true);
                    }
                }
            }
        }

        protected void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlNguyenLieu.SelectedValue) || string.IsNullOrEmpty(txtNewSoLuong.Text) || string.IsNullOrEmpty(txtNewDonGia.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    "showErrorAlert('Vui lòng chọn nguyên liệu, nhập số lượng và đơn giá!');", true);
                return;
            }

            if (!int.TryParse(txtNewSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    "showErrorAlert('Số lượng không hợp lệ!');", true);
                return;
            }

            if (!decimal.TryParse(txtNewDonGia.Text, out decimal donGia) || donGia < 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    "showErrorAlert('Đơn giá không hợp lệ!');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                try
                {
                    con.Open();

                    // Check if the ingredient already exists in the voucher
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM ChiTietPhieuXuat WHERE MaPhieu = @MaPhieu AND MaNguyenLieu = @MaNguyenLieu", con))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", ddlNguyenLieu.SelectedValue);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                "showErrorAlert('Nguyên liệu này đã tồn tại trong phiếu xuất!');", true);
                            return;
                        }
                    }

                    // Get DonViTinh from NguyenLieu
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT DonViTinh FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con))
                    {
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", ddlNguyenLieu.SelectedValue);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                    "showErrorAlert('Nguyên liệu không tồn tại!');", true);
                                return;
                            }

                            string donViTinh = dt.Rows[0]["DonViTinh"].ToString();

                            // Insert new detail
                            using (SqlCommand insertCmd = new SqlCommand(
                                "INSERT INTO ChiTietPhieuXuat (MaPhieu, MaNguyenLieu, SoLuong, DonViTinh, DonGia) " +
                                "VALUES (@MaPhieu, @MaNguyenLieu, @SoLuong, @DonViTinh, @DonGia)", con))
                            {
                                insertCmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                                insertCmd.Parameters.AddWithValue("@MaNguyenLieu", ddlNguyenLieu.SelectedValue);
                                insertCmd.Parameters.AddWithValue("@SoLuong", soLuong);
                                insertCmd.Parameters.AddWithValue("@DonViTinh", donViTinh);
                                insertCmd.Parameters.AddWithValue("@DonGia", donGia);
                                int rowsAffected = insertCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                                        "showSuccessAlert('Thêm chi tiết thành công!');", true);
                                    txtNewSoLuong.Text = "";
                                    txtNewDonGia.Text = "";
                                    ddlNguyenLieu.SelectedIndex = 0;
                                    LoadChiTietPhieuXuat();
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                        "showErrorAlert('Không thể thêm chi tiết!');", true);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi khi thêm chi tiết: {ex.Message.Replace("'", "\\'")}');", true);
                    System.Diagnostics.Debug.WriteLine($"Lỗi thêm chi tiết: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        protected void btnDeleteDetail_Click(object sender, EventArgs e)
        {
            string maNguyenLieu = hdnDeleteDetailId.Value;
            if (string.IsNullOrEmpty(maNguyenLieu))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    "showErrorAlert('Mã nguyên liệu không hợp lệ!');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM ChiTietPhieuXuat WHERE MaPhieu = @MaPhieu AND MaNguyenLieu = @MaNguyenLieu", con))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", maNguyenLieu);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                                "showSuccessAlert('Xóa chi tiết thành công!');", true);
                            LoadChiTietPhieuXuat();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                "showErrorAlert('Không thể xóa chi tiết!');", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi khi xóa chi tiết: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtThoiGianTao.Text) || !DateTime.TryParse(txtThoiGianTao.Text, out DateTime thoiGianTao))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    "showErrorAlert('Thời gian tạo không hợp lệ!');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
            {
                try
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Update PhieuXuat
                            using (SqlCommand cmd = new SqlCommand(
                                "UPDATE PhieuXuat SET ThoiGianTao = @ThoiGianTao, TongTien = @TongTien WHERE MaPhieu = @MaPhieu",
                                con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ThoiGianTao", thoiGianTao);
                                cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                                cmd.Parameters.AddWithValue("@TongTien", CalculateTongTien(con, transaction));
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                                        "showSuccessAlert('Cập nhật phiếu xuất thành công!');setTimeout(function(){window.location='QuanLyPhieuXuatHang.aspx';},2000);", true);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                        "showErrorAlert('Không thể cập nhật phiếu xuất!');", true);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                $"showErrorAlert('Lỗi khi lưu phiếu xuất: {ex.Message.Replace("'", "\\'")}');", true);
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi hệ thống: {ex.Message.Replace("'", "\\'")}');", true);
                    System.Diagnostics.Debug.WriteLine($"Lỗi hệ thống: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        private decimal CalculateTongTien(SqlConnection con, SqlTransaction transaction)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT SUM(CAST(COALESCE(SoLuong * DonGia, 0) AS DECIMAL(18,2))) AS TongTien " +
                    "FROM ChiTietPhieuXuat WHERE MaPhieu = @MaPhieu", con, transaction))
                {
                    cmd.Parameters.AddWithValue("@MaPhieu", Request.QueryString["MaPhieu"]);
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tính tổng tiền: {ex.Message}\n{ex.StackTrace}");
                return 0;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("QuanLyPhieuXuatHang.aspx");
        }
    }
}