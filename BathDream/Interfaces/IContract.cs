using BathDream.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Interfaces
{
    public interface IContract
    {
        public User User { get; set; }
        public Order Order { get; set; }
        public List<Work> UniqueWorks { get; set; }
    }
}
