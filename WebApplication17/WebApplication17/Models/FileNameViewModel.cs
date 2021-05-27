using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication17.Models
{
    public class FileNameViewModel
    {
        public string chosenFile { get; set; }
        public IEnumerable<SelectListItem> Files { get; set; }
    }
}