<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="DanhSachMonAn.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.DanhSachMonAn" %>
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
            margin: 0;
            overflow-x: hidden;
            position: relative;
        }
        .food_section {
            padding: 80px 0;
            background: url('../Images/food-bg.jpg') no-repeat center/cover;
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
        .control-container {
            display: flex;
            justify-content: center;
            align-items: center;
            flex-wrap: wrap;
            margin: 20px 0;
        }
        .form-control {
            border: none;
            border-bottom: 3px solid #ff6f61;
            background: rgba(255, 255, 255, 0.9);
            padding: 12px;
            transition: all 0.4s ease;
            font-size: 1.1rem;
            color: #ff0000;
            border-radius: 10px;
            width: 300px;
            margin: 0 10px;
            text-align: center;
        }
        .form-control:focus {
            border-color: #ffca28;
            box-shadow: 0 5px 15px rgba(255, 202, 40, 0.5);
            background: rgba(255, 255, 255, 1);
        }
        .form-control::placeholder {
            text-align: center;
            color: #ff6f61;
            opacity: 0.8;
        }
        select.form-control {
            appearance: none;
            background: rgba(255, 255, 255, 0.9) url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 24 24"><path fill="%23ff6f61" d="M7 10l5 5 5-5z"/></svg>') no-repeat right 10px center;
            padding-right: 30px;
            text-align: center;
            text-align-last: center;
            color: #ff6f61;
        }
        select.form-control option {
            text-align: center;
            color: #ff0000;
        }
        .btn {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            color: #ffffff;
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
            box-shadow: 0 20px 40px rgba(255, 111, 97, 0.8);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .filters_menu {
            display: flex;
            justify-content: center;
            flex-wrap: wrap;
            margin: 30px 0;
        }
        .filters_menu li {
            background: rgba(255, 255, 255, 0.9);
            padding: 10px 20px;
            margin: 5px;
            border-radius: 25px;
            cursor: pointer;
            transition: all 0.3s ease;
            font-weight: bold;
            color: #ff6f61;
            box-shadow: 0 5px 15px rgba(255, 111, 97, 0.3);
        }
        .filters_menu li:hover, .filters_menu li.active {
            background: linear-gradient(45deg, #ff6f61, #ffca28);
            color: #ffffff;
            transform: scale(1.1);
        }
        .box {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            padding: 20px;
            box-shadow: 0 15px 40px rgba(255, 111, 97, 0.5);
            transition: all 0.4s ease;
            position: relative;
            overflow: hidden;
            animation: slideInUp 1s ease-in-out;
            cursor: pointer;
        }
        .box:hover {
            transform: translateY(-15px) scale(1.05);
            box-shadow: 0 25px 50px rgba(255, 0, 0, 0.7);
        }
        .img-box img {
            border-radius: 15px;
            width: 100%;
            height: 200px;
            object-fit: cover;
            transition: all 0.4s ease;
        }
        .box:hover .img-box img {
            transform: scale(1.1);
        }
        .detail-box h5 {
            color: #ff6f61;
            font-size: 1.5rem;
            font-weight: bold;
            text-shadow: 1px 1px 5px rgba(255, 111, 97, 0.5);
        }
        .detail-box p {
            color: #ffffff;
            font-size: 1rem;
        }
        .options h6 {
            color: #ffca28;
            font-size: 1.2rem;
            font-weight: bold;
        }
        .options a {
            background: linear-gradient(45deg, #ff0000, #ff6f61);
            padding: 10px;
            border-radius: 50%;
            box-shadow: 0 5px 15px rgba(255, 0, 0, 0.5);
            transition: all 0.3s ease;
        }
        .options a:hover {
            transform: scale(1.2);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .discount-outside {
            color: #00ff00;
            font-size: 1rem;
            font-weight: bold;
            margin: 5px 0;
        }
        .original-price {
            color: #ffffff;
            font-size: 0.9rem;
            text-decoration: line-through;
            margin-left: 5px;
        }
        @keyframes glow {
            0% { text-shadow: 0 0 10px #ff6f61; }
            100% { text-shadow: 0 0 20px #ffca28, 0 0 30px #ff0000; }
        }
        @keyframes slideInUp {
            from { transform: translateY(100px); opacity: 0; }
            to { transform: translateY(0); opacity: 1; }
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
        .product-image {
            width: 100%;
            height: 200px;
            object-fit: cover;
            border-radius: 15px;
            margin-bottom: 15px;
        }
        .price {
            color: #ffca28;
            font-size: 1.5rem;
            font-weight: bold;
            margin: 10px 0;
        }
        .discount {
            color: #00ff00;
            font-size: 1.2rem;
            font-weight: bold;
            margin: 5px 0;
        }
        .description {
            color: #ffffff;
            font-size: 1rem;
            margin-bottom: 15px;
        }
        .quantity-container {
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 15px 0;
        }
        .quantity-btn {
            background: linear-gradient(45deg, #ff6f61, #ffca28);
            color: #ffffff;
            border: none;
            padding: 5px 15px;
            font-size: 1.2rem;
            border-radius: 50px;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        .quantity-btn:hover {
            transform: scale(1.1);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .quantity-input {
            width: 50px;
            text-align: center;
            border: none;
            background: rgba(255, 255, 255, 0.9);
            color: #ff6f61;
            font-size: 1.2rem;
            margin: 0 10px;
            border-radius: 5px;
        }
    </style>

    <script>
        window.onload = function () {
            particlesJS("particles-js", {
                "particles": {
                    "number": { "value": 150, "density": { "enable": true, "value_area": 800 } },
                    "color": { "value": ["#ff6f61", "#ff0000", "#ffca28"] },
                    "shape": { "type": "circle" },
                    "opacity": { "value": 0.8, "random": true, "anim": { "enable": true, "speed": 1 } },
                    "size": { "value": 4, "random": true, "anim": { "enable": true, "speed": 2 } },
                    "move": { "enable": true, "speed": 3, "direction": "none", "random": true, "out_mode": "out" }
                },
                "interactivity": {
                    "events": { "onhover": { "enable": true, "mode": "repulse" }, "onclick": { "enable": true, "mode": "push" } },
                    "modes": { "repulse": { "distance": 100 }, "push": { "particles_nb": 5 } }
                },
                "retina_detect": true
            });
        };

        function showProductDetails(productId, name, price, imageUrl, description, discount) {
            let quantity = 1;
            const discountedPrice = discount !== null && discount !== "null" ? price * (1 - discount / 100) : price;
            const htmlContent = `
                <img src="${imageUrl}" class="product-image" alt="${name}"/>
                <div class="price">${discountedPrice.toLocaleString('vi-VN')} đ ${discount !== null && discount !== "null" ? `(Giá gốc: ${price.toLocaleString('vi-VN')} đ)` : ''}</div>
                ${discount !== null && discount !== "null" ? `<div class="discount">Khuyến mãi: ${discount}%</div>` : ''}
                <div class="description">${description}</div>
                <div class="quantity-container">
                    <button type="button" class="quantity-btn" onclick="updateQuantity(-1)">-</button>
                    <input type="text" class="quantity-input" id="quantityInput" value="${quantity}" readonly/>
                    <button type="button" class="quantity-btn" onclick="updateQuantity(1)">+</button>
                </div>
            `;

            Swal.fire({
                title: name,
                html: htmlContent,
                showConfirmButton: true,
                showCancelButton: true,
                confirmButtonText: 'Đặt hàng ngay <i class="fas fa-cart-plus"></i>',
                cancelButtonText: 'Đóng',
                backdrop: `rgba(0, 0, 0, 0.7) url('https://sweetalert2.github.io/images/nyan-cat.gif') left top no-repeat`,
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button',
                    cancelButton: 'custom-swal-button'
                },
                preConfirm: () => {
                    return { quantity: quantity };
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    addToCart(productId, result.value.quantity, discountedPrice);
                }
            });

            window.updateQuantity = function (delta) {
                quantity = Math.max(1, quantity + delta);
                document.getElementById('quantityInput').value = quantity;
            };
        }

        function addToCart(productId, quantity, discountedPrice) {
            fetch('DanhSachMonAn.aspx/AddToCart', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({ productId: productId, quantity: quantity, giaMonAnSauKhuyenMai: discountedPrice })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d === "Success") {
                        Swal.fire({
                            title: 'Thêm vào giỏ hàng thành công!',
                            html: 'Món ăn đã được thêm vào giỏ hàng của bạn!',
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
                        }).then(() => {
                            window.location.replace('GioHang.aspx');
                        });
                    } else if (data.d === "NotLoggedIn") {
                        Swal.fire({
                            title: 'Chưa đăng nhập!',
                            html: 'Vui lòng đăng nhập để thêm món vào giỏ hàng!',
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
                        }).then(() => {
                            window.location.replace('DangNhap.aspx');
                        });
                    } else {
                        Swal.fire({
                            title: 'Lỗi!',
                            html: 'Có lỗi xảy ra: ' + data.d,
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
                })
                .catch(error => {
                    Swal.fire({
                        title: 'Lỗi!',
                        html: 'Có lỗi xảy ra: ' + error.message,
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
                });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="food_section layout_padding">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>
            <div class="heading_container heading_center">
                <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                <h2>Thực đơn của chúng tôi</h2>

                <div class="control-container">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Tìm món ăn yêu thích..." />
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Xóa" CssClass="btn" OnClick="btnClear_Click" />
                    <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged" CssClass="form-control">
                        <asp:ListItem Value="" Selected="True">Sắp xếp theo</asp:ListItem>
                        <asp:ListItem Value="price_desc">Giá: Cao đến thấp</asp:ListItem>
                        <asp:ListItem Value="price_asc">Giá: Thấp đến cao</asp:ListItem>
                        <asp:ListItem Value="bestseller">Bán chạy nhất</asp:ListItem>
                        <asp:ListItem Value="newest">Món mới nhất</asp:ListItem>
                        <asp:ListItem Value="discount_desc">Giảm sâu nhất</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn" OnClick="btnRefresh_Click" />
                </div>
            </div>

            <ul class="filters_menu">
                <li class="active" data-filter="*" data-id="0">Tất cả</li>
                <asp:Repeater ID="rCategory" runat="server">
                    <ItemTemplate>
                        <li data-filter=".<%# Regex.Replace(Eval("TenLoaiMonAn").ToString().ToLower(), @"\s+", "") %>"
                            data-id="<%# Eval("MaLoaiMonAn") %>"><%# Eval("TenLoaiMonAn") %></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>

            <div class="filters-content">
                <div class="row grid">
                    <asp:Repeater ID="rProducts" runat="server" OnItemCommand="rProducts_ItemCommand">
                        <ItemTemplate>
                            <div class="col-sm-6 col-lg-4 all <%# Regex.Replace(Eval("CategoryName").ToString().ToLower(), @"\s+", "") %>">
                                <div class="box" onclick='showProductDetails(
                                    "<%# Eval("MaMonAn") %>",
                                    "<%# Eval("TenMonAn") %>",
                                    <%# Eval("GiaMonAn") %>,
                                    "<%# Utils.GetImageUrl(Eval("DiaChiAnhMonAn")) %>",
                                    "<%# Eval("MoTaMonAn") %>",
                                    <%# Eval("KhuyenMai") != DBNull.Value ? Eval("KhuyenMai") : "null" %>
                                )'>
                                    <div>
                                        <div class="img-box">
                                            <img src='<%# Utils.GetImageUrl(Eval("DiaChiAnhMonAn")) %>' alt='<%# Eval("TenMonAn") %>' />
                                        </div>
                                        <div class="detail-box">
                                            <h5><%# Eval("TenMonAn") %></h5>
                                            <p><%# Eval("MoTaMonAn") %></p>
                                            <div class="options">
                                                <h6>
                                                    <%# Eval("KhuyenMai") != DBNull.Value ? ((double)Eval("GiaMonAn") * (1 - (double)Eval("KhuyenMai") / 100)).ToString("N0") + " VNĐ" : Eval("GiaMonAn", "{0:N0} VNĐ") %>
                                                    <%# Eval("KhuyenMai") != DBNull.Value ? "<span class=\"original-price\">" + Eval("GiaMonAn", "{0:N0} VNĐ") + "</span>" : "" %>
                                                </h6>
                                                <asp:LinkButton runat="server" ID="lbAddToCart" CommandName="addToCart" CommandArgument='<%# Eval("MaMonAn") %>' OnClientClick="event.stopPropagation();">
                                                    <i class="fas fa-cart-plus" style="color: #fff;"></i>
                                                </asp:LinkButton>
                                            </div>
                                            <%# Eval("KhuyenMai") != DBNull.Value ? "<p class=\"discount-outside\">Khuyến mãi: " + Eval("KhuyenMai") + "%</p>" : "" %>
                                            <p style="color: #ff4500; font-size: 14px;">
                                                Đã bán: <%# Eval("YearlyOrders", "{0:N0}") %>
                                            </p>
                                            <p style="color: #008000; font-size: 14px;">
                                                Ngày thêm: <%# Eval("NgayTaoMonAn", "{0:dd/MM/yyyy}") %>
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </section>
</asp:Content>