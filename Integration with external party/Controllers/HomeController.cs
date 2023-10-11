using CsvHelper;
using CsvHelper.Configuration;
using Integration_with_external_party.Models;
using IntegrationWithExternalParty.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // Constructor to initialize logger and database context.
        public HomeController(ILogger<HomeController> logger, EmployeeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // The main action that displays the home page.
        // If data was recently loaded from a CSV, it will fetch and show that data.
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

        // Action to handle the upload and processing of a CSV file.
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
        // A helper method to process the uploaded CSV file.
        // Reads the file, parses its content, and saves the data to the database.
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
        [HttpPost]
        public IActionResult SaveEditedData(EditEmployeeData editedData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing record from the database using the Id.
                    var existingEmployee = _context.Employees.FirstOrDefault(e => e.Id == editedData.Id);

                    if (existingEmployee != null)
                    {
                        // Update the properties of the existing record with the edited data.
                        existingEmployee.PayrollNumber = editedData.PayrollNumber;
                        existingEmployee.Forenames = editedData.Forenames;
                        existingEmployee.Surname = editedData.Surname;
                        existingEmployee.DateOfBirth = editedData.DateOfBirth;
                        existingEmployee.Telephone = editedData.Telephone;
                        existingEmployee.Mobile = editedData.Mobile;
                        existingEmployee.Address = editedData.Address;
                        existingEmployee.Address2 = editedData.Address2;
                        existingEmployee.Postcode = editedData.Postcode;
                        existingEmployee.EmailHome = editedData.EmailHome;
                        existingEmployee.StartDate = editedData.StartDate;
                        

                        // Save changes to the database.
                        _context.SaveChanges();

                        return Json(new { success = true, message = "Data updated successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Employee not found." });
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    // Handle database update exceptions
                    _logger.LogError(dbEx, "Failed to update employee data in the database.");
                    return Json(new { success = false, message = "Database error occurred. Please try again later." });
                }
                catch (ArgumentNullException argNullEx)
                {
                    // Handle specific exceptions if you expect them
                    _logger.LogError(argNullEx, "A null argument was passed where it wasn't expected.");
                    return Json(new { success = false, message = "Invalid data provided. Please check and try again." });
                }
                catch (Exception ex)
                {
                    // Handle other general exceptions
                    _logger.LogError(ex, "An unexpected error occurred while saving edited data.");
                    return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
                }
            }
            else
            {
                // The received data did not pass model validation.
                return Json(new { success = false, message = "Invalid data provided." });
            }
        }
    
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}