using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustXaml.Models
{
    public class File
    {
        public string Title { get; set; }

        public void Remove()
        {
            var evt = Messages.Messenger.Instance.GetEvent<Messages.RemoveRecentFile>();
            evt.Publish(this);
        }
    }
}
