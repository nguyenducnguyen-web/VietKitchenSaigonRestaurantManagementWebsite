<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="TrangChu.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.TrangChu" %>
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
        .offer_section, .about_section, .client_section {
            padding: 80px 0;
            background: url('../Images/home-bg.jpg') no-repeat center/cover;
            box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.6);
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
        .box {
            background: rgba(255, 255, 255, 0.95);
            padding: 20px;
            border-radius: 25px;
            box-shadow: 0 15px 40px rgba(255, 0, 0, 0.5);
            animation: slideInLeft 1.5s ease-in-out;
            transition: transform 0.4s ease;
        }
        .box:hover {
            transform: scale(1.05) translateY(-10px);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
        }
        .img-box img {
            border-radius: 20px;
            transition: transform 0.4s ease;
        }
        .img-box img:hover {
            transform: scale(1.1);
        }
        .detail-box h5, .detail-box h6 {
            color: #ff6f61;
            font-weight: bold;
            text-shadow: 1px 1px 5px rgba(255, 0, 0, 0.5);
        }
        .detail-box a {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            padding: 12px 30px;
            border-radius: 50px;
            font-weight: bold;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
        }
        .detail-box a:hover {
            transform: scale(1.1) translateY(-5px);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .client_section .box {
            background: rgba(0, 0, 0, 0.8);
            color: #ffffff;
            padding: 30px;
            border-radius: 20px;
            box-shadow: 0 15px 40px rgba(0, 0, 0, 0.7);
            animation: fadeInUp 1.5s ease-in-out;
        }
        .client_section .img-box img {
            border-radius: 50%;
            width: 100px;
            height: 100px;
            object-fit: cover;
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
        @keyframes glow {
            0% { text-shadow: 0 0 10px #ff6f61; }
            100% { text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000; }
        }
        @keyframes float {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-20px); }
        }
        @keyframes slideInLeft {
            from { transform: translateX(-100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        @keyframes fadeInUp {
            from { transform: translateY(50px); opacity: 0; }
            to { transform: translateY(0); opacity: 1; }
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- offer section -->
    <section class="offer_section layout_padding-bottom">
        <div class="offer_container position-relative">
            <div id="particles-js" class="particles"></div>
            <div class="container">
                <div class="heading_container text-center">
                    <h2>Khuyến mãi đặc biệt</h2>
                </div>
                <div class="row">
                    <asp:Repeater ID="rCategory" runat="server">
                        <ItemTemplate>
                            <div class="col-md-6">
                                <div class="box">
                                    <div class="img-box">
                                        <a href="DanhSachMonAn.aspx?id=<%# Eval("MaLoaiMonAn") %>">
                                            <img src="<%# Utils.GetImageUrl(Eval("DiaChiAnhLoaiMonAn")) %>" alt="">
                                        </a>
                                    </div>
                                    <div class="detail-box">
                                        <h5>Thứ năm ngon miệng</h5>
                                        <h6><span>Giảm </span> 20%</h6>
                                        <a href="DanhSachMonAn.aspx?id=<%# Eval("MaLoaiMonAn") %>">
                                            Đặt ngay bây giờ <i class="fas fa-arrow-right"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </section>

    <!-- about section -->
    <section class="about_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js-about" class="particles"></div>
            <div class="row">
                <div class="col-md-6">
                    <div class="img-box">
                        <img src="../Mau/images/anhkem.jpg" alt="">
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="detail-box">
                        <div class="heading_container">
                            <h2>Viet Kitchen Sài Gòn</h2>
                        </div>
                        <p>
                            Một bữa tiệc thịnh soạn giành riêng cho ngày cuối năm luôn là lựa chọn lý tưởng để quây quần bên gia đình và bạn bè. Vô vàn hải sản tươi ngon, đính kèm các món truyền thống ngày Tết hứa hẹn sẽ là phông nền hoàn hảo cho bữa sum vầy.
                        </p>
                        <a href="">Đọc tiếp</a>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- client section -->
    <section class="client_section layout_padding-bottom">
        <div class="container position-relative">
            <div id="particles-js-client" class="particles"></div>
            <div class="heading_container heading_center">
                <h2>Đôi lời review của khách hàng</h2>
            </div>
            <div class="carousel-wrap row">
                <div class="owl-carousel client_owl-carousel">
                    <div class="item">
                        <div class="box">
                            <div class="detail-box">
                                <p>
                                    Không gian của Viet Kitchen thật sự hiện đại và ấm cúng, rất phù hợp để tụ tập bạn bè hoặc gia đình. Các món ăn được chế biến tinh tế, kết hợp hoàn hảo giữa hương vị truyền thống Việt Nam và phong cách quốc tế.
                                </p>
                                <h6>Nguyễn Thị Thu</h6>
                                <p>Nhân viên kinh doanh</p>
                            </div>
                            <div class="img-box">
                                <img src="../Mau/images/client1.jpg" alt="" class="box-img">
                            </div>
                        </div>
                    </div>
                    <div class="item">
                        <div class="box">
                            <div class="detail-box">
                                <p>
                                    Đến Viet Kitchen là một trải nghiệm tuyệt vời! Thực đơn đa dạng, món ăn tươi ngon, trình bày đẹp mắt. Tôi rất ấn tượng với sự sáng tạo trong cách kết hợp các nguyên liệu địa phương.
                                </p>
                                <h6>Nguyễn Thảo My</h6>
                                <p>Sinh viên IT</p>
                            </div>
                            <div class="img-box">
                                <img src="../Mau/images/thaomy.jpg" alt="" class="box-img">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>