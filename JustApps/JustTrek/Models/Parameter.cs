using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustTrek.Models
{
    public class Parameter : IEquatable<Parameter>
    {
        public string Title { get; set; }
        public string Param { get; set; }

        public Kinds Kind { get; set; } = Kinds.None;
        public enum Kinds { Facebook, Twitter, Flickr, Rss, None }

        public override int GetHashCode() => ($"{Title};{Param}").GetHashCode();
        public static bool operator !=(Parameter p1, Parameter p2) => !p1.Equals(p2);
        public static bool operator ==(Parameter p1, Parameter p2) => p1.Equals(p2);
        public override bool Equals(object obj) => GetHashCode() == (obj as Parameter)?.GetHashCode();
        public bool Equals(Parameter parameter) => GetHashCode() == parameter?.GetHashCode();
    }
}
