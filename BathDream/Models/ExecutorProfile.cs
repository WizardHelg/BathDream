using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    [Table("ExecutorProfiles")]
    public class ExecutorProfile : UserProfile
    {
        public string About { get; set; }
        
        public double GetRating() => FeedBacks.Average(fb => fb.Rating);
    }
}
