namespace CitationVK.Models
{
    public class AccountClassifier
    {
        public int AccountId { get; set; }

        public Account Account { get; set; }

        public int ClassifierId { get; set; }

        public Classifier Classifier { get; set; }
    }
}
