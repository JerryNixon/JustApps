using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustXaml.Messages
{
    public class Messenger
    {
        public readonly static EventAggregator Instance = new EventAggregator();
    }
}
