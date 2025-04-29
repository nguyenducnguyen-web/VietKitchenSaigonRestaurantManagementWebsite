<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="GioHang.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.GioHang" %>
<%@ Import Namespace="WebDatDoAnOnline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/pa<a href="\\mac\home\downloads\webdatdoanonline\webdatdoanonline\nguoidung\GioHang.aspx.designer.cs">\\mac\home\downloads\webdatdoanonline\webdatdoanonline\nguoidung\GioHang.aspx.designer.cs</a>
<a href="\\mac\home\downloads\webdatdoanonline\webdatdoanonline\nguoidung\GioHang.aspx.cs">\\mac\home\downloads\webdatdoanonline\webdatdoanonline\nguoidung\GioHang.aspx.cs</a>
<a href="\\mac\home\downloads\webdatdoanonline\webdatdoanonline\nguoidung\GioHang.aspx">\\mac\home\downloads\webdatdoanonline\webdatdoanonline\nguoidung\GioHang.aspx</a>rticles.min.js"></script>
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
            background: url('../Images/cart-bg.jpg') no-repeat center/cover;
            box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.6);
            min-height: calc(100vh - 200px);
            position: relative;
            overflow: hidden;
        }
        .heading_container h2 {
            color: #ffffff;
            font-size: 3rem;
            text-transform: uppercase;
            text-shadow: 3px 3px 15px rgba(255, 111, 97, 0.8);
            animation: glow 2s infinite alternate;
        }
        .table {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 25px;
            box-shadow: 0 15px 40px rgba(255, 0, 0, 0.5);
            animation: slideInLeft 1.5s ease-in-out;
            overflow: hidden;
            position: relative;
        }
        .table::before {
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
        .table th {
            background: linear-gradient(45deg, #ff6f61, #ff0000);
            color: #fff;
            font-size: 1.2rem;
            text-transform: uppercase;
            padding: 20px;
            border: none;
        }
        .table td {
            padding: 20px;
            color: #ff0000;
            font-size: 1.1rem;
            vertical-align: middle;
            border-bottom: 2px solid #ff6f61;
        }
        .table img {
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(255, 0, 0, 0.3);
            transition: transform 0.3s ease;
        }
        .table img:hover {
            transform: scale(1.1);
        }
        .pro-qty input {
            width: 70px;
            padding: 10px;
            border: 3px solid #ff6f61;
            border-radius: 10px;
            background: transparent;
            color: #ff0000;
            font-size: 1.1rem;
            text-align: center;
            transition: all 0.4s ease;
        }
        .pro-qty input:focus {
            border-color: #ffca28;
            box-shadow: 0 5px 15px rgba(255, 202, 40, 0.5);
            background: rgba(255, 255, 255, 0.1);
        }
        .btn {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            padding: 15px 30px;
            font-size: 1.2rem;
            font-weight: bold;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
        }
        .btn:hover {
            transform: scale(1.1) translateY(-5px);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .fa-close {
            color: #ff0000;
            font-size: 1.5rem;
            transition: all 0.3s ease;
        }
        .fa-close:hover {
            color: #ffca28;
            transform: rotate(90deg);
        }
        /*.grand-total {
    background: linear-gradient(135deg, #ff6f61, #ff0000, #ffca28);*/ /* Trùng với nền body */
    /*color: #ffffff;*/ /* Đổi màu chữ thành trắng để nổi bật trên nền gradient */
    /*padding: 15px;
    border-radius: 15px;
    font-size: 1.5rem;
    font-weight: bold;
    text-align: right;
    box-shadow: 0 5px 15px rgba(255, 111, 97, 0.5);*/ /* Thêm chút bóng để nổi bật */
/*}*/
        /* SweetAlert2 Custom - Đồng bộ với TrangThongTinNguoiDung.aspx */
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
        /* Hiệu ứng động */
        /*@keyframes glow {
            0% { text-shadow: 0 0 10px #ff6f61; }
            100% { text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000; }
        }
        @keyframes sparkle {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }*/
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
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-20px); }
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
        };

        function showSuccessAlert(message) {
            Swal.fire({
                title: 'Thành công!',
                text: message,
                icon: 'success',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                }
            }).then(() => {
                confetti({
                    particleCount: 100,
                    spread: 70,
                    origin: { y: 0.6 }
                });
                document.getElementById('successSound').play();
            });
        }

        function showErrorAlert(message) {
            Swal.fire({
                title: 'Lỗi!',
                text: message,
                icon: 'error',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>
            <!-- Thêm GIF động -->
          
            <div class="heading_container text-center">
                <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                <h2>Giỏ Hàng Của Bạn</h2>
            </div>
            <asp:Repeater ID="rCartItem" runat="server" OnItemCommand="rCartItem_ItemCommand" OnItemDataBound="rCartItem_ItemDataBound">
                <HeaderTemplate>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Tên Món Ăn</th>
                                <th>Ảnh</th>
                                <th>Giá</th>
                                <th>Số Lượng</th>
                                <th>Tổng Tiền</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("TenMonAn") %>'></asp:Label>
                        </td>
                        <td>
                            <img width="60" src='<%# Utils.GetImageUrl(Eval("DiaChiAnhMonAn")) %>' alt="" />
                        </td>
                        <td>
                            <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("GiaMonAnSauKhuyenMai") %>'></asp:Label>
                            <asp:HiddenField ID="hdnProductId" runat="server" Value='<%# Eval("MaMonAn") %>' />
                            <asp:HiddenField ID="hdnQuantity" runat="server" Value='<%# Eval("Qty") %>' />
                            <asp:HiddenField ID="hdnPrdQuantity" runat="server" Value='<%# Eval("PrdQty") %>' />
                        </td>
                        <td>
                            <div class="pro-qty">
                                <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text='<%# Eval("SoLuong") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revQuantity" runat="server" ErrorMessage="*" ForeColor="Red"
                                    Font-Size="Small" ValidationExpression="[1-9]*" ControlToValidate="txtQuantity"
                                    Display="Dynamic" SetFocusOnError="true" EnableClientScript="true"></asp:RegularExpressionValidator>
                            </div>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalPrice" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbDelete" runat="server" CommandName="remove" CommandArgument='<%# Eval("MaMonAn") %>'>
                                <i class="fa fa-close"></i>
                            </asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td colspan="3"></td>
                        <td class="grand-total">
                            <b>Tổng Tiền:</b>
                        </td>
                        <td class="grand-total">
                           <% Response.Write(Session["grandTotalPrice"] != null ? String.Format("{0:N0} VND", Convert.ToDecimal(Session["grandTotalPrice"])) : "0 VND"); %>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <a href="DanhSachMonAn.aspx" class="btn">
                                <i class="fa fa-arrow-circle-left mr-2"></i>Tiếp Tục Chọn Món
                            </a>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbUpdateCart" runat="server" CommandName="updateCart" CssClass="btn">
                                <i class="fa fa-refresh mr-2"></i>Cập Nhật Giỏ Hàng
                            </asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbCheckout" runat="server" CommandName="checkout" CssClass="btn">
                                Tiến Hành Đặt Hàng<i class="fa fa-arrow-circle-right ml-2"></i>
                            </asp:LinkButton>
                        </td>
                    </tr>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </section>
</asp:Content>