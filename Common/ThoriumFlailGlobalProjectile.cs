using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static FlailMod.Common.FlailModGlobalProjectile;

namespace FlailMod.Common {
	public class ThoriumFlailGlobalProjectile : GlobalProjectile {
		private bool IsFlail = false;

		public override bool InstancePerEntity => true;

		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			if (FailProjectileTypes.Contains(projectile.type)) {
				IsFlail = true;
			}
		}

		public override void AI(Projectile projectile) {
			if (IsFlail) {
				Player player = Main.player[projectile.owner];
				FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();
				modPlayer.DisplayFlailBar = 60;

				if (CurrentAIState(projectile) == AIState.Spinning) {
					modPlayer.FLailCharge += 1f * modPlayer.FlailChargeSpeed;
					if (modPlayer.FLailCharge > 300f)
						modPlayer.FLailCharge = 300f;

					Vector2 direction = Vector2.Normalize(projectile.Center - player.Center);
					float distance = 0;//modPlayer.FLailCharge * 0.15f
					int launchTimeLimit = 0, ricochetTimeLimit = 0;
					float launchSpeed = 0, maxLaunchLength = 0, retractAcceleration = 0, maxRetractSpeed = 0, forcedRetractAcceleration = 0, maxForcedRetractSpeed = 0;
					ProjectileLoader.FlailStats(projectile, ref launchTimeLimit, ref launchSpeed, ref maxLaunchLength, ref retractAcceleration, ref maxRetractSpeed, ref forcedRetractAcceleration, ref maxForcedRetractSpeed, ref ricochetTimeLimit, ref distance);
					projectile.position += direction * distance;
				}
			}
		}

		public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox) {
			bool? value = base.Colliding(projectile, projHitbox, targetHitbox);
			if (value == true) return true;
			else if (IsFlail) {
				Player player = Main.player[projectile.owner];
				FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();
				if (CurrentAIState(projectile) == AIState.Spinning) {
					Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
					Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
					shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
					float hitRadius = 55f;// + modPlayer.FLailCharge * 0.15f + (projectile.type == ModContent.ProjectileType<Content.AnchorProjectile>() ? 16f : 0f); // The length of the semi-major radius of the ellipse (the long end)
					ProjectileLoader.FlailSpinCollisionRange(projectile, ref hitRadius);
					if (projectile.type == ModContent.ProjectileType<Content.AnchorProjectile>()) hitRadius += 16f;
					return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
				}
			}
			return value;
		}

		public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
			if (IsFlail) {
				Player player = Main.player[projectile.owner];
				FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();

				switch (CurrentAIState(projectile)) {
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

		public override void OnKill(Projectile projectile, int timeLeft) {
			if (IsFlail) {
				Player player = Main.player[projectile.owner];
				FlailPlayer modPlayer = player.GetModPlayer<FlailPlayer>();
				if (modPlayer.FLailCharge >= 300f && modPlayer.FlailScarab) modPlayer.TryHeal(10);
				modPlayer.FLailCharge = 0f;
			}
		}

		public static HashSet<int> FailProjectileTypes;
	}
}