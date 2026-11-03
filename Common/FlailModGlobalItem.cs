using FlailMod.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlailMod.Common
{
	public class FlailModGlobalItem : GlobalItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ToolTipDamageMultiplier[ItemID.Anchor] = 2f;
			ItemID.Sets.ToolTipDamageMultiplier[ItemID.Flairon] = 2f;
		}

		public override void SetDefaults(Item item)
		{
			switch (item.type) {
				case ItemID.Anchor:
				item.channel = true;
				item.shoot = ModContent.ProjectileType<AnchorProjectile>();
				item.damage = 110;
				break;
				case ItemID.Flairon:
				item.channel = true;
				item.shoot = ModContent.ProjectileType<FlaironProjectile>();
				break;
			}
		}
	}
}