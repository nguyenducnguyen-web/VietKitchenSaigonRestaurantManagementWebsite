<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="TrangThongTinNguoiDung.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.TrangThongTinNguoiDung" %>

<%@ Import Namespace="WebDatDoAnOnline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js"></script>
    <audio id="successSound" src="/Audio/chienthang.mp3" preload="auto"></audio>

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background: linear-gradient(135deg, #ff6f61, #ff0000, #ffca28);
            overflow-x: hidden;
            margin: 0;
            position: relative;
        }

        .book_section {
            padding: 80px 0;
            background: url('../Images/profile-bg.jpg') no-repeat center/cover;
            box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.6);
            min-height: calc(100vh - 200px);
            position: relative;
            overflow: hidden;
        }

        .heading_container h2 {
            color: #ffffff;
            font-size: 3.5rem;
            text-transform: uppercase;
            text-shadow: 3px 3px 15px rgba(255, 111, 97, 0.8);
            animation: glow 2s infinite alternate;
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

        .img-thumbnail {
            border: 5px solid #ff6f61;
            transition: transform 0.4s ease;
        }

            .img-thumbnail:hover {
                transform: scale(1.1);
                border-color: #ffca28;
                box-shadow: 0 10px 20px rgba(255, 202, 40, 0.6);
            }

        .btn-warning {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #fff;
            padding: 12px 30px;
            font-size: 1.1rem;
            font-weight: bold;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
        }

            .btn-warning:hover {
                transform: scale(1.1) translateY(-5px);
                background: linear-gradient(45deg, #ffca28, #ff6f61);
                box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
            }

        .userData h2, .userData h6 {
            color: #ff0000;
            text-shadow: 1px 1px 5px rgba(255, 111, 97, 0.5);
        }

        .nav-tabs .nav-link {
            color: #ff6f61;
            font-weight: bold;
            border: none;
            transition: all 0.3s ease;
        }

            .nav-tabs .nav-link.active {
                background: linear-gradient(45deg, #ff0000, #ff6f61);
                color: #fff;
                border-radius: 15px;
                box-shadow: 0 5px 15px rgba(255, 0, 0, 0.5);
            }

        .tab-content {
            padding: 20px;
            background: rgba(255, 255, 255, 0.9);
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(255, 111, 97, 0.3);
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
            }

        .badge {
            padding: 8px 15px;
            font-size: 1rem;
            border-radius: 20px;
            transition: transform 0.3s ease;
        }

            .badge:hover {
                transform: scale(1.1);
            }

        .custom-swal-popup {
            background: linear-gradient(135deg, #ff6f61, #ff0000) !important;
            border-radius: 20px !important;
            box-shadow: 0 10px 30px rgba(255, 111, 97, 0.8) !important;
        }

        .custom-swal-title {
            color: #ffffff !important;
            font-size: 2rem !important;
            text-shadow: 2px 2px 10px rgba(255, 255, 255, 0.5) !important;
        }

        .custom-swal-text {
            color: #ffffff !important;
            font-size: 1.2rem !important;
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

        @keyframes glow {
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

        .particles {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 0;
            pointer-events: none;
        }

        .floating-gif {
            position: absolute;
            z-index: 1;
            animation: float 6s infinite ease-in-out;
        }

        @keyframes float {
            0%, 100% {
                transform: translateY(0);
            }

            50% {
                transform: translateY(-20px);
            }
        }
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

            // Kiểm tra nếu vừa cập nhật xong thì redirect về trang chủ
            if (window.location.search.includes("updated=true")) {
                Swal.fire({
                    title: 'Cập nhật thành công!',
                    text: 'Bạn sẽ được đưa về trang chủ ngay bây giờ!',
                    icon: 'success',
                    customClass: {
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        content: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }
                }).then(() => {
                    confetti({ particleCount: 100, spread: 70, origin: { y: 0.6 } });
                    document.getElementById('successSound').play();
                    window.location.href = "Default.aspx"; // Redirect về trang chủ
                });
            } else {
                Swal.fire({
                    title: 'Chào mừng bạn!',
                    text: 'Hãy khám phá thông tin tuyệt vời của bạn nhé!',
                    icon: 'success',
                    customClass: {
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        content: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    }
                }).then(() => {
                    confetti({ particleCount: 100, spread: 70, origin: { y: 0.6 } });
                    document.getElementById('successSound').play();
                });
            }
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        string imageUrl = Session["DiaChiAnhAvatar"]?.ToString();
    %>

    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>


            <div class="heading_container text-center">
                <h2>Thông Tin Người Dùng</h2>
            </div>

            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="card-title mb-4">
                                <div class="d-flex justify-content-start">
                                    <div class="image-container">
                                        <img
                                            src="<%= Utils.GetImageUrl(imageUrl) %>"
                                            id="imgProfile"
                                            style="width: 150px; height: 150px;"
                                            class="img-thumbnail"
                                            onerror="this.src='../Images/No_image.png';" />
                                        <div class="middle pt-2">
                                            <a href="DangKyTaiKhoan.aspx?id=<%= Session["MaNguoiDung"] %>"
                                                class="btn btn-warning">
                                                <i class="fa fa-pencil"></i>Chỉnh sửa thông tin
                                            </a>
                                        </div>
                                    </div>

                                    <div class="userData ml-3">
                                        <h2 class="d-block"><% Response.Write(Session["TenNguoiDung"]); %></h2>
                                        <h6 class="d-block">
                                            <asp:Label ID="lblUsername" runat="server" ToolTip="Unique Username">
                                                <% Response.Write(Session["TenDangNhapNguoiDung"]); %>
                                            </asp:Label>
                                        </h6>
                                        <h6 class="d-block">
                                            <asp:Label ID="lblEmail" runat="server" ToolTip="User Email">
                                                <% Response.Write(Session["EmailNguoiDung"]); %>
                                            </asp:Label>
                                        </h6>
                                        <h6 class="d-block">
                                            <asp:Label ID="lblCreatedDate" runat="server" ToolTip="Account Created On">
                                                <% Response.Write(Session["NgayTaoNguoiDung"]); %>
                                            </asp:Label>
                                        </h6>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-12">
                                    <ul class="nav nav-tabs mb-4" id="myTab" role="tablist">
                                        <li class="nav-item">
                                            <a class="nav-link active text-info" id="basicInfo-tab" data-toggle="tab" href="#basicInfo" role="tab" aria-controls="basicInfo" aria-selected="true">
                                                <i class="fa fa-id-badge mr-2"></i>Thông tin cơ bản
                                            </a>
                                        </li>
                                        <li class="nav-item">
                                            <a class="nav-link text-info" id="connectedServices-tab" data-toggle="tab" href="#connectedServices" role="tab" aria-controls="connectedServices" aria-selected="false">
                                                <i class="fa fa-clock-o mr-2"></i>Lịch sử mua hàng
                                            </a>
                                        </li>
                                    </ul>

                                    <div class="tab-content ml-1" id="myTabContent">
                                        <div class="tab-pane fade show active" id="basicInfo" role="tabpanel" aria-labelledby="basicInfo-tab">
                                            <asp:Repeater ID="rUserProfile" runat="server">
                                                <ItemTemplate>
                                                    <div class="row">
                                                        <div class="col-sm-3 col-md-2 col-5">
                                                            <label style="font-weight: bold; color: #ff6f61;">Họ và tên</label>
                                                        </div>
                                                        <div class="col-md-8 col-6"><%# Eval("TenNguoiDung") %></div>
                                                    </div>
                                                    <hr />
                                                    <div class="row">
                                                        <div class="col-sm-3 col-md-2 col-5">
                                                            <label style="font-weight: bold; color: #ff6f61;">Username</label>
                                                        </div>
                                                        <div class="col-md-8 col-6"><%# Eval("TenDangNhapNguoiDung") %></div>
                                                    </div>
                                                    <hr />
                                                    <div class="row">
                                                        <div class="col-sm-3 col-md-2 col-5">
                                                            <label style="font-weight: bold; color: #ff6f61;">Số điện thoại</label>
                                                        </div>
                                                        <div class="col-md-8 col-6"><%# Eval("SdtNguoiDung") %></div>
                                                    </div>
                                                    <hr />
                                                    <div class="row">
                                                        <div class="col-sm-3 col-md-2 col-5">
                                                            <label style="font-weight: bold; color: #ff6f61;">Email</label>
                                                        </div>
                                                        <div class="col-md-8 col-6"><%# Eval("EmailNguoiDung") %></div>
                                                    </div>
                                                    <hr />
                                                    <div class="row">
                                                        <div class="col-sm-3 col-md-2 col-5">
                                                            <label style="font-weight: bold; color: #ff6f61;">Post Code</label>
                                                        </div>
                                                        <div class="col-md-8 col-6"><%# Eval("PostCode") %></div>
                                                    </div>
                                                    <hr />
                                                    <div class="row">
                                                        <div class="col-sm-3 col-md-2 col-5">
                                                            <label style="font-weight: bold; color: #ff6f61;">Địa chỉ</label>
                                                        </div>
                                                        <div class="col-md-8 col-6"><%# Eval("DiaChiNguoiDung") %></div>
                                                    </div>
                                                    <hr />
                                                </ItemTemplate>
                                            </asp:Repeater>

                                            <!-- Thêm phần cập nhật mật khẩu -->
                                            <div class="row mt-4">
                                                <div class="col-12 text-center">
                                                    <asp:Button ID="btnChangePassword" runat="server" Text="Đổi mật khẩu" CssClass="btn btn-warning" OnClick="btnChangePassword_Click" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="tab-pane fade" id="connectedServices" role="tabpanel" aria-labelledby="connectedServices-tab">
                                            <asp:Repeater ID="rPurchaseHistory" runat="server" OnItemDataBound="rPurchaseHistory_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="container">
                                                        <div class="row pt-1 pb-1" style="background-color: rgba(255, 111, 97, 0.2); border-radius: 10px;">
                                                            <div class="col-4">
                                                                <span class="badge badge-pill badge-dark text-white"><%# Eval("SrNo") %></span>
                                                                Phương thức: <%# Eval("PhuongThucThanhToan").ToString() == "cod" ? "Thanh toán khi nhận hàng" : Eval("PhuongThucThanhToan").ToString().ToUpper() %>
                                                            </div>
                                                            <div class="col-6">
                                                                <%# string.IsNullOrEmpty(Eval("GioHangNo").ToString()) ? "" : "Mã giỏ hàng: " + Eval("GioHangNo") %>
                                                            </div>
                                                            <div class="col-2" style="text-align: end">
                                                                <a href="HoaDon.aspx?id=<%# Eval("MaThanhToan") %>" class="btn btn-info btn-sm">
                                                                    <i class="fa fa-download mr-2"></i>Hoá đơn
                                                                </a>
                                                            </div>
                                                        </div>

                                                        <asp:HiddenField ID="hdnPaymentId" runat="server" Value='<%# Eval("MaThanhToan") %>' />

                                                        <asp:Repeater ID="rOrders" runat="server">
                                                            <HeaderTemplate>
                                                                <table class="table data-table-export table-responsive-sm table-bordered table-hover">
                                                                    <thead class="bg-dark text-white">
                                                                        <tr>
                                                                            <th>Tên món ăn</th>
                                                                            <th>Giá</th>
                                                                            <th>Số lượng</th>
                                                                            <th>Tổng tiền</th>
                                                                            <th>Mã đơn hàng</th>
                                                                            <th>Trạng thái</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblName" runat="server" Text='<%# Eval("TenMonAn") %>'></asp:Label></td>
                                                                    <td>
                                                                        <asp:Label ID="lblPrice" runat="server" Text='<%# string.IsNullOrEmpty(Eval("GiaMonAnSauKhuyenMai").ToString()) ? "" : Eval("GiaMonAnSauKhuyenMai") + " VND" %>'></asp:Label></td>
                                                                    <td>
                                                                        <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("SoLuong") %>'></asp:Label></td>
                                                                    <td>
                                                                        <asp:Label ID="lblTotalPrice" runat="server" Text='<%# Eval("TotalPrice") != DBNull.Value ? String.Format("{0:N0} VND", Convert.ToDouble(Eval("TotalPrice"))) : "0 VND" %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblOrderNo" runat="server" Text='<%# Eval("OrdersNo") %>'></asp:Label></td>
                                                                    <td>
                                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("TrangThai") %>' CssClass='<%# Eval("TrangThai").ToString() == "Đã giao hàng" ? "badge badge-success" : "badge badge-warning" %>'></asp:Label></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                </tbody>
                                                                </table>
                                                            </FooterTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
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
