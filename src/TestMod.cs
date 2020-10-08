using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

[assembly: ModInfo("testmod",
	Description = "Test Mod",
	Authors     = new []{ "McJty" })]


namespace Test {

    public class TestMod : ModSystem {

        public static string MODID = "testmod";

        public Dictionary<string, SeedBagInventory> seedBagInventories = new Dictionary<string, SeedBagInventory>();

        public override void Start(ICoreAPI api) {
            base.Start(api);
			api.RegisterBlockBehaviorClass(TestBehavior.NAME, typeof(TestBehavior));
            api.RegisterItemClass(SeedBag.NAME, typeof(SeedBag));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
        }
    }
    
}