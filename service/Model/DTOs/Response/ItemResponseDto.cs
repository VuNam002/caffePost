using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model.DTOs.Response
{
    public class ItemResponseDto
    {
        public int item_id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }           
        public int category_id { get; set; }
        public string? category_name { get; set; }
        public string? image_url { get; set; }
        public bool is_active { get; set; }          
        public DateTime? created_at { get; set; }    
        public DateTime? updated_at { get; set; }    
    }
}
