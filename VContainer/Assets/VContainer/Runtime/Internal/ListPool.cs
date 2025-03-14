using System;
using System.Collections.Generic;

namespace VContainer.Internal
{
    public static class ListPool<T>
    {
        const int DefaultCapacity = 32;

        private static readonly Stack<List<T>> _pool = new Stack<List<T>>(4);
        
        /// <summary>
        /// BufferScope supports releasing a buffer with using clause.
        /// </summary>
        public readonly struct BufferScope : IDisposable
        {
            private readonly List<T> _buffer;

            public BufferScope(List<T> buffer)
            {
                _buffer = buffer;
            }
            
            public void Dispose()
            {
                Release(_buffer);
            }
        }

        /// <summary>
        /// Get a buffer from the pool.
        /// </summary>
        /// <returns></returns>
        public static List<T> Get()
        {
            lock (_pool)
            {
                if (_pool.Count == 0)
                {
                    return new List<T>(DefaultCapacity);
                }

                return _pool.Pop();
            }
        }

        /// <summary>
        /// Get a buffer from the pool. Returning a disposable struct to support recycling via using clause.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static BufferScope Get(out List<T> buffer)
        {
            buffer = Get();
            return new BufferScope(buffer);
        }

        /// <summary>
        /// Declare a buffer won't be used anymore and put it back to the pool.  
        /// </summary>
        /// <param name="buffer"></param>
        public static void Release(List<T> buffer)
        {
            buffer.Clear();
            lock (_pool)
            {
                _pool.Push(buffer);
            }
        }
    }
}