using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustTrek.Models
{
    public class Parameter : IEquatable<Parameter>
    {
        public string Param { get; set; }
        public int Max { get; set; } = 20;

        public override int GetHashCode() => ($"{Max};{Param}").GetHashCode();
        public static bool operator !=(Parameter p1, Parameter p2) => !p1.Equals(p2);
        public static bool operator ==(Parameter p1, Parameter p2) => p1.Equals(p2);
        public override bool Equals(object obj) => GetHashCode() == (obj as Parameter)?.GetHashCode();
        public bool Equals(Parameter parameter) => GetHashCode() == parameter?.GetHashCode();
    }
}
