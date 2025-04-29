using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class LienHe : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                con = new SqlConnection(ConnectionSQL.GetConnectionString());
                cmd = new SqlCommand("ContactSp", con);
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim());
                cmd.Parameters.AddWithValue("@Message", txtMessage.Text.Trim());
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();

                // SweetAlert2 với màu cam đỏ, đỏ, trắng, và Nyan Cat
                string script = @"
            Swal.fire({
                title: 'Thành công!',
                html: 'Cảm ơn quý khách đã liên hệ với chúng tôi!<br>Chúng tôi sẽ xem xét yêu cầu/thắc mắc của quý khách!',
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", script, true);
                clear();
            }
            catch (Exception ex)
            {
                string errorScript = $"Swal.fire({{ title: 'Lỗi!', text: '{ex.Message}', icon: 'error', confirmButtonText: 'OK' }});";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
            }
            finally
            {
                con.Close();
            }
        }

        private void clear()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtSubject.Text = string.Empty;
            txtMessage.Text = string.Empty;
        }
    }
}