using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Text;

namespace WebDatDoAnOnline.Admin
{
    public partial class QuanLyBaoCao : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        // Định nghĩa font và màu sắc
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
                LoadReportList();
            }
        }

        private void LoadReportList()
        {
            var reports = new List<ReportInfo>
            {
                new ReportInfo { ReportID = 1, Title = "Thống kê khách hàng và số lượng đơn hàng" },
                new ReportInfo { ReportID = 2, Title = "Thống kê 10 đơn hàng giá trị cao nhất trong tháng qua" },
                new ReportInfo { ReportID = 3, Title = "Thống kê món ăn bán chạy nhất trong năm qua" },
                new ReportInfo { ReportID = 4, Title = "Thống kê nguyên liệu nhập nhiều nhất trong 3 tháng qua" },
                new ReportInfo { ReportID = 5, Title = "Thống kê tổng chi phí nhập theo nhà cung cấp trong năm" },
                new ReportInfo { ReportID = 6, Title = "Báo cáo tồn kho" },
                new ReportInfo { ReportID = 7, Title = "Thống kê tồn kho nguyên liệu dưới ngưỡng (100)" },
                new ReportInfo { ReportID = 8, Title = "Thống kê số lượng đặt bàn theo ngày trong tháng hiện tại" },
                new ReportInfo { ReportID = 9, Title = "Phân tích phương thức thanh toán phổ biến nhất" },
                new ReportInfo { ReportID = 10, Title = "Thống kê doanh thu và chi phí theo tháng" }
            };

            rptReports.DataSource = reports;
            rptReports.DataBind();
        }

        protected void rptReports_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ExportPDF")
            {
                int reportId = int.Parse(e.CommandArgument.ToString());
                ExportReportToPDF(reportId);
            }
            else if (e.CommandName == "ViewDetails")
            {
                int reportId = int.Parse(e.CommandArgument.ToString());
                ShowReportDetails(reportId);
            }
        }


        private void ShowReportDetails(int reportId)
        {
            DataTable data = GetData(reportId);
            if (data != null && data.Rows.Count > 0)
            {
                string reportTitle = GetReportTitle(reportId);
                string nguoiTaoName = "Admin"; // Có thể thay bằng Session["UserName"]
                StringBuilder html = new StringBuilder();

                // Logo
                string logoPath = Server.MapPath("~/Images/logovietkitchensg.png");
                if (File.Exists(logoPath))
                {
                    html.Append("<div class='logo'><img src='../Images/logovietkitchensg.png' alt='Logo' /></div>");
                }
                else
                {
                    html.Append("<div class='logo' style='font-size: 25px; font-weight: bold; color: #c8102e; text-align: center;'>NHÀ HÀNG VIET KITCHEN SÀI GÒN</div>");
                }

                // Tiêu đề
                html.Append($"<div class='pdf-title'>{Server.HtmlEncode(reportTitle.ToUpper())}</div>");
                html.Append("<div class='line-separator'></div>");

                // Thông tin báo cáo
                html.Append("<table class='info-table'>");
                html.Append("<tr>");
                html.Append($"<td class='left'>Mã báo cáo: BC{reportId}<br />Thời gian tạo: {DateTime.Now:dd/MM/yyyy HH:mm}</td>");
                html.Append($"<td class='right'>Người tạo: {Server.HtmlEncode(nguoiTaoName)}</td>");
                html.Append("</tr>");
                html.Append("</table>");

                // Danh sách các cột liên quan đến tiền
                var moneyColumns = new List<string>
                {
                    "Giá món ăn", "Tổng chi tiêu", "Tổng giá trị", "Đơn giá",
                    "Tổng chi phí", "Doanh thu", "Tổng doanh thu", "Tổng chi phí nhập",
                    "Lợi nhuận", "Giá trị trung bình"
                };

                // Bảng dữ liệu
                html.Append("<table class='data-table'>");
                html.Append("<tr>");
                foreach (DataColumn column in data.Columns)
                {
                    html.Append($"<th>{Server.HtmlEncode(column.ColumnName)}</th>");
                }
                html.Append("</tr>");
                foreach (DataRow row in data.Rows)
                {
                    html.Append("<tr>");
                    foreach (DataColumn column in data.Columns)
                    {
                        string cellValue = row[column].ToString();
                        if (moneyColumns.Contains(column.ColumnName) && decimal.TryParse(cellValue, out decimal moneyValue))
                        {
                            html.Append($"<td>{Server.HtmlEncode($"{moneyValue:N0} VNĐ")}</td>");
                        }
                        else
                        {
                            html.Append($"<td>{Server.HtmlEncode(cellValue)}</td>");
                        }
                    }
                    html.Append("</tr>");
                }
                html.Append("</table>");

                // Chữ ký
                html.Append("<table class='signature-table'>");
                html.Append("<tr>");
                html.Append("<td class='left signature-title'>NGƯỜI TẠO BÁO CÁO</td>");
                html.Append("<td class='right signature-title'>XÁC NHẬN QUẢN LÝ</td>");
                html.Append("</tr>");
                html.Append("<tr>");
                html.Append($"<td class='left'>{DateTime.Now:dd/MM/yyyy}<br /><div class='space'></div>{Server.HtmlEncode(nguoiTaoName)}</td>");
                html.Append($"<td class='right'>{DateTime.Now:dd/MM/yyyy}<br /><div class='space'></div>Nguyễn Thanh Trường</td>");
                html.Append("</tr>");
                html.Append("</table>");

                litReportDetails.Text = html.ToString();
                pnlReportDetails.Visible = true;
            }
            else
            {
                litReportDetails.Text = "<p>Không có dữ liệu</p>";
                pnlReportDetails.Visible = true;
            }
        }

        private void ExportReportToPDF(int reportId)
        {
            DataTable data = GetData(reportId);
            if (data == null || data.Rows.Count == 0)
            {
                Response.Write("<script>alert('Không có dữ liệu để xuất báo cáo!');</script>");
                return;
            }

            string reportTitle = GetReportTitle(reportId);
            byte[] pdfBytes = GeneratePDF(data, reportTitle, reportId);

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", $"attachment; filename=Report_{reportId}.pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();
        }


        private byte[] GeneratePDF(DataTable data, string reportTitle, int reportId)
        {
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

                Paragraph pdfTitle = new Paragraph(reportTitle.ToUpper(), fontTitle);
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
                paraLeft.Add($"Mã báo cáo: BC{reportId}\n");
                paraLeft.Add($"Thời gian tạo: {DateTime.Now:dd/MM/yyyy HH:mm}");
                cellLeft.AddElement(paraLeft);

                PdfPCell cellRight = new PdfPCell();
                cellRight.Border = Rectangle.NO_BORDER;
                Paragraph paraRight = new Paragraph();
                paraRight.Font = fontData;
                paraRight.Alignment = Element.ALIGN_RIGHT;
                string nguoiTaoName = "Admin";
                paraRight.Add($"Người tạo: {nguoiTaoName}");
                cellRight.AddElement(paraRight);

                infoTable.AddCell(cellLeft);
                infoTable.AddCell(cellRight);
                document.Add(infoTable);
                document.Add(Chunk.NEWLINE);

                PdfPTable pdfTable = new PdfPTable(data.Columns.Count);
                pdfTable.WidthPercentage = 100;
                float[] widths = new float[data.Columns.Count];
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    widths[i] = 100f / data.Columns.Count;
                }
                pdfTable.SetWidths(widths);

                // Danh sách các cột liên quan đến tiền
                var moneyColumns = new List<string>
                {
                    "Giá món ăn", "Tổng chi tiêu", "Tổng giá trị", "Đơn giá",
                    "Tổng chi phí", "Doanh thu", "Tổng doanh thu", "Tổng chi phí nhập",
                    "Lợi nhuận", "Giá trị trung bình"
                };

                foreach (DataColumn column in data.Columns)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(column.ColumnName, fontHeader));
                    headerCell.BackgroundColor = RED_COLOR;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerCell.Padding = 8;
                    headerCell.BorderColor = RED_COLOR;
                    pdfTable.AddCell(headerCell);
                }

                foreach (DataRow row in data.Rows)
                {
                    foreach (DataColumn column in data.Columns)
                    {
                        string cellValue = row[column].ToString();
                        if (moneyColumns.Contains(column.ColumnName) && decimal.TryParse(cellValue, out decimal moneyValue))
                        {
                            PdfPCell cell = new PdfPCell(new Phrase($"{moneyValue:N0} VNĐ", fontData));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.Padding = 5;
                            cell.BackgroundColor = GRAY_COLOR;
                            pdfTable.AddCell(cell);
                        }
                        else
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(cellValue, fontData));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.Padding = 5;
                            cell.BackgroundColor = GRAY_COLOR;
                            pdfTable.AddCell(cell);
                        }
                    }
                }

                document.Add(pdfTable);
                document.Add(Chunk.NEWLINE);

                PdfPTable signatureTable = new PdfPTable(2);
                signatureTable.WidthPercentage = 100;
                signatureTable.SetWidths(new float[] { 50f, 50f });
                signatureTable.SpacingBefore = 20;
                signatureTable.SpacingAfter = 20;

                PdfPCell employeeCell = new PdfPCell();
                employeeCell.Border = Rectangle.NO_BORDER;
                employeeCell.HorizontalAlignment = Element.ALIGN_LEFT;
                Paragraph employeePara = new Paragraph();
                employeePara.Font = fontSignatureTitle;
                employeePara.Add("NGƯỜI TẠO BÁO CÁO\n");
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
                managerPara.Add("XÁC NHẬN QUẢN LÝ\n");
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
                return ms.ToArray();
            }
        }

        private DataTable GetData(int reportId)
        {
            dt = new DataTable();
            string query = "";

            switch (reportId)
            {
                case 1:
                    query = @"SELECT TOP 100 WITH TIES
                                nd.TenNguoiDung AS N'Tên người dùng',
                                nd.TenDangNhapNguoiDung AS N'User name',
                                nd.SdtNguoiDung AS N'Số điện thoại',
                                COUNT(o.MaOrders) AS N'Số đơn hàng',
                                SUM(o.SoLuong * ma.GiaMonAn) AS N'Tổng chi tiêu'
                            FROM NguoiDung nd
                            JOIN Orders o ON nd.MaNguoiDung = o.MaNguoiDung
                            JOIN MonAn ma ON ma.MaMonAn = o.MaMonAn
                            GROUP BY nd.TenNguoiDung, nd.TenDangNhapNguoiDung, nd.SdtNguoiDung
                            ORDER BY COUNT(o.MaOrders) DESC";
                    break;
                case 2:
                    query = @"SELECT TOP 10 WITH TIES
                                o.OrdersNo AS N'Mã đơn hàng',
                                nd.TenNguoiDung AS N'Tên khách hàng',
                                nd.SdtNguoiDung AS N'Số điện thoại',
                                tt.PhuongThucThanhToan AS N'Phương thức thanh toán',
                                SUM(o.SoLuong * o.GiaMonAnSauKhuyenMai) AS N'Tổng giá trị'
                            FROM Orders o
                            JOIN NguoiDung nd ON o.MaNguoiDung = nd.MaNguoiDung
                            JOIN ThanhToan tt ON o.MaThanhToan=tt.MaThanhToan 
                            WHERE o.NgayTaoOrders >= DATEADD(MONTH, -1, GETDATE())
                            GROUP BY o.OrdersNo, nd.TenNguoiDung, nd.SdtNguoiDung, tt.PhuongThucThanhToan
                            ORDER BY SUM(o.SoLuong * o.GiaMonAnSauKhuyenMai) DESC";
                    break;
                case 3:
                    query = @"SELECT TOP 100 WITH TIES
                                ma.TenMonAn AS N'Tên món ăn',
                                ma.GiaMonAn AS N'Giá món ăn',
                                SUM(o.SoLuong) AS N'Tổng số lượng đã bán',
                                SUM(ma.GiaMonAn * o.SoLuong) AS N'Doanh thu'
                            FROM Orders o
                            JOIN MonAn ma ON o.MaMonAn = ma.MaMonAn
                            WHERE o.NgayTaoOrders >= DATEADD(YEAR, -1, GETDATE())
                            GROUP BY ma.TenMonAn, ma.GiaMonAn
                            ORDER BY SUM(ma.GiaMonAn * o.SoLuong) DESC";
                    break;
                case 4:
                    query = @"SELECT TOP 100 WITH TIES
                                nl.TenNguyenLieu AS N'Tên nguyên liệu',
                                ctpn.DonGia AS N'Đơn giá',
                                nl.DonViTinh AS N'Đơn vị tính',
                                SUM(ctpn.SoLuong) AS N'Tổng số lượng nhập',
                                SUM(ctpn.SoLuong * ctpn.DonGia) AS N'Tổng chi phí'
                            FROM NguyenLieu nl
                            JOIN ChiTietPhieuNhap ctpn ON nl.MaNguyenLieu = ctpn.MaNguyenLieu
                            JOIN PhieuNhap pn ON ctpn.MaPhieu = pn.MaPhieu
                            WHERE pn.ThoiGianTao >= DATEADD(MONTH, -3, GETDATE())
                            GROUP BY nl.TenNguyenLieu, ctpn.DonGia, nl.DonViTinh
                            ORDER BY SUM(ctpn.SoLuong * ctpn.DonGia) DESC";
                    break;
                case 5:
                    query = @"SELECT 
                                ncc.MaNhaCungCap AS N'Mã nhà cung cấp',
                                ncc.TenNhaCungCap AS N'Tên nhà cung cấp',
                                ncc.DiaChi AS N'Địa chỉ',
                                ncc.Sdt AS N'Số điện thoại',
                                COUNT(pn.MaPhieu) AS N'Tổng số phiếu nhập',
                                SUM(pn.TongTien) AS N'Tổng chi phí nhập'
                            FROM NhaCungCap ncc
                            JOIN PhieuNhap pn ON ncc.MaNhaCungCap = pn.MaNhaCungCap
                            JOIN ChiTietPhieuNhap ctpn ON pn.MaPhieu = ctpn.MaPhieu
                            WHERE YEAR(pn.ThoiGianTao) = YEAR(GETDATE())
                            GROUP BY ncc.MaNhaCungCap, ncc.TenNhaCungCap, ncc.DiaChi, ncc.Sdt
                            ORDER BY SUM(pn.TongTien) DESC";
                    break;
                case 6:
                    query = @"SELECT 
                                nl.MaNguyenLieu N'Mã nguyên liệu',
                                nl.TenNguyenLieu N'Tên nguyên liệu',
                                nl.DonViTinh AS N'Đơn vị tính',
                                COALESCE(SUM(ctpn.SoLuong), 0) AS N'Số lượng nhập',
                                COALESCE(SUM(ctpx.SoLuong), 0) AS N'Số lượng xuất',
                                nl.SoLuongTon N'Số lượng tồn'
                            FROM NguyenLieu nl
                            LEFT JOIN ChiTietPhieuNhap ctpn ON nl.MaNguyenLieu = ctpn.MaNguyenLieu
                            LEFT JOIN ChiTietPhieuXuat ctpx ON nl.MaNguyenLieu = ctpx.MaNguyenLieu
                            GROUP BY nl.MaNguyenLieu, nl.TenNguyenLieu, nl.DonViTinh, nl.SoLuongTon
                            ORDER BY nl.MaNguyenLieu";
                    break;
                case 7:
                    query = @"SELECT 
                                nl.MaNguyenLieu AS N'Mã nguyên liệu',
                                nl.TenNguyenLieu AS N'Tên nguyên liệu',
                                nl.SoLuongTon AS N'Số lượng tồn',
                                nl.DonViTinh AS N'Đơn vị tính'
                            FROM NguyenLieu nl
                            WHERE nl.SoLuongTon < 100
                            ORDER BY nl.SoLuongTon ASC";
                    break;
                case 8:
                    query = @"SELECT 
                                db.TenKhachHang AS N'Tên khách hàng',
                                db.SdtDatBan AS N'Số điện thoại',
                                db.EmailDatBan AS N'Email',
                                CONVERT(DATE, db.NgayDatBan) AS N'Ngày',
                                SUM(db.SoLuongKhach) AS N'Tổng số khách'
                            FROM DatBan db
                            WHERE YEAR(db.NgayDatBan) = YEAR(GETDATE())
                            GROUP BY CONVERT(DATE, db.NgayDatBan), db.TenKhachHang, db.SdtDatBan, db.EmailDatBan
                            ORDER BY CONVERT(DATE, db.NgayDatBan)";
                    break;
                case 9:
                    query = @"SELECT 
                                tt.PhuongThucThanhToan AS N'Phương thức thanh toán',
                                COUNT(tt.MaThanhToan) AS N'Số lượng giao dịch',
                                ROUND(AVG(o.GiaMonAnSauKhuyenMai * o.SoLuong), 2) AS N'Giá trị trung bình'
                            FROM ThanhToan tt
                            JOIN Orders o ON tt.MaThanhToan = o.MaThanhToan
                            GROUP BY tt.PhuongThucThanhToan
                            ORDER BY COUNT(tt.MaThanhToan) DESC";
                    break;
                case 10:
                    query = @"WITH ChiPhi AS (
                                SELECT 
                                    MONTH(pn.ThoiGianTao) AS Thang,
                                    YEAR(pn.ThoiGianTao) AS Nam,
                                    SUM(ctpn.SoLuong * ctpn.DonGia) AS TongChiPhi
                                FROM PhieuNhap pn
                                JOIN ChiTietPhieuNhap ctpn ON pn.MaPhieu = ctpn.MaPhieu
                                GROUP BY YEAR(pn.ThoiGianTao), MONTH(pn.ThoiGianTao)
                            ),
                            DoanhThu AS (
                                SELECT
                                    MONTH(o.NgayTaoOrders) AS Thang,
                                    YEAR(o.NgayTaoOrders) AS Nam,
                                    SUM((ma.GiaMonAn) * o.SoLuong) AS TongDoanhThu
                                FROM Orders o
                                JOIN MonAn ma ON o.MaMonAn = ma.MaMonAn
                                GROUP BY YEAR(o.NgayTaoOrders), MONTH(o.NgayTaoOrders)
                            )
                            SELECT 
                                COALESCE(cp.Nam, dt.Nam) AS N'Năm',
                                COALESCE(cp.Thang, dt.Thang) AS N'Tháng',
                                ISNULL(dt.TongDoanhThu, 0) AS N'Tổng doanh thu',
                                ISNULL(cp.TongChiPhi, 0) AS N'Tổng chi phí',
                                ISNULL(dt.TongDoanhThu, 0) - ISNULL(cp.TongChiPhi, 0) AS N'Lợi nhuận'
                            FROM ChiPhi cp
                            FULL OUTER JOIN DoanhThu dt 
                                ON cp.Nam = dt.Nam AND cp.Thang = dt.Thang
                            ORDER BY COALESCE(cp.Nam, dt.Nam) DESC, COALESCE(cp.Thang, dt.Thang) DESC";
                    break;
            }

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand(query, con);
            sda = new SqlDataAdapter(cmd);
            try
            {
                con.Open();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Lỗi kết nối cơ sở dữ liệu: {ex.Message}');</script>");
                return null;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        private string GetReportTitle(int reportId)
        {
            switch (reportId)
            {
                case 1: return "Thống kê khách hàng và số lượng đơn hàng";
                case 2: return "Thống kê 10 đơn hàng giá trị cao nhất trong tháng qua";
                case 3: return "Thống kê món ăn bán chạy nhất trong năm qua";
                case 4: return "Thống kê nguyên liệu nhập nhiều nhất trong 3 tháng qua";
                case 5: return "Thống kê tổng chi phí nhập theo nhà cung cấp trong năm";
                case 6: return "Báo cáo tồn kho";
                case 7: return "Thống kê tồn kho nguyên liệu dưới ngưỡng (100)";
                case 8: return "Thống kê số lượng đặt bàn theo ngày trong tháng hiện tại";
                case 9: return "Phân tích phương thức thanh toán phổ biến nhất";
                case 10: return "Thống kê doanh thu và chi phí theo tháng";
                default: return "Báo cáo";
            }
        }
    }

    public class ReportInfo
    {
        public int ReportID { get; set; }
        public string Title { get; set; }
    }

    public class PdfPageEventHelper : iTextSharp.text.pdf.PdfPageEventHelper
    {
        public Font FooterFont { get; set; }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            PdfPCell cell = new PdfPCell(new Phrase($"Trang {writer.PageNumber}", FooterFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            footerTable.AddCell(cell);
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 10, writer.DirectContent);
        }
    }
}