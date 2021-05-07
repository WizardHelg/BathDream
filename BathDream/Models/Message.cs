using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Message
    {
        public int Id { get; set;  }

        [Required]
        public string UserName { get; set; }

        public DateTime DateTime { get; set; }
        [Required]
        public string Text { get; set; }
        public string File { get; set; }

        public string UserID { get; set; }
        public virtual User Sender { get; set; }

    }
}
