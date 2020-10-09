using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using System;

namespace Test
{

    public class SeedBag : Item
    {

        private static SimpleParticleProperties particles = new SimpleParticleProperties(
                    1, 1,
                    ColorUtil.ColorFromRgba(50, 50, 50, 220),
                    new Vec3d(),
                    new Vec3d(),
                    new Vec3f(-0.25f, 0.1f, -0.25f),
                    new Vec3f(0.25f, 0.1f, 0.25f),
                    1.5f,
                    -0.075f,
                    0.25f,
                    0.25f,
                    EnumParticleModel.Quad
                );


        public static string NAME { get; } = "SeedBag";

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (byEntity.Controls.Sneak)
            {
                SeedBagInventory inventory = new SeedBagInventory("seedbagInv", "id", api);
                inventory.SyncFromSeedBag(slot.Itemstack);
                inventory.ResolveBlocksOrItems();
                inventory.OnInventoryClosed += OnCloseInventory;
                IPlayer player = (byEntity as EntityPlayer).Player;

                TestMod mod = api.ModLoader.GetModSystem<TestMod>();
                mod.seedBagInventories.Add(player.PlayerUID, inventory);
                inventory.SlotModified += index => OnSlotModified(player.PlayerUID, player.InventoryManager.ActiveHotbarSlot, index);

                player.InventoryManager.OpenInventory(inventory);

                if (byEntity.World is IClientWorldAccessor)
                {
                    GuiSeedBag guiSeedBag = new GuiSeedBag(api as ICoreClientAPI, inventory, slot);
                    guiSeedBag.TryOpen();
                }
            }
            handling = EnumHandHandling.Handled;
        }

        private void OnSlotModified(string playerID, ItemSlot activeHotbarSlot, int index)
        {
            Console.WriteLine("OnSlotModified");
            TestMod mod = api.ModLoader.GetModSystem<TestMod>();
            SeedBagInventory inventory;
            mod.seedBagInventories.TryGetValue(playerID, out inventory);
            if (!(inventory is null))
            {
                inventory.SyncToSeedBag(activeHotbarSlot);
                activeHotbarSlot.MarkDirty();
            }
        }

        private void OnCloseInventory(IPlayer player)
        {
            Console.WriteLine("OnCloseInventory");
            TestMod mod = api.ModLoader.GetModSystem<TestMod>();
            SeedBagInventory inventory;
            mod.seedBagInventories.TryGetValue(player.PlayerUID, out inventory);
            if (!(inventory is null))
            {
                inventory.SyncToSeedBag(player.InventoryManager.ActiveHotbarSlot);
                mod.seedBagInventories.Remove(player.PlayerUID);
                player.InventoryManager.ActiveHotbarSlot.MarkDirty();
            }
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (byEntity.Controls.Sneak)
            {
                return false;
            }
            if (byEntity.World is IClientWorldAccessor)
            {
                ModelTransform tf = new ModelTransform();
                tf.EnsureDefaultValues();

                tf.Origin.Set(0, -1, 0);
                tf.Rotation.Z = Math.Min(30, secondsUsed * 40);
                byEntity.Controls.UsingHeldItemTransformAfter = tf;

                if (secondsUsed > 0.6)
                {
                    Vec3d pos = byEntity.Pos.XYZ.Add(0, byEntity.LocalEyePos.Y, 0)
                            .Ahead(1f, byEntity.Pos.Pitch, byEntity.Pos.Yaw);
                    Vec3f speedVec = new Vec3d(0, 0, 0).Ahead(5, byEntity.Pos.Pitch, byEntity.Pos.Yaw).ToVec3f();
                    particles.MinVelocity = speedVec;
                    Random rand = new Random();
                    particles.Color = ColorUtil.ColorFromRgba(255, rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                    particles.MinPos = pos.AddCopy(-0.05, -0.05, -0.05);
                    particles.AddPos.Set(0.1, 0.1, 0.1);
                    particles.MinSize = 0.1F;
                    particles.SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.SINUS, 10);
                    byEntity.World.SpawnParticles(particles);
                }
            }
            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            base.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel);
        }

    }

}