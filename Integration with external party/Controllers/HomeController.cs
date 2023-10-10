using CsvHelper;
using CsvHelper.Configuration;
using Integration_with_external_party.Models;
using IntegrationWithExternalParty.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace Integration_with_external_party.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmployeeDbContext _context;
        public class FileProcessResult
        {
            public int RecordsProcessed { get; set; }
            public bool HasError { get; set; }
            public string ErrorMessage { get; set; }
        }
        public HomeController(ILogger<HomeController> logger, EmployeeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new CsvUploadViewModel();
            if (TempData["DataLoaded"] != null && (bool)TempData["DataLoaded"] == true)
            {
                model.CsvData = _context.Employees.ToList();
                _logger.LogInformation($"Loaded {model.CsvData.Count()} employees for the view.");
            }
            else
            {
                _logger.LogInformation("Data not loaded for the view.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImportData(CsvUploadViewModel model)
        {
            _logger.LogInformation("Import action method invoked");
            // Check for file's existence
            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                ViewBag.ErrorMessage = "Please upload a file.";
                return View("Index", model);
            }

            // Check for .csv extension
            var fileExtension = Path.GetExtension(model.CsvFile.FileName).ToLower();
            if (fileExtension != ".csv")
            {
                _logger.LogWarning($"Invalid file format: {fileExtension}");
                ViewBag.ErrorMessage = "Only CSV files are allowed.";
                return View("Index", model);
            }

            _logger.LogInformation("Starting CSV file import process.");

/*            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"{state.Key} - {error.ErrorMessage}");
                    }
                }
                return View("Index", model);
            }*/
            var result = await ProcessCsvFile(model.CsvFile);

            if (result.HasError)
            {
                _logger.LogError($"Import error: {result.ErrorMessage}");
                ViewBag.ErrorMessage = result.ErrorMessage;
                return View("Index", model);
            }

            _logger.LogInformation($"Successfully processed {result.RecordsProcessed} records from the CSV file.");
            TempData["SuccessMessage"] = $"Successfully processed {result.RecordsProcessed} records.";
            TempData["DataLoaded"] = true;
            return RedirectToAction("Index");


        }
        private async Task<FileProcessResult> ProcessCsvFile(IFormFile file)
        {
            var result = new FileProcessResult();
            int recordsProcessed = 0;

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                };

                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, config);

                // Register the class map.
                csv.Context.RegisterClassMap<EmployeeMap>();

                var records = csv.GetRecords<Employee>().ToList();

                foreach (var employee in records)
                {
                    _context.Employees.Add(employee);
                    recordsProcessed++;
                    _logger.LogDebug($"Processed record for employee {employee.Forenames} {employee.Surname}");
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Saved {recordsProcessed} employees to the database.");

                result.RecordsProcessed = recordsProcessed;
                result.HasError = false;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while processing the CSV file.");
                result.HasError = true;
                result.ErrorMessage = "An error occurred while processing the CSV file. Please check the file format and try again.";
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Saved employees to the database.");

            return result;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}