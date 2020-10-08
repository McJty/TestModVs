using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Test
{
    public class SeedBagInventory : InventoryBase
    {

        ItemSlot[] slots;

        private ItemSlot seedBagSlot;

        public SeedBagInventory(string className, string instanceID, ICoreAPI api, ItemSlot seedBagSlot) : base(className, instanceID, api)
        {
            slots = GenEmptySlots(8);
            this.seedBagSlot = seedBagSlot;
        }

        public override ItemSlot this[int slotId]
        {
            get => slots[slotId];
            set
            {
                slots[slotId] = value;
                seedBagSlot.Itemstack.Attributes.SetItemstack("s" + slotId, value.Itemstack);
            }
        }

        public override void OnItemSlotModified(ItemSlot slot)
        {
            base.OnItemSlotModified(slot);
            for (int i = 0 ; i < 8 ; i++)
            {
                if (slots[i] == slot)
                {
                    seedBagSlot.Itemstack.Attributes.SetItemstack("s" + i, slot.Itemstack);
                }
            }
        }


        public override int Count => slots.Length;

        internal void LateInitialize(string inventoryID, ICoreAPI api, ItemStack seedBagStack)
        {
            for (int i = 0; i < 8; i++)
            {
                slots[i].Itemstack = seedBagStack.Attributes.GetItemstack("s" + i);
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            SlotsFromTreeAttributes(tree, slots);
            for (int i = 0; i < 8; i++)
            {
                seedBagSlot.Itemstack.Attributes.SetItemstack("s" + i, slots[i].Itemstack);
            }

        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(slots, tree);
        }
    }
}