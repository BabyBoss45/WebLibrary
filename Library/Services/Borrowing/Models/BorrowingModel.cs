namespace Library.Services.Borrowing.Models
{
    public class BorrowingModel
    {
        public int Id { get; set; }
        
        public string IdUser { get; set; }
        
        public string StarDate { get; set; }
        
        public string EndDate { get; set; }

        public string IdStatus { get; set; }

    }
}
