using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marketplace.Models
{
    public class AdCategoryModel
    {
        public List<Ad> ads { get; set; }   
        public List<Category> categories { get; set; }        
    }
}