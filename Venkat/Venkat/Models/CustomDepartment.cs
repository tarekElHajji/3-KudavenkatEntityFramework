using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Venkat.Models
{
    [MetadataType(typeof(DepartmentMetaData))]
    public partial class Department
    {
    }

    public class DepartmentMetaData
    {
        [Display(Name = "Department Name")]
        public string Name { get; set; }
    }
}