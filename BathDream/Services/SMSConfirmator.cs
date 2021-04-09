using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BathDream.Services
{
    public class SMSConfirmator
    {
        readonly Timer timer;
        readonly Random random = new();
        readonly Dictionary<Guid, (int code, DateTime time)> _confirmation_pairs = new();
        
        public SMSConfirmator()
        {
            timer = new()
            {
                AutoReset = true,
                Interval = 300_000,
            };

            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime time = DateTime.Now;
            List<Guid> toRemove = new();

            foreach (var item in _confirmation_pairs)
                if ((time - item.Value.time).TotalMinutes > 5)
                    toRemove.Add(item.Key);

            toRemove.ForEach(i => _confirmation_pairs.Remove(i));
        }

        public (string guid, string code) AddNewSMSConfirmation()
        {
            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            } while (_confirmation_pairs.ContainsKey(guid));

            int code = random.Next(1000, 10_000);
            _confirmation_pairs.Add(guid, (code, DateTime.Now));

            return (guid.ToString(), code.ToString());
        }

        public bool TryConfirm(string strGuid, string vCode) =>
                Guid.TryParse(strGuid, out Guid guid)
                && int.TryParse(vCode, out int i_code)
                && _confirmation_pairs.TryGetValue(guid, out (int code, DateTime time) item)
                && item.code == i_code;
    }
}
