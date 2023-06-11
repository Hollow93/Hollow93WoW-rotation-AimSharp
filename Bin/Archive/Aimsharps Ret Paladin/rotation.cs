using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsRet : Rotation
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
           "Light's Judgment","Shield of Vengeance","Blessing of Spring","Blessing of Summer","Blessing of Autumn","Blessing of Winter","Avenging Wrath","Crusade","Ashen Hallow","Holy Avenger","Final Reckoning","Seraphim","Vanquishers Hammer","Execution Sentence","Divine Storm","Templar's Verdict","Divine Toll","Wake of Ashes","Blade of Justice","Hammer of Wrath","Judgment","Crusader Strike","Consecration","Arcane Torrent",
        };

        List<string> BuffsList = new List<string>
        {
            "Avenging Wrath","Crusade","Seraphim","Empyrean Power","Divine Purpose","Vanquisher's Hammer","Holy Avenger",
        };

        List<string> DebuffsList = new List<string>
        {
           "Judgment","Final Reckoning","Execution Sentence",
        };

        List<string> TotemsList = new List<string>
        {
            "Consecration"
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

            Settings.Add(new Setting("Paladin Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Ret", Color.Purple);
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

            Macros.Add("AshenHallowC", "/cast [@cursor] Ashen Hallow");
            Macros.Add("FinalReckoningC", "/cast [@cursor] Final Reckoning");


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
            int Mana = Aimsharp.Power("player");
            int HolyPower = Aimsharp.PlayerSecondaryPower();

            //Talents
            bool TalentHolyAvenger = Aimsharp.Talent(5, 2);
            bool TalentSeraphim = Aimsharp.Talent(5, 3);
            bool TalentCrusade = Aimsharp.Talent(7, 2);
            bool TalentFinalReckoning = Aimsharp.Talent(7, 3);
            bool TalentExecutionSentence = Aimsharp.Talent(1, 3);


            //buffs
            int BuffAvengingWrathRemains = Aimsharp.BuffRemaining("Avenging Wrath") - GCD;
            bool BuffAvengingWrathUp = BuffAvengingWrathRemains > 0;
            int BuffCrusadeRemains = Aimsharp.BuffRemaining("Crusade") - GCD;
            bool BuffCrusadeUp = BuffCrusadeRemains > 0;
            int BuffCrusadeStacks = Aimsharp.BuffStacks("Crusade");
            int BuffSeraphimRemains = Aimsharp.BuffRemaining("Seraphim") - GCD;
            bool BuffSeraphimUp = BuffSeraphimRemains > 0;
            int BuffEmpyreanPowerRemains = Aimsharp.BuffRemaining("Empyrean Power") - GCD;
            bool BuffEmpyreanPowerUp = BuffEmpyreanPowerRemains > 0;
            int BuffDivinePurposeRemains = Aimsharp.BuffRemaining("Divine Purpose") - GCD;
            bool BuffDivinePurposeUp = BuffDivinePurposeRemains > 0;
            int BuffVanquishersHammerRemains = Aimsharp.BuffRemaining("Vanquisher's Hammer") - GCD;
            bool BuffVanquishersHammerUp = BuffVanquishersHammerRemains > 0;
            int BuffHolyAvengerRemains = Aimsharp.BuffRemaining("Holy Avenger") - GCD;
            bool BuffHolyAvengerUp = BuffHolyAvengerRemains > 0;


            //debuffs
            int DebuffJudgmentRemains = Aimsharp.DebuffRemaining("Judgment") - GCD;
            bool DebuffJudgmentUp = DebuffJudgmentRemains > 0;
            int DebuffFinalReckoningRemains = Aimsharp.DebuffRemaining("Final Reckoning") - GCD;
            bool DebuffFinalReckoningUp = DebuffFinalReckoningRemains > 0;
            int DebuffExecutionSentenceRemains = Aimsharp.DebuffRemaining("Execution Sentence") - GCD;
            bool DebuffExecutionSentenceUp = DebuffExecutionSentenceRemains > 0;


            //cooldowns
            int CDAvengingWrathRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Avenging Wrath");
            bool CDAvengingWrathUp = CDAvengingWrathRemains <= 0;
            int CDCrusadeRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Crusade");
            bool CDCrusadeUp = CDCrusadeRemains <= 0;
            int CDHolyAvengerRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Holy Avenger");
            bool CDHolyAvengerUp = CDHolyAvengerRemains <= 0;
            int CDFinalReckoningRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Final Reckoning");
            bool CDFinalReckoningUp = CDFinalReckoningRemains <= 0;
            int CDVanquishersHammerRemains = Aimsharp.SpellCooldown("Vanquishers Hammer");
            bool CDVanquishersHammerUp = CDVanquishersHammerRemains <= 0;
            int CDExecutionSentenceRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Execution Sentence");
            bool CDExecutionSentenceUp = CDExecutionSentenceRemains <= 0;
            int CDDivineTollRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Divine Toll");
            bool CDDivineTollUp = CDDivineTollRemains <= 0;
            int CDWakeofAshesRemains = Aimsharp.SpellCooldown("Wake of Ashes");
            bool CDWakeofAshesUp = CDWakeofAshesRemains <= 0;
            int CDBladeofJusticeRemains = Aimsharp.SpellCooldown("Blade of Justice");
            bool CDBladeofJusticeUp = CDBladeofJusticeRemains <= 0;
            int CDHammerofWrathRemains = Aimsharp.SpellCooldown("Hammer of Wrath");
            bool CDHammerofWrathUp = CDHammerofWrathRemains <= 0;
            int CDJudgmentRemains = Aimsharp.SpellCooldown("Judgment");
            bool CDJudgmentUp = CDJudgmentRemains <= 0;
            int CDCrusaderStrikeRemains = Aimsharp.SpellCooldown("Crusader Strike");
            bool CDCrusaderStrikeUp = CDCrusaderStrikeRemains <= 0;
            int CDCrusaderStrikeCharges = Aimsharp.SpellCharges("Crusader Strike");
            int CDCrusaderStrikeFullRecharge = (int)(Aimsharp.RechargeTime("Crusader Strike") + 0f * (Aimsharp.MaxCharges("Crusader Strike") - CDCrusaderStrikeCharges - 1) / (1f + Haste));
            float CDCrusaderStrikeFractional = CDCrusaderStrikeCharges + (1 - (Aimsharp.RechargeTime("Crusader Strike") - GCD) / (0f / (1f + Haste)));
            CDCrusaderStrikeFractional = CDCrusaderStrikeFractional > Aimsharp.MaxCharges("Crusader Strike") ? Aimsharp.MaxCharges("Crusader Strike") : CDCrusaderStrikeFractional;
            int CDConsecrationRemains = Aimsharp.SpellCooldown("Consecration");
            bool CDConsecrationUp = CDConsecrationRemains <= 0;


            //specific variables


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastLightsJudgment = Aimsharp.CanCast("Light's Judgment", "player") && !SaveCooldowns && Fighting;
            bool CanCastShieldofVengeance = Aimsharp.CanCast("Shield of Vengeance", "player") && !SaveCooldowns && Fighting;
            bool CanCastBlessingofSpring = Aimsharp.CanCast("Blessing of Spring", "player") && Fighting;
            bool CanCastBlessingofSummer = Aimsharp.CanCast("Blessing of Summer", "player") && Fighting;
            bool CanCastBlessingofAutumn = Aimsharp.CanCast("Blessing of Autumn", "player") && Fighting;
            bool CanCastBlessingofWinter = Aimsharp.CanCast("Blessing of Winter", "player") && Fighting;
            bool CanCastAvengingWrath = Aimsharp.CanCast("Avenging Wrath", "player") && !SaveCooldowns && Fighting && !TalentCrusade;
            bool CanCastCrusade = Aimsharp.CanCast("Crusade", "player") && !SaveCooldowns && Fighting;
            bool CanCastAshenHallow = Aimsharp.CanCast("Ashen Hallow", "player") && !SaveCooldowns && !Moving && Fighting;
            bool CanCastHolyAvenger = Aimsharp.CanCast("Holy Avenger", "player") && !SaveCooldowns && Fighting;
            bool CanCastFinalReckoning = Aimsharp.CanCast("Final Reckoning", "player") && !SaveCooldowns && Fighting;
            bool CanCastSeraphim = Aimsharp.CanCast("Seraphim", "player") && Fighting;
            bool CanCastVanquishersHammer = Aimsharp.CanCast("Vanquishers Hammer") && Fighting;
            bool CanCastExecutionSentence = Aimsharp.CanCast("Execution Sentence") && Fighting && !SaveCooldowns;
            bool CanCastDivineStorm = Aimsharp.CanCast("Divine Storm", "player") && Fighting;
            bool CanCastTemplarsVerdict = Aimsharp.CanCast("Templar's Verdict") && Fighting;
            bool CanCastDivineToll = Aimsharp.CanCast("Divine Toll", "player") && !SaveCooldowns && Fighting;
            bool CanCastWakeofAshes = Aimsharp.CanCast("Wake of Ashes", "player") && Fighting;
            bool CanCastBladeofJustice = Aimsharp.CanCast("Blade of Justice") && Fighting;
            bool CanCastHammerofWrath = Aimsharp.CanCast("Hammer of Wrath") && Fighting;
            bool CanCastJudgment = Aimsharp.CanCast("Judgment") && Fighting;
            bool CanCastCrusaderStrike = Aimsharp.CanCast("Crusader Strike") && Fighting;
            bool CanCastConsecration = Aimsharp.CanCast("Consecration", "player") && Fighting;
            bool CanCastArcaneTorrent = Aimsharp.CanCast("Arcane Torrent", "player") && !SaveCooldowns && Fighting;

            List<int> hpg = new List<int> { CDWakeofAshesRemains, CDBladeofJusticeRemains, CDHammerofWrathRemains, CDJudgmentRemains, CDCrusaderStrikeRemains };
            int time_to_hpg = hpg.Min();

            //actions.finishers=variable,name=ds_castable,value=spell_targets.divine_storm>=2|buff.empyrean_power.up&debuff.judgment.down&buff.divine_purpose.down|spell_targets.divine_storm>=2&buff.crusade.up&buff.crusade.stack<10
            bool ds_castable = EnemiesInMelee >= 2 || BuffEmpyreanPowerUp && !DebuffJudgmentUp && !BuffDivinePurposeUp || EnemiesInMelee >= 2 && BuffCrusadeUp && BuffCrusadeStacks < 10;

            bool ConsecrationUp = Aimsharp.TotemRemaining("Consecration") - GCD > 0;

            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions+=/call_action_list,name=cooldowns
            //actions.cooldowns=potion,if=(buff.bloodlust.react|buff.avenging_wrath.up&buff.avenging_wrath.remains>18|buff.crusade.up&buff.crusade.remains<25)
            if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
            {
                if ((BloodlustUp || BuffAvengingWrathUp && BuffAvengingWrathRemains > 18000 || BuffCrusadeUp && BuffCrusadeRemains < 25000))
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //actions.cooldowns+=/lights_judgment,if=spell_targets.lights_judgment>=2|(!raid_event.adds.exists|raid_event.adds.in>75)
            if (CanCastLightsJudgment)
            {
                if (EnemiesInMelee >= 2 || !AOE)
                {
                    Aimsharp.Cast("Light's Judgment");
                    return true;
                }
            }

            //actions.cooldowns+=/fireblood,if=buff.avenging_wrath.up|buff.crusade.up&buff.crusade.stack=10
            if (BuffAvengingWrathUp || BuffCrusadeUp && BuffCrusadeStacks >= 10)
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

            //actions.cooldowns+=/shield_of_vengeance
            if (CanCastShieldofVengeance)
            {
                Aimsharp.Cast("Shield of Vengeance");
                return true;
            }

            //actions.cooldowns+=/blessing_of_the_seasons
            if (CanCastBlessingofAutumn)
            {
                Aimsharp.Cast("Blessing of Autumn");
                return true;
            }
            if (CanCastBlessingofWinter)
            {
                Aimsharp.Cast("Blessing of Winter");
                return true;
            }
            if (CanCastBlessingofSpring)
            {
                Aimsharp.Cast("Blessing of Spring");
                return true;
            }
            if (CanCastBlessingofSummer)
            {
                Aimsharp.Cast("Blessing of Summer");
                return true;
            }

            //buff.avenging_wrath.up|buff.crusade.up&buff.crusade.stack>=10
            if (BuffAvengingWrathUp || BuffCrusadeUp && BuffCrusadeStacks >= 10)
            {
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
            }

            //actions.cooldowns+=/avenging_wrath,if=(holy_power>=4&time<5|holy_power>=3&time>5|talent.holy_avenger.enabled&cooldown.holy_avenger.remains=0)&time_to_hpg=0
            if (CanCastAvengingWrath)
            {
                if ((HolyPower >= 4 && Time < 5000 || HolyPower >= 3 && Time > 5000 || TalentHolyAvenger && CDHolyAvengerUp) && time_to_hpg<=0)
                {
                    Aimsharp.Cast("Avenging Wrath", true);
                    return true;
                }
            }

            //actions.cooldowns+=/crusade,if=(holy_power>=4&time<5|holy_power>=3&time>5|talent.holy_avenger.enabled&cooldown.holy_avenger.remains=0)&time_to_hpg=0
            if (CanCastCrusade)
            {
                if((HolyPower>=4 && Time<5000 || HolyPower>=3 && Time>5000 || TalentHolyAvenger && CDHolyAvengerUp)&& time_to_hpg<=0)
                {
                    Aimsharp.Cast("Crusade", true);
                    return true;
                }
            }

            //actions.cooldowns+=/ashen_hallow
            if (CanCastAshenHallow)
            {
                Aimsharp.Cast("AshenHallowC");
                return true;
            }

            //actions.cooldowns+=/holy_avenger,if=time_to_hpg=0&((buff.avenging_wrath.up|buff.crusade.up)|(buff.avenging_wrath.down&cooldown.avenging_wrath.remains>40|buff.crusade.down&cooldown.crusade.remains>40))
            if (CanCastHolyAvenger)
            {
                if (time_to_hpg<=0 && ((BuffAvengingWrathUp || BuffCrusadeUp) || (!BuffAvengingWrathUp&&CDAvengingWrathRemains>40000||!BuffCrusadeUp&&CDCrusadeRemains>40000)))
                {
                    Aimsharp.Cast("Holy Avenger", true);
                    return true;
                }
            }

            //actions.cooldowns+=/final_reckoning,if=holy_power>=3&cooldown.avenging_wrath.remains>gcd&time_to_hpg=0&(!talent.seraphim.enabled|buff.seraphim.up)
            if (CanCastFinalReckoning)
            {
                if (HolyPower>=3 && CDAvengingWrathRemains>GCDMAX && time_to_hpg<=0 && (!TalentSeraphim||BuffSeraphimUp))
                {
                    Aimsharp.Cast("FinalReckoningC");
                    return true;
                }
            }

            //actions+=/call_action_list,name=generators
            //actions.generators=call_action_list,name=finishers,if=holy_power>=5|buff.holy_avenger.up|debuff.final_reckoning.up|debuff.execution_sentence.up|buff.memory_of_lucid_dreams.up|buff.seething_rage.up
            if (HolyPower>=5||BuffHolyAvengerUp||DebuffFinalReckoningUp||DebuffExecutionSentenceUp)
            {
                //actions.finishers+=/seraphim,if=((!talent.crusade.enabled&buff.avenging_wrath.up|cooldown.avenging_wrath.remains>25)|(buff.crusade.up|cooldown.crusade.remains>25))&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains<10)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains<10)&time_to_hpg=0
                if (CanCastSeraphim)
                {
                    if(((!TalentCrusade && BuffAvengingWrathUp || CDAvengingWrathRemains>25000) || (BuffCrusadeUp || CDCrusadeRemains>25000)) && (!TalentFinalReckoning||CDFinalReckoningRemains<10000)&&(!TalentExecutionSentence || CDExecutionSentenceRemains<10000)&&time_to_hpg<=0)
                    {
                        Aimsharp.Cast("Seraphim");
                        return true;
                    }
                }

                //actions.finishers+=/vanquishers_hammer,if=(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*10|debuff.final_reckoning.up)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*10|debuff.execution_sentence.up)|spell_targets.divine_storm>=2
                if (CanCastVanquishersHammer)
                {
                    if((!TalentFinalReckoning||CDFinalReckoningRemains>GCDMAX*10||DebuffFinalReckoningUp)&&(!TalentExecutionSentence||CDExecutionSentenceRemains>GCDMAX*10 || DebuffExecutionSentenceUp)||EnemiesInMelee>=2)
                    {
                        Aimsharp.Cast("Vanquisher's Hammer");
                        return true;
                    }
                }

                //actions.finishers+=/execution_sentence,if=spell_targets.divine_storm<=3&((!talent.crusade.enabled|buff.crusade.down&cooldown.crusade.remains>10)|buff.crusade.stack>=3|cooldown.avenging_wrath.remains>10|debuff.final_reckoning.up)&time_to_hpg=0
                if (CanCastExecutionSentence)
                {
                    if(EnemiesInMelee<=3 && ((!TalentCrusade||!BuffCrusadeUp && CDCrusadeRemains>10000)||BuffCrusadeStacks>=3||CDAvengingWrathRemains>10000||DebuffFinalReckoningUp)&&time_to_hpg<=0)
                    {
                        Aimsharp.Cast("Execution Sentence");
                        return true;
                    }
                }

                //actions.finishers+=/divine_storm,if=variable.ds_castable&!buff.vanquishers_hammer.up&((!talent.crusade.enabled|cooldown.crusade.remains>gcd*3)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*3|spell_targets.divine_storm>=3)|spell_targets.divine_storm>=2&(talent.holy_avenger.enabled&cooldown.holy_avenger.remains<gcd*3|buff.crusade.up&buff.crusade.stack<10))
                if (CanCastDivineStorm)
                {
                    if(ds_castable&&!BuffVanquishersHammerUp&&((!TalentCrusade||CDCrusadeRemains>GCDMAX*3)&&(!TalentExecutionSentence||CDExecutionSentenceRemains>GCDMAX*3||EnemiesInMelee>=3)||EnemiesInMelee>=2&&(TalentHolyAvenger&&CDHolyAvengerRemains<GCDMAX*3||BuffCrusadeUp&&BuffCrusadeStacks<10)))
                    {
                        Aimsharp.Cast("Divine Storm");
                        return true;
                    }
                }

                //actions.finishers+=/templars_verdict,if=(!talent.crusade.enabled|cooldown.crusade.remains>gcd*3)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*3&spell_targets.divine_storm<=3)&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*3)&(!covenant.necrolord.enabled|cooldown.vanquishers_hammer.remains>gcd)|talent.holy_avenger.enabled&cooldown.holy_avenger.remains<gcd*3|buff.holy_avenger.up|buff.crusade.up&buff.crusade.stack<10|buff.vanquishers_hammer.up
                if (CanCastTemplarsVerdict)
                {
                    if ((!TalentCrusade||CDCrusadeRemains>GCDMAX*3)&&(!TalentExecutionSentence||CDExecutionSentenceRemains>GCDMAX*3&&EnemiesInMelee<=3)&&(!TalentFinalReckoning||CDFinalReckoningRemains>GCDMAX*3)&&(!CovenantNecrolord||CDVanquishersHammerRemains>GCDMAX)||TalentHolyAvenger&&CDHolyAvengerRemains<GCDMAX*3||BuffHolyAvengerUp||BuffCrusadeUp&& BuffCrusadeStacks<10||BuffVanquishersHammerUp)
                    {
                        Aimsharp.Cast("Templar's Verdict");
                        return true;
                    }
                }
            }

            //actions.generators+=/divine_toll,if=!debuff.judgment.up&(!raid_event.adds.exists|raid_event.adds.in>30)&(holy_power<=2|holy_power<=4&(cooldown.blade_of_justice.remains>gcd*2|debuff.execution_sentence.up|debuff.final_reckoning.up))&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*10)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*10)
            if (CanCastDivineToll)
            {
                if(!DebuffJudgmentUp&&(!AOE||EnemiesInMelee>=2)&&(HolyPower<=2||HolyPower<=4&&(CDBladeofJusticeRemains>GCDMAX*2||DebuffExecutionSentenceUp||DebuffFinalReckoningUp))&&(!TalentFinalReckoning||CDFinalReckoningRemains>GCDMAX*10)&&(!TalentExecutionSentence||CDExecutionSentenceRemains>GCDMAX*10))
                {
                    Aimsharp.Cast("Divine Toll");
                    return true;
                }
            }

            //actions.generators+=/wake_of_ashes,if=(holy_power=0|holy_power<=2&(cooldown.blade_of_justice.remains>gcd*2|debuff.execution_sentence.up|debuff.final_reckoning.up))&(!raid_event.adds.exists|raid_event.adds.in>20)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>15)&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>15)
            if (CanCastWakeofAshes)
            {
                if((HolyPower==0||HolyPower<=2&&(CDBladeofJusticeRemains>GCDMAX*2||DebuffExecutionSentenceUp||DebuffFinalReckoningUp))&&(!AOE||EnemiesInMelee>=2)&&(!TalentExecutionSentence||CDExecutionSentenceRemains>15000)&&(!TalentFinalReckoning||CDFinalReckoningRemains>15000))
                {
                    Aimsharp.Cast("Wake of Ashes");
                    return true;
                }
            }

            //actions.generators+=/blade_of_justice,if=holy_power<=3
            if (CanCastBladeofJustice)
            {
                if(HolyPower<=3)
                {
                    Aimsharp.Cast("Blade of Justice");
                    return true;
                }
            }

            //actions.generators+=/hammer_of_wrath,if=holy_power<=4
            if (CanCastHammerofWrath)
            {
                if (HolyPower <= 4)
                {
                    Aimsharp.Cast("Hammer of Wrath");
                    return true;
                }
            }

            //actions.generators+=/judgment,if=!debuff.judgment.up&(holy_power<=2|holy_power<=4&cooldown.blade_of_justice.remains>gcd*2)
            if (CanCastJudgment)
            {
                if(!DebuffJudgmentUp&&(HolyPower<=2||HolyPower<=4&&CDBladeofJusticeRemains>GCDMAX*2))
                {
                    Aimsharp.Cast("Judgment");
                    return true;
                }
            }

            //actions.generators+=/call_action_list,name=finishers,if=(target.health.pct<=20|buff.avenging_wrath.up|buff.crusade.up|buff.empyrean_power.up)
            if((TargetHealthPct<=20||BuffAvengingWrathUp||BuffCrusadeUp||BuffEmpyreanPowerUp))
            {
                //actions.finishers+=/seraphim,if=((!talent.crusade.enabled&buff.avenging_wrath.up|cooldown.avenging_wrath.remains>25)|(buff.crusade.up|cooldown.crusade.remains>25))&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains<10)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains<10)&time_to_hpg=0
                if (CanCastSeraphim)
                {
                    if (((!TalentCrusade && BuffAvengingWrathUp || CDAvengingWrathRemains > 25000) || (BuffCrusadeUp || CDCrusadeRemains > 25000)) && (!TalentFinalReckoning || CDFinalReckoningRemains < 10000) && (!TalentExecutionSentence || CDExecutionSentenceRemains < 10000) && time_to_hpg <= 0)
                    {
                        Aimsharp.Cast("Seraphim");
                        return true;
                    }
                }

                //actions.finishers+=/vanquishers_hammer,if=(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*10|debuff.final_reckoning.up)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*10|debuff.execution_sentence.up)|spell_targets.divine_storm>=2
                if (CanCastVanquishersHammer)
                {
                    if ((!TalentFinalReckoning || CDFinalReckoningRemains > GCDMAX * 10 || DebuffFinalReckoningUp) && (!TalentExecutionSentence || CDExecutionSentenceRemains > GCDMAX * 10 || DebuffExecutionSentenceUp) || EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Vanquisher's Hammer");
                        return true;
                    }
                }

                //actions.finishers+=/execution_sentence,if=spell_targets.divine_storm<=3&((!talent.crusade.enabled|buff.crusade.down&cooldown.crusade.remains>10)|buff.crusade.stack>=3|cooldown.avenging_wrath.remains>10|debuff.final_reckoning.up)&time_to_hpg=0
                if (CanCastExecutionSentence)
                {
                    if (EnemiesInMelee <= 3 && ((!TalentCrusade || !BuffCrusadeUp && CDCrusadeRemains > 10000) || BuffCrusadeStacks >= 3 || CDAvengingWrathRemains > 10000 || DebuffFinalReckoningUp) && time_to_hpg <= 0)
                    {
                        Aimsharp.Cast("Execution Sentence");
                        return true;
                    }
                }

                //actions.finishers+=/divine_storm,if=variable.ds_castable&!buff.vanquishers_hammer.up&((!talent.crusade.enabled|cooldown.crusade.remains>gcd*3)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*3|spell_targets.divine_storm>=3)|spell_targets.divine_storm>=2&(talent.holy_avenger.enabled&cooldown.holy_avenger.remains<gcd*3|buff.crusade.up&buff.crusade.stack<10))
                if (CanCastDivineStorm)
                {
                    if (ds_castable && !BuffVanquishersHammerUp && ((!TalentCrusade || CDCrusadeRemains > GCDMAX * 3) && (!TalentExecutionSentence || CDExecutionSentenceRemains > GCDMAX * 3 || EnemiesInMelee >= 3) || EnemiesInMelee >= 2 && (TalentHolyAvenger && CDHolyAvengerRemains < GCDMAX * 3 || BuffCrusadeUp && BuffCrusadeStacks < 10)))
                    {
                        Aimsharp.Cast("Divine Storm");
                        return true;
                    }
                }

                //actions.finishers+=/templars_verdict,if=(!talent.crusade.enabled|cooldown.crusade.remains>gcd*3)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*3&spell_targets.divine_storm<=3)&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*3)&(!covenant.necrolord.enabled|cooldown.vanquishers_hammer.remains>gcd)|talent.holy_avenger.enabled&cooldown.holy_avenger.remains<gcd*3|buff.holy_avenger.up|buff.crusade.up&buff.crusade.stack<10|buff.vanquishers_hammer.up
                if (CanCastTemplarsVerdict)
                {
                    if ((!TalentCrusade || CDCrusadeRemains > GCDMAX * 3) && (!TalentExecutionSentence || CDExecutionSentenceRemains > GCDMAX * 3 && EnemiesInMelee <= 3) && (!TalentFinalReckoning || CDFinalReckoningRemains > GCDMAX * 3) && (!CovenantNecrolord || CDVanquishersHammerRemains > GCDMAX) || TalentHolyAvenger && CDHolyAvengerRemains < GCDMAX * 3 || BuffHolyAvengerUp || BuffCrusadeUp && BuffCrusadeStacks < 10 || BuffVanquishersHammerUp)
                    {
                        Aimsharp.Cast("Templar's Verdict");
                        return true;
                    }
                }
            }

            //actions.generators+=/crusader_strike,if=cooldown.crusader_strike.charges_fractional>=1.75&(holy_power<=2|holy_power<=3&cooldown.blade_of_justice.remains>gcd*2|holy_power=4&cooldown.blade_of_justice.remains>gcd*2&cooldown.judgment.remains>gcd*2)
            if (CanCastCrusaderStrike)
            {
                if(CDCrusaderStrikeFractional>=1.75&&(HolyPower<=2||HolyPower<=3&&CDBladeofJusticeRemains>GCDMAX*2||HolyPower==4&&CDBladeofJusticeRemains>GCDMAX*2&&CDJudgmentRemains>GCDMAX*2))
                {
                    Aimsharp.Cast("Crusader Strike");
                    return true;
                }
            }

            //actions.generators+=/call_action_list,name=finishers
            //actions.finishers+=/seraphim,if=((!talent.crusade.enabled&buff.avenging_wrath.up|cooldown.avenging_wrath.remains>25)|(buff.crusade.up|cooldown.crusade.remains>25))&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains<10)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains<10)&time_to_hpg=0
            if (CanCastSeraphim)
            {
                if (((!TalentCrusade && BuffAvengingWrathUp || CDAvengingWrathRemains > 25000) || (BuffCrusadeUp || CDCrusadeRemains > 25000)) && (!TalentFinalReckoning || CDFinalReckoningRemains < 10000) && (!TalentExecutionSentence || CDExecutionSentenceRemains < 10000) && time_to_hpg <= 0)
                {
                    Aimsharp.Cast("Seraphim");
                    return true;
                }
            }

            //actions.finishers+=/vanquishers_hammer,if=(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*10|debuff.final_reckoning.up)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*10|debuff.execution_sentence.up)|spell_targets.divine_storm>=2
            if (CanCastVanquishersHammer)
            {
                if ((!TalentFinalReckoning || CDFinalReckoningRemains > GCDMAX * 10 || DebuffFinalReckoningUp) && (!TalentExecutionSentence || CDExecutionSentenceRemains > GCDMAX * 10 || DebuffExecutionSentenceUp) || EnemiesInMelee >= 2)
                {
                    Aimsharp.Cast("Vanquisher's Hammer");
                    return true;
                }
            }

            //actions.finishers+=/execution_sentence,if=spell_targets.divine_storm<=3&((!talent.crusade.enabled|buff.crusade.down&cooldown.crusade.remains>10)|buff.crusade.stack>=3|cooldown.avenging_wrath.remains>10|debuff.final_reckoning.up)&time_to_hpg=0
            if (CanCastExecutionSentence)
            {
                if (EnemiesInMelee <= 3 && ((!TalentCrusade || !BuffCrusadeUp && CDCrusadeRemains > 10000) || BuffCrusadeStacks >= 3 || CDAvengingWrathRemains > 10000 || DebuffFinalReckoningUp) && time_to_hpg <= 0)
                {
                    Aimsharp.Cast("Execution Sentence");
                    return true;
                }
            }

            //actions.finishers+=/divine_storm,if=variable.ds_castable&!buff.vanquishers_hammer.up&((!talent.crusade.enabled|cooldown.crusade.remains>gcd*3)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*3|spell_targets.divine_storm>=3)|spell_targets.divine_storm>=2&(talent.holy_avenger.enabled&cooldown.holy_avenger.remains<gcd*3|buff.crusade.up&buff.crusade.stack<10))
            if (CanCastDivineStorm)
            {
                if (ds_castable && !BuffVanquishersHammerUp && ((!TalentCrusade || CDCrusadeRemains > GCDMAX * 3) && (!TalentExecutionSentence || CDExecutionSentenceRemains > GCDMAX * 3 || EnemiesInMelee >= 3) || EnemiesInMelee >= 2 && (TalentHolyAvenger && CDHolyAvengerRemains < GCDMAX * 3 || BuffCrusadeUp && BuffCrusadeStacks < 10)))
                {
                    Aimsharp.Cast("Divine Storm");
                    return true;
                }
            }

            //actions.finishers+=/templars_verdict,if=(!talent.crusade.enabled|cooldown.crusade.remains>gcd*3)&(!talent.execution_sentence.enabled|cooldown.execution_sentence.remains>gcd*3&spell_targets.divine_storm<=3)&(!talent.final_reckoning.enabled|cooldown.final_reckoning.remains>gcd*3)&(!covenant.necrolord.enabled|cooldown.vanquishers_hammer.remains>gcd)|talent.holy_avenger.enabled&cooldown.holy_avenger.remains<gcd*3|buff.holy_avenger.up|buff.crusade.up&buff.crusade.stack<10|buff.vanquishers_hammer.up
            if (CanCastTemplarsVerdict)
            {
                if ((!TalentCrusade || CDCrusadeRemains > GCDMAX * 3) && (!TalentExecutionSentence || CDExecutionSentenceRemains > GCDMAX * 3 && EnemiesInMelee <= 3) && (!TalentFinalReckoning || CDFinalReckoningRemains > GCDMAX * 3) && (!CovenantNecrolord || CDVanquishersHammerRemains > GCDMAX) || TalentHolyAvenger && CDHolyAvengerRemains < GCDMAX * 3 || BuffHolyAvengerUp || BuffCrusadeUp && BuffCrusadeStacks < 10 || BuffVanquishersHammerUp)
                {
                    Aimsharp.Cast("Templar's Verdict");
                    return true;
                }
            }

            //actions.generators+=/consecration,if=!consecration.up
            if (CanCastConsecration)
            {
                if (!ConsecrationUp)
                {
                    Aimsharp.Cast("Consecration");
                    return true;
                }
            }

            //actions.generators+=/crusader_strike,if=holy_power<=4
            if (CanCastCrusaderStrike)
            {
                if (HolyPower<=4)
                {
                    Aimsharp.Cast("Crusader Strike");
                    return true;
                }
            }

            //actions.generators+=/arcane_torrent,if=holy_power<=4
            if (CanCastArcaneTorrent)
            {
                if (HolyPower <= 4)
                {
                    Aimsharp.Cast("Arcane Torrent");
                    return true;
                }
            }

            //actions.generators+=/consecration,if=time_to_hpg>gcd
            if (CanCastConsecration)
            {
                if (time_to_hpg>GCDMAX)
                {
                    Aimsharp.Cast("Consecration");
                    return true;
                }
            }


            //fixes for execution sentence build
            if (TalentExecutionSentence && TalentSeraphim && TalentFinalReckoning && HolyPower >= 5)
            {
                if (CanCastDivineStorm && EnemiesInMelee > 1)
                {
                    Aimsharp.Cast("Divine Storm");
                    return true;
                }
            }

            if (TalentExecutionSentence && TalentSeraphim && TalentFinalReckoning && SaveCooldowns && HolyPower >= 5)
            {
                if (CanCastTemplarsVerdict)
                {
                    Aimsharp.Cast("Templar's Verdict");
                    return true;
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
