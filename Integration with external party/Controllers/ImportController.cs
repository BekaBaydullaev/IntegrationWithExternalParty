using IntegrationWithExternalParty.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class ImportController : Controller
{
    private readonly EmployeeDbContext _context;

    public ImportController(EmployeeDbContext context)
    {
        _context = context;
     
    }

    public IActionResult Index()
    {
        var viewModel = new CsvUploadViewModel
        {
            CsvData = _context.Employees.ToList()
        };
        return View(viewModel);
    }

    


}
