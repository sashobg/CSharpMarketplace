using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class Comment
    {

        public Comment()
        {

        }

        public Comment(string name, string content, int stars, string adId, DateTime dateCreated)
        {
           
            this.Name = name;
            this.Content = content;
            this.Stars = stars;                   
            this.AdId = adId;
            this.DateCreated = dateCreated;
        }

        [Key]

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Content { get; set; }

        [Required]
        public int Stars { get; set; }

        public DateTime DateCreated { get; set; }

        [ForeignKey("Ad")]
        public string AdId { get; set; }
        public virtual Ad Ad { get; set; }
    }
}