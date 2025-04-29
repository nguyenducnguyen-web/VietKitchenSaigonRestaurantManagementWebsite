using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web;

namespace WebDatDoAnOnline.NguoiDung
{
    public partial class DanhSachMonAn : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                getCategories();
                getProducts();
                string welcomeScript = @"
                    Swal.fire({
                        title: 'Chào mừng đến thực đơn!',
                        text: 'Khám phá những món ăn ngon tuyệt ngay bây giờ nhé!',
                        icon: 'success',
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            content: 'custom-swal-text',
                            confirmButton: 'custom-swal-button'
                        },
                        backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/3o6Zt6KHxJTbXCnSso/giphy.gif') center center no-repeat`,
                        didOpen: () => {
                            document.getElementById('successSound').play();
                            confetti({
                                particleCount: 200,
                                spread: 100,
                                colors: ['#ff6f61', '#ff0000', '#ffca28'],
                                origin: { y: 0.6 }
                            });
                        }
                    });
                ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "WelcomeAlert", welcomeScript, true);
            }
        }

        private void getCategories()
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Category_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "ACTIVECAT");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rCategory.DataSource = dt;
            rCategory.DataBind();
        }

        private void getProducts(string searchText = "", string sortOrder = "")
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            string query = @"
                SELECT 
                    m.*, 
                    l.TenLoaiMonAn AS CategoryName,
                    ISNULL((
                        SELECT SUM(o.SoLuong)
                        FROM Orders o
                        WHERE o.MaMonAn = m.MaMonAn
                        AND YEAR(o.NgayTaoOrders) = YEAR(GETDATE())
                    ), 0) AS YearlyOrders
                FROM MonAn m
                INNER JOIN LoaiMonAn l ON m.MaLoaiMonAn = l.MaLoaiMonAn
                WHERE m.CoHoatDongMonAn = 1";

            if (!string.IsNullOrEmpty(searchText))
            {
                query += " AND m.TenMonAn LIKE @SearchText";
            }

            if (!string.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder)
                {
                    case "price_desc": query += " ORDER BY m.GiaMonAn DESC"; break;
                    case "price_asc": query += " ORDER BY m.GiaMonAn ASC"; break;
                    case "bestseller": query += " ORDER BY YearlyOrders DESC"; break;
                    case "newest": query += " ORDER BY m.NgayTaoMonAn DESC"; break;
                    case "discount_desc": query += " ORDER BY ISNULL(m.KhuyenMai, 0) DESC"; break;
                }
            }

            cmd = new SqlCommand(query, con);
            if (!string.IsNullOrEmpty(searchText))
            {
                cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
            }

            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rProducts.DataSource = dt;
            rProducts.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string sortOrder = ddlSort.SelectedValue;
            getProducts(searchText, sortOrder);

            if (dt.Rows.Count == 0)
            {
                string script = @"
                    Swal.fire({
                        title: 'Oops! Không tìm thấy!',
                        html: 'Không có món ăn nào phù hợp với từ khóa <b>' + '" + searchText + @"' + '</b> cả! Thử từ khóa khác nhé!',
                        icon: 'warning',
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            htmlContainer: 'custom-swal-text',
                            confirmButton: 'custom-swal-button'
                        },
                        backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/3o7TKTDn976rzVgig8/giphy.gif') center center no-repeat`,
                        didOpen: () => {
                            document.getElementById('errorSound').play();
                        }
                    });
                ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "WarningAlert", script, true);
            }
            else
            {
                string script = @"
                    Swal.fire({
                        title: 'Tìm thấy rồi nè!',
                        html: 'Có ' + '" + dt.Rows.Count + @"' + ' món ăn phù hợp với tìm kiếm của bạn!',
                        icon: 'success',
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            htmlContainer: 'custom-swal-text',
                            confirmButton: 'custom-swal-button'
                        },
                        backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/l0HlPuurz4wGw3SPS/giphy.gif') center center no-repeat`,
                        didOpen: () => {
                            document.getElementById('successSound').play();
                            confetti({
                                particleCount: 150,
                                spread: 70,
                                colors: ['#ff6f61', '#ff0000', '#ffca28'],
                                origin: { y: 0.6 }
                            });
                        }
                    });
                ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", script, true);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlSort.SelectedIndex = 0;
            getProducts();
            string script = @"
                Swal.fire({
                    title: 'Làm mới siêu đỉnh!',
                    html: 'Danh sách món ăn đã được cập nhật mới toanh cho bạn!',
                    icon: 'success',
                    customClass: {
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    },
                    backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/26ufnwz3wDUli7GU0/giphy.gif') center center no-repeat`,
                    timer: 3000,
                    timerProgressBar: true,
                    didOpen: () => {
                        document.getElementById('successSound').play();
                        confetti({
                            particleCount: 200,
                            spread: 120,
                            colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                            origin: { y: 0.6 }
                        });
                    }
                });
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "RefreshAlert", script, true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            string sortOrder = ddlSort.SelectedValue;
            getProducts("", sortOrder);
            string script = @"
                Swal.fire({
                    title: 'Xóa sạch sẽ!',
                    html: 'Tìm kiếm đã được xóa, giờ bạn thấy hết món ngon rồi nhé!',
                    icon: 'info',
                    customClass: {
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    },
                    backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/3o6Zt6KHxJTbXCnSso/giphy.gif') center center no-repeat`,
                    didOpen: () => {
                        document.getElementById('successSound').play();
                        confetti({
                            particleCount: 100,
                            spread: 70,
                            colors: ['#ff6f61', '#ff0000', '#ffca28'],
                            origin: { y: 0.6 }
                        });
                    }
                });
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearAlert", script, true);
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string sortOrder = ddlSort.SelectedValue;
            getProducts(searchText, sortOrder);
            string sortMessage = "";
            switch (sortOrder)
            {
                case "price_desc": sortMessage = "Giá từ cao đến thấp"; break;
                case "price_asc": sortMessage = "Giá từ thấp đến cao"; break;
                case "bestseller": sortMessage = "Bán chạy nhất"; break;
                case "newest": sortMessage = "Món mới nhất"; break;
                case "discount_desc": sortMessage = "Giảm sâu nhất"; break;
                default: sortMessage = "Mặc định"; break;
            }
            string script = @"
                Swal.fire({
                    title: 'Sắp xếp đỉnh cao!',
                    html: 'Danh sách đã được sắp xếp theo <b>' + '" + sortMessage + @"' + '</b>!',
                    icon: 'success',
                    customClass: {
                        popup: 'custom-swal-popup',
                        title: 'custom-swal-title',
                        htmlContainer: 'custom-swal-text',
                        confirmButton: 'custom-swal-button'
                    },
                    backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/l0HlPuurz4wGw3SPS/giphy.gif') center center no-repeat`,
                    didOpen: () => {
                        document.getElementById('successSound').play();
                        confetti({
                            particleCount: 150,
                            spread: 90,
                            colors: ['#ff6f61', '#ff0000', '#ffca28'],
                            origin: { y: 0.6 }
                        });
                    }
                });
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SortAlert", script, true);
        }

        protected void rProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Session["MaNguoiDung"] != null)
            {
                bool isCartItemUpdated = false;
                int i = isItemExistInCart(Convert.ToInt32(e.CommandArgument));

                if (i == 0)
                {
                    // Tính giá đã giảm
                    double price = 0;
                    double discount = 0;
                    con = new SqlConnection(ConnectionSQL.GetConnectionString());
                    cmd = new SqlCommand("SELECT GiaMonAn, KhuyenMai FROM MonAn WHERE MaMonAn = @ProductId", con);
                    cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            price = Convert.ToDouble(reader["GiaMonAn"]);
                            discount = reader["KhuyenMai"] != DBNull.Value ? Convert.ToDouble(reader["KhuyenMai"]) : 0;
                        }
                    }
                    con.Close();

                    double giaMonAnSauKhuyenMai = discount > 0 ? price * (1 - discount / 100) : price;

                    cmd = new SqlCommand("Cart_Crud", con);
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@ProductId", e.CommandArgument);
                    cmd.Parameters.AddWithValue("@Quantity", 1);
                    cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
                    cmd.Parameters.AddWithValue("@GiaMonAnSauKhuyenMai", giaMonAnSauKhuyenMai);
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string errorScript = @"
                    Swal.fire({
                        title: 'Lỗi không ngờ tới!',
                        html: 'Có lỗi xảy ra: <b>" + ex.Message + @"</b><br>Vui lòng thử lại sau!',
                        icon: 'error',
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            htmlContainer: 'custom-swal-text',
                            confirmButton: 'custom-swal-button'
                        },
                        backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/3o7TKTDn976rzVgig8/giphy.gif') center center no-repeat`,
                        didOpen: () => {
                            document.getElementById('errorSound').play();
                        }
                    });
                ";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
                        return;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                else
                {
                    Utils utils = new Utils();
                    isCartItemUpdated = utils.updateCartQuantity(i + 1, Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["MaNguoiDung"]));
                }

                string successScript = @"
            Swal.fire({
                title: 'Thêm vào giỏ hàng thần tốc!',
                html: 'Món ăn đã nằm gọn trong giỏ hàng của bạn!<br>Đi xem giỏ hàng ngay nào!',
                icon: 'success',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                },
                backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/26ufnwz3wDUli7GU0/giphy.gif') center center no-repeat`,
                didOpen: () => {
                    document.getElementById('successSound').play();
                    confetti({
                        particleCount: 200,
                        spread: 120,
                        colors: ['#ff6f61', '#ff0000', '#ffca28', '#ffffff'],
                        origin: { y: 0.6 }
                    });
                }
            }).then(() => { window.location.replace('GioHang.aspx'); }); // Thay href bằng replace
        ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAlert", successScript, true);
            }
            else
            {
                string errorScript = @"
            Swal.fire({
                title: 'Chưa đăng nhập nha!',
                html: 'Bạn cần đăng nhập để thêm món ngon này vào giỏ hàng!<br>Đăng nhập ngay nhé!',
                icon: 'error',
                customClass: {
                    popup: 'custom-swal-popup',
                    title: 'custom-swal-title',
                    htmlContainer: 'custom-swal-text',
                    confirmButton: 'custom-swal-button'
                },
                backdrop: `rgba(0,0,0,0.7) url('https://media.giphy.com/media/3o7TKTDn976rzVgig8/giphy.gif') center center no-repeat`,
                didOpen: () => {
                    document.getElementById('errorSound').play();
                }
            }).then(() => { window.location.replace('DangNhap.aspx'); }); // Thay href bằng replace
        ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", errorScript, true);
            }
        }

        int isItemExistInCart(int productId)
        {
            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Cart_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "GETBYID");
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@UserId", Session["MaNguoiDung"]);
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            int quantity = 0;
            if (dt.Rows.Count > 0)
            {
                quantity = Convert.ToInt32(dt.Rows[0]["SoLuong"]);
            }
            return quantity;
        }

        [WebMethod]
        public static string AddToCart(int productId, int quantity, decimal giaMonAnSauKhuyenMai)
        {
            try
            {
                DanhSachMonAn page = HttpContext.Current.Handler as DanhSachMonAn;
                if (page.Session["MaNguoiDung"] == null)
                {
                    return "NotLoggedIn";
                }

                int userId = Convert.ToInt32(page.Session["MaNguoiDung"]);
                // Kiểm tra xem món ăn đã có trong giỏ hàng chưa
                int existingQuantity = page.isItemExistInCart(productId);

                using (SqlConnection con = new SqlConnection(ConnectionSQL.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("Cart_Crud", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (existingQuantity > 0)
                        {
                            // Nếu món ăn đã có trong giỏ, cập nhật số lượng
                            cmd.Parameters.AddWithValue("@Action", "UPDATE");
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                            cmd.Parameters.AddWithValue("@Quantity", existingQuantity + quantity); // Cộng thêm số lượng mới
                            cmd.Parameters.AddWithValue("@UserId", userId);
                        }
                        else
                        {
                            // Nếu món ăn chưa có, thêm mới
                            cmd.Parameters.AddWithValue("@Action", "INSERT");
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@GiaMonAnSauKhuyenMai", giaMonAnSauKhuyenMai);
                        }

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}