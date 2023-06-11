using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{
    /// <summary>
	/// From Kaneto:
	/// I used this rotation to create a better one for Prot Warriors. Feedback to xKaneto on Discord.
    /// </summary>
	
    public class KanetoProtWar : Rotation
    {

        public override void LoadSettings()
        {
            List<string> Trinkets = new List<string>(new string[] {"Sinful Gladiator's Medallion", "Sinful Aspirant's Medallion", "Generic", "None" });
            Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
            Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));

            List<string> Race = new List<string>(new string[] { "Orc", "Troll", "Dark Iron Dwarf", "Mag'har Orc", "Lightforged Draenei", "Bloodelf", "None" });
            Settings.Add(new Setting("Racial Power", Race, "None"));

            Settings.Add(new Setting("Use only free Revenge", false));

            Settings.Add(new Setting("Use Ignore Pain for survival @ HP%", 0, 100, 75));
            
            Settings.Add(new Setting("Auto Victory Rush @ HP%", 0, 100, 70));
            
            Settings.Add(new Setting("Auto Shout @ HP%", 0, 100, 33));
            
            Settings.Add(new Setting("Auto Last Stand @ HP%", 0, 100, 26));
            
            Settings.Add(new Setting("Auto Shield Wall @ HP%", 0, 100, 24));
            
            Settings.Add(new Setting("Auto Healthstone @ HP%", 0, 100, 25));
            
            Settings.Add(new Setting("First 5 Letters of the Addon:", "xxxxx"));

        }
		
        string TopTrinket;
        string BotTrinket;
        string RacialPower;
        string FiveLetters;

        public override void Initialize()
        {
            // Aimsharp.DebugMode();

            Aimsharp.PrintMessage("Kaneto's Protection Warrior", Color.Blue);
            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
			Aimsharp.PrintMessage(" ");
			Aimsharp.PrintMessage("Special Toggles for usage.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx StormBolt --Queues Storm Bolt up to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx RallyingCry --Queues Rallying Cry up to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx IntimidatingShout --Queues Intimidating Shout up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx Shockwave -- Queues Shockwave to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);
			Aimsharp.PrintMessage("--Raid Talents: 3221211, M+ Talents: 3221233", Color.Red);

            Aimsharp.Latency = 50;
            Aimsharp.QuickDelay = 125;
            Aimsharp.SlowDelay = 250;

            TopTrinket = GetDropDown("Top Trinket");
            BotTrinket = GetDropDown("Bot Trinket");
            RacialPower = GetDropDown("Racial Power");
            FiveLetters = GetString("First 5 Letters of the Addon:");

            if (RacialPower == "Orc")
                Spellbook.Add("Blood Fury");
            if (RacialPower == "Troll")
                Spellbook.Add("Berserking");
            if (RacialPower == "Dark Iron Dwarf")
                Spellbook.Add("Fireblood");
            if (RacialPower == "Mag'har Orc")
                Spellbook.Add("Ancestral Call");
            if (RacialPower == "Lightforged Draenei")
                Spellbook.Add("Light's Judgment");
            if (RacialPower == "Bloodelf")
                Spellbook.Add("Arcane Torrent");

            Spellbook.Add("Intercept");
            Spellbook.Add("Avatar");
            Spellbook.Add("Ignore Pain");
            Spellbook.Add("Demoralizing Shout");
            Spellbook.Add("Last Stand");
            Spellbook.Add("Thunder Clap");
            Spellbook.Add("Ravager");
            Spellbook.Add("Shield Block");
            Spellbook.Add("Shield Slam");
            Spellbook.Add("Devastate");
            Spellbook.Add("Victory Rush");
            Spellbook.Add("Revenge");
            Spellbook.Add("Intimidating Shout");
            Spellbook.Add("Shockwave");
            Spellbook.Add("Storm Bolt");
            Spellbook.Add("Rallying Cry");
            Spellbook.Add("Shield Wall");
			Spellbook.Add("Battle Shout");
			Spellbook.Add("Spear of Bastion");
			Spellbook.Add("Pummel");
			Spellbook.Add("Execute");

            Buffs.Add("Bloodlust");
            Buffs.Add("Heroism");
            Buffs.Add("Time Warp");
            Buffs.Add("Ancient Hysteria");
            Buffs.Add("Netherwinds");
            Buffs.Add("Drums of Rage");
            Buffs.Add("Lifeblood");
            Buffs.Add("Reckless Force");

            Buffs.Add("Victorious");
            Buffs.Add("Avatar");
            Buffs.Add("Ignore Pain");
            Buffs.Add("Shield Block");
            Buffs.Add("Revenge!");
			Buffs.Add("Battle Shout");

            Items.Add(TopTrinket);
            Items.Add(BotTrinket);
            Items.Add("Healthstone");
			Items.Add("Phial of Serenity");

            Macros.Add("TopTrinket", "/use 13");
            Macros.Add("BotTrinket", "/use 14");
            Macros.Add("potion", "/use Phial of Serenity");
            Macros.Add("StormBoltOff", "/"+FiveLetters+" StormBolt");
            Macros.Add("IntimidatingShoutOff", "/"+FiveLetters+" IntimidatingShout");
            Macros.Add("RallyingCryOff", "/"+FiveLetters+" RallyingCry");
            Macros.Add("ShockWaveOff", "/"+FiveLetters+" ShockWave");
			Macros.Add("BastionC","/cast [@player] Spear of Bastion");
			Macros.Add("RavageSelf", "/cast [@player] Ravager");

            
            CustomCommands.Add("SaveCooldowns");
            CustomCommands.Add("AOE");
            CustomCommands.Add("StormBolt");
            CustomCommands.Add("RallyingCry");
            CustomCommands.Add("IntimidatingShout");
            CustomCommands.Add("ShockWave");
        }


        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick() {
            bool UseOnlyFreeRevenge = GetCheckBox("Use only free Revenge");

            bool Fighting = Aimsharp.Range("target") <= 8 && Aimsharp.TargetIsEnemy();
            bool Moving = Aimsharp.PlayerIsMoving();
            float Haste = Aimsharp.Haste() / 100f;
            int Time = Aimsharp.CombatTime();
            int Range = Aimsharp.Range("target");
            int TargetHealth = Aimsharp.Health("target");
            string LastCast = Aimsharp.LastCast();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            bool NoCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            bool AOE = Aimsharp.IsCustomCodeOn("AOE");
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int GCDMAX = (int) (1500f / (Haste + 1f));
            int GCD = Aimsharp.GCD();
            int Latency = Aimsharp.Latency;
            int TargetTimeToDie = 1000000000;
            int PlayerHealth = Aimsharp.Health("player");
			
            bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) ||
                           Aimsharp.HasBuff("Heroism", "player", false) ||
                           Aimsharp.HasBuff("Time Warp", "player", false) ||
                           Aimsharp.HasBuff("Ancient Hysteria", "player", false) ||
                           Aimsharp.HasBuff("Netherwinds", "player", false) ||
                           Aimsharp.HasBuff("Drums of Rage", "player", false);
						   
            bool StormBolt = Aimsharp.IsCustomCodeOn("StormBolt");
            bool RallyingCry = Aimsharp.IsCustomCodeOn("RallyingCry");
            bool IntimidatingShout = Aimsharp.IsCustomCodeOn("IntimidatingShout");
            bool ShockWave = Aimsharp.IsCustomCodeOn("ShockWave");

            if (!AOE) {
                EnemiesNearTarget = 1;
                EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            }
			
			//Talent
			bool TalentStormbolt = Aimsharp.Talent(2, 3);
			bool TalentBoomingVoice = Aimsharp.Talent(3, 2);
            bool TalentUnstoppableForce = Aimsharp.Talent(6, 2);

            bool IgnorePainUP = Aimsharp.HasBuff("Ignore Pain");

            int Rage = Aimsharp.Power("player");
            int MaxRage = Aimsharp.PlayerMaxPower();
            int RageDefecit = MaxRage - Rage;

            int BuffAvatarRemains = Aimsharp.BuffRemaining("Avatar") - GCD;
            bool BuffAvatarUp = BuffAvatarRemains > 0;
            int BuffLastStandRemains = Aimsharp.BuffRemaining("Last Stand") - GCD;
            bool BuffLastStandUp = BuffLastStandRemains > 0;
            int BuffShieldBlockRemains = Aimsharp.BuffRemaining("Shield Block") - GCD;
            bool BuffShieldBlockUp = BuffShieldBlockRemains > 0;

            int CDAvatarRemains = Aimsharp.SpellCooldown("Avatar") - GCD;
            bool CDAvatarReady = CDAvatarRemains <= 10;
            int CDShieldSlamRemains = Aimsharp.SpellCooldown("Shield Slam") - GCD;
            bool CDShieldSlamReady = CDShieldSlamRemains <= 10;
            int CDDemoralizingShoutRemains = Aimsharp.SpellCooldown("Demoralizing Shout") - GCD;
            bool CDDemoralizingShoutReady = CDDemoralizingShoutRemains <= 10;

            int CDStormBoltRemains = Aimsharp.SpellCooldown("Storm Bolt");
            bool CDStormBoltUp = CDStormBoltRemains > GCD;
            int CDIntimidatingShoutRemains = Aimsharp.SpellCooldown("Intimidating Shout");
            int CDRallyingCryRemains = Aimsharp.SpellCooldown("Rallying Cry");
            int CDShockwaveRemains = Aimsharp.SpellCooldown("Shockwave");
			int CDSpearRemains = Aimsharp.SpellCooldown("Spear of Bastion") - GCD;
			bool CDSpearReady = CDSpearRemains <= 10;
			
			bool IsInterruptable = Aimsharp.IsInterruptable("target");
            int CastingRemaining = Aimsharp.CastingRemaining("target");
            int CastingElapsed = Aimsharp.CastingElapsed("target");
            bool IsChannelingTar = Aimsharp.IsChanneling("target");
            int KickValue = 300;
            int KickChannelsAfter = 500;

			int CovenantID = Aimsharp.CovenantID();
			bool CovenantKyrian = CovenantID == 1;
			bool CovenantVenthyr = CovenantID == 2;
			bool CovenantNightFae = CovenantID == 3;
			bool CovenantNecrolord = CovenantID == 4;

            if (IsChanneling)
                return false;

            
            if (Fighting) {
				
				
				// Interrupts
				if (Aimsharp.CanCast("Pummel"))
                {
                    if (IsInterruptable && !IsChannelingTar && CastingRemaining < KickValue && CastingElapsed > 500)
                    {
                        Aimsharp.Cast("Pummel", true);
                        return true;
                    }
                }

                if (Aimsharp.CanCast("Pummel"))
                {
                    if (IsInterruptable && IsChannelingTar && CastingElapsed > KickChannelsAfter)
                    {
                        Aimsharp.Cast("Pummel", true);
                        return true;
                    }
                }
				if (TalentStormbolt)
				{
					if (!Aimsharp.CanCast("Pummel")&& Aimsharp.CanCast("Storm Bolt") && CastingRemaining < KickValue && (IsChannelingTar || IsInterruptable || !IsInterruptable))
					{
						Aimsharp.Cast("Storm Bolt", true);
                        return true;
					}
				}

				if (Aimsharp.CanCast("Intimidating Shout") && CastingRemaining < KickValue && (IsInterruptable || IsChannelingTar))
				{
					Aimsharp.Cast("Intimidating Shout", true);
                    return true;
				}
			
                //actions +=/ use_items,if= cooldown.avatar.remains <= gcd | buff.avatar.up
                if (Aimsharp.CanUseTrinket(0)) {
                    if (CDAvatarRemains <= GCD || BuffAvatarUp) {
                        Aimsharp.Cast("TopTrinket", true);
                        return true;
                    }
                }

                if (Aimsharp.CanUseTrinket(1)) {
                    if (CDAvatarRemains <= GCD || BuffAvatarUp) {
                        Aimsharp.Cast("BotTrinket", true);
                        return true;
                    }
                }

                if (RacialPower == "Orc" && Fighting) {
                    if (Aimsharp.CanCast("Blood Fury", "player")) {
                        Aimsharp.Cast("Blood Fury", true);
                        return true;
                    }
                }

                if (RacialPower == "Troll" && Fighting) {
                    if (Aimsharp.CanCast("Berserking", "player")) {
                        Aimsharp.Cast("Berserking", true);
                        return true;
                    }
                }

                if (RacialPower == "Bloodelf" && Fighting) {
                    if (Aimsharp.CanCast("Arcane Torrent", "player")) {
                        Aimsharp.Cast("Arcane Torrent");
                        return true;
                    }
                }

                if (RacialPower == "Lightforged Draenei" && Fighting) {
                    if (Aimsharp.CanCast("Light's Judgment", "player")) {
                        Aimsharp.Cast("Light's Judgment", true);
                        return true;
                    }
                }

                if (RacialPower == "Dark Iron Dwarf" && Fighting) {
                    if (Aimsharp.CanCast("Fireblood", "player")) {
                        Aimsharp.Cast("Fireblood", true);
                        return true;
                    }
                }

                if (RacialPower == "Mag'har Orc" && Fighting) {
                    if (Aimsharp.CanCast("Ancestral Call", "player")) {
                        Aimsharp.Cast("Ancestral Call", true);
                        return true;
                    }
                }

                if (Fighting) {
                    if (PlayerHealth < 30) {
                        if (Aimsharp.CanUseItem("Phial of Serenity")) 
                        {
                            Aimsharp.Cast("potion", true);
                            return true;
                        }
                    }
                }

                //actions+=/ignore_pain,if=rage.deficit<25+20*talent.booming_voice.enabled*cooldown.demoralizing_shout.ready
                if (Aimsharp.CanCast("Ignore Pain", "player")) {
                    if ((RageDefecit < 25 + 20 * (TalentBoomingVoice ? 1 : 0) * (CDDemoralizingShoutReady ? 1 : 0)) || PlayerHealth <= GetSlider("Use Ignore Pain for survival @ HP%") && !IgnorePainUP) {
                        Aimsharp.Cast("Ignore Pain");
                        return true;
                    }
                }
				
                if (Aimsharp.CanCast("Avatar", "player") && !NoCooldowns) {
                    Aimsharp.Cast("Avatar");
                    return true;
                }

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

                // QUEUED Shockwave
                if (CDShockwaveRemains > 5000 && ShockWave) {
                    Aimsharp.Cast("ShockWaveOff");
                    return true;
                }

                if (ShockWave && Aimsharp.CanCast("Shockwave", "player")) {
                    Aimsharp.PrintMessage("Queued Shockwave");
                    Aimsharp.Cast("Shockwave");
                    return true;
                }
                
                // Auto Victory Rush
                if (Aimsharp.CanCast("Victory Rush")) {
                    if (PlayerHealth <= GetSlider("Auto Victory Rush @ HP%")) {
                        Aimsharp.Cast("Victory Rush");
                        return true;
                    }
                }
                
                if (Aimsharp.CanCast("Last Stand", "player")) {
                    if (PlayerHealth <= GetSlider("Auto Last Stand @ HP%")) {
                        Aimsharp.Cast("Last Stand");
                        return true;
                    }
                }
                
                if (Aimsharp.CanUseItem("Healthstone")) {
                    if (PlayerHealth <= GetSlider("Auto Healthstone @ HP%")) {
                        Aimsharp.Cast("Healthstone");
                        return true;
                    }
                }
                
                if (Aimsharp.CanCast("Rallying Cry", "player")) {
                    if (PlayerHealth <= GetSlider("Auto Shout @ HP%")) {
                        Aimsharp.Cast("Rallying Cry");
                        return true;
                    }
                }
                
                if (Aimsharp.CanCast("Shield Wall", "player")) {
                    if (PlayerHealth <= GetSlider("Auto Shield Wall @ HP%")) {
                        Aimsharp.Cast("Shield Wall");
                        return true;
                    }
                }
					
                #region AOE
                if (EnemiesInMelee >= 3) {
					
					//actions.aoe = Spear_of_Bastion
					if (EnemiesInMelee > 1 && CovenantKyrian && CDSpearReady && Fighting && BuffAvatarUp && !NoCooldowns){
					Aimsharp.Cast("BastionC");
					return true;
					}	
					
                    //actions.aoe = thunder_clap
                    if (Aimsharp.CanCast("Thunder Clap", "player")) {
                        Aimsharp.Cast("Thunder Clap");
                        return true;
                    }

                    //actions.aoe+=/demoralizing_shout,if=talent.booming_voice.enabled
                    if (Aimsharp.CanCast("Demoralizing Shout", "player") && TalentBoomingVoice) {
                        Aimsharp.Cast("Demoralizing Shout");
                        return true;
                    }

                    //actions.aoe+=/dragon_roar
                    if (Aimsharp.CanCast("Dragon Roar", "player")) {
                        Aimsharp.Cast("Dragon Roar");
                        return true;
                    }

                    //actions.aoe+=/revenge
                    if (Aimsharp.CanCast("Revenge", "player") && (UseOnlyFreeRevenge && Aimsharp.HasBuff("Revenge!") || !UseOnlyFreeRevenge) && Rage >= 40) {
                        Aimsharp.Cast("Revenge");
                        return true;
                    }

                    //actions.aoe+=/ravager
                    if (Aimsharp.CanCast("Ravager", "player")) {
                        Aimsharp.Cast("RavageSelf");
                        return true;
                    }

                    //actions.aoe+=/shield_block,if=cooldown.shield_slam.ready&buff.shield_block.down
                    if (Aimsharp.CanCast("Shield Block", "player") && CDShieldSlamReady && !BuffShieldBlockUp) {
                        Aimsharp.Cast("Shield Block");
                        return true;
                    }

                    //actions.aoe+=/shield_slam
                    if (Aimsharp.CanCast("Shield Slam")) {
                        Aimsharp.Cast("Shield Slam");
                        return true;
                    }
					
					//actions.aoe+=/shockwave
					if (EnemiesInMelee >= 2 && Aimsharp.CanCast("Shockwave", "player") && CastingRemaining < KickValue && (IsChannelingTar || IsInterruptable || !IsInterruptable)) {
                        Aimsharp.Cast("Shockwave");
                        return true;
                    }
                }
                
                #endregion

                #region Single Target
				
				//actions.st = Spear_of_Bastion
				if (CovenantKyrian && CDSpearReady && Fighting && BuffAvatarUp && !NoCooldowns){
					Aimsharp.Cast("BastionC");
					return true;
				}	
				
                //actions.st=thunder_clap,if=spell_targets.thunder_clap=2&talent.unstoppable_force.enabled&buff.avatar.up
                if (Aimsharp.CanCast("Thunder Clap", "player")) {
                    if (EnemiesInMelee >= 1 && BuffAvatarUp && TalentUnstoppableForce) {
                        Aimsharp.Cast("Thunder Clap");
                        return true;
                    }
                }

                //actions.st+=/shield_block,if=cooldown.shield_slam.ready&buff.shield_block.down
                if (Aimsharp.CanCast("Shield Block", "player") && CDShieldSlamReady && !BuffShieldBlockUp) {
                    Aimsharp.Cast("Shield Block");
                    return true;
                }

                //actions.st+=/shield_slam,if=buff.shield_block.up
                if (Aimsharp.CanCast("Shield Slam") && BuffShieldBlockUp) {
                    Aimsharp.Cast("Shield Slam");
                    return true;
                }

                //actions.st+=/thunder_clap,if=(talent.unstoppable_force.enabled&buff.avatar.up)
                if (Aimsharp.CanCast("Thunder Clap", "player")) {
                    if (BuffAvatarUp && TalentUnstoppableForce) {
                        Aimsharp.Cast("Thunder Clap");
                        return true;
                    }
                }

                //actions.st+=/demoralizing_shout,if=talent.booming_voice.enabled
                if (Aimsharp.CanCast("Demoralizing Shout", "player") && TalentBoomingVoice) {
                    Aimsharp.Cast("Demoralizing Shout");
                    return true;
                }

                //actions.st+=/shield_slam
                if (Aimsharp.CanCast("Shield Slam")) {
                    Aimsharp.Cast("Shield Slam");
                    return true;
                }
				
				//actions.st+=/shockwave
				if (Aimsharp.CanCast("Shockwave", "player") && CastingRemaining < KickValue && (IsChannelingTar || IsInterruptable || !IsInterruptable)) {
                       Aimsharp.Cast("Shockwave");
                       return true;
				}

                //actions.st+=/dragon_roar
                if (Aimsharp.CanCast("Dragon Roar", "player")) {
                    Aimsharp.Cast("Dragon Roar");
                    return true;
                }

                if (Aimsharp.CanCast("Thunder Clap", "player") && !Aimsharp.CanCast("Shield Slam")) {
                    Aimsharp.Cast("Thunder Clap");
                    return true;
                }

                if (Aimsharp.CanCast("Revenge", "player") && (UseOnlyFreeRevenge && Aimsharp.HasBuff("Revenge!") || !UseOnlyFreeRevenge) && Rage >= 70) {
                    Aimsharp.Cast("Revenge");
                    return true;
                }

                if (Aimsharp.CanCast("Ravager", "player")) {
                    Aimsharp.Cast("RavageSelf");
                    return true;
                }
				
				if ((Rage >= 20 && TargetHealth <= 20 ) || (CovenantVenthyr && TargetHealth >= 80 )) {
					
					if (Aimsharp.CanCast("Execute")) {
								Aimsharp.Cast("Execute");
								return true;
					}
				}

                if (Aimsharp.CanCast("Devastate")) {
                    Aimsharp.Cast("Devastate");
                    return true;
                }
                #endregion

            }

            return false;
        }
        


        public override bool OutOfCombatTick()
        {	
			if (Aimsharp.CanCast("Battle Shout", "player") && !Aimsharp.HasBuff("Battle Shout", "player", false)) {
				Aimsharp.Cast("Battle Shout");
				return true;
			}
            return false;
        }

    }
}
