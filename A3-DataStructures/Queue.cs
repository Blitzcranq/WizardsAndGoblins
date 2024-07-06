namespace COIS2020.vrajchauhan.Assignment3;

using System.Collections;
using System.Collections.Generic;


public class Queue<T> : IEnumerable<T>
{
    public const int DefaultCapacity = 8;

    private T?[] buffer;
    private int start;
    private int end;

    public bool IsEmpty
    {
        get
        {
            return Count == 0;
        }
    }
    public int Count
    {
        get
        {
            return end < start ? (Capacity - start + end) : (end - start);
        }
    }
    public int Capacity
    {
        get
        {
            return buffer.Length;
        }

    }
    public Queue() : this(DefaultCapacity)
    { }

    private void Grow()
    {
        T[] tmpBuffer = new T[Capacity * 2];
        Array.Copy(buffer, start, tmpBuffer, 0, Capacity - start);
        Array.Copy(buffer, 0, tmpBuffer, Capacity - start, end + 1);
        int oldLength = Count;
        buffer = tmpBuffer;
        end = oldLength;
        start = 0;

    }
    public Queue(int capacity)
    {
        buffer = new T?[capacity];
        start = 0;
        end = 0;
    }

    public void Enqueue(T item)
    {
        if (IsEmpty)
        {
            buffer[start] = item;
            end++;
        }
        else
        {
            if (Count == Capacity - 1) //capacity - 1 because we need to leave one spot empty to differentiate between full and empty
            {
                Grow();
            }

            buffer[end] = item;
            end = (end + 1) % Capacity;
        }

    }
    public T Dequeue()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Cannot remove when queue is empty");
        }

        T item = buffer[start]!;
        buffer[start] = default;
        start = (start + 1) % Capacity;
        return item;
    }
    public T Peek()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Cannot peek when queue is empty");
        }
        return buffer[start]!;

    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = start; i < start + Count; i++)
        {
            yield return buffer[i % Capacity]!;
        }
    }


}
