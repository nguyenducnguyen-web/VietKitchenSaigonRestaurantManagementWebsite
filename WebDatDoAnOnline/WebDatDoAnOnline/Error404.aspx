<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error404.aspx.cs" Inherits="WebDatDoAnOnline.Error404" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>404 - Không Tìm Thấy Trang</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/particles.js@2.0.0/particles.min.js"></script>
    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background: linear-gradient(135deg, #ff6f61, #ff0000, #ffca28);
            margin: 0;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            overflow: hidden;
            position: relative;
        }

        #particles-js {
            position: absolute;
            width: 100%;
            height: 100%;
            z-index: 1;
        }

        .container {
            text-align: center;
            z-index: 2;
            position: relative;
            padding: 40px;
            background: rgba(255, 255, 255, 0.15);
            border-radius: 25px;
            box-shadow: 0 15px 40px rgba(255, 0, 0, 0.5);
            backdrop-filter: blur(10px);
            border: 2px solid rgba(255, 255, 255, 0.3);
        }

        h1 {
            font-size: 12rem;
            color: #ffffff;
            text-shadow: 5px 5px 20px rgba(255, 111, 97, 0.8), 0 0 40px rgba(255, 202, 40, 0.6);
            margin: 0;
            animation: bounce 2s infinite;
        }

        h2 {
            font-size: 2.8rem;
            color: #ffca28;
            text-transform: uppercase;
            margin: 20px 0;
            text-shadow: 2px 2px 10px rgba(255, 0, 0, 0.7);
        }

        p {
            font-size: 1.3rem;
            color: #ffffff;
            margin-bottom: 30px;
            text-shadow: 1px 1px 5px rgba(0, 0, 0, 0.3);
        }

        .btn-home {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
            padding: 15px 40px;
            font-size: 1.5rem;
            font-weight: bold;
            border-radius: 50px;
            text-decoration: none;
            box-shadow: 0 10px 20px rgba(255, 0, 0, 0.6);
            transition: all 0.4s ease;
            display: inline-block;
        }

        .btn-home:hover {
            transform: scale(1.1) translateY(-5px);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
        }

        .floating-astronaut {
            position: absolute;
            width: 180px;
            animation: float 6s infinite ease-in-out;
            z-index: 3;
            filter: drop-shadow(0 0 10px rgba(255, 111, 97, 0.5));
        }

        .top-left { top: 10%; left: 10%; }
        .bottom-right { bottom: 10%; right: 10%; }

        @keyframes bounce {
            0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
            40% { transform: translateY(-30px); }
            60% { transform: translateY(-15px); }
        }

        @keyframes float {
            0%, 100% { transform: translateY(0) rotate(0deg); }
            50% { transform: translateY(-40px) rotate(15deg); }
        }

        /* Hiệu ứng thêm để đẹp hơn */
        .stars {
            position: absolute;
            width: 100%;
            height: 100%;
            background: url('https://www.transparenttextures.com/patterns/stardust.png');
            opacity: 0.3;
            z-index: 0;
            animation: twinkle 10s infinite;
        }

        @keyframes twinkle {
            0%, 100% { opacity: 0.3; }
            50% { opacity: 0.6; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="stars"></div>
        <div id="particles-js"></div>
       
        <div class="container">
            <h1>404</h1>
            <h2>Lạc Vào Vũ Trụ!</h2>
            <p>Trang bạn tìm kiếm không tồn tại trong hệ ngân hà này. Hãy quay về Trái Đất nhé!</p>
            <a href="NguoiDung/TrangChu.aspx" class="btn-home">Về Trang Chủ</a>
        </div>
    </form>

    <script>
        window.onload = function () {
            particlesJS("particles-js", {
                "particles": {
                    "number": { "value": 200, "density": { "enable": true, "value_area": 800 } },
                    "color": { "value": ["#ff6f61", "#ff0000", "#ffca28"] },
                    "shape": { "type": ["star", "circle"], "stroke": { "width": 0 } },
                    "opacity": { "value": 0.8, "random": true, "anim": { "enable": true, "speed": 1, "opacity_min": 0.1 } },
                    "size": { "value": 4, "random": true, "anim": { "enable": true, "speed": 3, "size_min": 0.5 } },
                    "move": { "enable": true, "speed": 4, "direction": "none", "random": true, "out_mode": "out" }
                },
                "interactivity": {
                    "events": { "onhover": { "enable": true, "mode": "repulse" }, "onclick": { "enable": true, "mode": "push" } },
                    "modes": { "repulse": { "distance": 100 }, "push": { "particles_nb": 10 } }
                },
                "retina_detect": true
            });
        };
    </script>
</body>
</html>