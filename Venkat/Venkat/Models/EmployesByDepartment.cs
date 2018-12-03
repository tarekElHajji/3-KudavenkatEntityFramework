using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Venkat.Models
{
    public class EmployeeByDepartment
    {
        [Key]
        public string Name { get; set; }
        public int Total { get; set; }
    }
}