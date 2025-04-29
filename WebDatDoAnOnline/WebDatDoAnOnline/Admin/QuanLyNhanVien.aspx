<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLyNhanVien.aspx.cs" Inherits="WebDatDoAnOnline.Admin.QuanLyNhanVien" %>
<%@ Import Namespace="WebDatDoAnOnline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
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
            content: '👤';
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

        .form-control, .form-check-input {
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

        .form-check-input {
            width: 20px;
            height: 20px;
            margin-right: 10px;
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

        .btn-danger {
            background: linear-gradient(45deg, #ff0000, #cc0000);
        }

        .btn-danger:hover {
            background: linear-gradient(45deg, #ff3333, #ff6f61);
            box-shadow: 0 20px 40px rgba(255, 0, 0, 0.8);
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

        .badge-danger {
            background: #ff0000;
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>

            <div class="heading_container text-center">
                <h2>Quản Lý Nhân Viên</h2>
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
                                            <h4 id="totalEmployees" runat="server">0</h4>
                                            <p>Tổng nhân viên</p>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="activeEmployees" runat="server">0</h4>
                                            <p>Nhân viên đang hoạt động</p>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xl-4">
                                        <div class="stats-card">
                                            <h4 id="salesEmployees" runat="server">0</h4>
                                            <p>Nhân viên bán hàng</p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <!-- Form thêm/sửa nhân viên (bên trái) -->
                                <div class="col-sm-12 col-md-4 col-lg-4 mobile-inputs">
                                    <asp:Panel ID="pManageEmployee" runat="server">
                                        <h4 class="sub-title">Thêm/Sửa nhân viên</h4>
                                        <div>
                                            <div class="form-group">
                                                <label>Tên nhân viên</label>
                                                <div>
                                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Vui lòng nhập tên nhân viên"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                        ErrorMessage="Bắt buộc nhập tên nhân viên" ForeColor="Red" Display="Dynamic"
                                                        SetFocusOnError="true" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                    <asp:HiddenField ID="hdnId" runat="server" Value="" />
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label>Tên đăng nhập</label>
                                                <div>
                                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Vui lòng nhập tên đăng nhập"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ErrorMessage="Bắt buộc nhập tên đăng nhập" ForeColor="Red" Display="Dynamic"
                                                        SetFocusOnError="true" ControlToValidate="txtUsername"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label>Số điện thoại</label>
                                                <div>
                                                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Vui lòng nhập số điện thoại"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                        ErrorMessage="Số điện thoại phải có 10 chữ số!" ForeColor="Red" Display="Dynamic"
                                                        SetFocusOnError="true" ControlToValidate="txtPhone" ValidationExpression="^\d{10}$"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label>Email</label>
                                                <div>
                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Vui lòng nhập email" TextMode="Email"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                                        ErrorMessage="Bắt buộc nhập email" ForeColor="Red" Display="Dynamic"
                                                        SetFocusOnError="true" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
                                                        ErrorMessage="Email không hợp lệ!" ForeColor="Red" Display="Dynamic"
                                                        SetFocusOnError="true" ControlToValidate="txtEmail"
                                                        ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label>Địa chỉ</label>
                                                <div>
                                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Vui lòng nhập địa chỉ" TextMode="MultiLine"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <label>Quyền</label>
                                                <div>
                                                    <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control">
                                                        <asp:ListItem Value="">Chọn quyền</asp:ListItem>
                                                        <asp:ListItem Value="Nhân viên bán hàng">Nhân viên bán hàng</asp:ListItem>
                                                        <asp:ListItem Value="Nhân viên kho">Nhân viên kho</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                                        ErrorMessage="Bắt buộc chọn quyền!" ForeColor="Red" Display="Dynamic"
                                                        SetFocusOnError="true" ControlToValidate="ddlRoles" InitialValue=""></asp:RequiredFieldValidator>
                                                </div>
                                            </div>

                                            <asp:Panel ID="pnlPassword" runat="server">
                                                <div class="form-group">
                                                    <label>Mật khẩu</label>
                                                    <div>
                                                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Vui lòng nhập mật khẩu" TextMode="Password"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                                            ErrorMessage="Bắt buộc nhập mật khẩu" ForeColor="Red" Display="Dynamic"
                                                            SetFocusOnError="true" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                            <div class="form-check pl-4">
                                                <asp:CheckBox ID="cbIsActive" runat="server" Text="Có hoạt động?" CssClass="form-check-input" />
                                            </div>

                                            <div class="pb-5 text-center">
                                                <asp:Button ID="btnAddOrUpdate" runat="server" Text="Thêm" CssClass="btn btn-primary" OnClick="btnAddOrUpdate_Click" />
                                                <asp:Button ID="btnClear" runat="server" Text="Xóa dữ liệu" CssClass="btn btn-primary" CausesValidation="false" OnClick="btnClear_Click" />
                                                <asp:Button ID="btnExit" runat="server" Text="Thoát" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnExit_Click" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <!-- Danh sách nhân viên (bên phải) -->
                                <div class="col-sm-12 col-md-8 col-lg-8">
                                    <h4 class="sub-title">Danh sách nhân viên</h4>
                                    <!-- Thanh công cụ tìm kiếm và sắp xếp -->
                                    <div class="row mb-3">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Tìm kiếm tên, email hoặc số điện thoại" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
                                                <asp:ListItem Value="" Selected="True">Sắp xếp theo</asp:ListItem>
                                                <asp:ListItem Value="name_asc">Tên: A-Z</asp:ListItem>
                                                <asp:ListItem Value="name_desc">Tên: Z-A</asp:ListItem>
                                                <asp:ListItem Value="newest">Mới nhất</asp:ListItem>
                                                <asp:ListItem Value="oldest">Cũ nhất</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="table-responsive">
                                        <asp:Repeater ID="rEmployee" runat="server" OnItemCommand="rEmployee_ItemCommand" OnItemDataBound="rEmployee_ItemDataBound">
                                            <HeaderTemplate>
                                                <table class="table data-table-export table-hover nowrap">
                                                    <thead>
                                                        <tr>
                                                            <th>STT</th>
                                                            <th class="table-plus">Tên nhân viên</th>
                                                            <th>Tên đăng nhập</th>
                                                            <th>Số điện thoại</th>
                                                            <th>Email</th>
                                                            <th>Địa chỉ</th>
                                                            <th>Quyền</th>
                                                            <th>Trạng thái</th>
                                                            <th>Ngày tạo</th>
                                                            <th class="datatable-nosort">Thao tác</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Container.ItemIndex + 1 %></td>
                                                    <td class="table-plus"><%# Eval("TenNhanVien") %></td>
                                                    <td><%# Eval("TenDangNhapNhanVien") %></td>
                                                    <td><%# Eval("SdtNhanVien") %></td>
                                                    <td><%# Eval("EmailNhanVien") %></td>
                                                    <td><%# Eval("DiaChiNhanVien") %></td>
                                                    <td><%# Eval("Quyen") %></td>
                                                    <td>
                                                        <asp:Label ID="lblIsActive" runat="server" Text='<%# Eval("TrangThai") %>'></asp:Label>
                                                    </td>
                                                    <td><%# Eval("NgayTaoNhanVien") %></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkEdit" Text="Chỉnh sửa" runat="server" CssClass="badge badge-primary" CausesValidation="false" CommandArgument='<%# Eval("TenDangNhapNhanVien") %>' CommandName="edit">
                                                            <i class="ti-pencil"></i>
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="lnkDelete" Text="Xóa" runat="server" CommandName="delete" CssClass="badge badge-danger" CommandArgument='<%# Eval("TenDangNhapNhanVien") %>' OnClientClick="return confirm('Bạn có thật sự muốn xóa nhân viên này?');" CausesValidation="false">
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
</asp:Content>