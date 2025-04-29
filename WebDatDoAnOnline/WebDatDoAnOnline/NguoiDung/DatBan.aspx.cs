using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DatBan : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            if (Page.IsValid) // Chỉ thực thi khi tất cả validation đều hợp lệ
            {
                try
                {
                    con = new SqlConnection(ConnectionSQL.GetConnectionString()); // Giả sử bạn có class ConnectionSQL
                    cmd = new SqlCommand("DatBanSp", con);
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@TenKhachHang", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@SdtDatBan", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmailDatBan", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoLuongKhach", Convert.ToInt32(ddlPeople.SelectedValue));
                    cmd.Parameters.AddWithValue("@NgayDatBan", txtDate.Text.Trim());
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.ExecuteNonQuery();

                    // Giữ nguyên successMessage và hiển thị trên lblMsg
                    string name = txtName.Text.Trim();
                    string phone = txtPhone.Text.Trim();
                    string email = txtEmail.Text.Trim();
                    string successMessage = $"Đặt bàn thành công cho {name}! Chúng tôi sẽ liên hệ qua {phone} hoặc {email}.";
                    lblMsg.Text = successMessage;
                    lblMsg.ForeColor = System.Drawing.Color.Green;

                    // Thêm script để ẩn lblMsg sau 15 giây
                    string hideScript = @"
                        setTimeout(function() {
                            var lblMsg = document.getElementById('" + lblMsg.ClientID + @"');
                            if (lblMsg) {
                                lblMsg.style.display = 'none';
                            }
                        }, 15000); // 
                    ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "HideMessage", hideScript, true);

                    // SweetAlert2 với màu cam đỏ, đỏ, trắng, và Nyan Cat
                    string successScript = @"
                        Swal.fire({
                            title: 'Đặt bàn thành công!',
                            html: 'Cảm ơn quý khách đã đặt bàn tại Việt Kitchen SG!<br>Chúng tôi sẽ liên hệ sớm nhất!',
                            icon: 'success',
                            confirmButtonText: 'Tuyệt vời!',
                            backdrop: `rgba(0, 0, 0, 0.7) url(""https://sweetalert2.github.io/images/nyan-cat.gif"") left top no-repeat`,
                            timer: 5000,
                            timerProgressBar: true,
                            customClass: {
                                popup: 'custom-swal-popup',
                                title: 'custom-swal-title',
                                htmlContainer: 'custom-swal-text',
                                confirmButton: 'custom-swal-button'
                            },
                            didOpen: () => {
                                document.getElementById('successSound').play();
                                confetti({
                                    particleCount: 150,
                                    spread: 90,
                                    colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                                    origin: { y: 0.6 }
                                });
                            }
                        });
                    ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert", successScript, true);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    string errorScript = $"Swal.fire({{ title: 'Lỗi!', text: '{ex.Message}', icon: 'error', confirmButtonText: 'OK', customClass: {{ popup: 'custom-swal-popup', title: 'custom-swal-title', htmlContainer: 'custom-swal-text', confirmButton: 'custom-swal-button' }} }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", errorScript, true);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            ddlPeople.SelectedIndex = 0;
            txtDate.Text = string.Empty;
        }
    }
}