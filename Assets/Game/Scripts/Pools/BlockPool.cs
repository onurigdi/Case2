using System.Collections.Generic;
using Game.Scripts.Managers.Block.Mono;
using Zenject;

namespace Game.Scripts.Pools
{
    public class BlockPool : MonoMemoryPool<Block>
    {
        private List<Block> _activeBlocks = new List<Block>();
        protected override void OnCreated(Block item)
        {
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(Block item)
        {
            item.gameObject.SetActive(true);
            item.Reset();
        }

        protected override void OnDespawned(Block item)
        {
            item.gameObject.SetActive(false);
        }


        public Block SpawnBlock()
        { 
            Block addedBlock = Spawn();
            _activeBlocks.Add(addedBlock);
            return addedBlock;
        }
        
        public void DeSpawnBlock(Block block)
        {
            _activeBlocks.Remove(block);
            Despawn(block);
        }

        public void DespawnAllActiveBlocks()
        {
            foreach (var block in _activeBlocks)
            {
                Despawn(block);  
            }
            _activeBlocks.Clear();
        }
        
    }
}
