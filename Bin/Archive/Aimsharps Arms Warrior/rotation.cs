using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsArms : Rotation
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
           "Sweeping Strikes","Bladestorm","Mortal Strike","Deadly Calm","Rend","Skullsplitter","Colossus Smash","Overpower","Execute","Ravager","Avatar","Cleave","Warbreaker","Whirlwind","Slam",
        };

        List<string> BuffsList = new List<string>
        {
            "Deadly Calm","Sweeping Strikes","Sudden Death"
        };

        List<string> DebuffsList = new List<string>
        {
           "Colossus Smash","Rend","Deep Wounds",
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
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Arms", Color.Purple);
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

            Macros.Add("RavagerC", "/cast [@cursor] Ravager");


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
            int BuffMemoryOfLucidDreamsRemaining = Aimsharp.BuffRemaining("Memory of Lucid Dreams") - GCD;
            bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemaining > 0;

            // General Debuffs


            // General Cooldowns
            int ConcentratedFlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            int CDGuardianOfAzerothRemaining = Aimsharp.SpellCooldown("Guardian of Azeroth") - GCD;
            bool CDGuardianOfAzerothUp = CDGuardianOfAzerothRemaining <= 0;

            //power
            int Rage = Aimsharp.Power("player");
            int MaxRage = Aimsharp.PlayerMaxPower();
            int RageDefecit = MaxRage - Rage;

            //Talents
            bool TalentMassacre = Aimsharp.Talent(3, 1);
            bool TalentDeadlyCalm = Aimsharp.Talent(6, 3);
            bool TalentCleave = Aimsharp.Talent(5, 3);
            bool TalentDreadnaught = Aimsharp.Talent(7, 2);
            bool TalentFervorofBattle = Aimsharp.Talent(3, 2);
            bool TalentWarbreaker = Aimsharp.Talent(5, 2);
            bool TalentRavager = Aimsharp.Talent(7, 3);


            //buffs
            int BuffDeadlyCalmRemains = Aimsharp.BuffRemaining("Deadly Calm") - GCD;
            bool BuffDeadlyCalmUp = BuffDeadlyCalmRemains > 0;
            int BuffSweepingStrikesRemains = Aimsharp.BuffRemaining("Sweeping Strikes") - GCD;
            bool BuffSweepingStrikesUp = BuffSweepingStrikesRemains > 0;
            int BuffOverpowerStacks = Aimsharp.BuffStacks("Overpower");
            int BuffSuddenDeathRemains = Aimsharp.BuffRemaining("Sudden Death") - GCD;
            bool BuffSuddenDeathUp = BuffSuddenDeathRemains > 0;


            //debuffs
            int DebuffColossusSmashRemains = Aimsharp.DebuffRemaining("Colossus Smash") - GCD;
            bool DebuffColossusSmashUp = DebuffColossusSmashRemains > 0;
            int DotRendRemains = Aimsharp.DebuffRemaining("Rend") - GCD;
            bool DotRendUp = DotRendRemains > 0;
            bool DotRendRefreshable = DotRendRemains < 4500;
            int DotDeepWoundsRemains = Aimsharp.DebuffRemaining("Deep Wounds") - GCD;
            bool DotDeepWoundsUp = DotDeepWoundsRemains > 0;


            //cooldowns
            int CDBladestormRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Bladestorm");
            bool CDBladestormUp = CDBladestormRemains <= 0;
            int CDMortalStrikeRemains = Aimsharp.SpellCooldown("Mortal Strike");
            bool CDMortalStrikeUp = CDMortalStrikeRemains <= 0;
            int CDColossusSmashRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Colossus Smash");
            bool CDColossusSmashUp = CDColossusSmashRemains <= 0;
            int CDOverpowerCharges = Aimsharp.SpellCharges("Overpower");
            int CDOverpowerFullRecharge = Aimsharp.RechargeTime("Overpower") + 12000 * (Aimsharp.MaxCharges("Overpower") - CDOverpowerCharges - 1);
            float CDOverpowerFractional = CDOverpowerCharges + (1 - (Aimsharp.RechargeTime("Overpower") - GCD) / 12000f);
            CDOverpowerFractional = CDOverpowerFractional > Aimsharp.MaxCharges("Overpower") ? Aimsharp.MaxCharges("Overpower") : CDOverpowerFractional;


            //specific variables


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastSweepingStrikes = Aimsharp.CanCast("Sweeping Strikes", "player") && Fighting;
            bool CanCastBladestorm = Aimsharp.CanCast("Bladestorm", "player") && Fighting && !SaveCooldowns && !TalentRavager;
            bool CanCastMortalStrike = Aimsharp.CanCast("Mortal Strike") && Fighting;
            bool CanCastDeadlyCalm = Aimsharp.CanCast("Deadly Calm", "player") && Fighting && !SaveCooldowns;
            bool CanCastRend = Aimsharp.CanCast("Rend") && Fighting;
            bool CanCastSkullsplitter = Aimsharp.CanCast("Skullsplitter") && Fighting;
            bool CanCastColossusSmash = Aimsharp.CanCast("Colossus Smash") && Fighting && !SaveCooldowns && !TalentWarbreaker;
            bool CanCastOverpower = Aimsharp.CanCast("Overpower") && Fighting;
            bool CanCastExecute = Aimsharp.CanCast("Execute") && Fighting;
            bool CanCastRavager = Aimsharp.CanCast("Ravager", "player") && Fighting;
            bool CanCastAvatar = Aimsharp.CanCast("Avatar", "player") && Fighting && !SaveCooldowns;
            bool CanCastCleave = Aimsharp.CanCast("Cleave", "player") && Fighting;
            bool CanCastWarbreaker = Aimsharp.CanCast("Warbreaker", "player") && Fighting;
            bool CanCastWhirlwind = Aimsharp.CanCast("Whirlwind", "player") && Fighting;
            bool CanCastSlam = Aimsharp.CanCast("Slam") && Fighting;





            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            if (DebuffColossusSmashUp)
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

                if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }

                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }
            }

            //actions+=/sweeping_strikes,if=spell_targets.whirlwind>1&cooldown.bladestorm.remains>12
            if (CanCastSweepingStrikes)
            {
                if (EnemiesInMelee > 1 && CDBladestormRemains > 12000)
                {
                    Aimsharp.Cast("Sweeping Strikes");
                    return true;
                }
            }

            //actions+=/run_action_list,name=hac,if=raid_event.adds.exists
            if (EnemiesInMelee > 1)
            {
                //actions.hac=skullsplitter,if=rage<60&buff.deadly_calm.down
                if (CanCastSkullsplitter)
                {
                    if (Rage < 60 && !BuffDeadlyCalmUp)
                    {
                        Aimsharp.Cast("Skullsplitter");
                        return true;
                    }
                }

                //actions.hac+=/avatar,if=cooldown.colossus_smash.remains<1
                if (CanCastAvatar)
                {
                    if (CDColossusSmashRemains < 1000)
                    {
                        Aimsharp.Cast("Avatar");
                        return true;
                    }
                }

                //actions.hac+=/cleave,if=dot.deep_wounds.remains<=gcd
                if (CanCastCleave)
                {
                    if (DotDeepWoundsRemains <= GCDMAX + GCD)
                    {
                        Aimsharp.Cast("Cleave");
                        return true;
                    }
                }

                //actions.hac+=/warbreaker
                if (CanCastWarbreaker)
                {
                    Aimsharp.Cast("Warbreaker");
                    return true;
                }

                //actions.hac+=/bladestorm
                if (CanCastBladestorm)
                {
                    Aimsharp.Cast("Bladestorm");
                    return true;
                }

                //actions.hac+=/ravager
                if (CanCastRavager)
                {
                    Aimsharp.Cast("RavagerC");
                    return true;
                }

                //actions.hac+=/colossus_smash
                if (CanCastColossusSmash)
                {
                    Aimsharp.Cast("Colossus Smash");
                    return true;
                }

                //actions.hac+=/rend,if=remains<=duration*0.3&buff.sweeping_strikes.up
                if (CanCastRend)
                {
                    if (DotRendRefreshable && BuffSweepingStrikesUp)
                    {
                        Aimsharp.Cast("Rend");
                        return true;
                    }
                }

                //actions.hac+=/cleave
                if (CanCastCleave)
                {
                    Aimsharp.Cast("Cleave");
                    return true;
                }

                //actions.hac+=/mortal_strike,if=buff.sweeping_strikes.up|dot.deep_wounds.remains<gcd&!talent.cleave.enabled
                if (CanCastMortalStrike)
                {
                    if (BuffSweepingStrikesUp || DotDeepWoundsRemains < GCDMAX && !TalentCleave)
                    {
                        Aimsharp.Cast("Mortal Strike");
                        return true;
                    }
                }

                //actions.hac+=/overpower,if=talent.dreadnaught.enabled
                if (CanCastOverpower)
                {
                    if (TalentDreadnaught)
                    {
                        Aimsharp.Cast("Overpower");
                        return true;
                    }
                }

                //actions.hac+=/execute,if=buff.sweeping_strikes.up
                if (CanCastExecute)
                {
                    if (BuffSweepingStrikesUp)
                    {
                        Aimsharp.Cast("Execute");
                        return true;
                    }
                }

                //actions.hac+=/overpower
                if (CanCastOverpower)
                {
                    Aimsharp.Cast("Overpower");
                    return true;
                }

                //actions.hac+=/whirlwind
                if (CanCastWhirlwind)
                {
                    Aimsharp.Cast("Whirlwind");
                    return true;
                }

                return false;
            }

            //actions+=/run_action_list,name=execute,if=(talent.massacre.enabled&target.health.pct<35)|target.health.pct<20|(target.health.pct>80&covenant.venthyr)
            if ((TalentMassacre && TargetHealthPct < 35) || TargetHealthPct < 20 || (TargetHealthPct > 80 && CovenantVenthyr))
            {
                //actions.execute=deadly_calm
                if (CanCastDeadlyCalm)
                {
                    Aimsharp.Cast("Deadly Calm");
                    return true;
                }

                //actions.execute+=/rend,if=remains<=duration*0.3
                if (CanCastRend)
                {
                    if (DotRendRefreshable)
                    {
                        Aimsharp.Cast("Rend");
                        return true;
                    }
                }

                //actions.execute+=/skullsplitter,if=rage<60&(!talent.deadly_calm.enabled|buff.deadly_calm.down)
                if (CanCastSkullsplitter)
                {
                    if (Rage < 60 && !BuffDeadlyCalmUp)
                    {
                        Aimsharp.Cast("Skullsplitter");
                        return true;
                    }
                }

                //actions.execute+=/avatar,if=cooldown.colossus_smash.remains<1
                if (CanCastAvatar)
                {
                    if (CDColossusSmashRemains < 1000)
                    {
                        Aimsharp.Cast("Avatar");
                        return true;
                    }
                }

                //actions.execute+=/cleave,if=spell_targets.whirlwind>1&dot.deep_wounds.remains<gcd
                if (CanCastCleave)
                {
                    if (EnemiesInMelee > 1 && DotDeepWoundsRemains < GCDMAX + GCD)
                    {
                        Aimsharp.Cast("Cleave");
                        return true;
                    }
                }

                //actions.execute+=/warbreaker
                if (CanCastWarbreaker)
                {
                    Aimsharp.Cast("Warbreaker");
                    return true;
                }

                //actions.execute+=/colossus_smash
                if (CanCastColossusSmash)
                {
                    Aimsharp.Cast("Colossus Smash");
                    return true;
                }

                //actions.execute+=/overpower,if=charges=2
                if (CanCastOverpower)
                {
                    if (CDOverpowerCharges == 2)
                    {
                        Aimsharp.Cast("Overpower");
                        return true;
                    }
                }

                //actions.execute+=/mortal_strike,if=dot.deep_wounds.remains<gcd
                if (CanCastMortalStrike)
                {
                    if (DotDeepWoundsRemains < GCDMAX + GCD)
                    {
                        Aimsharp.Cast("Mortal Strike");
                        return true;

                    }
                }

                //actions.execute+=/skullsplitter,if=rage<40
                if (CanCastSkullsplitter)
                {
                    if (Rage < 40)
                    {
                        Aimsharp.Cast("Skullsplitter");
                        return true;
                    }
                }

                //actions.execute+=/overpower
                if (CanCastOverpower)
                {
                    Aimsharp.Cast("Overpower");
                    return true;
                }

                //actions.execute+=/execute
                if (CanCastExecute)
                {
                    Aimsharp.Cast("Execute");
                    return true;
                }

                //actions.execute+=/bladestorm,if=rage<80
                if (CanCastBladestorm)
                {
                    if (Rage < 80)
                    {
                        Aimsharp.Cast("Bladestorm");
                        return true;
                    }
                }

                //actions.execute+=/ravager,if=rage<80
                if (CanCastRavager)
                {
                    if (Rage < 80)
                    {
                        Aimsharp.Cast("RavagerC");
                        return true;
                    }
                }

                return false;
            }

            //actions+=/run_action_list,name=single_target
            //actions.single_target=avatar,if=cooldown.colossus_smash.remains<1
            if (CanCastAvatar)
            {
                if (CDColossusSmashRemains < 1000)
                {
                    Aimsharp.Cast("Avatar");
                    return true;
                }
            }

            //actions.single_target+=/rend,if=remains<=duration*0.3
            if (CanCastRend)
            {
                if (DotRendRefreshable)
                {
                    Aimsharp.Cast("Rend");
                    return true;
                }
            }

            //actions.single_target+=/cleave,if=spell_targets.whirlwind>1&dot.deep_wounds.remains<gcd
            if (CanCastCleave)
            {
                if (EnemiesInMelee > 1 && DotDeepWoundsRemains <= GCDMAX + GCD)
                {
                    Aimsharp.Cast("Cleave");
                    return true;
                }
            }

            //actions.single_target+=/warbreaker
            if (CanCastWarbreaker)
            {
                Aimsharp.Cast("Warbreaker");
                return true;
            }

            //actions.execute+=/colossus_smash
            if (CanCastColossusSmash)
            {
                Aimsharp.Cast("Colossus Smash");
                return true;
            }

            //actions.single_target+=/bladestorm,if=debuff.colossus_smash.up&!covenant.venthyr
            if (CanCastBladestorm)
            {
                if (DebuffColossusSmashUp && !CovenantVenthyr)
                {
                    Aimsharp.Cast("Bladestorm");
                    return true;
                }
            }

            //actions.single_target+=/ravager,if=debuff.colossus_smash.up&!covenant.venthyr
            if (CanCastRavager)
            {
                if (DebuffColossusSmashUp && !CovenantVenthyr)
                {
                    Aimsharp.Cast("RavagerC");
                    return true;
                }
            }

            //actions.single_target+=/overpower,if=charges=2
            if (CanCastOverpower)
            {
                if (CDOverpowerCharges == 2)
                {
                    Aimsharp.Cast("Overpower");
                    return true;
                }
            }

            //actions.single_target+=/mortal_strike,if=buff.overpower.stack>=2&buff.deadly_calm.down|dot.deep_wounds.remains<=gcd
            if (CanCastMortalStrike)
            {
                if (BuffOverpowerStacks >= 2 && !BuffDeadlyCalmUp || DotDeepWoundsRemains <= GCDMAX + GCD)
                {
                    Aimsharp.Cast("Mortal Strike");
                    return true;

                }
            }

            //actions.single_target+=/deadly_calm
            if (CanCastDeadlyCalm)
            {
                Aimsharp.Cast("Deadly Calm");
                return true;
            }

            //actions.single_target+=/skullsplitter,if=rage<60&buff.deadly_calm.down
            if (CanCastSkullsplitter)
            {
                if (Rage < 60 && !BuffDeadlyCalmUp)
                {
                    Aimsharp.Cast("Skullsplitter");
                    return true;
                }
            }

            //actions.single_target+=/overpower
            if (CanCastOverpower)
            {
                Aimsharp.Cast("Overpower");
                return true;
            }

            //actions.single_target+=/execute,if=buff.sudden_death.react
            if (CanCastExecute)
            {
                Aimsharp.Cast("Execute");
                return true;
            }

            //actions.single_target+=/mortal_strike
            if (CanCastMortalStrike)
            {
                Aimsharp.Cast("Mortal Strike");
                return true;
            }

            //actions.single_target+=/bladestorm,if=debuff.colossus_smash.up&covenant.venthyr
            if (CanCastBladestorm)
            {
                if (DebuffColossusSmashUp && CovenantVenthyr)
                {
                    Aimsharp.Cast("Bladestorm");
                    return true;
                }
            }

            //actions.single_target+=/whirlwind,if=talent.fervor_of_battle.enabled&rage>60
            if (CanCastWhirlwind)
            {
                if (TalentFervorofBattle && Rage > 60)
                {
                    Aimsharp.Cast("Whirlwind");
                    return true;
                }
            }

            //actions.single_target+=/slam,if=rage>50
            if (CanCastSlam)
            {
                if (Rage>50)
                {
                    Aimsharp.Cast("Slam");
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
