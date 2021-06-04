using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Message
    {
        public int Id { get; set;  }
        public DateTime? DateTime { get; set; }
        public string Text { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public Order Order { get; set; }
        public bool IsReaded { get; set; } = false;
    }
}
