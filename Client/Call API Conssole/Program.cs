using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Call_API_Conssole;
using Newtonsoft.Json;
using System.Net.Http.Json; 
class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        bool running = true;
        while (running)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Chọn thao tác CRUD:");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read");
            Console.WriteLine("3. Update");
            Console.WriteLine("4. Delete");
            Console.WriteLine("5. ReadById");
            Console.WriteLine("clear. Xóa màn hình");
            Console.WriteLine("exit. Thoát chương trình");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await Create();
                    break;
                case "2":
                    await Read();
                    break;
                case "3":
                    await UpdateProduct();
                    break;
                case "4":
                    await Delete();
                    break;
                case "5":
                    await ReadID();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "exit":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    break;
            }

        }
        Console.WriteLine("Chương trình kết thúc.");
    }
        

    // Create
    private static async Task Create()
    {
        try
        {
            Console.WriteLine("Nhập tên sản phẩm:");
            string name = Console.ReadLine();

            Console.WriteLine("Nhập mô tả sản phẩm:");
            string description = Console.ReadLine();

            Console.WriteLine("Nhập số lượng:");
            int quantity = int.Parse(Console.ReadLine());

            Console.WriteLine("Nhập giá:");
            int price = int.Parse(Console.ReadLine());

            Console.WriteLine("Chọn hình ảnh mô tả sản phẩm (nhập đường dẫn đầy đủ):");
            string imagePath = Console.ReadLine()?.Trim('"'); // Loại bỏ dấu ngoặc kép nếu có

            if (!System.IO.File.Exists(imagePath))
            {
                Console.WriteLine("Đường dẫn hình ảnh không hợp lệ. Vui lòng kiểm tra lại.");
                return;
            }

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(name), "Name");
            formData.Add(new StringContent(description), "Description");
            formData.Add(new StringContent(quantity.ToString()), "Quantity");
            formData.Add(new StringContent(price.ToString()), "Price");

            byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", System.IO.Path.GetFileName(imagePath));

            HttpResponseMessage response = await client.PostAsync("http://localhost:5083/api/products/create", formData);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Tạo mới thành công: {result}");
            }
            else
            {
                Console.WriteLine($"Lỗi: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
        }
    }

    // Read by ID --- pass
    private static async Task ReadID()
    {
        try
        {
            Console.WriteLine("Nhập ID của sản phẩm cần tìm:");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID không hợp lệ. Vui lòng nhập một số.");
                return;
            }

            HttpResponseMessage response = await client.GetAsync($"http://localhost:5083/api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung phản hồi và chuyển đổi thành đối tượng Product
                var product = await response.Content.ReadFromJsonAsync<Product>();

                if (product != null)
                {
                    // Hiển thị thông tin sản phẩm
                    Console.WriteLine("-----------------------------");
                    Console.WriteLine($"ID: {product.ID}");
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"Description: {product.Description}");
                    Console.WriteLine($"Quantity: {product.Quantity}");
                    Console.WriteLine($"Price: {product.Price}");
                    Console.WriteLine($"Banner: {product.Banner}");
                    Console.WriteLine("-----------------------------");

                }
                else
                {
                    Console.WriteLine("Sản phẩm không tồn tại.");
                }
            }
            else
            {
                Console.WriteLine($"Lỗi: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
        }
    }

    // Read
    private static async Task Read()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("http://hlocalhost:5083/api/products");
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.ID}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"Banner: {product.Banner}");
                Console.WriteLine($"Description: {product.Description}");
                Console.WriteLine($"Quantity: {product.Quantity}");
                Console.WriteLine($"Price: {product.Price}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
        }
    }

    // Update
    private static async Task UpdateProduct()
    {
        try
        {
            // Nhập ID sản phẩm
            Console.WriteLine("Nhập ID của sản phẩm cần cập nhật:");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID không hợp lệ. Vui lòng nhập một số.");
                return;
            }

            // Lấy thông tin sản phẩm hiện tại
            var currentProduct = await client.GetAsync($"http://localhost:5083/api/products/{id}");
            if (!currentProduct.IsSuccessStatusCode)
            {
                Console.WriteLine("Không thể tìm thấy sản phẩm.");
                return;
            }

            var productData = await currentProduct.Content.ReadFromJsonAsync<Product>();

            var form = new MultipartFormDataContent();

            // Nhập tên sản phẩm mới
            Console.WriteLine("Nhập tên sản phẩm mới (hoặc nhấn C để giữ nguyên):");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name) && name.ToUpper() != "C")
            {
                form.Add(new StringContent(name), "Name");
            }
            else
            {
                form.Add(new StringContent(productData.Name), "Name"); // Giữ giá trị hiện tại
            }

            // Nhập mô tả sản phẩm mới
            Console.WriteLine("Nhập mô tả sản phẩm mới (hoặc nhấn C để giữ nguyên):");
            string description = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description) && description.ToUpper() != "C")
            {
                form.Add(new StringContent(description), "Description");
            }
            else
            {
                form.Add(new StringContent(productData.Description), "Description"); // Giữ giá trị hiện tại
            }

            // Nhập số lượng mới
            Console.WriteLine("Nhập số lượng mới (hoặc nhấn C để giữ nguyên):");
            string quantityInput = Console.ReadLine();
            if (int.TryParse(quantityInput, out int quantity) && quantityInput.ToUpper() != "C")
            {
                form.Add(new StringContent(quantity.ToString()), "Quantity");
            }
            else
            {
                form.Add(new StringContent(productData.Quantity.ToString()), "Quantity"); // Giữ giá trị hiện tại
            }

            // Nhập giá mới
            Console.WriteLine("Nhập giá mới (hoặc nhấn C để giữ nguyên):");
            string priceInput = Console.ReadLine();
            if (int.TryParse(priceInput, out int price) && priceInput.ToUpper() != "C")
            {
                form.Add(new StringContent(price.ToString()), "Price");
            }
            else
            {
                form.Add(new StringContent(productData.Price.ToString()), "Price"); // Giữ giá trị hiện tại
            }

            // Nhập hình ảnh mới
            Console.WriteLine("Chọn hình ảnh mô tả sản phẩm (nhập đường dẫn đầy đủ, hoặc nhấn C để không cập nhật hình ảnh):");
            string imagePath = Console.ReadLine();
            if (imagePath.ToUpper() != "C")
            {
                if (File.Exists(imagePath))
                {
                    var extension = Path.GetExtension(imagePath).ToLower();
                    string mimeType = extension switch
                    {
                        ".jpg" => "image/jpeg",
                        ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        _ => null
                    };

                    if (mimeType != null)
                    {
                        var fileStreamContent = new StreamContent(File.OpenRead(imagePath));
                        fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                        form.Add(fileStreamContent, "Image", Path.GetFileName(imagePath));
                    }
                    else
                    {
                        Console.WriteLine("Loại tệp không hợp lệ.");
                    }
                }
                else
                {
                    Console.WriteLine("Tệp hình ảnh không tồn tại.");
                }
            }

            // Gửi yêu cầu PUT đến API
            HttpResponseMessage response = await client.PutAsync($"http://localhost:5083/api/products/update/{id}", form);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Cập nhật thành công: {result}");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Lỗi: {response.StatusCode}, {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
        }
    }

    // Delete
    public static async Task Delete()
    {
        // Nhập ID sản phẩm cần xóa từ người dùng
        Console.WriteLine("Nhập ID của sản phẩm cần xóa:");
        string productId = Console.ReadLine();

        // Kiểm tra xem ID có hợp lệ không
        if (!string.IsNullOrEmpty(productId))
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Xác định URL API với ID động
                string apiUrl = $"http://localhost:5083/api/products/delete/{productId}";

                try
                {
                    // Gửi yêu cầu xóa tới API
                    HttpResponseMessage response = await httpClient.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Xóa sản phẩm thành công.");
                    }
                    else
                    {
                        Console.WriteLine($"Lỗi khi xóa sản phẩm: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("ID không hợp lệ.");
        }
    }


}
