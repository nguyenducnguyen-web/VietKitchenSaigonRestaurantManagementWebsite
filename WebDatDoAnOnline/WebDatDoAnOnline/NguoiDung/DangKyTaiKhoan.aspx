<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="DangKyTaiKhoan.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.DangKyTaiKhoan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Thêm các thư viện cần thiết -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js"></script>
    <!-- Âm thanh -->
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
            padding: 80px 0;
            background: url('../Images/register-bg.jpg') no-repeat center/cover;
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
        .form_container {
            background: rgba(255, 255, 255, 0.95);
            padding: 40px;
            border-radius: 25px;
            box-shadow: 0 15px 40px rgba(255, 0, 0, 0.5);
            animation: slideInLeft 1.5s ease-in-out;
            position: relative;
            overflow: hidden;
        }
        .form_container::before {
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
        .form-control {
            border: none;
            border-bottom: 3px solid #ff6f61;
            background: transparent;
            padding: 12px;
            transition: all 0.4s ease;
            font-size: 1.1rem;
            color: #ff0000;
        }
        .form-control:focus {
            border-color: #ffca28;
            box-shadow: 0 5px 15px rgba(255, 202, 40, 0.5);
            background: rgba(255, 255, 255, 0.1);
        }
        .btn_box .btn {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            padding: 15px 50px;
            font-size: 1.2rem;
            font-weight: bold;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
        }
        .btn_box .btn:hover {
            transform: scale(1.1) translateY(-10px);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .img-thumbnail {
            border: none;
            border-radius: 20px;
            box-shadow: 0 15px 40px rgba(255, 111, 97, 0.5);
            animation: float 4s infinite ease-in-out;
        }
        .text-info a {
            color: #ffca28;
            font-weight: bold;
            transition: all 0.3s ease;
        }
        .text-info a:hover {
            color: #ff6f61;
            text-decoration: underline;
        }
        /* SweetAlert2 Custom */
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
        @keyframes glow {
            0% { text-shadow: 0 0 10px #ff6f61; }
            100% { text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000; }
        }
        @keyframes sparkle {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        @keyframes float {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-20px); }
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

        function ImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%= imgUser.ClientID %>').prop('src', e.target.result)
                        .width(200)
                        .height(200)
                        .css('animation', 'float 4s infinite ease-in-out');
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>
            <div class="heading_container text-center">
                <asp:Label ID="lblHeaderMsg" runat="server" Text="<h2>ĐĂNG KÝ TÀI KHOẢN</h2>" />
                <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="mt-3 text-white" />
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="form_container">
                        <div>
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Bắt buộc nhập tên" ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:RegularExpressionValidator ID="revName" runat="server" ErrorMessage="Tên chỉ được nhập ký tự chữ cái" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^[a-zA-Z\s]+$" ControlToValidate="txtName" Font-Size="Small" />
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Nhập tên" ToolTip="Họ tên" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ErrorMessage="Bắt buộc nhập tên người dùng" ControlToValidate="txtUsername" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Nhập tên người dùng" ToolTip="Tên người dùng" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Bắt buộc nhập Email" ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Nhập Email" ToolTip="Email" TextMode="Email" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvDt" runat="server" ErrorMessage="Bắt buộc nhập số điện thoại" ControlToValidate="txtDt" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:RegularExpressionValidator ID="revDt" runat="server" ErrorMessage="Số điện thoại phải có đủ 10 chữ số" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^[0-9]{10}$" ControlToValidate="txtDt" Font-Size="Small" />
                            <asp:TextBox ID="txtDt" runat="server" CssClass="form-control" placeholder="Nhập số điện thoại" ToolTip="Số điện thoại" TextMode="Number" />
                        </div>
                        <!-- Thêm trường txtPassword -->
                        <div>
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Bắt buộc nhập mật khẩu" ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Nhập mật khẩu" ToolTip="Mật khẩu" TextMode="Password" />
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form_container">
                        <div>
                            <asp:RequiredFieldValidator ID="rfvDiaChi" runat="server" ErrorMessage="Bắt buộc nhập địa chỉ" ControlToValidate="txtDiaChi" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:TextBox ID="txtDiaChi" runat="server" CssClass="form-control" placeholder="Nhập địa chỉ" ToolTip="Địa chỉ" TextMode="MultiLine" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="rfvPostCode" runat="server" ErrorMessage="Bắt buộc nhập Post Code" ControlToValidate="txtPostCode" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                            <asp:RegularExpressionValidator ID="revPostCode" runat="server" ErrorMessage="PostCode phải có đủ 6 chữ số" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationExpression="^[0-9]{6}$" ControlToValidate="txtPostCode" Font-Size="Small" />
                            <asp:TextBox ID="txtPostCode" runat="server" CssClass="form-control" placeholder="Nhập Post Code" ToolTip="Post Code" TextMode="Number" />
                        </div>
                        <div>
                            <asp:FileUpload ID="fuUserImage" runat="server" CssClass="form-control" ToolTip="User Image" onchange="ImagePreview(this);" />
                        </div>
                        <!-- Thêm trường mật khẩu khi ở chế độ cập nhật -->
                        <asp:Panel ID="pnlPassword" runat="server" Visible="false">
                            <div>
                                <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" ErrorMessage="Bắt buộc nhập mật khẩu mới" ControlToValidate="txtNewPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" placeholder="Nhập mật khẩu mới" ToolTip="Mật khẩu mới" TextMode="Password" />
                            </div>
                            <div>
                                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ErrorMessage="Bắt buộc xác nhận mật khẩu" ControlToValidate="txtConfirmPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" Font-Size="Small" />
                                <asp:CompareValidator ID="cvConfirmPassword" runat="server" ErrorMessage="Mật khẩu xác nhận không khớp" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword" ForeColor="Red" Display="Dynamic" Font-Size="Small" />
                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="Xác nhận mật khẩu" ToolTip="Xác nhận mật khẩu" TextMode="Password" />
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="row pl-4 mt-4">
                    <div class="btn_box text-center w-100">
                        <asp:Button ID="btnRegister" runat="server" Text="Đăng ký" CssClass="btn" OnClick="btnRegister_Click" />
                        <asp:Label ID="lblAlreadyUser" runat="server" CssClass="pl-3 text-info" Text="Bạn đã đăng ký? <a href='DangNhap.aspx' class='badge badge-info'>Đăng nhập ở đây…</a>" />
                    </div>
                </div>
                <div class="row p-5 justify-content-center">
                    <asp:Image ID="imgUser" runat="server" CssClass="img-thumbnail" />
                </div>
            </div>
        </div>
    </section>
</asp:Content>