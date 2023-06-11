using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class SLFrostMage : Rotation
    {
        List<string> Racials = new List<string>
        {
            "Blood Fury","Berserking","Fireblood","Ancestral Call","Bag of Tricks"
        };

        List<string> CovenantAbilities = new List<string>
        {
            "Mirrors of Torment","Deathborne","Radiant Spark","Shifting Power"
        };

        List<string> Essences = new List<string>
        {
            "Blood of the Enemy","Guardian of Azeroth","Focused Azerite Beam","Worldvein Resonance","Memory of Lucid Dreams"
        };

        List<string> EssencesTargeted = new List<string>
        {
            "Concentrated Flame","Reaping Flames","The Unbound Force"
        };

        List<string> BloodlustEffects = new List<string>
        {
            "Bloodlust","Heroism","Time Warp","Primal Rage","Drums of Rage"
        };

        List<string> GeneralBuffs = new List<string>
        {
            "Memory of Lucid Dreams"
        };

        List<string> GeneralDebuffs = new List<string>
        {

        };

        List<string> SpellsList = new List<string>
        {
            "Rune of Power","Icy Veins","Frozen Orb","Blizzard","Flurry","Ice Nova","Comet Storm","Ice Lance","Frost Nova","Fire Blast","Arcane Explosion","Ebonbolt","Frostbolt","Glacial Spike","Ray of Frost","Ice Floes"
        };

        List<string> BuffsList = new List<string>
        {
            "Rune of Power","Icy Veins","Brain Freeze","Fingers of Frost","Disciplinary Command","Freezing Winds","Expanded Potential","Freezing Rain","Ice Floes"
        };

        List<string> DebuffsList = new List<string>
        {
            "Winter's Chill","Frost Nova","Freeze","Frostbite","Mirrors of Torment"
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

            Settings.Add(new Setting("Mage Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Grisly Icicle", "Disciplinary Command", "Cold Front", "Freezing Winds", "Glacial Fragments" }, "None"));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Frost Mage", Color.Purple);
            Aimsharp.PrintMessage("Version 2.0 (Shadowlands Pre-Patch)", Color.Purple);

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

            foreach (string Spell in Essences)
            {
                Spellbook.Add(Spell);
            }

            foreach (string Spell in EssencesTargeted)
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

            Macros.Add("Blizzcursor", "/cast [@cursor] Blizzard");

            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            //CustomFunctions.Add("Cascading Calamity Rank", "local isSelected\nlocal count = 0\nfor _, itemLocation in AzeriteUtil.EnumerateEquipedAzeriteEmpoweredItems() do\nisSelected = C_AzeriteEmpoweredItem.IsPowerSelected(itemLocation, 230)\nif isSelected then count = count + 1 end\nend\nreturn count");
        }

        Stopwatch DisciplinaryCommandTimer = new Stopwatch();

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
            bool Fighting = Aimsharp.Range("target") <= 40 && Aimsharp.TargetIsEnemy();
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int RangeToTarget = Aimsharp.Range();

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
            int BuffMemoryOfLucidDreamsRemaining = Aimsharp.BuffRemaining("Memory of Lucid Dreams") - GCD;
            bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemaining > 0;

            // General Debuffs


            // General Cooldowns
            int ConcentratedFlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            int CDGuardianOfAzerothRemaining = Aimsharp.SpellCooldown("Guardian of Azeroth") - GCD;
            bool CDGuardianOfAzerothUp = CDGuardianOfAzerothRemaining <= 0;

            // Mage power
            int Mana = Aimsharp.Power("player");
            //int FocusMax = Aimsharp.PlayerMaxPower();
            //float SoulShard = Aimsharp.PlayerSecondaryPower() / 10f;

            // Mage Talents
            bool TalentSplittingIce = Aimsharp.Talent(6, 2);

            // Mage buffs
            bool BuffRuneOfPowerUp = Aimsharp.HasBuff("Rune of Power", "player", false);
            int BuffBrainFreezeRemaining = Aimsharp.BuffRemaining("Brain Freeze") - GCD;
            bool BuffBrainFreezeUp = BuffBrainFreezeRemaining > 0;
            int BuffFingersOfFrostStacks = Aimsharp.BuffStacks("Fingers of Frost");
            int BuffDisciplinaryCommandRemaining = Aimsharp.BuffRemaining("Disciplinary Command") - GCD;
            bool BuffDisciplinaryCommandUp = BuffDisciplinaryCommandRemaining > 0;
            int BuffExpandedPotentialRemaining = Aimsharp.BuffRemaining("Expanded Potential") - GCD;
            bool BuffExpandedPotentialUp = BuffExpandedPotentialRemaining > 0;
            int BuffFreezingRainRemaining = Aimsharp.BuffRemaining("Freezing Rain") - GCD;
            bool BuffFreezingRainUp = BuffFreezingRainRemaining > 0;


            bool BuffDisciplinaryCommandReady = true;
            if (BuffDisciplinaryCommandUp) DisciplinaryCommandTimer.Restart();
            if (DisciplinaryCommandTimer.IsRunning && DisciplinaryCommandTimer.ElapsedMilliseconds < 30000) BuffDisciplinaryCommandReady = false;

            int BuffFreezingWindsRemaining = Aimsharp.BuffRemaining("Freezing Winds") - GCD;
            bool BuffFreezingWindsUp = BuffFreezingWindsRemaining > 0;
            int BuffIceFloesRemaining = Aimsharp.BuffRemaining("Ice Floes") - GCD;
            bool BuffIceFloesUp = BuffIceFloesRemaining > 0;


            // Mage debuffs
            int DebuffWintersChillRemaining = Aimsharp.DebuffRemaining("Winter's Chill") - GCD;
            bool DebuffWintersChillUp = DebuffWintersChillRemaining > 0;
            List<int> Frozen = new List<int>() { Aimsharp.DebuffRemaining("Frost Nova", "target", false) - GCD, Aimsharp.DebuffRemaining("Freeze", "target", false) - GCD, Aimsharp.DebuffRemaining("Frostbite", "target", false) - GCD };
            int DebuffFrozenRemaining = Frozen.Max();
            bool DebuffFrozenUp = DebuffFrozenRemaining > 0;
            int DebuffMotRemaining = Aimsharp.DebuffRemaining("Mirrors of Torment") - GCD;
            bool DebuffMotUp = DebuffMotRemaining > 0;

            // Mage cooldowns
            int CDIcyVeinsRemaining = Aimsharp.SpellCooldown("Icy Veins");
            bool CDIcyVeinsUp = CDIcyVeinsRemaining <= 0;

            // int BarbedShotFullRecharge = (int)(Aimsharp.RechargeTime("Barbed Shot") + (12000f / (1f + Haste)) * (1f - Aimsharp.SpellCharges("Barbed Shot")));
            // float BarbedShotChargesFractional_temp = Aimsharp.SpellCharges("Barbed Shot") + (1 - (Aimsharp.RechargeTime("Barbed Shot") - GCD) / ((12000f) / (1f + Haste)));
            // float BarbedShotChargesFractional = BarbedShotChargesFractional_temp > Aimsharp.MaxCharges("Barbed Shot") ? Aimsharp.MaxCharges("Barbed Shot") : BarbedShotChargesFractional_temp;
            // int CDBarbedShotCharges = Aimsharp.SpellCharges("Barbed Shot");

            // Mage specific variables
            //int CascadingCalamityRank = Aimsharp.CustomFunction("Cascading Calamity Rank");
            bool CastingEbonbolt = Aimsharp.CastingID("player") == 257537;
            bool CastingGlacialSpike = Aimsharp.CastingID("player") == 199786;
            bool CastingFrostbolt = Aimsharp.CastingID("player") == 116;
            bool CastingRadiantSpark = Aimsharp.CastingID("player") == 307443;
            float IceLanceTravelTime = ((RangeToTarget + 5f) / 50f) * 1000f;
            float GlacialSpikeTravelTime = ((RangeToTarget + 5f) / 40f) * 1000f;
            float GlacialSpikeCastTime = 3000f / (1f + Haste);
            List<int> ActiveConduits = Aimsharp.GetActiveConduits();

            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions+=/call_action_list,name=cds
            if (!SaveCooldowns)
            {
                //actions.cds=potion,if=prev_off_gcd.icy_veins|fight_remains<30
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false))
                {
                    if (TargetTimeToDie < 30000 || LastCast == "Icy Veins")
                    {
                        Aimsharp.Cast("DPS Pot", true);
                        return true;
                    }
                }

                //actions.cds+=/mirrors_of_torment,if=soulbind.wasteland_propriety.enabled
                if (Aimsharp.CanCast("Mirrors of Torment") && Fighting && !Moving)
                {
                    if (ActiveConduits.Contains(319983)) 
                    {
                        Aimsharp.Cast("Mirrors of Torment");
                        return true;
                    }
                }

                //actions.cds+=/deathborne
                if (Aimsharp.CanCast("Deathborne", "player") && Fighting && !Moving)
                {
                    Aimsharp.Cast("Deathborne");
                    return true;
                }

                //actions.cds+=/rune_of_power,if=cooldown.icy_veins.remains>15&buff.rune_of_power.down
                if (Aimsharp.CanCast("Rune of Power", "player") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    if (CDIcyVeinsRemaining > 15000 && !BuffRuneOfPowerUp)
                    {
                        Aimsharp.Cast("Rune of Power");
                        return true;
                    }
                }

                //actions.cds+=/icy_veins,if=buff.rune_of_power.down
                if (Aimsharp.CanCast("Icy Veins", "player") && Fighting)
                {
                    if (!BuffRuneOfPowerUp)
                    {
                        Aimsharp.Cast("Icy Veins");
                        return true;
                    }
                }

                //actions.cds+=/use_items
                if (Aimsharp.CanUseTrinket(0) && UseTopTrinket && Fighting)
                {
                    Aimsharp.Cast("TopTrinket", true);
                    return true;
                }

                if (Aimsharp.CanUseTrinket(1) && UseBottomTrinket && Fighting)
                {
                    Aimsharp.Cast("BottomTrinket", true);
                    return true;
                }

                //racials
                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }

                //essences
                foreach (string Essence in Essences)
                {
                    if (Aimsharp.CanCast(Essence, "player") && Fighting)
                    {
                        Aimsharp.Cast(Essence);
                        return true;
                    }
                }

                foreach (string Essence in EssencesTargeted)
                {
                    if (Essence == "Concentrated Flame")
                    {
                        if (Aimsharp.CanCast(Essence) && Fighting && ConcentratedFlameFullRecharge < GCDMAX)
                        {
                            Aimsharp.Cast(Essence);
                            return true;
                        }
                    }

                    else if (Aimsharp.CanCast(Essence) && Fighting)
                    {
                        Aimsharp.Cast(Essence);
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=aoe,if=active_enemies>=5
            if (EnemiesNearTarget >= 5)
            {
                //actions.aoe=frozen_orb
                if (Aimsharp.CanCast("Frozen Orb", "player") && Fighting)
                {
                    Aimsharp.Cast("Frozen Orb");
                    return true;
                }

                //actions.aoe+=/blizzard
                if (Aimsharp.CanCast("Blizzard", "player") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    Aimsharp.Cast("Blizzcursor");
                    return true;
                }

                //actions.aoe+=/flurry,if=(remaining_winters_chill=0|debuff.winters_chill.down)&(prev_gcd.1.ebonbolt|buff.brain_freeze.react&buff.fingers_of_frost.react=0)
                if (Aimsharp.CanCast("Flurry") && Fighting && (BuffFingersOfFrostStacks > 0 || (BuffIceFloesUp || !Moving)))
                {
                    if ((DebuffWintersChillRemaining <= 0 || !DebuffWintersChillUp) && (CastingEbonbolt || BuffBrainFreezeUp && BuffFingersOfFrostStacks == 0))
                    {
                        Aimsharp.Cast("Flurry");
                        return true;
                    }
                }

                //actions.aoe+=/ice_nova
                if (Aimsharp.CanCast("Ice Nova") && Fighting)
                {
                    Aimsharp.Cast("Ice Nova");
                    return true;
                }

                //actions.aoe+=/comet_storm
                if (Aimsharp.CanCast("Comet Storm") && Fighting)
                {
                    Aimsharp.Cast("Comet Storm");
                    return true;
                }

                //actions.aoe+=/ice_lance,if=buff.fingers_of_frost.react|debuff.frozen.remains>travel_time|remaining_winters_chill&debuff.winters_chill.remains>travel_time
                if (Aimsharp.CanCast("Ice Lance") && Fighting)
                {
                    if (BuffFingersOfFrostStacks > 0 || DebuffFrozenRemaining > IceLanceTravelTime || DebuffWintersChillRemaining > IceLanceTravelTime)
                    {
                        Aimsharp.Cast("Ice Lance");
                        return true;
                    }
                }

                //actions.aoe+=/radiant_spark
                if (Aimsharp.CanCast("Radiant Spark") && Fighting && !Moving)
                {
                    Aimsharp.Cast("Radiant Spark");
                    return true;
                }

                //actions.aoe+=/shifting_power
                if (Aimsharp.CanCast("Shifting Power", "player") && Fighting && !Moving)
                {
                    Aimsharp.Cast("Shifting Power");
                    return true;
                }

                //actions.aoe+=/mirrors_of_torment
                if (Aimsharp.CanCast("Mirrors of Torment") && Fighting && !Moving)
                {
                    Aimsharp.Cast("Mirrors of Torment");
                    return true;
                }

                //actions.aoe+=/frost_nova,if=runeforge.grisly_icicle.equipped&target.level<=level&debuff.frozen.down
                if (Aimsharp.CanCast("Frost Nova", "player") && Fighting && RangeToTarget < 12)
                {
                    if (RuneforgePower == "Grisly Icicle" && !DebuffFrozenUp)
                    {
                        Aimsharp.Cast("Frost Nova");
                        return true;
                    }
                }

                //actions.aoe+=/fire_blast,if=runeforge.disciplinary_command.equipped&cooldown.buff_disciplinary_command.ready&buff.disciplinary_command_fire.down
                if (Aimsharp.CanCast("Fire Blast") && Fighting)
                {
                    if (RuneforgePower == "Disciplinary Command" && BuffDisciplinaryCommandReady)
                    {
                        Aimsharp.Cast("Fire Blast");
                        return true;
                    }
                }

                //actions.aoe+=/arcane_explosion,if=mana.pct>30&!runeforge.cold_front.equipped&(!runeforge.freezing_winds.equipped|buff.freezing_winds.up)
                if (Aimsharp.CanCast("Arcane Explosion", "player") && Fighting && RangeToTarget < 10)
                {
                    if (Mana > 30 && RuneforgePower != "Cold Front" && (RuneforgePower != "Freezing Winds" || BuffFreezingWindsUp))
                    {
                        Aimsharp.Cast("Arcane Explosion");
                        return true;
                    }
                }

                //actions.aoe+=/ebonbolt
                if (Aimsharp.CanCast("Ebonbolt") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    Aimsharp.Cast("Ebonbolt");
                    return true;
                }

                //actions.aoe+=/ice_lance,if=runeforge.glacial_fragments.equipped&talent.splitting_ice.enabled
                if (Aimsharp.CanCast("Ice Lance") && Fighting)
                {
                    if (RuneforgePower == "Glacial Fragments" && TalentSplittingIce)
                    {
                        Aimsharp.Cast("Ice Lance");
                        return true;
                    }
                }

                //actions.aoe+=/frostbolt
                if (Aimsharp.CanCast("Frostbolt") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    Aimsharp.Cast("Frostbolt");
                    return true;
                }
            }

            //single target
            if (EnemiesNearTarget < 5)
            {
                //actions.st=flurry,if=(remaining_winters_chill=0|debuff.winters_chill.down)&(prev_gcd.1.ebonbolt|buff.brain_freeze.react&(prev_gcd.1.radiant_spark|prev_gcd.1.glacial_spike|prev_gcd.1.frostbolt|(debuff.mirrors_of_torment.up|buff.expanded_potential.react|buff.freezing_winds.up)&buff.fingers_of_frost.react=0))
                if (Aimsharp.CanCast("Flurry") && Fighting && (BuffFingersOfFrostStacks > 0 || (BuffIceFloesUp || !Moving)))
                {
                    if ((DebuffWintersChillRemaining <= 0 || !DebuffWintersChillUp) && (CastingEbonbolt || BuffBrainFreezeUp && (CastingRadiantSpark || CastingGlacialSpike || CastingFrostbolt || (DebuffMotUp || BuffExpandedPotentialUp || BuffFreezingWindsUp) && BuffFingersOfFrostStacks == 0)))
                    {
                        Aimsharp.Cast("Flurry");
                        return true;
                    }
                }

                //actions.st+=/frozen_orb
                if (Aimsharp.CanCast("Frozen Orb", "player") && Fighting)
                {
                    Aimsharp.Cast("Frozen Orb");
                    return true;
                }

                //actions.st+=/blizzard,if=buff.freezing_rain.up|active_enemies>=3|active_enemies>=2&!runeforge.cold_front.equipped
                if (Aimsharp.CanCast("Blizzard", "player") && Fighting && (BuffFreezingRainUp || (BuffIceFloesUp || !Moving)))
                {
                    if (BuffFreezingRainUp || EnemiesNearTarget >= 3 | EnemiesNearTarget >= 2 && RuneforgePower != "Cold Front")
                    {
                        Aimsharp.Cast("Blizzcursor");
                        return true;
                    }
                }

                //actions.st+=/ray_of_frost,if=remaining_winters_chill=1&debuff.winters_chill.remains
                if (Aimsharp.CanCast("Ray of Frost") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    if (DebuffWintersChillRemaining > 0)
                    {
                        Aimsharp.Cast("Ray of Frost");
                        return true;
                    }
                }

                //actions.st+=/glacial_spike,if=remaining_winters_chill&debuff.winters_chill.remains>cast_time+travel_time
                if (Aimsharp.CanCast("Glacial Spike") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    if (DebuffWintersChillRemaining > GlacialSpikeTravelTime + GlacialSpikeCastTime)
                    {
                        Aimsharp.Cast("Glacial Spike");
                        return true;
                    }
                }

                //actions.st+=/ice_lance,if=remaining_winters_chill&remaining_winters_chill>buff.fingers_of_frost.react&debuff.winters_chill.remains>travel_time
                if (Aimsharp.CanCast("Ice Lance") && Fighting)
                {
                    if (DebuffWintersChillRemaining > IceLanceTravelTime)
                    {
                        Aimsharp.Cast("Ice Lance");
                        return true;
                    }
                }

                //actions.aoe+=/comet_storm
                if (Aimsharp.CanCast("Comet Storm") && Fighting)
                {
                    Aimsharp.Cast("Comet Storm");
                    return true;
                }

                if (Aimsharp.CanCast("Ice Nova") && Fighting)
                {
                    Aimsharp.Cast("Ice Nova");
                    return true;
                }

                //actions.st+=/radiant_spark,if=buff.freezing_winds.up&active_enemies=1
                if (Aimsharp.CanCast("Radiant Spark") && Fighting && !Moving)
                {
                    if (BuffFreezingWindsUp && EnemiesNearTarget <= 1)
                    {
                        Aimsharp.Cast("Radiant Spark");
                        return true;
                    }
                }

                //actions.st+=/ice_lance,if=buff.fingers_of_frost.react|debuff.frozen.remains>travel_time
                if (Aimsharp.CanCast("Ice Lance") && Fighting)
                {
                    if (BuffFingersOfFrostStacks > 0 || DebuffFrozenRemaining > IceLanceTravelTime)
                    {
                        Aimsharp.Cast("Ice Lance");
                        return true;
                    }
                }

                //actions.st+=/ebonbolt
                if (Aimsharp.CanCast("Ebonbolt") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    Aimsharp.Cast("Ebonbolt");
                    return true;
                }

                //actions.st+=/radiant_spark,if=(!runeforge.freezing_winds.equipped|active_enemies>=2)&(buff.brain_freeze.react|soulbind.combat_meditation.enabled)
                if (Aimsharp.CanCast("Radiant Spark") && Fighting && !Moving)
                {
                    if ((RuneforgePower != "Freezing Winds" || EnemiesNearTarget >= 2) && (BuffBrainFreezeUp || ActiveConduits.Contains(328266))) 
                    {
                        Aimsharp.Cast("Radiant Spark");
                        return true;
                    }
                }

                //actions.st+=/shifting_power,if=active_enemies>=3
                if (Aimsharp.CanCast("Shifting Power", "player") && Fighting && !Moving && EnemiesNearTarget >= 3)
                {
                    Aimsharp.Cast("Shifting Power");
                    return true;
                }

                //actions.st+=/mirrors_of_torment
                if (Aimsharp.CanCast("Mirrors of Torment") && Fighting && !Moving)
                {
                    Aimsharp.Cast("Mirrors of Torment");
                    return true;
                }

                //actions.st+=/frost_nova,if=runeforge.grisly_icicle.equipped&target.level<=level&debuff.frozen.down
                if (Aimsharp.CanCast("Frost Nova", "player") && Fighting && RangeToTarget < 12)
                {
                    if (RuneforgePower == "Grisly Icicle" && !DebuffFrozenUp)
                    {
                        Aimsharp.Cast("Frost Nova");
                        return true;
                    }
                }

                //actions.st+=/fire_blast,if=runeforge.disciplinary_command.equipped&cooldown.buff_disciplinary_command.ready&buff.disciplinary_command_fire.down
                if (Aimsharp.CanCast("Fire Blast") && Fighting)
                {
                    if (RuneforgePower == "Disciplinary Command" && BuffDisciplinaryCommandReady)
                    {
                        Aimsharp.Cast("Fire Blast");
                        return true;
                    }
                }

                //actions.st+=/arcane_explosion,if=runeforge.disciplinary_command.equipped&cooldown.buff_disciplinary_command.ready&buff.disciplinary_command_arcane.down
                if (Aimsharp.CanCast("Arcane Explosion", "player") && Fighting)
                {
                    if (RuneforgePower == "Disciplinary Command" && BuffDisciplinaryCommandReady)
                    {
                        Aimsharp.Cast("Arcane Explosion");
                        return true;
                    }
                }

                //actions.st+=/glacial_spike,if=buff.brain_freeze.react
                if (Aimsharp.CanCast("Glacial Spike") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    if (BuffBrainFreezeUp)
                    {
                        Aimsharp.Cast("Glacial Spike");
                        return true;
                    }
                }

                //actions.st+=/frostbolt
                if (Aimsharp.CanCast("Frostbolt") && Fighting && (BuffIceFloesUp || !Moving))
                {
                    Aimsharp.Cast("Frostbolt");
                    return true;
                }
            }

            if (Aimsharp.CanCast("Ice Floes", "player") && Fighting)
            {
                Aimsharp.Cast("Ice Floes");
                return true;
            }

            if (Aimsharp.CanCast("Ice Lance") && Fighting)
            {
                Aimsharp.Cast("Ice Lance");
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
