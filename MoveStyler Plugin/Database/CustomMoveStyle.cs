using Reptile;
using Reptile.Phone;
using System;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Logging;

using MoveStylerMono;

namespace MoveStyler.Data
{
	public class CustomMoveStyle
	{
		private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} CustomMoveStyle");
		public MoveStyleDefinition Definition { get; private set; }
		public SfxCollection Sfx { get; private set; }
		public SfxCollectionID SfxID { get; private set; }

		// Get Custom Animator
		public RuntimeAnimatorController AnimController;

		// Get Custom Animator
		public Animator Anim;

		public int Index;

		public Player.AnimInfo AnimInfoBase;

		public CustomMovementStats custMovementStats;

		public MovementStats movementStats;

		public GameObject Visual
		{
			get
			{
				if (_visual == null)
				{
					CreateVisual();
				}
				return _visual;
			}
		}
		private GameObject _visual;

		public List<KeyValuePair<MeshRenderer, string>> Props;

		#region: Animation Hashes

		// Token: 0x0400139F RID: 5023
		private int canSprayHash;

		// Token: 0x040013A0 RID: 5024
		private int canShakeHash;

		// Token: 0x040013A1 RID: 5025
		private int canStartShakeHash;

		// Token: 0x040013A2 RID: 5026
		private int landRunHash;

		// Token: 0x040013A3 RID: 5027
		private int startRunHash;

		// Token: 0x040013A4 RID: 5028
		private int stopRunHash;

		// Token: 0x040013A5 RID: 5029
		private int walkHash;

		// Token: 0x040013A6 RID: 5030
		private int runHash;

		// Token: 0x040013A7 RID: 5031
		private int fallHash;

		// Token: 0x040013A8 RID: 5032
		private int idleHash;

		// Token: 0x040013A9 RID: 5033
		private int idleFidget1Hash;

		// Token: 0x040013AA RID: 5034
		private int phoneDirectionXHash;

		// Token: 0x040013AB RID: 5035
		private int phoneDirectionYHash;

		// Token: 0x040013AC RID: 5036
		private int turnDirectionXHash;

		// Token: 0x040013AD RID: 5037
		private int turnDirectionX2Hash;

		// Token: 0x040013AE RID: 5038
		private int turnDirectionX3Hash;

		// Token: 0x040013AF RID: 5039
		private int turnDirectionSkateboardHash;
#endregion
		/**
        public GraffitiArt Graffiti { get; private set; }

        private static readonly List<AudioClipID> VOICE_IDS = new List<AudioClipID>()
        {
            AudioClipID.VoiceDie,
            AudioClipID.VoiceDieFall,
            AudioClipID.VoiceTalk,
            AudioClipID.VoiceBoostTrick,
            AudioClipID.VoiceCombo,
            AudioClipID.VoiceGetHit,
            AudioClipID.VoiceJump
        };
        */

		//Constructor
		public CustomMoveStyle(MoveStyleDefinition definition, SfxCollectionID sfxID, int index)
        {
			Definition = definition;
			Index = index;
			SfxID = sfxID;

			//Create Anim Info
			createAnimInfo();         

			//Set the Animator reference
			Anim = Definition.StyleAnimator;
			AnimController = Anim.runtimeAnimatorController;

			movementStats = new MovementStats();

			movementStats.runSpeed				= Definition.runSpeed;
			movementStats.walkSpeed				= Definition.walkSpeed;
			movementStats.groundAcc				= Definition.groundAcc;
			movementStats.groundDecc			= Definition.groundDecc;
			movementStats.airAcc				= Definition.airAcc;
			movementStats.airDecc				= Definition.airDecc;
			movementStats.rotSpeedAtMaxSpeed	= Definition.rotSpeedAtMaxSpeed;
			movementStats.rotSpeedAtStill		= Definition.rotSpeedAtStill;
			movementStats.rotSpeedInAir			= Definition.rotSpeedInAir;
			movementStats.grindSpeed			= Definition.grindSpeed;
			movementStats.slideDeccHighSpeed	= Definition.slideDeccHighSpeed;
			movementStats.slideDeccLowSpeed		= Definition.slideDeccLowSpeed;

			DebugLog.LogMessage($"Created Movestats| Grindspeed: {movementStats.grindSpeed}");

			Props = new List<KeyValuePair<MeshRenderer, string>>();

		}

        private void CreateVisual()
        {
			DebugLog.LogMessage("Create Visual");

			GameObject parent = new GameObject($"{Definition.Movestylename} Visuals");
            MoveStyleDefinition moveStyleModel = UnityEngine.Object.Instantiate(Definition);

            //InitCharacterModel
            moveStyleModel.transform.SetParent(parent.transform, false);

            //InitMeshRendererForProps and attachment
            for (int i = 0; i < moveStyleModel.PropRenderers.Length; i++)
            {
                MeshRenderer renderer = moveStyleModel.PropRenderers[i];
				string bone = moveStyleModel.PropAttachmentBones[i];
				DebugLog.LogMessage("Bone:" + bone);

				renderer.receiveShadows = false;
                renderer.gameObject.layer = 15;
                renderer.gameObject.SetActive(true);
				renderer.gameObject.transform.SetParent(parent.transform);

				Props.Add(new KeyValuePair<MeshRenderer, string>(renderer, bone));
				DebugLog.LogMessage("Renderer: " + renderer.name);

			}

            //InitAnimatorForModel
            //moveStyleModel.GetComponentInChildren<Animator>().applyRootMotion = false;

            //InitCharacterVisuals
            parent.SetActive(false);

            _visual = parent;
        }

		void setPropVisualAttachment(Player player, int Index)
		{
			if (player == null) { return; };

			CharacterVisual charVisual = (CharacterVisual)player.GetField("characterVisual").GetValue(player);

			if (charVisual == null) { return; };
			//Set Transforms on Active Movestyle

			Visual.transform.SetToIdentity();
			Visual.transform.localScale = Vector3.one * 1f;


			switch (Index)
			{
				case 0:
					//ToDo
					foreach (KeyValuePair<MeshRenderer, string> prop in Props)
					{
						if (prop.Key == null) { continue; }
						
						prop.Key.gameObject.SetActive(false);
				
					}
					break;
				case 1:
					foreach (KeyValuePair<MeshRenderer, string> prop in Props)
					{
						if (prop.Key == null) { continue; }

						GameObject obj = prop.Key.gameObject;

						if (obj == null)
						{
							DebugLog.LogMessage("could not find Prop Obj");
							return;
						}

						obj.transform.parent = charVisual.gameObject.transform.FindRecursive(prop.Value); //Attach to bone
						obj.transform.SetToIdentity();
						obj.transform.localScale = Vector3.one * 1f;
						obj.SetActive(true);
					}
					break;

				case 2:
					//ToDo
					foreach (KeyValuePair<MeshRenderer, string> prop in Props)
					{
						if (prop.Key == null) { continue; }

						prop.Key.gameObject.SetActive(false);
					}
					Visual.SetActive(false);
					break;

				default:
					foreach (KeyValuePair<MeshRenderer, string> prop in Props)
					{
						if (prop.Key == null) { continue; }

						prop.Key.gameObject.SetActive(false);
					}
					Visual.SetActive(false);
					break;
			}
		}
	
        private void createAnimInfo()
        {
			//Init Hash Values
			this.canSprayHash = Animator.StringToHash("canSpray");
			this.canShakeHash = Animator.StringToHash("canShake");
			this.canStartShakeHash = Animator.StringToHash("canStartShake");
			this.landRunHash = Animator.StringToHash("landRun");
			this.startRunHash = Animator.StringToHash("startRun");
			this.stopRunHash = Animator.StringToHash("stopRun");
			this.walkHash = Animator.StringToHash("walk");
			this.runHash = Animator.StringToHash("run");
			this.fallHash = Animator.StringToHash("fall");
			this.idleHash = Animator.StringToHash("idle");
			this.idleFidget1Hash = Animator.StringToHash("idleFidget1");
			this.phoneDirectionXHash = Animator.StringToHash("phoneDirectionX");
			this.phoneDirectionYHash = Animator.StringToHash("phoneDirectionY");
			this.turnDirectionXHash = Animator.StringToHash("turnDirectionX");
			this.turnDirectionX2Hash = Animator.StringToHash("turnDirectionX2");
			this.turnDirectionX3Hash = Animator.StringToHash("turnDirectionX3");
			this.turnDirectionSkateboardHash = Animator.StringToHash("turnDirectionSkateboard");

			//Create a custom data class to gather data before Init

			//AnimInfoBase
			//AnimInfoBase.AnimInfo()

		}

        public void setAnimInfo(ref Player _player)
        {
			//Use the definition to do this.
			//Use another Animator as a base.
			//Local Vars

			MoveStyle moveStyle = (MoveStyle)this.Index;
			//Set the players current movestyle
			_player.GetField("moveStyle").SetValue(_player, moveStyle);

			DebugLog.LogMessage(String.Format("movestyle # : {0}", (int)moveStyle));

			int key = Animator.StringToHash("land");
			int key2 = Animator.StringToHash("halfStartRun");
			int key3 = Animator.StringToHash("halfStopRun");
			int key4 = Animator.StringToHash("turnRun");
			Player.AnimInfo animInfo = new Player.AnimInfo(_player, "walk", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			animInfo.fadeFrom.Add(this.runHash, 0.2f);
			animInfo.fadeFrom.Add(this.idleHash, 0.25f);
			animInfo.fadeFrom.Add(key, 0.35f);
			animInfo.fadeFrom.Add(this.stopRunHash, 0.4f);
			animInfo.fadeFrom.Add(this.startRunHash, 0.4f);
			animInfo.fadeFrom.Add(key2, 1f);
			animInfo.fadeFrom.Add(key3, 1f);
			new Player.AnimInfo(_player, "grindTrick0Hold", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "grindTrick1Hold", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "grindTrick2Hold", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "grindTrick3Hold", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			Player.AnimInfo animInfo2 = new Player.AnimInfo(_player, "groundBoostTrick0", Player.AnimType.GROUNDTRICK, "run", 0.75f, 0.95f, 1f);
			animInfo2.skipStartrun = true;
			animInfo2.fadeTo.Add(this.stopRunHash, 0.3f);
			Player.AnimInfo animInfo3 = new Player.AnimInfo(_player, "groundBoostTrick1", Player.AnimType.GROUNDTRICK, "run", 0.75f, 0.95f, 1f);
			animInfo3.skipStartrun = true;
			animInfo3.fadeTo.Add(this.stopRunHash, 0.3f);
			Player.AnimInfo animInfo4 = new Player.AnimInfo(_player, "groundBoostTrick2", Player.AnimType.GROUNDTRICK, "run", 0.75f, 0.95f, 1f);
			animInfo4.skipStartrun = true;
			animInfo4.fadeTo.Add(this.stopRunHash, 0.3f);
			Player.AnimInfo animInfo5 = new Player.AnimInfo(_player, "switchToEquippedMovestyleMoving", Player.AnimType.NORMAL, "run", 0f, 0f, 1f);
			animInfo5.skipStartrun = true;
			animInfo5.fadeTo.Add(this.stopRunHash, 0.3f);
			Player.AnimInfo animInfo6 = new Player.AnimInfo(_player, "switchToEquippedMovestyleStanding", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			animInfo6.fadeTo.Add(this.startRunHash, 0.1f);
			animInfo6.fadeTo.Add(this.idleHash, 0.1f);
			animInfo6.feetIK = true;
			Player.AnimInfo animInfo7 = new Player.AnimInfo(_player, "land", Player.AnimType.LAND, "idle", 0f, 0f, 0.9f);
			animInfo7.fadeFrom.Add(this.walkHash, 0.25f);
			animInfo7.fadeFrom.Add(this.runHash, 0.2f);
			animInfo7.fadeTo.Add(this.idleHash, 0.4f);
			animInfo7.feetIK = true;
			Player.AnimInfo animInfo8 = new Player.AnimInfo(_player, "run", Player.AnimType.RUN, "", 0f, 0f, 1f);
			animInfo8.fadeFrom.Add(this.walkHash, 0.05f);
			animInfo8.fadeFrom.Add(key4, 0.3f);
			animInfo8.fadeFrom.Add(this.stopRunHash, 0.05f);
			animInfo8.skipStartrun = true;
			Player.AnimInfo animInfo9 = new Player.AnimInfo(_player, "startRun", Player.AnimType.RUN, "run", 0f, 0.032f, 1f);
			animInfo9.fadeFrom.Add(this.idleHash, 0.05f);
			animInfo9.fadeFrom.Add(this.walkHash, 0.05f);
			animInfo9.skipStartrun = true;
			int key5 = Animator.StringToHash("jump");
			int key6 = Animator.StringToHash("airTrick0");
			int key7 = Animator.StringToHash("airTrick1");
			int key8 = Animator.StringToHash("airTrick2");
			int key9 = Animator.StringToHash("airTrick3");
			int key10 = Animator.StringToHash("airDash");
			int key11 = Animator.StringToHash("wallRunLeft");
			int key12 = Animator.StringToHash("wallRunRight");
			int key13 = Animator.StringToHash("grindTrick0Hold");
			int key14 = Animator.StringToHash("grindTrick1Hold");
			int key15 = Animator.StringToHash("grindTrick2Hold");
			int key16 = Animator.StringToHash("grindTrick3Hold");
			int key17 = Animator.StringToHash("grindBoostTrick0");
			int key18 = Animator.StringToHash("grindBoostTrick1");
			int key19 = Animator.StringToHash("grindBoostTrick2");
			int key20 = Animator.StringToHash("grindBoostTrick0Hold");
			int key21 = Animator.StringToHash("grindBoostTrick1Hold");
			int key22 = Animator.StringToHash("grindBoostTrick2Hold");
			int key23 = Animator.StringToHash("vaultSlide");
			int key24 = Animator.StringToHash("boostRun");
			int key25 = Animator.StringToHash("groundTrick0");
			int key26 = Animator.StringToHash("groundTrick1");
			int key27 = Animator.StringToHash("groundTrick2");
			int key28 = Animator.StringToHash("groundTrick3");
			int key29 = Animator.StringToHash("groundBoostTrick0");
			int key30 = Animator.StringToHash("groundBoostTrick1");
			int key31 = Animator.StringToHash("groundBoostTrick2");
			int key32 = Animator.StringToHash("grindTrick0");
			int key33 = Animator.StringToHash("grindTrick1");
			int key34 = Animator.StringToHash("grindTrick2");
			int key35 = Animator.StringToHash("grindTrick3");
			int key36 = Animator.StringToHash("slide");
			int key37 = Animator.StringToHash("roll");
			int key38 = Animator.StringToHash("slideToRun");
			int key39 = Animator.StringToHash("boostBrake");
			int key40 = Animator.StringToHash("startBoost");
			int key41 = Animator.StringToHash("switchToEquippedMovestyleGrind");
			int key42 = Animator.StringToHash("switchToEquippedMovestyleMoving");
			Player.AnimInfo animInfo10 = new Player.AnimInfo(_player, "fall", Player.AnimType.NORMAL, "fallIdle", 0f, 0f, 1f);
			animInfo10.fadeFrom.Add(this.runHash, 0.05f);
			animInfo10.fadeFrom.Add(this.walkHash, 0.5f);
			animInfo10.fadeFrom.Add(this.idleHash, 0.5f);
			animInfo10.fadeFrom.Add(this.stopRunHash, 0.5f);
			animInfo10.fadeFrom.Add(this.startRunHash, 0.5f);
			animInfo10.fadeFrom.Add(key5, 0.2f);
			animInfo10.fadeFrom.Add(key6, 0.2f);
			animInfo10.fadeFrom.Add(key7, 0.2f);
			animInfo10.fadeFrom.Add(key8, 0.2f);
			animInfo10.fadeFrom.Add(key9, 0.2f);
			animInfo10.fadeFrom.Add(key10, 0.4f);
			animInfo10.fadeFrom.Add(key11, 0.75f);
			animInfo10.fadeFrom.Add(key12, 0.75f);
			animInfo10.fadeFrom.Add(key13, 0.25f);
			animInfo10.fadeFrom.Add(key14, 0.25f);
			animInfo10.fadeFrom.Add(key15, 0.25f);
			animInfo10.fadeFrom.Add(key16, 0.25f);
			animInfo10.fadeFrom.Add(key17, 0.25f);
			animInfo10.fadeFrom.Add(key18, 0.25f);
			animInfo10.fadeFrom.Add(key19, 0.25f);
			animInfo10.fadeFrom.Add(key20, 0.25f);
			animInfo10.fadeFrom.Add(key21, 0.25f);
			animInfo10.fadeFrom.Add(key22, 0.25f);
			animInfo10.fadeFrom.Add(key, 0.5f);
			animInfo10.fadeFrom.Add(this.landRunHash, 0.5f);
			animInfo10.fadeFrom.Add(key23, 0.4f);
			animInfo10.fadeFrom.Add(key24, 1f);
			animInfo10.fadeFrom.Add(key25, 0.5f);
			animInfo10.fadeFrom.Add(key26, 0.5f);
			animInfo10.fadeFrom.Add(key27, 0.5f);
			animInfo10.fadeFrom.Add(key28, 0.5f);
			animInfo10.fadeFrom.Add(key29, 0.5f);
			animInfo10.fadeFrom.Add(key30, 0.5f);
			animInfo10.fadeFrom.Add(key31, 0.5f);
			animInfo10.fadeFrom.Add(key32, 0.5f);
			animInfo10.fadeFrom.Add(key33, 0.5f);
			animInfo10.fadeFrom.Add(key34, 0.5f);
			animInfo10.fadeFrom.Add(key35, 0.5f);
			animInfo10.fadeFrom.Add(key36, 0.35f);
			animInfo10.fadeFrom.Add(key37, 0.4f);
			animInfo10.fadeFrom.Add(key38, 0.4f);
			animInfo10.fadeFrom.Add(key39, 0.2f);
			animInfo10.fadeFrom.Add(key40, 0.4f);
			animInfo10.fadeFrom.Add(key41, 0.5f);
			animInfo10.fadeFrom.Add(key42, 0.5f);
			Player.AnimInfo animInfo11 = new Player.AnimInfo(_player, "grindTrick0", Player.AnimType.NORMAL, "grindTrick0Hold", 0.5f, 0f, 0.75f);
			animInfo11.fadeTo.Add(key32, 0.15f);
			animInfo11.fadeTo.Add(key33, 0.15f);
			animInfo11.fadeTo.Add(key34, 0.15f);
			animInfo11.fadeFrom.Add(key13, 0.1f);
			animInfo11.fadeFrom.Add(key14, 0.1f);
			animInfo11.fadeFrom.Add(key15, 0.1f);
			animInfo11.fadeFrom.Add(key16, 0.1f);
			animInfo11.fadeTo.Add(key13, 0.2f);
			Player.AnimInfo animInfo12 = new Player.AnimInfo(_player, "grindTrick1", Player.AnimType.NORMAL, "grindTrick1Hold", 0.5f, 0f, 0.75f);
			animInfo12.fadeTo.Add(key32, 0.15f);
			animInfo12.fadeTo.Add(key33, 0.15f);
			animInfo12.fadeTo.Add(key34, 0.15f);
			animInfo12.fadeFrom.Add(key13, 0.1f);
			animInfo12.fadeFrom.Add(key14, 0.1f);
			animInfo12.fadeFrom.Add(key15, 0.1f);
			animInfo12.fadeFrom.Add(key16, 0.1f);
			animInfo12.fadeTo.Add(key14, 0.2f);
			Player.AnimInfo animInfo13 = new Player.AnimInfo(_player, "grindTrick2", Player.AnimType.NORMAL, "grindTrick2Hold", 0.5f, 0f, 0.75f);
			animInfo13.fadeTo.Add(key32, 0.15f);
			animInfo13.fadeTo.Add(key33, 0.15f);
			animInfo13.fadeTo.Add(key34, 0.15f);
			animInfo13.fadeFrom.Add(key13, 0.1f);
			animInfo13.fadeFrom.Add(key14, 0.1f);
			animInfo13.fadeFrom.Add(key15, 0.1f);
			animInfo13.fadeFrom.Add(key16, 0.1f);
			animInfo13.fadeTo.Add(key15, 0.2f);
			Player.AnimInfo animInfo14 = new Player.AnimInfo(_player, "grindTrick3", Player.AnimType.NORMAL, "grindTrick3Hold", 0.5f, 0f, 0.75f);
			animInfo14.fadeTo.Add(key32, 0.15f);
			animInfo14.fadeTo.Add(key33, 0.15f);
			animInfo14.fadeTo.Add(key34, 0.15f);
			animInfo14.fadeFrom.Add(key13, 0.1f);
			animInfo14.fadeFrom.Add(key14, 0.1f);
			animInfo14.fadeFrom.Add(key15, 0.1f);
			animInfo14.fadeFrom.Add(key16, 0.1f);
			animInfo14.fadeTo.Add(key16, 0.2f);
			new Player.AnimInfo(_player, "switchToEquippedMovestyleGrind", Player.AnimType.NORMAL, "grindTrick1Hold", 0.5f, 0f, 0.75f).fadeTo.Add(key14, 0.2f);
			Player.AnimInfo animInfo15 = new Player.AnimInfo(_player, "groundTrick0", Player.AnimType.GROUNDTRICK, "run", 0f, 0f, 1f);
			animInfo15.skipStartrun = true;
			animInfo15.fadeTo.Add(this.stopRunHash, 0.3f);
			animInfo15.fadeTo.Add(key25, 0.1f);
			animInfo15.fadeTo.Add(key26, 0.1f);
			animInfo15.fadeTo.Add(key27, 0.1f);
			animInfo15.fadeFrom.Add(this.runHash, 0.01f);
			Player.AnimInfo animInfo16 = new Player.AnimInfo(_player, "groundTrick1", Player.AnimType.GROUNDTRICK, "run", 0f, 0f, 1f);
			animInfo16.skipStartrun = true;
			animInfo16.fadeTo.Add(this.stopRunHash, 0.3f);
			animInfo16.fadeTo.Add(key25, 0.1f);
			animInfo16.fadeTo.Add(key26, 0.1f);
			animInfo16.fadeTo.Add(key27, 0.1f);
			Player.AnimInfo animInfo17 = new Player.AnimInfo(_player, "groundTrick2", Player.AnimType.GROUNDTRICK, "run", 0f, 0f, 1f);
			animInfo17.skipStartrun = true;
			animInfo17.fadeTo.Add(this.stopRunHash, 0.3f);
			animInfo17.fadeTo.Add(key25, 0.1f);
			animInfo17.fadeTo.Add(key26, 0.1f);
			animInfo17.fadeTo.Add(key27, 0.1f);
			Player.AnimInfo animInfo18 = new Player.AnimInfo(_player, "groundTrick3", Player.AnimType.GROUNDTRICK, "run", 0f, 0f, 1f);
			animInfo18.skipStartrun = true;
			animInfo18.fadeTo.Add(this.stopRunHash, 0.3f);
			animInfo18.fadeTo.Add(key25, 0.1f);
			animInfo18.fadeTo.Add(key26, 0.1f);
			animInfo18.fadeTo.Add(key27, 0.1f);
			new Player.AnimInfo(_player, "grindBoostTrick0", Player.AnimType.NORMAL, "grindBoostTrick0Hold", 0.5f, 0f, 0.75f).fadeTo.Add(key20, 0.2f);
			new Player.AnimInfo(_player, "grindBoostTrick1", Player.AnimType.NORMAL, "grindBoostTrick1Hold", 0.5f, 0f, 0.75f).fadeTo.Add(key21, 0.2f);
			new Player.AnimInfo(_player, "grindBoostTrick2", Player.AnimType.NORMAL, "grindBoostTrick2Hold", 0.5f, 0f, 0.75f).fadeTo.Add(key22, 0.2f);
			Player.AnimInfo animInfo19 = new Player.AnimInfo(_player, "grindRetourEnd", Player.AnimType.NORMAL, "grindTrick3Hold", 0.5f, 0f, 0.75f);
			animInfo19.fadeTo.Add(key32, 0.15f);
			animInfo19.fadeTo.Add(key33, 0.15f);
			animInfo19.fadeTo.Add(key34, 0.15f);
			animInfo19.fadeTo.Add(key16, 0.2f);
			int key43 = Animator.StringToHash("fallIdle");
			Animator.StringToHash("poleFreezeIdle");
			int key44 = Animator.StringToHash("idleFidget2");
			int key45 = Animator.StringToHash("dance1");
			new Player.AnimInfo(_player, "fallIdle", Player.AnimType.NORMAL, "", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "poleFlip", Player.AnimType.NORMAL, "fall", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "hitBounce", Player.AnimType.NORMAL, "fallIdle", -1f, 0f, 1f).fadeTo.Add(key43, 0.75f);
			new Player.AnimInfo(_player, "poleFreeze", Player.AnimType.NORMAL, "poleFreezeIdle", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "jumpTrick1", Player.AnimType.NORMAL, "fallIdle", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "airTrick0", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "airTrick1", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "airTrick2", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "airTrick3", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "airBoostTrick0", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "airBoostTrick1", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			new Player.AnimInfo(_player, "airBoostTrick2", Player.AnimType.AIRTRICK, "fallIdle", -1f, 0f, 1f);
			Player.AnimInfo animInfo20 = new Player.AnimInfo(_player, "landRun", Player.AnimType.LAND, "run", 0f, 0f, 1f);
			animInfo20.fadeFrom.Add(this.walkHash, 0.25f);
			animInfo20.skipStartrun = true;
			new Player.AnimInfo(_player, "turnRun", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			Player.AnimInfo animInfo21 = new Player.AnimInfo(_player, "stopRun", Player.AnimType.STOPRUN, "idle", 0f, 0f, 1f);
			animInfo21.fadeFrom.Add(key23, 0.25f);
			animInfo21.fadeFrom.Add(this.idleHash, 0.25f);
			animInfo21.fadeFrom.Add(key2, 0.2f);
			animInfo21.fadeFrom.Add(this.runHash, 0.04f);
			animInfo21.fadeFrom.Add(this.startRunHash, 0.2f);
			animInfo21.fadeFrom.Add(key38, 0.5f);
			animInfo21.fadeFrom.Add(this.landRunHash, 0.5f);
			animInfo21.feetIK = true;
			animInfo21.skipStartrun = true;
			Player.AnimInfo animInfo22 = new Player.AnimInfo(_player, "halfStopRun", Player.AnimType.STOPRUN, "idle", 0f, 0f, 1f);
			animInfo22.fadeFrom.Add(this.idleHash, 0.25f);
			animInfo22.fadeFrom.Add(key2, 0.2f);
			animInfo22.fadeFrom.Add(this.startRunHash, 0.1f);
			animInfo22.feetIK = true;
			Player.AnimInfo animInfo23 = new Player.AnimInfo(_player, "boostBrake", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			animInfo23.fadeTo.Add(this.startRunHash, 0.1f);
			animInfo23.feetIK = true;
			new Player.AnimInfo(_player, "sit", Player.AnimType.NORMAL, "", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "startSit", Player.AnimType.NORMAL, "sit", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "stopSit", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			Player.AnimInfo animInfo24 = new Player.AnimInfo(_player, "idle", Player.AnimType.IDLE, "", 0f, 0f, 1f);
			animInfo24.fadeFrom.Add(this.walkHash, 0.15f);
			animInfo24.fadeFrom.Add(key39, 0.3f);
			animInfo24.fadeFrom.Add(this.landRunHash, 0.2f);
			animInfo24.fadeFrom.Add(this.startRunHash, 0.5f);
			animInfo24.fadeFrom.Add(this.stopRunHash, 1f);
			animInfo24.feetIK = true;
			animInfo24.fadeTo.Add(this.idleFidget1Hash, 0.2f);
			animInfo24.fadeTo.Add(key44, 0.2f);
			Player.AnimInfo animInfo25 = new Player.AnimInfo(_player, "idleFidget1", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			animInfo25.fadeFrom.Add(this.idleHash, 0.1f);
			animInfo25.feetIK = true;
			animInfo25.fadeTo.Add(this.idleHash, 0.2f);
			Player.AnimInfo animInfo26 = new Player.AnimInfo(_player, "idleFidget2", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			animInfo26.fadeFrom.Add(this.idleHash, 0.1f);
			animInfo26.feetIK = true;
			animInfo26.fadeTo.Add(this.idleHash, 0.2f);
			new Player.AnimInfo(_player, "airDash", Player.AnimType.NORMAL, "fall", 0f, 0f, 1f);
			Player.AnimInfo animInfo27 = new Player.AnimInfo(_player, "roll", Player.AnimType.NORMAL, "slide", 0f, 0f, 1f);
			animInfo27.fadeFrom.Add(key25, 0.2f);
			animInfo27.fadeFrom.Add(key26, 0.2f);
			animInfo27.fadeFrom.Add(key27, 0.2f);
			animInfo27.fadeFrom.Add(key29, 0.14f);
			animInfo27.fadeFrom.Add(key30, 0.14f);
			animInfo27.fadeFrom.Add(key31, 0.14f);
			animInfo27.fadeFrom.Add(this.idleHash, 0.1f);
			animInfo27.fadeFrom.Add(this.runHash, 0.025f);
			new Player.AnimInfo(_player, "slide", Player.AnimType.NORMAL, "", 0f, 0f, 1f).fadeFrom.Add(key37, 0.3f);
			new Player.AnimInfo(_player, "slideToRun", Player.AnimType.NORMAL, "run", 0f, 0f, 1f).skipStartrun = true;
			new Player.AnimInfo(_player, "vault", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			Player.AnimInfo animInfo28 = new Player.AnimInfo(_player, "vaultSlide", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			animInfo28.fadeTo.Add(this.runHash, 0.04f);
			animInfo28.skipStartrun = true;
			Player.AnimInfo animInfo29 = new Player.AnimInfo(_player, "halfStartRun", Player.AnimType.NORMAL, "run", 0f, 0f, 1f);
			animInfo29.fadeFrom.Add(this.stopRunHash, 0.2f);
			animInfo29.fadeFrom.Add(key3, 0.2f);
			animInfo29.skipStartrun = true;
			new Player.AnimInfo(_player, "dance1", Player.AnimType.OVERRULE_IDLE, "", 0f, 0f, 1f).fadeFrom.Add(this.idleHash, 0.15f);
			new Player.AnimInfo(_player, "dance2", Player.AnimType.OVERRULE_IDLE, "", 0f, 0f, 1f).fadeFrom.Add(key45, 0.1f);
			new Player.AnimInfo(_player, "dance3", Player.AnimType.OVERRULE_IDLE, "", 0f, 0f, 1f).fadeFrom.Add(key45, 0.1f);
			new Player.AnimInfo(_player, "dance4", Player.AnimType.OVERRULE_IDLE, "", 0f, 0f, 1f).fadeFrom.Add(key45, 0.1f);
			new Player.AnimInfo(_player, "headSpinRStart", Player.AnimType.NORMAL, "headSpinR", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "headSpinRStop", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "headSpinLStart", Player.AnimType.NORMAL, "headSpinL", 0f, 0f, 1f);
			new Player.AnimInfo(_player, "headSpinLStop", Player.AnimType.NORMAL, "idle", 0f, 0f, 1f);
		}

        //Set Movement Stats for Custom MoveStyles
        public void setCustomMovementStats(Player player)
        { 
            MovementStats stats = (MovementStats)player.GetField("stats").GetValue(player);
			stats = movementStats;

			player.GetField("stats").SetValue(player, stats);
		}

        //Set the props mode for custom MoveStyles
        public void setCustomMoveStylePropsMode(Player player , int Index)
        {
			/*
            ON_BACK = 0,
            ACTIVE = 1,
            OFF = 2
            */
			
			//Get Character Visual
			CharacterVisual charVisual = (CharacterVisual)player.GetField("characterVisual").GetValue(player);

			setPropVisualAttachment(player, Index);

			/*
			switch (Index)
            {
                case 0:
					DebugLog.LogMessage("Set CustomMoveStyle on Back");
					//Add the ability to use this
					Visual.SetActive(false);
                    //Do On Back Stuff
                    break;

                case 1:
                    DebugLog.LogMessage("Set CustomMoveStyle Active");
                    
					//Do attach to Bone
					break;

                case 2:
					DebugLog.LogMessage("Set CustomMoveStyle Off");
					Visual.SetActive(false);
                    break;
                
                default:
                    Visual.SetActive(false); 
                    break;
            } 
			*/
		}


		/*
        private void CreateSfxCollection()
        {
            if (!Definition.HasVoices())
            {
                return;
            }

            SfxCollection newCollection = ScriptableObject.CreateInstance<SfxCollection>();

            newCollection.audioClipContainers = new SfxCollection.RandomAudioClipContainer[VOICE_IDS.Count];
            for (int i = 0; i < VOICE_IDS.Count; i++)
            {
                newCollection.audioClipContainers[i] = new SfxCollection.RandomAudioClipContainer();
                newCollection.audioClipContainers[i].clipID = VOICE_IDS[i];
                newCollection.audioClipContainers[i].clips = null;
                newCollection.audioClipContainers[i].lastRandomClip = 0;
            }

            foreach (SfxCollection.RandomAudioClipContainer originalContainer in newCollection.audioClipContainers)
            {
                switch (originalContainer.clipID)
                {
                    case AudioClipID.VoiceDie:
                        if (Definition.VoiceDie.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceDie;
                        }
                        break;
                    case AudioClipID.VoiceDieFall:
                        if (Definition.VoiceDieFall.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceDieFall;
                        }
                        break;
                    case AudioClipID.VoiceTalk:
                        if (Definition.VoiceTalk.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceTalk;
                        }
                        break;
                    case AudioClipID.VoiceBoostTrick:
                        if (Definition.VoiceBoostTrick.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceBoostTrick;
                        }
                        break;
                    case AudioClipID.VoiceCombo:
                        if (Definition.VoiceCombo.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceCombo;
                        }
                        break;
                    case AudioClipID.VoiceGetHit:
                        if (Definition.VoiceGetHit.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceGetHit;
                        }
                        break;
                    case AudioClipID.VoiceJump:
                        if (Definition.VoiceJump.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceJump;
                        }
                        break;
                }
            }

            Sfx = newCollection;
        }

        public void ApplySfxCollection(SfxCollection collection)
        {
            if (Sfx == null)
            {
                Sfx = collection;
                return;
            }
            else
            {
                foreach (SfxCollection.RandomAudioClipContainer container in collection.audioClipContainers)
                {
                    //Add any missing entries
                    if (!VOICE_IDS.Contains(container.clipID))
                    {
                        Array.Resize(ref Sfx.audioClipContainers, Sfx.audioClipContainers.Length + 1);
                        Sfx.audioClipContainers[Sfx.audioClipContainers.Length - 1] = container;
                    }
                }
            }
        }
        public void ApplyShaderToOutfits(Shader shader)
        {
            foreach (CharacterOutfit outfit in Definition.Outfits)
            {
                foreach (CharacterOutfitRenderer container in outfit.MaterialContainers)
                {
                    for (int i = 0; i < container.Materials.Length; i++)
                    {
                        if (container.UseShaderForMaterial[i])
                        {
                            container.Materials[i].shader = shader;
                        }
                    }
                }
            }
        }
        */
	}
    }
