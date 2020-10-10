using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Test
{
    public class SeedBagInventory : InventoryBase
    {

        internal ItemSlot seedBagSlot;

        internal ItemSlot[] slots;

        public SeedBagInventory(string className, string instanceID, ICoreAPI api, ItemSlot seedBagSlot) : base(className, instanceID, api)
        {
            slots = new ItemSlot[8];
            for (int i = 0 ; i < 8 ; i++)
            {
                slots[i] = new ItemSlotSeeds(this);
            }
            this.seedBagSlot = seedBagSlot;
        }

        public void SyncToSeedBag()
        {
            if (seedBagSlot.Itemstack != null && seedBagSlot.Itemstack.Item is SeedBag)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (!(seedBagSlot.Itemstack is null))
                    {
                        seedBagSlot.Itemstack.Attributes.SetItemstack("s" + i, slots[i].Itemstack);
                    }
                }    
                seedBagSlot.MarkDirty();            
            }
        }

        public override float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            if (!(sourceSlot.Itemstack?.Item is ItemPlantableSeed))
            {
                return 0;
            }
            return base.GetSuitability(sourceSlot, targetSlot, isMerge);
        }

        internal void SyncFromSeedBag()
        {
            if (seedBagSlot.Itemstack != null && seedBagSlot.Itemstack.Item is SeedBag)
            {
                for (int i = 0; i < 8; i++)
                {
                    slots[i].Itemstack = seedBagSlot.Itemstack.Attributes.GetItemstack("s" + i);
                }
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