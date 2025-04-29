<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="CapNhatPhieuNhap.aspx.cs" Inherits="WebDatDoAnOnline.Admin.CapNhatPhieuNhap" %>
<%@ Import Namespace="WebDatDoAnOnline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js"></script>
    <audio id="successSound" src="/Audio/chienthang.mp3" preload="auto"></audio>
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
            content: '📦';
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

        select.form-control {
            appearance: none;
            background: rgba(255, 255, 255, 0.9) url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 24 24"><path fill="%23ff6f61" d="M7 10l5 5 5-5z"/></svg>') no-repeat right 10px center;
            padding-right: 30px;
            text-align: center;
            text-align-last: center;
            color: #ff6f61;
            font-size: 0.9rem;
            height: 40px;
        }

        select.form-control option {
            text-align: center;
            color: #ff0000;
            font-size: 0.9rem;
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
            0% {
                text-shadow: 0 0 10px #ff6f61;
            }
            100% {
                text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000;
            }
        }

        @keyframes sparkle {
            0% {
                transform: rotate(0deg);
            }
            100% {
                transform: rotate(360deg);
            }
        }

        @keyframes slideInLeft {
            from {
                transform: translateX(-100px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        @keyframes float {
            0%, 100% {
                transform: translateY(-50%) translateX(0);
            }
            50% {
                transform: translateY(-50%) translateX(10px);
            }
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

        function calculateTotalPrice() {
            var soLuong = parseFloat(document.getElementById('<%= txtQuantity.ClientID %>').value) || 0;
            var donGia = parseFloat(document.getElementById('<%= txtUnitPrice.ClientID %>').value) || 0;
            var thanhTien = soLuong * donGia;
            document.getElementById('<%= txtTotalPrice.ClientID %>').value = thanhTien.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>

            <div class="heading_container text-center">
                <h2>Cập Nhật Phiếu Nhập Hàng</h2>
            </div>

            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="align-align-self-end text-center mb-4">
                                <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert"></asp:Label>
                            </div>

                            <!-- Form Thông tin phiếu nhập -->
                            <h4 class="sub-title">Thông tin phiếu nhập</h4>
                            <div class="row mb-4">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Mã phiếu nhập</label>
                                        <asp:TextBox ID="txtMaPhieu" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Nhà cung cấp</label>
                                        <asp:DropDownList ID="ddlNhaCungCap" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <!-- Form Thêm chi tiết nguyên liệu -->
                            <h4 class="sub-title">Thêm chi tiết nguyên liệu</h4>
                            <div class="row mb-4">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Nguyên liệu</label>
                                        <asp:DropDownList ID="ddlNguyenLieu" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlNguyenLieu_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label>Đơn vị tính</label>
                                        <asp:TextBox ID="txtDonViTinh" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label>Số lượng</label>
                                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number" oninput="calculateTotalPrice()"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label>Đơn giá (VND)</label>
                                        <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" TextMode="Number" oninput="calculateTotalPrice()"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label>Thành tiền</label>
                                        <asp:TextBox ID="txtTotalPrice" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="text-center mb-4">
                                <asp:Button ID="btnAddDetail" runat="server" Text="Thêm chi tiết" CssClass="btn" OnClick="btnAddDetail_Click" />
                            </div>

                            <!-- Danh sách chi tiết phiếu nhập -->
                            <h4 class="sub-title">Danh sách chi tiết phiếu nhập</h4>
                            <div class="table-responsive">
                                <asp:Repeater ID="rChiTietPhieuNhap" runat="server" OnItemCommand="rChiTietPhieuNhap_ItemCommand">
                                    <HeaderTemplate>
                                        <table class="table data-table-export table-hover nowrap">
                                            <thead>
                                                <tr>
                                                    <th>STT</th>
                                                    <th>Mã nguyên liệu</th>
                                                    <th>Tên nguyên liệu</th>
                                                    <th>Đơn vị tính</th>
                                                    <th>Số lượng</th>
                                                    <th>Đơn giá</th>
                                                    <th>Thành tiền</th>
                                                    <th>Thao tác</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><%# Eval("MaNguyenLieu") %></td>
                                            <td><%# Eval("TenNguyenLieu") %></td>
                                            <td><%# Eval("DonViTinh") %></td>
                                            <td><%# Eval("SoLuong") %></td>
                                            <td><%# Eval("DonGia", "{0:N0} VND") %></td>
                                            <td><%# Eval("ThanhTien", "{0:N0} VND") %></td>
                                            <td>
                                                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="badge badge-primary"
                                                    CommandArgument='<%# Eval("MaNguyenLieu") %>' CommandName="delete">
                                                    <i class="ti-trash"></i> Xóa
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

                            <!-- Tổng tiền -->
                            <div class="text-right mt-4">
                                <h4>Tổng tiền: <asp:Label ID="lblTongTien" runat="server" Text="0 VND"></asp:Label></h4>
                            </div>

                            <!-- Nút thao tác -->
                            <div class="text-center mt-4">
                                <asp:Button ID="btnSave" runat="server" Text="Lưu phiếu nhập" CssClass="btn" OnClick="btnSave_Click" />
                                <asp:Button ID="btnExportPDF" runat="server" Text="Xuất PDF" CssClass="btn" OnClick="btnExportPDF_Click" />
                                <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn" OnClick="btnCancel_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>