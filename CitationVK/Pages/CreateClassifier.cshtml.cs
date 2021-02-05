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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.ML.DataOperationsCatalog;

namespace CitationVK.Pages
{
    public class CreateClassifierModel : PageModel
    {
        private readonly Models.Context _context;
        private readonly IHostingEnvironment _environment;

        public CreateClassifierModel(Models.Context context, IHostingEnvironment environment)
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

        [BindProperty]
        [Required]
        public string Name { get; set; }

        [BindProperty]
        [DisplayName("Dataset")]
        public int DatasetId { get; set; }

        public List<Models.Dataset> Datasets { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == DatasetId && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            Models.Dataset dataset = await _context.Datasets.FirstOrDefaultAsync(x => x.Id == DatasetId);
            List<Models.Article> articles = await _context.Articles.Where(x => x.DatasetId == dataset.Id).ToListAsync();

            if (articles.Count(x => x.Classification) < 5 || articles.Count(x => !x.Classification) < 5)
            {
                ModelState.AddModelError("DatasetId", "Dataset must have at least 5 articles of each classification.");
            }

            if (!ModelState.IsValid)
            {
                Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
                return Page();
            }

            List<ArticleData> data = articles.Select(x => new ArticleData() { Text = x.Title + " " + x.Abstract, Classification = x.Classification }).ToList();
            MLContext mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, 0.2);
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(ArticleData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            ITransformer model = estimator.Fit(splitDataView.TrainSet);
            IDataView predictions = model.Transform(splitDataView.TestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            var guid = Guid.NewGuid().ToString();
            var path = Path.Combine(_environment.WebRootPath, "classifiers", $"{guid}.zip");
            mlContext.Model.Save(model, dataView.Schema, path);

            Models.Classifier classifier = new Models.Classifier()
            {
                Name = Name,
                Accuracy = metrics.Accuracy,
                Precision = metrics.PositivePrecision,
                Recall = metrics.PositiveRecall,
                Date = DateTime.Now,
                Model = guid
            };

            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == HttpContext.Session.GetInt32("id"));
            _context.Classifiers.Add(classifier);
            _context.AccountClassifiers.Add(new Models.AccountClassifier() { Account = account, Classifier = classifier });
            await _context.SaveChangesAsync();
            return RedirectToPage("Classifiers");
        }
    }
}
