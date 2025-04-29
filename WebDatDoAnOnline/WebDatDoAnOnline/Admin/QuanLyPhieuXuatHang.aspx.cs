using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyPhieuXuatHang : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        // Định nghĩa màu sắc
        private static readonly BaseColor RED_COLOR = new BaseColor(200, 16, 46);
        private static readonly BaseColor GRAY_COLOR = new BaseColor(245, 245, 245);
        private static readonly BaseColor WHITE_COLOR = BaseColor.WHITE;

        // Định nghĩa font
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
                Session["breadCrum"] = "Quản Lý Phiếu Xuất Hàng";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../NguoiDung/DangNhap.aspx");
                }
                else
                {
                    LoadPhieuXuat();
                    LoadStatistics();
                }
            }
            lblMsg.Visible = false;
        }

        private void LoadStatistics()
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                cmd = new SqlCommand("SELECT COUNT(*) AS Total, SUM(TongTien) AS TotalAmount, " +
                    "(SELECT COUNT(*) FROM PhieuXuat WHERE CAST(ThoiGianTao AS DATE) = CAST(GETDATE() AS DATE)) AS TodayCount " +
                    "FROM PhieuXuat", con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    totalPhieuXuatCount.InnerText = dt.Rows[0]["Total"].ToString();
                    todayPhieuXuatCount.InnerText = dt.Rows[0]["TodayCount"].ToString();
                    totalAmount.InnerText = String.Format("{0:N0} VND", Convert.ToDecimal(dt.Rows[0]["TotalAmount"] ?? 0));
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi tải thống kê: {ex.Message.Replace("'", "\\'")}');", true);
                System.Diagnostics.Debug.WriteLine($"Lỗi tải thống kê: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void LoadPhieuXuat()
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                cmd = new SqlCommand("sp_GetPhieuXuat", con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    cmd.Parameters.AddWithValue("@SearchText", txtSearch.Text.Trim());
                }
                if (ddlSortType.SelectedValue != "0")
                {
                    cmd.Parameters.AddWithValue("@SortType", ddlSortType.SelectedValue);
                }

                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                rPhieuXuat.DataSource = dt;
                rPhieuXuat.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi tải danh sách phiếu xuất: {ex.Message.Replace("'", "\\'")}');", true);
                System.Diagnostics.Debug.WriteLine($"Lỗi tải danh sách phiếu xuất: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void ddlSortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPhieuXuat();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadPhieuXuat();
        }

        protected void rPhieuXuat_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;

            if (e.CommandName == "edit")
            {
                string maPhieu = e.CommandArgument.ToString();
                if (!string.IsNullOrEmpty(maPhieu))
                {
                    try
                    {
                        Response.Redirect($"CapNhatPhieuXuat.aspx?MaPhieu={maPhieu}");
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            $"showErrorAlert('Lỗi khi chuyển hướng: {ex.Message.Replace("'", "\\'")}');", true);
                        System.Diagnostics.Debug.WriteLine($"Lỗi chuyển hướng đến CapNhatPhieuXuat: {ex.Message}\n{ex.StackTrace}");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Mã phiếu xuất không hợp lệ!');", true);
                    System.Diagnostics.Debug.WriteLine("Lỗi: Mã phiếu xuất rỗng trong rPhieuXuat_ItemCommand");
                }
            }
            else if (e.CommandName == "exportPDF")
            {
                ExportToPDF(e.CommandArgument.ToString());
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string maPhieu = hdnDeletePhieuId.Value;
            if (string.IsNullOrEmpty(maPhieu))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    "showErrorAlert('Mã phiếu xuất không hợp lệ!');", true);
                System.Diagnostics.Debug.WriteLine("Lỗi: Mã phiếu xuất rỗng trong btnDelete_Click");
                return;
            }

            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Kiểm tra phiếu xuất tồn tại
                    cmd = new SqlCommand("SELECT COUNT(*) FROM PhieuXuat WHERE MaPhieu = @MaPhieu", con, transaction);
                    cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0)
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            "showErrorAlert('Phiếu xuất không tồn tại!');", true);
                        System.Diagnostics.Debug.WriteLine($"Lỗi: Phiếu xuất {maPhieu} không tồn tại");
                        return;
                    }

                    // Xóa chi tiết phiếu xuất
                    cmd = new SqlCommand("DELETE FROM ChiTietPhieuXuat WHERE MaPhieu = @MaPhieu", con, transaction);
                    cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmd.ExecuteNonQuery();

                    // Xóa phiếu xuất
                    cmd = new SqlCommand("DELETE FROM PhieuXuat WHERE MaPhieu = @MaPhieu", con, transaction);
                    cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                            "showSuccessAlert('Xóa phiếu xuất và chi tiết phiếu xuất thành công!');", true);
                        LoadPhieuXuat();
                        LoadStatistics();
                    }
                    else
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                            "showErrorAlert('Không thể xóa phiếu xuất!');", true);
                        System.Diagnostics.Debug.WriteLine($"Lỗi: Không thể xóa phiếu xuất {maPhieu}");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        $"showErrorAlert('Lỗi khi xóa phiếu xuất: {ex.Message.Replace("'", "\\'")}');", true);
                    System.Diagnostics.Debug.WriteLine($"Lỗi xóa phiếu xuất {maPhieu}: {ex.Message}\n{ex.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi hệ thống khi xóa phiếu xuất: {ex.Message.Replace("'", "\\'")}');", true);
                System.Diagnostics.Debug.WriteLine($"Lỗi hệ thống xóa phiếu xuất {maPhieu}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        [WebMethod]
        public static string GetPhieuXuatDetails(string phieuId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    con.Open();
                    StringBuilder html = new StringBuilder();

                    // Lấy thông tin phiếu xuất
                    using (SqlCommand cmd = new SqlCommand("sp_GetPhieuXuatDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaPhieu", phieuId);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            html.Append("<div style='padding: 20px;'>");
                            html.Append("<h4 style='color: #ffffff; text-align: center; margin-bottom: 20px; font-family: Poppins, sans-serif;'>Thông tin phiếu xuất</h4>");
                            html.Append("<div class='info-container'>");
                            html.Append("<div class='info-item'><strong>Mã phiếu xuất:</strong> ").Append(row["MaPhieu"] ?? "N/A").Append("</div>");
                            html.Append("<div class='info-item'><strong>Người tạo:</strong> ").Append(row["TenNguoiTao"] ?? "N/A").Append("</div>");
                            html.Append("<div class='info-item'><strong>Thời gian tạo:</strong> ").Append(row["ThoiGianTao"] != DBNull.Value ? Convert.ToDateTime(row["ThoiGianTao"]).ToString("dd/MM/yyyy HH:mm") : "N/A").Append("</div>");
                            html.Append("<div class='info-item'><strong>Tổng tiền:</strong> ").Append(row["TongTien"] != DBNull.Value ? String.Format("{0:N0} VND", Convert.ToDecimal(row["TongTien"])) : "0 VND").Append("</div>");
                            html.Append("</div>");
                        }
                        else
                        {
                            html.Append("<p style='text-align: center; color: #ffffff;'>Không tìm thấy thông tin phiếu xuất.</p>");
                            return html.ToString();
                        }
                    }

                    // Lấy chi tiết phiếu xuất
                    using (SqlCommand cmd = new SqlCommand("SELECT c.*, n.TenNguyenLieu FROM ChiTietPhieuXuat c JOIN NguyenLieu n ON c.MaNguyenLieu = n.MaNguyenLieu WHERE c.MaPhieu = @MaPhieu", con))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieu", phieuId);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dtDetails = new DataTable();
                        sda.Fill(dtDetails);

                        if (dtDetails.Rows.Count > 0)
                        {
                            html.Append("<h4 style='color: #ffffff; text-align: center; margin: 20px 0; font-family: Poppins, sans-serif;'>Chi tiết nguyên liệu</h4>");
                            html.Append("<table class='detail-table'>");
                            html.Append("<thead><tr>");
                            html.Append("<th style='width: 15%;'>Mã nguyên liệu</th>");
                            html.Append("<th style='width: 25%;'>Tên nguyên liệu</th>");
                            html.Append("<th style='width: 15%;'>Số lượng</th>");
                            html.Append("<th style='width: 15%;'>Đơn vị tính</th>");
                            html.Append("<th style='width: 15%;'>Đơn giá</th>");
                            html.Append("<th style='width: 15%;'>Thành tiền</th>");
                            html.Append("</tr></thead><tbody>");

                            foreach (DataRow detail in dtDetails.Rows)
                            {
                                double donGia = Convert.ToDouble(detail["DonGia"] ?? 0);
                                int soLuong = Convert.ToInt32(detail["SoLuong"] ?? 0);
                                double thanhTien = donGia * soLuong;
                                html.Append("<tr>");
                                html.Append("<td>").Append(detail["MaNguyenLieu"] ?? "N/A").Append("</td>");
                                html.Append("<td>").Append(detail["TenNguyenLieu"] ?? "N/A").Append("</td>");
                                html.Append("<td>").Append(detail["SoLuong"] ?? "0").Append("</td>");
                                html.Append("<td>").Append(detail["DonViTinh"] ?? "N/A").Append("</td>");
                                html.Append("<td style='text-align: right;'>").Append(String.Format("{0:N0} VND", donGia)).Append("</td>");
                                html.Append("<td style='text-align: right;'>").Append(String.Format("{0:N0} VND", thanhTien)).Append("</td>");
                                html.Append("</tr>");
                            }
                            html.Append("</tbody></table>");
                        }
                        else
                        {
                            html.Append("<p style='text-align: center; color: #ffffff; margin-top: 20px;'>Không có chi tiết nguyên liệu.</p>");
                        }
                    }

                    html.Append("</div>");
                    return html.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong GetPhieuXuatDetails: {ex.Message}\n{ex.StackTrace}");
                return "";
            }
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

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            // Clear search and sort filters
            txtSearch.Text = "";
            ddlSortType.SelectedValue = "0";

            // Reload data
            LoadPhieuXuat();
            LoadStatistics();

            // Show success alert
            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert",
                "showSuccessAlert('Làm mới danh sách thành công!');", true);
        }

        private void ExportToPDF(string maPhieu)
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                con.Open();

                // Lấy thông tin phiếu xuất
                cmd = new SqlCommand("sp_GetPhieuXuatDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                        "showErrorAlert('Không tìm thấy phiếu xuất!');", true);
                    return;
                }

                DataRow phieuXuat = dt.Rows[0];
                string nguoiTaoName = phieuXuat["TenNguoiTao"].ToString();

                using (MemoryStream ms = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4, 36, 36, 36, 36);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    // Add page number and watermark event
                    writer.PageEvent = new PdfPageEventHelper
                    {
                        FooterFont = fontFooter
                    };

                    document.Open();

                    // Header with Logo
                    string logoPath = HttpContext.Current.Server.MapPath("~/Images/logovietkitchensg.png");
                    if (File.Exists(logoPath))
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(100, 100);
                        logo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        logo.SpacingAfter = 10;
                        document.Add(logo);
                    }
                    else
                    {
                        Paragraph logoFallback = new Paragraph("NHÀ HÀNG VIET KITCHEN SÀI GÒN", fontTitle);
                        logoFallback.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        logoFallback.SpacingAfter = 10;
                        document.Add(logoFallback);
                    }

                    Paragraph pdfTitle = new Paragraph("PHIẾU XUẤT NGUYÊN LIỆU", fontTitle);
                    pdfTitle.Alignment = Element.ALIGN_CENTER;
                    pdfTitle.SpacingAfter = 10;
                    document.Add(pdfTitle);

                    LineSeparator line = new LineSeparator();
                    line.LineColor = RED_COLOR;
                    line.LineWidth = 1;
                    document.Add(new Chunk(line));
                    document.Add(Chunk.NEWLINE);

                    // Info Table
                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.SetWidths(new float[] { 50f, 50f });

                    PdfPCell cellLeft = new PdfPCell();
                    cellLeft.Border = Rectangle.NO_BORDER;
                    Paragraph paraLeft = new Paragraph();
                    paraLeft.Font = fontData;
                    paraLeft.Add($"Mã phiếu: {phieuXuat["MaPhieu"]}\n");
                    paraLeft.Add($"Thời gian tạo: {Convert.ToDateTime(phieuXuat["ThoiGianTao"]).ToString("dd/MM/yyyy HH:mm")}");
                    cellLeft.AddElement(paraLeft);

                    PdfPCell cellRight = new PdfPCell();
                    cellRight.Border = Rectangle.NO_BORDER;
                    Paragraph paraRight = new Paragraph();
                    paraRight.Font = fontData;
                    paraRight.Alignment = Element.ALIGN_RIGHT;
                    paraRight.Add($"Người tạo: {nguoiTaoName}");
                    cellRight.AddElement(paraRight);

                    infoTable.AddCell(cellLeft);
                    infoTable.AddCell(cellRight);
                    document.Add(infoTable);
                    document.Add(Chunk.NEWLINE);

                    // Details Table
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

                    cmd = new SqlCommand("SELECT c.*, n.TenNguyenLieu FROM ChiTietPhieuXuat c JOIN NguyenLieu n ON c.MaNguyenLieu = n.MaNguyenLieu WHERE c.MaPhieu = @MaPhieu", con);
                    cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    sda = new SqlDataAdapter(cmd);
                    DataTable dtDetails = new DataTable();
                    sda.Fill(dtDetails);

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

                    // Total Amount
                    Font totalFont = new Font(fontData);
                    totalFont.SetStyle(Font.BOLD);
                    totalFont.Color = RED_COLOR;
                    Paragraph paraTongThanhToan = new Paragraph($"Tổng tiền xuất: {Convert.ToDecimal(phieuXuat["TongTien"]):N0} VNĐ", totalFont);
                    paraTongThanhToan.Alignment = Element.ALIGN_RIGHT;
                    paraTongThanhToan.SpacingBefore = 10;
                    document.Add(paraTongThanhToan);

                    // Signature Table
                    document.Add(Chunk.NEWLINE);
                    PdfPTable signatureTable = new PdfPTable(2);
                    signatureTable.WidthPercentage = 100;
                    signatureTable.SetWidths(new float[] { 50f, 50f });
                    signatureTable.SpacingBefore = 20;
                    signatureTable.SpacingAfter = 20;

                    // Column 1: Nhân viên xuất hàng (Left)
                    PdfPCell employeeCell = new PdfPCell();
                    employeeCell.Border = Rectangle.NO_BORDER;
                    employeeCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    Paragraph employeePara = new Paragraph();
                    employeePara.Font = fontSignatureTitle;
                    employeePara.Add("NHÂN VIÊN XUẤT HÀNG\n");
                    employeePara.Font = fontData;
                    employeePara.Add(DateTime.Now.ToString("dd/MM/yyyy") + "\n\n\n\n\n");
                    employeePara.Add(nguoiTaoName);
                    employeeCell.AddElement(employeePara);
                    signatureTable.AddCell(employeeCell);

                    // Column 2: Xác nhận quản lý nhà hàng (Right)
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

                    // Footer
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
                    Response.AddHeader("content-disposition", $"attachment;filename=PhieuXuat_{maPhieu}.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert",
                    $"showErrorAlert('Lỗi khi xuất PDF: {ex.Message.Replace("'", "\\'")}');", true);
                System.Diagnostics.Debug.WriteLine($"Lỗi xuất PDF: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }
    }
}