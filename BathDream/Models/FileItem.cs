using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class FileItem
    {
        public int Id { get; set; }
        public string FrendlyName { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public Order Order { get; set; }
    }
}
