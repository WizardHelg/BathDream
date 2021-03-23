using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class FeedBack
    {
        public int Id { get; set; }
        public UserProfile Customer { get; set; }
        public int ExecutorProfileId { get; set; }
        public ExecutorProfile Executor { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
