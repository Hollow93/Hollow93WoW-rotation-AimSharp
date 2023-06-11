using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class SLEnhancementShaman : Rotation
    {
        List<string> Racials = new List<string>
        {
            "Blood Fury","Berserking","Fireblood","Ancestral Call","Bag of Tricks"
        };

        List<string> CovenantAbilities = new List<string>
        {
            "Primordial Wave","Chain Harvest"
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

        };

        List<string> GeneralDebuffs = new List<string>
        {

        };

        List<string> SpellsList = new List<string>
        {
            "Windstrike","Feral Spirit","Ascendance","Stormkeeper","Flame Shock","Frost Shock","Earthen Spike","Lightning Bolt","Elemental Blast","Lava Lash",
            "Stormstrike","Crash Lightning","Ice Strike","Sundering","Fire Nova","Windfury Totem","Chain Lightning","Windfury Totem"
        };

        List<string> BuffsList = new List<string>
        {
            "Ascendance","Stormkeeper","Hailstorm","Maelstrom Weapon","Hot Hand","Windfury Totem","Primordial Wave"
        };

        List<string> DebuffsList = new List<string>
        {
            "Flame Shock"
        };

        List<string> TotemsList = new List<string>
        {
            "Windfury Totem","Vesper Totem"
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
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None" }, "None"));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Enhancement Shaman", Color.Purple);
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

            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            CustomFunctions.Add("FlameShockCount", "local FlameShockCount = 0\nfor i=1,20 do\nlocal unit = \"nameplate\" .. i\nif UnitExists(unit) then\nif UnitCanAttack(\"player\", unit) then\nfor j = 1, 40 do\nlocal name,_,_,_,_,_,source = UnitDebuff(unit, j)\nif name == \"Flame Shock\" and source == \"player\" then\nFlameShockCount = FlameShockCount + 1\nend\nend\nend\nend\nend\nreturn FlameShockCount");

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
            bool Fighting = Aimsharp.Range("target") <= 40 && Aimsharp.TargetIsEnemy();
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();

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


            // General Debuffs


            // General Cooldowns
            int ConcentratedFlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            int CDPrimordialWaveRemaining = Aimsharp.SpellCooldown("Primordial Wave");
            bool CDPrimordialWaveUp = CDPrimordialWaveRemaining <= 0;

            // Shaman power
            //int Maelstrom = Aimsharp.Power("player");

            // Shaman Talents
            bool TalentAscendance = Aimsharp.Talent(7, 3);
            bool TalentStormkeeper = Aimsharp.Talent(6, 2);
            bool TalentFireNova = Aimsharp.Talent(4, 3);

            // Shaman buffs
            int BuffAscendanceRemaining = Aimsharp.BuffRemaining("Ascendance") - GCD;
            bool BuffAscendanceUp = BuffAscendanceRemaining > 0;
            int BuffStormkeeperRemaining = Aimsharp.BuffRemaining("Stormkeeper") - GCD;
            bool BuffStormkeeperUp = BuffStormkeeperRemaining > 0;
            int BuffHailstormRemaining = Aimsharp.BuffRemaining("Hailstorm") - GCD;
            bool BuffHailstormUp = BuffHailstormRemaining > 0;
            int BuffMaelstromWeaponStacks = Aimsharp.BuffStacks("Maelstrom Weapon");
            int BuffHotHandRemaining = Aimsharp.BuffRemaining("Hot Hand") - GCD;
            bool BuffHotHandUp = BuffHotHandRemaining > 0;
            bool BuffWindfuryTotemUp = Aimsharp.HasBuff("Windfury Totem", "player", false);
            int BuffPrimordialWaveRemaining = Aimsharp.BuffRemaining("Primordial Wave") - GCD;
            bool BuffPrimordialWaveUp = BuffPrimordialWaveRemaining > 0;

            // Shaman debuffs
            int DotFlameShockRemaining = Aimsharp.DebuffRemaining("Flame Shock") - GCD;
            bool DotFlameShockTicking = DotFlameShockRemaining > 0;
            bool DotFlameShockRefreshable = DotFlameShockRemaining <= 5400;

            // Shaman cooldowns
            int CDAscendanceRemaining = Aimsharp.SpellCooldown("Ascendance");
            bool CDAscendanceUp = CDAscendanceRemaining <= 0;

            // Shaman specific variables
            int TotemWindfuryTotemRemaining = Aimsharp.TotemRemaining("Windfury Totem");
            int FlameShockCount = Aimsharp.CustomFunction("FlameShockCount");

            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions=bloodlust
            //rotation does not auto bloodlust

            //actions+=/potion,if=expected_combat_length-time<60
            if (BloodlustUp || TargetTimeToDie <= 60000 || TargetHealthPct < 35)
            {
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false))
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }


            //actions+=/wind_shear
            //rotation does not auto interrupt

            //actions+=/auto_attack
            //actions+=/windstrike
            if (Aimsharp.CanCast("Windstrike") && Fighting)
            {
                if (BuffAscendanceUp)
                {
                    Aimsharp.Cast("Windstrike");
                    return true;
                }
            }



            if (!SaveCooldowns)
            {
                //actions+=/heart_essence
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

                //actions.cds+=/use_racials,if=!talent.ascendance.enabled|buff.ascendance.up|cooldown.ascendance.remains>50
                if (BuffAscendanceUp || !TalentAscendance || CDAscendanceRemaining > 50000)
                    foreach (string Racial in Racials)
                    {
                        if (Aimsharp.CanCast(Racial, "player") && Fighting)
                        {
                            Aimsharp.Cast(Racial, true);
                            return true;
                        }
                    }

                //actions+=/feral_spirit
                if (Aimsharp.CanCast("Feral Spirit", "player") && Fighting)
                {
                    Aimsharp.Cast("Feral Spirit");
                    return true;
                }

                //actions+=/ascendance
                if (Aimsharp.CanCast("Ascendance", "player") && Fighting)
                {
                    Aimsharp.Cast("Ascendance");
                    return true;
                }
            }

            //actions+=/call_action_list,name=single,if=active_enemies=1
            if (EnemiesInMelee <= 1)
            {
                //actions.single=primordial_wave,if=!buff.primordial_wave.up&(!talent.stormkeeper.enabled|buff.stormkeeper.up)
                if (Aimsharp.CanCast("Primordial Wave") && Fighting)
                {
                    if (!BuffPrimordialWaveUp && (!TalentStormkeeper || BuffStormkeeperUp))
                    {
                        Aimsharp.Cast("Primordial Wave");
                        return true;
                    }
                }

                //actions.single+=/flame_shock,if=!ticking
                if (Aimsharp.CanCast("Flame Shock") && Fighting)
                {
                    if (!DotFlameShockTicking)
                    {
                        Aimsharp.Cast("Flame Shock");
                        return true;
                    }
                }

                //actions.single+=/vesper_totem
                //not implemented

                //actions.single+=/frost_shock,if=buff.hailstorm.up
                if (Aimsharp.CanCast("Frost Shock") && Fighting)
                {
                    if (BuffHailstormUp)
                    {
                        Aimsharp.Cast("Frost Shock");
                        return true;
                    }
                }

                //actions.single+=/earthen_spike
                if (Aimsharp.CanCast("Earthen Spike") && Fighting)
                {
                    Aimsharp.Cast("Earthen Spike");
                    return true;
                }

                //actions.single+=/fae_transfusion
                //not implemented

                //actions.single+=/lightning_bolt,if=buff.stormkeeper.up&buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Lightning Bolt") && Fighting)
                {
                    if (BuffStormkeeperUp && BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Lightning Bolt");
                        return true;
                    }
                }

                //actions.single+=/elemental_blast,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Elemental Blast") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Elemental Blast");
                        return true;
                    }
                }

                //actions.single+=/chain_harvest,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Chain Harvest") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Chain Harvest");
                        return true;
                    }
                }

                //actions.single+=/lightning_bolt,if=buff.maelstrom_weapon.stack=10
                if (Aimsharp.CanCast("Lightning Bolt") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks == 10)
                    {
                        Aimsharp.Cast("Lightning Bolt");
                        return true;
                    }
                }

                //actions.single+=/lava_lash,if=buff.hot_hand.up
                if (Aimsharp.CanCast("Lava Lash") && Fighting)
                {
                    if (BuffHotHandUp)
                    {
                        Aimsharp.Cast("Lava Lash");
                        return true;
                    }
                }

                //actions.single+=/stormstrike
                if (Aimsharp.CanCast("Stormstrike") && Fighting)
                {
                    Aimsharp.Cast("Stormstrike");
                    return true;
                }

                //actions.single+=/stormkeeper,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Stormkeeper", "player") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Stormkeeper");
                        return true;
                    }
                }

                //actions.single+=/lava_lash
                if (Aimsharp.CanCast("Lava Lash") && Fighting)
                {
                    Aimsharp.Cast("Lava Lash");
                    return true;
                }

                //actions.single+=/crash_lightning
                if (Aimsharp.CanCast("Crash Lightning", "player") && Fighting)
                {
                    Aimsharp.Cast("Crash Lightning");
                    return true;
                }

                //actions.single+=/flame_shock,target_if=refreshable
                if (Aimsharp.CanCast("Flame Shock") && Fighting)
                {
                    if (DotFlameShockRefreshable)
                    {
                        Aimsharp.Cast("Flame Shock");
                        return true;
                    }
                }

                //actions.single+=/frost_shock
                if (Aimsharp.CanCast("Frost Shock") && Fighting)
                {
                    Aimsharp.Cast("Frost Shock");
                    return true;
                }

                //actions.single+=/ice_strike
                if (Aimsharp.CanCast("Ice Strike") && Fighting)
                {
                    Aimsharp.Cast("Ice Strike");
                    return true;
                }

                //actions.single+=/sundering
                if (Aimsharp.CanCast("Sundering","player") && Fighting)
                {
                    Aimsharp.Cast("Sundering");
                    return true;
                }

                //actions.single+=/fire_nova,if=active_dot.flame_shock
                if (Aimsharp.CanCast("Fire Nova","player") && Fighting)
                {
                    if (DotFlameShockTicking)
                    {
                        Aimsharp.Cast("Fire Nova");
                        return true;
                    }
                }

                //actions.single+=/lightning_bolt,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Lightning Bolt") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Lightning Bolt");
                        return true;
                    }
                }

                //actions.single+=/earth_elemental
                //not implemented

                //actions.single+=/windfury_totem,if=buff.windfury_totem.remains<30
                if (Aimsharp.CanCast("Windfury Totem","player"))
                {
                    if (!BuffWindfuryTotemUp || TotemWindfuryTotemRemaining < 30000)
                    {
                        Aimsharp.Cast("Windfury Totem");
                        return true;
                    }
                }
            }

            //actions+=/call_action_list,name=aoe,if=active_enemies>1
            if (EnemiesInMelee > 1)
            {
                //actions.aoe=frost_shock,if=buff.hailstorm.up
                if (Aimsharp.CanCast("Frost Shock") && Fighting)
                {
                    if (BuffHailstormUp)
                    {
                        Aimsharp.Cast("Frost Shock");
                        return true;
                    }
                }

                //actions.aoe+=/fire_nova,if=active_dot.flame_shock>=3
                if (Aimsharp.CanCast("Fire Nova", "player") && Fighting)
                {
                    if (FlameShockCount >= 3)
                    {
                        Aimsharp.Cast("Fire Nova");
                        return true;
                    }
                }

                //actions.aoe+=/flame_shock,target_if=refreshable,cycle_targets=1,if=talent.fire_nova.enabled|covenant.necrolord
                //tab target not implemented
                if (Aimsharp.CanCast("Flame Shock") && Fighting)
                {
                    if (DotFlameShockRefreshable)
                    {
                        Aimsharp.Cast("Flame Shock");
                        return true;
                    }
                }

                //actions.aoe+=/primordial_wave,target_if=min:dot.flame_shock.remains,cycle_targets=1,if=!buff.primordial_wave.up&(!talent.stormkeeper.enabled|buff.stormkeeper.up)
                if (Aimsharp.CanCast("Primordial Wave") && Fighting)
                {
                    if (!BuffPrimordialWaveUp && (!TalentStormkeeper || BuffStormkeeperUp))
                    {
                        Aimsharp.Cast("Primordial Wave");
                        return true;
                    }
                }

                //actions.aoe+=/vesper_totem
                //not implemented

                //actions.aoe+=/lightning_bolt,if=buff.primordial_wave.up&buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Lightning Bolt") && Fighting)
                {
                    if (BuffPrimordialWaveUp && BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Lightning Bolt");
                        return true;
                    }
                }

                //actions.aoe+=/crash_lightning
                if (Aimsharp.CanCast("Crash Lightning", "player") && Fighting)
                {
                    Aimsharp.Cast("Crash Lightning");
                    return true;
                }

                //actions.aoe+=/chain_lightning,if=buff.stormkeeper.up&buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Chain Lightning") && Fighting)
                {
                    if (BuffStormkeeperUp && BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Chain Lightning");
                        return true;
                    }
                }

                //actions.aoe+=/chain_harvest,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Chain Harvest") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Chain Harvest");
                        return true;
                    }
                }

                //actions.aoe+=/elemental_blast,if=buff.maelstrom_weapon.stack>=5&active_enemies!=3
                if (Aimsharp.CanCast("Elemental Blast") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5 && EnemiesInMelee != 3)
                    {
                        Aimsharp.Cast("Elemental Blast");
                        return true;
                    }
                }

                //actions.aoe+=/stormkeeper,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Stormkeeper", "player") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Stormkeeper");
                        return true;
                    }
                }

                //actions.aoe+=/chain_lightning,if=buff.maelstrom_weapon.stack=10
                if (Aimsharp.CanCast("Chain Lightning") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks == 10)
                    {
                        Aimsharp.Cast("Chain Lightning");
                        return true;
                    }
                }

                //actions.aoe+=/sundering
                if (Aimsharp.CanCast("Sundering", "player") && Fighting)
                {
                    Aimsharp.Cast("Sundering");
                    return true;
                }

                //actions.aoe+=/stormstrike
                if (Aimsharp.CanCast("Stormstrike") && Fighting)
                {
                    Aimsharp.Cast("Stormstrike");
                    return true;
                }

                //actions.aoe+=/lava_lash
                if (Aimsharp.CanCast("Lava Lash") && Fighting)
                {
                    Aimsharp.Cast("Lava Lash");
                    return true;
                }

                //actions.aoe+=/elemental_blast,if=buff.maelstrom_weapon.stack>=5&active_enemies=3
                if (Aimsharp.CanCast("Elemental Blast") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5 && EnemiesInMelee == 3)
                    {
                        Aimsharp.Cast("Elemental Blast");
                        return true;
                    }
                }

                //actions.aoe+=/fae_transfusion
                //not implemented

                //actions.aoe+=/frost_shock
                if (Aimsharp.CanCast("Frost Shock") && Fighting)
                {
                    Aimsharp.Cast("Frost Shock");
                    return true;
                }

                //actions.aoe+=/ice_strike
                if (Aimsharp.CanCast("Ice Strike") && Fighting)
                {
                    Aimsharp.Cast("Ice Strike");
                    return true;
                }

                //actions.aoe+=/chain_lightning,if=buff.maelstrom_weapon.stack>=5
                if (Aimsharp.CanCast("Chain Lightning") && Fighting)
                {
                    if (BuffMaelstromWeaponStacks >= 5)
                    {
                        Aimsharp.Cast("Chain Lightning");
                        return true;
                    }
                }

                //actions.aoe+=/fire_nova,if=active_dot.flame_shock>1
                if (Aimsharp.CanCast("Fire Nova", "player") && Fighting)
                {
                    if (FlameShockCount > 1)
                    {
                        Aimsharp.Cast("Fire Nova");
                        return true;
                    }
                }

                //actions.aoe+=/earthen_spike
                if (Aimsharp.CanCast("Earthen Spike") && Fighting)
                {
                    Aimsharp.Cast("Earthen Spike");
                    return true;
                }

                //actions.aoe+=/windfury_totem,if=buff.windfury_totem.remains<30
                if (Aimsharp.CanCast("Windfury Totem", "player"))
                {
                    if (!BuffWindfuryTotemUp || TotemWindfuryTotemRemaining < 30000)
                    {
                        Aimsharp.Cast("Windfury Totem");
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
