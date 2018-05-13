using System;
using System.Collections;
using System.Collections.Generic;

namespace ZqSpider.Core.Schedulers
{
    public class BloomFilter<T>
    {
        Random _random;
        int _bitSize, _numberOfHashes, _setSize;
        BitArray _bitArray;

        #region Constructors
        
        public BloomFilter(int bitSize, int setSize)
        {
            _bitSize = bitSize;
            _bitArray = new BitArray(bitSize);
            _setSize = setSize;
            _numberOfHashes = OptimalNumberOfHashes(_bitSize, _setSize);
        }
        
        public BloomFilter(int bitSize, int setSize, int numberOfHashes)
        {
            _bitSize = bitSize;
            _bitArray = new BitArray(bitSize);
            _setSize = setSize;
            _numberOfHashes = numberOfHashes;
        }
        #endregion

        #region Properties
        
        public int NumberOfHashes
        {
            set
            {
                _numberOfHashes = value;
            }
            get
            {
                return _numberOfHashes;
            }
        }

        public int SetSize
        {
            set
            {
                _setSize = value;
            }
            get
            {
                return _setSize;
            }
        }

        public int BitSize
        {
            set
            {
                _bitSize = value;
            }
            get
            {
                return _bitSize;
            }
        }
        #endregion

        #region Public Methods
        public void Add(T item)
        {
            _random = new Random(Hash(item));

            for (int i = 0; i < _numberOfHashes; i++)
                _bitArray[_random.Next(_bitSize)] = true;
        }

        public bool Contains(T item)
        {
            _random = new Random(Hash(item));

            for (int i = 0; i < _numberOfHashes; i++)
            {
                if (!_bitArray[_random.Next(_bitSize)])
                    return false;
            }

            return true;
        }

        public bool ContainsAny(List<T> items)
        {
            foreach (T item in items)
            {
                if (Contains(item))
                    return true;
            }

            return false;
        }

        public bool ContainsAll(List<T> items)
        {
            foreach (T item in items)
            {
                if (!Contains(item))
                    return false;
            }

            return true;
        }

        public double FalsePositiveProbability()
        {
            return Math.Pow((1 - Math.Exp(-_numberOfHashes * _setSize / (double)_bitSize)), _numberOfHashes);
        }
        #endregion

        #region Private Methods
        private int Hash(T item)
        {
            return item.GetHashCode();
        }

        private int OptimalNumberOfHashes(int bitSize, int setSize)
        {
            return (int)Math.Ceiling((bitSize / setSize) * Math.Log(2.0));
        }
        #endregion
    }
}
