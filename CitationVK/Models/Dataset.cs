﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CitationVK.Models
{
    public class Dataset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Created")]
        public DateTime Date { get; set; }

        public ICollection<Article> Articles { get; set; }

        public ICollection<AccountDataset> AccountDatasets { get; set; }
    }
}
