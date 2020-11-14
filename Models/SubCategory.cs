using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodChill.Models
{
    public class SubCategory
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Sub-Category Name")]
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
