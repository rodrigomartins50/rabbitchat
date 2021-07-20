using System;
using System.Collections.Generic;

namespace RabbitChat.Domain
{
    public class PagedList<T> : IEnumerable<T>, IDisposable
    {
        private readonly List<T> _registers;

        public PagedList()
        {
            _registers = new List<T>();
        }

        public T this[int index]
        {
            get { return _registers[index]; }
            set { _registers.Insert(index, value); }
        }

        public int Total { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return _registers.GetEnumerator();
        }

        public void Add(T item)
        {
            _registers.Add(item);
        }

        public List<T> ToList()
        {
            return _registers;
        }

        public void Dispose()
        {
            _registers.Clear();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
