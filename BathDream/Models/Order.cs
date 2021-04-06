﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public UserProfile Customer { get; set; }
        public ExecutorProfile Executor { get; set; }
        public Estimate Estimate { get; set; }
        public string Contract { get; set; }
        public string ObjectAdress { get; set; }
        public bool Signed { get; set; }
    }
}
