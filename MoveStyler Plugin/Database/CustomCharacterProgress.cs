using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;


namespace MoveStyler
{
	class CustomCharacterProgress : CharacterProgress
	{
		public CustomCharacterProgress()
		{
		}

		public CustomCharacterProgress(Characters setCharacter)
		{
			this.character = setCharacter;
			switch (this.character)
			{
				case Characters.girl1:
					this.unlocked = true;
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.frank:
					this.moveStyle = MoveStyle.BMX;
					return;
				case Characters.ringdude:
					this.moveStyle = MoveStyle.BMX;
					return;
				case Characters.metalHead:
					this.unlocked = true;
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.blockGuy:
					this.unlocked = true;
					this.moveStyle = MoveStyle.BMX;
					return;
				case Characters.spaceGirl:
					this.unlocked = true;
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.angel:
					this.moveStyle = MoveStyle.BMX;
					return;
				case Characters.eightBall:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.dummy:
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.dj:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.medusa:
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.boarder:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.headMan:
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.prince:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.jetpackBossPlayer:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.legendFace:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.oldheadPlayer:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.robot:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.skate:
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.wideKid:
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.futureGirl:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				case Characters.pufferGirl:
					this.moveStyle = MoveStyle.INLINE;
					return;
				case Characters.bunGirl:
					this.moveStyle = MoveStyle.BMX;
					return;
				case Characters.headManNoJetpack:
					break;
				case Characters.eightBallBoss:
					this.moveStyle = MoveStyle.SKATEBOARD;
					break;
				case Characters.legendMetalHead:
					this.moveStyle = MoveStyle.SKATEBOARD;
					return;
				default:
					return;
			}
		}
	}
}
