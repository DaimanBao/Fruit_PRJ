using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fruit_Store_PRJ.Pages
{
    public class CheckoutItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantity;
    }

    public class CheckoutModel : PageModel
    {
        private const string SessionKey = "CartSession";
        public List<CheckoutItem> OrderSummary { get; set; } = new List<CheckoutItem>();
        public decimal GrandTotal { get; set; }

        public IActionResult OnGet()
        {
            // 1. Lấy dữ liệu từ Session để hiển thị tóm tắt đơn hàng
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel.CartItem>>(SessionKey);

            if (cart == null || !cart.Any())
            {
                // Nếu giỏ hàng trống mà truy cập checkout thì quay lại trang giỏ hàng
                return RedirectToPage("/Cart");
            }

            // 2. Chuyển đổi dữ liệu từ CartItem sang CheckoutItem
            OrderSummary = cart.Select(c => new CheckoutItem
            {
                Name = c.Name,
                Quantity = c.Quantity,
                Price = c.Price
            }).ToList();

            GrandTotal = OrderSummary.Sum(x => x.Total);

            return Page();
        }

        public IActionResult OnPostPlaceOrder(string FullName, string Phone, string Address, string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Address))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin giao hàng.");
                return Page();
            }

            // Process order placement logic
            Console.WriteLine($"Order placed: {FullName}, {Phone}, {Address}, Payment: {paymentMethod}, Total: {GrandTotal:N0}đ");

            return RedirectToPage("/OrderSuccess");
        }
    }
}