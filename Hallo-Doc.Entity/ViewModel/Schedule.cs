using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Schedule
    {
        public string? AdminName { get; set; }
        public int AdminId { get; set; }
        [Required(ErrorMessage = "Please select any region.")]
        public int? RegionId { get; set; }
        public string? RegionName { get; set;}
        [Required(ErrorMessage = "Please select one Physician.")]
        public int PhysicianId { get; set; }
        public string? PhysicianName { get;set; }
        [Required(ErrorMessage = "Please select appropriate date.")]
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        [GreaterThan("starttime", ErrorMessage = "End time must be later than start time.")]
        public TimeOnly EndTime { get; set; }
        public short Status { get; set; }
        public BitArray? IsRepeat { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public string? Color { get; set; }
        public string? Border { get; set; }    
        public int ShiftId { get; set; }
    }
    public class GreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public GreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (TimeOnly)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (TimeOnly)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
