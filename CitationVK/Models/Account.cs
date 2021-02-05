using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CitationVK.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DisplayName("Security question")]
        public int Question { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Answer { get; set; }

        public string Salt { get; set; }

        [DisplayName("Admin")]
        public bool IsAdmin { get; set; }

        [DisplayName("Created")]
        public DateTime Date { get; set; }

        public ICollection<AccountClassifier> AccountClassifiers { get; set; }

        public ICollection<AccountDataset> AccountDatasets { get; set; }
    }
}
