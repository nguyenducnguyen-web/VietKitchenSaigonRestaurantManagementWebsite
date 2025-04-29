using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class HoaDon : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        private string customerName = "";
        private string customerPhone = "";
        private string customerEmail = "";
        private string customerAddress = "";
        private string paymentMethod = "";
        private string deliveryAddress = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["MaNguoiDung"] == null)
                {
                    Response.Redirect("DangNhap.aspx");
                }
                else if (Request.QueryString["id"] != null)
                {
                    rOrderItem.DataSource = GetOrderDetails();
                    rOrderItem.DataBind();
                }
                else
                {
                    ShowErrorAlert("Không tìm thấy hóa đơn!");
                }
            }
        }

        DataTable GetOrderDetails()
        {
            double grandTotal = 0;
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Invoice", con);
            cmd.Parameters.AddWithValue("@Action", "INVOICBYID");
            int paymentId = Convert.ToInt32(Request.QueryString["id"]);
            var userId = Session["MaNguoiDung"];
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            DataTable dtTemp = new DataTable();
            sda.Fill(dtTemp);

            dt = new DataTable();
            dt.Columns.Add("SrNo", typeof(long));
            dt.Columns.Add("OrdersNo", typeof(string));
            dt.Columns.Add("TenMonAn", typeof(string));
            dt.Columns.Add("GiaMonAnSauKhuyenMai", typeof(double));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("ThueGTGT", typeof(decimal));
            dt.Columns.Add("TotalPrice", typeof(double));

            foreach (DataRow row in dtTemp.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["SrNo"] = Convert.ToInt64(row["SrNo"]);
                newRow["OrdersNo"] = row["OrdersNo"].ToString();
                newRow["TenMonAn"] = row["TenMonAn"].ToString();
                newRow["GiaMonAnSauKhuyenMai"] = row["GiaMonAnSauKhuyenMai"] != DBNull.Value ? Convert.ToDouble(row["GiaMonAnSauKhuyenMai"]) : 0;
                newRow["SoLuong"] = row["SoLuong"] != DBNull.Value ? Convert.ToInt32(row["SoLuong"]) : 0;
                newRow["ThueGTGT"] = row["ThueGTGT"] != DBNull.Value ? Convert.ToDecimal(row["ThueGTGT"]) : 0;
                newRow["TotalPrice"] = row["TotalPrice"] != DBNull.Value ? Convert.ToDouble(row["TotalPrice"]) : 0;
                dt.Rows.Add(newRow);

                grandTotal += Convert.ToDouble(row["TotalPrice"]);
            }

            if (dt.Rows.Count > 0)
            {
                using (SqlCommand cmdUser = new SqlCommand("SELECT TenNguoiDung, SdtNguoiDung, EmailNguoiDung, DiaChiNguoiDung FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung", con))
                {
                    cmdUser.Parameters.AddWithValue("@MaNguoiDung", userId);
                    con.Open();
                    using (SqlDataReader reader = cmdUser.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customerName = reader["TenNguoiDung"].ToString();
                            customerPhone = reader["SdtNguoiDung"].ToString();
                            customerEmail = reader["EmailNguoiDung"].ToString();
                            customerAddress = reader["DiaChiNguoiDung"].ToString();
                        }
                    }
                    con.Close();
                }

                using (SqlCommand cmdPayment = new SqlCommand("SELECT PhuongThucThanhToan, DiaChi FROM ThanhToan WHERE MaThanhToan = @MaThanhToan", con))
                {
                    cmdPayment.Parameters.AddWithValue("@MaThanhToan", paymentId);
                    con.Open();
                    using (SqlDataReader reader = cmdPayment.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            paymentMethod = reader["PhuongThucThanhToan"].ToString();
                            deliveryAddress = reader["DiaChi"].ToString();
                        }
                    }
                    con.Close();
                }
            }

            if (dt.Rows.Count == 0)
            {
                ShowErrorAlert("Không tìm thấy chi tiết hóa đơn cho mã thanh toán này!");
                dt = CreateEmptyTable();
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr["SrNo"] = dt.Rows.Count + 1;
                dr["OrdersNo"] = "";
                dr["TenMonAn"] = "Tổng cộng";
                dr["GiaMonAnSauKhuyenMai"] = DBNull.Value;
                dr["SoLuong"] = DBNull.Value;
                dr["ThueGTGT"] = DBNull.Value;
                dr["TotalPrice"] = grandTotal;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        DataTable CreateEmptyTable()
        {
            dt = new DataTable();
            dt.Columns.Add("SrNo", typeof(long));
            dt.Columns.Add("OrdersNo", typeof(string));
            dt.Columns.Add("TenMonAn", typeof(string));
            dt.Columns.Add("GiaMonAnSauKhuyenMai", typeof(double));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("ThueGTGT", typeof(decimal));
            dt.Columns.Add("TotalPrice", typeof(double));
            DataRow dr = dt.NewRow();
            dr["SrNo"] = 1;
            dr["OrdersNo"] = "";
            dr["TenMonAn"] = "Không có đơn hàng";
            dr["GiaMonAnSauKhuyenMai"] = DBNull.Value;
            dr["SoLuong"] = DBNull.Value;
            dr["ThueGTGT"] = DBNull.Value;
            dr["TotalPrice"] = 0;
            dt.Rows.Add(dr);
            return dt;
        }

        protected void lbDownloadInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "order_invoice.pdf");
                DataTable dtbl = GetOrderDetails();
                ExportToPdf(dtbl, downloadPath, "HOÁ ĐƠN NHÀ HÀNG VIỆT KITCHEN SÀI GÒN");

                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=order_invoice.pdf");
                Response.TransmitFile(downloadPath);
                Response.Flush();

                ShowSuccessAlert("Hóa đơn đã được tải xuống thành công!");
                Response.End();
            }
            catch (Exception ex)
            {
                ShowErrorAlert("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }

        void ShowSuccessAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Thành công!',
                    html: '{message}',
                    icon: 'success',
                    confirmButtonText: 'Tuyệt vời!',
                    backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('successSound').play();
                        confetti({{
                            particleCount: 150,
                            spread: 90,
                            colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                            origin: {{ y: 0.6 }}
                        }});
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert", script, true);
        }

        void ShowErrorAlert(string message)
        {
            string script = $@"
                Swal.fire({{
                    title: 'Lỗi!',
                    html: '{message}',
                    icon: 'error',
                    confirmButtonText: 'Thử lại',
                    backdrop: `rgba(0, 0, 0, 0.7)`,
                    customClass: {{
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }},
                    didOpen: () => {{
                        document.getElementById('errorSound').play();
                    }}
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
        }

        void ExportToPdf(DataTable dtblTable, string strPdfPath, string strHeader)
        {
            FileStream fs = new FileStream(strPdfPath, FileMode.Create, FileAccess.Write, FileShare.None);
            Document document = new Document();
            document.SetPageSize(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            // Font Definitions with Vietnamese support
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            if (!File.Exists(fontPath))
            {
                fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            }
            BaseFont bfntHead = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            BaseFont bfntNormal = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 20, iTextSharp.text.Font.BOLD, new BaseColor(255, 111, 97));
            iTextSharp.text.Font fntSubHead = new iTextSharp.text.Font(bfntNormal, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font fntNormal = new iTextSharp.text.Font(bfntNormal, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font fntItalic = new iTextSharp.text.Font(bfntNormal, 10, iTextSharp.text.Font.ITALIC, BaseColor.GRAY);

            // Logo
            string logoPath = Server.MapPath("~/Images/logovietkitchensg.png");
            if (File.Exists(logoPath))
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(100f, 100f);
                logo.Alignment = Element.ALIGN_CENTER;
                document.Add(logo);
            }

            // Header
            Paragraph prgHeading = new Paragraph();
            prgHeading.Alignment = Element.ALIGN_CENTER;
            prgHeading.Add(new Chunk(strHeader.ToUpper(), fntHead));
            document.Add(prgHeading);

            // Author & Date
            Paragraph prgAuthor = new Paragraph();
            prgAuthor.Alignment = Element.ALIGN_RIGHT;
            prgAuthor.Add(new Chunk("Nhà hàng Viet Kitchen SG", fntItalic));
            prgAuthor.Add(new Chunk("\nNgày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fntItalic));
            document.Add(prgAuthor);

            // Customer Information
            document.Add(new Paragraph("\n"));
            Paragraph prgCustomer = new Paragraph();
            prgCustomer.Add(new Chunk("Thông tin khách hàng", fntSubHead));
            prgCustomer.SpacingAfter = 10f;
            document.Add(prgCustomer);

            PdfPTable customerTable = new PdfPTable(2);
            customerTable.WidthPercentage = 100;
            customerTable.SetWidths(new float[] { 1f, 3f });

            customerTable.AddCell(new PdfPCell(new Phrase("Tên:", fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase(customerName, fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase("Số điện thoại:", fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase(customerPhone, fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase("Email:", fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase(customerEmail, fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase("Địa chỉ:", fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase(customerAddress, fntNormal)) { Border = 0 });

            // Xử lý hiển thị phương thức thanh toán
            string displayPaymentMethod = paymentMethod;
            if (paymentMethod != null)
            {
                if (paymentMethod.ToLower() == "cod")
                {
                    displayPaymentMethod = "Thanh toán khi nhận hàng";
                }
                else if (paymentMethod.ToLower() == "card")
                {
                    displayPaymentMethod = "Thanh toán bằng thẻ tín dụng";
                }
                else if (paymentMethod.ToLower() == "qr")
                {
                    displayPaymentMethod = "Thanh toán bằng chuyển khoản ngân hàng";
                }
            }

            customerTable.AddCell(new PdfPCell(new Phrase("Phương thức thanh toán:", fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase(displayPaymentMethod, fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase("Địa chỉ nhận hàng:", fntNormal)) { Border = 0 });
            customerTable.AddCell(new PdfPCell(new Phrase(deliveryAddress, fntNormal)) { Border = 0 });

            document.Add(customerTable);

            // Separator
            document.Add(new Paragraph("\n"));
            PdfPTable separator = new PdfPTable(1);
            separator.WidthPercentage = 100;
            PdfPCell separatorCell = new PdfPCell(new Phrase(""));
            separatorCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            separatorCell.BorderColor = new BaseColor(255, 111, 97);
            separatorCell.BorderWidth = 1f;
            separator.AddCell(separatorCell);
            document.Add(separator);
            document.Add(new Paragraph("\n"));

            // Order Details Table
            Paragraph prgOrderDetails = new Paragraph();
            prgOrderDetails.Add(new Chunk("Chi tiết đơn hàng", fntSubHead));
            prgOrderDetails.SpacingAfter = 10f;
            document.Add(prgOrderDetails);

            PdfPTable table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 0.5f, 2f, 2f, 1f, 1f, 1f, 1.5f });

            // Table Header
            iTextSharp.text.Font fntColumnHeader = new iTextSharp.text.Font(bfntHead, 12, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
            string[] headers = { "STT", "Mã order", "Tên món", "Giá", "S.lượng", "Thuế GTGT", "Tổng tiền" };
            foreach (string header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fntColumnHeader));
                cell.BackgroundColor = new BaseColor(255, 0, 0);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                table.AddCell(cell);
            }

            // Table Data
            iTextSharp.text.Font fntColumnData = new iTextSharp.text.Font(bfntNormal, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            foreach (DataRow row in dtblTable.Rows)
            {
                table.AddCell(new PdfPCell(new Phrase(row["SrNo"].ToString(), fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });
                table.AddCell(new PdfPCell(new Phrase(row["OrdersNo"].ToString(), fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });
                table.AddCell(new PdfPCell(new Phrase(row["TenMonAn"].ToString(), fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });

                string price = row["GiaMonAnSauKhuyenMai"] != DBNull.Value ? String.Format("{0:N0} VND", Convert.ToDouble(row["GiaMonAnSauKhuyenMai"])) : "";
                table.AddCell(new PdfPCell(new Phrase(price, fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });

                string quantity = row["SoLuong"] != DBNull.Value ? row["SoLuong"].ToString() : "";
                table.AddCell(new PdfPCell(new Phrase(quantity, fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });

                string vat = row["ThueGTGT"] != DBNull.Value ? String.Format("{0:P0}", Convert.ToDecimal(row["ThueGTGT"])) : "";
                table.AddCell(new PdfPCell(new Phrase(vat, fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });

                string totalPrice = row["TotalPrice"] != DBNull.Value ? String.Format("{0:N0} VND", Convert.ToDouble(row["TotalPrice"])) : "";
                table.AddCell(new PdfPCell(new Phrase(totalPrice, fntColumnData)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5f });
            }

            document.Add(table);

            // Footer Note
            document.Add(new Paragraph("\n"));
            Paragraph prgFooter = new Paragraph();
            prgFooter.Alignment = Element.ALIGN_CENTER;
            prgFooter.Add(new Chunk("Cảm ơn quý khách đã ủng hộ Viet Kitchen SG!", fntItalic));
            document.Add(prgFooter);

            document.Close();
            writer.Close();
            fs.Close();
        }
    }
}