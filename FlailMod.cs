using FlailMod.Common;
using FlailMod.Content;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlailMod
{
	public class FlailMod : Mod
    {
        public static Mod ThoriumMod;

        public override void Load()
        {
            ThoriumMod = ModLoader.TryGetMod("ThoriumMod", out Mod mod) ? mod : null;
        }

        public override void PostSetupContent()
        {
            HashSet<int> list = [];

            list.Add(ModContent.ProjectileType<AnchorProjectile>());
            list.Add(ModContent.ProjectileType<FlaironProjectile>());

            if (ThoriumMod != null)
            {
                Span<string> TypesToTry = [
                    "TheSnowballPro",
                    "SparkingJellyBallPro",
                    "StarTrailPro",
                    "BoneFlayerTailPro",
                    "FleshMacePro",
                    "EbonyTailPro",
                    "TheSeaMinePro",
                    "TheJuggernautPro",
                    "LodeStoneBreakerPro",
                    "SteamFlailPro",
                    "LivewireCrasherPro"
                ];

                foreach (string str in TypesToTry)
                {
                    if (ThoriumMod.TryFind(str, out ModProjectile thoriumProjectile))
                    {
                        list.Add(thoriumProjectile.Type);
                    }
                }

                ThoriumMod.Call("AddFlailProjectileID", ModContent.ProjectileType<AnchorProjectile>());
                ThoriumMod.Call("AddFlailProjectileID", ModContent.ProjectileType<FlaironProjectile>());
            }

			ThoriumFlailGlobalProjectile.FailProjectileTypes = list;
        }

        public override void Unload()
        {
			ThoriumFlailGlobalProjectile.FailProjectileTypes = null;
        }
    }
}