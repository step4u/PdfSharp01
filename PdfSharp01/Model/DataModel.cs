using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfSharp01.Model
{
    public class DataModel
    {
        public int seq { get; set; }
        public string subject  { get; set; }
        public JObject[] content { get; set; }
    }
}
