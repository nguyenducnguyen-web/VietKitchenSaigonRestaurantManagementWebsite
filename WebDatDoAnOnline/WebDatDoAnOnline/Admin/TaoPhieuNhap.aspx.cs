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
    public partial class TaoPhieuNhap : System.Web.UI.Page
    {

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        // Temporary storage for purchase order details
        private List<PurchaseDetail> PurchaseDetails
        {
            get
            {
                if (Session["PurchaseDetails"] == null)
                    Session["PurchaseDetails"] = new List<PurchaseDetail>();
                return (List<PurchaseDetail>)Session["PurchaseDetails"];
            }
            set { Session["PurchaseDetails"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Tạo Phiếu Nhập";
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
                    LoadPurchaseDetails();
                    // Show Export PDF button if last created PhieuNhap exists
                    btnExportPDF.Visible = Session["LastCreatedPhieuNhap"] != null;
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
            cmd = new SqlCommand("SELECT COUNT(*) + 1 AS NextId FROM PhieuNhap", con);
            try
            {
                con.Open();
                string nextId = "PN" + cmd.ExecuteScalar().ToString(); // No leading zeros
                txtMaPhieu.Text = nextId;
            }
            catch
            {
                txtMaPhieu.Text = "PN1";
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadPurchaseDetails()
        {
            rPurchaseDetails.DataSource = PurchaseDetails;
            rPurchaseDetails.DataBind();
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            double total = PurchaseDetails.Sum(detail => detail.SoLuong * detail.DonGia);
            lblTongTien.Text = string.Format("{0:N0}", total);
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadIngredients();
        }

        protected void rIngredients_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "add")
            {
                try
                {
                    int soLuong = int.Parse(txtQuantity.Text.Trim());
                    double donGia = double.Parse(txtUnitPrice.Text.Trim());
                    if (soLuong <= 0)
                        throw new Exception("Số lượng phải lớn hơn 0!");
                    if (donGia <= 0)
                        throw new Exception("Đơn giá phải lớn hơn 0!");

                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    cmd = new SqlCommand("SELECT * FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con);
                    cmd.Parameters.AddWithValue("@MaNguyenLieu", e.CommandArgument);
                    sda = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        var existingDetail = PurchaseDetails.FirstOrDefault(d => d.MaNguyenLieu == e.CommandArgument.ToString());
                        if (existingDetail != null)
                        {
                            existingDetail.SoLuong += soLuong;
                        }
                        else
                        {
                            PurchaseDetails.Add(new PurchaseDetail
                            {
                                MaNguyenLieu = row["MaNguyenLieu"].ToString(),
                                TenNguyenLieu = row["TenNguyenLieu"].ToString(),
                                DonViTinh = row["DonViTinh"].ToString(),
                                SoLuong = soLuong,
                                DonGia = donGia
                            });
                        }
                        LoadPurchaseDetails();
                        lblMsg.Visible = true;
                        lblMsg.Text = "Thêm nguyên liệu thành công!";
                        lblMsg.CssClass = "alert alert-success";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Thêm nguyên liệu thành công!');", true);

                        // Reset input fields after successful addition
                        txtQuantity.Text = "1";
                        txtUnitPrice.Text = "";
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

        protected void rPurchaseDetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                string newQuantity = Request.Form["newQuantity"];
                if (string.IsNullOrEmpty(newQuantity))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "promptQuantity", $"var qty = prompt('Nhập số lượng mới:', ''); if (qty) {{ document.getElementById('{btnNhapHang.ClientID}').click(); }}", true);
                    return;
                }

                try
                {
                    int soLuong = int.Parse(newQuantity);
                    if (soLuong <= 0)
                        throw new Exception("Số lượng phải lớn hơn 0!");

                    var detail = PurchaseDetails.FirstOrDefault(d => d.MaNguyenLieu == e.CommandArgument.ToString());
                    if (detail != null)
                    {
                        detail.SoLuong = soLuong;
                        LoadPurchaseDetails();
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
                PurchaseDetails.RemoveAll(d => d.MaNguyenLieu == e.CommandArgument.ToString());
                LoadPurchaseDetails();
                lblMsg.Visible = true;
                lblMsg.Text = "Xóa nguyên liệu khỏi phiếu nhập thành công!";
                lblMsg.CssClass = "alert alert-success";
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Xóa nguyên liệu khỏi phiếu nhập thành công!');", true);
            }
        }

        protected void btnNhapHang_Click(object sender, EventArgs e)
        {
            if (PurchaseDetails.Count == 0)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Vui lòng chọn ít nhất một nguyên liệu để nhập hàng!";
                lblMsg.CssClass = "alert alert-warning";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Vui lòng chọn ít nhất một nguyên liệu để nhập hàng!');", true);
                return;
            }

            // Manually validate NhaCungCap
            if (ddlNhaCungCap.SelectedValue == "0")
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Vui lòng chọn nhà cung cấp!";
                lblMsg.CssClass = "alert alert-warning";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Vui lòng chọn nhà cung cấp!');", true);
                return;
            }

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            try
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    // Insert PhieuNhap
                    cmd = new SqlCommand("INSERT INTO PhieuNhap (MaPhieu, MaNhaCungCap, ThoiGianTao, NguoiTao, TongTien) VALUES (@MaPhieu, @MaNhaCungCap, @ThoiGianTao, @NguoiTao, @TongTien)", con, transaction);
                    cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                    cmd.Parameters.AddWithValue("@MaNhaCungCap", ddlNhaCungCap.SelectedValue);
                    cmd.Parameters.AddWithValue("@ThoiGianTao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@NguoiTao", Session["admin"].ToString()); // Store TenDangNhapNhanVien
                    cmd.Parameters.AddWithValue("@TongTien", PurchaseDetails.Sum(d => d.SoLuong * d.DonGia));
                    cmd.ExecuteNonQuery();

                    // Insert ChiTietPhieuNhap
                    foreach (var detail in PurchaseDetails)
                    {
                        // Verify NguyenLieu exists
                        cmd = new SqlCommand("SELECT COUNT(*) FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con, transaction);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", detail.MaNguyenLieu);
                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0)
                            throw new Exception($"Nguyên liệu {detail.MaNguyenLieu} không tồn tại!");

                        // Insert ChiTietPhieuNhap
                        cmd = new SqlCommand("INSERT INTO ChiTietPhieuNhap (MaPhieu, MaNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES (@MaPhieu, @MaNguyenLieu, @DonViTinh, @SoLuong, @DonGia)", con, transaction);
                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", detail.MaNguyenLieu);
                        cmd.Parameters.AddWithValue("@DonViTinh", detail.DonViTinh);
                        cmd.Parameters.AddWithValue("@SoLuong", detail.SoLuong);
                        cmd.Parameters.AddWithValue("@DonGia", detail.DonGia);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Store last created PhieuNhap for PDF export
                    var phieuNhap = new PhieuNhapInfo
                    {
                        MaPhieu = txtMaPhieu.Text,
                        MaNhaCungCap = ddlNhaCungCap.SelectedValue,
                        ThoiGianTao = DateTime.Now,
                        NguoiTao = Session["admin"].ToString(),
                        TongTien = PurchaseDetails.Sum(d => d.SoLuong * d.DonGia),
                        Details = PurchaseDetails.ToList()
                    };
                    Session["LastCreatedPhieuNhap"] = phieuNhap;

                    lblMsg.Visible = true;
                    lblMsg.Text = "Nhập hàng thành công";
                    lblMsg.CssClass = "alert alert-success";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Nhập hàng thành công');", true);
                    btnExportPDF.Visible = true; // Show Export PDF button
                    ClearForm();
                    LoadIngredients(); // Refresh ingredient list to reflect updated SoLuongTon

                    // Ask if user wants to download PDF
                    ScriptManager.RegisterStartupScript(this, GetType(), "askDownloadPDF",
                        $"Swal.fire({{title: 'Tải phiếu nhập', text: 'Bạn có muốn tải phiếu nhập PDF về máy không?', icon: 'question', showCancelButton: true, confirmButtonText: 'Có', cancelButtonText: 'Không', customClass: {{ popup: 'custom-swal-popup', title: 'custom-swal-title', htmlContainer: 'custom-swal-text', confirmButton: 'custom-swal-button', cancelButton: 'custom-swal-button' }}}}).then((result) => {{ if (result.isConfirmed) {{ document.getElementById('{btnExportPDF.ClientID}').click(); }} }});",
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
            if (Session["LastCreatedPhieuNhap"] == null)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Không tìm thấy thông tin phiếu nhập để xuất PDF!";
                lblMsg.CssClass = "alert alert-warning";
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Không tìm thấy thông tin phiếu nhập để xuất PDF!');", true);
                return;
            }

            try
            {
                var phieuNhap = (PhieuNhapInfo)Session["LastCreatedPhieuNhap"];
                byte[] pdfBytes = PDFGenerator.GeneratePhieuNhapPDF(phieuNhap);

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename=PhieuNhap_{phieuNhap.MaPhieu}.pdf");
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
            PurchaseDetails.Clear();
            txtQuantity.Text = "1";
            txtUnitPrice.Text = "";
            txtSearch.Text = "";
            lblTongTien.Text = "0";
            GenerateMaPhieu();
            LoadIngredients();
            LoadPurchaseDetails();
            ddlNhaCungCap.SelectedIndex = 0; // Reset supplier dropdown
        }

        // Helper class for temporary purchase details
        public class PurchaseDetail
        {
            public string MaNguyenLieu { get; set; }
            public string TenNguyenLieu { get; set; }
            public string DonViTinh { get; set; }
            public int SoLuong { get; set; }
            public double DonGia { get; set; }
        }

        // Helper class to store PhieuNhap info for PDF export
        public class PhieuNhapInfo
        {
            public string MaPhieu { get; set; }
            public string MaNhaCungCap { get; set; }
            public DateTime ThoiGianTao { get; set; }
            public string NguoiTao { get; set; }
            public double TongTien { get; set; }
            public List<PurchaseDetail> Details { get; set; }
        }
    }
}