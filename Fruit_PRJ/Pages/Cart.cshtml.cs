using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Fruit_Store_PRJ.Pages
{
    public class CartModel : PageModel
    {
        public class CartItem
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string ImageUrl { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Total => Price * Quantity;
        }

        // Dùng Session để lưu trữ nên không cần [BindProperty] trực tiếp cho toàn bộ List 
        // để tránh xung đột khi POST, chúng ta sẽ nạp từ Session trong mỗi Handler.
        public List<CartItem> Cart { get; set; } = new();

        [BindProperty]
        public string DeliveryDate { get; set; }

        [BindProperty]
        public string DeliveryTime { get; set; }

        public decimal SubTotal => Cart.Sum(x => x.Total);
        public decimal ShippingFee => 0; // Có thể thay đổi logic tính phí ship tại đây
        public decimal GrandTotal => SubTotal + ShippingFee;

        // Key dùng để lưu trong Session
        private const string SessionKey = "CartSession";

        public void OnGet()
        {
            LoadCart();
        }

        public IActionResult OnPostUpdateQuantity(int productId, int quantity)
        {
            LoadCart();
            var item = Cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCart();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveFromCart(int productId)
        {
            LoadCart();
            Cart.RemoveAll(x => x.ProductId == productId);
            SaveCart();
            return RedirectToPage();
        }

        public IActionResult OnPostCheckout()
        {
            LoadCart();

            if (!Cart.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToPage();
            }

            // Thực hiện Logic lưu đơn hàng vào Database tại đây...
            // Ví dụ: _orderService.CreateOrder(Cart, DeliveryDate, DeliveryTime);

            // Xóa giỏ hàng sau khi đặt thành công (tùy chọn)
            // HttpContext.Session.Remove(SessionKey);

            return RedirectToPage("CheckoutSuccess"); // Chuyển đến trang thông báo thành công
        }

        // --- Helper Methods để quản lý Session ---
        private void LoadCart()
        {
            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKey) ?? new List<CartItem>();
        }

        private void SaveCart()
        {
            HttpContext.Session.SetObjectAsJson(SessionKey, Cart);
        }
    }

    // Đảm bảo bạn có Extension này trong Project (có thể để cùng file hoặc file riêng)
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }
}