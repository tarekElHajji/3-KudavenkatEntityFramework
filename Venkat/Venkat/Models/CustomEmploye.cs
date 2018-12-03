using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Venkat.Models
{
    [MetadataType(typeof(EmployeeMetaData))]
    public partial class Employe
    {
    }

    public class EmployeeMetaData
    {
        [Required]
        public int IdEmployee { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        [Required]
        public string UrlImage { get; set; }
        [Required]
        public string AltImage { get; set; }

        [Display(Name = "Department")]
        [Required]
        public int DepartmentId { get; set; }


    }
}