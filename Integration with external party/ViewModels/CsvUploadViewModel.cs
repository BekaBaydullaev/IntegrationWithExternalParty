using System.ComponentModel.DataAnnotations;

namespace IntegrationWithExternalParty.Models
{

    public class CsvUploadViewModel
    {
        [Required(ErrorMessage = "Please choose a CSV file to upload.")]
        public IFormFile CsvFile { get; set; }

        public IEnumerable<Employee> CsvData { get; set; }
    }
}
