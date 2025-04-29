<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="BangDieuKhien.aspx.cs" Inherits="WebDatDoAnOnline.Admin.BangDieuKhien" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <audio id="successSound" src="/Audio/chienthang.mp3" preload="auto"></audio>
    <audio id="errorSound" src="/Audio/loi.mp3" preload="auto"></audio>

    <style>
        body {
            font-family: 'Inter', sans-serif;
            background: #f9fafb;
            margin: 0;
            overflow-x: hidden;
        }

        .dashboard-section {
            padding: 40px 20px;
            min-height: calc(100vh - 100px);
            background: #ffffff;
        }

        .heading-container {
            text-align: center;
            margin-bottom: 40px;
        }

        .heading-container h2 {
            color: #b91c1c;
            font-size: 2.5rem;
            font-weight: 700;
            margin: 0;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 40px;
        }

        .stats-card {
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            padding: 20px;
            text-align: center;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .stats-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 25px rgba(185, 28, 28, 0.2);
        }

        .stats-card h4 {
            color: #b91c1c;
            font-size: 2rem;
            font-weight: 600;
            margin: 10px 0;
        }

        .stats-card p {
            color: #4b5563;
            font-size: 1rem;
            font-weight: 500;
            text-transform: uppercase;
        }

        .widget-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
        }

        .widget-card {
            background: linear-gradient(135deg, #b91c1c, #dc2626);
            color: #ffffff;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            position: relative;
            overflow: hidden;
        }

        .widget-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 25px rgba(185, 28, 28, 0.3);
        }

        .widget-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: radial-gradient(circle at top left, rgba(255, 255, 255, 0.2), transparent);
            opacity: 0;
            transition: opacity 0.3s ease;
        }

        .widget-card:hover::before {
            opacity: 1;
        }

        .card-icon {
            font-size: 2rem;
            margin-bottom: 10px;
            color: #ffffff;
            opacity: 0.9;
        }

        .widget-card span {
            font-size: 0.9rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .widget-card h4 {
            font-size: 1.5rem;
            font-weight: 600;
            margin: 10px 0;
        }

        .widget-card a {
            color: #ffffff;
            font-size: 0.9rem;
            font-weight: 500;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            margin-top: 10px;
            transition: color 0.3s ease;
        }

        .widget-card a:hover {
            color: #f3f4f6;
        }

        .widget-card a i {
            margin-right: 8px;
        }

        .custom-swal-popup {
            background: #b91c1c !important;
            border-radius: 16px !important;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3) !important;
        }

        .custom-swal-title {
            color: #ffffff !important;
            font-size: 1.5rem !important;
        }

        .custom-swal-text {
            color: #f3f4f6 !important;
            font-size: 1rem !important;
        }

        .custom-swal-button {
            background: #ffffff !important;
            color: #b91c1c !important;
            padding: 10px 30px !important;
            border-radius: 8px !important;
            font-weight: 600 !important;
            transition: background 0.3s ease;
        }

        .custom-swal-button:hover {
            background: #f3f4f6 !important;
        }

        @media (max-width: 768px) {
            .heading-container h2 {
                font-size: 2rem;
            }

            .stats-grid, .widget-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="dashboard-section">
        <div class="container">
            <div class="heading-container">
                <h2>Bảng Điều Khiển</h2>
            </div>

            <div class="stats-grid">
                <div class="stats-card">
                    <h4><%= Session["soldAmount"] %></h4>
                    <p>Doanh thu</p>
                </div>
                <div class="stats-card">
                    <h4><%= Session["order"] %></h4>
                    <p>Đơn hàng</p>
                </div>
                <div class="stats-card">
                    <h4><%= Session["datban"] %></h4>
                    <p>Khách đặt bàn</p>
                </div>
            </div>

            <div class="widget-grid">
                <div class="widget-card">
                    <i class="fas fa-utensils card-icon"></i>
                    <span>Loại món ăn</span>
                    <h4><%= Session["category"] %></h4>
                    <a href="QuanLyLoaiMonAn.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-hamburger card-icon"></i>
                    <span>Món ăn</span>
                    <h4><%= Session["product"] %></h4>
                    <a href="QuanLyMonAn.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-shopping-cart card-icon"></i>
                    <span>Số lượng đơn hàng</span>
                    <h4><%= Session["order"] %></h4>
                    <a href="QuanLyTrangThaiDonHang.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-truck card-icon"></i>
                    <span>Vận chuyển đơn hàng</span>
                    <h4><%= Session["delivered"] %></h4>
                    <a href="QuanLyTrangThaiDonHang.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-hourglass-half card-icon"></i>
                    <span>Đơn hàng đang xử lý</span>
                    <h4><%= Session["pending"] %></h4>
                    <a href="QuanLyTrangThaiDonHang.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-users card-icon"></i>
                    <span>Tài khoản người dùng</span>
                    <h4><%= Session["user"] %></h4>
                    <a href="QuanLyTaiKhoanNguoiDung.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-money-bill-wave card-icon"></i>
                    <span>Doanh thu</span>
                    <h4><%= Session["soldAmount"] %></h4>
                    <a href="BaoCaoBanHang.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-chair card-icon"></i>
                    <span>Khách đặt bàn</span>
                    <h4><%= Session["datban"] %></h4>
                    <a href="QuanLyDatBan.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-envelope card-icon"></i>
                    <span>Liên hệ từ khách</span>
                    <h4><%= Session["contact"] %></h4>
                    <a href="QuanLyLienHeNguoiDung.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-file-import card-icon"></i>
                    <span>Phiếu nhập hàng</span>
                    <h4><%= Session["phieuNhap"] %></h4>
                    <a href="QuanLyPhieuNhapHang.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-file-export card-icon"></i>
                    <span>Phiếu xuất hàng</span>
                    <h4><%= Session["phieuXuat"] %></h4>
                    <a href="QuanLyPhieuXuatHang.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-boxes card-icon"></i>
                    <span>Nguyên liệu</span>
                    <h4><%= Session["nguyenLieu"] %></h4>
                    <a href="QuanLyNguyenLieu.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
                <div class="widget-card">
                    <i class="fas fa-truck-loading card-icon"></i>
                    <span>Nhà cung cấp</span>
                    <h4><%= Session["nhaCungCap"] %></h4>
                    <a href="QuanLyNhaCungCap.aspx"><i class="fas fa-eye"></i>Xem chi tiết</a>
                </div>
            </div>
        </div>
    </section>
</asp:Content>