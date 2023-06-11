using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API

namespace AimsharpWow.Modules {
	public class VidArmsWarrior : Rotation {

		public override void LoadSettings() {
			

			List<string> Trinkets = new List<string>(new string[] {
                "Generic",
				"None"
			});
			Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
			Settings.Add(new Setting("Top Trinket AOE?", false));
			Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
			Settings.Add(new Setting("Bot Trinket AOE?", false));
			Settings.Add(new Setting("Use item: Case Sens", "Healthstone"));
			Settings.Add(new Setting("Use item @ HP%", 0, 100, 25));
			Settings.Add(new Setting("First 5 Letters of the Addon:", "xxxxx"));
			List<string> Race = new List<string>(new string[] {
				"Orc",
				"Troll",
				"Dark Iron Dwarf",
				"Mag'har Orc",
				"Lightforged Draenei",
				"None"
			});
			Settings.Add(new Setting("Racial Power", Race, "None"));
			Settings.Add(new Setting("Test of Might Trait?", true));
			Settings.Add(new Setting("Mitigation"));
			Settings.Add(new Setting("Auto Victory Rush @ HP%", 0, 100, 60));
			Settings.Add(new Setting("Auto Shout @ HP%", 0, 100, 15));
			Settings.Add(new Setting("Auto Die by the Sword @ HP%", 0, 100, 0));
			Settings.Add(new Setting("Auto Stance @ HP%", 0, 100, 30));
			Settings.Add(new Setting("Auto Ignore Pain @ HP%", 0, 100, 10));
			Settings.Add(new Setting("Auto Healthstone @ HP%", 0, 100, 25));
			Settings.Add(new Setting("Unstance @ HP%", 0, 100, 70));
            Settings.Add(new Setting(""));
        }

		string MajorPower;
		string TopTrinket;
		string BotTrinket;
		string RacialPower;
		string usableitems;
		string FiveLetters;

		public override void Initialize() {
			Aimsharp.DebugMode();

			Aimsharp.PrintMessage("Vid Arms Warrior 1.04", Color.Yellow);
			Aimsharp.PrintMessage("Recommended PVE talents: 3323311", Color.Yellow);
			Aimsharp.PrintMessage("Recommended PVP talents: 2333211", Color.Yellow);
			Aimsharp.PrintMessage("Default mode: PVE, AOE ON, USE CDs/Pots", Color.Yellow);
			Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Yellow);
			Aimsharp.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
			Aimsharp.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
			Aimsharp.PrintMessage("/xxxxx noaoe --Toggles to turn off PVE AOE on/off.", Color.Orange);
			Aimsharp.PrintMessage("/xxxxx savepp -- Toggles the use of prepull", Color.Orange);
			Aimsharp.PrintMessage("/xxxxx StormBolt --Queues Storm Bolt up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx RallyingCry --Queues Rallying Cry up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx IntimidatingShout --Queues Intimidating SHout up to be used on the next GCD.",
				Color.Red);
			Aimsharp.PrintMessage("/xxxxx Bladestorm --Queues Bladestorm up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx DefensiveStance --Toggles on Defensive Stance", Color.Red);
			Aimsharp.PrintMessage("/xxxxx pvp --Toggles PVP Mode.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx Burst --Toggles Burst for pvp on.", Color.Red);

			Aimsharp.Latency = 100;
			Aimsharp.QuickDelay = 125;

			
			TopTrinket = GetDropDown("Top Trinket");
			BotTrinket = GetDropDown("Bot Trinket");
			RacialPower = GetDropDown("Racial Power");
			usableitems = GetString("Use item: Case Sens");
			FiveLetters = GetString("First 5 Letters of the Addon:");

			#region Spellbook

			

			if (RacialPower == "Orc") Spellbook.Add("Blood Fury");
			if (RacialPower == "Troll") Spellbook.Add("Berserking");
			if (RacialPower == "Dark Iron Dwarf") Spellbook.Add("Fireblood");
			if (RacialPower == "Mag'har Orc") Spellbook.Add("Ancestral Call");
			if (RacialPower == "Lightforged Draenei") Spellbook.Add("Light's Judgment");

			Spellbook.Add("Skullsplitter");
			Spellbook.Add("Colossus Smash");
			Spellbook.Add("Bladestorm");
			Spellbook.Add("Overpower");
			Spellbook.Add("Execute");
			Spellbook.Add("Mortal Strike");
			Spellbook.Add("Whirlwind");
			Spellbook.Add("Cleave");
			Spellbook.Add("Sweeping Strikes");
			Spellbook.Add("Overpower");
			Spellbook.Add("Victory Rush");
			Spellbook.Add("Die by the Sword");
			Spellbook.Add("Rend");
			Spellbook.Add("Avatar");
			Spellbook.Add("Warbreaker");
			Spellbook.Add("Battle Shout");
			Spellbook.Add("Sharpen Blade");
			Spellbook.Add("Slam");
			Spellbook.Add("Heroic Throw");
			Spellbook.Add("Hamstring");
			Spellbook.Add("Rallying Cry");
			Spellbook.Add("Defensive Stance");
			Spellbook.Add("Storm Bolt");
			Spellbook.Add("Intimidating Shout");
			Spellbook.Add("Deadly Calm");
			Spellbook.Add("Ignore Pain");

			Buffs.Add("Bloodlust");
			Buffs.Add("Heroism");
			Buffs.Add("Time Warp");
			Buffs.Add("Ancient Hysteria");
			Buffs.Add("Netherwinds");
			Buffs.Add("Drums of Rage");
			Buffs.Add("Lifeblood");
			Buffs.Add("Memory of Lucid Dreams");
			Buffs.Add("Reckless Force");
			Buffs.Add("Guardian of Azeroth");
			Buffs.Add("Crushing Assault");

			Buffs.Add("Berserker Rage");
			Buffs.Add("Meat Cleaver");
			Buffs.Add("Enrage");
			Buffs.Add("Furious Slash");
			Buffs.Add("Whirlwind");
			Buffs.Add("Test of Might");
			Buffs.Add("Avatar");
			Buffs.Add("Sharpen Blade");
			Buffs.Add("Battle Shout");
			Buffs.Add("Overpower");
			Buffs.Add("Bladestorm");
			Buffs.Add("Defensive Stance");
			Buffs.Add("Sweeping Strikes");
			Buffs.Add("Deadly Calm");
			Buffs.Add("Executioner's Precision");

			Debuffs.Add("Razor Coral");
			Debuffs.Add("Conductive Ink");
			Debuffs.Add("Shiver Venom");
			Debuffs.Add("Siegebreaker");
			Debuffs.Add("Colossus Smash");
			Debuffs.Add("Rend");
			Debuffs.Add("Hamstring");
			Debuffs.Add("Deep Wounds");
			Debuffs.Add("Concentrated Flame");

			Items.Add(TopTrinket);
			Items.Add(BotTrinket);
			Items.Add(usableitems);
			Items.Add("Healthstone");

			Macros.Add("ItemUse", "/use " + usableitems);
			Macros.Add("TopTrink", "/use 13");
			Macros.Add("BotTrink", "/use 14");
			Macros.Add("StormBoltOff", "/" + FiveLetters + " StormBolt");
			Macros.Add("IntimidatingShoutOff", "/" + FiveLetters + " IntimidatingShout");
			Macros.Add("RallyingCryOff", "/" + FiveLetters + " RallyingCry");
			Macros.Add("BladestormOff", "/" + FiveLetters + " Bladestorm");

			CustomCommands.Add("Burst");
			CustomCommands.Add("Potions");
			CustomCommands.Add("SaveCooldowns");
			CustomCommands.Add("noaoe");
			CustomCommands.Add("pvp");
			CustomCommands.Add("savepp");
			CustomCommands.Add("StormBolt");
			CustomCommands.Add("RallyingCry");
			CustomCommands.Add("IntimidatingShout");
			CustomCommands.Add("Bladestorm");
			CustomCommands.Add("DefensiveStance");

			#endregion
		}

		// optional override for the CombatTick which executes while in combat

		public override bool CombatTick() {

			#region MiscSetup

			bool Fighting = Aimsharp.Range("target") <= 8 && Aimsharp.TargetIsEnemy();
			bool Moving = Aimsharp.PlayerIsMoving();
			float Haste = Aimsharp.Haste() / 100f;
			int Time = Aimsharp.CombatTime();
			int Range = Aimsharp.Range("target");
			int TargetHealth = Aimsharp.Health("target");
			int PlayerHealth = Aimsharp.Health("player");
			string LastCast = Aimsharp.LastCast();
			bool IsChanneling = Aimsharp.IsChanneling("player");

			int EnemiesInMelee = Aimsharp.EnemiesInMelee();
			int GCDMAX = (int) (1500f / (Haste + 1f));
			int GCD = Aimsharp.GCD();
			int Latency = Aimsharp.Latency;
			bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) ||
			               Aimsharp.HasBuff("Heroism", "player", false) ||
			               Aimsharp.HasBuff("Time Warp", "player", false) ||
			               Aimsharp.HasBuff("Ancient Hysteria", "player", false) ||
			               Aimsharp.HasBuff("Netherwinds", "player", false) ||
			               Aimsharp.HasBuff("Drums of Rage", "player", false);
			int FlameFullRecharge = (int) (Aimsharp.RechargeTime("Concentrated Flame") - GCD +
			                               (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
			int ShiverVenomStacks = Aimsharp.DebuffStacks("Shiver Venom");

			int CDGuardianOfAzerothRemains = Aimsharp.SpellCooldown("Guardian of Azeroth") - GCD;
			bool BuffGuardianOfAzerothUp = Aimsharp.HasBuff("Guardian of Azeroth");
			int CDBloodOfTheEnemyRemains = Aimsharp.SpellCooldown("Blood of the Enemy") - GCD;
			int BuffMemoryOfLucidDreamsRemains = Aimsharp.BuffRemaining("Memory of Lucid Dreams") - GCD;
			int CDMemoryOfLucidDreamsRemains = Aimsharp.SpellCooldown("Memory of Lucid Dreams") - GCD;
			bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemains > 0;
			bool DebuffRazorCoralUp = Aimsharp.HasDebuff("Razor Coral");
			bool DebuffConductiveInkUp = Aimsharp.HasDebuff("Conductive Ink");
			int Rage = Aimsharp.Power("player");

			#endregion

			#region CommandsSetup

			// Commands
			bool UsePotion = Aimsharp.IsCustomCodeOn("Potions");
			bool NoCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
			bool OffAOE = Aimsharp.IsCustomCodeOn("noaoe");
			bool PVPmode = Aimsharp.IsCustomCodeOn("pvp");
			bool StormBolt = Aimsharp.IsCustomCodeOn("StormBolt");
			bool RallyingCry = Aimsharp.IsCustomCodeOn("RallyingCry");
			bool IntimidatingShout = Aimsharp.IsCustomCodeOn("IntimidatingShout");
			bool Bladestorm = Aimsharp.IsCustomCodeOn("Bladestorm");
			bool Burst = Aimsharp.IsCustomCodeOn("Burst");
			bool DefensiveStanceToggle = Aimsharp.IsCustomCodeOn("DefensiveStance");

			#endregion

			#region CooldownsSetup

			// CDS
			int CDSweepingStrikesRemains = Aimsharp.SpellCooldown("Sweeping Strikes") - GCD;
			int ColossusSmashRemains = Aimsharp.DebuffRemaining("Colossus Smash", "target") - GCD;
			int CDColossusSmashRemains = Aimsharp.SpellCooldown("Colossus Smash") - GCD;
			int CDBladestormRemains = Aimsharp.SpellCooldown("Bladestorm") - GCD;
			bool DebuffColossusSmashUp = ColossusSmashRemains > 0;
			int RendRemains = Aimsharp.DebuffRemaining("Rend") - GCD;
			bool DebuffRendUp = RendRemains > 0;
			bool DebuffHamstringUp = Aimsharp.DebuffRemaining("Hamstring") - GCD > 0;
			int BerserkerRageRemains = Aimsharp.BuffRemaining("Berserker Rage");
			bool BuffBerserkerRageUp = BerserkerRageRemains > 0;
			int CDStormBoltRemains = Aimsharp.SpellCooldown("Storm Bolt");
			bool CDStormBoltUp = CDStormBoltRemains > GCD;
			int CDIntimidatingShoutRemains = Aimsharp.SpellCooldown("Intimidating Shout");
			int CDRallyingCryRemains = Aimsharp.SpellCooldown("Rallying Cry");
			int CDDeadlyCalmRemains = Aimsharp.SpellCooldown("Deadly Calm") - GCD;
			bool CDDeadlyCalmUp = CDDeadlyCalmRemains > 0;
			int CDMortalStrikeRemains = Aimsharp.SpellCooldown("Mortal Strike") - GCD;
			bool CDMortalStrikeUp = CDMortalStrikeRemains > 0;
			int CDSkullsplitterRemains = Aimsharp.SpellCooldown("Skullsplitter") - GCD;
			int CDOverpowerRemains = Aimsharp.SpellCooldown("Overpower") - GCD;
			int CDCleaveRemains = Aimsharp.SpellCooldown("Cleave") - GCD;

			#endregion

			#region TalentsSetup

			//Talents
			bool TalentDoubleTime = Aimsharp.Talent(2, 1);
			bool TalentStormbolt = Aimsharp.Talent(2, 3);
			bool TalentMassacre = Aimsharp.Talent(3, 1);
			bool TalentFervorOfBattle = Aimsharp.Talent(3, 2);
			bool TalentCollateralDamage = Aimsharp.Talent(5, 1);
			bool TalentWarbreaker = Aimsharp.Talent(5, 2);
			bool TalentCleave = Aimsharp.Talent(5, 3);
			bool TalentAvatar = Aimsharp.Talent(6, 2);
			bool TalentDreadnaught = Aimsharp.Talent(7, 2);
			bool TalentDeadlyCalm = Aimsharp.Talent(6, 3);

			#endregion

			#region BuffsSetup

			//Buffs
			bool SweepingStrikesUp = Aimsharp.HasBuff("Sweeping Strikes", "player");
			bool TestOfMightUp = Aimsharp.HasBuff("Test of Might", "player");
			bool BladestormUp = Aimsharp.HasBuff("Bladestorm", "player");
			bool BattleShoutUp = Aimsharp.HasBuff("Battle Shout", "player");
			bool BuffSharpenBladeUp = Aimsharp.HasBuff("Sharpen Blade", "player");
			bool BuffOverpower = Aimsharp.HasBuff("Overpower", "player");
			int BuffStacksOP = Aimsharp.BuffStacks("Overpower", "player");
			bool BuffCrushingAssault = Aimsharp.HasBuff("Crushing Assault", "player");
			bool BuffDeadlyCalmUp = Aimsharp.HasBuff("Deadly Calm", "player");

			int BuffTestOfMightRemains = Aimsharp.BuffRemaining("Test of Might", "player") - GCD;
			bool BuffTestOfMightReady = BuffTestOfMightRemains <= 800 && BuffTestOfMightRemains >= 500;
			int BuffExecutionersPrecisionStacks = Aimsharp.BuffStacks("Executioner's Precision", "player");
			int BuffWhirlwindRemains = Aimsharp.BuffRemaining("Whirlwind") - GCD;
			bool BuffWhirlwindUp = BuffWhirlwindRemains > 0;

			#endregion

			#region DebuffsSetup

			//Debuffs
			bool DebuffDeepWoundsUp = Aimsharp.HasDebuff("Deep Wounds", "target");
			int DebuffDeepWoundsRemaining = Aimsharp.DebuffRemaining("Deep Wounds", "target");
			int DebuffConcentratedFlameRemaining = Aimsharp.DebuffRemaining("Concentrated Flame", "target");
			bool DebuffConcentratedFlameUp = Aimsharp.HasDebuff("Concentrated Flame", "target");

			#endregion

			#region OptionsSetup

			// Options
			bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
			bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");
			bool TestOfMightTrait = GetCheckBox("Test of Might Trait?");

			#endregion

			if (OffAOE) {
				EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
			}

			if (IsChanneling) return false;

			#region Cooldowns

			//COOLDOWNS
			if (!NoCooldowns && Fighting) {

				//TRINKET 1
				if (Aimsharp.CanUseTrinket(0) && TopTrinket == "Generic") {
					if (!TopTrinketAOE) {
						Aimsharp.Cast("TopTrink", true);
						return true;
					}
					else if (EnemiesInMelee >= 1) {
						Aimsharp.Cast("TopTrink", true);
						return true;
					}
				}

				//TRINKET 2
				if (Aimsharp.CanUseTrinket(1) && BotTrinket == "Generic") {
					if (!BotTrinketAOE) {
						Aimsharp.Cast("BotTrink", true);
						return true;
					}
					else if (EnemiesInMelee >= 1) {
						Aimsharp.Cast("BotTrink", true);
						return true;
					}
				}

				//POTION
				if (Aimsharp.CanUseItem(usableitems) && usableitems != "None" && !UsePotion) {
					if (EnemiesInMelee >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
						Aimsharp.Cast("ItemUse", true);
						return true;
					}
				}

				//BLOOD FURY
				if (RacialPower == "Orc") {
					if (Aimsharp.CanCast("Blood Fury", "player")) {
						if (DebuffColossusSmashUp) {
							Aimsharp.Cast("Blood Fury", true);
							return true;
						}
					}
				}

				//BERSERKING
				if (RacialPower == "Troll") {
					if (Aimsharp.CanCast("Berserking", "player") &&DebuffColossusSmashUp) {
						Aimsharp.Cast("Berserking", true);
						return true;
					}
				}

				//LIGHTS JUDGEMENT
				if (RacialPower == "Lightforged Draenei") {
					if (Aimsharp.CanCast("Light's Judgment", "player") && !DebuffColossusSmashUp) {
						Aimsharp.Cast("Light's Judgment", true);
						return true;
					}
				}

				//FIREBLOOD
				if (RacialPower == "Dark Iron Dwarf") {
					if (Aimsharp.CanCast("Fireblood", "player") && (DebuffColossusSmashUp)) {
						Aimsharp.Cast("Fireblood", true);
						return true;
					}
				}

				//ANCESTRAL CALL
				if (RacialPower == "Mag'har Orc") {
					if (Aimsharp.CanCast("Ancestral Call", "player") && DebuffColossusSmashUp) {
						Aimsharp.Cast("Ancestral Call", true);
						return true;
					}
				}

				//AVATAR								
				if (TalentAvatar) {
					if (Aimsharp.CanCast("Avatar", "player") && !PVPmode) {
						if (CDColossusSmashRemains < 8000) {
							Aimsharp.Cast("Avatar");
							return true;
						}

					}
				}


				



			}

			#endregion

			#region Utility

			// Utility


			// QUEUED STORMBOLT
			if (CDStormBoltRemains > 5000 && StormBolt) {
				Aimsharp.Cast("StormBoltOff");
				return true;
			}

			if (StormBolt && Aimsharp.CanCast("Storm Bolt")) {
				Aimsharp.PrintMessage("Queued Storm Bolt");
				Aimsharp.Cast("Storm Bolt");
				return true;
			}

			// QUEUED INTIMIDATING SHOUT
			if (CDIntimidatingShoutRemains > 5000 && IntimidatingShout) {
				Aimsharp.Cast("IntimidatingShoutOff");
				return true;
			}

			if (IntimidatingShout && Aimsharp.CanCast("Intimidating Shout")) {
				Aimsharp.PrintMessage("Queued Intimidating Shout");
				Aimsharp.Cast("Intimidating Shout");
				return true;
			}

			// QUEUED RALLYING CRY
			if (CDRallyingCryRemains > 5000 && RallyingCry) {
				Aimsharp.Cast("RallyingCryOff");
				return true;
			}

			if (RallyingCry && Aimsharp.CanCast("Rallying Cry", "player")) {
				Aimsharp.PrintMessage("Queued Rallying Cry");
				Aimsharp.Cast("Rallying Cry");
				return true;
			}

			// QUEUED BLADESTORM
			if (CDBladestormRemains > 5000 && Bladestorm) {
				Aimsharp.Cast("BladestormOff");
				return true;
			}

			if (Bladestorm && Aimsharp.CanCast("Bladestorm", "player")) {
				Aimsharp.PrintMessage("Queued Bladestorm");
				Aimsharp.Cast("Bladestorm");
				return true;
			}

			// Auto Victory Rush
			if (Aimsharp.CanCast("Victory Rush")) {
				if (PlayerHealth <= GetSlider("Auto Victory Rush @ HP%")) {
					Aimsharp.Cast("Victory Rush");
					return true;
				}
			}

			// Auto Commanding Shout
			if (Aimsharp.CanCast("Rallying Cry", "player")) {
				if (PlayerHealth <= GetSlider("Auto Shout @ HP%")) {
					Aimsharp.Cast("Rallying Cry");
					return true;
				}
			}

			//Auto Healthstone
			if (Aimsharp.CanUseItem("Healthstone")) {
				if (PlayerHealth <= GetSlider("Auto Healthstone @ HP%")) {
					Aimsharp.Cast("Healthstone");
					return true;
				}
			}

			// Auto Defensive Stance
			if (!Aimsharp.HasBuff("Defensive Stance", "player") && Aimsharp.CanCast("Defensive Stance", "player")) {
				if (PlayerHealth <= GetSlider("Auto Stance @ HP%") || DefensiveStanceToggle) {
					Aimsharp.Cast("Defensive Stance");
					return true;
				}
			}

			if (Aimsharp.HasBuff("Defensive Stance", "player") && Aimsharp.CanCast("Defensive Stance", "player")) {
				if (PlayerHealth >= GetSlider("Unstance @ HP%") && !DefensiveStanceToggle) {
					Aimsharp.Cast("Defensive Stance");
					return true;
				}
			}

			if (Aimsharp.CanCast("Die by the Sword", "player")) {
				if (PlayerHealth <= GetSlider("Auto Die by the Sword @ HP%")) {
					Aimsharp.Cast("Die by the Sword");
					return true;
				}
			}
			
			if (Aimsharp.CanCast("Ignore Pain", "player")) {
				if (PlayerHealth <= GetSlider("Auto Ignore Pain @ HP%")) {
					Aimsharp.Cast("Ignore Pain");
					return true;
				}
			}

			#endregion

			#region PVE Rotation

			// PVE Rotation
			if (!PVPmode) {

				if (Fighting) {

					//SWEEPING STRIKES actions+=/sweeping_strikes,if=spell_targets.whirlwind>1&(cooldown.bladestorm.remains>10|cooldown.colossus_smash.remains>8|azerite.test_of_might.enabled)

					if (Aimsharp.CanCast("Sweeping Strikes", "player") && !OffAOE) {
						if (EnemiesInMelee > 1 && (CDBladestormRemains > 10000 || CDColossusSmashRemains > 8000 ||
						                           TestOfMightTrait)) {
							Aimsharp.Cast("Sweeping Strikes");
							return true;
						}
					}

					#region Single Target

					//NO AOE
					if (EnemiesInMelee >= 1) {

						//EXECUTE RANGE
						if (TargetHealth <= 20 || (TalentMassacre && TargetHealth <= 35) || (Aimsharp.CovenantID() == 2 && TargetHealth >= 80 )) {

							if (Aimsharp.CanCast("Deadly Calm", "player") && !NoCooldowns) {
								Aimsharp.Cast("Deadly Calm");
								return true;
							}

							if (Aimsharp.CanCast("Skullsplitter") &&
							    ((!BuffMemoryOfLucidDreamsUp && Rage < 52) || Rage < 20)) {
								Aimsharp.Cast("Skullsplitter");
								return true;

							}


							if (MajorPower != "Memory of Lucid Dreams" || BuffMemoryOfLucidDreamsUp ||
							    CDMemoryOfLucidDreamsRemains > 10000) {

								if (!TalentWarbreaker) {
									if (CDColossusSmashRemains <= 0 || Aimsharp.CanCast("Colossus Smash")) {
										Aimsharp.Cast("Colossus Smash");
										return true;
									}
								}
								else {
									if (Aimsharp.CanCast("Warbreaker", "player")) {
										Aimsharp.Cast("Warbreaker");
										return true;
									}
								}

							}

							//actions.execute+=/mortal_strike,if=dot.deep_wounds.remains<=duration*0.3&(spell_targets.whirlwind=1|!spell_targets.whirlwind>1&!talent.cleave.enabled)
							if ((DebuffDeepWoundsRemaining <= 3600 && EnemiesInMelee == 1 ||
							     EnemiesInMelee > 1 && !TalentCleave) && Aimsharp.CanCast("Mortal Strike")) {
								Aimsharp.Cast("Mortal Strike");
								return true;
							}

							if ((EnemiesInMelee > 2 && DebuffDeepWoundsRemaining < 3600 || EnemiesInMelee > 3) &&
							    ((CDCleaveRemains <= 0 && Rage >= 20) || Aimsharp.CanCast("Cleave"))) {
								Aimsharp.Cast("Cleave");
								return true;
							}


							if (!BuffDeadlyCalmUp && !BuffMemoryOfLucidDreamsUp && TestOfMightUp && Rage < 30 &&
							    Aimsharp.CanCast("Bladestorm", "player")) {
								Aimsharp.Cast("Bladestorm");
								return true;
							}


							if ((BuffMemoryOfLucidDreamsUp || BuffDeadlyCalmUp || TestOfMightUp || DebuffColossusSmashUp
							) && Aimsharp.CanCast("Execute")) {
								Aimsharp.Cast("Execute");
								return true;
							}


							if (BuffCrushingAssault && !BuffMemoryOfLucidDreamsUp && Aimsharp.CanCast("Slam")) {
								Aimsharp.Cast("Slam");
								return true;
							}





							if (CDOverpowerRemains <= 0 || Aimsharp.CanCast("Overpower")) {
								Aimsharp.Cast("Overpower");
								return true;
							}

							if (Aimsharp.CanCast("Execute")) {
								Aimsharp.Cast("Execute");
								return true;
							}

						}
						else {
							//EXECUTE END


							//SINGLE TARGET > 20%
							if (RendRemains <= 4500 && Aimsharp.CanCast("Rend")) {
								Aimsharp.Cast("Rend");
								return true;
							}

							if (Aimsharp.CanCast("Deadly Calm", "player") && !NoCooldowns) {
								Aimsharp.Cast("Deadly Calm");
								return false;
							}

							if (((Rage < 60 && !BuffMemoryOfLucidDreamsUp && !BuffDeadlyCalmUp) || Rage < 20) &&
							    Aimsharp.CanCast("Skullsplitter")) {
								Aimsharp.Cast("Skullsplitter");
								return true;
							}

							if ((DebuffDeepWoundsRemaining < 3600 && (EnemiesInMelee == 1 || !TalentCleave)) &&
							    Aimsharp.CanCast("Mortal Strike")) {
								Aimsharp.Cast("Mortal Strike");
								return true;

							}

							if (DebuffDeepWoundsRemaining < 3600 && EnemiesInMelee > 2 &&
							    Aimsharp.CanCast("Cleave", "player")) {
								Aimsharp.Cast("Cleave");
								return true;
							}

							if (!TalentWarbreaker) {
								if (Aimsharp.CanCast("Colossus Smash")) {
									Aimsharp.Cast("Colossus Smash");
									return true;
								}
							}
							else {
								if (Aimsharp.CanCast("Warbreaker", "player")) {
									Aimsharp.Cast("Warbreaker");
									return true;
								}
							}



							//SUDDEN DEATH PROC, PROBABLY
							if (Aimsharp.CanCast("Execute")) {
								Aimsharp.Cast("Execute");
								return true;
							}

							if (!CDMortalStrikeUp && (!TalentDeadlyCalm || !CDDeadlyCalmUp) &&
							    ((!TestOfMightTrait && DebuffColossusSmashUp) || TestOfMightUp) &&
							    !BuffMemoryOfLucidDreamsUp && Rage < 40 && Aimsharp.CanCast("Bladestorm", "player")) {
								Aimsharp.Cast("Bladestorm");
								return true;
							}

							if ((Aimsharp.CanCast("Mortal Strike")) &&
							    (EnemiesInMelee == 1 || !TalentCleave)) {
								Aimsharp.Cast("Mortal Strike");
								return true;
							}

							if (EnemiesInMelee > 2 && Aimsharp.CanCast("Cleave", "player")) {
								Aimsharp.Cast("Cleave");
								return true;
							}


							//actions.single_target+=/whirlwind,if=(((buff.memory_of_lucid_dreams.up)|(debuff.colossus_smash.up)|(buff.deadly_calm.up))&talent.fervor_of_battle.enabled)|((buff.memory_of_lucid_dreams.up|rage>89)&debuff.colossus_smash.up&buff.test_of_might.down&!talent.fervor_of_battle.enabled)
							if (Aimsharp.CanCast("Whirlwind", "player") &&
							    ((((BuffMemoryOfLucidDreamsUp) || (DebuffColossusSmashUp) || (BuffDeadlyCalmUp)) &&
							      TalentFervorOfBattle) || ((BuffMemoryOfLucidDreamsUp || Rage > 89) &&
							                                DebuffColossusSmashUp && !TestOfMightUp &&
							                                !TalentFervorOfBattle))) {
								Aimsharp.Cast("Whirlwind");
								return true;
							}



							if (((Rage < 30 && BuffMemoryOfLucidDreamsUp && DebuffColossusSmashUp) ||
							     (Rage < 70 && !BuffMemoryOfLucidDreamsUp)) &&
							    (CDOverpowerRemains <= 0 || Aimsharp.CanCast("Overpower"))) {
								Aimsharp.Cast("Overpower");
								return true;
							}

							if (!TalentFervorOfBattle && Aimsharp.CanCast("Slam") &&
							    (BuffMemoryOfLucidDreamsUp || DebuffColossusSmashUp)) {
								Aimsharp.Cast("Slam");
								return true;
							}

							if (Aimsharp.CanCast("Overpower")) {
								Aimsharp.Cast("Overpower");
								return true;
							}


							if (TalentFervorOfBattle &&
							    (TestOfMightUp || !DebuffColossusSmashUp && !TestOfMightUp && Rage > 60) &&
							    Aimsharp.CanCast("Whirlwind", "player")) {
								Aimsharp.Cast("Whirlwind");
								return true;
							}

							if (!TalentFervorOfBattle && Aimsharp.CanCast("Slam")) {
								Aimsharp.Cast("Slam");
								return true;
							}
						}
					}

					//SINGLE TARGET OVER

					#endregion

					
					#region AOE
					//FIVE TARGETS+
					if(EnemiesInMelee > 4 && !OffAOE) {
						
						if(Rage < 60 && CDSkullsplitterRemains <= 0) {
							Aimsharp.Cast("Skullsplitter");
							return true;
						}
						
						if (!TalentWarbreaker) {
							if (CDColossusSmashRemains <= 0 && TargetHealth >= 5) {
								Aimsharp.Cast("Colossus Smash");
								return true;
							}
						} 
						else {
							if (Aimsharp.CanCast("Warbreaker", "player")) {
								Aimsharp.Cast("Warbreaker");
								return true;
							}
						}
						
						if(!SweepingStrikesUp && ((ColossusSmashRemains >4500 && !TestOfMightTrait) || TestOfMightUp) && CDBladestormRemains <= 0) {
							Aimsharp.Cast("Bladestorm");
							return true;
						}

						if (Aimsharp.CanCast("Deadly Calm", "player") && !NoCooldowns) {
							Aimsharp.Cast("Deadly Calm");
							return true;
						}
						
						if(CDCleaveRemains <= 0 && Rage >= 20) {
							Aimsharp.Cast("Cleave");
							return true;
						}
						
						if((!TalentCleave && DebuffDeepWoundsRemaining <2000) || ((Aimsharp.CanCast("Execute") && TargetHealth >20) && (SweepingStrikesUp || CDSweepingStrikesRemains>8000))) {
							if(Aimsharp.CanCast("Execute")) {
								Aimsharp.Cast("Execute");
								return true;
							}
						}
						
						if(Rage > 60 && Aimsharp.CanCast("Whirlwind", "player")) {
							Aimsharp.Cast("Whirlwind");
							return true;
						}
						
						if(Aimsharp.CanCast("Overpower") || CDOverpowerRemains <= 0) {
							Aimsharp.Cast("Overpower");
							return true;
						}
						
						if(Aimsharp.CanCast("Whirlwind", "player")) {
							Aimsharp.Cast("Whirlwind");
							return true;
						}
						
						
						
						
						
						
						
						
						
						
						
						
						if (Aimsharp.CanCast("Heroic Throw")) {
							if (Rage <= 30 && Range <= 30) {
								Aimsharp.Cast("Heroic Throw");
								return true;
							}
						}
						
					}
					#endregion
					
				}
			}

			#endregion

			#region PVP Rotation

			//PVP Rotation
			if (PVPmode) {


				if (Aimsharp.CanCast("Sharpen Blade")) {
					if (!BuffSharpenBladeUp && TargetHealth <= 40) {
						Aimsharp.Cast("Sharpen Blade");
						return true;
					}
				}

				if (Aimsharp.CanCast("Heroic Throw")) {
					if (Rage <= 30 && Range <= 30 && Range > 8) {
						Aimsharp.Cast("Heroic Throw");
						return true;
					}
				}

				if (Fighting || Burst) {
					if (Burst) {
						if (Aimsharp.CanCast("Avatar", "player")) {
							Aimsharp.Cast("Avatar");
							return true;
						}

						if (TalentWarbreaker && Aimsharp.CanCast("Warbreaker", "player") && EnemiesInMelee >= 2) {
							Aimsharp.Cast("Warbreaker");
							return true;
						}

						if (MajorPower == "Reaping Flames" &&
						    (TargetHealth >= 80 || TargetHealth <= 20 || (TargetHealth >= 40 && TargetHealth <= 70)) &&
						    Aimsharp.CanCast("Reaping Flames")) {
							Aimsharp.Cast("Reaping Flames");
							return true;
						}

						if (BuffCrushingAssault && !BuffMemoryOfLucidDreamsUp && Aimsharp.CanCast("Slam")) {
							Aimsharp.Cast("Slam");
							return true;
						}

						if (Aimsharp.CanCast("Execute")) {
							Aimsharp.Cast("Execute");
							return true;
						}

						if (BuffStacksOP >= 2 && Aimsharp.CanCast("Overpower")) {
							Aimsharp.Cast("Overpower");
							return true;
						}


						if (EnemiesInMelee >= 2 && Aimsharp.CanCast("Sweeping Strikes")) {
							Aimsharp.Cast("Sweeping Strikes");
							return true;
						}

						if (!TalentWarbreaker) {
							if (CDColossusSmashRemains <= 0 || Aimsharp.CanCast("Colossus Smash")) {
								Aimsharp.Cast("Colossus Smash");
								return true;
							}
						}
						else {
							if (Aimsharp.CanCast("Warbreaker", "player")) {
								Aimsharp.Cast("Warbreaker");
								return true;
							}
						}

						if (Aimsharp.CanCast("Overpower") || CDOverpowerRemains <= 0) {
							Aimsharp.Cast("Overpower");
							return true;
						}

						if (Aimsharp.CanCast("Mortal Strike") || CDMortalStrikeRemains <= 0) {
							Aimsharp.Cast("Mortal Strike");
							return true;
						}

						if (Aimsharp.CanCast("Bladestorm", "player") || CDBladestormRemains <= 0) {
							Aimsharp.Cast("Bladestorm");
							return true;
						}

						if (!DebuffRendUp && Aimsharp.CanCast("Rend")) {
							Aimsharp.Cast("Rend");
							return true;
						}

						if (Aimsharp.CanCast("Slam")) {
							Aimsharp.Cast("Slam");
							return true;
						}
						
						if (!DebuffHamstringUp && Aimsharp.CanCast("Hamstring")) {
							Aimsharp.Cast("Hamstring");
							return true;
						}
					}
					else {
						if (Aimsharp.CanCast("Execute")) {
							Aimsharp.Cast("Execute");
							return true;
						}

						if (BuffCrushingAssault && !BuffMemoryOfLucidDreamsUp && Aimsharp.CanCast("Slam")) {
							Aimsharp.Cast("Slam");
							return true;
						}

						if (BuffStacksOP >= 2 && Aimsharp.CanCast("Overpower")) {
							Aimsharp.Cast("Overpower");
							return true;
						}

						if (!DebuffRendUp && Aimsharp.CanCast("Rend")) {
							Aimsharp.Cast("Rend");
							return true;
						}

						if (Aimsharp.CanCast("Overpower") || CDOverpowerRemains <= 0) {
							Aimsharp.Cast("Overpower");
							return true;
						}

						if (MajorPower == "Reaping Flames" && (TargetHealth >= 80 || TargetHealth <= 20) &&
						    Aimsharp.CanCast("Reaping Flames")) {
							Aimsharp.Cast("Reaping Flames");
							return true;
						}

						if (TalentWarbreaker && Aimsharp.CanCast("Warbreaker", "player") && EnemiesInMelee >= 2) {
							Aimsharp.Cast("Warbreaker");
							return true;
						}

						if (Aimsharp.CanCast("Mortal Strike") || CDMortalStrikeRemains <= 0) {
							Aimsharp.Cast("Mortal Strike");
							return true;
						}

						if (EnemiesInMelee >= 2 && Aimsharp.CanCast("Sweeping Strikes")) {
							Aimsharp.Cast("Sweeping Strikes");
							return true;
						}

						if (!DebuffHamstringUp && Aimsharp.CanCast("Hamstring")) {
							Aimsharp.Cast("Hamstring");
							return true;
						}

						if (Rage >= 50 && Aimsharp.CanCast("Slam")) {
							Aimsharp.Cast("Slam");
							return true;
						}




					}
				}
			}

			#endregion

			return false;
		}
	

	public override bool OutOfCombatTick() {
			bool Prepull = Aimsharp.IsCustomCodeOn("savepp");
			int PlayerHealth = Aimsharp.Health("player");
			bool DefensiveStanceToggle = Aimsharp.IsCustomCodeOn("DefensiveStance");
			
			if (Aimsharp.CanCast("Battle Shout", "player") && !Aimsharp.HasBuff("Battle Shout", "player", false)) {
				Aimsharp.Cast("Battle Shout");
				return true;
			}
			
			// Auto Defensive Stance
			if (!Aimsharp.HasBuff("Defensive Stance", "player") && Aimsharp.CanCast("Defensive Stance", "player")) {
				if (PlayerHealth <= GetSlider("Auto Stance @ HP%") || DefensiveStanceToggle) {
					Aimsharp.Cast("Defensive Stance");
					return true;
				}
			}
				
			if (Aimsharp.HasBuff("Defensive Stance", "player") && Aimsharp.CanCast("Defensive Stance", "player")) {
				if (PlayerHealth >= GetSlider("Unstance @ HP%") && !DefensiveStanceToggle) {
					Aimsharp.Cast("Defensive Stance");
					return true;
				}
			}
			
			
			return false;
		}
	}
}							