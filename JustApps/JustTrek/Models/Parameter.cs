using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustTrek.Models
{
    public class Parameter
    {
        public string Title { get; set; }
        public string Param { get; set; }

        public override int GetHashCode() => ($"{Title};{Param}").GetHashCode();
        public static bool operator !=(Parameter p1, Parameter p2) => !p1.Equals(p2);
        public static bool operator ==(Parameter p1, Parameter p2) => p1.Equals(p2);
        public override bool Equals(object obj) => GetHashCode() == (obj as Parameter)?.GetHashCode();
    }
}
