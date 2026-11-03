using FlailMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using rail;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlailMod.Content
{
	public class DripplerCripplerProjectile : ILoadable {
		void ILoadable.Load(Mod mod) {
			IL_Projectile.AI_015_Flails += _IL_Projectile_AI_015_Flails;
		}

		static void _IL_Projectile_AI_015_Flails(ILContext il) {
			ILCursor c = new(il);
			ILLabel label = default;
			while (c.TryGotoNext(MoveType.After,
				i => i.MatchLdfld<Projectile>(nameof(Projectile.type)),
				i => i.MatchLdcI4(ProjectileID.DripplerFlail),
				i => i.MatchBneUn(out label)
			)) {
				ILCursor check = new(c);
				if (!check.TryGotoNext(MoveType.Before, i => i.MatchCall<Projectile>(nameof(Projectile.NewProjectile))) || check.Index > label.Target.Offset) continue;
				c.EmitLdarg0();
				c.EmitDelegate(ShootBall);
				c.EmitBr(label);
			}
		}
		static void ShootBall(Projectile projectile) {
			Player player = Main.player[projectile.owner];
			FlailPlayer flailPlayer = player.GetModPlayer<FlailPlayer>();
			if (flailPlayer.FLailCharge >= 300f) {
				Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, projectile.velocity, ProjectileID.DripplerFlailExtraBall, projectile.damage * 3, projectile.knockBack, projectile.owner);
			}
		}
		void ILoadable.Unload() { }
	}
}
