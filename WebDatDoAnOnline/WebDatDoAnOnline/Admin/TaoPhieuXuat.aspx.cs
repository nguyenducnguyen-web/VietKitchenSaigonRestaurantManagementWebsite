using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.Admin
{
    public partial class TaoPhieuXuat : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        // Temporary storage for export order details
        private List<ExportDetail> ExportDetails
        {
            get
            {
                if (Session["ExportDetails"] == null)
                    Session["ExportDetails"] = new List<ExportDetail>();
                return (List<ExportDetail>)Session["ExportDetails"];
            }
            set { Session["ExportDetails"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Tạo Phiếu Xuất";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    LoadIngredients();
                    GenerateMaPhieu();
                    // Set txtNguoiTao to TenNhanVien instead of TenDangNhapNhanVien
                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    cmd = new SqlCommand("SELECT TenNhanVien FROM NhanVien WHERE TenDangNhapNhanVien = @TenDangNhap", con);
                    cmd.Parameters.AddWithValue("@TenDangNhap", Session["admin"].ToString());
                    try
                    {
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        txtNguoiTao.Text = result != null ? result.ToString() : Session["admin"].ToString();
                    }
                    catch
                    {
                        txtNguoiTao.Text = Session["admin"].ToString();
                    }
                    finally
                    {
                        con.Close();
                    }
                    LoadExportDetails();
                    // Show Export PDF button if last created PhieuXuat exists
                    btnExportPDF.Visible = Session["LastCreatedPhieuXuat"] != null;
                }
            }
            lblMsg.Visible = false;
        }

        private void LoadIngredients()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT * FROM NguyenLieu", con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            // Apply search filter
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                dt = dt.AsEnumerable()
                    .Where(row => row.Field<string>("TenNguyenLieu").ToLower().Contains(txtSearch.Text.Trim().ToLower()))
                    .CopyToDataTable();
            }

            rIngredients.DataSource = dt;
            rIngredients.DataBind();
        }

        private void GenerateMaPhieu()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("SELECT COUNT(*) + 1 AS NextId FROM PhieuXuat", con);
            try
            {
                con.Open();
                string nextId = "PX" + cmd.ExecuteScalar().ToString(); // No leading zeros
                txtMaPhieu.Text = nextId;
            }
            catch
            {
                txtMaPhieu.Text = "PX1";
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadExportDetails()
        {
            rExportDetails.DataSource = ExportDetails;
            rExportDetails.DataBind();
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            double total = ExportDetails.Sum(detail => detail.SoLuong * detail.DonGia);
            lblTongTien.Text = string.Format("{0:N0}", total);
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadIngredients();
        }

        private double GetLatestUnitPrice(string maNguyenLieu)
        {
            double donGia = 0;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand(
                @"SELECT TOP 1 cpn.DonGia 
                  FROM ChiTietPhieuNhap cpn 
                  INNER JOIN PhieuNhap pn ON cpn.MaPhieu = pn.MaPhieu 
                  WHERE cpn.MaNguyenLieu = @MaNguyenLieu 
                  ORDER BY pn.ThoiGianTao DESC", con);
            cmd.Parameters.AddWithValue("@MaNguyenLieu", maNguyenLieu);
            try
            {
                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    donGia = Convert.ToDouble(result);
                }
                else
                {
                    throw new Exception("Không tìm thấy đơn giá cho nguyên liệu này trong phiếu nhập!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy đơn giá: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return donGia;
        }

        protected void rIngredients_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "add")
            {
                try
                {
                    int soLuong = int.Parse(txtQuantity.Text.Trim());
                    if (soLuong <= 0)
                        throw new Exception("Số lượng phải lớn hơn 0!");

                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    cmd = new SqlCommand("SELECT * FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con);
                    cmd.Parameters.AddWithValue("@MaNguyenLieu", e.CommandArgument);
                    sda = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        int soLuongTon = Convert.ToInt32(row["SoLuongTon"]);
                        if (soLuong > soLuongTon)
                            throw new Exception($"Số lượng tồn ({soLuongTon}) không đủ để xuất {soLuong}!");

                        // Fetch the latest unit price from ChiTietPhieuNhap
                        double donGia = GetLatestUnitPrice(e.CommandArgument.ToString());
                        if (donGia <= 0)
                            throw new Exception("Đơn giá phải lớn hơn 0!");

                        var existingDetail = ExportDetails.FirstOrDefault(d => d.MaNguyenLieu == e.CommandArgument.ToString());
                        if (existingDetail != null)
                        {
                            if (existingDetail.SoLuong + soLuong > soLuongTon)
                                throw new Exception($"Tổng số lượng ({existingDetail.SoLuong + soLuong}) vượt quá số lượng tồn ({soLuongTon})!");
                            existingDetail.SoLuong += soLuong;
                            existingDetail.DonGia = donGia; // Update unit price in case it changed
                        }
                        else
                        {
                            ExportDetails.Add(new ExportDetail
                            {
                                MaNguyenLieu = row["MaNguyenLieu"].ToString(),
                                TenNguyenLieu = row["TenNguyenLieu"].ToString(),
                                DonViTinh = row["DonViTinh"].ToString(),
                                SoLuong = soLuong,
                                DonGia = donGia
                            });
                        }
                        LoadExportDetails();
                        lblMsg.Visible = true;
                        lblMsg.Text = "Thêm nguyên liệu thành công!";
                        lblMsg.CssClass = "alert alert-success";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Thêm nguyên liệu thành công!');", true);

                        // Reset input fields after successful addition
                        txtQuantity.Text = "1";
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Lỗi: " + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi: {ex.Message}');", true);
                }
            }
        }

        protected void rExportDetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                string newQuantity = Request.Form["newQuantity"];
                if (string.IsNullOrEmpty(newQuantity))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "promptQuantity", $"var qty = prompt('Nhập số lượng mới:', ''); if (qty) {{ document.getElementById('{btnXuatHang.ClientID}').click(); }}", true);
                    return;
                }

                try
                {
                    int soLuong = int.Parse(newQuantity);
                    if (soLuong <= 0)
                        throw new Exception("Số lượng phải lớn hơn 0!");

                    var detail = ExportDetails.FirstOrDefault(d => d.MaNguyenLieu == e.CommandArgument.ToString());
                    if (detail != null)
                    {
                        con = new SqlConnection(ConnectionSQL.GetConnectionString());
                        cmd = new SqlCommand("SELECT SoLuongTon FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", detail.MaNguyenLieu);
                        con.Open();
                        int soLuongTon = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();

                        if (soLuong > soLuongTon)
                            throw new Exception($"Số lượng mới ({soLuong}) vượt quá số lượng tồn ({soLuongTon})!");

                        // Update quantity and refresh unit price
                        detail.SoLuong = soLuong;
                        detail.DonGia = GetLatestUnitPrice(detail.MaNguyenLieu);
                        LoadExportDetails();
                        lblMsg.Visible = true;
                        lblMsg.Text = "Cập nhật số lượng thành công!";
                        lblMsg.CssClass = "alert alert-success";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Cập nhật số lượng thành công!');", true);
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Lỗi: " + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi: {ex.Message}');", true);
                }
            }
            else if (e.CommandName == "delete")
            {
                ExportDetails.RemoveAll(d => d.MaNguyenLieu == e.CommandArgument.ToString());
                LoadExportDetails();
                lblMsg.Visible = true;
                lblMsg.Text = "Xóa nguyên liệu khỏi phiếu xuất thành công!";
                lblMsg.CssClass = "alert alert-success";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Xóa nguyên liệu khỏi phiếu xuất thành công!');", true);
            }
        }

        protected void btnXuatHang_Click(object sender, EventArgs e)
        {
            if (ExportDetails.Count == 0)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Vui lòng chọn ít nhất một nguyên liệu để xuất hàng!";
                lblMsg.CssClass = "alert alert-warning";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Vui lòng chọn ít nhất một nguyên liệu để xuất hàng!');", true);
                return;
            }

            // Check SoLuongTon for all details
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            try
            {
                con.Open();
                foreach (var detail in ExportDetails)
                {
                    cmd = new SqlCommand("SELECT SoLuongTon FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con);
                    cmd.Parameters.AddWithValue("@MaNguyenLieu", detail.MaNguyenLieu);
                    int soLuongTon = Convert.ToInt32(cmd.ExecuteScalar());
                    if (detail.SoLuong > soLuongTon)
                        throw new Exception($"Nguyên liệu {detail.TenNguyenLieu}: Số lượng xuất ({detail.SoLuong}) vượt quá số lượng tồn ({soLuongTon})!");
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi: {ex.Message}');", true);
                con.Close();
                return;
            }

            try
            {
                using (var transaction = con.BeginTransaction())
                {
                    // Insert PhieuXuat
                    cmd = new SqlCommand("INSERT INTO PhieuXuat (MaPhieu, ThoiGianTao, NguoiTao, TongTien) VALUES (@MaPhieu, @ThoiGianTao, @NguoiTao, @TongTien)", con, transaction);
                    cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                    cmd.Parameters.AddWithValue("@ThoiGianTao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@NguoiTao", Session["admin"].ToString());
                    cmd.Parameters.AddWithValue("@TongTien", ExportDetails.Sum(d => d.SoLuong * d.DonGia));
                    cmd.ExecuteNonQuery();

                    // Insert ChiTietPhieuXuat
                    foreach (var detail in ExportDetails)
                    {
                        // Verify NguyenLieu exists
                        cmd = new SqlCommand("SELECT COUNT(*) FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con, transaction);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", detail.MaNguyenLieu);
                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0)
                            throw new Exception($"Nguyên liệu {detail.MaNguyenLieu} không tồn tại!");

                        // Insert ChiTietPhieuXuat
                        cmd = new SqlCommand("INSERT INTO ChiTietPhieuXuat (MaPhieu, MaNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES (@MaPhieu, @MaNguyenLieu, @DonViTinh, @SoLuong, @DonGia)", con, transaction);
                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", detail.MaNguyenLieu);
                        cmd.Parameters.AddWithValue("@DonViTinh", detail.DonViTinh);
                        cmd.Parameters.AddWithValue("@SoLuong", detail.SoLuong);
                        cmd.Parameters.AddWithValue("@DonGia", detail.DonGia);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Store last created PhieuXuat for PDF export
                    var phieuXuat = new PhieuXuatInfo
                    {
                        MaPhieu = txtMaPhieu.Text,
                        ThoiGianTao = DateTime.Now,
                        NguoiTao = Session["admin"].ToString(),
                        TongTien = ExportDetails.Sum(d => d.SoLuong * d.DonGia),
                        Details = ExportDetails.ToList()
                    };
                    Session["LastCreatedPhieuXuat"] = phieuXuat;

                    lblMsg.Visible = true;
                    lblMsg.Text = "Xuất hàng thành công";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Xuất hàng thành công');", true);
                    btnExportPDF.Visible = true; // Show Export PDF button
                    ClearForm();
                    LoadIngredients(); // Refresh ingredient list to reflect updated SoLuongTon

                    // Ask if user wants to download PDF
                    ScriptManager.RegisterStartupScript(this, GetType(), "askDownloadPDF",
                        $"Swal.fire({{title: 'Tải phiếu xuất', text: 'Bạn có muốn tải phiếu xuất PDF về máy không?', icon: 'question', showCancelButton: true, confirmButtonText: 'Có', cancelButtonText: 'Không', customClass: {{ popup: 'custom-swal-popup', title: 'custom-swal-title', htmlContainer: 'custom-swal-text', confirmButton: 'custom-swal-button', cancelButton: 'custom-swal-button' }}}}).then((result) => {{ if (result.isConfirmed) {{ document.getElementById('{btnExportPDF.ClientID}').click(); }} }});",
                        true);
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (Session["LastCreatedPhieuXuat"] == null)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Không tìm thấy thông tin phiếu xuất để xuất PDF!";
                lblMsg.CssClass = "alert alert-warning";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Không tìm thấy thông tin phiếu xuất để xuất PDF!');", true);
                return;
            }

            try
            {
                var phieuXuat = (PhieuXuatInfo)Session["LastCreatedPhieuXuat"];
                byte[] pdfBytes = PDFGenerator.GeneratePhieuXuatPDF(phieuXuat);

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename=PhieuXuat_{phieuXuat.MaPhieu}.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Lỗi khi xuất PDF: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Lỗi khi xuất PDF: {ex.Message}');", true);
            }
        }

        private void ClearForm()
        {
            ExportDetails.Clear();
            txtQuantity.Text = "1";
            txtSearch.Text = "";
            lblTongTien.Text = "0";
            GenerateMaPhieu();
            LoadIngredients();
            LoadExportDetails();
        }

        // Helper class for temporary export details
        public class ExportDetail
        {
            public string MaNguyenLieu { get; set; }
            public string TenNguyenLieu { get; set; }
            public string DonViTinh { get; set; }
            public int SoLuong { get; set; }
            public double DonGia { get; set; }
        }

        // Helper class to store PhieuXuat info for PDF export
        public class PhieuXuatInfo
        {
            public string MaPhieu { get; set; }
            public DateTime ThoiGianTao { get; set; }
            public string NguoiTao { get; set; }
            public double TongTien { get; set; }
            public List<ExportDetail> Details { get; set; }
        }
    }
}