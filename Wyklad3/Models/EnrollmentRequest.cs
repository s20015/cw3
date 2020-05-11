using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Wyklad3.Models
{
    public class EnrollmentRequest
    {
        [RegularExpression("^s[0-9]{5}$")]
        public string IndexNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        
        [Required]
        public string Studies { get; set; }
    }
}
