using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace WebDatDoAnOnline.Admin
{
    public partial class CapNhatPhieuNhap : System.Web.UI.Page
    {
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataAdapter sda;
        private DataTable dt;

        // Định nghĩa màu sắc và font cho PDF
        private static readonly BaseColor RED_COLOR = new BaseColor(200, 16, 46);
        private static readonly BaseColor GRAY_COLOR = new BaseColor(245, 245, 245);
        private static readonly BaseColor WHITE_COLOR = BaseColor.WHITE;
        private static readonly BaseFont baseFont = BaseFont.CreateFont(HttpContext.Current.Server.MapPath("~/Fonts/Roboto-Regular.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        private static readonly BaseFont baseFontBold = BaseFont.CreateFont(HttpContext.Current.Server.MapPath("~/Fonts/Roboto-Bold.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        private static readonly Font fontTitle = new Font(baseFontBold, 25, Font.NORMAL, RED_COLOR);
        private static readonly Font fontHeader = new Font(baseFontBold, 11, Font.BOLD, WHITE_COLOR);
        private static readonly Font fontData = new Font(baseFont, 11, Font.NORMAL, BaseColor.BLACK);
        private static readonly Font fontFooter = new Font(baseFont, 9, Font.NORMAL, BaseColor.GRAY);
        private static readonly Font fontSignatureTitle = new Font(baseFontBold, 11, Font.BOLD, BaseColor.BLACK);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Cập Nhật Phiếu Nhập Hàng";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                    return;
                }

                string maPhieu = Request.QueryString["MaPhieu"];
                if (string.IsNullOrEmpty(maPhieu))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Mã phiếu nhập không hợp lệ!');", true);
                    return;
                }

                txtMaPhieu.Text = maPhieu;
                LoadNhaCungCap();
                LoadNguyenLieu();
                LoadPhieuNhapDetails(maPhieu);
            }
            lblMsg.Visible = false;
        }

        private void LoadNhaCungCap()
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                cmd = new SqlCommand("SELECT MaNhaCungCap, TenNhaCungCap FROM NhaCungCap", con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                ddlNhaCungCap.DataSource = dt;
                ddlNhaCungCap.DataTextField = "TenNhaCungCap";
                ddlNhaCungCap.DataValueField = "MaNhaCungCap";
                ddlNhaCungCap.DataBind();
                ddlNhaCungCap.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Chọn nhà cung cấp", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi tải danh sách nhà cung cấp: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void LoadNguyenLieu()
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                cmd = new SqlCommand("SELECT MaNguyenLieu, TenNguyenLieu FROM NguyenLieu", con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                ddlNguyenLieu.DataSource = dt;
                ddlNguyenLieu.DataTextField = "TenNguyenLieu";
                ddlNguyenLieu.DataValueField = "MaNguyenLieu";
                ddlNguyenLieu.DataBind();
                ddlNguyenLieu.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Chọn nguyên liệu", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi tải danh sách nguyên liệu: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void LoadPhieuNhapDetails(string maPhieu)
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();

                // Load thông tin phiếu nhập
                cmd = new SqlCommand("SELECT MaPhieu, MaNhaCungCap, TongTien FROM PhieuNhap WHERE MaPhieu = @MaPhieu", con);
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtMaPhieu.Text = row["MaPhieu"].ToString();
                    ddlNhaCungCap.SelectedValue = row["MaNhaCungCap"].ToString();
                    lblTongTien.Text = String.Format("{0:N0} VND", Convert.ToDecimal(row["TongTien"]));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Không tìm thấy phiếu nhập!');", true);
                    return;
                }

                // Load chi tiết phiếu nhập
                cmd = new SqlCommand(
                    "SELECT c.MaNguyenLieu, n.TenNguyenLieu, c.DonViTinh, c.SoLuong, c.DonGia, (c.SoLuong * c.DonGia) AS ThanhTien " +
                    "FROM ChiTietPhieuNhap c JOIN NguyenLieu n ON c.MaNguyenLieu = n.MaNguyenLieu " +
                    "WHERE c.MaPhieu = @MaPhieu", con);
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                rChiTietPhieuNhap.DataSource = dt;
                rChiTietPhieuNhap.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi tải chi tiết phiếu nhập: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void ddlNguyenLieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlNguyenLieu.SelectedValue))
                {
                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    con.Open();
                    cmd = new SqlCommand("SELECT DonViTinh FROM NguyenLieu WHERE MaNguyenLieu = @MaNguyenLieu", con);
                    cmd.Parameters.AddWithValue("@MaNguyenLieu", ddlNguyenLieu.SelectedValue);
                    var donViTinh = cmd.ExecuteScalar()?.ToString();
                    txtDonViTinh.Text = donViTinh ?? "";
                }
                else
                {
                    txtDonViTinh.Text = "";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi tải đơn vị tính: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void btnAddDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlNguyenLieu.SelectedValue) || string.IsNullOrEmpty(txtQuantity.Text) || string.IsNullOrEmpty(txtUnitPrice.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Vui lòng nhập đầy đủ thông tin chi tiết!');", true);
                    return;
                }

                int soLuong;
                double donGia;
                if (!int.TryParse(txtQuantity.Text, out soLuong) || !double.TryParse(txtUnitPrice.Text, out donGia) || soLuong <= 0 || donGia < 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Số lượng và đơn giá phải là số hợp lệ, lớn hơn 0!');", true);
                    return;
                }

                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra nguyên liệu đã tồn tại
                        cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM ChiTietPhieuNhap WHERE MaPhieu = @MaPhieu AND MaNguyenLieu = @MaNguyenLieu",
                            con, transaction);
                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", ddlNguyenLieu.SelectedValue);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Cập nhật chi tiết
                            cmd = new SqlCommand(
                                "UPDATE ChiTietPhieuNhap SET SoLuong = SoLuong + @SoLuong, DonGia = @DonGia, DonViTinh = @DonViTinh " +
                                "WHERE MaPhieu = @MaPhieu AND MaNguyenLieu = @MaNguyenLieu",
                                con, transaction);
                        }
                        else
                        {
                            // Thêm chi tiết mới
                            cmd = new SqlCommand(
                                "INSERT INTO ChiTietPhieuNhap (MaPhieu, MaNguyenLieu, DonViTinh, SoLuong, DonGia) " +
                                "VALUES (@MaPhieu, @MaNguyenLieu, @DonViTinh, @SoLuong, @DonGia)",
                                con, transaction);
                        }

                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        cmd.Parameters.AddWithValue("@MaNguyenLieu", ddlNguyenLieu.SelectedValue);
                        cmd.Parameters.AddWithValue("@DonViTinh", txtDonViTinh.Text);
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.ExecuteNonQuery();

                        // Cập nhật tổng tiền
                        cmd = new SqlCommand(
                            "UPDATE PhieuNhap SET TongTien = (SELECT SUM(SoLuong * DonGia) FROM ChiTietPhieuNhap WHERE MaPhieu = @MaPhieu) " +
                            "WHERE MaPhieu = @MaPhieu",
                            con, transaction);
                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                            "showSuccessAlert('Thêm chi tiết phiếu nhập thành công!');", true);
                        LoadPhieuNhapDetails(txtMaPhieu.Text);
                        ClearDetailForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            $"showErrorAlert('Lỗi khi thêm chi tiết: {ex.Message.Replace("'", "\\'")}');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void ClearDetailForm()
        {
            ddlNguyenLieu.SelectedIndex = 0;
            txtDonViTinh.Text = "";
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtTotalPrice.Text = "";
        }

        protected void rChiTietPhieuNhap_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                try
                {
                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Xóa chi tiết
                            cmd = new SqlCommand(
                                "DELETE FROM ChiTietPhieuNhap WHERE MaPhieu = @MaPhieu AND MaNguyenLieu = @MaNguyenLieu",
                                con, transaction);
                            cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                            cmd.Parameters.AddWithValue("@MaNguyenLieu", e.CommandArgument);
                            cmd.ExecuteNonQuery();

                            // Cập nhật tổng tiền
                            cmd = new SqlCommand(
                                "UPDATE PhieuNhap SET TongTien = ISNULL((SELECT SUM(SoLuong * DonGia) FROM ChiTietPhieuNhap WHERE MaPhieu = @MaPhieu), 0) " +
                                "WHERE MaPhieu = @MaPhieu",
                                con, transaction);
                            cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                            cmd.ExecuteNonQuery();

                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                                "showSuccessAlert('Xóa chi tiết phiếu nhập thành công!');", true);
                            LoadPhieuNhapDetails(txtMaPhieu.Text);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                                $"showErrorAlert('Lỗi khi xóa chi tiết: {ex.Message.Replace("'", "\\'")}');", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi: {ex.Message.Replace("'", "\\'")}');", true);
                }
                finally
                {
                    if (con != null && con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlNhaCungCap.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Vui lòng chọn nhà cung cấp!');", true);
                    return;
                }

                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Cập nhật thông tin phiếu nhập
                        cmd = new SqlCommand(
                            "UPDATE PhieuNhap SET MaNhaCungCap = @MaNhaCungCap WHERE MaPhieu = @MaPhieu",
                            con, transaction);
                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        cmd.Parameters.AddWithValue("@MaNhaCungCap", ddlNhaCungCap.SelectedValue);
                        cmd.ExecuteNonQuery();

                        // Cập nhật tồn kho
                        cmd = new SqlCommand(
                            "SELECT MaNguyenLieu, SoLuong FROM ChiTietPhieuNhap WHERE MaPhieu = @MaPhieu",
                            con, transaction);
                        cmd.Parameters.AddWithValue("@MaPhieu", txtMaPhieu.Text);
                        sda = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            cmd = new SqlCommand(
                                "UPDATE NguyenLieu SET SoLuongTon = SoLuongTon + @SoLuong WHERE MaNguyenLieu = @MaNguyenLieu",
                                con, transaction);
                            cmd.Parameters.AddWithValue("@MaNguyenLieu", row["MaNguyenLieu"]);
                            cmd.Parameters.AddWithValue("@SoLuong", Convert.ToInt32(row["SoLuong"]));
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                            "showSuccessAlert('Cập nhật phiếu nhập thành công!');", true);
                        Response.Redirect("QuanLyPhieuNhapHang.aspx");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            $"showErrorAlert('Lỗi khi lưu phiếu nhập: {ex.Message.Replace("'", "\\'")}');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            ExportToPDF(txtMaPhieu.Text);
        }

        private void ExportToPDF(string maPhieu)
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();

                // Lấy thông tin phiếu nhập
                cmd = new SqlCommand(
                    "SELECT p.MaPhieu, p.ThoiGianTao, p.NguoiTao, p.MaNhaCungCap, p.TongTien " +
                    "FROM PhieuNhap p WHERE p.MaPhieu = @MaPhieu", con);
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Không tìm thấy phiếu nhập!');", true);
                    return;
                }

                DataRow phieuNhap = dt.Rows[0];
                string nguoiTaoName = phieuNhap["NguoiTao"].ToString();
                string nhaCungCapName = phieuNhap["MaNhaCungCap"].ToString();

                // Lấy tên người tạo và nhà cung cấp
                cmd = new SqlCommand("SELECT TenNhanVien FROM NhanVien WHERE TenDangNhapNhanVien = @TenDangNhap", con);
                cmd.Parameters.AddWithValue("@TenDangNhap", phieuNhap["NguoiTao"]);
                var result = cmd.ExecuteScalar();
                if (result != null) nguoiTaoName = result.ToString();

                cmd = new SqlCommand("SELECT TenNhaCungCap FROM NhaCungCap WHERE MaNhaCungCap = @MaNhaCungCap", con);
                cmd.Parameters.AddWithValue("@MaNhaCungCap", phieuNhap["MaNhaCungCap"]);
                result = cmd.ExecuteScalar();
                if (result != null) nhaCungCapName = result.ToString();

                // Lấy chi tiết phiếu nhập
                cmd = new SqlCommand(
                    "SELECT c.MaNguyenLieu, n.TenNguyenLieu, c.DonViTinh, c.SoLuong, c.DonGia " +
                    "FROM ChiTietPhieuNhap c JOIN NguyenLieu n ON c.MaNguyenLieu = n.MaNguyenLieu " +
                    "WHERE c.MaPhieu = @MaPhieu", con);
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                sda = new SqlDataAdapter(cmd);
                DataTable dtDetails = new DataTable();
                sda.Fill(dtDetails);

                using (MemoryStream ms = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4, 36, 36, 36, 36);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    writer.PageEvent = new PdfPageEventHelper
                    {
                        FooterFont = fontFooter
                    };

                    document.Open();

                    string logoPath = HttpContext.Current.Server.MapPath("~/Images/logovietkitchensg.png");
                    if (File.Exists(logoPath))
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(100, 100);
                        logo.Alignment = Element.ALIGN_CENTER;
                        logo.SpacingAfter = 10;
                        document.Add(logo);
                    }
                    else
                    {
                        Paragraph logoFallback = new Paragraph("NHÀ HÀNG VIET KITCHEN SÀI GÒN", fontTitle);
                        logoFallback.Alignment = Element.ALIGN_CENTER;
                        logoFallback.SpacingAfter = 10;
                        document.Add(logoFallback);
                    }

                    Paragraph pdfTitle = new Paragraph("PHIẾU NHẬP NGUYÊN LIỆU", fontTitle);
                    pdfTitle.Alignment = Element.ALIGN_CENTER;
                    pdfTitle.SpacingAfter = 10;
                    document.Add(pdfTitle);

                    LineSeparator line = new LineSeparator();
                    line.LineColor = RED_COLOR;
                    line.LineWidth = 1;
                    document.Add(new Chunk(line));
                    document.Add(Chunk.NEWLINE);

                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.SetWidths(new float[] { 50f, 50f });

                    PdfPCell cellLeft = new PdfPCell();
                    cellLeft.Border = Rectangle.NO_BORDER;
                    Paragraph paraLeft = new Paragraph();
                    paraLeft.Font = fontData;
                    paraLeft.Add($"Mã phiếu: {phieuNhap["MaPhieu"]}\n");
                    paraLeft.Add($"Thời gian tạo: {Convert.ToDateTime(phieuNhap["ThoiGianTao"]).ToString("dd/MM/yyyy HH:mm")}");
                    cellLeft.AddElement(paraLeft);

                    PdfPCell cellRight = new PdfPCell();
                    cellRight.Border = Rectangle.NO_BORDER;
                    Paragraph paraRight = new Paragraph();
                    paraRight.Font = fontData;
                    paraRight.Alignment = Element.ALIGN_RIGHT;
                    paraRight.Add($"Người tạo: {nguoiTaoName}\n");
                    paraRight.Add($"Nhà cung cấp: {nhaCungCapName}");
                    cellRight.AddElement(paraRight);

                    infoTable.AddCell(cellLeft);
                    infoTable.AddCell(cellRight);
                    document.Add(infoTable);
                    document.Add(Chunk.NEWLINE);

                    PdfPTable pdfTable = new PdfPTable(6);
                    pdfTable.WidthPercentage = 100;
                    pdfTable.SetWidths(new float[] { 10f, 25f, 10f, 15f, 10f, 15f });

                    string[] headers = { "Mã NL", "Tên nguyên liệu", "Đơn vị tính", "Đơn giá", "Số lượng", "Thành tiền" };
                    foreach (string header in headers)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(header, fontHeader));
                        headerCell.BackgroundColor = RED_COLOR;
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        headerCell.Padding = 8;
                        headerCell.BorderColor = RED_COLOR;
                        pdfTable.AddCell(headerCell);
                    }

                    foreach (DataRow detail in dtDetails.Rows)
                    {
                        PdfPCell cell;

                        cell = new PdfPCell(new Phrase(detail["MaNguyenLieu"].ToString(), fontData));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        cell.BackgroundColor = GRAY_COLOR;
                        pdfTable.AddCell(cell);

                        cell = new PdfPCell(new Phrase(detail["TenNguyenLieu"].ToString(), fontData));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.Padding = 5;
                        cell.BackgroundColor = GRAY_COLOR;
                        pdfTable.AddCell(cell);

                        cell = new PdfPCell(new Phrase(detail["DonViTinh"].ToString(), fontData));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        cell.BackgroundColor = GRAY_COLOR;
                        pdfTable.AddCell(cell);

                        double donGia = Convert.ToDouble(detail["DonGia"] ?? 0);
                        cell = new PdfPCell(new Phrase($"{donGia:N0} VNĐ", fontData));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Padding = 5;
                        cell.BackgroundColor = GRAY_COLOR;
                        pdfTable.AddCell(cell);

                        int soLuong = Convert.ToInt32(detail["SoLuong"] ?? 0);
                        cell = new PdfPCell(new Phrase(soLuong.ToString(), fontData));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        cell.BackgroundColor = GRAY_COLOR;
                        pdfTable.AddCell(cell);

                        double thanhTien = donGia * soLuong;
                        cell = new PdfPCell(new Phrase($"{thanhTien:N0} VNĐ", fontData));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Padding = 5;
                        cell.BackgroundColor = GRAY_COLOR;
                        pdfTable.AddCell(cell);
                    }

                    document.Add(pdfTable);
                    document.Add(Chunk.NEWLINE);

                    Font totalFont = new Font(fontData);
                    totalFont.SetStyle(Font.BOLD);
                    totalFont.Color = RED_COLOR;
                    Paragraph paraTongThanhToan = new Paragraph($"Tổng tiền nhập: {Convert.ToDecimal(phieuNhap["TongTien"]):N0} VNĐ", totalFont);
                    paraTongThanhToan.Alignment = Element.ALIGN_RIGHT;
                    paraTongThanhToan.SpacingBefore = 10;
                    document.Add(paraTongThanhToan);

                    document.Add(Chunk.NEWLINE);
                    PdfPTable signatureTable = new PdfPTable(3);
                    signatureTable.WidthPercentage = 100;
                    signatureTable.SetWidths(new float[] { 33f, 34f, 33f });
                    signatureTable.SpacingBefore = 20;
                    signatureTable.SpacingAfter = 20;

                    PdfPCell supplierCell = new PdfPCell();
                    supplierCell.Border = Rectangle.NO_BORDER;
                    supplierCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    Paragraph supplierPara = new Paragraph();
                    supplierPara.Font = fontSignatureTitle;
                    supplierPara.Add("ĐƠN VỊ CUNG CẤP\n");
                    supplierPara.Font = fontData;
                    supplierPara.Add(DateTime.Now.ToString("dd/MM/yyyy") + "\n\n\n\n\n");
                    supplierPara.Add(nhaCungCapName);
                    supplierCell.AddElement(supplierPara);
                    signatureTable.AddCell(supplierCell);

                    PdfPCell employeeCell = new PdfPCell();
                    employeeCell.Border = Rectangle.NO_BORDER;
                    employeeCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    Paragraph employeePara = new Paragraph();
                    employeePara.Font = fontSignatureTitle;
                    employeePara.Add("NHÂN VIÊN NHẬP HÀNG\n");
                    employeePara.Font = fontData;
                    employeePara.Add(DateTime.Now.ToString("dd/MM/yyyy") + "\n\n\n\n\n");
                    employeePara.Add(nguoiTaoName);
                    employeeCell.AddElement(employeePara);
                    signatureTable.AddCell(employeeCell);

                    PdfPCell managerCell = new PdfPCell();
                    managerCell.Border = Rectangle.NO_BORDER;
                    managerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Paragraph managerPara = new Paragraph();
                    managerPara.Font = fontSignatureTitle;
                    managerPara.Add("XÁC NHẬN QUẢN LÝ NHÀ HÀNG\n");
                    managerPara.Font = fontData;
                    managerPara.Add(DateTime.Now.ToString("dd/MM/yyyy") + "\n\n\n\n\n");
                    managerPara.Add("Nguyễn Thanh Trường");
                    managerCell.AddElement(managerPara);
                    signatureTable.AddCell(managerCell);

                    document.Add(signatureTable);

                    document.Add(Chunk.NEWLINE);
                    LineSeparator footerLine = new LineSeparator();
                    footerLine.LineColor = RED_COLOR;
                    footerLine.LineWidth = 1;
                    document.Add(new Chunk(footerLine));

                    Paragraph footer = new Paragraph();
                    footer.Font = fontFooter;
                    footer.Alignment = Element.ALIGN_CENTER;
                    footer.Add("Chương trình xây dựng website quản lý đặt món ăn online - Nhà hàng Viet Kitchen SG\n");
                    footer.Add("Địa chỉ: 15 Đ. Tôn Đức Thắng, Bến Nghé, Quận 1, Hồ Chí Minh\n");
                    footer.Add("Email: nhahangvietkitchensaigon@gmail.com | Hotline: 028 3822 0033\n");
                    footer.Add($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    document.Add(footer);

                    document.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", $"attachment;filename=PhieuNhap_{maPhieu}.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi xuất PDF: {ex.Message.Replace("'", "\\'")}');", true);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("QuanLyPhieuNhapHang.aspx");
        }

        private class PdfPageEventHelper : iTextSharp.text.pdf.PdfPageEventHelper
        {
            public Font FooterFont { get; set; }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPTable footerTable = new PdfPTable(1);
                footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                footerTable.DefaultCell.Border = Rectangle.NO_BORDER;
                footerTable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell cell = new PdfPCell(new Phrase($"Trang {writer.PageNumber}", FooterFont));
                cell.Border = Rectangle.NO_BORDER;
                footerTable.AddCell(cell);

                footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 10, writer.DirectContent);

                PdfContentByte canvas = writer.DirectContentUnder;
                Font watermarkFont = new Font(baseFont, 50, Font.NORMAL, new BaseColor(200, 16, 46, 128));
                Phrase watermark = new Phrase("Nhà hàng Viet Kitchen Sài Gòn", watermarkFont);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, watermark,
                    document.PageSize.Width / 2, document.PageSize.Height / 2, 45);
            }
        }
    }
}