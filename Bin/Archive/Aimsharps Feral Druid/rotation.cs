using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsFeral : Rotation
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
           "Cat Form","Prowl","Ferocious Bite","Feral Frenzy","Rake","Moonfire","Thrash","Brutal Slash","Swipe","Shred","Incarnation: King of the Jungle","Tiger's Fury","Shadowmeld","Ravenous Frenzy","Convoke the Spirits","Adaptive Swarm","Primal Wrath","Rip","Savage Roar","Berserk"
        };

        List<string> BuffsList = new List<string>
        {
            "Cat Form","Prowl","Shadowmeld","Berserk","Sudden Ambush","Bloodtalons","Clearcasting","Incarnation: King of the Jungle","Apex Predator's Craving","Tiger's Fury","Savage Roar",
        };

        List<string> DebuffsList = new List<string>
        {
           "Rake","Rip","Thrash","Adaptive Swarm","Moonfire",
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

            Settings.Add(new Setting("Druid Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Feral", Color.Purple);
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



            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            //CustomFunctions.Add("RuneforgeFallenCrusader", "if select(1,GetWeaponEnchantInfo()) then if select(4,GetWeaponEnchantInfo()) == 3368 then return 1 end end if select(5,GetWeaponEnchantInfo()) then if select(8,GetWeaponEnchantInfo()) == 3368 then return 1 end end return 0");
            //CustomFunctions.Add("RuneforgeRazorice", "if select(1,GetWeaponEnchantInfo()) then if select(4,GetWeaponEnchantInfo()) == 3370 then return 1 end end if select(5,GetWeaponEnchantInfo()) then if select(8,GetWeaponEnchantInfo()) == 3370 then return 1 end end return 0");
            //CustomFunctions.Add("Revolving Blades Rank", "local isSelected\nlocal count = 0\nfor _, itemLocation in AzeriteUtil.EnumerateEquipedAzeriteEmpoweredItems() do\nisSelected = C_AzeriteEmpoweredItem.IsPowerSelected(itemLocation, 126)\nif isSelected then count = count + 1 end\nend\nreturn count");
            //CustomFunctions.Add("Chaotic Transformation Rank", "local isSelected\nlocal count = 0\nfor _, itemLocation in AzeriteUtil.EnumerateEquipedAzeriteEmpoweredItems() do\nisSelected = C_AzeriteEmpoweredItem.IsPowerSelected(itemLocation, 220)\nif isSelected then count = count + 1 end\nend\nreturn count");
        }


        Stopwatch BTRakeTimer = new Stopwatch();
        Stopwatch BTMoonfireTimer = new Stopwatch();
        Stopwatch BTThrashTimer = new Stopwatch();
        Stopwatch BTBrutalSlashTimer = new Stopwatch();
        Stopwatch BTSwipeTimer = new Stopwatch();
        Stopwatch BTShredTimer = new Stopwatch();


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
            bool Fighting = Aimsharp.Range("target") <= 10 && Aimsharp.TargetIsEnemy();
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
            int GCDMAX = 1000;
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
            int Energy = Aimsharp.Power("player");
            int ComboPoints = Aimsharp.PlayerSecondaryPower();
            int MaxEnergy = Aimsharp.PlayerMaxPower();
            int EnergyDefecit = MaxEnergy - Energy;


            //Talents
            bool TalentBloodtalons = Aimsharp.Talent(7, 2);
            bool TalentPredator = Aimsharp.Talent(1, 1);
            bool TalentPrimalWrath = Aimsharp.Talent(6, 3);
            bool TalentSabertooth = Aimsharp.Talent(1, 2);
            bool TalentLunarInspiration = Aimsharp.Talent(1, 3);
            bool TalentIncarnation = Aimsharp.Talent(5, 3);
            bool TalentBrutalSlash = Aimsharp.Talent(6, 2);


            //buffs
            bool BuffCatFormUp = Aimsharp.HasBuff("Cat Form");
            bool BuffProwlUp = Aimsharp.HasBuff("Prowl");
            bool BuffShadowmeldUp = Aimsharp.HasBuff("Shadowmeld");
            int BuffBerserkRemains = Aimsharp.BuffRemaining("Berserk") - GCD;
            bool BuffBerserkUp = BuffBerserkRemains > 0;
            int BuffSuddenAmbushRemains = Aimsharp.BuffRemaining("Sudden Ambush") - GCD;
            bool BuffSuddenAmbushUp = BuffSuddenAmbushRemains > 0;
            int BuffBloodtalonsRemains = Aimsharp.BuffRemaining("Bloodtalons") - GCD;
            bool BuffBloodtalonsUp = BuffBloodtalonsRemains > 0;
            int BuffClearcastingRemains = Aimsharp.BuffRemaining("Clearcasting") - GCD;
            bool BuffClearcastingUp = BuffClearcastingRemains > 0;
            int BuffIncarnationKingoftheJungleRemains = Aimsharp.BuffRemaining("Incarnation: King of the Jungle") - GCD;
            bool BuffIncarnationKingoftheJungleUp = BuffIncarnationKingoftheJungleRemains > 0;
            int BuffApexPredatorsCravingRemains = Aimsharp.BuffRemaining("Apex Predator's Craving") - GCD;
            bool BuffApexPredatorsCravingUp = BuffApexPredatorsCravingRemains > 0;
            int BuffTigersFuryRemains = Aimsharp.BuffRemaining("Tiger's Fury") - GCD;
            bool BuffTigersFuryUp = BuffTigersFuryRemains > 0;
            int BuffSavageRoarRemains = Aimsharp.BuffRemaining("Savage Roar") - GCD;
            bool BuffSavageRoarUp = BuffSavageRoarRemains > 0;


            //debuffs
            int DotRakeRemains = Aimsharp.DebuffRemaining("Rake") - GCD;
            bool DotRakeUp = DotRakeRemains > 0;
            bool DotRakeRefreshable = DotRakeRemains < 4500;
            int DotRipRemains = Aimsharp.DebuffRemaining("Rip") - GCD;
            bool DotRipUp = DotRipRemains > 0;
            bool DotRipRefreshable = DotRipRemains < 6000;
            int DotThrashRemains = Aimsharp.DebuffRemaining("Thrash") - GCD;
            bool DotThrashUp = DotThrashRemains > 0;
            bool DotThrashRefreshable = DotThrashRemains < 4500;
            int DotAdaptiveSwarmRemains = Aimsharp.DebuffRemaining("Adaptive Swarm") - GCD;
            bool DotAdaptiveSwarmUp = DotAdaptiveSwarmRemains > 0;
            int DotMoonfireRemains = Aimsharp.DebuffRemaining("Moonfire") - GCD;
            bool DotMoonfireUp = DotMoonfireRemains > 0;
            bool DotMoonfireRefreshable = DotMoonfireRemains < 4800;


            //cooldowns
            int CDBrutalSlashCharges = Aimsharp.SpellCharges("Brutal Slash");
            int CDBrutalSlashFullRecharge = (int)(Aimsharp.RechargeTime("Brutal Slash") + 0f * (Aimsharp.MaxCharges("Brutal Slash") - CDBrutalSlashCharges - 1) / (1f + Haste));
            float CDBrutalSlashFractional = CDBrutalSlashCharges + (1 - (Aimsharp.RechargeTime("Brutal Slash") - GCD) / (0f / (1f + Haste)));
            CDBrutalSlashFractional = CDBrutalSlashFractional > Aimsharp.MaxCharges("Brutal Slash") ? Aimsharp.MaxCharges("Brutal Slash") : CDBrutalSlashFractional;


            //specific variables
            bool ConduitDeepAllegiance = ActiveConduits.Contains(341378);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastCatForm = Aimsharp.CanCast("Cat Form", "player") && Fighting;
            bool CanCastProwl = Aimsharp.CanCast("Prowl", "player") && Fighting;
            bool CanCastFerociousBite = Aimsharp.CanCast("Ferocious Bite") && Fighting;
            bool CanCastFeralFrenzy = Aimsharp.CanCast("Feral Frenzy") && Fighting;
            bool CanCastRake = Aimsharp.CanCast("Rake") && Fighting;
            bool CanCastMoonfire = Aimsharp.CanCast("Moonfire") && Fighting;
            bool CanCastThrash = Aimsharp.CanCast("Thrash", "player") && Fighting;
            bool CanCastBrutalSlash = Aimsharp.CanCast("Brutal Slash", "player") && Fighting;
            bool CanCastSwipe = Aimsharp.CanCast("Swipe", "player") && Fighting && !TalentBrutalSlash;
            bool CanCastShred = Aimsharp.CanCast("Shred") && Fighting;
            bool CanCastIncarnationKingoftheJungle = Aimsharp.CanCast("Incarnation: King of the Jungle", "player") && !SaveCooldowns && Fighting;
            bool CanCastTigersFury = Aimsharp.CanCast("Tiger's Fury", "player") && Fighting;
            bool CanCastRavenousFrenzy = Aimsharp.CanCast("Ravenous Frenzy", "player") && !SaveCooldowns && Fighting;
            bool CanCastConvoketheSpirits = Aimsharp.CanCast("Convoke the Spirits", "player") && !SaveCooldowns && Fighting;
            bool CanCastAdaptiveSwarm = Aimsharp.CanCast("Adaptive Swarm") && Fighting;
            bool CanCastPrimalWrath = Aimsharp.CanCast("Primal Wrath", "player") && Fighting;
            bool CanCastRip = Aimsharp.CanCast("Rip") && Fighting;
            bool CanCastBerserk = Aimsharp.CanCast("Berserk", "player") && Fighting && !TalentIncarnation && !SaveCooldowns;
            bool CanCastSavageRoar = Aimsharp.CanCast("Savage Roar", "player") && Fighting;

            if (LastCast == "Rake" && !BTRakeTimer.IsRunning)
                BTRakeTimer.Restart();
            if (LastCast == "Moonfire" && !BTMoonfireTimer.IsRunning)
                BTMoonfireTimer.Restart();
            if (LastCast == "Thrash" && !BTThrashTimer.IsRunning)
                BTThrashTimer.Restart();
            if (LastCast == "Brutal Slash" && !BTBrutalSlashTimer.IsRunning)
                BTBrutalSlashTimer.Restart();
            if (LastCast == "Swipe" && !BTSwipeTimer.IsRunning)
                BTSwipeTimer.Restart();
            if (LastCast == "Shred" && !BTShredTimer.IsRunning)
                BTShredTimer.Restart();

            if (BTRakeTimer.ElapsedMilliseconds > 4000)
                BTRakeTimer.Reset();
            if (BTMoonfireTimer.ElapsedMilliseconds > 4000)
                BTMoonfireTimer.Reset();
            if (BTThrashTimer.ElapsedMilliseconds > 4000)
                BTThrashTimer.Reset();
            if (BTBrutalSlashTimer.ElapsedMilliseconds > 4000)
                BTBrutalSlashTimer.Reset();
            if (BTSwipeTimer.ElapsedMilliseconds > 4000)
                BTSwipeTimer.Reset();
            if (BTShredTimer.ElapsedMilliseconds > 4000)
                BTShredTimer.Reset();

            bool BuffBTRakeUp = BTRakeTimer.IsRunning && BTRakeTimer.ElapsedMilliseconds + GCD < 4000;
            bool BuffBTMoonfireUp = BTMoonfireTimer.IsRunning && BTMoonfireTimer.ElapsedMilliseconds + GCD < 4000;
            bool BuffBTThrashUp = BTThrashTimer.IsRunning && BTThrashTimer.ElapsedMilliseconds + GCD < 4000;
            bool BuffBTBrutalSlashUp = BTBrutalSlashTimer.IsRunning && BTBrutalSlashTimer.ElapsedMilliseconds + GCD < 4000;
            bool BuffBTSwipeUp = BTSwipeTimer.IsRunning && BTSwipeTimer.ElapsedMilliseconds + GCD < 4000;
            bool BuffBTShredUp = BTShredTimer.IsRunning && BTShredTimer.ElapsedMilliseconds + GCD < 4000;
            int BuffBsIncRemains = Math.Max(BuffBerserkRemains, BuffIncarnationKingoftheJungleRemains);
            bool BuffBsIncUp = BuffBsIncRemains - GCD > 0;

            int ActiveBTTriggers = 0;
            if (BuffBTRakeUp)
                ActiveBTTriggers++;
            if (BuffBTMoonfireUp)
                ActiveBTTriggers++;
            if (BuffBTThrashUp)
                ActiveBTTriggers++;
            if (BuffBTBrutalSlashUp)
                ActiveBTTriggers++;
            if (BuffBTSwipeUp)
                ActiveBTTriggers++;
            if (BuffBTShredUp)
                ActiveBTTriggers++;

            float EnergyRegen = 10f * (1f + Haste) * (BuffSavageRoarRemains + GCD > 1000 ? 1.1f : 1f);
            int TimeUntilMaxEnergy = (int)((EnergyDefecit * 1000f) / EnergyRegen);



            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions=tigers_fury,if=buff.cat_form.down
            if (CanCastTigersFury)
            {
                if (!BuffCatFormUp)
                {
                    Aimsharp.Cast("Tiger's Fury");
                    return true;
                }
            }

            //actions+=/cat_form,if=buff.cat_form.down
            if (CanCastCatForm)
            {
                if (!BuffCatFormUp)
                {
                    Aimsharp.Cast("Cat Form");
                    return true;
                }
            }

            //actions+=/prowl
            if (CanCastProwl)
            {
                Aimsharp.Cast("Prowl");
                return true;
            }

            //actions+=/run_action_list,name=stealth,if=buff.shadowmeld.up|buff.prowl.up
            if (BuffShadowmeldUp || BuffProwlUp)
            {
                //actions.stealth=run_action_list,name=bloodtalons,if=talent.bloodtalons.enabled&buff.bloodtalons.down
                if (TalentBloodtalons && !BuffBloodtalonsUp)
                {
                    //actions.bloodtalons=rake,target_if=(!ticking|(refreshable&persistent_multiplier>dot.rake.pmultiplier))&buff.bt_rake.down&druid.rake.ticks_gained_on_refresh>=2
                    if (CanCastRake)
                    {
                        if ((!DotRakeUp || DotRakeRefreshable) && !BuffBTRakeUp)
                        {
                            Aimsharp.Cast("Rake");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/lunar_inspiration,target_if=refreshable&buff.bt_moonfire.down
                    if (CanCastMoonfire && BuffCatFormUp && TalentLunarInspiration)
                    {
                        if (DotMoonfireRefreshable && !BuffBTMoonfireUp)
                        {
                            Aimsharp.Cast("Moonfire");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/thrash_cat,target_if=refreshable&buff.bt_thrash.down&druid.thrash_cat.ticks_gained_on_refresh>8
                    if (CanCastThrash && BuffCatFormUp)
                    {
                        if (DotThrashRefreshable && !BuffBTThrashUp)
                        {
                            Aimsharp.Cast("Thrash");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/brutal_slash,if=buff.bt_brutal_slash.down
                    if (CanCastBrutalSlash)
                    {
                        if (!BuffBTBrutalSlashUp)
                        {
                            Aimsharp.Cast("Brutal Slash");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/swipe_cat,if=buff.bt_swipe.down&spell_targets.swipe_cat>1
                    if (CanCastSwipe && BuffCatFormUp)
                    {
                        if (!BuffBTSwipeUp && EnemiesInMelee > 1)
                        {
                            Aimsharp.Cast("Swipe");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/shred,if=buff.bt_shred.down
                    if (CanCastShred)
                    {
                        if (!BuffBTShredUp)
                        {
                            Aimsharp.Cast("Shred");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/swipe_cat,if=buff.bt_swipe.down
                    if (CanCastSwipe && BuffCatFormUp)
                    {
                        if (!BuffBTSwipeUp)
                        {
                            Aimsharp.Cast("Swipe");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/thrash_cat,if=buff.bt_thrash.down
                    if (CanCastThrash && BuffCatFormUp)
                    {
                        if (!BuffBTThrashUp)
                        {
                            Aimsharp.Cast("Thrash");
                            return true;
                        }
                    }

                    return false;
                }

                //actions.stealth+=/rake,target_if=dot.rake.pmultiplier<1.5&druid.rake.ticks_gained_on_refresh>2
                if (CanCastRake)
                {
                    Aimsharp.Cast("Rake");
                    return true;
                }

                //actions.stealth+=/shred
                if (CanCastShred)
                {
                    Aimsharp.Cast("Shred");
                    return true;
                }

                return false;
            }

            //actions+=/call_action_list,name=cooldown
            //actions.cooldown=berserk
            if (CanCastBerserk)
            {
                Aimsharp.Cast("Berserk", true);
                return true;
            }

            //actions.cooldown+=/incarnation
            if (CanCastIncarnationKingoftheJungle)
            {
                Aimsharp.Cast("Incarnation: King of the Jungle", true);
                return true;
            }

            //actions.cooldown+=/tigers_fury,if=energy.deficit>55|(buff.bs_inc.up&buff.bs_inc.remains<13)|(talent.predator.enabled&variable.shortest_ttd<3)
            if (CanCastTigersFury)
            {
                if (EnergyDefecit > 55 || (BuffBsIncUp && BuffBsIncRemains < 13000) || (TalentPredator && TargetTimeToDie < 3000))
                {
                    Aimsharp.Cast("Tiger's Fury");
                    return true;
                }
            }

            //actions.cooldown+=/berserking,if=buff.tigers_fury.up|buff.bs_inc.up
            if (BuffTigersFuryUp || BuffBsIncUp)
            {
                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }
            }

            if (BuffBsIncUp)
            {
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //actions.cooldown+=/ravenous_frenzy,if=buff.bs_inc.up|fight_remains<21
            if (CanCastRavenousFrenzy)
            {
                if (BuffBsIncUp || TargetTimeToDie < 21000)
                {
                    Aimsharp.Cast("Ravenous Frenzy");
                    return true;
                }
            }

            //actions.cooldown+=/convoke_the_spirits,if=(dot.rip.remains>4&(buff.tigers_fury.down|buff.tigers_fury.remains<4)&combo_points=0&dot.thrash_cat.ticking&dot.rake.ticking)|fight_remains<5
            if (CanCastConvoketheSpirits)
            {
                if ((DotRipRemains > 4000 && (!BuffTigersFuryUp || BuffTigersFuryRemains < 4000) && ComboPoints == 0 && DotThrashUp && DotRakeUp) || TargetTimeToDie < 5000)
                {
                    Aimsharp.Cast("Convoke the Spirits");
                    return true;
                }
            }

            //actions.cooldown+=/adaptive_swarm,target_if=max:time_to_die*(combo_points=5&!dot.adaptive_swarm_damage.ticking)
            if (CanCastAdaptiveSwarm)
            {
                Aimsharp.Cast("Adaptive Swarm");
                return true;
            }

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

            //actions+=/run_action_list,name=finisher,if=combo_points>=(5-variable.4cp_bite)
            if (ComboPoints >= 5)
            {
                //actions.finisher=savage_roar,if=buff.savage_roar.down|buff.savage_roar.remains<(combo_points*6+1)*0.3
                if (CanCastSavageRoar)
                {
                    if (!BuffSavageRoarUp || BuffSavageRoarRemains < (ComboPoints * 6000 + 1000) * .3)
                    {
                        Aimsharp.Cast("Savage Roar");
                        return true;
                    }
                }

                //actions.finisher+=/primal_wrath,if=druid.primal_wrath.ticks_gained_on_refresh>(variable.rip_ticks>?variable.best_rip)|spell_targets.primal_wrath>(3+1*talent.sabertooth.enabled)
                if (CanCastPrimalWrath)
                {
                    if (!DotRipUp || (DotRipRemains + ComboPoints * (TalentSabertooth ? 1000 : 0)) < 6000 || EnemiesInMelee > (3 + (TalentSabertooth ? 1 : 0)))
                    {
                        if (EnemiesInMelee > 1)
                        {
                            Aimsharp.Cast("Primal Wrath");
                            return true;
                        }
                    }
                }

                //actions.finisher+=/rip,target_if=(!ticking|(remains+combo_points*talent.sabertooth.enabled)<duration*0.3|dot.rip.pmultiplier<persistent_multiplier)&druid.rip.ticks_gained_on_refresh>variable.rip_ticks
                if (CanCastRip)
                {
                    if (!DotRipUp || (DotRipRemains + ComboPoints * (TalentSabertooth ? 1000 : 0)) < 6000)
                    {
                        Aimsharp.Cast("Rip");
                        return true;
                    }
                }

                if (CanCastFerociousBite)
                {
                    if (Energy >= 50)
                    {
                        Aimsharp.Cast("Ferocious Bite");
                        return true;
                    }
                }

                return false;
            }

            //actions+=/run_action_list,name=stealth,if=buff.bs_inc.up|buff.sudden_ambush.up
            if (BuffBsIncUp || BuffSuddenAmbushUp)
            {
                //actions.stealth=run_action_list,name=bloodtalons,if=talent.bloodtalons.enabled&buff.bloodtalons.down
                if (TalentBloodtalons && !BuffBloodtalonsUp)
                {
                    //actions.bloodtalons=rake,target_if=(!ticking|(refreshable&persistent_multiplier>dot.rake.pmultiplier))&buff.bt_rake.down&druid.rake.ticks_gained_on_refresh>=2
                    if (CanCastRake)
                    {
                        if ((!DotRakeUp || DotRakeRefreshable) && !BuffBTRakeUp)
                        {
                            Aimsharp.Cast("Rake");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/lunar_inspiration,target_if=refreshable&buff.bt_moonfire.down
                    if (CanCastMoonfire && BuffCatFormUp && TalentLunarInspiration)
                    {
                        if (DotMoonfireRefreshable && !BuffBTMoonfireUp)
                        {
                            Aimsharp.Cast("Moonfire");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/thrash_cat,target_if=refreshable&buff.bt_thrash.down&druid.thrash_cat.ticks_gained_on_refresh>8
                    if (CanCastThrash && BuffCatFormUp)
                    {
                        if (DotThrashRefreshable && !BuffBTThrashUp)
                        {
                            Aimsharp.Cast("Thrash");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/brutal_slash,if=buff.bt_brutal_slash.down
                    if (CanCastBrutalSlash)
                    {
                        if (!BuffBTBrutalSlashUp)
                        {
                            Aimsharp.Cast("Brutal Slash");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/swipe_cat,if=buff.bt_swipe.down&spell_targets.swipe_cat>1
                    if (CanCastSwipe && BuffCatFormUp)
                    {
                        if (!BuffBTSwipeUp && EnemiesInMelee > 1)
                        {
                            Aimsharp.Cast("Swipe");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/shred,if=buff.bt_shred.down
                    if (CanCastShred)
                    {
                        if (!BuffBTShredUp)
                        {
                            Aimsharp.Cast("Shred");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/swipe_cat,if=buff.bt_swipe.down
                    if (CanCastSwipe && BuffCatFormUp)
                    {
                        if (!BuffBTSwipeUp)
                        {
                            Aimsharp.Cast("Swipe");
                            return true;
                        }
                    }

                    //actions.bloodtalons+=/thrash_cat,if=buff.bt_thrash.down
                    if (CanCastThrash && BuffCatFormUp)
                    {
                        if (!BuffBTThrashUp)
                        {
                            Aimsharp.Cast("Thrash");
                            return true;
                        }
                    }

                    return false;
                }

                //actions.stealth+=/rake,target_if=dot.rake.pmultiplier<1.5&druid.rake.ticks_gained_on_refresh>2
                if (CanCastRake)
                {
                    Aimsharp.Cast("Rake");
                    return true;
                }

                //actions.stealth+=/shred
                if (CanCastShred)
                {
                    Aimsharp.Cast("Shred");
                    return true;
                }

                return false;
            }

            //actions+=/pool_resource,if=talent.bloodtalons.enabled&buff.bloodtalons.down&(energy+3.5*energy.regen+(40*buff.clearcasting.up))<(115-23*buff.incarnation_king_of_the_jungle.up)&active_bt_triggers=0
            if (TalentBloodtalons && !BuffBloodtalonsUp && (Energy + 3.5 * EnergyRegen + (40 * (BuffClearcastingUp ? 1 : 0))) < (115 - 23 * (BuffIncarnationKingoftheJungleUp ? 1 : 0)) && ActiveBTTriggers == 0)
            {
                return false;
            }

            //actions+=/run_action_list,name=bloodtalons,if=talent.bloodtalons.enabled&(buff.bloodtalons.down|active_bt_triggers=2)
            if (TalentBloodtalons && (!BuffBloodtalonsUp || ActiveBTTriggers == 2))
            {
                //actions.bloodtalons=rake,target_if=(!ticking|(refreshable&persistent_multiplier>dot.rake.pmultiplier))&buff.bt_rake.down&druid.rake.ticks_gained_on_refresh>=2
                if (CanCastRake)
                {
                    if ((!DotRakeUp || DotRakeRefreshable) && !BuffBTRakeUp)
                    {
                        Aimsharp.Cast("Rake");
                        return true;
                    }
                }

                //actions.bloodtalons+=/lunar_inspiration,target_if=refreshable&buff.bt_moonfire.down
                if (CanCastMoonfire && BuffCatFormUp && TalentLunarInspiration)
                {
                    if (DotMoonfireRefreshable && !BuffBTMoonfireUp)
                    {
                        Aimsharp.Cast("Moonfire");
                        return true;
                    }
                }

                //actions.bloodtalons+=/thrash_cat,target_if=refreshable&buff.bt_thrash.down&druid.thrash_cat.ticks_gained_on_refresh>8
                if (CanCastThrash && BuffCatFormUp)
                {
                    if (DotThrashRefreshable && !BuffBTThrashUp)
                    {
                        Aimsharp.Cast("Thrash");
                        return true;
                    }
                }

                //actions.bloodtalons+=/brutal_slash,if=buff.bt_brutal_slash.down
                if (CanCastBrutalSlash)
                {
                    if (!BuffBTBrutalSlashUp)
                    {
                        Aimsharp.Cast("Brutal Slash");
                        return true;
                    }
                }

                //actions.bloodtalons+=/swipe_cat,if=buff.bt_swipe.down&spell_targets.swipe_cat>1
                if (CanCastSwipe && BuffCatFormUp)
                {
                    if (!BuffBTSwipeUp && EnemiesInMelee > 1)
                    {
                        Aimsharp.Cast("Swipe");
                        return true;
                    }
                }

                //actions.bloodtalons+=/shred,if=buff.bt_shred.down
                if (CanCastShred)
                {
                    if (!BuffBTShredUp)
                    {
                        Aimsharp.Cast("Shred");
                        return true;
                    }
                }

                //actions.bloodtalons+=/swipe_cat,if=buff.bt_swipe.down
                if (CanCastSwipe && BuffCatFormUp)
                {
                    if (!BuffBTSwipeUp)
                    {
                        Aimsharp.Cast("Swipe");
                        return true;
                    }
                }

                //actions.bloodtalons+=/thrash_cat,if=buff.bt_thrash.down
                if (CanCastThrash && BuffCatFormUp)
                {
                    if (!BuffBTThrashUp)
                    {
                        Aimsharp.Cast("Thrash");
                        return true;
                    }
                }

                return false;
            }

            //actions+=/ferocious_bite,target_if=max:target.time_to_die,if=buff.apex_predators_craving.up&(!talent.bloodtalons.enabled|buff.bloodtalons.up)
            if (CanCastFerociousBite)
            {
                if (BuffApexPredatorsCravingUp && (!TalentBloodtalons || BuffBloodtalonsUp))
                {
                    Aimsharp.Cast("Ferocious Bite");
                    return true;
                }
            }

            //actions+=/feral_frenzy,if=combo_points=0
            if (CanCastFeralFrenzy)
            {
                if (ComboPoints == 0)
                {
                    Aimsharp.Cast("Feral Frenzy");
                    return true;
                }
            }

            //actions+=/pool_resource,for_next=1
            if (DotRakeRefreshable && !CanCastRake && Aimsharp.TargetIsEnemy())
            {
                return false;
            }

            //actions+=/rake,target_if=(refreshable|persistent_multiplier>dot.rake.pmultiplier)&druid.rake.ticks_gained_on_refresh>spell_targets.swipe_cat*2-2
            if (CanCastRake)
            {
                if (DotRakeRefreshable)
                {
                    Aimsharp.Cast("Rake");
                    return true;
                }
            }

            //actions+=/moonfire_cat,target_if=refreshable&druid.moonfire.ticks_gained_on_refresh>spell_targets.swipe_cat*2-2
            if (CanCastMoonfire && BuffCatFormUp && TalentLunarInspiration)
            {
                if (DotMoonfireRefreshable)
                {
                    Aimsharp.Cast("Moonfire");
                    return true;
                }
            }

            //actions+=/pool_resource,for_next=1
            if (DotThrashRefreshable && !CanCastThrash && Aimsharp.TargetIsEnemy())
            {
                return false;
            }

            //actions+=/thrash_cat,if=refreshable&druid.thrash_cat.ticks_gained_on_refresh>variable.thrash_ticks
            if (CanCastThrash && BuffCatFormUp)
            {
                if (DotThrashRefreshable)
                {
                    Aimsharp.Cast("Thrash");
                    return true;
                }
            }

            //actions+=/brutal_slash,if=(buff.tigers_fury.up&(raid_event.adds.in>(1+max_charges-charges_fractional)*recharge_time))&(spell_targets.brutal_slash*action.brutal_slash.damage%action.brutal_slash.cost)>(action.shred.damage%action.shred.cost)
            if (CanCastBrutalSlash)
            {
                if ((BuffTigersFuryUp) && (EnemiesInMelee > 1))
                {
                    Aimsharp.Cast("Brutal Slash");
                    return true;
                }
            }

            //actions+=/swipe_cat,if=spell_targets.swipe_cat>1
            if (CanCastSwipe && BuffCatFormUp)
            {
                if (EnemiesInMelee > 1)
                {
                    Aimsharp.Cast("Swipe");
                    return true;
                }
            }

            //actions+=/shred,if=buff.clearcasting.up
            if (CanCastShred)
            {
                if (BuffClearcastingUp)
                {
                    Aimsharp.Cast("Shred");
                    return true;
                }
            }

            //actions.filler+=/shred
            if (CanCastShred)
            {
                Aimsharp.Cast("Shred");
                return true;
            }

            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

    }
}
