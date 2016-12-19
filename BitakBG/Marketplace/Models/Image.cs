using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class Image
    {
        public Image()
        {
            
        }

        public Image(string id, string fileName, bool isItPrimary, string adId)
        {
            this.Id = id;
            this.FileName = fileName;
            this.IsItPrimary = isItPrimary;
            this.AdId = adId;            
        }

        [Key]

        public string Id { get; set; }

        public string FileName { get; set; }        

        public bool IsItPrimary { get; set; }

        [ForeignKey("Ad")]
        public string AdId { get; set; }
        public virtual Ad Ad { get; set; }

        
    }
}