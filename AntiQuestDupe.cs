using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB.SteelSeries;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace FishDuplicationPrevention
{
    [ApiVersion(2, 1)]
    public class Main : TerrariaPlugin
    {
        public override string Name => "AntiQuestFishDupe";
        public override string Author => "TRANQUILZOIIP - github.com/bbeeeeenn";
        public override string Description => base.Description;
        public override Version Version => base.Version;

        public Main(Terraria.Main game)
            : base(game) { }

        public override void Initialize()
        {
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            }
            base.Dispose(disposing);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            using var reader = new BinaryReader(
                new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)
            );
            if (args.MsgID == PacketTypes.ItemDrop)
            {
                var itemID = reader.ReadInt16();
                _ = reader.ReadSingle();
                _ = reader.ReadSingle();
                _ = reader.ReadSingle();
                _ = reader.ReadSingle();
                _ = reader.ReadInt16();
                _ = reader.ReadByte();
                _ = reader.ReadByte();
                var netID = reader.ReadInt16();

                Item DroppedItem = TShock.Utils.GetItemById(netID);
                if (DroppedItem.questItem)
                {
                    for (int i = 0; i <= Terraria.Main.maxItems; i++)
                    {
                        if (i == itemID)
                            continue;
                        Item currItem = Terraria.Main.item[i];
                        if (currItem.Name == DroppedItem.Name && currItem.active)
                        {
                            currItem.active = false;
                            NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, null, i);
                        }
                    }
                }
            }
        }
    }
}
