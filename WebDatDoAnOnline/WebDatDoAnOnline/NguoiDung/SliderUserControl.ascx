<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SliderUserControl.ascx.cs" Inherits="WebDatDoAnOnline.NguoiDung.SliderUserControl" %>

<link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
<script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js"></script>
<audio id="welcomeSound" src="/Audio/lovely.m4a" preload="auto"></audio>

<style>
    .slider_section {
        position: relative;
        padding: 100px 0;
        background: url('../Images/slider-bg.jpg') no-repeat center/cover;
        box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.5);
        overflow: hidden;
    }
    .carousel-inner {
        position: relative;
        z-index: 2;
    }
    .detail-box h1 {
        color: #ffffff;
        font-size: 4rem;
        text-transform: uppercase;
        text-shadow: 3px 3px 15px rgba(255, 111, 97, 0.8);
        animation: glow 2s infinite alternate;
    }
    .detail-box p {
        color: #fff;
        font-size: 1.2rem;
        line-height: 1.8;
        text-shadow: 1px 1px 5px rgba(0, 0, 0, 0.7);
        animation: fadeInUp 1.5s ease-in-out;
    }
    .btn-box a {
        background: linear-gradient(45deg, #ff0000, #ff6f61);
        color: #ffffff;
        padding: 15px 40px;
        border-radius: 50px;
        font-weight: bold;
        font-size: 1.2rem;
        box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
        transition: all 0.4s ease;
        text-decoration: none;
        display: inline-flex;
        align-items: center;
    }
    .btn-box a i {
        margin-left: 10px;
    }
    .btn-box a:hover {
        transform: scale(1.1) translateY(-5px);
        background: linear-gradient(45deg, #ffca28, #ff6f61);
        box-shadow: 0 15px 30px rgba(255, 111, 97, 0.8);
    }
    /* Thiết kế lại nút điều hướng carousel */
    .carousel-indicators {
        position: absolute;
        bottom: 20px;
        z-index: 3;
        display: flex;
        justify-content: center;
        align-items: center;
        gap: 15px; /* Khoảng cách giữa các nút */
    }
    .carousel-indicators li {
        width: 20px;
        height: 20px;
        border-radius: 50%;
        background: rgba(255, 111, 97, 0.6); /* Màu nền mờ ban đầu */
        border: 2px solid #ffca28;
        cursor: pointer;
        transition: all 0.4s ease;
        box-shadow: 0 0 10px rgba(255, 111, 97, 0.5);
        position: relative;
        overflow: hidden;
    }
    .carousel-indicators li::before {
        content: '';
        position: absolute;
        top: 50%;
        left: 50%;
        width: 0;
        height: 0;
        background: radial-gradient(circle, #ff6f61, #ff0000);
        border-radius: 50%;
        transform: translate(-50%, -50%);
        transition: width 0.4s ease, height 0.4s ease;
    }
    .carousel-indicators li:hover::before,
    .carousel-indicators li.active::before {
        width: 200%;
        height: 200%;
    }
    .carousel-indicators li:hover,
    .carousel-indicators li.active {
        transform: scale(1.3);
        background: #ffca28; /* Màu nổi bật khi active hoặc hover */
        box-shadow: 0 0 20px rgba(255, 202, 40, 0.8);
    }
    .particles {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 1;
        pointer-events: none;
    }
    @keyframes glow {
        0% { text-shadow: 0 0 10px #ff6f61; }
        100% { text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000; }
    }
    @keyframes fadeInUp {
        from { transform: translateY(50px); opacity: 0; }
        to { transform: translateY(0); opacity: 1; }
    }
    @keyframes slideInLeft {
        from { transform: translateX(-100%); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }
</style>

<section class="slider_section">
    <div id="particles-js-slider" class="particles"></div>
   
    <div id="customCarousel1" class="carousel slide" data-ride="carousel">
        <div class="carousel-inner">
            <div class="carousel-item active">
                <div class="container">
                    <div class="row">
                        <div class="col-md-7 col-lg-6">
                            <div class="detail-box" style="animation: slideInLeft 1.5s ease-in-out;">
                                <h1>Viet Kitchen Sài Gòn</h1>
                                <p>
                                    Viet Kitchen là một địa điểm ăn uống và giao lưu theo phong cách sống hiện đại tại Thành phố Hồ Chí Minh, nơi thiết kế và không gian hiện đại hòa quyện cùng hương vị truyền thống và quốc tế. Các món ăn quốc tế kết hợp với hương vị Việt Nam địa phương mang đến sự đổi mới độc đáo cho những món ăn kinh điển gần gũi và xa lạ.
                                </p>
                                <div class="btn-box">
                                    <a href="DanhSachMonAn.aspx" class="btn1">Đặt ngay bây giờ <i class="fas fa-arrow-right"></i></a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="carousel-item">
                <div class="container">
                    <div class="row">
                        <div class="col-md-7 col-lg-6">
                            <div class="detail-box" style="animation: slideInLeft 1.5s ease-in-out;">
                                <h1>Viet Kitchen Sài Gòn</h1>
                                <p>
                                    Nguyên liệu tươi ngon, các loại thảo mộc đặc biệt, và những món ăn quốc tế yêu thích kết hợp tạo nên các món ăn được thực khách ưa chuộng.
                                </p>
                                <div class="btn-box">
                                    <a href="DanhSachMonAn.aspx" class="btn1">Đặt ngay bây giờ <i class="fas fa-arrow-right"></i></a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="carousel-item">
                <div class="container">
                    <div class="row">
                        <div class="col-md-7 col-lg-6">
                            <div class="detail-box" style="animation: slideInLeft 1.5s ease-in-out;">
                                <h1>Viet Kitchen Sài Gòn</h1>
                                <p>
                                    Cùng với dịch vụ thân thiện, ấm áp, thiết kế độc đáo và những bàn ăn chung của nhà hàng biến không gian trở thành trung tâm xã hội, lý tưởng cho những bữa trưa nhanh đầy hương vị hay bữa tối sang trọng bên bạn bè và người thân yêu.
                                </p>
                                <div class="btn-box">
                                    <a href="DanhSachMonAn.aspx" class="btn1">Đặt ngay bây giờ <i class="fas fa-arrow-right"></i></a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="container">
            <ol class="carousel-indicators">
                <li data-target="#customCarousel1" data-slide-to="0" class="active"></li>
                <li data-target="#customCarousel1" data-slide-to="1"></li>
                <li data-target="#customCarousel1" data-slide-to="2"></li>
            </ol>
        </div>
    </div>
</section>

<script>
    window.onload = function () {
        particlesJS("particles-js-slider", {
            "particles": {
                "number": { "value": 150, "density": { "enable": true, "value_area": 800 } },
                "color": { "value": ["#ff6f61", "#ff0000", "#ffca28"] },
                "shape": { "type": "circle" },
                "opacity": { "value": 0.7, "random": true, "anim": { "enable": true, "speed": 1 } },
                "size": { "value": 4, "random": true, "anim": { "enable": true, "speed": 2 } },
                "move": { "enable": true, "speed": 3, "direction": "none", "random": true, "out_mode": "out" }
            },
            "interactivity": {
                "events": { "onhover": { "enable": true, "mode": "repulse" }, "onclick": { "enable": true, "mode": "push" } },
                "modes": { "repulse": { "distance": 100 }, "push": { "particles_nb": 5 } }
            },
            "retina_detect": true
        });

        const welcomeSound = document.getElementById('welcomeSound');
        welcomeSound.play();
        confetti({
            particleCount: 100,
            spread: 70,
            origin: { y: 0.6 }
        });
    };

    document.querySelectorAll('.btn1').forEach(btn => {
        btn.addEventListener('click', function () {
            confetti({
                particleCount: 150,
                spread: 100,
                colors: ['#ff6f61', '#ff0000', '#ffca28']
            });
        });
    });
</script>