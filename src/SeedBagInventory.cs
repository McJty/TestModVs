using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Test
{
    public class SeedBagInventory : InventoryBase
    {

        ItemSlot[] slots;

        public SeedBagInventory(string className, string instanceID, ICoreAPI api) : base(className, instanceID, api)
        {
            slots = GenEmptySlots(8);
        }

        public void SyncToSeedBag(ItemSlot seedBagSlot)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!(seedBagSlot.Itemstack is null))
                {
                    seedBagSlot.Itemstack.Attributes.SetItemstack("s" + i, slots[i].Itemstack);
                }
            }                
        }

        internal void SyncFromSeedBag(ItemStack seedBagStack)
        {
            for (int i = 0; i < 8; i++)
            {
                slots[i].Itemstack = seedBagStack.Attributes.GetItemstack("s" + i);
            }
        }

        public override ItemSlot this[int slotId]
        {
            get => slots[slotId];
            set => slots[slotId] = value;
        }

        public override int Count => slots.Length;

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            SlotsFromTreeAttributes(tree, slots);

        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(slots, tree);
        }
    }
}