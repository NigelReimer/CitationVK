using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CitationVK.Models
{
    public class Classifier
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public double Accuracy { get; set; }

        public double Precision { get; set; }

        public double Recall { get; set; }

        [DisplayName("Trained")]
        public DateTime Date { get; set; }

        public ICollection<AccountClassifier> AccountClassifiers { get; set; }
    }
}
