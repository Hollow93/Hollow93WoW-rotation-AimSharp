using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class BeastMastery : Rotation
    {
        List<string> Racials = new List<string>
        {
            "Blood Fury","Berserking","Fireblood","Ancestral Call","Bag of Tricks"
        };

        List<string> CovenantAbilities = new List<string>
        {

        };

        List<string> BloodlustEffects = new List<string>
        {
            "Bloodlust","Heroism","Time Warp","Primal Rage","Drums of Rage"
        };

        List<string> GeneralBuffs = new List<string>
        {

        };

        List<string> GeneralDebuffs = new List<string>
        {

        };

        List<string> SpellsList = new List<string>
        {
           "Light's Judgment","Barbed Shot","Multi-Shot","Tar Trap","Flare","Death Chakram","Wild Spirits","Bestial Wrath","Resonating Arrow","Stampede","Flayed Shot","Kill Shot","Chimaera Shot","Bloodshed","A Murder of Crows","Barrage","Kill Command","Dire Beast","Cobra Shot","Freezing Trap","Arcane Pulse","Aspect of the Wild","Revive Pet"
        };

        List<string> BuffsList = new List<string>
        {
            "Wild Spirits","Aspect of the Wild","Bestial Wrath","Flayer's Mark","Nesingwary's Trapping Apparatus","Frenzy","Beast Cleave","Flamewaker's Cobra Sting"
        };

        List<string> DebuffsList = new List<string>
        {
           "Barbed Shot","Tar Trap"
        };

        List<string> TotemsList = new List<string>
        {

        };

        List<string> MacroCommands = new List<string>
        {
            "AOE","SaveCooldowns"
        };

        public override void LoadSettings()
        {
            Settings.Add(new Setting("General Settings"));
            Settings.Add(new Setting("Use Top Trinket:", false));
            Settings.Add(new Setting("Use Bottom Trinket:", false));
            Settings.Add(new Setting("Use DPS Potion:", false));
            Settings.Add(new Setting("Potion name:", "Potion of Unbridled Fury"));

            Settings.Add(new Setting("Hunter Settings"));
            Settings.Add(new Setting("Auto Revive Pet?", true));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Sephuzs Proclamation", "Soulforge Embers", "Nessingwarys Trapping Apparatus", "Qapla Eredun War Order", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Beast Mastery", Color.Purple);
            Aimsharp.PrintMessage("Version 2.1", Color.Purple);

            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

            Aimsharp.Latency = 120;
            Aimsharp.QuickDelay = 150;
            Aimsharp.SlowDelay = 350;

            foreach (string Spell in SpellsList)
            {
                Spellbook.Add(Spell);
            }

            foreach (string Spell in CovenantAbilities)
            {
                Spellbook.Add(Spell);
            }

            foreach (string Spell in Racials)
            {
                Spellbook.Add(Spell);
            }

            foreach (string Buff in GeneralBuffs)
            {
                Buffs.Add(Buff);
            }

            foreach (string Buff in BuffsList)
            {
                Buffs.Add(Buff);
            }

            foreach (string Buff in BloodlustEffects)
            {
                Buffs.Add(Buff);
            }

            foreach (string Debuff in DebuffsList)
            {
                Debuffs.Add(Debuff);
            }

            foreach (string Debuff in GeneralDebuffs)
            {
                Debuffs.Add(Debuff);
            }

            foreach (string Totem in TotemsList)
            {
                Totems.Add(Totem);
            }

            Items.Add(GetString("Potion name:"));

            Macros.Add("DPS Pot", "/use " + GetString("Potion name:"));
            Macros.Add("TopTrinket", "/use 13");
            Macros.Add("BottomTrinket", "/use 14");

            Macros.Add("TarTrapC", "/cast [@cursor] Tar Trap");
            Macros.Add("FlareC", "/cast [@cursor] Flare");
            Macros.Add("WildSpiritsC", "/cast [@cursor] Wild Spirits");
            Macros.Add("ResonatingArrowC", "/cast [@cursor] Resonating Arrow");
            Macros.Add("FreezingTrapC", "/cast [@cursor] Freezing Trap");


            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            //CustomFunctions.Add("RuneforgeFallenCrusader", "if select(1,GetWeaponEnchantInfo()) then if select(4,GetWeaponEnchantInfo()) == 3368 then return 1 end end if select(5,GetWeaponEnchantInfo()) then if select(8,GetWeaponEnchantInfo()) == 3368 then return 1 end end return 0");
            //CustomFunctions.Add("RuneforgeRazorice", "if select(1,GetWeaponEnchantInfo()) then if select(4,GetWeaponEnchantInfo()) == 3370 then return 1 end end if select(5,GetWeaponEnchantInfo()) then if select(8,GetWeaponEnchantInfo()) == 3370 then return 1 end end return 0");
            //CustomFunctions.Add("Revolving Blades Rank", "local isSelected\nlocal count = 0\nfor _, itemLocation in AzeriteUtil.EnumerateEquipedAzeriteEmpoweredItems() do\nisSelected = C_AzeriteEmpoweredItem.IsPowerSelected(itemLocation, 126)\nif isSelected then count = count + 1 end\nend\nreturn count");
            //CustomFunctions.Add("Chaotic Transformation Rank", "local isSelected\nlocal count = 0\nfor _, itemLocation in AzeriteUtil.EnumerateEquipedAzeriteEmpoweredItems() do\nisSelected = C_AzeriteEmpoweredItem.IsPowerSelected(itemLocation, 220)\nif isSelected then count = count + 1 end\nend\nreturn count");
        }


        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick()
        {
            #region Simc Conditionals

            // Aimsharp Settings
            bool UsePotion = GetCheckBox("Use DPS Potion:");
            bool UseTopTrinket = GetCheckBox("Use Top Trinket:");
            bool UseBottomTrinket = GetCheckBox("Use Bottom Trinket:");
            string PotionName = GetString("Potion name:");
            string RuneforgePower = GetDropDown("Legendary power equipped:");

            // Custom commands
            bool SaveCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            bool AOE = Aimsharp.IsCustomCodeOn("AOE");

            // Base simc 
            int Time = Aimsharp.CombatTime();
            bool Fighting = Aimsharp.Range("target") <= 45 && Aimsharp.TargetIsEnemy();
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int RangeToTarget = Aimsharp.Range();
            List<int> ActiveConduits = Aimsharp.GetActiveConduits();

            if (!AOE)
            {
                EnemiesNearTarget = 1;
                EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            }
            float Haste = Aimsharp.Haste() / 100f;
            int GCD = Aimsharp.GCD();
            int GCDMAX = (int)(1500f / (Haste + 1f));
            int TargetTimeToDie = 1000000; //need to implement time to die estimation
            int TargetHealthPct = Aimsharp.Health("target");
            int PlayerLevel = Aimsharp.GetPlayerLevel();
            bool Moving = Aimsharp.PlayerIsMoving();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            int PlayerCastingID = Aimsharp.CastingID("player");
            string LastCast = Aimsharp.LastCast();
            int CovenantID = Aimsharp.CovenantID();
            bool CovenantKyrian = CovenantID == 1;
            bool CovenantVenthyr = CovenantID == 2;
            bool CovenantNightFae = CovenantID == 3;
            bool CovenantNecrolord = CovenantID == 4;

            // General buffs
            bool BloodlustUp = false;
            foreach (string BloodlustEffect in BloodlustEffects)
            {
                if (Aimsharp.HasBuff(BloodlustEffect))
                {
                    BloodlustUp = true;
                    break;
                }
            }


            //power
            int Focus = Aimsharp.Power("player");
            int FocusMax = Aimsharp.PlayerMaxPower();
            int FocusDefecit = FocusMax - Focus;

            //Talents
            bool TalentKillerInstinct = Aimsharp.Talent(1, 1);
            bool TalentScentofBlood = Aimsharp.Talent(2, 1);


            //buffs
            int BuffWildSpiritsRemains = Aimsharp.BuffRemaining("Wild Spirits") - GCD;
            bool BuffWildSpiritsUp = BuffWildSpiritsRemains > 0;
            int BuffAspectoftheWildRemains = Aimsharp.BuffRemaining("Aspect of the Wild") - GCD;
            bool BuffAspectoftheWildUp = BuffAspectoftheWildRemains > 0;
            int BuffBestialWrathRemains = Aimsharp.BuffRemaining("Bestial Wrath") - GCD;
            bool BuffBestialWrathUp = BuffBestialWrathRemains > 0;
            int BuffFlayersMarkRemains = Aimsharp.BuffRemaining("Flayer's Mark") - GCD;
            bool BuffFlayersMarkUp = BuffFlayersMarkRemains > 0;
            int BuffNesingwarysTrappingApparatusRemains = Aimsharp.BuffRemaining("Nesingwary's Trapping Apparatus") - GCD;
            bool BuffNesingwarysTrappingApparatusUp = BuffNesingwarysTrappingApparatusRemains > 0;
            int BuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy") - GCD;
            bool BuffFrenzyUp = BuffFrenzyRemains > 0;
            int BuffBeastCleaveRemains = Aimsharp.BuffRemaining("Beast Cleave") - GCD;
            bool BuffBeastCleaveUp = BuffBeastCleaveRemains > 0;
            int PetMainBuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy", "pet") - GCD;
            bool PetMainBuffFrenzyUp = PetMainBuffFrenzyRemains > 0;
            int BuffFlamewakersCobraStingRemains = Aimsharp.BuffRemaining("Flamewaker's Cobra Sting") - GCD;
            bool BuffFlamewakersCobraStingUp = BuffFlamewakersCobraStingRemains > 0;
            int PetMainBuffBeastCleaveRemains = Aimsharp.BuffRemaining("Beast Cleave", "pet") - GCD;
            bool PetMainBuffBeastCleaveUp = PetMainBuffBeastCleaveRemains > 0;


            //debuffs
            int DotBarbedShotRemains = Aimsharp.DebuffRemaining("Barbed Shot") - GCD;
            bool DotBarbedShotUp = DotBarbedShotRemains > 0;
            int DebuffTarTrapRemains = Aimsharp.DebuffRemaining("Tar Trap", "target", false) - GCD;
            bool DebuffTarTrapUp = DebuffTarTrapRemains > 0;


            //cooldowns
            int CDBarbedShotCharges = Aimsharp.SpellCharges("Barbed Shot");
            int CDBarbedShotFullRecharge = (int)(Aimsharp.RechargeTime("Barbed Shot") + 0f * (Aimsharp.MaxCharges("Barbed Shot") - CDBarbedShotCharges - 1) / (1f + Haste));
            float CDBarbedShotFractional = CDBarbedShotCharges + (1 - (Aimsharp.RechargeTime("Barbed Shot") - GCD) / (0f / (1f + Haste)));
            CDBarbedShotFractional = CDBarbedShotFractional > Aimsharp.MaxCharges("Barbed Shot") ? Aimsharp.MaxCharges("Barbed Shot") : CDBarbedShotFractional;
            int CDFlareRemains = Aimsharp.SpellCooldown("Flare");
            bool CDFlareUp = CDFlareRemains <= 0;
            int CDWildSpiritsRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Wild Spirits");
            bool CDWildSpiritsUp = CDWildSpiritsRemains <= 0;
            int CDBestialWrathRemains = Aimsharp.SpellCooldown("Bestial Wrath");
            bool CDBestialWrathUp = CDBestialWrathRemains <= 0;
            int CDKillCommandRemains = Aimsharp.SpellCooldown("Kill Command");
            bool CDKillCommandUp = CDKillCommandRemains <= 0;


            //specific variables
            bool RuneforgeSephuzsProclamation = RuneforgePower == "Sephuzs Proclamation";
            bool RuneforgeSoulforgeEmbers = RuneforgePower == "Soulforge Embers";
            bool RuneforgeNessingwarysTrappingApparatus = RuneforgePower == "Nessingwarys Trapping Apparatus";
            bool RuneforgeQaplaEredunWarOrder = RuneforgePower == "Qapla Eredun War Order";
            bool ConduitNiyasToolsPoison = ActiveConduits.Contains(320660);
            bool ConduitReversalofFortune = ActiveConduits.Contains(339495);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastLightsJudgment = Aimsharp.CanCast("Light's Judgment", "player") && !SaveCooldowns && Fighting;
            bool CanCastBarbedShot = Aimsharp.CanCast("Barbed Shot") && Fighting;
            bool CanCastMultiShot = Aimsharp.CanCast("Multi-Shot") && Fighting;
            bool CanCastTarTrap = Aimsharp.CanCast("Tar Trap", "player") && Fighting;
            bool CanCastFlare = Aimsharp.CanCast("Flare", "player") && Fighting;
            bool CanCastDeathChakram = Aimsharp.CanCast("Death Chakram") && Fighting;
            bool CanCastWildSpirits = Aimsharp.CanCast("Wild Spirits", "player") && !SaveCooldowns && Fighting;
            bool CanCastBestialWrath = Aimsharp.CanCast("Bestial Wrath", "player") && Fighting;
            bool CanCastResonatingArrow = Aimsharp.CanCast("Resonating Arrow", "player") && !SaveCooldowns && Fighting;
            bool CanCastStampede = Aimsharp.CanCast("Stampede", "player") && !SaveCooldowns && Fighting;
            bool CanCastFlayedShot = Aimsharp.CanCast("Flayed Shot") && Fighting;
            bool CanCastKillShot = Aimsharp.CanCast("Kill Shot") && Fighting;
            bool CanCastChimaeraShot = Aimsharp.CanCast("Chimaera Shot") && Fighting;
            bool CanCastBloodshed = Aimsharp.CanCast("Bloodshed") && Fighting;
            bool CanCastAMurderofCrows = Aimsharp.CanCast("A Murder of Crows") && Fighting;
            bool CanCastBarrage = Aimsharp.CanCast("Barrage", "player") && Fighting;
            bool CanCastKillCommand = Aimsharp.CanCast("Kill Command") && Fighting;
            bool CanCastDireBeast = Aimsharp.CanCast("Dire Beast") && Fighting;
            bool CanCastCobraShot = Aimsharp.CanCast("Cobra Shot") && Fighting;
            bool CanCastFreezingTrap = Aimsharp.CanCast("Freezing Trap", "player") && Fighting;
            bool CanCastArcanePulse = Aimsharp.CanCast("Arcane Pulse", "player") && !SaveCooldowns && Fighting;
            bool CanCastAspectoftheWild = Aimsharp.CanCast("Aspect of the Wild", "player") && !SaveCooldowns && Fighting;


            bool RevivePetEnabled = GetCheckBox("Auto Revive Pet?");
            int BarbedShotBuffCount = Aimsharp.BuffStacks("Barbed Shot");
            int BarbedShotCountForGCD = 0;
            for (int i = 0; i < BarbedShotBuffCount; i++)
            {
                if (Aimsharp.BuffInfoDetailed("player", "Barbed Shot", true)[i]["Remaining"] - GCD > GCDMAX)
                    BarbedShotCountForGCD++;
            }

            float FocusRegen = 10f * (1f + Haste) + BarbedShotCountForGCD * 2.5f;
            float FocusTimeToMax = (FocusMax - Focus) * 1000f / FocusRegen;
            int PlayerCasting = Aimsharp.CastingID("player");
            int PlayerCastRemaining = Aimsharp.CastingRemaining("player");

            bool ReviveExecuting = PlayerCasting == 982 ? true : false && PlayerCastRemaining > 200;
            int PetHealth = Aimsharp.Health("pet");

            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            if (RevivePetEnabled)
            {
                if (Aimsharp.CanCast("Revive Pet") && PetHealth < 1)
                {
                    if (!ReviveExecuting)
                    {
                        Aimsharp.Cast("Revive Pet");
                        return true;
                    }
                }
            }

            //actions+=/use_items

            if (Aimsharp.CanUseTrinket(0) && UseTopTrinket && Fighting && !SaveCooldowns)
            {
                Aimsharp.Cast("TopTrinket", true);
                return true;
            }

            if (Aimsharp.CanUseTrinket(1) && UseBottomTrinket && Fighting && !SaveCooldowns)
            {
                Aimsharp.Cast("BottomTrinket", true);
                return true;
            }


            //actions+=/call_action_list,name=cds
            //actions.cds+=/berserking,if=(buff.wild_spirits.up|!covenant.night_fae&buff.aspect_of_the_wild.up&buff.bestial_wrath.up)&(target.time_to_die>cooldown.berserking.duration+duration|(target.health.pct<35|!talent.killer_instinct))|target.time_to_die<13
            if ((BuffWildSpiritsUp || !CovenantNightFae && BuffAspectoftheWildUp && BuffBestialWrathUp) && (true || (TargetHealthPct < 35 || !TalentKillerInstinct)) || TargetTimeToDie < 13000)

                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }

            //actions.cds+=/lights_judgment
            if (CanCastLightsJudgment)
            {
                Aimsharp.Cast("Light's Judgment");
                return true;
            }

            //actions.cds+=/potion,if=buff.aspect_of_the_wild.up|target.time_to_die<26
            if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
            {
                if (BuffAspectoftheWildUp || TargetTimeToDie < 26000)
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //actions+=/call_action_list,name=st,if=active_enemies<2
            if (EnemiesNearTarget < 2)
            {
                //actions.st=aspect_of_the_wild
                if (CanCastAspectoftheWild)
                {
                    Aimsharp.Cast("Aspect of the Wild");
                    return true;
                }


                //actions.st+=/barbed_shot,if=pet.main.buff.frenzy.up&pet.main.buff.frenzy.remains<=gcd
                if (CanCastBarbedShot)
                {
                    if (PetMainBuffFrenzyUp && PetMainBuffFrenzyRemains <= GCDMAX) // add 100 gcd to safely overlap accounting for latency
                    {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }

                //actions.st+=/tar_trap,if=runeforge.soulforge_embers&tar_trap.remains<gcd&cooldown.flare.remains<gcd
                if (CanCastTarTrap)
                {
                    if (RuneforgeSoulforgeEmbers && DebuffTarTrapRemains < GCDMAX && CDFlareRemains < GCDMAX) //temp fix to detect tar trap, won't work if target is immune to snares?
                    {
                        Aimsharp.Cast("TarTrapC");
                        return true;
                    }
                }

                //actions.st+=/flare,if=tar_trap.up&runeforge.soulforge_embers
                if (CanCastFlare)
                {
                    if (DebuffTarTrapUp && RuneforgeSoulforgeEmbers) //temp fix to detect tar trap, won't work if target is immune to snares?
                    {
                        Aimsharp.Cast("FlareC");
                        return true;
                    }
                }

                //actions.st+=/bloodshed
                if (CanCastBloodshed)
                {
                    Aimsharp.Cast("Bloodshed");
                    return true;
                }

                //actions.st+=/wild_spirits
                if (CanCastWildSpirits)
                {
                    Aimsharp.Cast("WildSpiritsC");
                    return true;
                }

                //actions.st+=/flayed_shot
                if (CanCastFlayedShot)
                {
                    Aimsharp.Cast("Flayed Shot");
                    return true;
                }

                //actions.st+=/kill_shot,if=buff.flayers_mark.remains<5|target.health.pct<=20
                if (CanCastKillShot)
                {
                    if (BuffFlayersMarkRemains < 5000 || TargetHealthPct <= 20)
                    {
                        Aimsharp.Cast("Kill Shot");
                        return true;
                    }
                }

                //actions.st+=/barbed_shot,if=(cooldown.wild_spirits.remains>full_recharge_time|!covenant.night_fae)&(cooldown.bestial_wrath.remains<12*charges_fractional+gcd&talent.scent_of_blood|full_recharge_time<gcd&cooldown.bestial_wrath.remains)|target.time_to_die<9
                if (CanCastBarbedShot)
                {
                    if ((CDWildSpiritsRemains > CDBarbedShotFullRecharge || !CovenantNightFae) && (CDBestialWrathRemains < 12000 * CDBarbedShotFractional + GCDMAX && TalentScentofBlood || CDBarbedShotFullRecharge < GCDMAX && !CDBestialWrathUp) || TargetTimeToDie < 9000)
                    {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }

                //actions.st+=/death_chakram,if=focus+cast_regen<focus.max
                if (CanCastDeathChakram)
                {
                    if (Focus + 3 < FocusMax)
                    {
                        Aimsharp.Cast("Death Chakram");
                        return true;
                    }
                }

                //actions.st+=/stampede,if=buff.aspect_of_the_wild.up|target.time_to_die<15
                if (CanCastStampede)
                {
                    if (BuffAspectoftheWildUp || TargetTimeToDie < 15000)
                    {
                        Aimsharp.Cast("Stampede");
                        return true;
                    }
                }

                //actions.st+=/a_murder_of_crows
                if (CanCastAMurderofCrows)
                {
                    Aimsharp.Cast("A Murder of Crows");
                    return true;
                }

                //actions.st+=/resonating_arrow,if=buff.bestial_wrath.up|target.time_to_die<10
                if (CanCastResonatingArrow)
                {
                    if (BuffBestialWrathUp || TargetTimeToDie < 10000)
                    {
                        Aimsharp.Cast("ResonatingArrowC");
                        return true;
                    }
                }

                //actions.st+=/bestial_wrath,if=cooldown.wild_spirits.remains>15|!covenant.night_fae|target.time_to_die<15
                if (CanCastBestialWrath)
                {
                    if (CDWildSpiritsRemains > 15000 || !CovenantNightFae || TargetTimeToDie < 15000)
                    {
                        Aimsharp.Cast("Bestial Wrath");
                        return true;
                    }
                }

                //actions.st+=/chimaera_shot
                if (CanCastChimaeraShot)
                {
                    Aimsharp.Cast("Chimaera Shot");
                    return true;
                }

                //actions.st+=/kill_command
                if (CanCastKillCommand)
                {
                    Aimsharp.Cast("Kill Command");
                    return true;
                }

                //actions.st+=/dire_beast
                if (CanCastDireBeast)
                {
                    Aimsharp.Cast("Dire Beast");
                    return true;
                }

                //actions.st+=/cobra_shot,if=(focus-cost+focus.regen*(cooldown.kill_command.remains-1)>action.kill_command.cost|cooldown.kill_command.remains>1+gcd)|(buff.bestial_wrath.up|buff.nesingwarys_trapping_apparatus.up)&!runeforge.qapla_eredun_war_order|target.time_to_die<3
                if (CanCastCobraShot)
                {
                    if ((Focus - 35 + FocusRegen * ((CDKillCommandRemains - 1000) / 1000) > (BuffFlamewakersCobraStingUp ? 0 : 30) || CDKillCommandRemains > 1000 + GCDMAX) || (BuffBestialWrathUp || BuffNesingwarysTrappingApparatusUp) && !RuneforgeQaplaEredunWarOrder || TargetTimeToDie < 3000)
                    {
                        Aimsharp.Cast("Cobra Shot");
                        return true;
                    }
                }

                //actions.st+=/barbed_shot,if=buff.wild_spirits.up
                if (CanCastBarbedShot)
                {
                    if (BuffWildSpiritsUp)
                    {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }

                //actions.st+=/arcane_pulse,if=buff.bestial_wrath.down|target.time_to_die<5
                if (CanCastArcanePulse)
                {
                    if (!BuffBestialWrathUp || TargetTimeToDie < 5000)
                    {
                        Aimsharp.Cast("Arcane Pulse");
                        return true;
                    }
                }

                //actions.st+=/tar_trap,if=runeforge.soulforge_embers|runeforge.nessingwarys_trapping_apparatus
                if (CanCastTarTrap)
                {
                    if (RuneforgeSoulforgeEmbers || RuneforgeNessingwarysTrappingApparatus)
                    {
                        Aimsharp.Cast("TarTrapC");
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=cleave,if=active_enemies>1
            if (EnemiesNearTarget > 1)
            {
                //actions.cleave=aspect_of_the_wild
                if (CanCastAspectoftheWild)
                {
                    Aimsharp.Cast("Aspect of the Wild");
                    return true;
                }

                //actions.cleave+=/barbed_shot,target_if=min:dot.barbed_shot.remains,if=pet.main.buff.frenzy.up&pet.main.buff.frenzy.remains<=gcd
                if (CanCastBarbedShot)
                {
                    if (PetMainBuffFrenzyUp && PetMainBuffFrenzyRemains <= GCDMAX) // add 100 gcd to safely overlap accounting for latency
                    {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }

                //actions.cleave+=/multishot,if=gcd-pet.main.buff.beast_cleave.remains>0.25
                if (CanCastMultiShot)
                {
                    if (!PetMainBuffBeastCleaveUp)
                    {
                        Aimsharp.Cast("Multi-Shot");
                        return true;
                    }
                }

                //actions.cleave+=/tar_trap,if=runeforge.soulforge_embers&tar_trap.remains<gcd&cooldown.flare.remains<gcd
                if (CanCastTarTrap)
                {
                    if (RuneforgeSoulforgeEmbers && DebuffTarTrapRemains < GCDMAX && CDFlareRemains < GCDMAX) //temp fix to detect tar trap, won't work if target is immune to snares?
                    {
                        Aimsharp.Cast("TarTrapC");
                        return true;
                    }
                }

                //actions.cleave+=/flare,if=tar_trap.up&runeforge.soulforge_embers
                if (CanCastFlare)
                {
                    if (DebuffTarTrapUp && RuneforgeSoulforgeEmbers) //temp fix to detect tar trap, won't work if target is immune to snares?
                    {
                        Aimsharp.Cast("FlareC");
                        return true;
                    }
                }

                //actions.cleave+=/death_chakram,if=focus+cast_regen<focus.max
                if (CanCastDeathChakram)
                {
                    if (Focus + 21 < FocusMax)
                    {
                        Aimsharp.Cast("Death Chakram");
                        return true;
                    }
                }

                //actions.cleave+=/wild_spirits
                if (CanCastWildSpirits)
                {
                    Aimsharp.Cast("WildSpiritsC");
                    return true;
                }

                //actions.cleave+=/barbed_shot,target_if=min:dot.barbed_shot.remains,if=full_recharge_time<gcd&cooldown.bestial_wrath.remains|cooldown.bestial_wrath.remains<12+gcd&talent.scent_of_blood
                if (CanCastBarbedShot)
                {
                    if (CDBarbedShotFullRecharge < GCDMAX && CDBestialWrathRemains > 0 || CDBestialWrathRemains < 12000 + GCDMAX && TalentScentofBlood)
                    {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }

                //actions.cleave+=/bestial_wrath
                if (CanCastBestialWrath)
                {
                    Aimsharp.Cast("Bestial Wrath");
                    return true;
                }

                //actions.cleave+=/resonating_arrow
                if (CanCastResonatingArrow)
                {
                    Aimsharp.Cast("ResonatingArrowC");
                    return true;
                }

                //actions.cleave+=/stampede,if=buff.aspect_of_the_wild.up|target.time_to_die<15
                if (CanCastStampede)
                {
                    if (BuffAspectoftheWildUp || TargetTimeToDie < 15000)
                    {
                        Aimsharp.Cast("Stampede");
                        return true;
                    }
                }

                //actions.cleave+=/flayed_shot
                if (CanCastFlayedShot)
                {
                    Aimsharp.Cast("Flayed Shot");
                    return true;
                }

                //actions.cleave+=/kill_shot
                if (CanCastKillShot)
                {
                    Aimsharp.Cast("Kill Shot");
                    return true;
                }

                //actions.cleave+=/chimaera_shot
                if (CanCastChimaeraShot)
                {
                    Aimsharp.Cast("Chimaera Shot");
                    return true;
                }

                //actions.cleave+=/bloodshed
                if (CanCastBloodshed)
                {
                    Aimsharp.Cast("Bloodshed");
                    return true;
                }

                //actions.cleave+=/a_murder_of_crows
                if (CanCastAMurderofCrows)
                {
                    Aimsharp.Cast("A Murder of Crows");
                    return true;
                }

                //actions.cleave+=/barrage,if=pet.main.buff.frenzy.remains>execute_time
                if (CanCastBarrage)
                {
                    if (PetMainBuffFrenzyRemains > 3000/(1+Haste))
                    {
                        Aimsharp.Cast("Barrage");
                        return true;
                    }
                }

                //actions.cleave+=/kill_command,if=focus>cost+action.multishot.cost
                if (CanCastKillCommand)
                {
                    if (Focus> (BuffFlamewakersCobraStingUp ? 0 : 30) + 40)
                    {
                        Aimsharp.Cast("Kill Command");
                        return true;
                    }
                }

                //actions.cleave+=/dire_beast
                if (CanCastDireBeast)
                {
                    Aimsharp.Cast("Dire Beast");
                    return true;
                }

                //actions.cleave+=/barbed_shot,target_if=min:dot.barbed_shot.remains,if=target.time_to_die<9
                if (CanCastBarbedShot)
                {
                    if (TargetTimeToDie<9000)
                    {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }

                //actions.cleave+=/cobra_shot,if=focus.time_to_max<gcd*2
                if (CanCastCobraShot)
                {
                    if (FocusTimeToMax<GCDMAX * 2)
                    {
                        Aimsharp.Cast("Cobra Shot");
                        return true;
                    }
                }

                //actions.cleave+=/tar_trap,if=runeforge.soulforge_embers|runeforge.nessingwarys_trapping_apparatus
                if (CanCastTarTrap)
                {
                    if (RuneforgeSoulforgeEmbers || RuneforgeNessingwarysTrappingApparatus)
                    {
                        Aimsharp.Cast("TarTrapC");
                        return true;
                    }
                }

            }

            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

    }
}
