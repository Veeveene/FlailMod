using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlailMod.Common
{
    public class FlailModGlobalProjectile : GlobalProjectile
    {
        private bool IsFlail = false;

        private static bool IsMace(Projectile projectile) => projectile.type == ProjectileID.Mace || projectile.type == ProjectileID.FlamingMace;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.aiStyle == ProjAIStyleID.Flail)
            {
                IsFlail = true;
            }
		}
		public override bool PreAI(Projectile projectile) {
			if (IsFlail) {
				Player player = Main.player[projectile.owner];
				FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();
				modPlayer.DisplayFlailBar = 60;

				if (CurrentAIState(projectile) == AIState.Spinning) {
					modPlayer.FLailCharge += 1f * modPlayer.FlailChargeSpeed;
					if (modPlayer.FLailCharge > 300f)
						modPlayer.FLailCharge = 300f;
				}
			}
			return true;
		}
		public override void FlailStats(Projectile projectile, ref int launchTimeLimit, ref float launchSpeed, ref float maxLaunchLength, ref float retractAcceleration, ref float maxRetractSpeed, ref float forcedRetractAcceleration, ref float maxForcedRetractSpeed, ref int ricochetTimeLimit, ref float spinVisualDistance) {
			if (!IsMace(projectile)) spinVisualDistance += Main.player[projectile.owner].GetModPlayer<FlailPlayer>().FLailCharge * 0.15f;
		}
		public override void FlailSpinCollisionRange(Projectile projectile, ref float range) {
			if (IsMace(projectile)) return;
			range += Main.player[projectile.owner].GetModPlayer<FlailPlayer>().FLailCharge * 0.15f;
			if (projectile.type == ProjectileID.Anchor) range += 16;
		}

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (IsFlail)
            {
                Player player = Main.player[projectile.owner];
                FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();

                switch (CurrentAIState(projectile))
                {
                    case AIState.Spinning:
                        modPlayer.FLailCharge += 20f * modPlayer.FlailChargeSpeed;
                        if (modPlayer.FLailCharge > 300f)
                            modPlayer.FLailCharge = 300f;
                        modifiers.FinalDamage *= 1f * (1f + modPlayer.FLailCharge / 300f * (modPlayer.FlailChargeDamage - 1f));
                        break;
                    case AIState.LaunchingForward:
                        modifiers.FinalDamage *= 0.5f * (1f + modPlayer.FLailCharge * 0.01f * modPlayer.FlailChargeDamage);
                        break;
                    default:
                        modifiers.FinalDamage *= 0.5f * (1f + modPlayer.FLailCharge * 0.005f * modPlayer.FlailChargeDamage);
                        break;
                }
            }
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (IsFlail)
            {
                Player player = Main.player[projectile.owner];
                FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();
                if (modPlayer.FLailCharge >= 300f && modPlayer.FlailScarab) modPlayer.TryHeal(10);
                modPlayer.FLailCharge = 0f;
            }
        }

		public enum AIState
        {
            Spinning,
            LaunchingForward,
            Retracting,
            UnusedState,
            ForcedRetracting,
            Ricochet,
            Dropping
        }

        public static AIState CurrentAIState(Projectile projectile) => (AIState)projectile.ai[0];
    }
}