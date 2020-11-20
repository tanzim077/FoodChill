﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodChill.Models
{
    public class MenuItem
    {
        public int ID { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Spicyness { get; set; }
        
        public string Description { get; set; }
        
        public enum Espicy { NA = 0, Not_Spicy = 1, Spicy =2, Very_Spicy = 3 }
        
        public string Image { get; set; }
        
        [Display (Name ="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display (Name ="Sub-Category")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1,int.MaxValue, ErrorMessage ="Price should be greater than /$1")]
        public double Price { get; set; }
    }
}
