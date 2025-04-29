<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLyDatBan.aspx.cs" Inherits="WebDatDoAnOnline.Admin.QuanLyDatBan" %>
<%@ Import Namespace="WebDatDoAnOnline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js"></script>
    <audio id="successSound" src="/Audio/thanhcong.mp3" preload="auto"></audio>
    <audio id="errorSound" src="/Audio/loi.mp3" preload="auto"></audio>

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background: linear-gradient(135deg, #ff6f61, #ff0000, #ffca28);
            overflow-x: hidden;
            margin: 0;
            position: relative;
        }

        .book_section {
            padding: 80px 20px;
            background: linear-gradient(135deg, #ff6f61, #ff0000, #ffca28);
            box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.6);
            min-height: calc(100vh - 150px);
            position: relative;
            overflow: hidden;
        }

        .heading_container h2 {
            color: #ffffff;
            font-size: 3.5rem;
            text-transform: uppercase;
            text-shadow: 3px 3px 15px rgba(255, 111, 97, 0.8);
            animation: glow 2s infinite alternate;
            margin-bottom: 40px;
        }

        .sub-title {
            color: #ff6f61;
            font-size: 2rem;
            font-weight: bold;
            text-transform: uppercase;
            text-shadow: 2px 2px 10px rgba(255, 111, 97, 0.5);
            animation: glow 2s infinite alternate;
            margin-bottom: 30px;
            position: relative;
            display: inline-block;
        }

        .sub-title::before {
            content: '';
            position: absolute;
            bottom: -5px;
            left: 0;
            width: 100%;
            height: 3px;
            background: linear-gradient(45deg, #ff0000, #ffca28);
            border-radius: 2px;
        }

        .sub-title::after {
            content: '🍽️';
            position: absolute;
            right: -30px;
            top: 50%;
            transform: translateY(-50%);
            font-size: 1.5rem;
            animation: float 2s infinite ease-in-out;
        }

        .card {
            background: rgba(255, 255, 255, 0.95);
            padding: 40px;
            border-radius: 25px;
            box-shadow: 0 15px 40px rgba(255, 0, 0, 0.5);
            animation: slideInLeft 1.5s ease-in-out;
            position: relative;
            overflow: hidden;
        }

        .card::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: radial-gradient(circle, rgba(255, 202, 40, 0.3), transparent);
            animation: sparkle 5s infinite;
            pointer-events: none;
        }

        .stats-card {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            box-shadow: 0 15px 40px rgba(255, 111, 97, 0.5);
            transition: all 0.4s ease;
            animation: slideInUp 1s ease-in-out;
            padding: 20px;
            text-align: center;
        }

        .stats-card:hover {
            transform: scale(1.05) translateY(-10px);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
        }

        .stats-card h4 {
            color: #ff6f61;
            font-size: 2rem;
            font-weight: bold;
            margin: 10px 0;
            text-shadow: 2px 2px 10px rgba(255, 111, 97, 0.5);
        }

        .stats-card p {
            color: #ff0000;
            font-size: 1.2rem;
            font-weight: bold;
        }

        .stats-section {
            margin-bottom: 40px;
        }

        .form-group label {
            color: #ff6f61;
            font-weight: bold;
            font-size: 1.1rem;
        }

        .form-control {
            border: none;
            border-bottom: 3px solid #ff6f61;
            background: rgba(255, 255, 255, 0.9);
            padding: 10px;
            transition: all 0.4s ease;
            font-size: 0.9rem;
            color: #ff0000;
            border-radius: 10px;
            width: 100%;
            margin: 10px 0;
            text-align: center;
        }

        .form-control:focus {
            border-color: #ffca28;
            box-shadow: 0 5px 15px rgba(255, 202, 40, 0.5);
            background: rgba(255, 255, 255, 1);
        }

        .btn {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #fff;
            padding: 12px 30px;
            font-size: 1.1rem;
            font-weight: bold;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
            margin: 0 10px;
        }

        .btn:hover {
            transform: scale(1.1) translateY(-5px);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
        }

        .table {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.3);
        }

        .table thead {
            background: linear-gradient(45deg, #ff6f61, #ff0000);
            color: #fff;
            text-transform: uppercase;
        }

        .table th, .table td {
            padding: 15px;
            text-align: center;
            vertical-align: middle;
            border: none;
        }

        .table tbody tr {
            transition: all 0.3s ease;
        }

        .table tbody tr:hover {
            background: rgba(255, 202, 40, 0.2);
            transform: scale(1.02);
            box-shadow: 0 5px 15px rgba(255, 111, 97, 0.3);
        }

        .badge {
            padding: 8px 15px;
            font-size: 1rem;
            border-radius: 20px;
            font-weight: bold;
            transition: transform 0.3s ease;
        }

        .badge:hover {
            transform: scale(1.1);
        }

        .badge-danger {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            box-shadow: 0 5px 15px rgba(255, 0, 0, 0.5);
        }

        .badge-danger:hover {
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }

        .badge-info {
            background: linear-gradient(45deg, #ff6f61, #ffca28);
            color: #ffffff;
            box-shadow: 0 5px 15px rgba(255, 202, 40, 0.5);
        }

        .badge-info:hover {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
        }

        .modal-content {
            background: linear-gradient(135deg, #ff6f61, #ffca28);
            border-radius: 20px;
            box-shadow: 0 10px 30px rgba(255, 111, 97, 0.8);
            color: #ffffff;
        }

        .modal-header, .modal-footer {
            border: none;
        }

        .modal-title {
            color: #ffffff;
            font-size: 1.8rem;
            text-shadow: 2px 2px 10px rgba(255, 255, 255, 0.5);
        }

        .modal-body p {
            color: #ffffff;
            font-size: 1.1rem;
        }

        .custom-swal-popup {
            background: linear-gradient(135deg, #ff6f61, #ff0000) !important;
            border-radius: 20px !important;
            box-shadow: 0 10px 30px rgba(255, 111, 97, 0.8) !important;
            width: 500px !important;
        }

        .custom-swal-title {
            color: #ffffff !important;
            font-size: 1.8rem !important;
            text-shadow: 2px 2px 10px rgba(255, 255, 255, 0.5) !important;
        }

        .custom-swal-text {
            color: #ffffff !important;
            font-size: 1.1rem !important;
        }

        .custom-swal-button {
            background: #ff0000 !important;
            color: #ffffff !important;
            padding: 12px 40px !important;
            border-radius: 25px !important;
            font-weight: bold !important;
            box-shadow: 0 5px 15px rgba(255, 0, 0, 0.6) !important;
        }

        .custom-swal-button:hover {
            background: #ff3333 !important;
            transform: scale(1.1) !important;
        }

        .particles {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 0;
            pointer-events: none;
        }

        /*@keyframes glow {
            0% { text-shadow: 0 0 10px #ff6f61; }
            100% { text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000; }
        }

        @keyframes sparkle {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        @keyframes slideInLeft {
            from { transform: translateX(-100px); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }

        @keyframes slideInUp {
            from { transform: translateY(100px); opacity: 0; }
            to { transform: translateY(0); opacity: 1; }
        }

        @keyframes float {
            0%, 100% { transform: translateY(-50%) translateX(0); }
            50% { transform: translateY(-50%) translateX(10px); }
        }*/
    </style>

    <script>
        window.onload = function () {
            particlesJS("particles-js", {
                "particles": {
                    "number": { "value": 200, "density": { "enable": true, "value_area": 800 } },
                    "color": { "value": ["#ff6f61", "#ff0000", "#ffca28"] },
                    "shape": { "type": "star" },
                    "opacity": { "value": 0.8, "random": true, "anim": { "enable": true, "speed": 1, "opacity_min": 0.1 } },
                    "size": { "value": 3, "random": true, "anim": { "enable": true, "speed": 3, "size_min": 0.5 } },
                    "move": { "enable": true, "speed": 2, "direction": "none", "random": true, "out_mode": "out" }
                },
                "interactivity": {
                    "events": { "onhover": { "enable": true, "mode": "repulse" }, "onclick": { "enable": true, "mode": "push" } },
                    "modes": { "repulse": { "distance": 100 }, "push": { "particles_nb": 10 } }
                },
                "retina_detect": true
            });

            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%= lblMsg.ClientID %>").style.display = "none";
            }, seconds * 1000);

            // Auto-trigger search on input for txtSearchName
            document.getElementById("<%= txtSearchName.ClientID %>").addEventListener("input", function () {
                __doPostBack("<%= txtSearchName.UniqueID %>", "");
            });

            // Auto-trigger search on input for txtSearchPhone
            document.getElementById("<%= txtSearchPhone.ClientID %>").addEventListener("input", function () {
                __doPostBack("<%= txtSearchPhone.UniqueID %>", "");
            });

            // Auto-trigger search on input for txtSearchEmail
            document.getElementById("<%= txtSearchEmail.ClientID %>").addEventListener("input", function () {
                __doPostBack("<%= txtSearchEmail.UniqueID %>", "");
            });
        };

        function showSuccessAlert(message) {
            Swal.fire({
                title: 'Thành công!',
                html: message,
                icon: 'success',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                },
                backdrop: `rgba(0, 0, 0, 0.7) url('https://sweetalert2.github.io/images/nyan-cat.gif') left top no-repeat`,
                didOpen: () => {
                    document.getElementById('successSound').play();
                    confetti({
                        particleCount: 200,
                        spread: 120,
                        colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                        origin: { y: 0.6 }
                    });
                }
            });
        }

        function showErrorAlert(message) {
            Swal.fire({
                title: 'Lỗi!',
                html: message,
                icon: 'error',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                },
                backdrop: `rgba(0, 0, 0, 0.7) url('https://sweetalert2.github.io/images/nyan-cat.gif') left top no-repeat`,
                didOpen: () => {
                    document.getElementById('errorSound').play();
                }
            });
        }

        function showBookingDetails(bookingId) {
            fetch('QuanLyDatBan.aspx/GetBookingDetails', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({ bookingId: bookingId })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        document.getElementById('bookingDetailsContent').innerHTML = data.d;
                        $('#bookingDetailsModal').modal('show');
                    } else {
                        showErrorAlert('Không thể tải chi tiết đặt bàn!');
                    }
                })
                .catch(error => {
                    showErrorAlert('Lỗi: ' + error.message);
                });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>

            <div class="heading_container text-center">
                <h2>Quản Lý Đặt Bàn</h2>
            </div>

            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="align-align-self-end text-center mb-4">
                                <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert"></asp:Label>
                            </div>

                            <!-- Phần thống kê nhanh -->
                            <div class="stats-section">
                                <div class="row">
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="todayBookingsCount" runat="server">0</h4>
                                            <p>Đặt bàn hôm nay</p>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="totalBookingsCount" runat="server">0</h4>
                                            <p>Tổng số đặt bàn</p>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="averageGuestsCount" runat="server">0</h4>
                                            <p>Số khách trung bình</p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <!-- Thanh công cụ tìm kiếm -->
                                <div class="col-sm-12 col-md-12 col-lg-12">
                                    <h4 class="sub-title">Danh sách đặt bàn</h4>
                                    <div class="row mb-3">
                                        <div class="col-md-4">
                                            <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" placeholder="Tìm kiếm theo tên khách hàng" AutoPostBack="true" OnTextChanged="txtSearchName_TextChanged"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:TextBox ID="txtSearchPhone" runat="server" CssClass="form-control" placeholder="Tìm kiếm theo số điện thoại" AutoPostBack="true" OnTextChanged="txtSearchPhone_TextChanged"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:TextBox ID="txtSearchEmail" runat="server" CssClass="form-control" placeholder="Tìm kiếm theo email" AutoPostBack="true" OnTextChanged="txtSearchEmail_TextChanged"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:Repeater ID="rBookings" runat="server" OnItemCommand="rBookings_ItemCommand">
                                            <HeaderTemplate>
                                                <table class="table data-table-export table-hover nowrap">
                                                    <thead>
                                                        <tr>
                                                            <th>STT</th>
                                                            <th class="table-plus">Tên khách hàng</th>
                                                            <th>Số điện thoại</th>
                                                            <th>Email</th>
                                                            <th>Số lượng khách</th>
                                                            <th>Ngày đặt bàn</th>
                                                            <th class="datatable-nosort">Thao tác</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Container.ItemIndex + 1 %></td>
                                                    <td class="table-plus"><%# Eval("TenKhachHang") %></td>
                                                    <td><%# Eval("SdtDatBan") %></td>
                                                    <td><%# Eval("EmailDatBan") %></td>
                                                    <td><%# Eval("SoLuongKhach") %></td>
                                                    <td><%# Eval("NgayDatBan") %></td>
                                                    <td>
                                                        <a href="javascript:void(0)" onclick="showBookingDetails('<%# Eval("MaDatBan") %>')" class="badge badge-info">
                                                            <i class="ti-eye"></i> Xem chi tiết
                                                        </a>
                                                        <asp:LinkButton ID="lnkDelete" Text="Xoá" runat="server" CommandName="delete"
                                                            CssClass="badge badge-danger" CommandArgument='<%# Eval("MaDatBan") %>'
                                                            OnClientClick="return confirm('Bạn có muốn xóa thông tin đặt bàn này không?');">
                                                            <i class="ti-trash"></i>
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </tbody>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Modal chi tiết đặt bàn -->
    <div class="modal fade" id="bookingDetailsModal" tabindex="-1" role="dialog" aria-labelledby="bookingDetailsModalLabel" aria-hidden="true">
       （如: 1, 2, 3, ...）
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="bookingDetailsModalLabel">Chi tiết đặt bàn</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body" id="bookingDetailsContent">
                    <!-- Nội dung chi tiết đặt bàn sẽ được tải động -->
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-dismiss="modal">Đóng</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>