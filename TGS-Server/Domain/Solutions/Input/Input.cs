using static AngouriMath.Entity;

namespace Domain
{
    public interface Input
    {
        Variable variable { get; set; }
        public bool Equals(object obj);
        public int GetHashCode();
        public string ToString();
    }
}
