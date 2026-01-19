using Fruit_PRJ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fruit_PRJ.Models;

namespace Fruit_Store_PRJ.Pages
{
    // Class Product có thể để dùng chung hoặc khai báo tại đây




    public class ProductDetailModel : PageModel
    {
        // Đối tượng chứa dữ liệu sản phẩm để hiển thị lên HTML

        public ProductServices _productServices;
        
        public ProductDetailModel(ProductServices productServices)
        {
            _productServices = productServices;
        }

        public Product? product { get; set; } = new Product();

        public void OnGet(int? id)
        {
            product = _productServices.GetProductById(id.Value);
           
        }

        public IActionResult OnPostAddToCart(int productId, int quantity)
        {
            // Logic xử lý khi nhấn nút "Thêm vào giỏ"
            Console.WriteLine($"Đã thêm {quantity} kg sản phẩm {productId} vào giỏ hàng.");
            return RedirectToPage("/Cart");
        }
    }
}