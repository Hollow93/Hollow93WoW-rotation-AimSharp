using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class Aimsharp9_1Elemental : Rotation
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
           "Flame Shock","Primordial Wave","Fire Elemental","Vesper Totem","Fae Transfusion","Earthquake","Chain Harvest","Stormkeeper","Echoing Shock","Ascendance","Liquid Magma Totem","Chain Lightning","Earth Shock","Lava Burst","Elemental Blast","Lava Beam","Frost Shock","Lightning Bolt","Static Discharge","Earth Elemental","Icefury",
        };

        List<string> BuffsList = new List<string>
        {
            "Primordial Wave","Wind Gust","Ascendance","Master of the Elements","Echoing Shock","Echoes of Great Sundering","Icefury","Lava Surge","Stormkeeper","Bloodlust","Elemental Equilibrium"
        };

        List<string> DebuffsList = new List<string>
        {
           "Flame Shock","Elemental Equilibrium"
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

            Settings.Add(new Setting("Shaman Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Skybreakers Fiery Demise", "Echoes of Great Sundering", "Windspeakers Lava Resurgence", "Elemental Equilibrium", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Aimsharp 9_1Elemental", Color.Purple);
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

            Macros.Add("VesperTotemC", "/cast [@cursor] Vesper Totem");
            Macros.Add("FaeTransfusionC", "/cast [@cursor] Fae Transfusion");
            Macros.Add("EarthquakeC", "/cast [@cursor] Earthquake");
            Macros.Add("LiquidMagmaTotemC", "/cast [@cursor] Liquid Magma Totem");


            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            CustomFunctions.Add("FlameShockCount", "local FlameShockCount = 0\nfor i=1,20 do\nlocal unit = \"nameplate\" .. i\nif UnitExists(unit) then\nif UnitCanAttack(\"player\", unit) then\nfor j = 1, 40 do\nlocal name,_,_,_,_,_,source = UnitDebuff(unit, j)\nif name == \"Flame Shock\" and source == \"player\" then\nFlameShockCount = FlameShockCount + 1\nend\nend\nend\nend\nend\nreturn FlameShockCount");

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
            int Maelstrom = Aimsharp.Power("player");
            int Mana = Aimsharp.PlayerSecondaryPower();
            int MaxMaelstrom = Aimsharp.PlayerMaxPower();
            int MaelstromDefecit = MaxMaelstrom - Maelstrom;

            //Talents
            bool TalentElementalBlast = Aimsharp.Talent(2, 3);
            bool TalentMasteroftheElements = Aimsharp.Talent(4, 1);
            bool TalentAscendance = Aimsharp.Talent(7, 3);
            bool TalentStormElemental = Aimsharp.Talent(4, 2);
            bool TalentStormkeeper = Aimsharp.Talent(7, 2);
            bool TalentEchoingShock = Aimsharp.Talent(2, 2);
            bool TalentIcefury = Aimsharp.Talent(6, 3);
            bool TalentLiquidMagmaTotem = Aimsharp.Talent(4, 3);
            bool TalentEchooftheElements = Aimsharp.Talent(1, 2);
            bool TalentStaticDischarge = Aimsharp.Talent(1, 3);
            bool TalentPrimalElementalist = Aimsharp.Talent(6, 2);


            //buffs
            int BuffPrimordialWaveRemains = Aimsharp.BuffRemaining("Primordial Wave") - GCD;
            bool BuffPrimordialWaveUp = BuffPrimordialWaveRemains > 0;
            int BuffWindGustRemains = Aimsharp.BuffRemaining("Wind Gust") - GCD;
            bool BuffWindGustUp = BuffWindGustRemains > 0;
            int BuffWindGustStacks = Aimsharp.BuffStacks("Wind Gust");
            int BuffAscendanceRemains = Aimsharp.BuffRemaining("Ascendance") - GCD;
            bool BuffAscendanceUp = BuffAscendanceRemains > 0;
            int BuffMasteroftheElementsRemains = Aimsharp.BuffRemaining("Master of the Elements") - GCD;
            bool BuffMasteroftheElementsUp = BuffMasteroftheElementsRemains > 0;
            int BuffEchoingShockRemains = Aimsharp.BuffRemaining("Echoing Shock") - GCD;
            bool BuffEchoingShockUp = BuffEchoingShockRemains > 0;
            int BuffEchoesofGreatSunderingRemains = Aimsharp.BuffRemaining("Echoes of Great Sundering") - GCD;
            bool BuffEchoesofGreatSunderingUp = BuffEchoesofGreatSunderingRemains > 0;
            int BuffIcefuryRemains = Aimsharp.BuffRemaining("Icefury") - GCD;
            bool BuffIcefuryUp = BuffIcefuryRemains > 0;
            int BuffIcefuryStacks = Aimsharp.BuffStacks("Icefury");
            int BuffLavaSurgeRemains = Aimsharp.BuffRemaining("Lava Surge") - GCD;
            bool BuffLavaSurgeUp = BuffLavaSurgeRemains > 0;
            int BuffStormkeeperRemains = Aimsharp.BuffRemaining("Stormkeeper") - GCD;
            bool BuffStormkeeperUp = BuffStormkeeperRemains > 0;
            int BuffStormkeeperStacks = Aimsharp.BuffStacks("Stormkeeper");
            int BuffBloodlustRemains = Aimsharp.BuffRemaining("Bloodlust") - GCD;
            bool BuffBloodlustUp = BuffBloodlustRemains > 0;
            int BuffElementalEquilibriumRemains = Aimsharp.BuffRemaining("Elemental Equilibrium") - GCD;
            bool BuffElementalEquilibriumUp = BuffElementalEquilibriumRemains > 0;


            //debuffs
            int DotFlameShockRemains = Aimsharp.DebuffRemaining("Flame Shock") - GCD;
            bool DotFlameShockUp = DotFlameShockRemains > 0;
            bool DotFlameShockRefreshable = DotFlameShockRemains < 5400;
            int DebuffElementalEquilibriumRemains = Aimsharp.BuffRemaining("Elemental Equilibrium","player") - GCD;
            bool DebuffElementalEquilibriumUp = BuffElementalEquilibriumRemains > 0;


            //cooldowns
            int CDPrimordialWaveRemains = Aimsharp.SpellCooldown("Primordial Wave");
            bool CDPrimordialWaveUp = CDPrimordialWaveRemains <= 0;
            int CDAscendanceRemains = Aimsharp.SpellCooldown("Ascendance");
            bool CDAscendanceUp = CDAscendanceRemains <= 0;
            int CDLavaBurstRemains = Aimsharp.SpellCooldown("Lava Burst");
            bool CDLavaBurstUp = CDLavaBurstRemains <= 0;
            int CDLavaBurstCharges = Aimsharp.SpellCharges("Lava Burst");
            int CDLavaBurstFullRecharge = Aimsharp.RechargeTime("Lava Burst") + 8000 * (Aimsharp.MaxCharges("Lava Burst") - CDLavaBurstCharges - 1);
            float CDLavaBurstFractional = CDLavaBurstCharges + (1 - (Aimsharp.RechargeTime("Lava Burst") - GCD) / 8000f);
            CDLavaBurstFractional = CDLavaBurstFractional > Aimsharp.MaxCharges("Lava Burst") ? Aimsharp.MaxCharges("Lava Burst") : CDLavaBurstFractional;
            int CDElementalBlastRemains = Aimsharp.SpellCooldown("Elemental Blast");
            bool CDElementalBlastUp = CDElementalBlastRemains <= 0;
            int CDIcefuryRemains = Aimsharp.SpellCooldown("Icefury");
            bool CDIcefuryUp = CDIcefuryRemains <= 0;


            //specific variables
            bool RuneforgeSkybreakersFieryDemise = RuneforgePower == "Skybreakers Fiery Demise";
            bool RuneforgeEchoesofGreatSundering = RuneforgePower == "Echoes of Great Sundering";
            bool RuneforgeWindspeakersLavaResurgence = RuneforgePower == "Windspeakers Lava Resurgence";
            bool RuneforgeElementalEquilibrium = RuneforgePower == "Elemental Equilibrium";
            bool ConduitLeadbyExample = ActiveConduits.Contains(342156);
            bool PetStormElementalActive = Aimsharp.PlayerHasPet() && TalentStormElemental;
            int ActiveFlameShocks = Aimsharp.CustomFunction("FlameShockCount");

            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastFlameShock = Aimsharp.CanCast("Flame Shock") && Fighting;
            bool CanCastPrimordialWave = Aimsharp.CanCast("Primordial Wave") && Fighting;
            bool CanCastFireElemental = Aimsharp.CanCast("Fire Elemental") && Fighting;
            bool CanCastVesperTotem = Aimsharp.CanCast("Vesper Totem", "player") && Fighting;
            bool CanCastFaeTransfusion = Aimsharp.CanCast("Fae Transfusion", "player") && !SaveCooldowns && Fighting;
            bool CanCastEarthquake = Aimsharp.CanCast("Earthquake", "player") && Fighting;
            bool CanCastChainHarvest = Aimsharp.CanCast("Chain Harvest") && !SaveCooldowns && !Moving && Fighting;
            bool CanCastStormkeeper = Aimsharp.CanCast("Stormkeeper", "player") && !Moving && Fighting;
            bool CanCastEchoingShock = Aimsharp.CanCast("Echoing Shock") && Fighting;
            bool CanCastAscendance = Aimsharp.CanCast("Ascendance", "player") && Fighting;
            bool CanCastLiquidMagmaTotem = Aimsharp.CanCast("Liquid Magma Totem", "player") && Fighting;
            bool CanCastChainLightning = Aimsharp.CanCast("Chain Lightning") && (BuffStormkeeperUp || !Moving) && Fighting;
            bool CanCastEarthShock = Aimsharp.CanCast("Earth Shock") && Fighting;
            bool CanCastLavaBurst = Aimsharp.CanCast("Lava Burst") && (BuffLavaSurgeUp || !Moving) && Fighting;
            bool CanCastElementalBlast = Aimsharp.CanCast("Elemental Blast") && !Moving && Fighting;
            bool CanCastLavaBeam = Aimsharp.CanCast("Lava Beam") && !Moving && Fighting;
            bool CanCastFrostShock = Aimsharp.CanCast("Frost Shock") && Fighting;
            bool CanCastLightningBolt = Aimsharp.CanCast("Lightning Bolt") && (BuffStormkeeperUp || !Moving) && Fighting;
            bool CanCastStaticDischarge = Aimsharp.CanCast("Static Discharge", "player") && Fighting;
            bool CanCastEarthElemental = Aimsharp.CanCast("Earth Elemental") && Fighting;
            bool CanCastIcefury = Aimsharp.CanCast("Icefury") && !Moving && Fighting;
            bool CanCastStormElemental = Aimsharp.CanCast("Storm Elemental") && Fighting && TalentStormElemental;


            int CastingRemaining = Aimsharp.CastingRemaining("player");

            //predictions
            if (PlayerCastingID == 51505 && CastingRemaining < 500)
            {
                BuffMasteroftheElementsUp = TalentMasteroftheElements;
                Maelstrom = Maelstrom + 10;
            }

            if (PlayerCastingID == 117014 && CastingRemaining < 500)
            {
                BuffMasteroftheElementsUp = false;
                Maelstrom = Maelstrom + 30;
            }

            if (PlayerCastingID == 188196 && CastingRemaining < 500)
            {
                BuffMasteroftheElementsUp = false;
                Maelstrom = Maelstrom + 8;
            }

            if (PlayerCastingID == 188443 && CastingRemaining < 500)
            {
                BuffMasteroftheElementsUp = false;
                Maelstrom = Maelstrom + 8;
            }

            if (PlayerCastingID == 210714 && CastingRemaining < 500)
            {
                BuffMasteroftheElementsUp = false;
                Maelstrom = Maelstrom + 25;
            }

            if (PlayerCastingID == 191634 && CastingRemaining < 500)
            {
                BuffStormkeeperRemains = 15000;
                BuffStormkeeperUp = true;
            }


            // end of Simc conditionals
            #endregion



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

            //actions+=/flame_shock,if=(!talent.elemental_blast.enabled)&!ticking&!pet.storm_elemental.active&(spell_targets.chain_lightning<3|talent.master_of_the_elements.enabled|runeforge.skybreakers_fiery_demise.equipped)
            if (CanCastFlameShock)
            {
                if ((!TalentElementalBlast) && !DotFlameShockUp && !PetStormElementalActive && (EnemiesNearTarget < 3 || TalentMasteroftheElements || RuneforgeSkybreakersFieryDemise))
                    return FlameShock();
            }

            //actions+=/primordial_wave,target_if=min:dot.flame_shock.remains,cycle_targets=1,if=!buff.primordial_wave.up&(!pet.storm_elemental.active|spell_targets.chain_lightning<3&buff.wind_gust.stack<20|soulbind.lead_by_example.enabled)&(spell_targets.chain_lightning<5|talent.master_of_the_elements.enabled|runeforge.skybreakers_fiery_demise.equipped|soulbind.lead_by_example.enabled)
            if (CanCastPrimordialWave)
            {
                if (!BuffPrimordialWaveUp && (!PetStormElementalActive || EnemiesNearTarget < 3 && BuffWindGustStacks < 20 || ConduitLeadbyExample) && (EnemiesNearTarget < 5 || TalentMasteroftheElements || RuneforgeSkybreakersFieryDemise || ConduitLeadbyExample))
                    return PrimordialWave();
            }

            //actions+=/flame_shock,if=!ticking&(!pet.storm_elemental.active|spell_targets.chain_lightning<3&buff.wind_gust.stack<20)&(spell_targets.chain_lightning<3|talent.master_of_the_elements.enabled|runeforge.skybreakers_fiery_demise.equipped)
            if (CanCastFlameShock)
            {
                if (!DotFlameShockUp && (!PetStormElementalActive || EnemiesNearTarget < 3 && BuffWindGustStacks < 20) && (EnemiesNearTarget < 3 || TalentMasteroftheElements || RuneforgeSkybreakersFieryDemise))
                    return FlameShock();
            }

            //actions+=/fire_elemental
            if (CanCastFireElemental)
            {
                return FireElemental();
            }

            //actions+=/blood_fury,if=!talent.ascendance.enabled|buff.ascendance.up|cooldown.ascendance.remains>50
            if (!TalentAscendance || BuffAscendanceUp || CDAscendanceRemains > 50000)
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

            //actions+=/vesper_totem,if=covenant.kyrian
            if (CanCastVesperTotem)
            {
                return VesperTotem();
            }

            //actions+=/fae_transfusion,if=covenant.night_fae&(!talent.master_of_the_elements.enabled|buff.master_of_the_elements.up)&spell_targets.chain_lightning<3
            if (CanCastFaeTransfusion)
            {
                if ((!TalentMasteroftheElements || BuffMasteroftheElementsUp) && EnemiesNearTarget < 3)
                    return FaeTransfusion();
            }

            //actions+=/run_action_list,name=aoe,if=active_enemies>2&(spell_targets.chain_lightning>2|spell_targets.lava_beam>2)
            if (EnemiesNearTarget>2)
            {
                if (CanCastStormElemental)
                {
                    return StormElemental();
                }

                //actions.aoe+=/earthquake,if=buff.echoing_shock.up
                if (CanCastEarthquake && BuffEchoingShockUp)
                    return Earthquake();

                //actions.aoe+=/chain_harvest
                if (CanCastChainHarvest)
                    return ChainHarvest();

                //actions.aoe+=/stormkeeper,if=talent.stormkeeper.enabled
                if (CanCastStormkeeper)
                    return Stormkeeper();

                //actions.aoe+=/flame_shock,if=(active_dot.flame_shock<2&active_enemies<=3&cooldown.primordial_wave.remains<16&covenant.necrolord&!pet.storm_elemental.active|active_dot.flame_shock<1&active_enemies>=4&!pet.storm_elemental.active&talent.master_of_the_elements.enabled)|(runeforge.skybreakers_fiery_demise.equipped&!pet.storm_elemental.active),target_if=refreshable
                if (CanCastFlameShock)
                {
                    if (((ActiveFlameShocks < 2 && EnemiesNearTarget <= 3 && CDPrimordialWaveRemains < 16000 && CovenantNecrolord && !PetStormElementalActive || ActiveFlameShocks < 1 && EnemiesNearTarget >= 4 && !PetStormElementalActive && TalentMasteroftheElements) || (RuneforgeSkybreakersFieryDemise && !PetStormElementalActive)) && DotFlameShockRefreshable)
                        return FlameShock();
                }

                //actions.aoe+=/flame_shock,if=!active_dot.flame_shock&!pet.storm_elemental.active&(talent.master_of_the_elements.enabled|runeforge.skybreakers_fiery_demise.equipped)
                if (CanCastFlameShock)
                {
                    if (ActiveFlameShocks == 0 && !PetStormElementalActive && (TalentMasteroftheElements || RuneforgeSkybreakersFieryDemise))
                        return FlameShock();
                }

                //actions.aoe+=/echoing_shock,if=talent.echoing_shock.enabled&maelstrom>=60&(runeforge.echoes_of_great_sundering.equipped&buff.echoes_of_great_sundering.up|!runeforge.echoes_of_great_sundering.equipped)
                if (CanCastEchoingShock)
                {
                    if (TalentEchoingShock && Maelstrom >= 60 && (RuneforgeEchoesofGreatSundering && BuffEchoesofGreatSunderingUp || !RuneforgeEchoesofGreatSundering))
                        return EchoingShock();
                }

                //actions.aoe+=/ascendance,if=talent.ascendance.enabled&(!pet.storm_elemental.active)&(!talent.icefury.enabled|!buff.icefury.up&!cooldown.icefury.up)
                if (CanCastAscendance)
                {
                    if (TalentAscendance && (!PetStormElementalActive) && (!TalentIcefury || !BuffIcefuryUp && !CDIcefuryUp))
                        return Ascendance();
                }

                //actions.aoe+=/liquid_magma_totem,if=talent.liquid_magma_totem.enabled
                if (CanCastLiquidMagmaTotem)
                    return LiquidMagmaTotem();

                //actions.aoe+=/chain_lightning,if=spell_targets.chain_lightning<4&buff.master_of_the_elements.up&maelstrom<50
                if (CanCastChainLightning)
                {
                    if (EnemiesNearTarget < 4 && BuffMasteroftheElementsUp && Maelstrom < 50)
                        return ChainLightning();
                }

                //actions.aoe+=/earth_shock,if=runeforge.echoes_of_great_sundering.equipped&!buff.echoes_of_great_sundering.up
                if (CanCastEarthShock)
                {
                    if (RuneforgeEchoesofGreatSundering && !BuffEchoesofGreatSunderingUp)
                        return EarthShock();
                }

                //actions.aoe+=/lava_burst,target_if=dot.flame_shock.remains,if=spell_targets.chain_lightning<4&(!pet.storm_elemental.active)&(buff.lava_surge.up&!buff.master_of_the_elements.up&talent.master_of_the_elements.enabled)
                if (CanCastLavaBurst)
                {
                    if (EnemiesNearTarget < 4 && (!PetStormElementalActive) && (BuffLavaSurgeUp && !BuffMasteroftheElementsUp && TalentMasteroftheElements))
                        return LavaBurst();
                }

                //actions.aoe+=/earthquake,if=spell_targets.chain_lightning>=2&!runeforge.echoes_of_great_sundering.equipped&(talent.master_of_the_elements.enabled&maelstrom>=50&!buff.master_of_the_elements.up)
                if (CanCastEarthquake)
                {
                    if (EnemiesNearTarget >= 2 && !RuneforgeEchoesofGreatSundering && (TalentMasteroftheElements && Maelstrom >= 50 && !BuffMasteroftheElementsUp))
                        return Earthquake();
                }

                //actions.aoe+=/lava_burst,target_if=dot.flame_shock.remains,if=buff.lava_surge.up&buff.primordial_wave.up
                if (CanCastLavaBurst)
                {
                    if (BuffLavaSurgeUp && BuffPrimordialWaveUp)
                        return LavaBurst();
                }

                //actions.aoe+=/lava_burst,target_if=dot.flame_shock.remains,if=spell_targets.chain_lightning<4&runeforge.skybreakers_fiery_demise.equipped&buff.lava_surge.up&talent.master_of_the_elements.enabled&!buff.master_of_the_elements.up&maelstrom>=50
                if (CanCastLavaBurst)
                {
                    if (EnemiesNearTarget < 4 && RuneforgeSkybreakersFieryDemise && BuffLavaSurgeUp && TalentMasteroftheElements && !BuffMasteroftheElementsUp && Maelstrom >= 50)
                        return LavaBurst();
                }

                //actions.aoe+=/lava_burst,target_if=dot.flame_shock.remains,if=(spell_targets.chain_lightning<4&runeforge.skybreakers_fiery_demise.equipped&talent.master_of_the_elements.enabled)|(talent.master_of_the_elements.enabled&maelstrom>=50&!buff.master_of_the_elements.up&(!runeforge.echoes_of_great_sundering.equipped|buff.echoes_of_great_sundering.up)&!runeforge.skybreakers_fiery_demise.equipped)
                if (CanCastLavaBurst)
                {
                    if ((EnemiesNearTarget < 4 && RuneforgeSkybreakersFieryDemise && TalentMasteroftheElements) || (TalentMasteroftheElements && Maelstrom >= 50 && !BuffMasteroftheElementsUp && (!RuneforgeEchoesofGreatSundering || BuffEchoesofGreatSunderingUp) && !RuneforgeSkybreakersFieryDemise))
                        return LavaBurst();
                }

                //actions.aoe+=/lava_burst,target_if=dot.flame_shock.remains,if=spell_targets.chain_lightning=4&runeforge.skybreakers_fiery_demise.equipped&buff.lava_surge.up&talent.master_of_the_elements.enabled&!buff.master_of_the_elements.up&maelstrom>=50
                if (CanCastLavaBurst)
                {
                    if (EnemiesNearTarget == 4 && RuneforgeSkybreakersFieryDemise && BuffLavaSurgeUp && TalentMasteroftheElements && !BuffMasteroftheElementsUp && Maelstrom >= 50)
                        return LavaBurst();
                }

                //actions.aoe+=/earthquake,if=spell_targets.chain_lightning>=2
                if (CanCastEarthquake)
                {
                    if (EnemiesNearTarget >= 2)
                        return Earthquake();
                }

                //actions.aoe+=/chain_lightning,if=buff.stormkeeper.remains<3*gcd*buff.stormkeeper.stack
                if (CanCastChainLightning)
                {
                    if (BuffStormkeeperRemains - 500 < 3 * GCDMAX * BuffStormkeeperStacks)
                        return ChainLightning();
                }

                //actions.aoe+=/lava_burst,if=buff.lava_surge.up&spell_targets.chain_lightning<4&(!pet.storm_elemental.active)&dot.flame_shock.ticking
                if (CanCastLavaBurst)
                {
                    if (BuffLavaSurgeUp && EnemiesNearTarget < 4 && (!PetStormElementalActive) && DotFlameShockUp)
                        return LavaBurst();
                }

                //actions.aoe+=/elemental_blast,if=talent.elemental_blast.enabled&spell_targets.chain_lightning<5&(!pet.storm_elemental.active)
                if (CanCastElementalBlast)
                {
                    if (TalentElementalBlast && EnemiesNearTarget < 5 && (!PetStormElementalActive))
                        return ElementalBlast();
                }

                //actions.aoe+=/lava_beam,if=talent.ascendance.enabled
                if (CanCastLavaBeam && BuffAscendanceUp)
                    return LavaBeam();

                //actions.aoe+=/chain_lightning
                if (CanCastChainLightning)
                    return ChainLightning();

                //actions.aoe+=/lava_burst,moving=1,if=buff.lava_surge.up&cooldown_react
                if (CanCastLavaBurst)
                {
                    if (Moving && BuffLavaSurgeUp)
                        return LavaBurst();
                }

                //actions.aoe+=/flame_shock,moving=1,target_if=refreshable
                if (CanCastFlameShock)
                {
                    if (Moving && DotFlameShockRefreshable)
                        return FlameShock();
                }

                //actions.aoe+=/frost_shock,moving=1
                if (CanCastFrostShock)
                {
                    if (Moving)
                        return FrostShock();
                }
            }

            //actions+=/run_action_list,name=single_target,if=!talent.storm_elemental.enabled&active_enemies<=2
            if (!TalentStormElemental && EnemiesNearTarget<=2)
            {
                //actions.single_target=lightning_bolt,if=(buff.stormkeeper.remains<1.1*gcd*buff.stormkeeper.stack)
                if (CanCastLightningBolt)
                {
                    if (BuffStormkeeperUp && (BuffStormkeeperRemains - 500 < GCDMAX * BuffStormkeeperStacks))
                        return LightningBolt();
                }

                //actions.single_target+=/frost_shock,if=talent.icefury.enabled&buff.icefury.up&buff.icefury.remains<1.1*gcd*buff.icefury.stack
                if (CanCastFrostShock)
                {
                    if (TalentIcefury && BuffIcefuryUp && BuffIcefuryRemains - 500 < GCDMAX * BuffIcefuryStacks)
                        return FrostShock();
                }

                //actions.single_target+=/flame_shock,target_if=(!ticking|dot.flame_shock.remains<=gcd|talent.ascendance.enabled&dot.flame_shock.remains<(cooldown.ascendance.remains+buff.ascendance.duration)&cooldown.ascendance.remains<4)&(buff.lava_surge.up|!buff.bloodlust.up)
                if (CanCastFlameShock)
                {
                    if ((!DotFlameShockUp || DotFlameShockRemains <= GCDMAX || TalentAscendance && DotFlameShockRemains < (CDAscendanceRemains + 15000) && CDAscendanceRemains < 4000) && (BuffLavaSurgeUp || !BloodlustUp))
                        return FlameShock();
                }

                //actions.single_target+=/flame_shock,if=buff.primordial_wave.up,target_if=min:dot.flame_shock.remains,cycle_targets=1,target_if=refreshable
                if (CanCastFlameShock)
                {
                    if (BuffPrimordialWaveUp && DotFlameShockRefreshable)
                        return FlameShock();
                }

                //actions.single_target+=/ascendance,if=talent.ascendance.enabled&(time>=60|buff.bloodlust.up)&(cooldown.lava_burst.remains>0)&(!talent.icefury.enabled|!buff.icefury.up&!cooldown.icefury.up)
                if (CanCastAscendance)
                {
                    if (TalentAscendance && (Time >= 60000 || BloodlustUp) && (CDLavaBurstRemains > 0) && (!TalentIcefury || !BuffIcefuryUp && !CDIcefuryUp))
                        return Ascendance();
                }

                //actions.single_target+=/lava_burst,if=buff.lava_surge.up&(runeforge.windspeakers_lava_resurgence.equipped|!buff.master_of_the_elements.up&talent.master_of_the_elements.enabled)
                if (CanCastLavaBurst)
                {
                    if (BuffLavaSurgeUp && (RuneforgeWindspeakersLavaResurgence || !BuffMasteroftheElementsUp && TalentMasteroftheElements))
                        return LavaBurst();
                }

                //actions.single_target+=/elemental_blast,if=talent.elemental_blast.enabled&(maelstrom<70)
                if (CanCastElementalBlast)
                {
                    if (TalentElementalBlast && Maelstrom < 70)
                        return ElementalBlast();
                }

                //actions.single_target+=/stormkeeper,if=talent.stormkeeper.enabled&(raid_event.adds.count<3|raid_event.adds.in>50)&(maelstrom<44)
                if (CanCastStormkeeper)
                {
                    if (TalentStormkeeper && Maelstrom < 44)
                        return Stormkeeper();
                }

                //actions.single_target+=/echoing_shock,if=talent.echoing_shock.enabled&cooldown.lava_burst.remains<=gcd
                if (CanCastEchoingShock)
                {
                    if (TalentEchoingShock && CDLavaBurstRemains <= GCDMAX)
                        return EchoingShock();
                }

                //actions.single_target+=/lava_burst,if=talent.echoing_shock.enabled&buff.echoing_shock.up
                if (CanCastLavaBurst)
                {
                    if (TalentEchoingShock && BuffEchoingShockUp)
                        return LavaBurst();
                }

                //actions.single_target+=/liquid_magma_totem,if=talent.liquid_magma_totem.enabled
                if (CanCastLiquidMagmaTotem)
                {
                    return LiquidMagmaTotem();
                }

                //actions.single_target+=/earthquake,if=buff.echoes_of_great_sundering.up&talent.master_of_the_elements.enabled&buff.master_of_the_elements.up
                if (CanCastEarthquake)
                {
                    if (BuffEchoesofGreatSunderingUp && TalentMasteroftheElements && BuffMasteroftheElementsUp)
                        return Earthquake();
                }

                //actions.single_target+=/lightning_bolt,if=buff.stormkeeper.up&buff.master_of_the_elements.up&maelstrom<60
                if (CanCastLightningBolt)
                {
                    if (BuffStormkeeperUp && BuffMasteroftheElementsUp && Maelstrom < 60)
                        return LightningBolt();
                }

                //actions.single_target+=/earthquake,if=buff.echoes_of_great_sundering.up&(talent.master_of_the_elements.enabled&(buff.master_of_the_elements.up|cooldown.lava_burst.remains>0&maelstrom>=92|spell_targets.chain_lightning<2&buff.stormkeeper.up&cooldown.lava_burst.remains<=gcd)|!talent.master_of_the_elements.enabled|cooldown.elemental_blast.remains<=1.1*gcd*2)
                if (CanCastEarthquake)
                {
                    if (BuffEchoesofGreatSunderingUp && (TalentMasteroftheElements && (BuffMasteroftheElementsUp || CDLavaBurstRemains > 0 && Maelstrom >= 92 || EnemiesNearTarget < 2 && BuffStormkeeperUp && CDLavaBurstRemains <= GCDMAX) || !TalentMasteroftheElements || CDElementalBlastRemains - 500 <= GCDMAX * 2))
                        return Earthquake();
                }

                //actions.single_target+=/earthquake,if=spell_targets.chain_lightning>1&!dot.flame_shock.refreshable&!runeforge.echoes_of_great_sundering.equipped&(!talent.master_of_the_elements.enabled|buff.master_of_the_elements.up|cooldown.lava_burst.remains>0&maelstrom>=92)
                if (CanCastEarthquake)
                {
                    if (EnemiesNearTarget > 1 && !DotFlameShockRefreshable && !RuneforgeEchoesofGreatSundering && (!TalentMasteroftheElements || BuffMasteroftheElementsUp || CDLavaBurstRemains > 0 && Maelstrom >= 92))
                        return Earthquake();
                }

                //actions.single_target+=/lava_burst,if=cooldown_react&(!buff.master_of_the_elements.up&buff.icefury.up)
                if (CanCastLavaBurst)
                {
                    if (!BuffMasteroftheElementsUp && BuffIcefuryUp)
                        return LavaBurst();
                }

                //actions.single_target+=/lava_burst,if=cooldown_react&charges>talent.echo_of_the_elements.enabled&!buff.icefury.up
                if (CanCastLavaBurst)
                {
                    if (CDLavaBurstFullRecharge < GCD && !BuffIcefuryUp)
                        return LavaBurst();
                }

                //actions.single_target+=/lava_burst,if=talent.echo_of_the_elements.enabled&!buff.master_of_the_elements.up&maelstrom>=50&!buff.echoes_of_great_sundering.up
                if (CanCastLavaBurst)
                {
                    if (TalentEchooftheElements && !BuffMasteroftheElementsUp && Maelstrom >= 50 && !BuffEchoesofGreatSunderingUp)
                        return LavaBurst();
                }

                //actions.single_target+=/earth_shock,if=(runeforge.echoes_of_great_sundering.equipped|spell_targets.chain_lightning<2)&(talent.master_of_the_elements.enabled&!buff.echoes_of_great_sundering.up&(buff.master_of_the_elements.up|maelstrom>=92|spell_targets.chain_lightning<2&buff.stormkeeper.up&cooldown.lava_burst.remains<=gcd)|!talent.master_of_the_elements.enabled|cooldown.elemental_blast.remains<=1.1*gcd*2)
                if (CanCastEarthShock)
                {
                    if ((RuneforgeEchoesofGreatSundering || EnemiesNearTarget < 2) && (TalentMasteroftheElements && !BuffEchoesofGreatSunderingUp && (BuffMasteroftheElementsUp || Maelstrom >= 92 || EnemiesNearTarget < 2 && BuffStormkeeperUp && CDLavaBurstRemains <= GCDMAX) || !TalentMasteroftheElements || CDElementalBlastRemains - 500 <= GCDMAX * 2))
                        return EarthShock();
                }

                //actions.single_target+=/frost_shock,if=talent.icefury.enabled&talent.master_of_the_elements.enabled&buff.icefury.up&buff.master_of_the_elements.up
                if (CanCastFrostShock)
                {
                    if (TalentIcefury && TalentMasteroftheElements && BuffIcefuryUp && BuffMasteroftheElementsUp)
                        return FrostShock();
                }

                //actions.single_target+=/lava_burst,if=buff.ascendance.up
                if (CanCastLavaBurst)
                {
                    if (BuffAscendanceUp)
                        return LavaBurst();
                }

                //actions.single_target+=/lava_burst,if=cooldown_react&!talent.master_of_the_elements.enabled
                if (CanCastLavaBurst)
                {
                    if (!TalentMasteroftheElements)
                        return LavaBurst();
                }

                //actions.single_target+=/icefury,if=talent.icefury.enabled&!(maelstrom>35&cooldown.lava_burst.remains<=0)
                if (CanCastIcefury)
                {
                    if (!(Maelstrom > 35 && CDLavaBurstRemains <= 0))
                        return Icefury();
                }

                //actions.single_target+=/frost_shock,if=talent.icefury.enabled&buff.icefury.up&(buff.icefury.remains<gcd*4*buff.icefury.stack|buff.stormkeeper.up|!talent.master_of_the_elements.enabled)
                if (CanCastFrostShock)
                {
                    if (TalentIcefury && BuffIcefuryUp && (BuffIcefuryRemains - 500 < GCDMAX * 4 * BuffIcefuryStacks || BuffStormkeeperUp || !TalentMasteroftheElements))
                        return FrostShock();
                }

                //actions.single_target+=/lava_burst
                if (CanCastLavaBurst)
                {
                    return LavaBurst();
                }

                //actions.single_target+=/flame_shock,target_if=refreshable
                if (CanCastFlameShock)
                {
                    if (DotFlameShockRefreshable)
                        return FlameShock();
                }

                //actions.single_target+=/frost_shock,if=runeforge.elemental_equilibrium.equipped&!buff.elemental_equilibrium_debuff.up&!talent.elemental_blast.enabled&!talent.echoing_shock.enabled
                if (CanCastFrostShock)
                {
                    if (RuneforgeElementalEquilibrium && !DebuffElementalEquilibriumUp && !TalentElementalBlast && !TalentEchoingShock)
                        return FrostShock();
                }

                //actions.single_target+=/chain_harvest
                if (CanCastChainHarvest)
                    return ChainHarvest();

                //actions.single_target+=/static_discharge,if=talent.static_discharge.enabled
                if (CanCastStaticDischarge)
                    return StaticDischarge();

                //actions.single_target+=/chain_lightning,if=spell_targets.chain_lightning>1
                if (CanCastChainLightning)
                {
                    if (EnemiesNearTarget > 1)
                        return ChainLightning();
                }

                //actions.single_target+=/lightning_bolt
                if (CanCastLightningBolt)
                {
                    return LightningBolt();
                }

                //actions.single_target+=/flame_shock,moving=1,target_if=refreshable
                if (CanCastFlameShock && Moving && DotFlameShockRefreshable)
                    return FlameShock();

                //actions.single_target+=/frost_shock,moving=1
                if (CanCastFrostShock && Moving)
                    return FrostShock();

                //actions.single_target+=/frost_shock,if=talent.icefury.enabled&buff.icefury.up&buff.icefury.remains<1.1*gcd*buff.icefury.stack
                if (CanCastFrostShock)
                {
                    if (TalentIcefury && BuffIcefuryUp && BuffIcefuryRemains - 500 < GCDMAX * BuffIcefuryStacks)
                        return FrostShock();
                }
            }

            //actions+=/run_action_list,name=se_single_target,if=talent.storm_elemental.enabled&active_enemies<=2
            if (TalentStormElemental && EnemiesNearTarget<=2)
            {
                //actions.se_single_target=storm_elemental
                if (CanCastStormElemental)
                    return StormElemental();

                //actions.se_single_target+=/frost_shock,if=talent.icefury.enabled&buff.icefury.up&buff.icefury.remains<1.1*gcd*buff.icefury.stack&buff.wind_gust.stack<18
                if (CanCastFrostShock)
                {
                    if (TalentIcefury && BuffIcefuryUp && BuffIcefuryRemains - 500 < GCDMAX * BuffIcefuryStacks && BuffWindGustStacks < 18)
                        return FrostShock();
                }

                //actions.se_single_target+=/elemental_blast,if=talent.elemental_blast.enabled
                if (CanCastElementalBlast)
                    return ElementalBlast();

                //actions.se_single_target+=/stormkeeper,if=talent.stormkeeper.enabled
                if (CanCastStormkeeper)
                    return Stormkeeper();

                //actions.se_single_target+=/echoing_shock,if=talent.echoing_shock.enabled&cooldown.lava_burst.remains<=gcd&spell_targets.chain_lightning<2|maelstrom>=60&spell_targets.chain_lightning>=2&(!runeforge.echoes_of_great_sundering.equipped|buff.echoes_of_great_sundering.up)|spell_targets.chain_lightning<2&buff.wind_gust.stack>=18&(!runeforge.echoes_of_great_sundering.equipped|buff.echoes_of_great_sundering.up)&maelstrom>=60
                if (CanCastEchoingShock)
                {
                    if (TalentEchoingShock && CDLavaBurstRemains <= GCDMAX && EnemiesNearTarget < 2 || Maelstrom >= 60 && EnemiesNearTarget >= 2 && (!RuneforgeEchoesofGreatSundering || BuffEchoesofGreatSunderingUp) || EnemiesNearTarget < 2 && BuffWindGustStacks >= 18 && (!RuneforgeEchoesofGreatSundering || BuffEchoesofGreatSunderingUp) && Maelstrom >= 60)
                        return EchoingShock();
                }

                //actions.se_single_target+=/lava_burst,if=(buff.wind_gust.stack<18&!buff.bloodlust.up)|buff.lava_surge.up
                if (CanCastLavaBurst)
                {
                    if ((BuffWindGustStacks < 18 && !BloodlustUp) || BuffLavaSurgeUp)
                        return LavaBurst();
                }

                //actions.se_single_target+=/lava_burst,if=talent.echoing_shock.enabled&buff.echoing_shock.up&spell_targets.chain_lightning<2
                if (CanCastLavaBurst)
                {
                    if (TalentEchoingShock && BuffEchoingShockUp && EnemiesNearTarget < 2)
                        return LavaBurst();
                }

                //actions.se_single_target+=/earthquake,if=talent.echoing_shock.enabled&buff.echoing_shock.up&spell_targets.chain_lightning>=2
                if (CanCastEarthquake)
                {
                    if (TalentEchoingShock && BuffEchoingShockUp && EnemiesNearTarget >= 2)
                        return Earthquake();
                }

                //actions.se_single_target+=/lightning_bolt,if=buff.stormkeeper.up
                if (CanCastLightningBolt && BuffStormkeeperUp)
                    return LightningBolt();

                //actions.se_single_target+=/earthquake,if=buff.echoes_of_great_sundering.up
                if (CanCastEarthquake && BuffEchoesofGreatSunderingUp)
                    return Earthquake();

                //actions.se_single_target+=/earth_shock,if=spell_targets.chain_lightning<2&maelstrom>=60&(buff.wind_gust.stack<20|maelstrom>90)|(runeforge.echoes_of_great_sundering.equipped&!buff.echoes_of_great_sundering.up)
                if (CanCastEarthShock)
                {
                    if (EnemiesNearTarget < 2 && Maelstrom >= 60 && (BuffWindGustStacks < 20 || Maelstrom > 90) || (RuneforgeEchoesofGreatSundering && !BuffEchoesofGreatSunderingUp))
                        return EarthShock();
                }

                //actions.se_single_target+=/earthquake,if=(spell_targets.chain_lightning>1)&(!dot.flame_shock.refreshable)
                if (CanCastEarthquake)
                {
                    if ((EnemiesNearTarget > 1) && (!DotFlameShockRefreshable))
                        return Earthquake();
                }

                //actions.se_single_target+=/chain_lightning,if=active_enemies>1&pet.storm_elemental.active&buff.bloodlust.up
                if (CanCastChainLightning)
                {
                    if (EnemiesNearTarget > 1 && PetStormElementalActive && BloodlustUp)
                        return ChainLightning();
                }

                //actions.se_single_target+=/lightning_bolt,if=pet.storm_elemental.active&buff.bloodlust.up
                if (CanCastLightningBolt)
                {
                    if (PetStormElementalActive && BloodlustUp)
                        return LightningBolt();
                }

                //actions.se_single_target+=/lava_burst,if=buff.ascendance.up
                if (CanCastLavaBurst && BuffAscendanceUp)
                    return LavaBurst();

                //actions.se_single_target+=/lava_burst,if=cooldown_react
                if (CanCastLavaBurst)
                    return LavaBurst();

                //actions.se_single_target+=/frost_shock,if=talent.icefury.enabled&buff.icefury.up
                if (CanCastFrostShock && TalentIcefury && BuffIcefuryUp)
                    return FrostShock();

                //actions.single_target+=/chain_harvest
                if (CanCastChainHarvest)
                    return ChainHarvest();

                //actions.single_target+=/static_discharge,if=talent.static_discharge.enabled
                if (CanCastStaticDischarge)
                    return StaticDischarge();

                //actions.single_target+=/chain_lightning,if=spell_targets.chain_lightning>1
                if (CanCastChainLightning)
                {
                    if (EnemiesNearTarget > 1)
                        return ChainLightning();
                }

                //actions.single_target+=/lightning_bolt
                if (CanCastLightningBolt)
                {
                    return LightningBolt();
                }

                //actions.single_target+=/flame_shock,moving=1,target_if=refreshable
                if (CanCastFlameShock && Moving && DotFlameShockRefreshable)
                    return FlameShock();

                //actions.single_target+=/frost_shock,moving=1
                if (CanCastFrostShock && Moving)
                    return FrostShock();

                //actions.single_target+=/frost_shock,if=talent.icefury.enabled&buff.icefury.up&buff.icefury.remains<1.1*gcd*buff.icefury.stack
                if (CanCastFrostShock)
                {
                    if (TalentIcefury && BuffIcefuryUp && BuffIcefuryRemains - 500 < GCDMAX * BuffIcefuryStacks)
                        return FrostShock();
                }
            }


            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

        bool FlameShock() { Aimsharp.Cast("Flame Shock"); return true; }
        bool PrimordialWave() { Aimsharp.Cast("Primordial Wave"); return true; }
        bool FireElemental() { Aimsharp.Cast("Fire Elemental"); return true; }
        bool VesperTotem() { Aimsharp.Cast("VesperTotemC"); return true; }
        bool FaeTransfusion() { Aimsharp.Cast("FaeTransfusionC"); return true; }
        bool Earthquake() { Aimsharp.Cast("EarthquakeC"); return true; }
        bool ChainHarvest() { Aimsharp.Cast("Chain Harvest"); return true; }
        bool Stormkeeper() { Aimsharp.Cast("Stormkeeper"); return true; }
        bool EchoingShock() { Aimsharp.Cast("Echoing Shock"); return true; }
        bool Ascendance() { Aimsharp.Cast("Ascendance"); return true; }
        bool LiquidMagmaTotem() { Aimsharp.Cast("LiquidMagmaTotemC"); return true; }
        bool ChainLightning() { Aimsharp.Cast("Chain Lightning"); return true; }
        bool EarthShock() { Aimsharp.Cast("Earth Shock"); return true; }
        bool LavaBurst() { Aimsharp.Cast("Lava Burst"); return true; }
        bool ElementalBlast() { Aimsharp.Cast("Elemental Blast"); return true; }
        bool LavaBeam() { Aimsharp.Cast("Lava Beam"); return true; }
        bool FrostShock() { Aimsharp.Cast("Frost Shock"); return true; }
        bool LightningBolt() { Aimsharp.Cast("Lightning Bolt"); return true; }
        bool StaticDischarge() { Aimsharp.Cast("Static Discharge"); return true; }
        bool EarthElemental() { Aimsharp.Cast("Earth Elemental"); return true; }
        bool Icefury() { Aimsharp.Cast("Icefury"); return true; }
        bool StormElemental() { Aimsharp.Cast("Storm Elemental"); return true; }

    }
}
