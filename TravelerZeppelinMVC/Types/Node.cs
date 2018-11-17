using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelerZeppelinMVC.Types
{
    public class Node
    {
        public bool? IsCompleted { get; set; }
        public string Plate { get; set; }
        public List<string> SubPlates { get; set; }
        public double Value { get; set; }
    }
}