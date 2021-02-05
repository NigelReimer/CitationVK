using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CitationVK.Data
{
    public static class Utilities
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static readonly Dictionary<int, string> Questions = new Dictionary<int, string>()
        {
            { 1, "What was your childhood nickname?" },
            { 2, "In what city did you meet your significant other?" },
            { 3, "What is the name of your favorite childhood friend?" },
            { 4, "What was the name of your first stuffed animal?" },
            { 5, "In what city or town was your first job?" }
        };

        public static async Task<List<Models.Article>> GetArticles(List<Models.Article> articles)
        {
            string pmids = string.Join(",", articles.Select(x => x.Pmid).ToList());
            string requestUri = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id={pmids}&rettype=null&retmode=xml";
            string response = await httpClient.GetStringAsync(requestUri);

            foreach (XElement element in XElement.Parse(response).Elements())
            {
                try
                {
                    Models.Article article = articles.FirstOrDefault(x => x.Pmid == element.Element("MedlineCitation").Element("PMID").Value);

                    article.Title = element.Element("MedlineCitation").Element("Article").Element("ArticleTitle").Value;
                    article.Date = DateTime.Now;

                    try
                    {
                        article.Abstract = element.Element("MedlineCitation").Element("Article").Element("Abstract").Element("AbstractText").Value;
                    }
                    catch
                    {
                        article.Abstract = null;
                    }
                }
                catch
                {
                    continue;
                }
            }

            articles = articles.Where(x => x.Abstract != null).ToList();
            return articles;
        }
    }
}
