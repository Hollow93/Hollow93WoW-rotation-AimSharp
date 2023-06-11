using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsUnholy : Rotation
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
           "Arcane Torrent","Light's Judgment","Arcane Pulse","Outbreak","Epidemic","Scourge Strike","Clawing Shadows","Death and Decay","Death's Due","Defile","Festering Strike","Army of the Dead","Unholy Blight","Dark Transformation","Apocalypse","Summon Gargoyle","Unholy Assault","Soul Reaper","Raise Dead","Swarming Mist","Abomination Limb","Shackle the Unworthy","Death Coil",
        };

        List<string> BuffsList = new List<string>
        {
            "Unholy Assault","Unholy Strength","Swarming Mist","Dark Transformation","Runic Corruption","Sudden Doom","Death and Decay","Defile","Death's Due"
        };

        List<string> DebuffsList = new List<string>
        {
           "Festering Wound","Virulent Plague","Frost Fever","Blood Plague","Unholy Blight",
        };

        List<string> TotemsList = new List<string>
        {
            "Ebon Gargoyle"
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

            Settings.Add(new Setting("DeathKnight Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Superstrain", "Deadliest Coil", "Phearomones", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Unholy", Color.Purple);
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

            Macros.Add("DeathandDecayC", "/cast [@cursor] Death and Decay");
            Macros.Add("DeathsDueC", "/cast [@cursor] Death's Due");
            Macros.Add("DefileC", "/cast [@cursor] Defile");


            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            CustomFunctions.Add("FWoundCount", "local FlameShockCount = 0\nfor i=1,20 do\nlocal unit = \"nameplate\" .. i\nif UnitExists(unit) then\nif UnitCanAttack(\"player\", unit) then\nfor j = 1, 40 do\nlocal name,_,_,_,_,_,source = UnitDebuff(unit, j)\nif name == \"Festering Wound\" and source == \"player\" then\nFlameShockCount = FlameShockCount + 1\nend\nend\nend\nend\nend\nreturn FlameShockCount");

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
            bool Fighting = Aimsharp.Range("target") <= 30 && Aimsharp.TargetIsEnemy();
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
            int RunicPower = Aimsharp.Power("player");
            int Runes = Aimsharp.PlayerSecondaryPower();
            int MaxRunicPower = Aimsharp.PlayerMaxPower();
            int RunicDefecit = MaxRunicPower - RunicPower;

            //Talents
            bool TalentSummonGargoyle = Aimsharp.Talent(7, 2);
            bool TalentArmyoftheDamned = Aimsharp.Talent(7, 1);
            bool TalentUnholyBlight = Aimsharp.Talent(2, 3);
            bool TalentDefile = Aimsharp.Talent(6, 3);
            bool TalentUnholyPact = Aimsharp.Talent(6, 2);
            bool TalentClawingShadows = Aimsharp.Talent(1, 3);


            //buffs
            int BuffUnholyAssaultRemains = Aimsharp.BuffRemaining("Unholy Assault") - GCD;
            bool BuffUnholyAssaultUp = BuffUnholyAssaultRemains > 0;
            int BuffUnholyStrengthRemains = Aimsharp.BuffRemaining("Unholy Strength") - GCD;
            bool BuffUnholyStrengthUp = BuffUnholyStrengthRemains > 0;
            int BuffSwarmingMistRemains = Aimsharp.BuffRemaining("Swarming Mist") - GCD;
            bool BuffSwarmingMistUp = BuffSwarmingMistRemains > 0;
            int BuffDarkTransformationRemains = Aimsharp.BuffRemaining("Dark Transformation") - GCD;
            bool BuffDarkTransformationUp = BuffDarkTransformationRemains > 0;
            int BuffRunicCorruptionRemains = Aimsharp.BuffRemaining("Runic Corruption") - GCD;
            bool BuffRunicCorruptionUp = BuffRunicCorruptionRemains > 0;
            int BuffSuddenDoomRemains = Aimsharp.BuffRemaining("Sudden Doom") - GCD;
            bool BuffSuddenDoomUp = BuffSuddenDoomRemains > 0;


            //debuffs
            int DebuffFesteringWoundRemains = Aimsharp.DebuffRemaining("Festering Wound") - GCD;
            bool DebuffFesteringWoundUp = DebuffFesteringWoundRemains > 0;
            int DebuffFesteringWoundStacks = Aimsharp.DebuffStacks("Festering Wound");
            int DotVirulentPlagueRemains = Aimsharp.DebuffRemaining("Virulent Plague") - GCD;
            bool DotVirulentPlagueUp = DotVirulentPlagueRemains > 0;
            bool DotVirulentPlagueRefreshable = DotVirulentPlagueRemains < 8100;
            int DotFrostFeverRemains = Aimsharp.DebuffRemaining("Frost Fever") - GCD;
            bool DotFrostFeverUp = DotFrostFeverRemains > 0;
            bool DotFrostFeverRefreshable = DotFrostFeverRemains < 7200;
            int DotBloodPlagueRemains = Aimsharp.DebuffRemaining("Blood Plague") - GCD;
            bool DotBloodPlagueUp = DotBloodPlagueRemains > 0;
            bool DotBloodPlagueRefreshable = DotBloodPlagueRemains < 7200;
            int DotUnholyBlightRemains = Aimsharp.DebuffRemaining("Unholy Blight") - GCD;
            bool DotUnholyBlightUp = DotUnholyBlightRemains > 0;


            //cooldowns
            int CDArmyoftheDeadRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Army of the Dead");
            bool CDArmyoftheDeadUp = CDArmyoftheDeadRemains <= 0;
            int CDUnholyBlightRemains = Aimsharp.SpellCooldown("Unholy Blight");
            bool CDUnholyBlightUp = CDUnholyBlightRemains <= 0;
            int CDApocalypseRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Apocalypse");
            bool CDApocalypseUp = CDApocalypseRemains <= 0;
            int CDSummonGargoyleRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Summon Gargoyle");
            bool CDSummonGargoyleUp = CDSummonGargoyleRemains <= 0;
            int CDDeathandDecayRemains = Aimsharp.SpellCooldown("Death and Decay");
            bool CDDeathandDecayUp = CDDeathandDecayRemains <= 0;
            int CDDefileRemains = Aimsharp.SpellCooldown("Defile");
            bool CDDefileUp = CDDefileRemains <= 0;


            //specific variables
            bool RuneforgeSuperstrain = RuneforgePower == "Superstrain";
            bool RuneforgeDeadliestCoil = RuneforgePower == "Deadliest Coil";
            bool RuneforgePhearomones = RuneforgePower == "Phearomones";
            bool ConduitConvocationoftheDead = ActiveConduits.Contains(338553);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");

            bool PetGhoulActive = Aimsharp.PlayerHasPet();
            //CaNCasts
            bool CanCastArcaneTorrent = Aimsharp.CanCast("Arcane Torrent", "player") && !SaveCooldowns && Fighting;
            bool CanCastLightsJudgment = Aimsharp.CanCast("Light's Judgment", "player") && !SaveCooldowns && Fighting;
            bool CanCastArcanePulse = Aimsharp.CanCast("Arcane Pulse", "player") && !SaveCooldowns && Fighting;
            bool CanCastOutbreak = Aimsharp.CanCast("Outbreak") && Fighting;
            bool CanCastEpidemic = Aimsharp.CanCast("Epidemic", "player") && Fighting;
            bool CanCastScourgeStrike = Aimsharp.CanCast("Scourge Strike") && Fighting && !TalentClawingShadows;
            bool CanCastClawingShadows = Aimsharp.CanCast("Clawing Shadows") && Fighting && TalentClawingShadows;
            bool CanCastDeathandDecay = Aimsharp.CanCast("Death and Decay", "player") && Fighting && !TalentDefile && !CovenantNightFae;
            bool CanCastDeathsDue = Aimsharp.CanCast("Death's Due", "player") && Fighting && CovenantNightFae;
            bool CanCastDefile = Aimsharp.CanCast("Defile", "player") && Fighting && TalentDefile;
            bool CanCastFesteringStrike = Aimsharp.CanCast("Festering Strike") && Fighting;
            bool CanCastArmyoftheDead = Aimsharp.CanCast("Army of the Dead","player") && !SaveCooldowns && Fighting;
            bool CanCastUnholyBlight = Aimsharp.CanCast("Unholy Blight", "player") && Fighting;
            bool CanCastDarkTransformation = Aimsharp.CanCast("Dark Transformation", "player") && Fighting && PetGhoulActive;
            bool CanCastApocalypse = Aimsharp.CanCast("Apocalypse") && !SaveCooldowns && Fighting;
            bool CanCastSummonGargoyle = Aimsharp.CanCast("Summon Gargoyle") && !SaveCooldowns && Fighting;
            bool CanCastUnholyAssault = Aimsharp.CanCast("Unholy Assault") && !SaveCooldowns && Fighting;
            bool CanCastSoulReaper = Aimsharp.CanCast("Soul Reaper") && Fighting;
            bool CanCastRaiseDead = Aimsharp.CanCast("Raise Dead", "player") && Fighting;
            bool CanCastSwarmingMist = Aimsharp.CanCast("Swarming Mist", "player") && !SaveCooldowns && Fighting;
            bool CanCastAbominationLimb = Aimsharp.CanCast("Abomination Limb", "player") && !SaveCooldowns && Fighting;
            bool CanCastShackletheUnworthy = Aimsharp.CanCast("Shackle the Unworthy") && !SaveCooldowns && Fighting;
            bool CanCastDeathCoil = Aimsharp.CanCast("Death Coil") && Fighting;


            //actions+=/variable,name=pooling_for_gargoyle,value=cooldown.summon_gargoyle.remains<5&talent.summon_gargoyle
            bool pooling_for_gargoyle = CDSummonGargoyleRemains < 5000 && TalentSummonGargoyle;
            //actions+=/variable,name=st_planning,value=active_enemies=1&(!raid_event.adds.exists|raid_event.adds.in>15)
            bool st_planning = EnemiesInMelee <= 1;

            bool PetGargoyleActive = Aimsharp.TotemRemaining("Ebon Gargoyle") - GCD > 0;
            bool PetApocGhoulActive = CDApocalypseRemains >= 75000;
            bool PetArmyGhoulActive = CDArmyoftheDeadRemains >= 465000;
            bool DeathandDecayTicking = Aimsharp.HasBuff("Death and Decay") || Aimsharp.HasBuff("Defile") || Aimsharp.HasBuff("Death's Due");
            
            int FWoundTargets = Aimsharp.CustomFunction("FWoundCount");


            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            if (CanCastRaiseDead)
                if (!PetGhoulActive)
                    return RaiseDead();

            //actions+=/arcane_torrent,if=runic_power.deficit>65&(pet.gargoyle.active|!talent.summon_gargoyle.enabled)&rune.deficit>=5
            if (CanCastArcaneTorrent)
            {
                if (RunicDefecit>65 && (PetGargoyleActive || !TalentSummonGargoyle) && Runes<=1)
                {
                    return ArcaneTorrent();
                }
            }

            //actions+=/blood_fury,if=pet.gargoyle.active|buff.unholy_assault.up|talent.army_of_the_damned&pet.apoc_ghoul.active&(pet.army_ghoul.active|cooldown.army_of_the_dead.remains>cooldown.blood_fury.duration%3)|target.time_to_die<=buff.blood_fury.duration
            if (PetGargoyleActive || BuffUnholyAssaultUp || TalentArmyoftheDamned && PetApocGhoulActive && (PetArmyGhoulActive || CDArmyoftheDeadRemains > 30000))
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

            //actions+=/lights_judgment,if=buff.unholy_strength.up
            if (CanCastLightsJudgment)
            {
                if (BuffUnholyStrengthUp)
                {
                    return LightsJudgment();
                }
            }

            //actions+=/arcane_pulse,if=active_enemies>=2|(rune.deficit>=5&runic_power.deficit>=60)
            if (CanCastArcanePulse)
            {
                if (EnemiesInMelee>=2 || (Runes<=1 && RunicDefecit>=60))
                {
                    return ArcanePulse();
                }
            }

            //actions+=/outbreak,if=dot.virulent_plague.refreshable&!talent.unholy_blight&!raid_event.adds.exists
            if (CanCastOutbreak)
            {
                if (DotVirulentPlagueRefreshable && !TalentUnholyBlight && EnemiesInMelee<=1)
                {
                    return Outbreak();
                }
            }

            //actions+=/outbreak,if=dot.virulent_plague.refreshable&(!talent.unholy_blight|talent.unholy_blight&cooldown.unholy_blight.remains)&active_enemies>=2
            if (CanCastOutbreak)
            {
                if (DotVirulentPlagueRefreshable && (!TalentUnholyBlight || TalentUnholyBlight&&CDUnholyBlightRemains>0)&&EnemiesInMelee>=2)
                {
                    return Outbreak();
                }
            }

            //actions+=/outbreak,if=runeforge.superstrain&(dot.frost_fever.refreshable|dot.blood_plague.refreshable)
            if (CanCastOutbreak)
                if (RuneforgeSuperstrain && (DotFrostFeverRefreshable||DotBloodPlagueRefreshable))
                        return Outbreak();

            //actions+=/call_action_list,name=covenants
            //actions.covenants=swarming_mist,if=variable.st_planning&runic_power.deficit>6
            if (CanCastSwarmingMist)
                if (st_planning && RunicDefecit > 6)
                    return SwarmingMist();

            //actions.covenants+=/swarming_mist,if=cooldown.apocalypse.remains&(active_enemies>=2&active_enemies<=5&runic_power.deficit>(active_enemies*6)|active_enemies>5&runic_power.deficit>30)
            if (CanCastSwarmingMist)
                if (CDApocalypseRemains > 0 && (EnemiesInMelee >= 2 && EnemiesInMelee <= 5 && RunicDefecit > (EnemiesInMelee * 6) || EnemiesInMelee > 5 && RunicDefecit > 30))
                    return SwarmingMist();

            //actions.covenants+=/abomination_limb,if=variable.st_planning&rune.time_to_4>(3+buff.runic_corruption.remains)
            if (CanCastAbominationLimb)
                if (st_planning && Aimsharp.TimeUntilRunes(4) > (3000 + BuffRunicCorruptionRemains))
                    return AbominationLimb();

            //actions.covenants+=/abomination_limb,if=active_enemies>=2&rune.time_to_4>(3+buff.runic_corruption.remains)
            if (CanCastAbominationLimb)
                if (EnemiesInMelee >= 2 && Aimsharp.TimeUntilRunes(4) > (3000 + BuffRunicCorruptionRemains))
                    return AbominationLimb();

            //actions.covenants+=/shackle_the_unworthy,if=variable.st_planning&cooldown.apocalypse.remains
            if (CanCastShackletheUnworthy)
                if (st_planning && CDApocalypseRemains > 0)
                    return ShackletheUnworthy();

            //actions.covenants+=/shackle_the_unworthy,if=active_enemies>=2&(death_and_decay.ticking|raid_event.adds.remains<=14)
            if (CanCastShackletheUnworthy)
                if (EnemiesInMelee>=2)
                    return ShackletheUnworthy();

            //actions+=/call_action_list,name=cooldowns
            //actions.cooldowns=potion,if=pet.gargoyle.active|buff.unholy_assault.up|talent.army_of_the_damned&(pet.army_ghoul.active|pet.apoc_ghoul.active|cooldown.army_of_the_dead.remains>target.time_to_die)
            if (PetGargoyleActive||BuffUnholyAssaultUp||TalentArmyoftheDamned&&(PetArmyGhoulActive||PetApocGhoulActive||CDArmyoftheDeadRemains>TargetTimeToDie))
            {
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //trinkets
            if (CDApocalypseRemains > 0)
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

            //actions.cooldowns+=/army_of_the_dead,if=debuff.festering_wound.up&cooldown.unholy_blight.remains<5&talent.unholy_blight|!talent.unholy_blight
            if (CanCastArmyoftheDead)
                if (DebuffFesteringWoundUp && CDUnholyBlightRemains < 5000 && TalentUnholyBlight || !TalentUnholyBlight)
                    return ArmyoftheDead();

            //actions.cooldowns+=/unholy_blight,if=variable.st_planning&(cooldown.army_of_the_dead.remains>5|death_knight.disable_aotd)&(cooldown.apocalypse.ready&(debuff.festering_wound.stack>=4|rune>=3)|cooldown.apocalypse.remains)
            if (CanCastUnholyBlight)
                if (st_planning && (CDArmyoftheDeadRemains > 5000 || !SaveCooldowns) && (CDApocalypseUp && (DebuffFesteringWoundStacks >= 4 || Runes >= 3) || CDApocalypseRemains > 0))
                    return UnholyBlight();

            //actions.cooldowns+=/unholy_blight,if=active_enemies>=2
            if (CanCastUnholyBlight)
                if (EnemiesInMelee >= 2)
                    return UnholyBlight();

            //actions.cooldowns+=/dark_transformation,if=variable.st_planning&cooldown.unholy_blight.remains&(!runeforge.deadliest_coil|runeforge.deadliest_coil&(!buff.dark_transformation.up&!talent.unholy_pact|talent.unholy_pact))
            if (CanCastDarkTransformation)
                if (st_planning && CDUnholyBlightRemains > 0 && (!RuneforgeDeadliestCoil || RuneforgeDeadliestCoil && (!BuffDarkTransformationUp && !TalentUnholyPact || TalentUnholyPact)))
                    return DarkTransformation();

            //actions.cooldowns+=/dark_transformation,if=variable.st_planning&!talent.unholy_blight
            if (CanCastDarkTransformation)
                if (st_planning && !TalentUnholyBlight)
                    return DarkTransformation();

            //actions.cooldowns+=/dark_transformation,if=active_enemies>=2
            if (CanCastDarkTransformation)
                if (EnemiesInMelee >= 2)
                    return DarkTransformation();

            //actions.cooldowns+=/apocalypse,if=active_enemies=1&debuff.festering_wound.stack>=4&((!talent.unholy_blight|talent.army_of_the_damned|conduit.convocation_of_the_dead)|talent.unholy_blight&!talent.army_of_the_damned&dot.unholy_blight.remains)
            if (CanCastApocalypse)
                if (EnemiesInMelee == 1 && DebuffFesteringWoundStacks >= 4 && ((!TalentUnholyBlight || TalentArmyoftheDamned || ConduitConvocationoftheDead) || TalentUnholyBlight && !TalentArmyoftheDamned && DotUnholyBlightRemains > 0))
                    return Apocalypse();

            //actions.cooldowns+=/apocalypse,target_if=max:debuff.festering_wound.stack,if=active_enemies>=2&debuff.festering_wound.stack>=4&!death_and_decay.ticking
            if (CanCastApocalypse)
                if (EnemiesInMelee >= 2 && DebuffFesteringWoundStacks >= 4 && !DeathandDecayTicking)
                    return Apocalypse();

            //actions.cooldowns+=/summon_gargoyle,if=runic_power.deficit<14
            if (CanCastSummonGargoyle)
                if (RunicDefecit < 14)
                    return SummonGargoyle();

            //actions.cooldowns+=/unholy_assault,if=variable.st_planning&debuff.festering_wound.stack<2&(pet.apoc_ghoul.active|conduit.convocation_of_the_dead&cooldown.apocalypse.remains)
            if (CanCastUnholyAssault)
                if (st_planning && DebuffFesteringWoundStacks < 2 && (PetApocGhoulActive || ConduitConvocationoftheDead && CDApocalypseRemains > 0))
                    return UnholyAssault();

            //actions.cooldowns+=/unholy_assault,target_if=min:debuff.festering_wound.stack,if=active_enemies>=2&debuff.festering_wound.stack<2
            if (CanCastUnholyAssault)
                if (EnemiesInMelee >= 2 && DebuffFesteringWoundStacks < 2)
                    return UnholyAssault();

            //actions.cooldowns+=/soul_reaper,target_if=target.time_to_pct_35<5&target.time_to_die>5
            if (CanCastSoulReaper)
                if (TargetTimeToDie > 5000 && TargetHealthPct <= 35)
                    return SoulReaper();

            //actions.cooldowns+=/raise_dead,if=!pet.ghoul.active
            if (CanCastRaiseDead)
                if (!PetGhoulActive)
                    return RaiseDead();

            //actions+=/run_action_list,name=aoe_setup,if=active_enemies>=2&(cooldown.death_and_decay.remains<10&!talent.defile|cooldown.defile.remains<10&talent.defile)&!death_and_decay.ticking
            if (EnemiesInMelee>=2 && (CDDeathandDecayRemains<10000 && !TalentDefile || CDDefileRemains<10000&&TalentDefile)&&!DeathandDecayTicking)
            {
                //actions.aoe_setup=any_dnd,if=death_knight.fwounded_targets=active_enemies|raid_event.adds.exists&raid_event.adds.remains<=11
                if (EnemiesInMelee>=2)
                {
                    if (CanCastDeathandDecay)
                        return DeathandDecay();
                    if (CanCastDefile)
                        return Defile();
                    if (CanCastDeathsDue)
                        return DeathsDue();
                }

                //actions.aoe_setup+=/any_dnd,if=death_knight.fwounded_targets>=5
                if (FWoundTargets>=5)
                {
                    if (CanCastDeathandDecay)
                        return DeathandDecay();
                    if (CanCastDefile)
                        return Defile();
                    if (CanCastDeathsDue)
                        return DeathsDue();
                }

                //actions.aoe_setup+=/epidemic,if=!variable.pooling_for_gargoyle
                if (CanCastEpidemic)
                    if (!pooling_for_gargoyle)
                        return Epidemic();

                //actions.aoe_setup+=/festering_strike,target_if=max:debuff.festering_wound.stack,if=debuff.festering_wound.stack<=3&cooldown.apocalypse.remains<3
                if (CanCastFesteringStrike)
                    if (DebuffFesteringWoundStacks <= 3 && CDApocalypseRemains < 3000)
                        return FesteringStrike();

                //actions.aoe_setup+=/festering_strike,target_if=debuff.festering_wound.stack<1
                if (CanCastFesteringStrike)
                    if (DebuffFesteringWoundStacks < 1)
                        return FesteringStrike();

                //actions.aoe_setup+=/festering_strike,target_if=min:debuff.festering_wound.stack,if=rune.time_to_4<(cooldown.death_and_decay.remains&!talent.defile|cooldown.defile.remains&talent.defile)
                if (CanCastFesteringStrike)
                    if (Aimsharp.TimeUntilRunes(4)<((CDDeathandDecayRemains>0&&!TalentDefile||CDDefileRemains>0&&TalentDefile) ? 1000 : 0 ))
                        return FesteringStrike();

                return false;
            }

            //actions+=/run_action_list,name=aoe_burst,if=active_enemies>=2&death_and_decay.ticking
            if (DeathandDecayTicking && EnemiesInMelee>=2)
            {
                //actions.aoe_burst=epidemic,if=runic_power.deficit<(10+death_knight.fwounded_targets*3)&death_knight.fwounded_targets<6&!variable.pooling_for_gargoyle|buff.swarming_mist.up
                if (CanCastEpidemic)
                    if (RunicDefecit < (10 + FWoundTargets * 3) && FWoundTargets < 6 && !pooling_for_gargoyle || BuffSwarmingMistUp)
                        return Epidemic();

                //actions.aoe_burst+=/epidemic,if=runic_power.deficit<25&death_knight.fwounded_targets>5&!variable.pooling_for_gargoyle
                if (CanCastEpidemic)
                    if (RunicDefecit<25&&FWoundTargets>5&&!pooling_for_gargoyle)
                        return Epidemic();

                //actions.aoe_burst+=/epidemic,if=!death_knight.fwounded_targets&!variable.pooling_for_gargoyle|fight_remains<5|raid_event.adds.exists&raid_event.adds.remains<5
                if (CanCastEpidemic)
                    if (FWoundTargets == 0 && !pooling_for_gargoyle || TargetTimeToDie < 5000)
                        return Epidemic();

                //actions.aoe_burst+=/wound_spender
                if (CanCastScourgeStrike)
                    return ScourgeStrike();
                if (CanCastClawingShadows)
                    return ClawingShadows();

                //actions.aoe_burst+=/epidemic,if=!variable.pooling_for_gargoyle
                if (CanCastEpidemic)
                    if (!pooling_for_gargoyle)
                        return Epidemic();

                return false;
            }

            //actions+=/run_action_list,name=generic_aoe,if=active_enemies>=2&(!death_and_decay.ticking&(cooldown.death_and_decay.remains>10&!talent.defile|cooldown.defile.remains>10&talent.defile))
            if (EnemiesInMelee>=2&&(!DeathandDecayTicking&&(CDDeathandDecayRemains>10000&&!TalentDefile||CDDefileRemains>10000&&TalentDefile)))
            {
                //actions.generic_aoe=epidemic,if=buff.sudden_doom.react
                if (CanCastEpidemic)
                    if (BuffSuddenDoomUp)
                        return Epidemic();

                //actions.generic_aoe+=/epidemic,if=!variable.pooling_for_gargoyle
                if (CanCastEpidemic)
                    if (!pooling_for_gargoyle)
                        return Epidemic();

                //actions.generic_aoe+=/wound_spender,target_if=max:debuff.festering_wound.stack,if=(cooldown.apocalypse.remains>5&debuff.festering_wound.up|debuff.festering_wound.stack>4)&(fight_remains<cooldown.death_and_decay.remains+10|fight_remains>cooldown.apocalypse.remains)
                if ((CDApocalypseRemains>5000&&DebuffFesteringWoundUp||DebuffFesteringWoundStacks>4)&&(TargetTimeToDie<CDDeathandDecayRemains+10000||TargetTimeToDie>CDApocalypseRemains))
                {
                    if (CanCastScourgeStrike)
                        return ScourgeStrike();
                    if (CanCastClawingShadows)
                        return ClawingShadows();
                }

                //actions.generic_aoe+=/festering_strike,target_if=max:debuff.festering_wound.stack,if=debuff.festering_wound.stack<=3&cooldown.apocalypse.remains<3|debuff.festering_wound.stack<1
                if (CanCastFesteringStrike)
                    if (DebuffFesteringWoundStacks <= 3 && CDApocalypseRemains < 3000 || DebuffFesteringWoundStacks < 1)
                        return FesteringStrike();

                //actions.generic_aoe+=/festering_strike,target_if=min:debuff.festering_wound.stack,if=cooldown.apocalypse.remains>5&debuff.festering_wound.stack<1
                if (CanCastFesteringStrike)
                    if (CDApocalypseRemains>5000&&DebuffFesteringWoundStacks<1)
                        return FesteringStrike();

                return false;
            }

            //actions.generic=death_coil,if=buff.sudden_doom.react&!variable.pooling_for_gargoyle|pet.gargoyle.active
            if (CanCastDeathCoil)
                if (BuffSuddenDoomUp && !pooling_for_gargoyle || PetGargoyleActive)
                    return DeathCoil();

            //actions.generic+=/death_coil,if=runic_power.deficit<13&!variable.pooling_for_gargoyle
            if (CanCastDeathCoil)
                if (RunicDefecit<13&&!pooling_for_gargoyle)
                    return DeathCoil();

            //actions.generic+=/any_dnd,if=cooldown.apocalypse.remains&(talent.defile.enabled|covenant.night_fae|runeforge.phearomones)
            if (CDApocalypseRemains>0&&(TalentDefile||CovenantNightFae||RuneforgePhearomones))
            {
                if (CanCastDeathandDecay)
                    return DeathandDecay();
                if (CanCastDefile)
                    return Defile();
                if (CanCastDeathsDue)
                    return DeathsDue();
            }

            //actions.generic+=/wound_spender,if=debuff.festering_wound.stack>4
            if (DebuffFesteringWoundStacks>4)
            {
                if (CanCastScourgeStrike)
                    return ScourgeStrike();
                if (CanCastClawingShadows)
                    return ClawingShadows();
            }

            //actions.generic+=/wound_spender,if=debuff.festering_wound.up&cooldown.apocalypse.remains>5&(!talent.unholy_blight|talent.army_of_the_damned|conduit.convocation_of_the_dead|raid_event.adds.exists)
            if (DebuffFesteringWoundUp&&CDApocalypseRemains>5000&&(!TalentUnholyBlight||TalentArmyoftheDamned||ConduitConvocationoftheDead))
            {
                if (CanCastScourgeStrike)
                    return ScourgeStrike();
                if (CanCastClawingShadows)
                    return ClawingShadows();
            }

            //actions.generic+=/wound_spender,if=debuff.festering_wound.up&talent.unholy_blight&!talent.army_of_the_damned&!conduit.convocation_of_the_dead&!raid_event.adds.exists&(cooldown.unholy_blight.remains>5&cooldown.apocalypse.ready&!dot.unholy_blight.remains|!cooldown.apocalypse.ready)
            if (DebuffFesteringWoundUp&&TalentUnholyBlight&&!TalentArmyoftheDamned&&!ConduitConvocationoftheDead&&EnemiesInMelee<=1&&(CDUnholyBlightRemains>5000&&CDApocalypseUp&&!DotUnholyBlightUp||!CDApocalypseUp))
            {
                if (CanCastScourgeStrike)
                    return ScourgeStrike();
                if (CanCastClawingShadows)
                    return ClawingShadows();
            }

            //actions.generic+=/death_coil,if=runic_power.deficit<20&!variable.pooling_for_gargoyle
            if (CanCastDeathCoil)
                if (RunicDefecit < 20 && !pooling_for_gargoyle)
                    return DeathCoil();

            //actions.generic+=/festering_strike,if=debuff.festering_wound.stack<1
            if (CanCastFesteringStrike)
                if (DebuffFesteringWoundStacks<1)
                    return FesteringStrike();

            //actions.generic+=/festering_strike,if=debuff.festering_wound.stack<4&cooldown.apocalypse.remains<3&(!talent.unholy_blight|talent.army_of_the_damned|conduit.convocation_of_the_dead|raid_event.adds.exists)
            if (CanCastFesteringStrike)
                if (DebuffFesteringWoundStacks < 4 && CDApocalypseRemains < 3000 && (!TalentUnholyBlight || TalentArmyoftheDamned || ConduitConvocationoftheDead))
                    return FesteringStrike();

            //actions.generic+=/festering_strike,if=debuff.festering_wound.stack<4&talent.unholy_blight&!talent.army_of_the_damned&!conduit.convocation_of_the_dead&!raid_event.adds.exists&cooldown.apocalypse.ready&(cooldown.unholy_blight.remains<3|dot.unholy_blight.remains)
            if (CanCastFesteringStrike)
                if (DebuffFesteringWoundStacks < 4 && TalentUnholyBlight&&!TalentArmyoftheDamned&&!ConduitConvocationoftheDead&& EnemiesInMelee <= 1&&CDApocalypseUp && (CDUnholyBlightRemains<3000||DotUnholyBlightUp))
                    return FesteringStrike();

            //actions.generic+=/death_coil,if=!variable.pooling_for_gargoyle
            if (CanCastDeathCoil)
                if (!pooling_for_gargoyle)
                    return DeathCoil();

            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

        bool ArcaneTorrent() { Aimsharp.Cast("Arcane Torrent"); return true; }
        bool LightsJudgment() { Aimsharp.Cast("Light's Judgment"); return true; }
        bool ArcanePulse() { Aimsharp.Cast("Arcane Pulse"); return true; }
        bool Outbreak() { Aimsharp.Cast("Outbreak"); return true; }
        bool Epidemic() { Aimsharp.Cast("Epidemic"); return true; }
        bool ScourgeStrike() { Aimsharp.Cast("Scourge Strike"); return true; }
        bool ClawingShadows() { Aimsharp.Cast("Clawing Shadows"); return true; }
        bool DeathandDecay() { Aimsharp.Cast("DeathandDecayC"); return true; }
        bool DeathsDue() { Aimsharp.Cast("DeathsDueC"); return true; }
        bool Defile() { Aimsharp.Cast("DefileC"); return true; }
        bool FesteringStrike() { Aimsharp.Cast("Festering Strike"); return true; }
        bool ArmyoftheDead() { Aimsharp.Cast("Army of the Dead"); return true; }
        bool UnholyBlight() { Aimsharp.Cast("Unholy Blight"); return true; }
        bool DarkTransformation() { Aimsharp.Cast("Dark Transformation"); return true; }
        bool Apocalypse() { Aimsharp.Cast("Apocalypse"); return true; }
        bool SummonGargoyle() { Aimsharp.Cast("Summon Gargoyle"); return true; }
        bool UnholyAssault() { Aimsharp.Cast("Unholy Assault"); return true; }
        bool SoulReaper() { Aimsharp.Cast("Soul Reaper"); return true; }
        bool RaiseDead() { Aimsharp.Cast("Raise Dead"); return true; }
        bool SwarmingMist() { Aimsharp.Cast("Swarming Mist"); return true; }
        bool AbominationLimb() { Aimsharp.Cast("Abomination Limb"); return true; }
        bool ShackletheUnworthy() { Aimsharp.Cast("Shackle the Unworthy"); return true; }
        bool DeathCoil() { Aimsharp.Cast("Death Coil"); return true; }


    }
}
