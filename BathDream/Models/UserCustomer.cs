using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class UserCustomer : User
    {
        public List<FeedBack> FeedBacks { get; set; } = new List<FeedBack>();
    }
}
