using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Test {

    public class TestBehavior : BlockBehavior {

      public static string NAME { get; } = "TestBlock";
      
      public TestBehavior(Block block)
        : base(block) {  }

      public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer,
            BlockSelection blockSel, ref EnumHandling handling) {
                  (world as IServerWorldAccessor)?.CreateExplosion(
                      blockSel.Position, EnumBlastType.RockBlast, 5.0, 8.0);
                  handling = EnumHandling.PreventDefault;
                  return true;
      }

    }

}