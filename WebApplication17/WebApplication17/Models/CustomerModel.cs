using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace WebApplication17.Models
{
    public class CustomerModel
    {


        public string CustomerId { get; set; }


        public string Name { get; set; }


        public string Country { get; set; }

    }
}