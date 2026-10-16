using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Customer
    {
        private string _customerCode = string.Empty;
        private string _customerName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string? _companyName;
        private string _address = string.Empty;
        private string _city = string.Empty;
        private string _state = string.Empty;
        private string _status = "Active";

        [Key]
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer code is required.")]
        [StringLength(50, ErrorMessage = "Customer code cannot exceed 50 characters.")]
        [Display(Name = "Customer Code")]
        public string CustomerCode
        {
            get => _customerCode;
            set => _customerCode = value?.Trim().ToUpperInvariant() ?? string.Empty;
        }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        [Display(Name = "Customer Name")]
        public string CustomerName
        {
            get => _customerName;
            set => _customerName = value?.Trim() ?? string.Empty;
        }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [Display(Name = "Email Address")]
        public string Email
        {
            get => _email;
            set => _email = value?.Trim() ?? string.Empty;
        }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be a valid 10-digit Indian mobile number (starting with 6-9).")]
        [Display(Name = "Phone Number")]
        public string Phone
        {
            get => _phone;
            set => _phone = value?.Trim() ?? string.Empty;
        }

        [StringLength(150, ErrorMessage = "Company name cannot exceed 150 characters.")]
        [Display(Name = "Company Name")]
        public string? CompanyName
        {
            get => _companyName;
            set => _companyName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        [Display(Name = "Street Address")]
        public string Address
        {
            get => _address;
            set => _address = value?.Trim() ?? string.Empty;
        }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        [Display(Name = "City")]
        public string City
        {
            get => _city;
            set => _city = value?.Trim() ?? string.Empty;
        }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        [Display(Name = "State")]
        public string State
        {
            get => _state;
            set => _state = value?.Trim() ?? string.Empty;
        }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        [Display(Name = "Status")]
        public string Status
        {
            get => _status;
            set => _status = string.IsNullOrWhiteSpace(value) ? "Active" : value.Trim();
        }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100, ErrorMessage = "Created by cannot exceed 100 characters.")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; } = "System";
    }
}
