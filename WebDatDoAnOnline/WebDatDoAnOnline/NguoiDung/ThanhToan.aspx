<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="ThanhToan.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.ThanhToan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Thêm các thư viện cần thiết -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.5.1/dist/confetti.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/particles.js/2.0.0/particles.min.js"></script>
    <audio id="successSound" src="/Audio/chienthang.mp3" preload="auto"></audio> 

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background: linear-gradient(135deg, #00c4cc, #007bff, #ff6f61);
            overflow-x: hidden;
            margin: 0;
            position: relative;
        }
        .book_section {
            padding: 80px 0;
            background: url('../Images/payment-bg.jpg') no-repeat center/cover;
            box-shadow: inset 0 0 0 2000px rgba(0, 0, 0, 0.6);
            min-height: calc(100vh - 200px);
            position: relative;
            overflow: hidden;
        }
        .heading_container h1 {
            color: #ffffff;
            font-size: 3.5rem;
            text-transform: uppercase;
            text-shadow: 3px 3px 15px rgba(0, 123, 255, 0.8);
            animation: glow 2s infinite alternate;
        }
        .card {
            background: rgba(255, 255, 255, 0.95);
            padding: 40px;
            border-radius: 25px;
            box-shadow: 0 15px 40px rgba(0, 123, 255, 0.5);
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
            background: radial-gradient(circle, rgba(0, 196, 204, 0.3), transparent);
            animation: sparkle 5s infinite;
            pointer-events: none;
        }
        .form-control {
            border: none;
            border-bottom: 3px solid #00c4cc;
            background: transparent;
            padding: 12px;
            transition: all 0.4s ease;
            font-size: 1.1rem;
            color: #007bff;
        }
        .form-control:focus {
            border-color: #ff6f61;
            box-shadow: 0 5px 15px rgba(255, 111, 97, 0.5);
            background: rgba(255, 255, 255, 0.1);
        }
        .nav-pills .nav-link {
            background: rgba(255, 255, 255, 0.8);
            color: #007bff;
            border-radius: 50px;
            margin: 5px;
            transition: all 0.4s ease;
        }
        .nav-pills .nav-link.active {
            background: linear-gradient(45deg, #00c4cc, #007bff);
            color: #ffffff;
            box-shadow: 0 10px 20px rgba(0, 123, 255, 0.6);
        }
        .btn {
            background: linear-gradient(45deg, #007bff, #00c4cc);
            color: #ffffff;
            padding: 15px 50px;
            font-size: 1.2rem;
            font-weight: bold;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 10px 20px rgba(0, 123, 255, 0.6);
            transition: all 0.4s ease;
        }
        .btn:hover {
            transform: scale(1.1) translateY(-10px);
            box-shadow: 0 20px 40px rgba(0, 196, 204, 0.8);
            background: linear-gradient(45deg, #ff6f61, #00c4cc);
        }
        .badge-success {
            background: linear-gradient(45deg, #00c4cc, #007bff);
            padding: 10px 20px;
            font-size: 1.2rem;
            border-radius: 20px;
            box-shadow: 0 5px 15px rgba(0, 123, 255, 0.5);
        }
        .floating-gif {
            position: absolute;
            z-index: 1;
            animation: float 6s infinite ease-in-out;
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
        /* CSS cho giao diện QR */
        #qrCodeContainer {
            text-align: center;
            margin-top: 20px;
            padding: 20px;
            background: rgba(255, 255, 255, 0.9);
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(0, 123, 255, 0.4);
            animation: fadeIn 1s ease-in-out;
        }
        #qrCodeImage {
            width: 200px;
            height: 200px;
            border: 5px solid #00c4cc;
            border-radius: 10px;
            padding: 10px;
            background: #ffffff;
        }
        .qr-info {
            color: #007bff;
            font-size: 1.1rem;
            margin-top: 15px;
            text-align: left;
        }
        .qr-info img {
            width: 30px;
            height: 30px;
            vertical-align: middle;
            margin-left: 5px;
        }
        .qr-buttons {
            margin-top: 20px;
        }
        .btn-cancel {
            background: linear-gradient(45deg, #ff6f61, #ff0000);
            color: #ffffff;
            padding: 10px 30px;
            font-size: 1rem;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 5px 15px rgba(255, 111, 97, 0.6);
            transition: all 0.4s ease;
            margin-right: 10px;
        }
        .btn-cancel:hover {
            transform: scale(1.1) translateY(-5px);
            box-shadow: 0 10px 20px rgba(255, 111, 97, 0.8);
            background: linear-gradient(45deg, #ffca28, #ff6f61);
        }
        .btn-confirm {
            background: linear-gradient(45deg, #00c4cc, #007bff);
            color: #ffffff;
            padding: 10px 30px;
            font-size: 1rem;
            border-radius: 50px;
            text-transform: uppercase;
            box-shadow: 0 5px 15px rgba(0, 123, 255, 0.6);
            transition: all 0.4s ease;
        }
        .btn-confirm:hover {
            transform: scale(1.1) translateY(-5px);
            box-shadow: 0 10px 20px rgba(0, 196, 204, 0.8);
            background: linear-gradient(45deg, #ff6f61, #00c4cc);
        }
        /*@keyframes glow {
            0% { text-shadow: 0 0 10px #00c4cc; }
            100% { text-shadow: 0 0 20px #ff6f61, 0 0 30px #007bff; }
        }
        @keyframes sparkle {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        @keyframes float {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-20px); }
        }
        @keyframes fadeIn {
            0% { opacity: 0; transform: translateY(20px); }
            100% { opacity: 1; transform: translateY(0); }
        }*/
        .line-spacing {
            margin-bottom: 10px;
        }
    </style>

    <script>
        window.onload = function () {
            particlesJS("particles-js", {
                "particles": {
                    "number": { "value": 150, "density": { "enable": true, "value_area": 800 } },
                    "color": { "value": ["#00c4cc", "#007bff", "#ff6f61"] },
                    "shape": { "type": "circle" },
                    "opacity": { "value": 0.8, "random": true, "anim": { "enable": true, "speed": 1, "opacity_min": 0.1 } },
                    "size": { "value": 4, "random": true, "anim": { "enable": true, "speed": 3, "size_min": 0.5 } },
                    "move": { "enable": true, "speed": 3, "direction": "none", "random": true, "out_mode": "out" }
                },
                "interactivity": {
                    "events": { "onhover": { "enable": true, "mode": "repulse" }, "onclick": { "enable": true, "mode": "push" } },
                    "modes": { "repulse": { "distance": 100 }, "push": { "particles_nb": 5 } }
                },
                "retina_detect": true
            });

            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=lblMsg.ClientID %>").style.display = "none";
            }, seconds * 1000);
        };

        function showSuccessAlert(message, redirectUrl) {
            Swal.fire({
                title: 'Thành công!',
                text: message,
                icon: 'success',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                },
                didOpen: () => {
                    confetti({
                        particleCount: 100,
                        spread: 70,
                        origin: { y: 0.6 }
                    });
                    document.getElementById('successSound').play();
                }
            }).then(() => {
                if (redirectUrl) window.location.href = redirectUrl;
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

        function showQRCode() {
            var address = document.getElementById('<%= txtQRAddress.ClientID %>').value;
            if (!address) {
                showErrorAlert('Vui lòng nhập địa chỉ nhận hàng!');
                return false;
            }
            document.getElementById('qrCodeContainer').style.display = 'block';
            return false;
        }

        function hideQRCode() {
            document.getElementById('qrCodeContainer').style.display = 'none';
        }

        function DisableBackButton() {
            window.history.forward();
        }
        DisableBackButton();
        window.onload = DisableBackButton;
        window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() };
        window.onunload = function () { void (0) };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="book_section">
        <div class="container position-relative">
            <div id="particles-js" class="particles"></div>
            <div class="heading_container text-center">
                <h1>Thanh Toán Hóa Đơn</h1>
            </div>
            <div class="row pb-5">
                <div class="col-lg-6 mx-auto">
                    <div class="card">
                        <div class="card-header">
                            <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert"></asp:Label>
                            <ul role="tablist" class="nav nav-pills nav-fill mb-3">
                                <li class="nav-item"><a data-toggle="pill" href="#credit-card" class="nav-link active"><i class="fa fa-credit-card mr-2"></i>Thẻ tín dụng</a></li>
                                <li class="nav-item"><a data-toggle="pill" href="#paypal" class="nav-link"><i class="fa fa-money mr-2"></i>COD</a></li>
                                <li class="nav-item"><a data-toggle="pill" href="#qr-code" class="nav-link"><i class="fa fa-qrcode mr-2"></i>Thanh toán bằng QR</a></li>
                            </ul>
                        </div>
                        <div class="tab-content">
                            <!-- Thanh toán bằng thẻ tín dụng -->
                            <div id="credit-card" class="tab-pane fade show active pt-3">
                                <div role="form">
                                    <div class="form-group">
                                        <label>
                                            <h6>Tên chủ thẻ</h6>
                                        </label>
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên chủ thẻ"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Bắt buộc nhập tên" ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Tên chỉ chứa chữ cái" ValidationExpression="^[a-zA-Z\s]+$" ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RegularExpressionValidator>
                                    </div>
                                    <div class="form-group">
                                        <label>
                                            <h6>Số thẻ</h6>
                                        </label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtCardNo" runat="server" CssClass="form-control" placeholder="Số thẻ (16 số)" TextMode="Number"></asp:TextBox>
                                            <div class="input-group-append">
                                                <span class="input-group-text text-muted">
                                                    <i class="fa fa-cc-visa mx-1"></i>
                                                    <i class="fa fa-cc-mastercard mx-1"></i>
                                                    <i class="fa fa-cc-amex mx-1"></i>
                                                </span>
                                            </div>
                                        </div>
                                        <asp:RequiredFieldValidator ID="rfvCardNo" runat="server" ErrorMessage="Bắt buộc nhập số thẻ" ControlToValidate="txtCardNo" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Số thẻ phải 16 số" ValidationExpression="[0-9]{16}" ControlToValidate="txtCardNo" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RegularExpressionValidator>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-8">
                                            <div class="form-group">
                                                <label>
                                                    <h6>Ngày hết hạn</h6>
                                                </label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtExpMonth" runat="server" CssClass="form-control" placeholder="MM" TextMode="Number"></asp:TextBox>
                                                    <asp:TextBox ID="txtExpYear" runat="server" CssClass="form-control" placeholder="YYYY" TextMode="Number"></asp:TextBox>
                                                </div>
                                                <asp:RequiredFieldValidator ID="rfvExpMonth" runat="server" ErrorMessage="Bắt buộc nhập tháng" ControlToValidate="txtExpMonth" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="Tháng phải 2 số" ValidationExpression="[0-9]{2}" ControlToValidate="txtExpMonth" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="rfvExpYear" runat="server" ErrorMessage="Bắt buộc nhập năm" ControlToValidate="txtExpYear" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ErrorMessage="Năm phải 4 số" ValidationExpression="[0-9]{4}" ControlToValidate="txtExpYear" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="col-sm-4">
                                            <div class="form-group">
                                                <label data-toggle="tooltip" title="Mã CVV 3 số ở mặt sau thẻ">
                                                    <h6>CVV <i class="fa fa-question-circle"></i></h6>
                                                </label>
                                                <asp:TextBox ID="txtCvv" runat="server" CssClass="form-control" placeholder="CVV" TextMode="Number"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvCvv" runat="server" ErrorMessage="Bắt buộc nhập CVV" ControlToValidate="txtCvv" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ErrorMessage="CVV phải 3 số" ValidationExpression="[0-9]{3}" ControlToValidate="txtCvv" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label>
                                            <h6>Địa chỉ nhận hàng</h6>
                                        </label>
                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Địa chỉ nhận hàng" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Bắt buộc nhập địa chỉ" ControlToValidate="txtAddress" ForeColor="Red" Display="Dynamic" ValidationGroup="card">*</asp:RequiredFieldValidator>
                                    </div>
                                    <div class="text-center">
                                        <asp:LinkButton ID="lbCardSubmit" runat="server" CssClass="btn" ValidationGroup="card" OnClick="lbCardSubmit_Click">Xác nhận thanh toán</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <!-- Thanh toán COD -->
                            <div id="paypal" class="tab-pane fade pt-3">
                                <div class="form-group">
                                    <label><h6>Địa chỉ nhận hàng</h6></label>
                                    <asp:TextBox ID="txtCODAddress" runat="server" CssClass="form-control" placeholder="Địa chỉ nhận hàng" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCODAddress" runat="server" ErrorMessage="Bắt buộc nhập địa chỉ" ControlToValidate="txtCODAddress" ForeColor="Red" Display="Dynamic" ValidationGroup="cod">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="text-center">
                                    <asp:LinkButton ID="lbCodSubmit" runat="server" CssClass="btn" ValidationGroup="cod" OnClick="lbCodSubmit_Click"><i class="fa fa-cart-arrow-down mr-2"></i>Xác nhận đơn hàng</asp:LinkButton>
                                </div>
                                <p class="text-muted text-center mt-3">Lưu ý: Thanh toán đầy đủ khi nhận hàng.</p>
                            </div>
                            <!-- Thanh toán bằng QR -->
                            <div id="qr-code" class="tab-pane fade pt-3">
                                <div class="form-group">
                                    <label>
                                        <h6>Địa chỉ nhận hàng</h6>
                                    </label>
                                    <asp:TextBox ID="txtQRAddress" runat="server" CssClass="form-control" placeholder="Địa chỉ nhận hàng" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvQRAddress" runat="server" ErrorMessage="Bắt buộc nhập địa chỉ" ControlToValidate="txtQRAddress" ForeColor="Red" Display="Dynamic" ValidationGroup="qr">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="text-center">
                                    <button type="button" class="btn" onclick="showQRCode()">Hiển thị QR Code</button>
                                </div>
                                <div id="qrCodeContainer" style="display: none;">
                                    <img id="qrCodeImage" src="/Images/qrthanhtoan.png" alt="QR Code Thanh Toán" />
                                    <div class="qr-info">
                                        <p><strong>Ngân hàng:</strong> Ngân hàng quân đội MB Bank
                                            <img src="/Images/mbbanklogo.png" alt="MB Bank Logo" /></p>
                                        <p><strong>Tên tài khoản:</strong> Nguyễn Đức Nguyên</p>
                                        <p><strong>Số tiền:</strong> <% Response.Write(Session["grandTotalPriceWithVAT"] != null ? String.Format("{0:N0} VND", Convert.ToDecimal(Session["grandTotalPriceWithVAT"])) : "0 VND"); %></p>
                                        <p><strong>Nội dung:</strong> <% Response.Write(Session["OrderNo"] != null ? Session["OrderNo"].ToString() : "N/A"); %></p>
                                    </div>
                                    <div class="qr-buttons">
                                        <button class="btn-cancel" onclick="hideQRCode()">Hủy bỏ</button>
                                        <asp:LinkButton ID="lbQRSubmit" runat="server" CssClass="btn-confirm" ValidationGroup="qr" OnClick="lbQRSubmit_Click">Xác nhận đã thanh toán</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-center">
                            <b class="badge badge-success">
                                <div class="line-spacing">
                                    Tổng tiền trước thuế: <% Response.Write(Session["grandTotalPrice"] != null ? String.Format("{0:N0} VND", Convert.ToDecimal(Session["grandTotalPrice"])) : "0 VND"); %>
                                </div>
                                <div class="line-spacing">
                                    Thuế GTGT (8%): <% Response.Write(Session["grandTotalPrice"] != null ? String.Format("{0:N0} VND", Convert.ToDecimal(Session["grandTotalPrice"]) * 0.08m) : "0 VND"); %>
                                </div>
                                <div>
                                    Tổng tiền sau thuế: <% Response.Write(Session["grandTotalPriceWithVAT"] != null ? String.Format("{0:N0} VND", Convert.ToDecimal(Session["grandTotalPriceWithVAT"])) : "0 VND"); %>
                                </div>
                            </b>
                            <div class="pt-1">
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="Red" ValidationGroup="card" HeaderText="Vui lòng sửa các lỗi sau:" />
                                <asp:ValidationSummary ID="ValidationSummary2" runat="server" ForeColor="Red" ValidationGroup="cod" HeaderText="Vui lòng sửa các lỗi sau:" />
                                <asp:ValidationSummary ID="ValidationSummary3" runat="server" ForeColor="Red" ValidationGroup="qr" HeaderText="Vui lòng sửa các lỗi sau:" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>