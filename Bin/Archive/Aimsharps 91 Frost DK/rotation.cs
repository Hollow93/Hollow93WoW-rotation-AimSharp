using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class SLFrostDK : Rotation
    {
        List<string> Racials = new List<string>
        {
            "Blood Fury","Berserking","Fireblood","Ancestral Call","Bag of Tricks"
        };

        List<string> CovenantAbilities = new List<string>
        {

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
           "Howling Blast","Breath of Sindragosa","Pillar of Frost","Glacial Advance","Frost Strike","Empower Rune Weapon","Remorseless Winter","Frostwyrm's Fury","Hypothermic Presence","Raise Dead","Death's Due","Swarming Mist","Abomination Limb",
           "Shackle the Unworthy","Chains of Ice","Obliterate","Howling Blast","Frostscythe","Death and Decay","Horn of Winter"
        };

        List<string> BuffsList = new List<string>
        {
            "Icy Talons","Pillar of Frost","Empower Rune Weapon","Breath of Sindragosa","Remorseless Winter","Unholy Strength","Cold Heart","Rime","Killing Machine","Eradicating Blow","Unleashed Frenzy"
        };

        List<string> DebuffsList = new List<string>
        {
           "Frost Fever","Razorice"
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

            Settings.Add(new Setting("Death Knight Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Biting Cold" }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Frost DK", Color.Purple);
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

            Macros.Add("DDcursor", "/cast [@cursor] Death's Due");
            Macros.Add("DnDcursor", "/cast [@cursor] Death and Decay");

            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            CustomFunctions.Add("RuneforgeFallenCrusader", "if select(1,GetWeaponEnchantInfo()) then if select(4,GetWeaponEnchantInfo()) == 3368 then return 1 end end if select(5,GetWeaponEnchantInfo()) then if select(8,GetWeaponEnchantInfo()) == 3368 then return 1 end end return 0");
            CustomFunctions.Add("RuneforgeRazorice", "if select(1,GetWeaponEnchantInfo()) then if select(4,GetWeaponEnchantInfo()) == 3370 then return 1 end end if select(5,GetWeaponEnchantInfo()) then if select(8,GetWeaponEnchantInfo()) == 3370 then return 1 end end return 0");
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
            int GCDMAX = (int)(1500f / (Haste + 1f));
            int TargetTimeToDie = 1000000; //need to implement time to die estimation
            int TargetHealthPct = Aimsharp.Health("target");
            int PlayerLevel = Aimsharp.GetPlayerLevel();
            bool Moving = Aimsharp.PlayerIsMoving();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            int PlayerCastingID = Aimsharp.CastingID("player");
            string LastCast = Aimsharp.LastCast();
            int CovenantID = Aimsharp.CovenantID();

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

            // DK power
            int RunicPower = Aimsharp.Power("player");
            int Runes = Aimsharp.PlayerSecondaryPower();
            int MaxRunicPower = Aimsharp.PlayerMaxPower();
            int RunicDefecit = MaxRunicPower - RunicPower;

            // DK Talents
            bool TalentIceCap = Aimsharp.Talent(7, 1);
            bool TalentObliteration = Aimsharp.Talent(7, 2);
            bool TalentBreathofSindragosa = Aimsharp.Talent(7, 3);
            bool TalentGatheringStorm = Aimsharp.Talent(6, 1);
            bool TalentColdHeart = Aimsharp.Talent(1, 3);
            bool TalentRunicAttenuation = Aimsharp.Talent(2, 1);
            bool TalentFrostscythe = Aimsharp.Talent(4, 3);
            bool TalentFrozenPulse = Aimsharp.Talent(4, 2);

            // DK buffs
            int BuffIcyTalonsRemaining = Aimsharp.BuffRemaining("Icy Talons") - GCD;
            bool BuffIcyTalonsUp = BuffIcyTalonsRemaining > 0;
            int BuffPillarofFrostRemaining = Aimsharp.BuffRemaining("Pillar of Frost") - GCD;
            bool BuffPillarofFrostUp = BuffPillarofFrostRemaining > 0;
            int BuffEmpowerRuneWeaponRemaining = Aimsharp.BuffRemaining("Empower Rune Weapon") - GCD;
            bool BuffEmpowerRuneWeaponUp = BuffEmpowerRuneWeaponRemaining > 0;
            bool BuffBreathofSindragosaUp = Aimsharp.HasBuff("Breath of Sindragosa");
            int BuffRemorselessWinterRemaining = Aimsharp.BuffRemaining("Remorseless Winter") - GCD;
            bool BuffRemorselessWinterUp = BuffRemorselessWinterRemaining > 0;
            int BuffUnholyStrengthRemaining = Aimsharp.BuffRemaining("Unholy Strength") - GCD;
            bool BuffUnholyStrengthUp = BuffUnholyStrengthRemaining > 0;
            int BuffKillingMachineRemaining = Aimsharp.BuffRemaining("Killing Machine") - GCD;
            bool BuffKillingMachineUp = BuffKillingMachineRemaining > 0;
            int BuffColdHeartStacks = Aimsharp.BuffStacks("Cold Heart");
            int BuffRimeRemaining = Aimsharp.BuffRemaining("Rime") - GCD;
            bool BuffRimeUp = BuffRimeRemaining > 0;
            int BuffEradicatingBlowStacks = Aimsharp.BuffStacks("Eradicating Blow");
            int BuffEradicatingBlowRemaining = Aimsharp.BuffRemaining("Eradicating Blow") - GCD;
            bool BuffEradicatingBlowUp = BuffEradicatingBlowRemaining > 0;
            int BuffUnleashedFrenzyRemaining = Aimsharp.BuffRemaining("Unleashed Frenzy") - GCD;
            bool BuffUnleashedFrenzyUp = BuffUnleashedFrenzyRemaining > 0;

            // DK debuffs
            int DotFrostFeverRemaining = Aimsharp.DebuffRemaining("Frost Fever") - GCD;
            bool DotFrostFeverTicking = DotFrostFeverRemaining > 0;
            int DebuffRazoriceStacks = Aimsharp.DebuffStacks("Razorice");
            int DebuffRazoriceRemaining = Aimsharp.DebuffRemaining("Razorice") - GCD;
            bool DebuffRazoriceTicking = DebuffRazoriceRemaining > 0;

            // DK cooldowns
            int CDBreathofSindragosaRemaining = Aimsharp.SpellCooldown("Breath of Sindragosa");
            bool CDBreathofSindragosaUp = CDBreathofSindragosaRemaining <= 0;
            int CDPillarofFrostRemaining = Aimsharp.SpellCooldown("Pillar of Frost");
            bool CDPillarofFrostUp = CDPillarofFrostRemaining <= 0;
            int CDDeathandDecayRemaining = Aimsharp.SpellCooldown("Death and Decay");
            bool CDDeathandDecayUp = CDDeathandDecayRemaining <= 0;
            int CDRemorselessWinterRemaining = Aimsharp.SpellCooldown("Remorseless Winter");
            bool CDRemorselessWinterUp = CDRemorselessWinterRemaining <= 0;

            // int BarbedShotFullRecharge = (int)(Aimsharp.RechargeTime("Barbed Shot") + (12000f / (1f + Haste)) * (1f - Aimsharp.SpellCharges("Barbed Shot")));
            // float BarbedShotChargesFractional_temp = Aimsharp.SpellCharges("Barbed Shot") + (1 - (Aimsharp.RechargeTime("Barbed Shot") - GCD) / ((12000f) / (1f + Haste)));
            // float BarbedShotChargesFractional = BarbedShotChargesFractional_temp > Aimsharp.MaxCharges("Barbed Shot") ? Aimsharp.MaxCharges("Barbed Shot") : BarbedShotChargesFractional_temp;
            // int CDBarbedShotCharges = Aimsharp.SpellCharges("Barbed Shot");

            // DK specific variables
            bool RuneforgeFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            bool RuneforgeRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            bool RuneforgeBitingCold = RuneforgePower == "Biting Cold";
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");
            // bool ConduitSerratedGlaive = Aimsharp.GetActiveConduits().Contains(339230);





            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions+=/howling_blast,if=!dot.frost_fever.ticking&(talent.icecap.enabled|cooldown.breath_of_sindragosa.remains>15|talent.obliteration.enabled&cooldown.pillar_of_frost.remains<dot.frost_fever.remains)
            if (Aimsharp.CanCast("Howling Blast") && Fighting)
            {
                if (!DotFrostFeverTicking && (TalentIceCap || CDBreathofSindragosaRemaining > 15000 || TalentObliteration && CDPillarofFrostRemaining < DotFrostFeverRemaining))
                {
                    Aimsharp.Cast("Howling Blast");
                    return true;
                }
            }

            //actions+=/glacial_advance,if=buff.icy_talons.remains<=gcd&buff.icy_talons.up&spell_targets.glacial_advance>=2&(!talent.breath_of_sindragosa.enabled|cooldown.breath_of_sindragosa.remains>15)
            if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
            {
                if (BuffIcyTalonsRemaining <= GCDMAX && BuffIcyTalonsUp && EnemiesInMelee >= 2 && (!TalentBreathofSindragosa || CDBreathofSindragosaRemaining > 15000))
                {
                    Aimsharp.Cast("Glacial Advance");
                    return true;
                }
            }

            //actions+=/frost_strike,if=buff.icy_talons.remains<=gcd&buff.icy_talons.up&(!talent.breath_of_sindragosa.enabled|cooldown.breath_of_sindragosa.remains>15)
            if (Aimsharp.CanCast("Frost Strike") && Fighting)
            {
                if (BuffIcyTalonsRemaining <= GCDMAX && BuffIcyTalonsUp && (!TalentBreathofSindragosa || CDBreathofSindragosaRemaining > 15000))
                {
                    Aimsharp.Cast("Frost Strike");
                    return true;
                }
            }

            //actions+=/call_action_list,name=cooldowns
            if (!SaveCooldowns)
            {
                //actions+=/call_action_list,name=covenants
                //actions.covenants=deaths_due,if=raid_event.adds.in>15|!raid_event.adds.exists|active_enemies>=2
                if (Aimsharp.CanCast("Death's Due", "player") && Fighting)
                {
                    Aimsharp.Cast("DDcursor");
                    return true;
                }

                //actions.covenants+=/swarming_mist,if=active_enemies=1&runic_power.deficit>3&cooldown.pillar_of_frost.remains<3&!talent.breath_of_sindragosa&(!raid_event.adds.exists|raid_event.adds.in>15)
                if (Aimsharp.CanCast("Swarming Mist", "player") && Fighting)
                {
                    if (EnemiesInMelee == 1 && RunicDefecit > 3 && CDPillarofFrostRemaining < 3000 && !TalentBreathofSindragosa)
                    {
                        Aimsharp.Cast("Swarming Mist");
                        return true;
                    }
                }

                //actions.covenants+=/swarming_mist,if=active_enemies>=2&!talent.breath_of_sindragosa
                if (Aimsharp.CanCast("Swarming Mist", "player") && Fighting)
                {
                    if (EnemiesInMelee >= 2 && !TalentBreathofSindragosa)
                    {
                        Aimsharp.Cast("Swarming Mist");
                        return true;
                    }
                }

                //actions.covenants+=/swarming_mist,if=talent.breath_of_sindragosa&(buff.breath_of_sindragosa.up&(active_enemies=1&runic_power.deficit>40|active_enemies>=2&runic_power.deficit>60)|!buff.breath_of_sindragosa.up&cooldown.breath_of_sindragosa.remains)
                if (Aimsharp.CanCast("Swarming Mist", "player") && Fighting)
                {
                    if (TalentBreathofSindragosa && (BuffBreathofSindragosaUp && (EnemiesInMelee == 1 && RunicDefecit > 40 || EnemiesInMelee >= 2 && RunicDefecit > 60) || !BuffBreathofSindragosaUp && CDBreathofSindragosaRemaining > GCDMAX))
                    {
                        Aimsharp.Cast("Swarming Mist");
                        return true;
                    }
                }

                //actions.covenants+=/abomination_limb,if=active_enemies=1&cooldown.pillar_of_frost.remains<3&(!raid_event.adds.exists|raid_event.adds.in>15)
                if (Aimsharp.CanCast("Abomination Limb", "player") && Fighting)
                {
                    if (EnemiesInMelee == 1 && CDPillarofFrostRemaining < 3000)
                    {
                        Aimsharp.Cast("Abomination Limb");
                        return true;
                    }
                }

                //actions.covenants+=/abomination_limb,if=active_enemies>=2
                if (Aimsharp.CanCast("Abomination Limb", "player") && Fighting)
                {
                    if (EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Abomination Limb");
                        return true;
                    }
                }

                //actions.covenants+=/shackle_the_unworthy,if=active_enemies=1&cooldown.pillar_of_frost.remains<3&(!raid_event.adds.exists|raid_event.adds.in>15)
                if (Aimsharp.CanCast("Shackle the Unworthy") && Fighting)
                {
                    if (EnemiesInMelee == 1 && CDPillarofFrostRemaining < 3000)
                    {
                        Aimsharp.Cast("Shackle the Unworthy");
                        return true;
                    }
                }

                //actions.covenants+=/shackle_the_unworthy,if=active_enemies>=2
                if (Aimsharp.CanCast("Shackle the Unworthy") && Fighting)
                {
                    if (EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Shackle the Unworthy");
                        return true;
                    }
                }

                //actions.cooldowns=use_items,if=cooldown.pillar_of_frost.ready|cooldown.pillar_of_frost.remains>20
                if (CDPillarofFrostUp || CDPillarofFrostRemaining > 20000)
                {
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
                }

                //actions.cooldowns+=/potion,if=buff.pillar_of_frost.up&buff.empower_rune_weapon.up
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false))
                {
                    if (BuffPillarofFrostUp && BuffEmpowerRuneWeaponUp)
                    {
                        Aimsharp.Cast("DPS Pot", true);
                        return true;
                    }
                }

                //racials
                if (BuffPillarofFrostUp)
                {
                    foreach (string Racial in Racials)
                    {
                        if (Aimsharp.CanCast(Racial, "player") && Fighting)
                        {
                            Aimsharp.Cast(Racial, true);
                            return true;
                        }
                    }
                }

                //actions.cooldowns+=/empower_rune_weapon,if=talent.obliteration.enabled&(cooldown.pillar_of_frost.ready&rune.time_to_5>gcd&runic_power.deficit>=10|buff.pillar_of_frost.up&rune.time_to_5>gcd)|fight_remains<20
                if (Aimsharp.CanCast("Empower Rune Weapon", "player") && Fighting)
                {
                    if (TalentObliteration && (CDPillarofFrostUp && Aimsharp.TimeUntilRunes(5) - GCD > GCDMAX && RunicDefecit >= 10 || BuffPillarofFrostUp && Aimsharp.TimeUntilRunes(5) - GCD > GCDMAX) || TargetTimeToDie < 20000)
                    {
                        Aimsharp.Cast("Empower Rune Weapon");
                        return true;
                    }
                }

                //actions.cooldowns+=/empower_rune_weapon,if=talent.breath_of_sindragosa.enabled&runic_power.deficit>40&rune.time_to_5>gcd&(buff.breath_of_sindragosa.up|fight_remains<20)
                if (Aimsharp.CanCast("Empower Rune Weapon", "player") && Fighting)
                {
                    if (TalentBreathofSindragosa && RunicDefecit > 40 && Aimsharp.TimeUntilRunes(5) - GCD > GCDMAX && (BuffBreathofSindragosaUp || TargetTimeToDie < 20000))
                    {
                        Aimsharp.Cast("Empower Rune Weapon");
                        return true;
                    }
                }

                //actions.cooldowns+=/empower_rune_weapon,if=talent.icecap.enabled&rune<3
                if (Aimsharp.CanCast("Empower Rune Weapon", "player") && Fighting)
                {
                    if (TalentIceCap && Runes < 3)
                    {
                        Aimsharp.Cast("Empower Rune Weapon");
                        return true;
                    }
                }

                //actions.cooldowns+=/pillar_of_frost,if=talent.breath_of_sindragosa&(cooldown.breath_of_sindragosa.remains|cooldown.breath_of_sindragosa.ready&runic_power.deficit<60)
                if (Aimsharp.CanCast("Pillar of Frost", "player") && Fighting)
                {
                    if (TalentBreathofSindragosa && (CDBreathofSindragosaRemaining > GCD || CDBreathofSindragosaUp && RunicDefecit < 60))
                    {
                        Aimsharp.Cast("Pillar of Frost");
                        return true;
                    }
                }

                //actions.cooldowns+=/pillar_of_frost,if=talent.icecap&!buff.pillar_of_frost.up
                if (Aimsharp.CanCast("Pillar of Frost", "player") && Fighting)
                {
                    if (TalentIceCap && !BuffPillarofFrostUp)
                    {
                        Aimsharp.Cast("Pillar of Frost");
                        return true;
                    }
                }

                //actions.cooldowns+=/pillar_of_frost,if=talent.obliteration&(talent.gathering_storm.enabled&buff.remorseless_winter.up|!talent.gathering_storm.enabled)
                if (Aimsharp.CanCast("Pillar of Frost", "player") && Fighting)
                {
                    if (TalentObliteration && (TalentGatheringStorm && BuffRemorselessWinterUp || !TalentGatheringStorm))
                    {
                        Aimsharp.Cast("Pillar of Frost");
                        return true;
                    }
                }

                //actions.cooldowns+=/breath_of_sindragosa,if=buff.pillar_of_frost.up
                if (Aimsharp.CanCast("Breath of Sindragosa", "player") && Fighting)
                {
                    if (BuffPillarofFrostUp)
                    {
                        Aimsharp.Cast("Breath of Sindragosa");
                        return true;
                    }
                }

                //actions.cooldowns+=/frostwyrms_fury,if=buff.pillar_of_frost.remains<gcd&buff.pillar_of_frost.up&!talent.obliteration
                if (Aimsharp.CanCast("Frostwyrm's Fury", "player") && Fighting)
                {
                    if (BuffPillarofFrostRemaining < GCDMAX && BuffPillarofFrostUp && !TalentObliteration)
                    {
                        Aimsharp.Cast("Frostwyrm's Fury");
                        return true;
                    }
                }

                //actions.cooldowns+=/frostwyrms_fury,if=active_enemies>=2&cooldown.pillar_of_frost.remains+15>target.time_to_die|fight_remains<gcd
                if (Aimsharp.CanCast("Frostwyrm's Fury", "player") && Fighting)
                {
                    if (EnemiesInMelee >= 2 && CDPillarofFrostRemaining + 15000 > TargetTimeToDie || TargetTimeToDie < GCDMAX + GCD)
                    {
                        Aimsharp.Cast("Frostwyrm's Fury");
                        return true;
                    }
                }

                //actions.cooldowns+=/frostwyrms_fury,if=talent.obliteration&!buff.pillar_of_frost.up&((buff.unholy_strength.up|!death_knight.runeforge.fallen_crusader)&(debuff.razorice.stack=5|!death_knight.runeforge.razorice))
                if (Aimsharp.CanCast("Frostwyrm's Fury", "player") && Fighting)
                {
                    if (TalentObliteration && !BuffPillarofFrostUp && ((BuffUnholyStrengthUp || !RuneforgeFallenCrusader) && (DebuffRazoriceStacks == 5 || !RuneforgeRazorice)))
                    {
                        Aimsharp.Cast("Frostwyrm's Fury");
                        return true;
                    }
                }

                //actions.cooldowns+=/hypothermic_presence,if=talent.breath_of_sindragosa&runic_power.deficit>40&rune>=3&buff.pillar_of_frost.up|!talent.breath_of_sindragosa&runic_power.deficit>=25
                if (Aimsharp.CanCast("Hypothermic Presence", "player") && Fighting)
                {
                    if (TalentBreathofSindragosa && RunicDefecit > 40 && Runes >= 3 && BuffPillarofFrostUp || !TalentBreathofSindragosa && RunicDefecit >= 25)
                    {
                        Aimsharp.Cast("Hypothermic Presence");
                        return true;
                    }
                }

                //actions.cooldowns+=/raise_dead,if=buff.pillar_of_frost.up
                if (Aimsharp.CanCast("Raise Dead", "player") && Fighting)
                {
                    if (BuffPillarofFrostUp)
                    {
                        Aimsharp.Cast("Raise Dead");
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=cold_heart,if=talent.cold_heart&(buff.cold_heart.stack>=10&(debuff.razorice.stack=5|!death_knight.runeforge.razorice)|fight_remains<=gcd)          
            if (TalentColdHeart && (BuffColdHeartStacks >= 10 && (DebuffRazoriceStacks == 5 || !RuneforgeRazorice) || TargetTimeToDie <= GCDMAX))
            {
                //actions.cold_heart=chains_of_ice,if=fight_remains<gcd|buff.pillar_of_frost.remains<3&buff.cold_heart.stack=20&!talent.obliteration
                if (Aimsharp.CanCast("Chains of Ice") && Fighting)
                {
                    if (TargetTimeToDie < GCDMAX || BuffPillarofFrostRemaining < 3000 && BuffColdHeartStacks == 20 && !TalentObliteration)
                    {
                        Aimsharp.Cast("Chains of Ice");
                        return true;
                    }
                }

                //actions.cold_heart+=/chains_of_ice,if=talent.obliteration&!buff.pillar_of_frost.up&(buff.cold_heart.stack>=16&buff.unholy_strength.up|buff.cold_heart.stack>=19)
                if (Aimsharp.CanCast("Chains of Ice") && Fighting)
                {
                    if (TalentObliteration && !BuffPillarofFrostUp && (BuffColdHeartStacks >= 16 && BuffUnholyStrengthUp || BuffColdHeartStacks >= 19))
                    {
                        Aimsharp.Cast("Chains of Ice");
                        return true;
                    }
                }
            }

            //actions+=/run_action_list,name=bos_ticking,if=buff.breath_of_sindragosa.up
            if (BuffBreathofSindragosaUp)
            {
                //actions.bos_ticking=obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power<=40
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (RunicPower <= 40)
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                //actions.bos_ticking+=/remorseless_winter,if=talent.gathering_storm|active_enemies>=2
                if (Aimsharp.CanCast("Remorseless Winter", "player") && Fighting)
                {
                    if (TalentGatheringStorm || EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Remorseless Winter");
                        return true;
                    }
                }

                //actions.bos_ticking+=/howling_blast,if=buff.rime.up
                if (Aimsharp.CanCast("Howling Blast") && Fighting)
                {
                    if (BuffRimeUp)
                    {
                        Aimsharp.Cast("Howling Blast");
                        return true;
                    }
                }

                //actions.bos_ticking+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=rune.time_to_4<gcd|runic_power<=45
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (Aimsharp.TimeUntilRunes(4) < GCDMAX + GCD || RunicPower <= 45)
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                //actions.bos_ticking+=/frostscythe,if=buff.killing_machine.up&spell_targets.frostscythe>=2&(!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae)
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (BuffKillingMachineUp && EnemiesInMelee >= 2 && (CDDeathandDecayRemaining < 20000 && CovenantID == 3 || CovenantID != 3))
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.bos_ticking+=/horn_of_winter,if=runic_power.deficit>=40&rune.time_to_3>gcd
                if (Aimsharp.CanCast("Horn of Winter", "player") && Fighting)
                {
                    if (RunicDefecit >= 40 && Aimsharp.TimeUntilRunes(3) > GCDMAX + GCD)
                    {
                        Aimsharp.Cast("Horn of Winter");
                        return true;
                    }
                }

                //actions.bos_ticking+=/frostscythe,if=spell_targets.frostscythe>=2&(!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae)
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (EnemiesInMelee >= 2 && (CDDeathandDecayRemaining < 20000 && CovenantID == 3 || CovenantID != 3))
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.bos_ticking+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit>25|rune>3
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (RunicDefecit > 25 || Runes > 3)
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                return false;
            }

            //actions+=/run_action_list,name=bos_pooling,if=talent.breath_of_sindragosa&(cooldown.breath_of_sindragosa.remains<10)
            if (TalentBreathofSindragosa && (CDBreathofSindragosaRemaining < 10000))
            {
                //actions.bos_pooling=howling_blast,if=buff.rime.up
                if (Aimsharp.CanCast("Howling Blast") && Fighting)
                {
                    if (BuffRimeUp)
                    {
                        Aimsharp.Cast("Howling Blast");
                        return true;
                    }
                }

                //actions.bos_pooling+=/remorseless_winter,if=talent.gathering_storm&rune>=5|active_enemies>=2
                if (Aimsharp.CanCast("Remorseless Winter", "player") && Fighting)
                {
                    if (TalentGatheringStorm && Runes >= 5 || EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Remorseless Winter");
                        return true;
                    }
                }

                //actions.bos_pooling+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit>=25
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (RunicDefecit > 25)
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                //actions.bos_pooling+=/glacial_advance,if=runic_power.deficit<20&spell_targets.glacial_advance>=2&cooldown.pillar_of_frost.remains>5
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    if (RunicDefecit < 20 && EnemiesInMelee >= 2 && CDPillarofFrostRemaining > 5000)
                    {
                        Aimsharp.Cast("Glacial Advance");
                        return true;
                    }
                }

                //actions.bos_pooling+=/frost_strike,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit<20&cooldown.pillar_of_frost.remains>5
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    if (RunicDefecit < 20 && CDPillarofFrostRemaining > 5000)
                    {
                        Aimsharp.Cast("Frost Strike");
                        return true;
                    }
                }

                //actions.bos_pooling+=/frostscythe,if=buff.killing_machine.up&runic_power.deficit>(15+talent.runic_attenuation*3)&spell_targets.frostscythe>=2&(!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae)
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (BuffKillingMachineUp && RunicDefecit > (15 + (TalentRunicAttenuation ? 3 : 0)) && EnemiesInMelee >= 2 && (CDDeathandDecayRemaining < 20 && CovenantID == 3 || CovenantID != 3))
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.bos_pooling+=/frostscythe,if=runic_power.deficit>=(35+talent.runic_attenuation*3)&spell_targets.frostscythe>=2&(!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae)
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (RunicDefecit >= (35 + (TalentRunicAttenuation ? 3 : 0)) && EnemiesInMelee >= 2 && (CDDeathandDecayRemaining < 20 && CovenantID == 3 || CovenantID != 3))
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.bos_pooling+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit>=(35+talent.runic_attenuation*3)
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (RunicDefecit >= (35 + (TalentRunicAttenuation ? 3 : 0)))
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                //actions.bos_pooling+=/glacial_advance,if=cooldown.pillar_of_frost.remains>rune.time_to_4&runic_power.deficit<40&spell_targets.glacial_advance>=2
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    if (CDPillarofFrostRemaining > Aimsharp.TimeUntilRunes(4) && RunicDefecit < 40 && EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Glacial Advance");
                        return true;
                    }
                }

                //actions.bos_pooling+=/frost_strike,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=cooldown.pillar_of_frost.remains>rune.time_to_4&runic_power.deficit<40
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    if (CDPillarofFrostRemaining > Aimsharp.TimeUntilRunes(4) && RunicDefecit < 40)
                    {
                        Aimsharp.Cast("Frost Strike");
                        return true;
                    }
                }

                return false;
            }

            //actions+=/run_action_list,name=obliteration,if=buff.pillar_of_frost.up&talent.obliteration
            if (BuffPillarofFrostUp && TalentObliteration)
            {
                //actions.obliteration=remorseless_winter,if=talent.gathering_storm&active_enemies>=3
                if (Aimsharp.CanCast("Remorseless Winter", "player") && Fighting)
                {
                    if (TalentGatheringStorm && EnemiesInMelee >= 3)
                    {
                        Aimsharp.Cast("Remorseless Winter");
                        return true;
                    }
                }

                //actions.obliteration+=/howling_blast,if=!dot.frost_fever.ticking&!buff.killing_machine.up
                if (Aimsharp.CanCast("Howling Blast") && Fighting)
                {
                    if (!DotFrostFeverTicking && !BuffKillingMachineUp)
                    {
                        Aimsharp.Cast("Howling Blast");
                        return true;
                    }
                }

                //actions.obliteration+=/frostscythe,if=buff.killing_machine.react&spell_targets.frostscythe>=2&(!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae)
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (BuffKillingMachineUp && EnemiesInMelee >= 2 && (CDDeathandDecayRemaining < 20 && CovenantID == 3 || CovenantID != 3))
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.obliteration+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=buff.killing_machine.react|!buff.rime.up&spell_targets.howling_blast>=3
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (BuffKillingMachineUp || !BuffRimeUp && EnemiesInMelee >= 3)
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                //actions.obliteration+=/glacial_advance,if=spell_targets.glacial_advance>=2&(runic_power.deficit<10|rune.time_to_2>gcd)|(debuff.razorice.stack<5|debuff.razorice.remains<15)
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    if (EnemiesInMelee >= 2 && (RunicDefecit < 10 || Aimsharp.TimeUntilRunes(2) > GCDMAX + GCD) || (DebuffRazoriceStacks < 5 || DebuffRazoriceRemaining < 15000))
                    {
                        Aimsharp.Cast("Glacial Advance");
                        return true;
                    }
                }

                //actions.obliteration+=/frost_strike,if=conduit.eradicating_blow&buff.eradicating_blow.stack=2&active_enemies=1
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    if (ActiveConduits.Contains(337934) && BuffEradicatingBlowStacks == 2 && EnemiesInMelee == 1)
                    {
                        Aimsharp.Cast("Frost Strike");
                        return true;
                    }
                }

                //actions.obliteration+=/howling_blast,if=buff.rime.up&spell_targets.howling_blast>=2
                if (Aimsharp.CanCast("Howling Blast") && Fighting)
                {
                    if (BuffRimeUp && EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Howling Blast");
                        return true;
                    }
                }

                //actions.obliteration+=/glacial_advance,if=spell_targets.glacial_advance>=2
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    if (EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Glacial Advance");
                        return true;
                    }
                }

                //actions.obliteration+=/frost_strike,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit<10|rune.time_to_2>gcd|!buff.rime.up
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    if (RunicDefecit < 10 || Aimsharp.TimeUntilRunes(2) > GCDMAX + GCD || !BuffRimeUp)
                    {
                        Aimsharp.Cast("Frost Strike");
                        return true;
                    }
                }

                //actions.obliteration+=/howling_blast,if=buff.rime.up
                if (Aimsharp.CanCast("Howling Blast") && Fighting)
                {
                    if (BuffRimeUp)
                    {
                        Aimsharp.Cast("Howling Blast");
                        return true;
                    }
                }

                //actions.obliteration+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    Aimsharp.Cast("Obliterate");
                    return true;
                }

                return false;
            }

            //actions+=/run_action_list,name=aoe,if=active_enemies>=2
            if (EnemiesInMelee >= 2)
            {
                //actions.aoe=remorseless_winter
                if (Aimsharp.CanCast("Remorseless Winter", "player") && Fighting)
                {
                    Aimsharp.Cast("Remorseless Winter");
                    return true;
                }

                //actions.aoe+=/glacial_advance,if=talent.frostscythe
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    if (TalentFrostscythe)
                    {
                        Aimsharp.Cast("Glacial Advance");
                        return true;
                    }
                }

                //actions.aoe+=/frost_strike,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=cooldown.remorseless_winter.remains<=2*gcd&talent.gathering_storm
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    if (CDRemorselessWinterRemaining <= 2 * GCDMAX && TalentGatheringStorm)
                    {
                        Aimsharp.Cast("Frost Strike");
                        return true;
                    }
                }

                //actions.aoe+=/howling_blast,if=buff.rime.up
                if (Aimsharp.CanCast("Howling Blast") && Fighting)
                {
                    if (BuffRimeUp)
                    {
                        Aimsharp.Cast("Howling Blast");
                        return true;
                    }
                }

                //actions.aoe+=/frostscythe,if=buff.killing_machine.up&(!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae)
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (BuffKillingMachineUp && (CDDeathandDecayRemaining < 20 && CovenantID == 3 || CovenantID != 3))
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.aoe+=/glacial_advance,if=runic_power.deficit<(15+talent.runic_attenuation*3)
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    if (RunicDefecit < (15 + (TalentRunicAttenuation ? 3 : 0)))
                    {
                        Aimsharp.Cast("Glacial Advance");
                        return true;
                    }
                }

                //actions.aoe+=/frost_strike,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit<(15+talent.runic_attenuation*3)
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    if (RunicDefecit < (15 + (TalentRunicAttenuation ? 3 : 0)))
                    {
                        Aimsharp.Cast("Frost Strike");
                        return true;
                    }
                }

                //actions.aoe+=/frostscythe,if=!death_and_decay.ticking&covenant.night_fae|!covenant.night_fae
                if (Aimsharp.CanCast("Frostscythe", "player") && Fighting)
                {
                    if (CDDeathandDecayRemaining < 20 && CovenantID == 3 || CovenantID != 3)
                    {
                        Aimsharp.Cast("Frostscythe");
                        return true;
                    }
                }

                //actions.aoe+=/obliterate,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice,if=runic_power.deficit>(25+talent.runic_attenuation*3)
                if (Aimsharp.CanCast("Obliterate") && Fighting)
                {
                    if (RunicDefecit > (25 + (TalentRunicAttenuation ? 3 : 0)))
                    {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                }

                //actions.aoe+=/glacial_advance
                if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
                {
                    Aimsharp.Cast("Glacial Advance");
                    return true;
                }

                //actions.aoe+=/frost_strike,target_if=max:(debuff.razorice.stack+1)%(debuff.razorice.remains+1)*death_knight.runeforge.razorice
                if (Aimsharp.CanCast("Frost Strike") && Fighting)
                {
                    Aimsharp.Cast("Frost Strike");
                    return true;
                }

                //actions.aoe+=/horn_of_winter
                if (Aimsharp.CanCast("Horn of Winter", "player") && Fighting)
                {
                    Aimsharp.Cast("Horn of Winter");
                    return true;
                }

                return false;
            }

            //actions.standard=remorseless_winter,if=talent.gathering_storm|conduit.everfrost|runeforge.biting_cold
            if (Aimsharp.CanCast("Remorseless Winter", "player") && Fighting)
            {
                if (TalentGatheringStorm || ActiveConduits.Contains(337988) || RuneforgeBitingCold)
                {
                    Aimsharp.Cast("Remorseless Winter");
                    return true;
                }
            }

            //actions.standard+=/glacial_advance,if=!death_knight.runeforge.razorice&(debuff.razorice.stack<5|debuff.razorice.remains<7)
            if (Aimsharp.CanCast("Glacial Advance", "player") && Fighting)
            {
                if (!RuneforgeRazorice || (DebuffRazoriceStacks < 5 || DebuffRazoriceRemaining < 7000))
                {
                    Aimsharp.Cast("Glacial Advance");
                    return true;
                }
            }

            //actions.standard+=/frost_strike,if=cooldown.remorseless_winter.remains<=2*gcd&talent.gathering_storm
            if (Aimsharp.CanCast("Frost Strike") && Fighting)
            {
                if (CDRemorselessWinterRemaining <= 2 * GCDMAX && TalentGatheringStorm)
                {
                    Aimsharp.Cast("Frost Strike");
                    return true;
                }
            }

            //actions.standard+=/frost_strike,if=conduit.eradicating_blow&buff.eradicating_blow.stack=2|conduit.unleashed_frenzy&buff.unleashed_frenzy.remains<3&buff.unleashed_frenzy.up
            if (Aimsharp.CanCast("Frost Strike") && Fighting)
            {
                if (ActiveConduits.Contains(337934) && BuffEradicatingBlowStacks == 2 || ActiveConduits.Contains(338492) && BuffUnleashedFrenzyRemaining < 3000 && BuffUnleashedFrenzyUp)
                {
                    Aimsharp.Cast("Frost Strike");
                    return true;
                }
            }

            //actions.standard+=/howling_blast,if=buff.rime.up
            if (Aimsharp.CanCast("Howling Blast") && Fighting)
            {
                if (BuffRimeUp)
                {
                    Aimsharp.Cast("Howling Blast");
                    return true;
                }
            }

            //actions.standard+=/obliterate,if=!buff.frozen_pulse.up&talent.frozen_pulse
            if (Aimsharp.CanCast("Obliterate") && Fighting)
            {
                if (Runes > 3 && TalentFrozenPulse)
                {
                    Aimsharp.Cast("Obliterate");
                    return true;
                }
            }

            //actions.standard+=/frost_strike,if=runic_power.deficit<(15+talent.runic_attenuation*3)
            if (Aimsharp.CanCast("Frost Strike") && Fighting)
            {
                if (RunicDefecit < (15 + (TalentRunicAttenuation ? 3 : 0)))
                {
                    Aimsharp.Cast("Frost Strike");
                    return true;
                }
            }

            //actions.standard+=/obliterate,if=runic_power.deficit>(25+talent.runic_attenuation*3)
            if (Aimsharp.CanCast("Obliterate") && Fighting)
            {
                if (RunicDefecit > (25 + (TalentRunicAttenuation ? 3 : 0)))
                {
                    Aimsharp.Cast("Obliterate");
                    return true;
                }
            }

            //actions.standard+=/frost_strike
            if (Aimsharp.CanCast("Frost Strike") && Fighting)
            {
                Aimsharp.Cast("Frost Strike");
                return true;
            }

            //actions.standard+=/horn_of_winter
            if (Aimsharp.CanCast("Horn of Winter", "player") && Fighting)
            {
                Aimsharp.Cast("Horn of Winter");
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
