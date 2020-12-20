using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vcfqrtestNetFramework.Models
{
    public class VCardPageModelResponse
    {
        public VCard VCard { get; set; }
        public Setting Setting { get; set; }
        public FileStreamResult QrCode { get; set; }
    }
}