using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class AdViewModel
    {
        public string Id { get; set; }

        public int Approved { get; set; }

        [Required]
        [StringLength(50)]
        public string Заглавие { get; set; }

        [Required]
        public string Съдържание { get; set; }
        public string AuthorId { get; set; }
        public decimal Цена { get; set; }

        public int Категория { get; set; }
        public ICollection<Ad> Ads { get; set; }
        public List<Category> Categories { get; internal set; }

        
        public int Град { get; set; }        
        public List<Town> Towns { get; internal set; }

        




    }
}