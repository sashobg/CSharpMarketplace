using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class AdViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public ICollection<Ad> Ads { get; set; }
        public List<Category> Categories { get; internal set; }

        public int TownId { get; set; }        
        public List<Town> Towns { get; internal set; }


    }
}