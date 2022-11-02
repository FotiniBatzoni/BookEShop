using System.ComponentModel.DataAnnotations;

namespace BookEShopWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name ="Display Order")]
        [Range(1,100,ErrorMessage ="Display Order must be betwwen 1 and 100")]
        public int DisplayOrder { get; set; }

        public DateTime CreateDateTime { get; set; } = DateTime.Now;
    }
}
