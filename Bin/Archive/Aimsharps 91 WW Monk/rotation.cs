using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsWindwalker : Rotation
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
           "Chi Burst","Chi Wave","Invoke Xuen, the White Tiger","Serenity","Fist of the White Tiger","Fists of Fury","Weapons of Order","Expel Harm","Tiger Palm","Whirling Dragon Punch","Energizing Elixir","Spinning Crane Kick","Rising Sun Kick","Rushing Jade Wind","Bonedust Brew","Crackling Jade Lightning","Blackout Kick","Touch of Death","Faeline Stomp","Fallen Order","Storm, Earth, and Fire","Touch of Karma",
        };

        List<string> BuffsList = new List<string>
        {
            "Serenity","Storm, Earth, and Fire","Weapons of Order","Dance of Chi-Ji","Rushing Jade Wind","The Emperor's Capacitor","Blackout Kick!","Energizing Elixir","Chi Energy"
        };

        List<string> DebuffsList = new List<string>
        {
           "Bonedust Brew",
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

            Settings.Add(new Setting("Monk Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Windwalker", Color.Purple);
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

            Macros.Add("BonedustBrewC", "/cast [@cursor] Bonedust Brew");


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
            int BuffMemoryOfLucidDreamsRemaining = Aimsharp.BuffRemaining("Memory of Lucid Dreams") - GCD;
            bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemaining > 0;

            // General Debuffs


            // General Cooldowns
            int ConcentratedFlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            int CDGuardianOfAzerothRemaining = Aimsharp.SpellCooldown("Guardian of Azeroth") - GCD;
            bool CDGuardianOfAzerothUp = CDGuardianOfAzerothRemaining <= 0;

            //power
            int Energy = Aimsharp.Power("player");
            int Chi = Aimsharp.PlayerSecondaryPower();
            int MaxEnergy = Aimsharp.PlayerMaxPower();
            int EnergyDefecit = MaxEnergy - Energy;

            //Talents
            bool TalentEnergizingElixir = Aimsharp.Talent(3, 3);
            bool TalentHitCombo = Aimsharp.Talent(6, 1);
            bool TalentWhirlingDragonPunch = Aimsharp.Talent(7, 2);
            bool TalentSerenity = Aimsharp.Talent(7, 3);
            bool TalentAscension = Aimsharp.Talent(3, 1);


            //buffs
            int BuffSerenityRemains = Aimsharp.BuffRemaining("Serenity") - GCD;
            bool BuffSerenityUp = BuffSerenityRemains > 0;
            int BuffStormEarthandFireRemains = Aimsharp.BuffRemaining("Storm, Earth, and Fire") - GCD;
            bool BuffStormEarthandFireUp = BuffStormEarthandFireRemains > 0;
            int BuffWeaponsofOrderRemains = Aimsharp.BuffRemaining("Weapons of Order") - GCD;
            bool BuffWeaponsofOrderUp = BuffWeaponsofOrderRemains > 0;
            int BuffDanceofChiJiRemains = Aimsharp.BuffRemaining("Dance of Chi-Ji") - GCD;
            bool BuffDanceofChiJiUp = BuffDanceofChiJiRemains > 0;
            int BuffRushingJadeWindRemains = Aimsharp.BuffRemaining("Rushing Jade Wind") - GCD;
            bool BuffRushingJadeWindUp = BuffRushingJadeWindRemains > 0;
            int BuffTheEmperorsCapacitorRemains = Aimsharp.BuffRemaining("The Emperor's Capacitor") - GCD;
            bool BuffTheEmperorsCapacitorUp = BuffTheEmperorsCapacitorRemains > 0;
            int BuffTheEmperorsCapacitorStacks = Aimsharp.BuffStacks("The Emperor's Capacitor");
            int BuffBlackoutKickRemains = Aimsharp.BuffRemaining("Blackout Kick!") - GCD;
            bool BuffBlackoutKickUp = BuffBlackoutKickRemains > 0;
            int BuffEnergizingElixirRemains = Aimsharp.BuffRemaining("Energizing Elixir") - GCD;
            bool BuffEnergizingElixirUp = BuffEnergizingElixirRemains > 0;
            int BuffChiEnergyStacks = Aimsharp.BuffStacks("Chi Energy");


            //debuffs
            int DebuffBonedustBrewRemains = Aimsharp.DebuffRemaining("Bonedust Brew") - GCD;
            bool DebuffBonedustBrewUp = DebuffBonedustBrewRemains > 0;


            //cooldowns
            int CDInvokeXuentheWhiteTigerRemains = Aimsharp.SpellCooldown("Invoke Xuen, the White Tiger");
            bool CDInvokeXuentheWhiteTigerUp = CDInvokeXuentheWhiteTigerRemains <= 0;
            int CDSerenityRemains = Aimsharp.SpellCooldown("Serenity");
            bool CDSerenityUp = CDSerenityRemains <= 0;
            int CDFistsofFuryRemains = Aimsharp.SpellCooldown("Fists of Fury");
            bool CDFistsofFuryUp = CDFistsofFuryRemains <= 0;
            int CDWeaponsofOrderRemains = Aimsharp.SpellCooldown("Weapons of Order");
            bool CDWeaponsofOrderUp = CDWeaponsofOrderRemains <= 0;
            int CDWhirlingDragonPunchRemains = Aimsharp.SpellCooldown("Whirling Dragon Punch");
            bool CDWhirlingDragonPunchUp = CDWhirlingDragonPunchRemains <= 0;
            int CDRisingSunKickRemains = Aimsharp.SpellCooldown("Rising Sun Kick");
            bool CDRisingSunKickUp = CDRisingSunKickRemains <= 0;
            int CDBonedustBrewRemains = Aimsharp.SpellCooldown("Bonedust Brew");
            bool CDBonedustBrewUp = CDBonedustBrewRemains <= 0;
            int CDStormEarthandFireRemains = Aimsharp.SpellCooldown("Storm, Earth, and Fire");
            bool CDStormEarthandFireUp = CDStormEarthandFireRemains <= 0;
            int StormEarthandFireCharges = Aimsharp.SpellCharges("Storm, Earth, and Fire");
            int StormEarthandFireFullRecharge = Aimsharp.RechargeTime("Storm, Earth, and Fire") + 90000 * (Aimsharp.MaxCharges("Storm, Earth, and Fire") - StormEarthandFireCharges - 1);
            float StormEarthandFireFractional = StormEarthandFireCharges + (1 - (Aimsharp.RechargeTime("Storm, Earth, and Fire") - GCD) / 90000f);
            StormEarthandFireFractional = StormEarthandFireFractional > Aimsharp.MaxCharges("Storm, Earth, and Fire") ? Aimsharp.MaxCharges("Storm, Earth, and Fire") : StormEarthandFireFractional;


            //specific variables
            bool ConduitCalculatedStrikes = ActiveConduits.Contains(336526);
            int MaxChi = TalentAscension ? 6 : 5;
            float EnergyRegen = 10f * (1f + Haste) * (TalentAscension ? 1.1f : 1f) + (BuffEnergizingElixirRemains > 1000 ? 15f : BuffEnergizingElixirRemains * 15f / 1000f);
            int TimeUntilMaxEnergy = (int)((EnergyDefecit * 1000f) / EnergyRegen);
            float fof_execute_time = 4000f / (1f + Haste);

            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastChiBurst = Aimsharp.CanCast("Chi Burst", "player") && Fighting && !Moving;
            bool CanCastChiWave = Aimsharp.CanCast("Chi Wave") && Fighting;
            bool CanCastInvokeXuentheWhiteTiger = Aimsharp.CanCast("Invoke Xuen, the White Tiger") && Fighting;
            bool CanCastSerenity = Aimsharp.CanCast("Serenity", "player") && Fighting;
            bool CanCastFistoftheWhiteTiger = Aimsharp.CanCast("Fist of the White Tiger") && Fighting;
            bool CanCastFistsofFury = Aimsharp.CanCast("Fists of Fury", "player") && Fighting;
            bool CanCastWeaponsofOrder = Aimsharp.CanCast("Weapons of Order", "player") && Fighting;
            bool CanCastExpelHarm = Aimsharp.CanCast("Expel Harm", "player") && Fighting;
            bool CanCastTigerPalm = Aimsharp.CanCast("Tiger Palm") && Fighting;
            bool CanCastWhirlingDragonPunch = Aimsharp.CanCast("Whirling Dragon Punch", "player") && Fighting;
            bool CanCastEnergizingElixir = Aimsharp.CanCast("Energizing Elixir", "player") && Fighting;
            bool CanCastSpinningCraneKick = Aimsharp.CanCast("Spinning Crane Kick", "player") && Fighting;
            bool CanCastRisingSunKick = Aimsharp.CanCast("Rising Sun Kick") && Fighting;
            bool CanCastRushingJadeWind = Aimsharp.CanCast("Rushing Jade Wind") && Fighting;
            bool CanCastBonedustBrew = Aimsharp.CanCast("Bonedust Brew", "player") && Fighting;
            bool CanCastCracklingJadeLightning = Aimsharp.CanCast("Crackling Jade Lightning") && Fighting && !Moving;
            bool CanCastBlackoutKick = Aimsharp.CanCast("Blackout Kick") && Fighting;
            bool CanCastTouchofDeath = Aimsharp.CanCast("Touch of Death") && Fighting;
            bool CanCastFaelineStomp = Aimsharp.CanCast("Faeline Stomp", "player") && Fighting;
            bool CanCastFallenOrder = Aimsharp.CanCast("Fallen Order", "player") && Fighting;
            bool CanCastStormEarthandFire = Aimsharp.CanCast("Storm, Earth, and Fire", "player") && Fighting;
            bool CanCastTouchofKarma = Aimsharp.CanCast("Touch of Karma") && Fighting;

            //actions+=/variable,name=hold_xuen,op=set,value=cooldown.invoke_xuen_the_white_tiger.remains>fight_remains|fight_remains<120&fight_remains>cooldown.serenity.remains&cooldown.serenity.remains>10
            bool hold_xuen = CDInvokeXuentheWhiteTigerRemains > TargetTimeToDie || TargetTimeToDie < 120000 && TargetTimeToDie > CDSerenityRemains && CDSerenityRemains > 10000;
            bool PetXuenActive = Aimsharp.PlayerHasPet();


            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions+=/potion,if=(buff.serenity.up|buff.storm_earth_and_fire.up)&pet.xuen_the_white_tiger.active|fight_remains<=60
            if (UsePotion && Aimsharp.CanUseItem(PotionName, false))
            {
                if ((BuffSerenityUp || BuffStormEarthandFireUp) && PetXuenActive || TargetTimeToDie <= 60000)
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //actions+=/call_action_list,name=serenity,if=buff.serenity.up
            if (BuffSerenityUp)
            {
                //actions.serenity=fists_of_fury,if=buff.serenity.remains<1
                if (CanCastFistsofFury)
                {
                    if (BuffSerenityRemains < 1000)
                    {
                        Aimsharp.Cast("Fists of Fury");
                        return true;
                    }
                }

                //actions.serenity+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&(active_enemies>=3|active_enemies>1&!cooldown.rising_sun_kick.up)
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && (EnemiesInMelee >= 3 || EnemiesInMelee > 1 && !CDRisingSunKickUp))
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.serenity+=/rising_sun_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike
                if (CanCastRisingSunKick)
                {
                    if (LastCast != "Rising Sun Kick")
                    {
                        Aimsharp.Cast("Rising Sun Kick");
                        return true;
                    }
                }

                //actions.serenity+=/fists_of_fury,if=active_enemies>=3
                if (CanCastFistsofFury)
                {
                    if (EnemiesInMelee >= 3)
                    {
                        Aimsharp.Cast("Fists of Fury");
                        return true;
                    }
                }

                //actions.serenity+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&buff.dance_of_chiji.up
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && BuffDanceofChiJiUp)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.serenity+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=(combo_strike|!talent.hit_combo.enabled)&buff.weapons_of_order_ww.up&cooldown.rising_sun_kick.remains>2
                if (CanCastBlackoutKick)
                {
                    if ((LastCast != "Blackout Kick" || !TalentHitCombo) && BuffWeaponsofOrderUp && CDRisingSunKickRemains > 2000)
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }

                //actions.serenity+=/fist_of_the_white_tiger,interrupt=1
                if (CanCastFistoftheWhiteTiger)
                {
                    Aimsharp.Cast("Fist of the White Tiger");
                    return true;
                }

                //actions.serenity+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&debuff.bonedust_brew.up
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && DebuffBonedustBrewUp)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.serenity+=/fist_of_the_white_tiger,target_if=min:debuff.mark_of_the_crane.remains,if=chi<3
                if (CanCastFistoftheWhiteTiger)
                {
                    if (Chi < 3)
                    {
                        Aimsharp.Cast("Fist of the White Tiger");
                        return true;
                    }
                }

                //actions.serenity+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike|!talent.hit_combo.enabled
                if (CanCastBlackoutKick)
                {
                    if (LastCast != "Blackout Kick" || !TalentHitCombo)
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }

                //actions.serenity+=/spinning_crane_kick
                if (CanCastSpinningCraneKick)
                {
                    Aimsharp.Cast("Spinning Crane Kick");
                    return true;
                }

            }

            //actions+=/call_action_list,name=weapons_of_order,if=buff.weapons_of_order.up
            if (BuffWeaponsofOrderUp)
            {
                if(!SaveCooldowns)
                //actions.weapons_of_order=call_action_list,name=cd_sef,if=!talent.serenity.enabled
                if (!TalentSerenity)
                {
                    //actions.cd_sef=invoke_xuen_the_white_tiger,if=!variable.hold_xuen|fight_remains<25
                    if (CanCastInvokeXuentheWhiteTiger)
                    {
                        if (!hold_xuen || TargetTimeToDie < 25000)
                        {
                            Aimsharp.Cast("Invoke Xuen, the White Tiger");
                            return true;
                        }
                    }

                    //actions.cd_sef+=/touch_of_death,if=buff.storm_earth_and_fire.down&pet.xuen_the_white_tiger.active|fight_remains<10|fight_remains>180
                    if (CanCastTouchofDeath)
                    {
                        if (!BuffStormEarthandFireUp && PetXuenActive || TargetTimeToDie < 10000 || TargetTimeToDie > 180000)
                        {
                            Aimsharp.Cast("Touch of Death");
                            return true;
                        }
                    }

                    //actions.cd_sef+=/weapons_of_order,if=(raid_event.adds.in>45|raid_event.adds.up)&cooldown.rising_sun_kick.remains<execute_time
                    if (CanCastWeaponsofOrder)
                    {
                        if (CDRisingSunKickRemains < GCDMAX)
                        {
                            Aimsharp.Cast("Weapons of Order");
                            return true;
                        }
                    }

                    //actions.cd_sef+=/faeline_stomp,if=combo_strike&(raid_event.adds.in>10|raid_event.adds.up)
                    if (CanCastFaelineStomp)
                    {
                        if (LastCast != "Faeline Stomp")
                        {
                            Aimsharp.Cast("Faeline Stomp");
                            return true;
                        }
                    }

                    //actions.cd_serenity+=/fallen_order
                    if (CanCastFallenOrder)
                    {
                        Aimsharp.Cast("Fallen Order");
                        return true;
                    }

                    //actions.cd_serenity+=/bonedust_brew
                    if (CanCastBonedustBrew)
                    {
                        Aimsharp.Cast("BonedustBrewC");
                        return true;
                    }

                    //actions.cd_sef+=/storm_earth_and_fire,if=cooldown.storm_earth_and_fire.charges=2|fight_remains<20|(raid_event.adds.remains>15|!covenant.kyrian&((raid_event.adds.in>cooldown.storm_earth_and_fire.full_recharge_time|!raid_event.adds.exists)&(cooldown.invoke_xuen_the_white_tiger.remains>cooldown.storm_earth_and_fire.full_recharge_time|variable.hold_xuen))&cooldown.fists_of_fury.remains<=9&chi>=2&cooldown.whirling_dragon_punch.remains<=12)
                    if (CanCastStormEarthandFire)
                    {
                        if (StormEarthandFireCharges == 2 || TargetTimeToDie < 20000 || (!CovenantKyrian && ((true) && (CDInvokeXuentheWhiteTigerRemains > StormEarthandFireFullRecharge || hold_xuen)) && CDFistsofFuryRemains <= 9000 && Chi >= 2 && CDWhirlingDragonPunchRemains <= 12000))
                        {
                            Aimsharp.Cast("Storm, Earth, and Fire");
                            return true;
                        }
                    }

                    //actions.cd_sef+=/storm_earth_and_fire,if=covenant.kyrian&(buff.weapons_of_order.up|(fight_remains<cooldown.weapons_of_order.remains|cooldown.weapons_of_order.remains>cooldown.storm_earth_and_fire.full_recharge_time)&cooldown.fists_of_fury.remains<=9&chi>=2&cooldown.whirling_dragon_punch.remains<=12)
                    if (CanCastStormEarthandFire)
                    {
                        if (CovenantKyrian && (BuffWeaponsofOrderUp || (TargetTimeToDie < CDWeaponsofOrderRemains || CDWeaponsofOrderRemains > StormEarthandFireFullRecharge) && CDFistsofFuryRemains <= 9000 && Chi >= 2 && CDWhirlingDragonPunchRemains <= 12000))
                        {
                            Aimsharp.Cast("Storm, Earth, and Fire");
                            return true;
                        }
                    }



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

                    foreach (string Racial in Racials)
                    {
                        if (Aimsharp.CanCast(Racial, "player") && Fighting && (CDInvokeXuentheWhiteTigerRemains > 30000 || hold_xuen))
                        {
                            Aimsharp.Cast(Racial, true);
                            return true;
                        }
                    }
                }

                if(!SaveCooldowns)
                //actions.weapons_of_order+=/call_action_list,name=cd_serenity,if=talent.serenity.enabled
                if (TalentSerenity)
                {
                    //actions.cd_serenity=variable,name=serenity_burst,op=set,value=cooldown.serenity.remains<1|pet.xuen_the_white_tiger.active&cooldown.serenity.remains>30|fight_remains<20
                    bool serenity_burst = CDSerenityRemains < 1000 || PetXuenActive && CDSerenityRemains > 30000 || TargetTimeToDie < 20000;

                    //actions.cd_serenity+=/invoke_xuen_the_white_tiger,if=!variable.hold_xuen|fight_remains<25
                    if (CanCastInvokeXuentheWhiteTiger)
                    {
                        if (!hold_xuen || TargetTimeToDie < 25000)
                        {
                            Aimsharp.Cast("Invoke Xuen, the White Tiger");
                            return true;
                        }
                    }

                    //actions.cd_serenity+=/blood_fury,if=variable.serenity_burst
                    foreach (string Racial in Racials)
                    {
                        if (Aimsharp.CanCast(Racial, "player") && Fighting && serenity_burst)
                        {
                            Aimsharp.Cast(Racial, true);
                            return true;
                        }
                    }

                    //actions.cd_serenity+=/touch_of_death,if=fight_remains>180|pet.xuen_the_white_tiger.active|fight_remains<10
                    if (CanCastTouchofDeath)
                    {
                        if (TargetTimeToDie > 180000 || PetXuenActive || TargetTimeToDie < 10000)
                        {
                            Aimsharp.Cast("Touch of Death");
                            return true;
                        }
                    }

        

                    //actions.cd_serenity+=/weapons_of_order,if=cooldown.rising_sun_kick.remains<execute_time
                    if (CanCastWeaponsofOrder)
                    {
                        if (CDRisingSunKickRemains < GCDMAX)
                        {
                            Aimsharp.Cast("Weapons of Order");
                            return true;
                        }
                    }

                    //actions.cd_serenity+=/faeline_stomp
                    if (CanCastFaelineStomp)
                    {
                        Aimsharp.Cast("Faeline Stomp");
                        return true;
                    }

                    //actions.cd_serenity+=/fallen_order
                    if (CanCastFallenOrder)
                    {
                        Aimsharp.Cast("Fallen Order");
                        return true;
                    }

                    //actions.cd_serenity+=/bonedust_brew
                    if (CanCastBonedustBrew)
                    {
                        Aimsharp.Cast("BonedustBrewC");
                        return true;
                    }

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

                    //actions.cd_serenity+=/serenity,if=cooldown.rising_sun_kick.remains<2|fight_remains<15
                    if (CanCastSerenity)
                    {
                        if (CDRisingSunKickRemains < 2000 || TargetTimeToDie < 15000)
                        {
                            Aimsharp.Cast("Serenity");
                            return true;
                        }
                    }
                }

                //actions.weapons_of_order+=/energizing_elixir,if=chi.max-chi>=2&energy.time_to_max>3
                if (CanCastEnergizingElixir)
                {
                    if (MaxChi - Chi >= 2 && TimeUntilMaxEnergy > 3000)
                    {
                        Aimsharp.Cast("Energizing Elixir");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/rising_sun_kick,target_if=min:debuff.mark_of_the_crane.remains
                if (CanCastRisingSunKick)
                {
                    Aimsharp.Cast("Rising Sun Kick");
                    return true;
                }

                //actions.weapons_of_order+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&buff.dance_of_chiji.up
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && BuffDanceofChiJiUp)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/fists_of_fury,if=active_enemies>=2&buff.weapons_of_order_ww.remains<1
                if (CanCastFistsofFury)
                {
                    if (EnemiesInMelee >= 2 && BuffWeaponsofOrderRemains < 1000)
                    {
                        Aimsharp.Cast("Fists of Fury");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/whirling_dragon_punch,if=active_enemies>=2
                if (CanCastWhirlingDragonPunch)
                {
                    if (EnemiesInMelee >= 2)
                    {
                        Aimsharp.Cast("Whirling Dragon Punch");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&active_enemies>=3&buff.weapons_of_order_ww.up
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && EnemiesInMelee >= 3 && BuffWeaponsofOrderUp)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike&active_enemies<=2
                if (CanCastBlackoutKick)
                {
                    if (LastCast != "Blackout Kick" && EnemiesInMelee <= 2)
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/whirling_dragon_punch
                if (CanCastWhirlingDragonPunch)
                {
                    Aimsharp.Cast("Whirling Dragon Punch");
                    return true;
                }

                //actions.weapons_of_order+=/fists_of_fury,interrupt=1,if=buff.storm_earth_and_fire.up&raid_event.adds.in>cooldown.fists_of_fury.duration*0.6
                if (CanCastFistsofFury)
                {
                    if (BuffStormEarthandFireUp)
                    {
                        Aimsharp.Cast("Fists of Fury");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/spinning_crane_kick,if=buff.chi_energy.stack>30-5*active_enemies
                if (CanCastSpinningCraneKick)
                {
                    if (BuffChiEnergyStacks > 30 - 5 * EnemiesInMelee)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/fist_of_the_white_tiger,target_if=min:debuff.mark_of_the_crane.remains,if=chi<3
                if (CanCastFistoftheWhiteTiger)
                {
                    if (Chi < 3)
                    {
                        Aimsharp.Cast("Fist of the White Tiger");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/expel_harm,if=chi.max-chi>=1
                if (CanCastExpelHarm)
                {
                    if (MaxChi - Chi >= 1)
                    {
                        Aimsharp.Cast("Expel Harm");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/chi_burst,if=chi.max-chi>=(1+active_enemies>1)
                if (CanCastChiBurst)
                {
                    if (MaxChi - Chi >= (1 + (EnemiesInMelee > 1 ? 1 : 0)))
                    {
                        Aimsharp.Cast("Chi Burst");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains+(debuff.recently_rushing_tiger_palm.up*20),if=(!talent.hit_combo.enabled|combo_strike)&chi.max-chi>=2
                if (CanCastTigerPalm)
                {
                    if ((!TalentHitCombo || LastCast != "Tiger Palm") && MaxChi - Chi >= 2)
                    {
                        Aimsharp.Cast("Tiger Palm");
                        return true;
                    }
                }

                //actions.weapons_of_order+=/chi_wave
                if (CanCastChiWave)
                {
                    Aimsharp.Cast("Chi Wave");
                    return true;
                }

                //actions.weapons_of_order+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=chi>=3|buff.weapons_of_order_ww.up
                if (CanCastBlackoutKick)
                {
                    if (Chi >= 3 || BuffWeaponsofOrderUp)
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=opener,if=time<4&chi<5&!pet.xuen_the_white_tiger.active
            if (Time < 4000 && Chi < 5 && !PetXuenActive)
            {
                //actions.opener=fist_of_the_white_tiger,target_if=min:debuff.mark_of_the_crane.remains,if=chi.max-chi>=3
                if (CanCastFistoftheWhiteTiger)
                {
                    if (MaxChi - Chi >= 3)
                    {
                        Aimsharp.Cast("Fist of the White Tiger");
                        return true;
                    }
                }

                //actions.opener+=/expel_harm,if=talent.chi_burst.enabled&chi.max-chi>=3
                if (CanCastExpelHarm)
                {
                    if (MaxChi - Chi >= 3)
                    {
                        Aimsharp.Cast("Expel Harm");
                        return true;
                    }
                }

                //actions.opener+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains+(debuff.recently_rushing_tiger_palm.up*20),if=combo_strike&chi.max-chi>=2
                if (CanCastTigerPalm)
                {
                    if (LastCast != "Tiger Palm" && MaxChi - Chi >= 2)
                    {
                        Aimsharp.Cast("Tiger Palm");
                        return true;
                    }
                }

                //actions.opener+=/chi_wave,if=chi.max-chi=2
                if (CanCastChiWave)
                {
                    if (MaxChi - Chi == 2)
                    {
                        Aimsharp.Cast("Chi Wave");
                        return true;
                    }
                }

                //actions.opener+=/expel_harm
                if (CanCastExpelHarm)
                {
                    Aimsharp.Cast("Expel Harm");
                    return true;
                }

                //actions.opener+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains+(debuff.recently_rushing_tiger_palm.up*20),if=chi.max-chi>=2
                if (CanCastTigerPalm)
                {
                    if (MaxChi - Chi >= 2)
                    {
                        Aimsharp.Cast("Tiger Palm");
                        return true;
                    }
                }
            }

            //actions+=/fist_of_the_white_tiger,target_if=min:debuff.mark_of_the_crane.remains,if=chi.max-chi>=3&(energy.time_to_max<1|energy.time_to_max<4&cooldown.fists_of_fury.remains<1.5|cooldown.weapons_of_order.remains<2)
            if (CanCastFistoftheWhiteTiger)
            {
                if (MaxChi - Chi >= 3 && (TimeUntilMaxEnergy < 1000 || TimeUntilMaxEnergy < 4000 && CDFistsofFuryRemains < 1500 || CDWeaponsofOrderRemains < 2000))
                {
                    Aimsharp.Cast("Fist of the White Tiger");
                    return true;
                }
            }

            //actions+=/expel_harm,if=chi.max-chi>=1&(energy.time_to_max<1|cooldown.serenity.remains<2|energy.time_to_max<4&cooldown.fists_of_fury.remains<1.5|cooldown.weapons_of_order.remains<2)
            if (CanCastExpelHarm)
            {
                if (MaxChi - Chi >= 1 && (TimeUntilMaxEnergy < 1000 || CDSerenityRemains < 2000 || TimeUntilMaxEnergy < 4000 && CDFistsofFuryRemains < 1500 || CDWeaponsofOrderRemains < 2000))
                {
                    Aimsharp.Cast("Expel Harm");
                    return true;
                }
            }

            //actions+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike&chi.max-chi>=2&(energy.time_to_max<1|cooldown.serenity.remains<2|energy.time_to_max<4&cooldown.fists_of_fury.remains<1.5|cooldown.weapons_of_order.remains<2)
            if (CanCastTigerPalm)
            {
                if (LastCast != "Tiger Palm" && MaxChi - Chi >= 2 && (TimeUntilMaxEnergy < 1000 || CDSerenityRemains < 2000 || TimeUntilMaxEnergy < 4000 && CDFistsofFuryRemains < 1500 || CDWeaponsofOrderRemains < 2000))
                {
                    Aimsharp.Cast("Tiger Palm");
                    return true;
                }
            }

            if (!SaveCooldowns)
            //actions+=/call_action_list,name=cd_sef,if=!talent.serenity.enabled
            if (!TalentSerenity)
            {
                //actions.cd_sef=invoke_xuen_the_white_tiger,if=!variable.hold_xuen|fight_remains<25
                if (CanCastInvokeXuentheWhiteTiger)
                {
                    if (!hold_xuen || TargetTimeToDie < 25000)
                    {
                        Aimsharp.Cast("Invoke Xuen, the White Tiger");
                        return true;
                    }
                }

                //actions.cd_sef+=/touch_of_death,if=buff.storm_earth_and_fire.down&pet.xuen_the_white_tiger.active|fight_remains<10|fight_remains>180
                if (CanCastTouchofDeath)
                {
                    if (!BuffStormEarthandFireUp && PetXuenActive || TargetTimeToDie < 10000 || TargetTimeToDie > 180000)
                    {
                        Aimsharp.Cast("Touch of Death");
                        return true;
                    }
                }

                //actions.cd_sef+=/weapons_of_order,if=(raid_event.adds.in>45|raid_event.adds.up)&cooldown.rising_sun_kick.remains<execute_time
                if (CanCastWeaponsofOrder)
                {
                    if (CDRisingSunKickRemains < GCDMAX)
                    {
                        Aimsharp.Cast("Weapons of Order");
                        return true;
                    }
                }

                //actions.cd_sef+=/faeline_stomp,if=combo_strike&(raid_event.adds.in>10|raid_event.adds.up)
                if (CanCastFaelineStomp)
                {
                    if (LastCast != "Faeline Stomp")
                    {
                        Aimsharp.Cast("Faeline Stomp");
                        return true;
                    }
                }

                //actions.cd_serenity+=/fallen_order
                if (CanCastFallenOrder)
                {
                    Aimsharp.Cast("Fallen Order");
                    return true;
                }

                //actions.cd_serenity+=/bonedust_brew
                if (CanCastBonedustBrew)
                {
                    Aimsharp.Cast("BonedustBrewC");
                    return true;
                }

                //actions.cd_sef+=/storm_earth_and_fire,if=cooldown.storm_earth_and_fire.charges=2|fight_remains<20|(raid_event.adds.remains>15|!covenant.kyrian&((raid_event.adds.in>cooldown.storm_earth_and_fire.full_recharge_time|!raid_event.adds.exists)&(cooldown.invoke_xuen_the_white_tiger.remains>cooldown.storm_earth_and_fire.full_recharge_time|variable.hold_xuen))&cooldown.fists_of_fury.remains<=9&chi>=2&cooldown.whirling_dragon_punch.remains<=12)
                if (CanCastStormEarthandFire)
                {
                    if (StormEarthandFireCharges == 2 || TargetTimeToDie < 20000 || (!CovenantKyrian && ((true) && (CDInvokeXuentheWhiteTigerRemains > StormEarthandFireFullRecharge || hold_xuen)) && CDFistsofFuryRemains <= 9000 && Chi >= 2 && CDWhirlingDragonPunchRemains <= 12000))
                    {
                        Aimsharp.Cast("Storm, Earth, and Fire");
                        return true;
                    }
                }

                //actions.cd_sef+=/storm_earth_and_fire,if=covenant.kyrian&(buff.weapons_of_order.up|(fight_remains<cooldown.weapons_of_order.remains|cooldown.weapons_of_order.remains>cooldown.storm_earth_and_fire.full_recharge_time)&cooldown.fists_of_fury.remains<=9&chi>=2&cooldown.whirling_dragon_punch.remains<=12)
                if (CanCastStormEarthandFire)
                {
                    if (CovenantKyrian && (BuffWeaponsofOrderUp || (TargetTimeToDie < CDWeaponsofOrderRemains || CDWeaponsofOrderRemains > StormEarthandFireFullRecharge) && CDFistsofFuryRemains <= 9000 && Chi >= 2 && CDWhirlingDragonPunchRemains <= 12000))
                    {
                        Aimsharp.Cast("Storm, Earth, and Fire");
                        return true;
                    }
                }



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

                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && (CDInvokeXuentheWhiteTigerRemains > 30000 || hold_xuen))
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }
            }

            if(!SaveCooldowns)
            //actions+=/call_action_list,name=cd_serenity,if=talent.serenity.enabled
            if (TalentSerenity)
            {
                //actions.cd_serenity=variable,name=serenity_burst,op=set,value=cooldown.serenity.remains<1|pet.xuen_the_white_tiger.active&cooldown.serenity.remains>30|fight_remains<20
                bool serenity_burst = CDSerenityRemains < 1000 || PetXuenActive && CDSerenityRemains > 30000 || TargetTimeToDie < 20000;

                //actions.cd_serenity+=/invoke_xuen_the_white_tiger,if=!variable.hold_xuen|fight_remains<25
                if (CanCastInvokeXuentheWhiteTiger)
                {
                    if (!hold_xuen || TargetTimeToDie < 25000)
                    {
                        Aimsharp.Cast("Invoke Xuen, the White Tiger");
                        return true;
                    }
                }

                //actions.cd_serenity+=/blood_fury,if=variable.serenity_burst
                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && serenity_burst)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }

                //actions.cd_serenity+=/touch_of_death,if=fight_remains>180|pet.xuen_the_white_tiger.active|fight_remains<10
                if (CanCastTouchofDeath)
                {
                    if (TargetTimeToDie > 180000 || PetXuenActive || TargetTimeToDie < 10000)
                    {
                        Aimsharp.Cast("Touch of Death");
                        return true;
                    }
                }



                //actions.cd_serenity+=/weapons_of_order,if=cooldown.rising_sun_kick.remains<execute_time
                if (CanCastWeaponsofOrder)
                {
                    if (CDRisingSunKickRemains < GCDMAX)
                    {
                        Aimsharp.Cast("Weapons of Order");
                        return true;
                    }
                }

                //actions.cd_serenity+=/faeline_stomp
                if (CanCastFaelineStomp)
                {
                    Aimsharp.Cast("Faeline Stomp");
                    return true;
                }

                //actions.cd_serenity+=/fallen_order
                if (CanCastFallenOrder)
                {
                    Aimsharp.Cast("Fallen Order");
                    return true;
                }

                //actions.cd_serenity+=/bonedust_brew
                if (CanCastBonedustBrew)
                {
                    Aimsharp.Cast("BonedustBrewC");
                    return true;
                }

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

                //actions.cd_serenity+=/serenity,if=cooldown.rising_sun_kick.remains<2|fight_remains<15
                if (CanCastSerenity)
                {
                    if (CDRisingSunKickRemains < 2000 || TargetTimeToDie < 15000)
                    {
                        Aimsharp.Cast("Serenity");
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=st,if=active_enemies<3
            if (EnemiesInMelee < 3)
            {
                //actions.st=whirling_dragon_punch,if=raid_event.adds.in>cooldown.whirling_dragon_punch.duration*0.8|raid_event.adds.up
                if (CanCastWhirlingDragonPunch)
                {
                    Aimsharp.Cast("Whirling Dragon Punch");
                    return true;
                }

                //actions.st+=/energizing_elixir,if=chi.max-chi>=2&energy.time_to_max>3|chi.max-chi>=4&(energy.time_to_max>2|!prev_gcd.1.tiger_palm)
                if (CanCastEnergizingElixir)
                {
                    if (MaxChi - Chi >= 2 && TimeUntilMaxEnergy > 3000 || MaxChi-Chi>=4 && (TimeUntilMaxEnergy>2000 || LastCast != "Tiger Palm"))
                    {
                        Aimsharp.Cast("Energizing Elixir");
                        return true;
                    }
                }

                //actions.st+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&buff.dance_of_chiji.up&(raid_event.adds.in>buff.dance_of_chiji.remains-2|raid_event.adds.up)
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && BuffDanceofChiJiUp)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.st+=/rising_sun_kick,target_if=min:debuff.mark_of_the_crane.remains,if=cooldown.serenity.remains>1|!talent.serenity.enabled
                if (CanCastRisingSunKick)
                {
                    if (CDSerenityRemains>1000 || !TalentSerenity)
                    {
                        Aimsharp.Cast("Rising Sun Kick");
                        return true;
                    }
                }

                //actions.st+=/fists_of_fury,if=(raid_event.adds.in>cooldown.fists_of_fury.duration*0.8|raid_event.adds.up)&(energy.time_to_max>execute_time-1|chi.max-chi<=1|buff.storm_earth_and_fire.remains<execute_time+1)|fight_remains<execute_time+1
                if (CanCastFistsofFury)
                {
                    if ((MaxChi-Chi<=1 || TimeUntilMaxEnergy>fof_execute_time-1000||BuffStormEarthandFireRemains<fof_execute_time+1000)||TargetTimeToDie<fof_execute_time+1000)
                    {
                        Aimsharp.Cast("Fists of Fury");
                        return true;
                    }
                }

                //actions.st+=/crackling_jade_lightning,if=buff.the_emperors_capacitor.stack>19&energy.time_to_max>execute_time-1&cooldown.rising_sun_kick.remains>execute_time|buff.the_emperors_capacitor.stack>14&(cooldown.serenity.remains<5&talent.serenity.enabled|cooldown.weapons_of_order.remains<5&covenant.kyrian|fight_remains<5)
                if (CanCastCracklingJadeLightning)
                {
                    if (BuffTheEmperorsCapacitorStacks>19 && TimeUntilMaxEnergy>fof_execute_time-1000&&CDRisingSunKickRemains>fof_execute_time || BuffTheEmperorsCapacitorStacks>14&&(CDSerenityRemains<5000&&TalentSerenity||CDWeaponsofOrderRemains<5000&&CovenantKyrian||TargetTimeToDie<5000))
                    {
                        Aimsharp.Cast("Crackling Jade Lightning");
                        return true;
                    }
                }

                //actions.st+=/rushing_jade_wind,if=buff.rushing_jade_wind.down&active_enemies>1
                if (CanCastRushingJadeWind)
                {
                    if (!BuffRushingJadeWindUp && EnemiesInMelee>1)
                    {
                        Aimsharp.Cast("Rushing Jade Wind");
                        return true;
                    }
                }

                //actions.st+=/fist_of_the_white_tiger,target_if=min:debuff.mark_of_the_crane.remains,if=chi<3
                if (CanCastFistoftheWhiteTiger)
                {
                    if (Chi < 3)
                    {
                        Aimsharp.Cast("Fist of the White Tiger");
                        return true;
                    }
                }

                //actions.st+=/expel_harm,if=chi.max-chi>=1
                if (CanCastExpelHarm)
                {
                    if (MaxChi - Chi >= 1)
                    {
                        Aimsharp.Cast("Expel Harm");
                        return true;
                    }
                }

                //actions.st+=/chi_burst,if=chi.max-chi>=1&active_enemies=1&raid_event.adds.in>20|chi.max-chi>=2&active_enemies>=2
                if (CanCastChiBurst)
                {
                    if (MaxChi - Chi >= 1 && EnemiesInMelee==1 || MaxChi-Chi>=2 && EnemiesInMelee>=2)
                    {
                        Aimsharp.Cast("Chi Burst");
                        return true;
                    }
                }

                //actions.st+=/chi_wave
                if (CanCastChiWave)
                {
                    Aimsharp.Cast("Chi Wave");
                    return true;
                }

                //actions.st+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains+(debuff.recently_rushing_tiger_palm.up*20),if=combo_strike&chi.max-chi>=2&buff.storm_earth_and_fire.down
                if (CanCastTigerPalm)
                {
                    if (LastCast != "Tiger Palm" && MaxChi - Chi >= 2 && !BuffStormEarthandFireUp)
                    {
                        Aimsharp.Cast("Tiger Palm");
                        return true;
                    }
                }

                //actions.st+=/spinning_crane_kick,if=buff.chi_energy.stack>30-5*active_enemies&buff.storm_earth_and_fire.down&(cooldown.rising_sun_kick.remains>2&cooldown.fists_of_fury.remains>2|cooldown.rising_sun_kick.remains<3&cooldown.fists_of_fury.remains>3&chi>3|cooldown.rising_sun_kick.remains>3&cooldown.fists_of_fury.remains<3&chi>4|chi.max-chi<=1&energy.time_to_max<2)|buff.chi_energy.stack>10&fight_remains<7
                if (CanCastSpinningCraneKick)
                {
                    if (BuffChiEnergyStacks>30-5*EnemiesInMelee&& !BuffStormEarthandFireUp && (CDRisingSunKickRemains>2000 && CDFistsofFuryRemains>2000||CDRisingSunKickRemains<3000&&CDFistsofFuryRemains>3000&&Chi>3 || CDFistsofFuryRemains<3000 && Chi>3 || MaxChi-Chi<=1 && TimeUntilMaxEnergy<2000) || BuffChiEnergyStacks>10 && TargetTimeToDie<7000)
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.st+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike&(talent.serenity.enabled&cooldown.serenity.remains<3|cooldown.rising_sun_kick.remains>1&cooldown.fists_of_fury.remains>1|cooldown.rising_sun_kick.remains<3&cooldown.fists_of_fury.remains>3&chi>2|cooldown.rising_sun_kick.remains>3&cooldown.fists_of_fury.remains<3&chi>3|chi>5|buff.bok_proc.up)
                if (CanCastBlackoutKick)
                {
                    if (LastCast != "Blackout Kick" && (TalentSerenity && CDSerenityRemains < 3000 || CDRisingSunKickRemains > 1000 && CDFistsofFuryRemains > 1000 || CDRisingSunKickRemains < 3000 && CDFistsofFuryRemains > 3000 && Chi > 2 || CDRisingSunKickRemains > 3000 && CDFistsofFuryRemains < 3000 && Chi > 3 || Chi>5 || BuffBlackoutKickUp))
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }

                //actions.st+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains+(debuff.recently_rushing_tiger_palm.up*20),if=combo_strike&chi.max-chi>=2
                if (CanCastTigerPalm)
                {
                    if (LastCast != "Tiger Palm" && MaxChi - Chi >= 2)
                    {
                        Aimsharp.Cast("Tiger Palm");
                        return true;
                    }
                }

                //actions.st+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike&cooldown.fists_of_fury.remains<3&chi=2&prev_gcd.1.tiger_palm&energy.time_to_50<1
                if (CanCastBlackoutKick)
                {
                    if (LastCast != "Blackout Kick" && CDFistsofFuryRemains<3000 && Chi==2 && LastCast == "Tiger Palm" && Energy>=50)
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }

                //actions.st+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike&energy.time_to_max<2&(chi.max-chi<=1|prev_gcd.1.tiger_palm)
                if (CanCastBlackoutKick)
                {
                    if (LastCast != "Blackout Kick" && TimeUntilMaxEnergy<2000 && (MaxChi-Chi<=1 || LastCast == "Tiger Palm"))
                    {
                        Aimsharp.Cast("Blackout Kick");
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=aoe,if=active_enemies>=3
            if (EnemiesInMelee>=3)
            {
                //actions.aoe=whirling_dragon_punch
                if (CanCastWhirlingDragonPunch)
                {
                    Aimsharp.Cast("Whirling Dragon Punch");
                    return true;
                }

                //actions.aoe+=/energizing_elixir,if=chi.max-chi>=2&energy.time_to_max>2|chi.max-chi>=4
                if (CanCastEnergizingElixir)
                {
                    if (MaxChi - Chi >= 2 && TimeUntilMaxEnergy > 2000 || MaxChi-Chi>=4)
                    {
                        Aimsharp.Cast("Energizing Elixir");
                        return true;
                    }
                }

                //actions.aoe+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&(buff.dance_of_chiji.up|debuff.bonedust_brew.up)
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes || LastCast != "Spinning Crane Kick") && (BuffDanceofChiJiUp || DebuffBonedustBrewUp))
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.aoe+=/fists_of_fury,if=energy.time_to_max>execute_time|chi.max-chi<=1
                if (CanCastFistsofFury)
                {
                    if (TimeUntilMaxEnergy>fof_execute_time || MaxChi-Chi<=1)
                    {
                        Aimsharp.Cast("Fists of Fury");
                        return true;
                    }
                }

                //actions.aoe+=/rising_sun_kick,target_if=min:debuff.mark_of_the_crane.remains,if=(talent.whirling_dragon_punch.enabled&cooldown.rising_sun_kick.duration>cooldown.whirling_dragon_punch.remains+4)&(cooldown.fists_of_fury.remains>3|chi>=5)
                if (CanCastRisingSunKick)
                {
                    if ((TalentWhirlingDragonPunch && 10000>CDWhirlingDragonPunchRemains+4000)&& (CDFistsofFuryRemains>3000||Chi>=5))
                    {
                        Aimsharp.Cast("Rising Sun Kick");
                        return true;
                    }
                }

                //actions.aoe+=/spinning_crane_kick,if=(!talent.hit_combo.enabled&conduit.calculated_strikes.enabled|combo_strike)&((cooldown.bonedust_brew.remains>2&(chi>3|cooldown.fists_of_fury.remains>6)&(chi>=5|cooldown.fists_of_fury.remains>2))|energy.time_to_max<=3)
                if (CanCastSpinningCraneKick)
                {
                    if ((!TalentHitCombo && ConduitCalculatedStrikes||LastCast !="Spinning Crane Kick")&&((CDBonedustBrewRemains>2000&&(Chi>3||CDFistsofFuryRemains>6000)&&(Chi>=5||CDFistsofFuryRemains>2000))|TimeUntilMaxEnergy<=3000))
                    {
                        Aimsharp.Cast("Spinning Crane Kick");
                        return true;
                    }
                }

                //actions.aoe+=/expel_harm,if=chi.max-chi>=1
                if (CanCastExpelHarm)
                {
                    if (MaxChi - Chi >= 1)
                    {
                        Aimsharp.Cast("Expel Harm");
                        return true;
                    }
                }

                //actions.aoe+=/fist_of_the_white_tiger,target_if=min:debuff.mark_of_the_crane.remains,if=chi.max-chi>=3
                if (CanCastFistoftheWhiteTiger)
                {
                    if (MaxChi - Chi >= 3)
                    {
                        Aimsharp.Cast("Fist of the White Tiger");
                        return true;
                    }
                }

                //actions.aoe+=/chi_burst,if=chi.max-chi>=2
                if (CanCastChiBurst)
                {
                    if (MaxChi - Chi >= 2)
                    {
                        Aimsharp.Cast("Chi Burst");
                        return true;
                    }
                }

                //actions.aoe+=/crackling_jade_lightning,if=buff.the_emperors_capacitor.stack>19&energy.time_to_max>execute_time-1&cooldown.fists_of_fury.remains>execute_time
                if (CanCastCracklingJadeLightning)
                {
                    if (BuffTheEmperorsCapacitorStacks > 19 && TimeUntilMaxEnergy > fof_execute_time - 1000 && CDFistsofFuryRemains>fof_execute_time)
                    {
                        Aimsharp.Cast("Crackling Jade Lightning");
                        return true;
                    }
                }

                //actions.aoe+=/tiger_palm,target_if=min:debuff.mark_of_the_crane.remains+(debuff.recently_rushing_tiger_palm.up*20),if=chi.max-chi>=2&(!talent.hit_combo.enabled|combo_strike)
                if (CanCastTigerPalm)
                {
                    if (MaxChi-Chi>=2 && (!TalentHitCombo||LastCast != "Tiger Palm"))
                    {
                        Aimsharp.Cast("Tiger Palm");
                        return true;
                    }
                }

                //actions.aoe+=/chi_wave,if=combo_strike
                if (CanCastChiWave)
                {
                    if (LastCast != "Chi Wave")
                    {
                        Aimsharp.Cast("Chi Wave");
                        return true;
                    }
                }

                //actions.aoe+=/blackout_kick,target_if=min:debuff.mark_of_the_crane.remains,if=combo_strike&(buff.bok_proc.up|talent.hit_combo.enabled&prev_gcd.1.tiger_palm&chi=2&cooldown.fists_of_fury.remains<3|chi.max-chi<=1&prev_gcd.1.spinning_crane_kick&energy.time_to_max<3)
                if (CanCastBlackoutKick)
                {
                    if (LastCast != "Blackout Kick" && (BuffBlackoutKickUp||TalentHitCombo&&LastCast == "Tiger Palm" && Chi==2&&CDFistsofFuryRemains<3000||MaxChi-Chi<=1 && LastCast =="Spinning Crane Kick" && TimeUntilMaxEnergy<3000))
                    {
                        Aimsharp.Cast("Blackout Kick");
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
