using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CitationVK.Models
{
    public class Configuration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [DisplayName("Public account creation")]
        public bool IsPublic { get; set; }
    }
}
