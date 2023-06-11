using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsFury : Rotation
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
           "Rampage","Recklessness","Whirlwind","Light's Judgment","Crushing Blow","Siegebreaker","Condemn","Execute","Bladestorm","Bloodthirst","Bloodbath","Dragon Roar","Onslaught","Raging Blow",
        };

        List<string> BuffsList = new List<string>
        {
            "Whirlwind","Recklessness","Enrage","Will of the Berserker","Frenzy","Sudden Death"
        };

        List<string> DebuffsList = new List<string>
        {
           "Siegebreaker",
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

            Settings.Add(new Setting("Warrior Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Will of the Berserker", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Fury", Color.Purple);
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
            int Rage = Aimsharp.Power("player");
            int RageMax = Aimsharp.PlayerMaxPower();
            int RageDefecit = RageMax - Rage;

            //Talents
            bool TalentRecklessAbandon = Aimsharp.Talent(7, 2);
            bool TalentAngerManagement = Aimsharp.Talent(7, 1);
            bool TalentMassacre = Aimsharp.Talent(3, 1);
            bool TalentCruelty = Aimsharp.Talent(5, 3);


            //buffs
            int BuffWhirlwindRemains = Aimsharp.BuffRemaining("Whirlwind") - GCD;
            bool BuffWhirlwindUp = BuffWhirlwindRemains > 0;
            int BuffRecklessnessRemains = Aimsharp.BuffRemaining("Recklessness") - GCD;
            bool BuffRecklessnessUp = BuffRecklessnessRemains > 0;
            int BuffEnrageRemains = Aimsharp.BuffRemaining("Enrage") - GCD;
            bool BuffEnrageUp = BuffEnrageRemains > 0;
            int BuffWilloftheBerserkerRemains = Aimsharp.BuffRemaining("Will of the Berserker") - GCD;
            bool BuffWilloftheBerserkerUp = BuffWilloftheBerserkerRemains > 0;
            int BuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy") - GCD;
            bool BuffFrenzyUp = BuffFrenzyRemains > 0;


            //debuffs
            int DebuffSiegebreakerRemains = Aimsharp.DebuffRemaining("Siegebreaker") - GCD;
            bool DebuffSiegebreakerUp = DebuffSiegebreakerRemains > 0;


            //cooldowns
            int CDRecklessnessRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Recklessness");
            bool CDRecklessnessUp = CDRecklessnessRemains <= 0;
            int CDCrushingBlowCharges = Aimsharp.SpellCharges("Crushing Blow");
            int CDCrushingBlowFullRecharge = (int)(Aimsharp.RechargeTime("Crushing Blow") + 8000f * (Aimsharp.MaxCharges("Crushing Blow") - CDCrushingBlowCharges - 1) / (1f + Haste));
            float CDCrushingBlowFractional = CDCrushingBlowCharges + (1 - (Aimsharp.RechargeTime("Crushing Blow") - GCD) / (8000f / (1f + Haste)));
            CDCrushingBlowFractional = CDCrushingBlowFractional > Aimsharp.MaxCharges("Crushing Blow") ? Aimsharp.MaxCharges("Crushing Blow") : CDCrushingBlowFractional;
            int CDRagingBlowCharges = Aimsharp.SpellCharges("Raging Blow");
            int CDRagingBlowFullRecharge = (int)(Aimsharp.RechargeTime("Raging Blow") + 8000f * (Aimsharp.MaxCharges("Raging Blow") - CDRagingBlowCharges - 1) / (1f + Haste));
            float CDRagingBlowFractional = CDRagingBlowCharges + (1 - (Aimsharp.RechargeTime("Raging Blow") - GCD) / (8000f / (1f + Haste)));
            CDRagingBlowFractional = CDRagingBlowFractional > Aimsharp.MaxCharges("Raging Blow") ? Aimsharp.MaxCharges("Raging Blow") : CDRagingBlowFractional;


            //specific variables
            bool RuneforgeWilloftheBerserker = RuneforgePower == "Will of the Berserker";
            bool ConduitViciousContempt = ActiveConduits.Contains(337302);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastRampage = Aimsharp.CanCast("Rampage") && Fighting;
            bool CanCastRecklessness = Aimsharp.CanCast("Recklessness", "player") && !SaveCooldowns && Fighting;
            bool CanCastWhirlwind = Aimsharp.CanCast("Whirlwind", "player") && Fighting;
            bool CanCastLightsJudgment = Aimsharp.CanCast("Light's Judgment", "player") && !SaveCooldowns && Fighting;
            bool CanCastCrushingBlow = Aimsharp.CanCast("Crushing Blow") && Fighting;
            bool CanCastSiegebreaker = Aimsharp.CanCast("Siegebreaker") && Fighting;
            bool CanCastCondemn = Aimsharp.CanCast("Condemn") && Fighting && CovenantVenthyr && (TargetHealthPct>=80||TargetHealthPct<=20||Aimsharp.HasBuff("Sudden Death"));
            bool CanCastExecute = Aimsharp.CanCast("Execute") && Fighting && !CovenantVenthyr;
            bool CanCastBladestorm = Aimsharp.CanCast("Bladestorm", "player") && !SaveCooldowns && Fighting;
            bool CanCastBloodthirst = Aimsharp.CanCast("Bloodthirst") && Fighting;
            bool CanCastBloodbath = Aimsharp.CanCast("Bloodbath") && Fighting;
            bool CanCastDragonRoar = Aimsharp.CanCast("Dragon Roar", "player") && Fighting;
            bool CanCastOnslaught = Aimsharp.CanCast("Onslaught") && Fighting;
            bool CanCastRagingBlow = Aimsharp.CanCast("Raging Blow") && Fighting;





            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;


            //actions+=/potion
            if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
            {
                Aimsharp.Cast("DPS Pot", true);
                return true;
            }

            //actions+=/rampage,if=cooldown.recklessness.remains<3&talent.reckless_abandon.enabled
            if (CanCastRampage)
                if (CDRecklessnessRemains < 3000 && TalentRecklessAbandon)
                    return Rampage();

            //actions+=/recklessness,if=gcd.remains=0&((buff.bloodlust.up|talent.anger_management.enabled|raid_event.adds.in>10)|target.time_to_die>100|(talent.massacre.enabled&target.health.pct<35)|target.health.pct<20|target.time_to_die<15&raid_event.adds.in>10)&(spell_targets.whirlwind=1|buff.meat_cleaver.up)
            if (CanCastRecklessness)
                if (((BloodlustUp || TalentAngerManagement || EnemiesInMelee <= 1) || TargetTimeToDie > 100000 || (TalentMassacre && TargetHealthPct < 35) || TargetHealthPct < 20 || TargetTimeToDie < 15000 && EnemiesInMelee<=1) && (EnemiesInMelee == 1 || BuffWhirlwindUp))
                    return Recklessness();

            //actions+=/whirlwind,if=spell_targets.whirlwind>1&!buff.meat_cleaver.up|raid_event.adds.in<gcd&!buff.meat_cleaver.up
            if (CanCastWhirlwind)
                if (EnemiesInMelee > 1 && !BuffWhirlwindUp)
                    return Whirlwind();

            //items
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

            //actions+=/berserking,if=buff.recklessness.up
            if (BuffRecklessnessUp)
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

            if (CanCastLightsJudgment)
                if (!BuffRecklessnessUp && !DebuffSiegebreakerUp)
                    return LightsJudgment();

            //actions+=/run_action_list,name=single_target
            //actions.single_target=raging_blow,if=runeforge.will_of_the_berserker.equipped&buff.will_of_the_berserker.remains<gcd
            if (CanCastRagingBlow)
                if (RuneforgeWilloftheBerserker && BuffWilloftheBerserkerRemains < GCDMAX)
                    return RagingBlow();

            //actions.single_target+=/crushing_blow,if=runeforge.will_of_the_berserker.equipped&buff.will_of_the_berserker.remains<gcd
            if (CanCastCrushingBlow)
                if (RuneforgeWilloftheBerserker && BuffWilloftheBerserkerRemains < GCDMAX)
                    return CrushingBlow();

            //actions.single_target+=/siegebreaker,if=spell_targets.whirlwind>1|raid_event.adds.in>15
            if (CanCastSiegebreaker)
                    return Siegebreaker();

            //actions.single_target+=/rampage,if=buff.recklessness.up|(buff.enrage.remains<gcd|rage>90)|buff.frenzy.remains<1.5
            if (CanCastRampage)
                if (BuffRecklessnessUp || (BuffEnrageRemains < GCDMAX || Rage > 90) || BuffFrenzyRemains < 1500)
                    return Rampage();

            //actions.single_target+=/condemn
            if (CanCastCondemn)
                return Condemn();

            if (CanCastExecute)
                return Execute();

            //actions.single_target+=/bladestorm,if=buff.enrage.up&(spell_targets.whirlwind>1|raid_event.adds.in>45)
            if (CanCastBladestorm)
                if (BuffEnrageUp)
                    return Bladestorm();

            //actions.single_target+=/bloodthirst,if=buff.enrage.down|conduit.vicious_contempt.rank>5&target.health.pct<35&!talent.cruelty.enabled
            if (CanCastBloodthirst)
                if (!BuffEnrageUp || ConduitViciousContempt && TargetHealthPct < 35 && !TalentCruelty) //need to implement conduit ranks
                    return Bloodthirst();

            //actions.single_target+=/bloodbath,if=buff.enrage.down|conduit.vicious_contempt.rank>5&target.health.pct<35&!talent.cruelty.enabled
            if (CanCastBloodbath)
                if (!BuffEnrageUp || ConduitViciousContempt && TargetHealthPct < 35 && !TalentCruelty)
                    return Bloodbath();

            //actions.single_target+=/dragon_roar,if=buff.enrage.up&(spell_targets.whirlwind>1|raid_event.adds.in>15)
            if (CanCastDragonRoar)
                if (BuffEnrageUp)
                    return DragonRoar();

            //actions.single_target+=/onslaught
            if (CanCastOnslaught)
                return Onslaught();

            //actions.single_target+=/raging_blow,if=charges=2
            if (CanCastRagingBlow)
                if (CDRagingBlowCharges == 2)
                    return RagingBlow();

            //actions.single_target+=/crushing_blow,if=charges=2
            if (CanCastCrushingBlow)
                if (CDCrushingBlowCharges == 2)
                    return CrushingBlow();

            //actions.single_target+=/bloodthirst
            if (CanCastBloodthirst)
                    return Bloodthirst();

            if (CanCastBloodbath)
                    return Bloodbath();

            if (CanCastRagingBlow)
                    return RagingBlow();

            if (CanCastCrushingBlow)
                    return CrushingBlow();

            //actions.single_target+=/whirlwind
            if (CanCastWhirlwind)
                return Whirlwind();

            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

        bool Rampage() { Aimsharp.Cast("Rampage"); return true; }
        bool Recklessness() { Aimsharp.Cast("Recklessness"); return true; }
        bool Whirlwind() { Aimsharp.Cast("Whirlwind"); return true; }
        bool LightsJudgment() { Aimsharp.Cast("Light's Judgment"); return true; }
        bool CrushingBlow() { Aimsharp.Cast("Crushing Blow"); return true; }
        bool Siegebreaker() { Aimsharp.Cast("Siegebreaker"); return true; }
        bool Condemn() { Aimsharp.Cast("Condemn"); return true; }
        bool Execute() { Aimsharp.Cast("Execute"); return true; }
        bool Bladestorm() { Aimsharp.Cast("Bladestorm"); return true; }
        bool Bloodthirst() { Aimsharp.Cast("Bloodthirst"); return true; }
        bool Bloodbath() { Aimsharp.Cast("Bloodbath"); return true; }
        bool DragonRoar() { Aimsharp.Cast("Dragon Roar"); return true; }
        bool Onslaught() { Aimsharp.Cast("Onslaught"); return true; }
        bool RagingBlow() { Aimsharp.Cast("Raging Blow"); return true; }


    }
}
