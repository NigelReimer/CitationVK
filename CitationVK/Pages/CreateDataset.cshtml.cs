using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class CreateDatasetModel : PageModel
    {
        private readonly Models.Context _context;

        public CreateDatasetModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Dataset Dataset { get; set; }

        [BindProperty]
        public string InputMethod { get; set; }

        [BindProperty]
        public string InputString { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.Datasets.FirstOrDefaultAsync(x => x.Name == Dataset.Name && (x.AccountDatasets.FirstOrDefault(y => y.AccountId == HttpContext.Session.GetInt32("id")) != null)) != null)
            {
                ModelState.AddModelError("Dataset.Name", "You already have a dataset with this name.");
                return Page();
            }

            List<Models.Article> articles = new List<Models.Article>();

            if (InputMethod == "string")
            {
                if (string.IsNullOrEmpty(InputString))
                {
                    ModelState.AddModelError("InputString", "The articles from text field is required.");
                }
                else if (!Regex.IsMatch(InputString, @"(\d+,[01])(;\d+,[01])*"))
                {
                    ModelState.AddModelError("InputString", "Article list is not in the correct format.");
                }
                else
                {
                    List<string> inputList = InputString.Split(';').ToList();

                    foreach (string article in inputList)
                    {
                        articles.Add(new Models.Article() { Pmid = article.Split(',')[0], Classification = article.Split(',')[1] == "1" });
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

                                if (!Regex.IsMatch(article, @"(\d+,[01])"))
                                {
                                    ModelState.AddModelError("InputFile", "The chosen articles file has an invalid structure.");
                                    return Page();
                                }

                                articles.Add(new Models.Article() { Pmid = article.Split(',')[0], Classification = article.Split(',')[1] == "1" });
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

            if (!ModelState.IsValid)
            {
                return Page();
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
