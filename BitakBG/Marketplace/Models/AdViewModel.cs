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
        [Display(Name = "Заглавие")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Текст")]
        public string Content { get; set; }
        public string AuthorId { get; set; }
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        public ICollection<Ad> Ads { get; set; }
        public List<Category> Categories { get; internal set; }

        [Display(Name = "Град")]
        public int TownId { get; set; }        
        public List<Town> Towns { get; internal set; }

        




    }
}