using Microsoft.AspNetCore.Mvc;
using IntegrationWithExternalParty.Models;

namespace IntegrationWithExternalParty.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }

        // Other actions for CRUD operations
    }
}
