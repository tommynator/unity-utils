using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LimitedQueue<T> : IEnumerable<T>
{
    T[] queue;
    int capacity;
    int head;
    int size;

    public LimitedQueue(int capacity)
    {
        this.capacity = capacity;
        this.queue = new T[capacity];
    }

    public void Enqueue(T value)
    {
        queue[head] = value;
        head = (head + 1) % capacity;
        size = Mathf.Min(size + 1, capacity);
    }

    public T Dequeue()
    {
        if(size == 0)
            throw new System.InvalidOperationException();

        var i = (capacity + head - size) % capacity;
        size = Mathf.Max(size - 1, 0);
        return queue[i];
    }

    public void Clear()
    {
        size  = 0;
        head = 0;
    }

    public int Count
    {
        get { return size; }
    }

    public T[] ToArray()
    {
        int idx;
        T[] array = new T[size];
        for(int i = 0; i < size; i++)
        {
            idx = (i + capacity + head - size) % capacity;
            array[i] = queue[idx];
        }
        return array;
    }

    public List<T> ToList()
    {
        int idx;
        List<T> list = new List<T>(size);
        for(int i = 0; i < size; i++)
        {
            idx = (i + capacity + head - size) % capacity;
            list.Add(queue[idx]);
        }
        return list;
    }

    public T this[int index]
    {
        get 
        {
            index = Mathf.Clamp(index, 0, size - 1);
            var i = (index + capacity + head - size) % capacity;
            return queue[i];
        }
        set 
        {
            index = Mathf.Clamp(index, 0, size - 1);
            var i = (index + capacity + head - size) % capacity;
            queue[i] = value;
        }
    }

    public int LastIndex { get { return (capacity + head - 1) % capacity; } }
    public int SecondLastIndex { get { return (capacity + head - 2) % capacity; } }

    public T Last
    {
        get { return queue[LastIndex]; }
        set { queue[LastIndex] = value; }
    }

    public T SecondLast
    {
        get { return queue[SecondLastIndex]; }
        set { queue[SecondLastIndex] = value; }
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }
    
    [Serializable]
    public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
        private LimitedQueue<T> queue;
        private int next;
        private T current;
        
        object IEnumerator.Current
        {
            get
            {
                this.VerifyState();
                if (this.next <= 0)
                {
                    throw new InvalidOperationException();
                }
                return this.current;
            }
        }
        
        public T Current  {  get   {  return this.current;   }  }
        
        internal Enumerator(LimitedQueue<T> queue)
        {
            this.queue = queue;
            this.current = default(T);
            this.next = 0;
        }
        
        void IEnumerator.Reset()
        {
            this.VerifyState();
            this.next = 0;
        }
        
        public void Dispose()
        {
            this.queue = null;
        }
        
        private void VerifyState()
        {
            if (this.queue == null)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }
        
        public bool MoveNext()
        {
            this.VerifyState();
            if (this.next < 0)
            {
                return false;
            }
            if (this.next < this.queue.size)
            {
                this.current = this.queue[this.next++];
                return true;
            }
            this.next = -1;
            return false;
        }
    }
}