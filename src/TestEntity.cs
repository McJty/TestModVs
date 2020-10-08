using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Test {

    public class TestEntity : BlockEntity {

        public int timer;

        public override void Initialize(ICoreAPI api) {
            base.Initialize(api);
            RegisterGameTickListener(OnTick, 20);
        }

        public void OnTick(float par) {

        }

        public override void ToTreeAttributes(ITreeAttribute tree) {
            base.ToTreeAttributes(tree);
            tree.SetInt("timer", timer);
        }

        public override void FromTreeAtributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve) {
            base.FromTreeAtributes(tree, worldAccessForResolve);
            timer = tree.GetInt("timer");
        }

    }

}