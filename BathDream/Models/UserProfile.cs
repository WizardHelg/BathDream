using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public List<FeedBack> FeedBacks { get; set; }
        public List<Order> Orders { get; set; }
    }
}
