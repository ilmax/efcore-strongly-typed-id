using System;

namespace StronglyTypedId
{
    public abstract class LongTypedIdValueBase : IEquatable<LongTypedIdValueBase>
    {
        public long Value { get; }

        protected LongTypedIdValueBase(long value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LongTypedIdValueBase other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(LongTypedIdValueBase other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.Equals(other.Value);
        }

        public static bool operator ==(LongTypedIdValueBase obj1, LongTypedIdValueBase obj2)
        {
            if (Equals(obj1, null))
            {
                if (Equals(obj2, null))
                {
                    return true;
                }
                return false;
            }
            return obj1.Equals(obj2);
        }

        public static bool operator ==(LongTypedIdValueBase left, long right)
        {
            if (left is null)
                return false;

            return left.Value == right;
        }

        public static bool operator !=(LongTypedIdValueBase x, LongTypedIdValueBase y)
        {
            return !(x == y);
        }

        public static bool operator !=(LongTypedIdValueBase left, long right)
        {
            return !(left == right);
        }

        public static bool operator ==(LongTypedIdValueBase left, int right)
        {
            if (left is null)
                return false;

            return left.Value == right;
        }

        public static bool operator !=(LongTypedIdValueBase left, int right)
        {
            return !(left == right);
        }

        public static bool operator >(LongTypedIdValueBase left, int right)
        {
            if (left is null)
                return false;

            return left.Value > right;
        }

        public static bool operator <(LongTypedIdValueBase left, int right)
        {
            return !(left > right);
        }

        public override string ToString() => Value.ToString();
    }
}