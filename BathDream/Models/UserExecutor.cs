using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class UserExecutor : User
    {
        public string About { get; set; }
        public int Rating => FeedBacks.Sum(f => f.Rating);
        public List<FeedBack> FeedBacks { get; set; } = new List<FeedBack>();
    }
}
