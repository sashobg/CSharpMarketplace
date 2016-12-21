using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class Ad
    {

            public ICollection<Image> images;
              public ICollection<Comment> comments;


        public Ad()
            {
                 this.images = new HashSet<Image>();
                 this.comments = new HashSet<Comment>();
            }

            public Ad(string id, int approved, string authorId, string title, string content,decimal price, int categoryId, int townId, int viewCount, DateTime dateCreated, string fileName)
            {
                this.Id = id;
                this.Approved = approved;
                this.AuthorId = authorId;
                this.Title = title;
                this.Content = content;
                this.Price = price;                
                this.CategoryId = categoryId;
                this.TownId = townId;
                this.ViewCount = viewCount;
                this.DateCreated = dateCreated;
                this.primaryImageName = fileName;

             }

            [Key]

            public string Id { get; set; }

            public int Approved { get; set; }

            [Required]
            [StringLength(255)]
            public string Title { get; set; }

            public string Content { get; set; }

            public decimal Price { get; set; }

            [ForeignKey("Author")]
            public string AuthorId { get; set; }

            public virtual ApplicationUser Author { get; set; }

            public bool IsAuthor(string name)
            {
                return this.Author.UserName.Equals(name);
            }

            public string primaryImageName { get; set; }

            public virtual ICollection<Image> Images { get; set;} 

            
            [ForeignKey("Category")]
            public int CategoryId { get; set; }
            public virtual Category Category { get; set; }

             [ForeignKey("Town")]
             public int TownId { get; set; }
             public virtual Town Town { get; set; }

            public int ViewCount { get; set; }

            public DateTime DateCreated { get; set; }

             public virtual ICollection<Comment> Comments { get; set; }  

    }
}