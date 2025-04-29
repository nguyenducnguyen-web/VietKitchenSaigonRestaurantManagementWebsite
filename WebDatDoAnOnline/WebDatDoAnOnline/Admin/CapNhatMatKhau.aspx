<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="CapNhatMatKhau.aspx.cs" Inherits="WebDatDoAnOnline.Admin.CapNhatMatKhau" %>
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
            content: '🔒';
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
                        particleCount: 200, spread: 120, colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'], origin: { y: 0.6 }
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
                <h2>Cập Nhật Mật Khẩu</h2>
            </div>

            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="align-align-self-end text-center mb-4">
                                <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert"></asp:Label>
                            </div>

                            <asp:Panel ID="pChangePassword" runat="server">
                                <h4 class="sub-title">Đổi mật khẩu</h4>
                                <div>
                                    <div class="form-group">
                                        <label>Mật khẩu hiện tại</label>
                                        <div>
                                            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" placeholder="Nhập mật khẩu hiện tại" TextMode="Password" ValidationGroup="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server"
                                                ErrorMessage="Bắt buộc nhập mật khẩu hiện tại" ForeColor="Red" Display="Dynamic"
                                                SetFocusOnError="true" ControlToValidate="txtCurrentPassword" ValidationGroup="Password"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label>Mật khẩu mới</label>
                                        <div>
                                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" placeholder="Nhập mật khẩu mới" TextMode="Password" ValidationGroup="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server"
                                                ErrorMessage="Bắt buộc nhập mật khẩu mới" ForeColor="Red" Display="Dynamic"
                                                SetFocusOnError="true" ControlToValidate="txtNewPassword" ValidationGroup="Password"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label>Xác nhận mật khẩu mới</label>
                                        <div>
                                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="Xác nhận mật khẩu mới" TextMode="Password" ValidationGroup="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server"
                                                ErrorMessage="Bắt buộc xác nhận mật khẩu" ForeColor="Red" Display="Dynamic"
                                                SetFocusOnError="true" ControlToValidate="txtConfirmPassword" ValidationGroup="Password"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="cvConfirmPassword" runat="server"
                                                ErrorMessage="Mật khẩu xác nhận không khớp!" ForeColor="Red" Display="Dynamic"
                                                ControlToValidate="txtConfirmPassword" ControlToCompare="txtNewPassword" ValidationGroup="Password"></asp:CompareValidator>
                                        </div>
                                    </div>

                                    <div class="pb-5 text-center">
                                        <asp:Button ID="btnUpdatePassword" runat="server" Text="Cập nhật mật khẩu" CssClass="btn btn-primary" OnClick="btnUpdatePassword_Click" ValidationGroup="Password" />
                                        <asp:Button ID="btnClearPassword" runat="server" Text="Xóa dữ liệu" CssClass="btn btn-primary" CausesValidation="false" OnClick="btnClearPassword_Click" />
                                        <asp:Button ID="btnBack" runat="server" Text="Quay lại" CssClass="btn btn-danger" CausesValidation="false" OnClick="btnBack_Click" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>