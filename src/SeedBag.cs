using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using System;
using Vintagestory.GameContent;
using Vintagestory.API.Server;

namespace Test
{

    public class SeedBag : Item
    {

        public static string NAME { get; } = "SeedBag";

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (byEntity.Controls.Sneak)
            {
                OpenSeedBagGui(slot, byEntity);
            }
            else
            {
                if (byEntity.World is IServerWorldAccessor)
                {
                    TryPlacingSeeds(slot, byEntity, blockSel);
                }
            }
            handling = EnumHandHandling.Handled;
        }

        private void TryPlacingSeeds(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel)
        {
            if (blockSel == null) return;

            IPlayer byPlayer = (byEntity as EntityPlayer) ?. Player;

            BlockPos pos = blockSel.Position;

            SeedBagInventory inventory = new SeedBagInventory("seedbagInv", "id", api);
            inventory.SyncFromSeedBag(slot.Itemstack);

            for (int x = -1 ; x <= 1 ; x++)
            {
                for (int z = -1 ; z <= 1 ; z++)
                {
                    BlockPos p = pos.AddCopy(x, 0, z);
                    blockSel.Position = p;
                    BlockEntity be = byEntity.World.BlockAccessor.GetBlockEntity(p);
                    if (be is BlockEntityFarmland)
                    {
                        foreach (ItemSlot seedSlot in inventory.slots)
                        {
                            ItemStack seed = seedSlot.Itemstack;
                            if (!(seed is null) && !(seed.Item is null) && seed.StackSize > 0)
                            {
                                EnumHandHandling handling = EnumHandHandling.Handled;
                                seed.Item.OnHeldInteractStart(seedSlot, byEntity, blockSel, null, false, ref handling);
                                break;
                            }
                        }
                    }
                }
            }

            inventory.SyncToSeedBag(slot);
            slot.MarkDirty();
        }

        private void OpenSeedBagGui(ItemSlot slot, EntityAgent byEntity)
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

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction
                {
                    HotKeyCode = "sneak",
                    ActionLangCode = "heldhelp-gui",
                    MouseButton = EnumMouseButton.Right
                },
                new WorldInteraction
                {
                    ActionLangCode = "heldhelp-plant",
                    MouseButton = EnumMouseButton.Right
                }

            };//.Append(base.GetHeldInteractionHelp(inSlot));
        }

    }

}