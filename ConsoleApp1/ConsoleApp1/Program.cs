using System;
using System.Collections.Generic;

namespace LibraryManagement
{
    // --- PHẦN TRỪU TƯỢNG (ABSTRACTION) ---
    // Định nghĩa lớp trừu tượng cho tất cả các tài liệu trong thư viện
    public abstract class LibraryItem
    {
        // Thuộc tính đóng gói (Encapsulation)
        public string Id { get; private set; }
        public string Title { get; set; }
        public bool IsAvailable { get; protected set; }

        protected LibraryItem(string id, string title)
        {
            Id = id;
            Title = title;
            IsAvailable = true; // Mặc định tài liệu mới nhập kho sẽ sẵn sàng cho mượn
        }

        // Phương thức trừu tượng yêu cầu các tài liệu con phải tự hiển thị chi tiết của mình
        public abstract void DisplayDetails();
    }

    // --- TÍNH KẾ THỪA (INHERITANCE) ---
    // Sách là một loại tài liệu thư viện
    public class Book : LibraryItem
    {
        public string Author { get; set; }
        public int PageCount { get; set; }

        public Book(string id, string title, string author, int pageCount)
            : base(id, title) // Gọi Constructor của lớp cha (LibraryItem)
        {
            Author = author;
            PageCount = pageCount;
        }

        // --- TÍNH ĐA HÌNH (POLYMORPHISM - OVERRIDING) ---
        public override void DisplayDetails()
        {
            string status = IsAvailable ? "Sẵn sàng" : "Đã được mượn";
            Console.WriteLine($"[SÁCH] Mã: {Id} | Tên: {Title} | Tác giả: {Author} | Số trang: {PageCount} trang | Trạng thái: {status}");
        }
    }

    // Tạp chí cũng là một loại tài liệu thư viện
    public class Magazine : LibraryItem
    {
        public int IssueNumber { get; set; }

        public Magazine(string id, string title, int issueNumber)
            : base(id, title)
        {
            IssueNumber = issueNumber;
        }

        public override void DisplayDetails()
        {
            string status = IsAvailable ? "Sẵn sàng" : "Đã được mượn";
            Console.WriteLine($"[TẠP CHÍ] Mã: {Id} | Tên: {Title} | Số phát hành: #{IssueNumber} | Trạng thái: {status}");
        }
    }

    // --- INTERFACE (KHẢ NĂNG MƯỢN TRẢ) ---
    public interface IBorrowable
    {
        void BorrowItem(string user);
        void ReturnItem();
    }

    // Lớp Sách có thể mượn được nên chúng ta kế thừa thêm interface IBorrowable
    public class BorrowableBook : Book, IBorrowable
    {
        public string CurrentBorrower { get; private set; }

        public BorrowableBook(string id, string title, string author, int pageCount)
            : base(id, title, author, pageCount)
        {
            CurrentBorrower = string.Empty;
        }

        public void BorrowItem(string user)
        {
            if (IsAvailable)
            {
                IsAvailable = false;
                CurrentBorrower = user;
                Console.WriteLine($"Thành công: Đã cho {user} mượn cuốn '{Title}'!");
            }
            else
            {
                Console.WriteLine($"Thất bại: Cuốn '{Title}' hiện tại không có sẵn.");
            }
        }

        public void ReturnItem()
        {
            if (!IsAvailable)
            {
                IsAvailable = true;
                Console.WriteLine($"Thành công: Đã trả lại cuốn '{Title}' vào kho sách.");
                CurrentBorrower = string.Empty;
            }
            else
            {
                Console.WriteLine($"Thông báo: Cuốn '{Title}' đã nằm trong kho từ trước.");
            }
        }
    }

    // --- LỚP QUẢN LÝ (CONTROLLER) ---
    public class Library
    {
        private List<LibraryItem> items = new List<LibraryItem>();

        public void AddItem(LibraryItem item)
        {
            items.Add(item);
            Console.WriteLine($"Đã thêm tài liệu mới: {item.Title}");
        }

        public void ShowAllItems()
        {
            Console.WriteLine("\n--- DANH SÁCH TÀI LIỆU TRONG THƯ VIỆN ---");
            if (items.Count == 0)
            {
                Console.WriteLine("Thư viện hiện tại đang trống.");
                return;
            }
            foreach (var item in items)
            {
                item.DisplayDetails(); // Đa hình quyết định hàm DisplayDetails nào được gọi
            }
            Console.WriteLine("-------------------------------------------\n");
        }

        // Tìm kiếm tài liệu theo Mã
        public LibraryItem FindItem(string id)
        {
            return items.Find(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }

    // --- CHƯƠNG TRÌNH CHÍNH ---
    class Program
    {
        static void Main(string[] args)
        {
            // Thiết lập mã hóa hiển thị tiếng Việt trên Console
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Library myLibrary = new Library();

            // 1. Thêm tài liệu mới vào thư viện
            Console.WriteLine(">>> BƯỚC 1: NHẬP SÁCH VÀO THƯ VIỆN");
            BorrowableBook book1 = new BorrowableBook("B01", "Lập trình C# nâng cao", "Nguyễn Văn Thư", 450);
            Magazine mag1 = new Magazine("M01", "Tạp chí Khoa học & Công nghệ", 112);

            myLibrary.AddItem(book1);
            myLibrary.AddItem(mag1);

            // 2. Hiển thị kho lưu trữ ban đầu
            myLibrary.ShowAllItems();

            // 3. Thực hiện nghiệp vụ mượn sách (Sử dụng Interface thông qua Ép kiểu an toàn)
            Console.WriteLine(">>> BƯỚC 2: THỰC HIỆN MƯỢN SÁCH");
            LibraryItem itemToBorrow = myLibrary.FindItem("B01");

            if (itemToBorrow is IBorrowable borrowable)
            {
                // Thử mượn lần 1
                borrowable.BorrowItem("Trần Anh Tuấn");

                // Thử mượn lần 2 (khi sách đã bị mượn)
                borrowable.BorrowItem("Lê Hoàng Nam");
            }

            // 4. Xem lại trạng thái sau khi mượn
            myLibrary.ShowAllItems();

            // 5. Trả sách
            Console.WriteLine(">>> BƯỚC 3: THỰC HIỆN TRẢ SÁCH");
            if (itemToBorrow is IBorrowable returnable)
            {
                returnable.ReturnItem();
            }

            // 6. Hiển thị kết quả cuối cùng
            myLibrary.ShowAllItems();

            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}