<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLyTrangThaiDonHang.aspx.cs" Inherits="WebDatDoAnOnline.Admin.QuanLyTrangThaiDonHang" %>
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

        .badge-success {
            background: #00ff00;
            color: #ffffff;
        }

        .badge-warning {
            background: #ffca28;
            color: #ffffff;
        }

        .badge-primary {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            box-shadow: 0 5px 15px rgba(255, 0, 0, 0.5);
        }

        .badge-primary:hover {
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

        @keyframes slideInUp {
            from {
                transform: translateY(100px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
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

            document.getElementById("<%= txtSearch.ClientID %>").addEventListener("input", function () {
                 __doPostBack("<%= txtSearch.UniqueID %>", "");
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

        function showOrderDetails(orderId) {
            fetch('QuanLyTrangThaiDonHang.aspx/GetOrderDetails', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({ orderId: orderId })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        document.getElementById('orderDetailsContent').innerHTML = data.d;
                        $('#orderDetailsModal').modal('show');
                    } else {
                        showErrorAlert('Không thể tải chi tiết đơn hàng!');
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
                <h2>Quản Lý Trạng Thái Đơn Hàng</h2>
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
                                            <h4 id="pendingOrdersCount" runat="server">0</h4>
                                            <p>Đang xử lý</p>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="shippingOrdersCount" runat="server">0</h4>
                                            <p>Đang vận chuyển</p>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="deliveredOrdersCount" runat="server">0</h4>
                                            <p>Đã giao hàng</p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <!-- Form cập nhật trạng thái (bên trái) -->
                                <div class="col-sm-12 col-md-4 col-lg-4 mobile-inputs">
                                    <asp:Panel ID="pUpdateOrderStatus" runat="server">
                                        <h4 class="sub-title">Cập nhật trạng thái</h4>
                                        <div>
                                            <div class="form-group">
                                                <label>Trạng thái hóa đơn</label>
                                                <div>
                                                    <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="form-control">
                                                        <asp:ListItem Value="0">Chọn trạng thái</asp:ListItem>
                                                        <asp:ListItem>Đang xử lý</asp:ListItem>
                                                        <asp:ListItem>Đang trên đường vận chuyển</asp:ListItem>
                                                        <asp:ListItem>Đã giao hàng</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvDdlOrderStatus" runat="server" ForeColor="Red" ControlToValidate="ddlOrderStatus"
                                                        ErrorMessage="Bắt buộc chọn trạng thái đơn hàng!" SetFocusOnError="true" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>
                                                    <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                </div>
                                            </div>
                                            <div class="pb-5 text-center">
                                                <asp:Button ID="btnUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
                                                <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn btn-primary" OnClick="btnCancel_Click" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <!-- Danh sách đơn hàng (bên phải) -->
                                <div class="col-sm-12 col-md-8 col-lg-8">
                                    <h4 class="sub-title">Quản lý danh sách đơn đặt hàng</h4>
                                    <!-- Thanh công cụ lọc và tìm kiếm -->
                                    <div class="row mb-3">
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Tất cả trạng thái</asp:ListItem>
                                                <asp:ListItem>Đang xử lý</asp:ListItem>
                                                <asp:ListItem>Đang trên đường vận chuyển</asp:ListItem>
                                                <asp:ListItem>Đã giao hàng</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Tìm kiếm mã đơn hàng" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:Repeater ID="rOrderStatus" runat="server" OnItemCommand="rOrderStatus_ItemCommand">
                                            <HeaderTemplate>
                                                <table class="table data-table-export table-hover nowrap">
                                                    <thead>
                                                        <tr>
                                                            <th>STT</th>
                                                            <th class="table-plus">Mã đơn hàng</th>
                                                            <th>Ngày đặt hàng</th>
                                                            <th>Trạng thái</th>
                                                            <th>Tên món ăn</th>
                                                            <th>Tổng tiền</th>
                                                            <th>Phương thức thanh toán</th>
                                                            <th class="datatable-nosort">Thao tác</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Container.ItemIndex + 1 %></td>
                                                    <td class="table-plus"><%# Eval("OrdersNo") %></td>
                                                    <td><%# Eval("NgayTaoOrders") %></td>
                                                    <td>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("TrangThai") %>'
                                                            CssClass='<%# Eval("TrangThai").ToString() == "Đã giao hàng" ? "badge badge-success" : "badge badge-warning" %>'>
                                                        </asp:Label>
                                                    </td>
                                                    <td><%# Eval("TenMonAn") %></td>
                                                    <td><%# Eval("TotalPrice") != DBNull.Value ? String.Format("{0:N0} VND", Convert.ToDecimal(Eval("TotalPrice"))) : "0 VND" %></td>
                                                    <td><%# Eval("PhuongThucThanhToan") %></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkEdit" Text="Cập nhật" runat="server" CssClass="badge badge-primary"
                                                            CommandArgument='<%# Eval("MaOrders") %>' CommandName="edit">
                                                            <i class="ti-pencil"></i>
                                                        </asp:LinkButton>
                                                        <a href="javascript:void(0)" onclick="showOrderDetails('<%# Eval("MaOrders") %>')" class="badge badge-info">
                                                            <i class="ti-eye"></i> Xem chi tiết
                                                        </a>
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

    <!-- Modal chi tiết đơn hàng -->
    <div class="modal fade" id="orderDetailsModal" tabindex="-1" role="dialog" aria-labelledby="orderDetailsModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="orderDetailsModalLabel">Chi tiết đơn hàng</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body" id="orderDetailsContent">
                    <!-- Nội dung chi tiết đơn hàng sẽ được tải động -->
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-dismiss="modal">Đóng</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>