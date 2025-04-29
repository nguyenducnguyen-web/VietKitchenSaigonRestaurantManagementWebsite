<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="XacNhanMaOTP.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.XacNhanMaOTP" %>
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
        }
        .book_section {
            padding: 80px 0;
            background: url('../Images/login-bg.jpg') no-repeat center/cover;
            box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.6);
            min-height: calc(100vh - 200px);
            position: relative;
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
        .btn-custom {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            padding: 15px 50px;
            font-size: 1.2rem;
            font-weight: bold;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
            border: none;
        }
        .btn-custom:hover {
            transform: scale(1.1) translateY(-10px);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .otp-container {
            text-align: center;
            margin: 20px 0;
        }
        .otp-container #countdownTimer {
            display: inline-block;
            background-color: #e7f3ff;
            color: #ff6f61;
            font-size: 1.5rem;
            font-weight: bold;
            padding: 10px 20px;
            border-radius: 8px;
            letter-spacing: 2px;
        }
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

        /* CSS tùy chỉnh cho SweetAlert2 */
        .custom-swal-popup {
            background: #ff0000 !important;
            color: #ffffff !important;
            border-radius: 15px !important;
        }
        .custom-swal-title {
            color: #ffffff !important;
            font-size: 1.5rem !important;
        }
        .custom-swal-text {
            color: #ffffff !important;
            font-size: 1.2rem !important;
        }
        .custom-swal-button {
            background: linear-gradient(45deg, #ff6f61, #ffca28) !important;
            color: #ffffff !important;
            border: none !important;
            border-radius: 25px !important;
            padding: 10px 30px !important;
            font-size: 1.1rem !important;
            transition: all 0.3s ease !important;
        }
        .custom-swal-button:hover {
            transform: scale(1.1) !important;
            box-shadow: 0 5px 15px rgba(255, 111, 97, 0.8) !important;
        }
    </style>

    <script>
        window.onload = function () {
            // Khởi tạo Particles.js
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

            // Bộ đếm ngược 10 phút (600 giây)
            let timeLeft = 600; // 10 phút tính bằng giây
            const timerElement = document.getElementById('countdownTimer');

            function updateTimer() {
                if (timeLeft <= 0) {
                    // Hiển thị thông báo hết thời gian với SweetAlert2
                    Swal.fire({
                        title: 'Hết thời gian!',
                        html: 'Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới.',
                        icon: 'error',
                        confirmButtonText: 'Quay lại',
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            htmlContainer: 'custom-swal-text',
                            confirmButton: 'custom-swal-button'
                        },
                        didOpen: () => {
                            document.getElementById('errorSound').play();
                        }
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = 'KhoiPhucMatKhau.aspx';
                        }
                    });
                    return;
                }

                // Tính phút và giây
                let minutes = Math.floor(timeLeft / 60);
                let seconds = timeLeft % 60;

                // Định dạng MM:SS
                minutes = minutes < 10 ? "0" + minutes : minutes;
                seconds = seconds < 10 ? "0" + seconds : seconds;

                // Cập nhật giao diện
                timerElement.textContent = `Thời hạn: ${minutes}:${seconds}`;

                // Giảm thời gian và gọi lại sau 1 giây
                timeLeft--;
                setTimeout(updateTimer, 1000);
            }

            // Bắt đầu đếm ngược
            updateTimer();
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>
            <div class="heading_container text-center">
                <h2>XÁC NHẬN MÃ OTP</h2>
            </div>
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <div class="form_container">
                        <div>
                            <asp:Label ID="lblOtp" runat="server" Text="Nhập mã xác nhận gồm 6 chữ số" CssClass="text-dark font-weight-bold"></asp:Label>
                            <p class="text-muted">Mã đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư hoặc thư rác.</p>
                            <asp:RequiredFieldValidator ID="rfvOtp" runat="server" ErrorMessage="Mã OTP không được để trống" ControlToValidate="txtOtp" 
                                ForeColor="Red" Display="Dynamic" Font-Size="Small"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revOtp" runat="server" ErrorMessage="Mã OTP phải là 6 chữ số" 
                                ControlToValidate="txtOtp" ValidationExpression="^\d{6}$" 
                                ForeColor="Red" Display="Dynamic" Font-Size="Small"></asp:RegularExpressionValidator>
                            <asp:TextBox ID="txtOtp" runat="server" CssClass="form-control" placeholder="Nhập mã OTP (6 số)" MaxLength="6"></asp:TextBox>
                        </div>
                        <div class="otp-container">
                            <span id="countdownTimer">Thời hạn: 10:00</span>
                        </div>
                        <div class="btn_box text-center mt-4">
                            <asp:Button ID="btnVerifyOtp" runat="server" Text="Xác nhận mã" CssClass="btn-custom" OnClick="btnVerifyOtp_Click" CausesValidation="true" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>