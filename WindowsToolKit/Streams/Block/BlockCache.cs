using System.Collections.Generic;
namespace WindowsToolKit.Streams
{
    public class BlockCache<T> where T : Block, new()
    {
        private readonly Dictionary<long, T> _blocks;
        private int _blocksCreated;
        private readonly int _blockSize;
        private readonly List<T> _freeBlocks;
        private readonly LinkedList<T> _lru;
        private readonly int _totalBlocks;
        public BlockCache(int blockSize, int blockCount)
        {
            _blockSize = blockSize;
            _totalBlocks = blockCount;
            _blocks = new Dictionary<long, T>();
            _lru = new LinkedList<T>();
            _freeBlocks = new List<T>(_totalBlocks);
            FreeBlockCount = _totalBlocks;
        }
        public int FreeBlockCount { get; private set; }
        public bool ContainsBlock(long position)
        {
            return _blocks.ContainsKey(position);
        }
        public bool TryGetBlock(long position, out T block)
        {
            if (_blocks.TryGetValue(position, out block))
            {
                _lru.Remove(block);
                _lru.AddFirst(block);
                return true;
            }
            return false;
        }
        public T GetBlock(long position)
        {
            T result;
            if (TryGetBlock(position, out result))
                return result;
            result = GetFreeBlock();
            result.Position = position;
            result.Available = -1;
            StoreBlock(result);
            return result;
        }
        public void ReleaseBlock(long position)
        {
            T block;
            if (_blocks.TryGetValue(position, out block))
            {
                _blocks.Remove(position);
                _lru.Remove(block);
                _freeBlocks.Add(block);
                FreeBlockCount++;
            }
        }

        private void StoreBlock(T block)
        {
            _blocks[block.Position] = block;
            _lru.AddFirst(block);
        }
        private T GetFreeBlock()
        {
            T block;
            if (_freeBlocks.Count > 0)
            {
                int idx = _freeBlocks.Count - 1;
                block = _freeBlocks[idx];
                _freeBlocks.RemoveAt(idx);
                FreeBlockCount--;
            }
            else if (_blocksCreated < _totalBlocks)
            {
                block = new T();
                block.Data = new byte[_blockSize];
                _blocksCreated++;
                FreeBlockCount--;
            }
            else
            {
                block = _lru.Last.Value;
                _lru.RemoveLast();
                _blocks.Remove(block.Position);
            }
            return block;
        }
    }
}