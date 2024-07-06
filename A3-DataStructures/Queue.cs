namespace COIS2020.vrajchauhan.Assignment3;

using System.Collections;
using System.Collections.Generic;


public class Queue<T> : IEnumerable<T>
{
    public const int DefaultCapacity = 8; //`DefaultCapacity` is a constant that represents the default capacity of the queue.

    private T?[] buffer; //`buffer` is an array of type `T` that represents the queue.
    private int start; //`start` is an integer that represents the index of the first element in the queue.
    private int end; //`end` is an integer that represents the index of the last element in the queue.

    public bool IsEmpty //`IsEmpty` is a boolean property that returns true if the queue is empty and false otherwise.
    {
        get
        {
            return start == end;
        }
    }
    public int Count //`Count` is an integer property that returns the number of elements in the queue.
    {
        get
        {
            return end < start ? (buffer.Length - start + end) : (end - start); //`Count` returns the number of elements in the queue by calculating the difference between the `end` and `start` indices.
        }
    }
    public int Capacity //`Capacity` is an integer property that returns the capacity of the queue.
    {
        get
        {
            return buffer.Length - 1; //`Capacity` returns the capacity of the queue by subtracting 1 from the length of the `buffer` array.
        }

    }
    public Queue() : this(DefaultCapacity)  //`Queue` is a constructor that initializes the queue with the default capacity.
    { }

    private void Grow() //`Grow` is a private method that increases the capacity of the queue by creating a new buffer with double the capacity and copying the elements from the old buffer to the new buffer.
    {
        T[] tmpBuffer = new T[(Capacity + 1) * 2]; //`Grow` creates a new buffer with double the capacity of the old buffer.
        Array.Copy(buffer, start, tmpBuffer, 0, buffer.Length - start); //`Grow` copies the elements from the old buffer to the new buffer.
        Array.Copy(buffer, 0, tmpBuffer, buffer.Length - start, end + 1); //`Grow` copies the elements from the old buffer to the new buffer.
        int oldLength = Count; //`Grow` stores the old length of the queue.
        buffer = tmpBuffer; //`Grow` assigns the new buffer to the old buffer.
        end = oldLength; //`Grow` updates the end index of the queue.
        start = 0; //`Grow` updates the start index of the queue.

    }
    public Queue(int capacity) //one parameter constructor
    {
        buffer = new T?[capacity];
        start = 0;
        end = 0;
    }

    public void Enqueue(T item) //`Enqueue` is a method that adds an element to the end of the queue.
    {
        if (IsEmpty)
        {
            buffer[start] = item; //`Enqueue` adds the element to the start index of the queue if the queue is empty.
            end++;
        }
        else
        {
            if (Count == Capacity) //`Enqueue` checks if the queue is full and calls the `Grow` method if necessary.
            {
                Grow();
            }

            buffer[end] = item; //`Enqueue` adds the element to the end index of the queue.
            end = (end + 1) % buffer.Length; //`Enqueue` updates the end index of the queue.
        }

    }
    public T Dequeue() //`Dequeue` is a method that removes and returns the first element in the queue.
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Cannot remove when queue is empty"); //`Dequeue` throws an exception if the queue is empty.
        }

        T item = buffer[start]!; //`Dequeue` removes and returns the first element in the queue.
        buffer[start] = default; //`Dequeue` sets the first element to the default value.
        start = (start + 1) % buffer.Length; //`Dequeue` increments the start index of the queue.
        return item; //`Dequeue` returns the removed element.
    }
    public T Peek() //`Peek` is a method that returns the first element in the queue without removing it.
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Cannot peek when queue is empty"); //`Peek` throws an exception if the queue is empty.
        }
        return buffer[start]!; //`Peek` returns the first element in the queue.

    }
    IEnumerator IEnumerable.GetEnumerator() //`IEnumerable.GetEnumerator` is a method that returns an enumerator for the queue.
    {
        return GetEnumerator(); //`IEnumerable.GetEnumerator` returns the enumerator for the queue.
    }

    public IEnumerator<T> GetEnumerator()  //`GetEnumerator` is a method that returns an enumerator for the queue.
    {
        for (int i = start; i < end; i++) //`GetEnumerator` is a method that returns an enumerator for the queue.
        {
            yield return buffer[i % Capacity]!; //`GetEnumerator` returns the elements in the queue using a loop.
        }
    }


}
