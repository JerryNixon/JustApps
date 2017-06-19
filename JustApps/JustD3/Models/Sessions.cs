using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustD3.Models
{
    public class Sessions : List<Session>
    {
        public Sessions()
        {

        }

        public Sessions(IEnumerable<Session> collection) : base(collection)
        {
        }

        public DateTime Date { get; internal set; }
    }
}
