namespace DataStructures.StringLists
{
    /// <summary>
    /// List that can contain multiple strings. The list can contain duplicates. If the user does not want
    /// duplicates in the list, it is their responsibility not to insert identical elements multiple times.
    /// </summary>
    /// <authors>cei, Sean Hartwig, Harris von Hassell</authors>
    public class StringList
    {
        /// <summary>
        /// Reference to list head
        /// </summary>
        private IStringList head;

        /// <summary>
        /// Constructor for the empty list.
        /// </summary>
        public StringList()
        {
            head = new StringListEmpty();
        }

        /// <summary>
        /// Constructor that creates a list based on the passed strings. To optimize runtime,
        /// prepend() is used here.
        /// The constructor is only used for testing, but also in another package,
        /// so the visibility must be public.
        /// </summary>
        /// <param name="values">All values (strings) to be added to the list.</param>
        public StringList(params string[] values)
        {
            head = new StringListEmpty();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                head = head.Prepend(values[i]);
            }
        }

        /// <summary>
        /// Checks if the list is empty.
        /// </summary>
        /// <returns>true if the list is empty.</returns>
        public bool IsEmpty()
        {
            return head.IsEmpty();
        }

        /// <summary>
        /// Returns the number of list elements.
        /// </summary>
        /// <returns>The number of list elements, 0 if the list is empty.</returns>
        public int Size()
        {
            return head.Size();
        }

        /// <summary>
        /// Checks if the element is contained in the list.
        /// </summary>
        /// <param name="elem">The element to search for</param>
        /// <returns>true if the given element is contained in the list</returns>
        public bool Contains(string elem)
        {
            return head.Contains(elem);
        }

        /// <summary>
        /// Adds the given element to the beginning of the list. The list may contain duplicates.
        /// </summary>
        /// <param name="elem">Element to add</param>
        internal void Prepend(string elem)
        {
            head = head.Prepend(elem);
        }

        /// <summary>
        /// Adds the given element to the end of the list. The list may contain duplicates.
        /// </summary>
        /// <param name="elem">The element to add to the end</param>
        public void Append(string elem)
        {
            head = head.Append(elem);
        }

        /// <summary>
        /// Returns the element at the given index. If the index is not valid, null is returned.
        /// </summary>
        /// <param name="idx">Index where the searched element is located</param>
        /// <returns>The payload of the searched element or null if the index is not valid.</returns>
        public string GetAt(int idx)
        {
            return head.GetAt(idx);
        }

        /// <summary>
        /// Returns the payload of the first list element, null if the list is empty.
        /// </summary>
        /// <returns>The payload of the first list element, null if the list is empty.</returns>
        public string GetPayloadAtHead()
        {
            return head.IsEmpty() ? null : head.GetPayload();
        }

        /// <summary>
        /// Determines the index of a given element in the list. If the element is not contained, null
        /// is returned. That's why the data type int? (nullable int) is used instead of primitive int.
        /// If the element is contained multiple times, the index of the first occurrence is returned.
        /// It does not search for the same element (same reference), but for equal elements (equals).
        /// </summary>
        /// <param name="elem">The element whose index should be determined</param>
        /// <returns>Index of the first occurrence of the element or null if the element is not contained</returns>
        public int? GetIndexOf(string elem)
        {
            int? index = null;
            int idx = 0;
            /*
             * To iterate through the list, a variable 'cursor' is used. This can be
             * set to the next element without changing anything in the list.
             * If instead the 'head' were moved forward, the list would be
             * empty after using this loop, or would only contain the elements after the searched one.
             */
            // Traverse through the list with a cursor and check each element
            IStringList cursor = head;
            while (index == null && !cursor.IsEmpty())
            {
                if (cursor.GetPayload().Equals(elem))
                {
                    index = idx;
                }
                else
                {
                    idx++;
                    cursor = cursor.GetNext();
                }
            }
            return index;
        }

        /// <summary>
        /// Removes the element from the list. If the element is not contained, the list remains unchanged.
        /// </summary>
        /// <param name="elem">The element to remove</param>
        public void Remove(string elem)
        {
            head = head.Remove(elem);
        }

        /// <summary>
        /// Returns a copy of the list.
        /// </summary>
        /// <returns>The copy of this list</returns>
        public StringList Copy()
        {
            StringList copy = new StringList();
            IStringList cursor = head;
            while (!cursor.IsEmpty())
            {
                copy.Append(cursor.GetPayload());
                cursor = cursor.GetNext();
            }
            return copy;
        }

        /// <summary>
        /// Creates a string representation of the list.
        /// </summary>
        /// <returns>All values from the list separated by commas but without a comma at the end.</returns>
        public override string ToString()
        {
            return head.ToString();
        }

        /// <summary>
        /// Checks if the passed object is an equal list. Two lists are equal if they contain the same
        /// elements in the same order.
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>true if obj is an equal list</returns>
        public override bool Equals(object obj)
        {
            if (obj is not StringList other)
                return false;
            return this.head.Equals(other.head);
        }

        public override int GetHashCode()
        {
            return head.GetHashCode();
        }
    }
}