using Newtonsoft.Json;

namespace Library.Services.Isbndb.Models
{
    public class IsbndbBookModel
    { // описать все структуры из api
        public string Id { get; set; }
        public string Title { get; set; }
        [JsonProperty("title_long")]
        public string TitleLong { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        [JsonProperty("dewey_decimal")]
        public string DeweyDecimal { get; set; }
        public string Binding { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        [JsonProperty("date_published")]
        public string DataPublished { get; set; }
        public string Edition { get; set; }
        public string dimensions { get; set; }
        [JsonProperty("dimensions_structured")]
        public IsbndbBookDimentionsModel DimentionsStructure { get; set; }
        public string Overview { get; set;}
        //public Image
        public int msrp {  get; set; }
        public int excerpt { get; set; }
        public string synopsis {  get; set; }
        public List<string> authors { get; set; }
        

    } 
}
