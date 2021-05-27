using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Order
    {
        public enum Statuses
        {
            Temp = 0,
            New = 1,
            Brief = 2,
            ToExecute = 4,
            Executing = 8
        }

        public int Id { get; set; }
        public Statuses Status { get; set; }
        public string StatusName()
        {
            if ((Status & Statuses.ToExecute) > 0)
            {
                if ((Status & Statuses.New) > 0)
                    return "ToExecute";
                else
                    return "Executing";
            }
            else
                return "Brief";
            //if (Status == (Statuses.Brief | Statuses.New)) statusName = "Brief";
            //else if (Status == Statuses.ToExecute) statusName = "ToExecute";
            //else if (Status == Statuses.Executing) statusName = "Executing";
        }
        public DateTime Date { get; set; }
        public UserProfile Customer { get; set; }
        public ExecutorProfile Executor { get; set; }
        public Estimate Estimate { get; set; }
        public string Contract { get; set; }
        public string ObjectAdress { get; set; }
        public bool Signed { get; set; }
        public int SelectedItemId { get; set; }
    }
}
