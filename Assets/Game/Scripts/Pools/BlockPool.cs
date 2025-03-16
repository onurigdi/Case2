using Game.Scripts.Managers.Block.Mono;
using Zenject;

namespace Game.Scripts.Pools
{
    public class BlockPool : MonoMemoryPool<Block>
    {
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
    }
}
