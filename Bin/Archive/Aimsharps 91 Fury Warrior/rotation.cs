using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class Aimsharp9_1Fury : Rotation
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
           "Rampage","Recklessness","Whirlwind","Bladestorm","Condemn","Siegebreaker","Crushing Blow","Execute","Bloodthirst","Bloodbath","Dragon Roar","Onslaught","Raging Blow","Spear of Bastion"
        };

        List<string> BuffsList = new List<string>
        {
            "Whirlwind","Recklessness","Siegebreaker","Enrage","First Strike","Will of the Berserker","Frenzy","Merciless Bonegrinder","Bladestorm"
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
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Signet of Tormented Kings", "Sinful Surge", "Elysian Might", "Will of the Berserker", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Aimsharp 9_1Fury", Color.Purple);
            Aimsharp.PrintMessage("Version 2.1", Color.Purple);

            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

            Aimsharp.Latency = 100;
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

            Macros.Add("CancelBuff", "/cancelaura Bladestorm");



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
            int Runes = Aimsharp.PlayerSecondaryPower();
            int MaxRage = Aimsharp.PlayerMaxPower();
            int RageDefecit = MaxRage - Rage;

            //Talents
            bool TalentMassacre = Aimsharp.Talent(3, 1);
            bool TalentRecklessAbandon = Aimsharp.Talent(7, 2);
            bool TalentAngerManagement = Aimsharp.Talent(7, 1);
            bool TalentCruelty = Aimsharp.Talent(5, 3);


            //buffs
            int BuffWhirlwindRemains = Aimsharp.BuffRemaining("Whirlwind") - GCD;
            bool BuffWhirlwindUp = BuffWhirlwindRemains > 0;
            int BuffRecklessnessRemains = Aimsharp.BuffRemaining("Recklessness") - GCD;
            bool BuffRecklessnessUp = BuffRecklessnessRemains > 0;
            int BuffSiegebreakerRemains = Aimsharp.BuffRemaining("Siegebreaker") - GCD;
            bool BuffSiegebreakerUp = BuffSiegebreakerRemains > 0;
            int BuffEnrageRemains = Aimsharp.BuffRemaining("Enrage") - GCD;
            bool BuffEnrageUp = BuffEnrageRemains > 0;
            int BuffFirstStrikeRemains = Aimsharp.BuffRemaining("First Strike") - GCD;
            bool BuffFirstStrikeUp = BuffFirstStrikeRemains > 0;
            int BuffWilloftheBerserkerRemains = Aimsharp.BuffRemaining("Will of the Berserker") - GCD;
            bool BuffWilloftheBerserkerUp = BuffWilloftheBerserkerRemains > 0;
            int BuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy") - GCD;
            bool BuffFrenzyUp = BuffFrenzyRemains > 0;
            int BuffMercilessBonegrinderRemains = Aimsharp.BuffRemaining("Merciless Bonegrinder") - GCD;
            bool BuffMercilessBonegrinderUp = BuffMercilessBonegrinderRemains > 0;


            //debuffs
            int DebuffSiegebreakerRemains = Aimsharp.DebuffRemaining("Siegebreaker") - GCD;
            bool DebuffSiegebreakerUp = DebuffSiegebreakerRemains > 0;


            //cooldowns
            int CDRecklessnessRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Recklessness");
            bool CDRecklessnessUp = CDRecklessnessRemains <= 0;
            int CDSpearofBastionRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Spear of Bastion");
            bool CDSpearofBastionUp = CDSpearofBastionRemains <= 0;
            int CDCondemnRemains = Aimsharp.SpellCooldown("Condemn");
            bool CDCondemnUp = CDCondemnRemains <= 0;
            int CDCrushingBlowCharges = Aimsharp.SpellCharges("Crushing Blow");
            int CDCrushingBlowFullRecharge = (int)(Aimsharp.RechargeTime("Crushing Blow") + 8000f * (Aimsharp.MaxCharges("Crushing Blow") - CDCrushingBlowCharges - 1) / (1f + Haste));
            float CDCrushingBlowFractional = CDCrushingBlowCharges + (1 - (Aimsharp.RechargeTime("Crushing Blow") - GCD) / (8000f / (1f + Haste)));
            CDCrushingBlowFractional = CDCrushingBlowFractional > Aimsharp.MaxCharges("Crushing Blow") ? Aimsharp.MaxCharges("Crushing Blow") : CDCrushingBlowFractional;
            int CDRagingBlowCharges = Aimsharp.SpellCharges("Raging Blow");
            int CDRagingBlowFullRecharge = (int)(Aimsharp.RechargeTime("Raging Blow") + 8000f * (Aimsharp.MaxCharges("Raging Blow") - CDRagingBlowCharges - 1) / (1f + Haste));
            float CDRagingBlowFractional = CDRagingBlowCharges + (1 - (Aimsharp.RechargeTime("Raging Blow") - GCD) / (8000f / (1f + Haste)));
            CDRagingBlowFractional = CDRagingBlowFractional > Aimsharp.MaxCharges("Raging Blow") ? Aimsharp.MaxCharges("Raging Blow") : CDRagingBlowFractional;


            //specific variables
            bool RuneforgeSignetofTormentedKings = RuneforgePower == "Signet of Tormented Kings";
            bool RuneforgeSinfulSurge = RuneforgePower == "Sinful Surge";
            bool RuneforgeElysianMight = RuneforgePower == "Elysian Might";
            bool RuneforgeWilloftheBerserker = RuneforgePower == "Will of the Berserker";
            bool ConduitFirstStrike = ActiveConduits.Contains(325069);
            bool ConduitViciousContempt = ActiveConduits.Contains(337302);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastRampage = Aimsharp.CanCast("Rampage") && Fighting;
            bool CanCastRecklessness = Aimsharp.CanCast("Recklessness") && !SaveCooldowns && Fighting;
            bool CanCastWhirlwind = Aimsharp.CanCast("Whirlwind", "player") && Fighting;
            bool CanCastBladestorm = Aimsharp.CanCast("Bladestorm", "player") && !SaveCooldowns && Fighting;
            bool CanCastCondemn = Aimsharp.CanCast("Condemn") && Fighting;
            bool CanCastSiegebreaker = Aimsharp.CanCast("Siegebreaker") && Fighting;
            bool CanCastCrushingBlow = Aimsharp.CanCast("Crushing Blow") && Fighting;
            bool CanCastExecute = Aimsharp.CanCast("Execute") && Fighting;
            bool CanCastBloodthirst = Aimsharp.CanCast("Bloodthirst") && Fighting;
            bool CanCastBloodbath = Aimsharp.CanCast("Bloodbath") && Fighting;
            bool CanCastDragonRoar = Aimsharp.CanCast("Dragon Roar", "player") && Fighting;
            bool CanCastOnslaught = Aimsharp.CanCast("Onslaught") && Fighting;
            bool CanCastRagingBlow = Aimsharp.CanCast("Raging Blow") && Fighting;





            // end of Simc conditionals
            #endregion

            //actions+=/variable,name=execute_phase,value=talent.massacre&target.health.pct<35|target.health.pct<20|target.health.pct>80&covenant.venthyr
            bool execute_phase = TalentMassacre && TargetHealthPct < 35 || TargetHealthPct < 20 || TargetHealthPct > 80 && CovenantVenthyr;

            //actions+=/variable,name=unique_legendaries,value=runeforge.signet_of_tormented_kings|runeforge.sinful_surge|runeforge.elysian_might
            bool unique_legendaries = RuneforgeSignetofTormentedKings || RuneforgeSinfulSurge || RuneforgeElysianMight;




            //never interrupt channels 
            if (IsChanneling)
                return false;

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

            if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
            {
                Aimsharp.Cast("DPS Pot", true);
                return true;
            }

            //actions+=/rampage,if=cooldown.recklessness.remains<3&talent.reckless_abandon.enabled
            if (CanCastRampage)
            {
                if (CDRecklessnessRemains < 3000 && TalentRecklessAbandon)
                    return Rampage();
            }

            //actions+=/recklessness,if=runeforge.sinful_surge&gcd.remains=0&(variable.execute_phase|(target.time_to_pct_35>40&talent.anger_management|target.time_to_pct_35>70&!talent.anger_management))&(spell_targets.whirlwind=1|buff.meat_cleaver.up)
            if (CanCastRecklessness)
            {
                if (RuneforgeSinfulSurge && (EnemiesInMelee == 1 || BuffWhirlwindUp))
                    return Recklessness();
            }

            //actions+=/recklessness,if=runeforge.elysian_might&gcd.remains=0&(cooldown.spear_of_bastion.remains<5|cooldown.spear_of_bastion.remains>20)&((buff.bloodlust.up|talent.anger_management.enabled|raid_event.adds.in>10)|target.time_to_die>100|variable.execute_phase|target.time_to_die<15&raid_event.adds.in>10)&(spell_targets.whirlwind=1|buff.meat_cleaver.up)
            if (CanCastRecklessness)
            {
                if (RuneforgeElysianMight && (CDSpearofBastionRemains<5000 || CDSpearofBastionRemains>20000) && ((BloodlustUp || TalentAngerManagement) || TargetTimeToDie>100000 || execute_phase) && (EnemiesInMelee==1 || BuffWhirlwindUp))
                    return Recklessness();
            }

            //actions+=/recklessness,if=!variable.unique_legendaries&gcd.remains=0&((buff.bloodlust.up|talent.anger_management.enabled|raid_event.adds.in>10)|target.time_to_die>100|variable.execute_phase|target.time_to_die<15&raid_event.adds.in>10)&(spell_targets.whirlwind=1|buff.meat_cleaver.up)
            if (CanCastRecklessness)
            {
                if (unique_legendaries && ((BloodlustUp || TalentAngerManagement) || TargetTimeToDie > 100000 || execute_phase) && (EnemiesInMelee == 1 || BuffWhirlwindUp))
                    return Recklessness();
            }

            //actions+=/recklessness,use_off_gcd=1,if=runeforge.signet_of_tormented_kings.equipped&gcd.remains&prev_gcd.1.rampage&((buff.bloodlust.up|talent.anger_management.enabled|raid_event.adds.in>10)|target.time_to_die>100|variable.execute_phase|target.time_to_die<15&raid_event.adds.in>10)&(spell_targets.whirlwind=1|buff.meat_cleaver.up)
            if (CanCastRecklessness)
            {
                if (RuneforgeSignetofTormentedKings && LastCast == "Rampage" && ((BloodlustUp || TalentAngerManagement) || TargetTimeToDie > 100000 || execute_phase) && (EnemiesInMelee == 1 || BuffWhirlwindUp))
                    return Recklessness();
            }

            //actions+=/whirlwind,if=spell_targets.whirlwind>1&!buff.meat_cleaver.up|raid_event.adds.in<gcd&!buff.meat_cleaver.up
            if (CanCastWhirlwind)
            {
                if (EnemiesInMelee > 1 && !BuffWhirlwindUp)
                    return Whirlwind();
            }

            //actions+=/blood_fury
            foreach (string Racial in Racials)
            {
                if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns)
                {
                    Aimsharp.Cast(Racial, true);
                    return true;
                }
            }

            //actions+=/call_action_list,name=aoe
            //actions.aoe=cancel_buff,name=bladestorm,if=spell_targets.whirlwind>1&gcd.remains=0&soulbind.first_strike&buff.first_strike.remains&buff.enrage.remains<gcd
            if (EnemiesInMelee>1 && GCD<=0 && ConduitFirstStrike && BuffFirstStrikeUp && BuffEnrageRemains<GCDMAX)
            {
                Aimsharp.Cast("CancelBuff");
                return true;
            }

            //actions.aoe+=/bladestorm,if=buff.enrage.up&spell_targets.whirlwind>2
            if (CanCastBladestorm)
            {
                if (BuffEnrageUp && EnemiesInMelee > 2)
                    return Bladestorm();
            }

            //actions.aoe+=/condemn,if=spell_targets.whirlwind>1&(buff.enrage.up|buff.recklessness.up&runeforge.sinful_surge)&variable.execute_phase
            if (CanCastCondemn)
            {
                if (EnemiesInMelee > 1 && (BuffEnrageUp || BuffRecklessnessUp && RuneforgeSinfulSurge) && execute_phase)
                    return Condemn();
            }

            //actions.aoe+=/siegebreaker,if=spell_targets.whirlwind>1
            if (CanCastSiegebreaker)
            {
                if (EnemiesInMelee > 1)
                    return Siegebreaker();
            }

            //actions.aoe+=/rampage,if=spell_targets.whirlwind>1
            if (CanCastRampage)
            {
                if (EnemiesInMelee > 1)
                    return Rampage();
            }

            //actions.aoe+=/bladestorm,if=buff.enrage.remains>gcd*2.5&spell_targets.whirlwind>1
            if (CanCastBladestorm)
            {
                if (BuffEnrageRemains > GCDMAX * 2.5 && EnemiesInMelee > 1)
                    return Bladestorm();
            }

            //actions.single_target=raging_blow,if=runeforge.will_of_the_berserker.equipped&buff.will_of_the_berserker.remains<gcd
            if (CanCastRagingBlow)
            {
                if (RuneforgeWilloftheBerserker && BuffWilloftheBerserkerRemains < GCDMAX)
                    return RagingBlow();
            }

            //actions.single_target+=/crushing_blow,if=runeforge.will_of_the_berserker.equipped&buff.will_of_the_berserker.remains<gcd
            if (CanCastCrushingBlow)
            {
                if (RuneforgeWilloftheBerserker && BuffWilloftheBerserkerRemains < GCDMAX)
                    return CrushingBlow();
            }

            //actions.single_target+=/cancel_buff,name=bladestorm,if=spell_targets.whirlwind=1&gcd.remains=0&(talent.massacre.enabled|covenant.venthyr.enabled)&variable.execute_phase&(rage>90|!cooldown.condemn.remains)
            if (EnemiesInMelee == 1 && GCD <= 0 && (TalentMassacre||CovenantVenthyr)&&execute_phase&&(Rage>90 || CDCondemnUp))
            {
                Aimsharp.Cast("CancelBuff");
                return true;
            }

            //actions.single_target+=/condemn,if=(buff.enrage.up|buff.recklessness.up&runeforge.sinful_surge)&variable.execute_phase
            if (CanCastCondemn)
            {
                if ((BuffEnrageUp || BuffRecklessnessUp && RuneforgeSinfulSurge) && execute_phase)
                    return Condemn();
            }

            //actions.single_target+=/siegebreaker,if=spell_targets.whirlwind>1|raid_event.adds.in>15
            if (CanCastSiegebreaker)
            {
                return Siegebreaker();
            }

            //actions.single_target+=/rampage,if=buff.recklessness.up|(buff.enrage.remains<gcd|rage>90)|buff.frenzy.remains<1.5
            if (CanCastRampage)
            {
                if (BuffRecklessnessUp || (BuffEnrageRemains < GCDMAX || Rage > 90) || BuffFrenzyRemains < 1500)
                    return Rampage();
            }

            //actions.single_target+=/condemn
            //actions.single_target +=/ execute
            if (CanCastCondemn)
                return Condemn();

            if (CanCastExecute)
                return Execute();

            //actions.single_target+=/bladestorm,if=buff.enrage.up&(!buff.recklessness.remains|rage<50)&(spell_targets.whirlwind=1&raid_event.adds.in>45|spell_targets.whirlwind=2)
            if (CanCastBladestorm)
            {
                if (BuffEnrageUp && (!BuffRecklessnessUp || Rage < 50) && (EnemiesInMelee == 1 || EnemiesInMelee == 2))
                    return Bladestorm();
            }

            //actions.single_target+=/bloodthirst,if=buff.enrage.down|conduit.vicious_contempt.rank>5&target.health.pct<35
            if (CanCastBloodthirst)
            {
                if (!BuffEnrageUp || ConduitViciousContempt && TargetHealthPct < 35)
                    return Bloodthirst();
            }

            //actions.single_target+=/bloodbath,if=buff.enrage.down|conduit.vicious_contempt.rank>5&target.health.pct<35&!talent.cruelty.enabled
            if (CanCastBloodbath)
            {
                if (!BuffEnrageUp || ConduitViciousContempt && TargetHealthPct < 35 && !TalentCruelty)
                    return Bloodbath();
            }

            //actions.single_target+=/dragon_roar,if=buff.enrage.up&(spell_targets.whirlwind>1|raid_event.adds.in>15)
            if (CanCastDragonRoar)
            {
                if (BuffEnrageUp)
                    return DragonRoar();
            }

            //actions.single_target+=/onslaught
            if (CanCastOnslaught)
                return Onslaught();

            //actions.single_target+=/whirlwind,if=buff.merciless_bonegrinder.up&spell_targets.whirlwind>3
            if (CanCastWhirlwind)
            {
                if (BuffMercilessBonegrinderUp && EnemiesInMelee > 3)
                    return Whirlwind();
            }

            //actions.single_target+=/raging_blow,if=charges=2|buff.recklessness.up&variable.execute_phase&talent.massacre.enabled
            if (CanCastRagingBlow)
            {
                if (CDRagingBlowFullRecharge < GCD || BuffRecklessnessUp && execute_phase && TalentMassacre)
                    return RagingBlow();
            }

            //actions.single_target+=/crushing_blow,if=charges=2|buff.recklessness.up&variable.execute_phase&talent.massacre.enabled
            if (CanCastCrushingBlow)
            {
                if (CDCrushingBlowFullRecharge < GCD || BuffRecklessnessUp && execute_phase && TalentMassacre)
                    return CrushingBlow();
            }

            /*  actions.single_target+=/bloodthirst
                actions.single_target+=/bloodbath
                actions.single_target+=/raging_blow
                actions.single_target+=/crushing_blow
                actions.single_target+=/whirlwind*/

            if (CanCastBloodthirst)
                return Bloodthirst();

            if (CanCastBloodbath)
                return Bloodbath();

            if (CanCastRagingBlow)
                return RagingBlow();

            if (CanCastCrushingBlow)
                return CrushingBlow();

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
        bool Bladestorm() { Aimsharp.Cast("Bladestorm"); return true; }
        bool Condemn() { Aimsharp.Cast("Condemn"); return true; }
        bool Siegebreaker() { Aimsharp.Cast("Siegebreaker"); return true; }
        bool CrushingBlow() { Aimsharp.Cast("Crushing Blow"); return true; }
        bool Execute() { Aimsharp.Cast("Execute"); return true; }
        bool Bloodthirst() { Aimsharp.Cast("Bloodthirst"); return true; }
        bool Bloodbath() { Aimsharp.Cast("Bloodbath"); return true; }
        bool DragonRoar() { Aimsharp.Cast("Dragon Roar"); return true; }
        bool Onslaught() { Aimsharp.Cast("Onslaught"); return true; }
        bool RagingBlow() { Aimsharp.Cast("Raging Blow"); return true; }


    }
}
