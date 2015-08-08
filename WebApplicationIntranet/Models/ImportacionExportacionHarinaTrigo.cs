using Domain;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ImportacionExportacionHarinaTrigo
    {
        public Query<ImportacionHarinaTrigo> ImportacionHarinaTrigo { get; set; }
        public Query<ExportacionHarinaTrigo> ExportacionHarinaTrigo { get; set; }

    }
}