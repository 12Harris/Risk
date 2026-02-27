namespace DataStructures.StringLists
{
    public interface IStringList
    {
        string GetPayload();
        IStringList GetNext();

        bool IsEmpty();
        int Size();
        bool Contains(string payload);

        IStringList Prepend(string payload);
        IStringList Append(string payload);
        IStringList Remove(string payload);

        string GetAt(int index);

        string ToString();
        bool Equals(object obj);
    }
}