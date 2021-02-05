using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CitationVK.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("PMID")]
        public string Pmid { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public bool Classification { get; set; }

        [DisplayName("Retrieved")]
        public DateTime Date { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }
    }
}
