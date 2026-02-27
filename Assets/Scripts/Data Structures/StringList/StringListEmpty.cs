using System;

namespace DataStructures.StringLists
{
    /// <summary>
    /// Repräsentiert ein leeres Listenelement (Listenende).
    /// </summary>
    public class StringListEmpty : IStringList
    {
        public string GetPayload()
        {
            throw new InvalidOperationException(
                "Ein leeres Listenelement darf keine Nutzlast haben");
        }

        public IStringList GetNext()
        {
            throw new InvalidOperationException(
                "Ein leeres Listenelement hat keinen Nachfolger");
        }

        public bool IsEmpty()
        {
            return true;
        }

        public int Size()
        {
            return 0;
        }

        public bool Contains(string payload)
        {
            return false;
        }

        public IStringList Prepend(string payload)
        {
            return new StringListElement(payload, this);
        }

        public IStringList Append(string payload)
        {
            return Prepend(payload);
        }

        public IStringList Remove(string payload)
        {
            return this;
        }

        public string GetAt(int index)
        {
            return null;
        }

        public override string ToString()
        {
            return "";
        }

        public override bool Equals(object obj)
        {
            return obj is StringListEmpty;
        }

        public override int GetHashCode()
        {
            return typeof(StringListEmpty).GetHashCode();
        }
    }
}