using System.Reflection;

namespace Dapper.API.Enums.EnumBase
{
    public abstract class Enumeration
    {
        public string Name { get; protected set; }

        public int Value { get; protected set; }
    }

    public abstract class Enumeration<T> : Enumeration, IComparable
        where T : Enumeration<T>
    {
        public Enumeration(int value, string name) => (Value, Name) = (value, name);

        public static T Get(int value)
        {
            return GetAll().FirstOrDefault(m => m.Value == value) as T;
        }

        public static bool Get(int value, out T enumeration)
        {
            enumeration = Get(value);

            return enumeration != null;
        }

        public static T Get(string name)
        {
            return GetAll().FirstOrDefault(m => m.Name == name) as T;
        }

        public static bool Get(string name, out T enumeration)
        {
            enumeration = Get(name);

            return enumeration != null;
        }

        public static IEnumerable<T> GetAll()
        {
            // Get all the fields of the <T> that are public, static and create within the type itself (no inherited fields allowed)
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null)) // Extracts the values of the fields
                .Cast<T>(); // Returns an IEnumberable of type <T>
        }

        public override string ToString() => Value.ToString();

        public override bool Equals(object obj)
        {
            // Unboxes the other object to the current Type
            var otherValue = obj as T;

            // If the value is null, return false
            if (otherValue == null)
            {
                return false;
            }

            // Checks if this type and the current type are the same type
            var typeMatches = GetType().Equals(obj.GetType());

            // Checks their values are the same
            var valueMatches = Value.Equals(otherValue.Value);

            // If they are the same type and have the same value return true, else false
            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(object other) => Value.CompareTo(((T)other).Value);
    }
}
