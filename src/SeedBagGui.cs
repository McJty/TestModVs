using System;
using Vintagestory.API.Client;

namespace Test
{

    public class GuiSeedBag : GuiDialog
    {

        public override string ToggleKeyCombinationCode => null;


        private SeedBagInventory inventory;

        public GuiSeedBag(ICoreClientAPI api, SeedBagInventory inventory) : base(api)
        {
            this.inventory = inventory;
            SetupDialog();
            this.OnClosed += CloseMe;
        }

        private void CloseMe()
        {
            object packet = inventory.Close(capi.World.Player);
            capi.Network.SendPacketClient(packet);
            TestMod mod = capi.ModLoader.GetModSystem<TestMod>();
            mod.seedBagInventories.Remove(capi.World.Player.PlayerUID);
        }

        private void SetupDialog()
        {
            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            // Just a simple 300x300 pixel box
            ElementBounds textBounds = ElementBounds.Fixed(0, 40, 300, 120);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(textBounds);

            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("myAwesomeDialog", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddItemSlotGrid(inventory, SendInvPacket, 4, textBounds)
                .Compose()
            ;
        }

        private void SendInvPacket(object packet)
        {
            capi.Network.SendPacketClient(packet);
        }

    }

}