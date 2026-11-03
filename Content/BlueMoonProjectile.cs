using FlailMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlailMod.Content; 
public class BlueMoonProjectile : GlobalProjectile {
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.BlueMoon;
	public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
		switch (FlailModGlobalProjectile.CurrentAIState(projectile)) {
			case FlailModGlobalProjectile.AIState.LaunchingForward:
			case FlailModGlobalProjectile.AIState.Retracting:
			if (Main.player[projectile.owner].GetModPlayer<FlailPlayer>().FLailCharge == 300f) 				modifiers.SetCrit();
			break;
		}
	}
}
