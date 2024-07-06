namespace COIS2020.vrajchauhan.Assignment3;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // For NotNull attributes



public sealed class Node<T>
{
    public T Item { get; set; }

    // "internal" = only things within `A3-DataStructures` have access (AKA can access within LinkedList, but not from
    // within Program.cs)
    public Node<T>? Next { get; internal set; }
    public Node<T>? Prev { get; internal set; }

    // Constructor
    public Node(T item)
    {
        Item = item;
    }
}

// LinkedList class
public class LinkedList<T> : IEnumerable<T>
{
    public Node<T>? Head { get; protected set; }
    public Node<T>? Tail { get; protected set; }

    public int Count
    { // This is a property, not a field. It's a "getter" that calculates the value when you access it.
        get
        {
            int count = 0;
            Node<T>? curr = Head;
            while (curr != null)
            {
                count++;
                curr = curr.Next;
            }
            return count;

        }
    }
    public LinkedList()
    {
        Head = null;
        Tail = null;
    }


    // IEnumerable is done for you:

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); // Call the <T> version

    public IEnumerator<T> GetEnumerator()
    {
        Node<T>? curr = Head;
        while (curr != null)
        {
            yield return curr.Item;
            curr = curr.Next;
        }
    }


    // This getter is done for you:

    /// <summary>
    /// Determines whether or not this list is empty or not.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Head))] // (these "attributes" tell the `?` thingies that Head and Tail are not
    [MemberNotNullWhen(false, nameof(Tail))] // null whenever this getter returns `false`, which stops the `!` warnings)
    public bool IsEmpty
    {
        get
        {
            bool h = Head == null;
            bool t = Tail == null;
            if (h ^ t) // Can't hurt to do a sanity check while we're here.
                throw new Exception("Head and Tail should either both be null or both non-null.");
            return h;
        }
    }


    // --------------------------------------------------------------
    // Put your code down here:
    // --------------------------------------------------------------

    // AddFront, AddBack, InsertAfter, InsertBefore, Remove, SplitAfter, AppendAll, Find

    //AddFront adds a new node to the front of the list
    public void AddFront(T item)
    {
        if (IsEmpty) //If the list is empty, the new node is both the head and the tail
        {

            Head = new Node<T>(item);
            Tail = Head;
        }
        else //If the list is not empty, the new node is added to the front of the list
        {
            Node<T> curr = Head;
            Head = new Node<T>(item);
            Head.Next = curr; //The next node of the new node is the current head
            Head.Next.Prev = Head; //The previous node of the next node is the new node
        }


    }
    public void AddFront(Node<T> node) //Overload of AddFront that takes a node as a parameter
    {
        AddFront(node.Item);
    }

    //AddBack adds a new node to the back of the list
    public void AddBack(T item)
    {
        if (IsEmpty) //If the list is empty, the new node is both the head and the tail
        {
            Head = new Node<T>(item);
            Tail = Head;
        }
        else //If the list is not empty, the new node is added to the back of the list
        {
            Node<T> last = Tail;
            last.Next = new Node<T>(item);
            Tail = last.Next; //The new node is the new tail
            Tail.Prev = last; //The previous node of the new node is the last node
        }
    }
    public void AddBack(Node<T> node) //Overload of AddBack that takes a node as a parameter
    {
        AddBack(node.Item);
    }

    //InsertAfter inserts a new node after a specified node
    public void InsertAfter(Node<T> node, T item)
    {
        if (node == null) //If the node is null, an exception is thrown
        {
            throw new ArgumentNullException("Node cannot be null.");
        }

        Node<T> newNode = new Node<T>(item); //The new node is created and is inserted after the specified node 

        newNode.Next = node.Next;
        newNode.Prev = node;

        //If the new node is not the last node, the next node's previous node is the new node
        node.Next = newNode;

        if (newNode.Next != null)
        {
            newNode.Next.Prev = newNode;
        }
        else
        {
            Tail = newNode;
        }
    }

    //Overload of InsertAfter that takes a node as a parameter
    public void InsertAfter(Node<T> node, Node<T> newNode)
    {
        InsertAfter(node, newNode.Item);

    }

    //InsertBefore inserts a new node before a specified node
    public void InsertBefore(Node<T> node, T item)
    {
        if (node.Prev == null) //If the previous node of the specified node is null, the new node is added to the front of the list
        {
            AddFront(item);
        }
        else
        {
            InsertAfter(node.Prev, item); //If the previous node of the specified node is not null, the new node is inserted after the previous node becuase why not??
        }
    }
    //Overload of InsertBefore that takes a node as a parameter
    public void InsertBefore(Node<T> node, Node<T> newNode)
    {
        InsertBefore(node, newNode.Item);

    }

    //Remove removes a specified node from the list
    public void Remove(Node<T> node)
    {
        if (IsEmpty || node == null) //If the list is empty or the node is null, an exception is thrown
        {
            throw new InvalidOperationException("Cannot remove from an empty list or remove a null node.");
        }

        if (node == Head) //If the node is the head, the head is set to the next node
        {
            Head = Head.Next;
            if (Head != null) //If the new head is not null, the previous node of the new head is null
            {
                Head.Prev = null;
            }
            else //If the new head is null, the tail is also null
            {
                Tail = null;
            }
        }
        else if (node == Tail) //If the node is the tail, the tail is set to the previous node
        {
            Tail = Tail.Prev;
            if (Tail != null) //If the new tail is not null, the next node of the new tail is null
            {
                Tail.Next = null;
            }
            else //If the new tail is null, the head is also null
            {
                Head = null;
            }
        }
        else //If the node is neither the head nor the tail, the previous node's next node is set to the next node and the next node's previous node is set to the previous node
        {
            node.Prev!.Next = node.Next;
            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
        }

    }

    //Remove removes a specified item from the list
    public void Remove(T item)

    {
        if (Find(item) != null && item != null) //If the item is found in the list, the node is removed
        {
            Remove(Find(item)!);
        }
        else //If the item is not found in the list, an exception is thrown
        {
            throw new InvalidOperationException("Item not found in list.");
        }
    }

    //SplitAfter splits the list after a specified node
    public LinkedList<T> SplitAfter(Node<T> node)
    {
        if (node == null || node.Next == null)
        {
            throw new InvalidOperationException("Node is null or does not have a next node.");
        }
        //The new linked list is created and the head of the new linked list is set to the next node of the specified node
        LinkedList<T> newLinkedList = new LinkedList<T>();
        newLinkedList.Head = node.Next;
        newLinkedList.Tail = Tail;

        //The next node of the specified node is set to null and the previous node of the head of the new linked list is also set to null
        newLinkedList.Head.Prev = null;

        Tail = node;
        node.Next = null;

        return newLinkedList;
    }

    //AppendAll appends all the nodes of another linked list to the end of the current linked list
    public void AppendAll(LinkedList<T> otherList)
    {
        if (otherList == null || otherList.IsEmpty)
        {
            return;
        }

        if (IsEmpty) //If the current linked list is empty, the head and tail of the current linked list are set to the head and tail of the other linked list
        {
            Head = otherList.Head;
            Tail = otherList.Tail;
        }
        else
        {
            Tail.Next = otherList.Head; //tail of current list points to head of other list
            otherList.Head.Prev = Tail;  //head of other list points to tail of current list
            Tail = otherList.Tail;    //tail of current list is now the tail of other list
        }

        otherList.Head = null; //head of other list is now null
        otherList.Tail = null; //tail of other list is now null
    }

    //Find finds a specified item in the list
    public Node<T>? Find(T item) //Returns the node that contains the specified item
    {
        if (IsEmpty) //If the list is empty, null is returned
        {
            return null;
        }
        Node<T> curr = Head; //The current node is set to the head of the list
        EqualityComparer<T> comparer = EqualityComparer<T>.Default; //The comparer is set to the default comparer
        while (curr != null)
        {
            if (comparer.Equals(curr.Item, item))
            {
                return curr;
            }
            curr = curr.Next!; //The current node is set to the next node
        }
        return null;
    }
}

