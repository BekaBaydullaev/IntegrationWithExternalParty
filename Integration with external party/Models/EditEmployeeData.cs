using System.ComponentModel.DataAnnotations;

namespace IntegrationWithExternalParty.Models
{
    public class EditEmployeeData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Payroll Number is required.")]
        [StringLength(50, ErrorMessage = "Payroll Number cannot exceed 50 characters.")]
        public string PayrollNumber { get; set; }

        [Required(ErrorMessage = "Forenames are required.")]
        [StringLength(100, ErrorMessage = "Forenames cannot exceed 100 characters.")]
        public string Forenames { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot exceed 100 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20, ErrorMessage = "Telephone number cannot exceed 20 digits.")]
        public string Telephone { get; set; }

        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 digits.")]
        public string Mobile { get; set; }

        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string Address { get; set; }

        [StringLength(255, ErrorMessage = "Address2 cannot exceed 255 characters.")]
        public string Address2 { get; set; }

        [StringLength(20, ErrorMessage = "Postcode cannot exceed 20 characters.")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string EmailHome { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }
    }
}
