using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace WebDatDoAnOnline.Admin
{
    public static class PDFGenerator
    {
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

        public static byte[] GeneratePhieuNhapPDF(TaoPhieuNhap.PhieuNhapInfo phieuNhap)
        {
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
                    Image logo = Image.GetInstance(logoPath);
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

                // Info Table
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 50f, 50f });

                PdfPCell cellLeft = new PdfPCell();
                cellLeft.Border = Rectangle.NO_BORDER;
                Paragraph paraLeft = new Paragraph();
                paraLeft.Font = fontData;
                paraLeft.Add($"Mã phiếu: {phieuNhap.MaPhieu}\n");
                paraLeft.Add($"Thời gian tạo: {phieuNhap.ThoiGianTao:dd/MM/yyyy HH:mm}");
                cellLeft.AddElement(paraLeft);

                PdfPCell cellRight = new PdfPCell();
                cellRight.Border = Rectangle.NO_BORDER;
                Paragraph paraRight = new Paragraph();
                paraRight.Font = fontData;
                paraRight.Alignment = Element.ALIGN_RIGHT;

                // Fetch NguoiTao and NhaCungCap names
                string nguoiTaoName = phieuNhap.NguoiTao;
                string nhaCungCapName = phieuNhap.MaNhaCungCap;
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    con.Open();
                    // Get NguoiTao name
                    using (SqlCommand cmd = new SqlCommand("SELECT TenNhanVien FROM NhanVien WHERE TenDangNhapNhanVien = @TenDangNhap", con))
                    {
                        cmd.Parameters.AddWithValue("@TenDangNhap", phieuNhap.NguoiTao);
                        var result = cmd.ExecuteScalar();
                        if (result != null) nguoiTaoName = result.ToString();
                    }
                    // Get NhaCungCap name
                    using (SqlCommand cmd = new SqlCommand("SELECT TenNhaCungCap FROM NhaCungCap WHERE MaNhaCungCap = @MaNhaCungCap", con))
                    {
                        cmd.Parameters.AddWithValue("@MaNhaCungCap", phieuNhap.MaNhaCungCap);
                        var result = cmd.ExecuteScalar();
                        if (result != null) nhaCungCapName = result.ToString();
                    }
                    con.Close();
                }

                paraRight.Add($"Người tạo: {nguoiTaoName}\n");
                paraRight.Add($"Nhà cung cấp: {nhaCungCapName}");
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

                foreach (var detail in phieuNhap.Details)
                {
                    PdfPCell cell;

                    cell = new PdfPCell(new Phrase(detail.MaNguyenLieu, fontData));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(detail.TenNguyenLieu, fontData));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(detail.DonViTinh, fontData));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase($"{detail.DonGia:N0} VNĐ", fontData));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(detail.SoLuong.ToString(), fontData));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase($"{(detail.SoLuong * detail.DonGia):N0} VNĐ", fontData));
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
                Paragraph paraTongThanhToan = new Paragraph($"Tổng tiền nhập: {phieuNhap.TongTien:N0} VNĐ", totalFont);
                paraTongThanhToan.Alignment = Element.ALIGN_RIGHT;
                paraTongThanhToan.SpacingBefore = 10;
                document.Add(paraTongThanhToan);

                // Signature Table
                document.Add(Chunk.NEWLINE);
                PdfPTable signatureTable = new PdfPTable(3);
                signatureTable.WidthPercentage = 100;
                signatureTable.SetWidths(new float[] { 33f, 34f, 33f });
                signatureTable.SpacingBefore = 20;
                signatureTable.SpacingAfter = 20;

                // Column 1: Đơn vị cung cấp (Center)
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

                // Column 2: Nhân viên nhập hàng (Left)
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

                // Column 3: Xác nhận quản lý nhà hàng (Right)
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
                return ms.ToArray();
            }
        }

        public static byte[] GeneratePhieuXuatPDF(TaoPhieuXuat.PhieuXuatInfo phieuXuat)
        {
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
                    Image logo = Image.GetInstance(logoPath);
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
                paraLeft.Add($"Mã phiếu: {phieuXuat.MaPhieu}\n");
                paraLeft.Add($"Thời gian tạo: {phieuXuat.ThoiGianTao:dd/MM/yyyy HH:mm}");
                cellLeft.AddElement(paraLeft);

                PdfPCell cellRight = new PdfPCell();
                cellRight.Border = Rectangle.NO_BORDER;
                Paragraph paraRight = new Paragraph();
                paraRight.Font = fontData;
                paraRight.Alignment = Element.ALIGN_RIGHT;

                // Fetch NguoiTao name
                string nguoiTaoName = phieuXuat.NguoiTao;
                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TenNhanVien FROM NhanVien WHERE TenDangNhapNhanVien = @TenDangNhap", con))
                    {
                        cmd.Parameters.AddWithValue("@TenDangNhap", phieuXuat.NguoiTao);
                        var result = cmd.ExecuteScalar();
                        if (result != null) nguoiTaoName = result.ToString();
                    }
                    con.Close();
                }

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

                foreach (var detail in phieuXuat.Details)
                {
                    PdfPCell cell;

                    cell = new PdfPCell(new Phrase(detail.MaNguyenLieu, fontData));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(detail.TenNguyenLieu, fontData));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(detail.DonViTinh, fontData));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase($"{detail.DonGia:N0} VNĐ", fontData));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(detail.SoLuong.ToString(), fontData));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    cell.BackgroundColor = GRAY_COLOR;
                    pdfTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase($"{(detail.SoLuong * detail.DonGia):N0} VNĐ", fontData));
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
                Paragraph paraTongThanhToan = new Paragraph($"Tổng tiền xuất: {phieuXuat.TongTien:N0} VNĐ", totalFont);
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
                return ms.ToArray();
            }
        }

        private class PdfPageEventHelper : iTextSharp.text.pdf.PdfPageEventHelper
        {
            public Font FooterFont { get; set; }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfContentByte cb = writer.DirectContent;

                // Add watermark
                PdfGState gstate = new PdfGState();
                gstate.FillOpacity = 0.3f; // 30% opacity
                cb.SetGState(gstate);
                cb.BeginText();
                cb.SetFontAndSize(baseFont, 50);
                cb.SetColorFill(RED_COLOR);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "Nhà hàng Viet Kitchen Sài Gòn",
                    (document.Left + document.Right) / 2, (document.Top + document.Bottom) / 2, 45);
                cb.EndText();

                // Add page number
                Phrase footer = new Phrase($"Trang {writer.PageNumber}", FooterFont);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT, footer,
                    document.Right, document.Bottom - 10, 0);
            }
        }
    }
}

