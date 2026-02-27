using System;

namespace DataStructures.StringLists
{
    /// <summary>
    /// Repräsentiert ein nicht-leeres Listenelement mit Nutzlast.
    /// </summary>
    public class StringListElement : IStringList
    {
        private string payload;
        private IStringList next;

        public StringListElement(string payload)
            : this(payload, new StringListEmpty())
        {
        }

        public StringListElement(string payload, IStringList next)
        {
            this.payload = payload;
            this.next = next;
        }

        public string GetPayload()
        {
            return payload;
        }

        public IStringList GetNext()
        {
            return next;
        }

        public bool IsEmpty()
        {
            return false;
        }

        public int Size()
        {
            return 1 + next.Size();
        }

        public bool Contains(string payload)
        {
            return this.payload.Equals(payload) || next.Contains(payload);
        }

        public IStringList Prepend(string payload)
        {
            return new StringListElement(payload, this);
        }

        public IStringList Append(string payload)
        {
            next = next.Append(payload);
            return this;
        }

        public IStringList Remove(string payload)
        {
            if (this.payload.Equals(payload))
            {
                return next;
            }

            next = next.Remove(payload);
            return this;
        }

        public string GetAt(int index)
        {
            if (index > 0)
            {
                return next.GetAt(index - 1);
            }
            else if (index == 0)
            {
                return payload;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return next.IsEmpty()
                ? payload
                : payload + "," + next.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (StringListElement)obj;
            return payload.Equals(other.payload) && next.Equals(other.next);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(payload, next);
        }
    }
}