using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad3.Models
{
    public class Promotion
    {
        [Required]
        public string Studies { get; set; }

        [Required]
        public int Semester { get; set; }

    }
}
