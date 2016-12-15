using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class Town
    {
        
            private ICollection<Ad> ads;

            public Town()
            {
                this.ads = new HashSet<Ad>();
            }

            [Key]
            public int Id { get; set; }

            [Required]
            [Index(IsUnique = true)]
            [StringLength(20)]
            public string Name { get; set; }

            public virtual ICollection<Ad> Ads { get; set; }
    }
}
