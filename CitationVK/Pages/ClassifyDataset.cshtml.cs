using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class ClassifyDatasetModel : PageModel
    {
        private readonly Models.Context _context;
        private readonly IHostingEnvironment _environment;

        public ClassifyDatasetModel(Models.Context context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public class ArticleData
        {
            [LoadColumn(0)]
            public string Text { get; set; }

            [LoadColumn(1), ColumnName("Label")]
            public bool Classification { get; set; }
        }

        public class ArticlePrediction : ArticleData
        {
            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }

            public float Probability { get; set; }

            public float Score { get; set; }
        }

        [BindProperty]
        public Models.Dataset Dataset { get; set; }

        [BindProperty]
        public string InputMethod { get; set; }

        [BindProperty]
        public string InputString { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        [DisplayName("Classifier")]
        public int ClassifierId { get; set; }

        public List<Models.Classifier> Classifiers { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            Classifiers = await _context.Classifiers.Where(x => x.AccountClassifiers.Any(y => y.ClassifierId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.Datasets.FirstOrDefaultAsync(x => x.Name == Dataset.Name && (x.AccountDatasets.FirstOrDefault(y => y.AccountId == HttpContext.Session.GetInt32("id")) != null)) != null)
            {
                ModelState.AddModelError("Dataset.Name", "You already have a dataset with this name.");
                Classifiers = await _context.Classifiers.Where(x => x.AccountClassifiers.Any(y => y.ClassifierId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
                return Page();
            }

            List<Models.Article> articles = new List<Models.Article>();

            if (InputMethod == "string")
            {
                if (string.IsNullOrEmpty(InputString))
                {
                    ModelState.AddModelError("InputString", "The articles from text field is required.");
                }
                else if (!Regex.IsMatch(InputString, @"(\d+?)(?:,|$)"))
                {
                    ModelState.AddModelError("InputString", "Article list is not in the correct format.");
                }
                else
                {
                    List<string> inputList = InputString.Split(',').ToList();

                    foreach (string article in inputList)
                    {
                        articles.Add(new Models.Article() { Pmid = article });
                    }

                    articles = await Data.Utilities.GetArticles(articles);

                    if (articles.Count < 1)
                    {
                        ModelState.AddModelError("InputString", "No valid articles were contained within your input.");
                    }
                }
            }
            else if (InputMethod == "file")
            {
                if (InputFile == null)
                {
                    ModelState.AddModelError("InputFile", "The articles from file field is required.");
                }
                else if (InputFile.FileName.Substring(InputFile.FileName.Length - 4) != ".csv")
                {
                    ModelState.AddModelError("InputFile", "The chosen articles file is of the wrong type.");
                }
                else
                {
                    try
                    {
                        using (StreamReader streamReader = new StreamReader(InputFile.OpenReadStream()))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                string article = streamReader.ReadLine();

                                if (!Regex.IsMatch(article, @"(\d+)"))
                                {
                                    ModelState.AddModelError("InputFile", "The chosen articles file has an invalid structure.");
                                }

                                articles.Add(new Models.Article() { Pmid = article });
                            }
                        }

                        articles = await Data.Utilities.GetArticles(articles);

                        if (articles.Count < 1)
                        {
                            ModelState.AddModelError("InputFile", "No valid articles were contained within your input.");
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("InputFile", "The chosen articles file has an invalid structure.");
                    }
                }
            }
            else
            {
                return RedirectToPage("Error");
            }

            var classifier = await _context.Classifiers.FirstOrDefaultAsync(x => x.Id == ClassifierId);

            if (classifier == null || await _context.AccountClassifiers.FirstOrDefaultAsync(x => x.ClassifierId == classifier.Id && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            if (!ModelState.IsValid)
            {
                Classifiers = await _context.Classifiers.Where(x => x.AccountClassifiers.Any(y => y.ClassifierId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
                return Page();
            }

            var path = Path.Combine(_environment.WebRootPath, "classifiers", $"{classifier.Model}.zip");
            MLContext mlContext = new MLContext();
            DataViewSchema modelSchema;
            ITransformer trainedModel = mlContext.Model.Load(path, out modelSchema);
            List<ArticleData> data = articles.Select(x => new ArticleData() { Text = x.Title + " " + x.Abstract }).ToList();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);
            IDataView predictions = trainedModel.Transform(dataView);
            IEnumerable<ArticlePrediction> predictedResults = mlContext.Data.CreateEnumerable<ArticlePrediction>(predictions, reuseRowObject: false);

            for (var i = 0; i < articles.Count(); i++)
            {
                articles[i].Classification = predictedResults.ElementAt(i).Prediction;
            }

            Dataset.Articles = articles;
            Dataset.Date = DateTime.Now;
            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == HttpContext.Session.GetInt32("id"));
            _context.Datasets.Add(Dataset);
            _context.AccountDatasets.Add(new Models.AccountDataset() { Account = account, Dataset = Dataset });
            await _context.SaveChangesAsync();
            return RedirectToPage("Datasets");
        }
    }
}
