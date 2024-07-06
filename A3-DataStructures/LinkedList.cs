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


    public Node(T item)
    {
        Item = item;
    }
}


public class LinkedList<T> : IEnumerable<T>
{
    public Node<T>? Head { get; protected set; }
    public Node<T>? Tail { get; protected set; }

    public int Count

    {
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
    public void AddFront(T item)
    {
        if (IsEmpty)
        {

            Head = new Node<T>(item);
            Tail = Head;
        }
        else
        {
            Node<T> curr = Head;
            Head = new Node<T>(item);
            Head.Next = curr;
            Head.Next.Prev = Head;
        }


    }
    public void AddFront(Node<T> node)
    {
        AddFront(node.Item);
    }
    public void AddBack(T item)
    {
        if (IsEmpty)
        {
            Head = new Node<T>(item);
            Tail = Head;
        }
        else
        {
            Node<T> last = Tail;
            last.Next = new Node<T>(item);
            Tail = last.Next;
            Tail.Prev = last;
        }
    }
    public void AddBack(Node<T> node)
    {
        AddBack(node.Item);
    }
    public void InsertAfter(Node<T> node, T item)
    {
        if (node == null)
        {
            throw new ArgumentNullException("Node cannot be null.");
        }

        Node<T> newNode = new Node<T>(item)
        {
            Next = node.Next,
            Prev = node
        };



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
    public void InsertAfter(Node<T> node, Node<T> newNode)
    {
        InsertAfter(node, newNode.Item);

    }
    public void InsertBefore(Node<T> node, T item)
    {
        if (node.Prev == null)
        {
            AddFront(item);
        }
        else
        {
            InsertAfter(node.Prev, item);
        }
    }
    public void InsertBefore(Node<T> node, Node<T> newNode)
    {
        InsertBefore(node, newNode.Item);

    }
    public void Remove(Node<T> node)
    {
        if (IsEmpty || node == null)
        {
            throw new InvalidOperationException("Cannot remove from an empty list or remove a null node.");
        }

        if (node == Head)
        {
            Head = Head.Next;
            if (Head != null)
            {
                Head.Prev = null;
            }
            else
            {
                Tail = null;
            }
        }
        else if (node == Tail)
        {
            Tail = Tail.Prev;
            if (Tail != null)
            {
                Tail.Next = null;
            }
        }
        else
        {
            node.Prev!.Next = node.Next;
            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
        }
    }

    public void Remove(T item)

    {
        Remove(Find(item)!);
    }
    public LinkedList<T> SplitAfter(Node<T> node)
    {
        if (node == null || node.Next == null)
        {
            throw new InvalidOperationException("Node is null or does not have a next node.");
        }

        LinkedList<T> newLinkedList = new LinkedList<T>
        {
            Head = node.Next,
            Tail = Tail
        };
        newLinkedList.Head.Prev = null;

        Tail = node;
        node.Next = null;

        return newLinkedList;
    }

    public void AppendAll(LinkedList<T> otherList)
    {
        if (otherList == null || otherList.IsEmpty)
        {
            return;
        }

        if (IsEmpty)
        {
            Head = otherList.Head;
            Tail = otherList.Tail;
        }
        else
        {
            Tail.Next = otherList.Head;
            otherList.Head.Prev = Tail;
            Tail = otherList.Tail;
        }
    }

    public Node<T>? Find(T item)
    {
        if (IsEmpty)
        {
            return null;
        }
        Node<T> curr = Head;
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        while (curr != null)
        {
            if (comparer.Equals(curr.Item, item))
            {
                return curr;
            }
            curr = curr.Next!;
        }
        return null;
    }
}

