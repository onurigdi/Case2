using Game.Scripts.Controllers.Stack.Mono;
using Zenject;

namespace Game.Scripts.Pools
{
    public class StackPool : MonoMemoryPool<Stack>
    {
        protected override void OnCreated(Stack item)
        {
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(Stack item)
        {
            item.gameObject.SetActive(true);
            
        }

        protected override void OnDespawned(Stack item)
        {
            item.gameObject.SetActive(false);
        }
    }
}
