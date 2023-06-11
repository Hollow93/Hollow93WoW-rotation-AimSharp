using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsAffliction : Rotation
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
           "Phantom Singularity","Agony","Haunt","Seed of Corruption","Vile Taint","Unstable Affliction","Siphon Life","Corruption","Malefic Rapture","Dark Soul: Misery","Drain Life","Drain Soul","Shadow Bolt","Decimating Bolt","Soul Rot","Scouring Tithe","Summon Darkglare","Impending Catastrophe",
        };

        List<string> BuffsList = new List<string>
        {
            "Inevitable Demise",
        };

        List<string> DebuffsList = new List<string>
        {
           "Shadow Embrace","Haunt","Impending Catastrophe","Phantom Singularity","Soul Rot","Agony","Seed of Corruption","Corruption","Unstable Affliction","Siphon Life","Vile Taint",
        };

        List<string> TotemsList = new List<string>
        {

        };

        List<string> MacroCommands = new List<string>
        {
            "AOE","SaveCooldowns","Seed"
        };

        public override void LoadSettings()
        {
            Settings.Add(new Setting("General Settings"));
            Settings.Add(new Setting("Use Top Trinket:", false));
            Settings.Add(new Setting("Use Bottom Trinket:", false));
            Settings.Add(new Setting("Use DPS Potion:", false));
            Settings.Add(new Setting("Potion name:", "Potion of Unbridled Fury"));

            Settings.Add(new Setting("Warlock Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Affliction", Color.Purple);
            Aimsharp.PrintMessage("Version 2.1", Color.Purple);

            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx Seed", Color.Blue);
            Aimsharp.PrintMessage("--Forces of Seed of Corruption cast.", Color.Blue);
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

            Macros.Add("VileTaintC", "/cast [@cursor] Vile Taint");


            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            CustomFunctions.Add("SeedCount", "local FlameShockCount = 0\nfor i=1,20 do\nlocal unit = \"nameplate\" .. i\nif UnitExists(unit) then\nif UnitCanAttack(\"player\", unit) then\nfor j = 1, 40 do\nlocal name,_,_,_,_,_,source = UnitDebuff(unit, j)\nif name == \"Seed of Corruption\" and source == \"player\" then\nFlameShockCount = FlameShockCount + 1\nend\nend\nend\nend\nend\nreturn FlameShockCount");

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
            bool Seed = Aimsharp.IsCustomCodeOn("Seed");

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
            bool CovenantNone = CovenantID == 0;

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
            float SoulShard = Aimsharp.PlayerSecondaryPower() / 10f;

            //Talents
            bool TalentPhantomSingularity = Aimsharp.Talent(4, 2);
            bool TalentSowtheSeeds = Aimsharp.Talent(4, 1);
            bool TalentSiphonLife = Aimsharp.Talent(2, 3);
            bool TalentVileTaint = Aimsharp.Talent(4, 3);
            bool TalentHaunt = Aimsharp.Talent(6, 2);
            bool TalentCreepingDeath = Aimsharp.Talent(7, 2);
            bool TalentAbsoluteCorruption = Aimsharp.Talent(2, 2);
            bool TalentDrainSoul = Aimsharp.Talent(1, 3);


            //buffs
            int BuffInevitableDemiseRemains = Aimsharp.BuffRemaining("Inevitable Demise") - GCD;
            bool BuffInevitableDemiseUp = BuffInevitableDemiseRemains > 0;
            int BuffInevitableDemiseStacks = Aimsharp.BuffStacks("Inevitable Demise");


            //debuffs
            int DebuffShadowEmbraceRemains = Aimsharp.DebuffRemaining("Shadow Embrace") - GCD;
            bool DebuffShadowEmbraceUp = DebuffShadowEmbraceRemains > 0;
            int DebuffShadowEmbraceStacks = Aimsharp.DebuffStacks("Shadow Embrace");
            int DebuffHauntRemains = Aimsharp.DebuffRemaining("Haunt") - GCD;
            bool DebuffHauntUp = DebuffHauntRemains > 0;
            int DotImpendingCatastropheRemains = Aimsharp.DebuffRemaining("Impending Catastrophe") - GCD;
            bool DotImpendingCatastropheUp = DotImpendingCatastropheRemains > 0;
            int DotPhantomSingularityRemains = Aimsharp.DebuffRemaining("Phantom Singularity") - GCD;
            bool DotPhantomSingularityUp = DotPhantomSingularityRemains > 0;
            int DotSoulRotRemains = Aimsharp.DebuffRemaining("Soul Rot") - GCD;
            bool DotSoulRotUp = DotSoulRotRemains > 0;
            int DotAgonyRemains = Aimsharp.DebuffRemaining("Agony") - GCD;
            bool DotAgonyUp = DotAgonyRemains > 0;
            bool DotAgonyRefreshable = DotAgonyRemains < 5400 * (TalentCreepingDeath ? .85 : 1);
            int DotSeedofCorruptionRemains = Aimsharp.DebuffRemaining("Seed of Corruption") - GCD;
            bool DotSeedofCorruptionUp = DotSeedofCorruptionRemains > 0;
            int DotCorruptionRemains = TalentAbsoluteCorruption ? 999999 : Aimsharp.DebuffRemaining("Corruption") - GCD;
            bool DotCorruptionUp = TalentAbsoluteCorruption ? Aimsharp.HasDebuff("Corruption") : DotCorruptionRemains > 0;
            bool DotCorruptionRefreshable = DotCorruptionRemains < 4200 * (TalentCreepingDeath ? .85 : 1) * (TalentAbsoluteCorruption ? 0 : 1);
            int DotUnstableAfflictionRemains = Aimsharp.DebuffRemaining("Unstable Affliction") - GCD;
            bool DotUnstableAfflictionUp = DotUnstableAfflictionRemains > 0;
            bool DotUnstableAfflictionRefreshable = DotUnstableAfflictionRemains < 6300 * (TalentCreepingDeath ? .85 : 1);
            int DotSiphonLifeRemains = Aimsharp.DebuffRemaining("Siphon Life") - GCD;
            bool DotSiphonLifeUp = DotSiphonLifeRemains > 0;
            bool DotSiphonLifeRefreshable = DotSiphonLifeRemains < 4500 * (TalentCreepingDeath ? .85 : 1);
            int DotVileTaintRemains = Aimsharp.DebuffRemaining("Vile Taint") - GCD;
            bool DotVileTaintUp = DotVileTaintRemains > 0;


            //cooldowns
            int CDPhantomSingularityRemains = Aimsharp.SpellCooldown("Phantom Singularity");
            bool CDPhantomSingularityUp = CDPhantomSingularityRemains <= 0;
            int CDSoulRotRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Soul Rot");
            bool CDSoulRotUp = CDSoulRotRemains <= 0;
            int CDSummonDarkglareRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Summon Darkglare");
            bool CDSummonDarkglareUp = CDSummonDarkglareRemains <= 0;
            int CDImpendingCatastropheRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Impending Catastrophe");
            bool CDImpendingCatastropheUp = CDImpendingCatastropheRemains <= 0;


            //specific variables
            bool ConduitCorruptingLeer = ActiveConduits.Contains(339455);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastPhantomSingularity = Aimsharp.CanCast("Phantom Singularity") && Fighting;
            bool CanCastAgony = Aimsharp.CanCast("Agony") && Fighting;
            bool CanCastHaunt = Aimsharp.CanCast("Haunt") && !Moving && Fighting;
            bool CanCastSeedofCorruption = Aimsharp.CanCast("Seed of Corruption") && !Moving && Fighting && PlayerCastingID != 27243;
            bool CanCastVileTaint = Aimsharp.CanCast("Vile Taint", "player") && !Moving && Fighting;
            bool CanCastUnstableAffliction = Aimsharp.CanCast("Unstable Affliction") && !Moving && Fighting && PlayerCastingID != 316099;
            bool CanCastSiphonLife = Aimsharp.CanCast("Siphon Life") && Fighting;
            bool CanCastCorruption = Aimsharp.CanCast("Corruption") && Fighting;
            bool CanCastMaleficRapture = Aimsharp.CanCast("Malefic Rapture", "player") && !Moving && Fighting;
            bool CanCastDarkSoulMisery = Aimsharp.CanCast("Dark Soul: Misery", "player") && !SaveCooldowns && Fighting;
            bool CanCastDrainLife = Aimsharp.CanCast("Drain Life") && !Moving && Fighting && PlayerCastingID != 234153;
            bool CanCastDrainSoul = Aimsharp.CanCast("Drain Soul") && !Moving && Fighting && PlayerCastingID != 198590;
            bool CanCastShadowBolt = Aimsharp.CanCast("Shadow Bolt") && !Moving && Fighting && !TalentDrainSoul;
            bool CanCastDecimatingBolt = Aimsharp.CanCast("Decimating Bolt") && !Moving && Fighting;
            bool CanCastSoulRot = Aimsharp.CanCast("Soul Rot") && !SaveCooldowns && !Moving && Fighting;
            bool CanCastScouringTithe = Aimsharp.CanCast("Scouring Tithe") && !Moving && Fighting;
            bool CanCastSummonDarkglare = Aimsharp.CanCast("Summon Darkglare", "player") && !SaveCooldowns && Fighting;
            bool CanCastImpendingCatastrophe = Aimsharp.CanCast("Impending Catastrophe") && !SaveCooldowns && !Moving && Fighting;

            int SeedCount = Aimsharp.CustomFunction("SeedCount");



            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            //if (IsChanneling)
            //    return false;
            if (Seed)
                if (CanCastSeedofCorruption)
                    return SeedofCorruption();

            //actions=call_action_list,name=aoe,if=active_enemies>3
            if (EnemiesNearTarget > 3)
            {
                //actions.aoe=phantom_singularity
                if (CanCastPhantomSingularity)
                    return PhantomSingularity();

                //actions.aoe+=/haunt
                if (CanCastHaunt)
                    return Haunt();

                //actions.aoe+=/call_action_list,name=darkglare_prep,if=covenant.venthyr&dot.impending_catastrophe_dot.ticking&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
                //actions.aoe+=/call_action_list,name=darkglare_prep,if=covenant.night_fae&dot.soul_rot.ticking&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
                //actions.aoe+=/call_action_list,name=darkglare_prep,if=(covenant.necrolord|covenant.kyrian|covenant.none)&dot.phantom_singularity.ticking&dot.phantom_singularity.remains<2

                if ((CovenantVenthyr && DotImpendingCatastropheUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000) || !TalentPhantomSingularity) ||
                    (CovenantNightFae && DotSoulRotUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains < 2000 || !TalentPhantomSingularity)) ||
                    ((CovenantNecrolord || CovenantKyrian || CovenantNone) && DotPhantomSingularityUp && DotPhantomSingularityRemains < 2000))
                {
                    //actions.darkglare_prep=vile_taint,if=cooldown.summon_darkglare.remains<2
                    if (CanCastVileTaint)
                        if (CDSummonDarkglareRemains < 2000)
                            return VileTaint();

                    //actions.darkglare_prep+=/dark_soul
                    if (CanCastDarkSoulMisery)
                        return DarkSoulMisery();

                    //actions.darkglare_prep+=/potion
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

                    //actions.darkglare_prep+=/call_action_list,name=covenant,if=!covenant.necrolord&cooldown.summon_darkglare.remains<2
                    if (!CovenantNecrolord && CDSummonDarkglareRemains < 2000)
                    {
                        //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                        if (CanCastImpendingCatastrophe)
                            if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                                return ImpendingCatastrophe();

                        //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                        if (CanCastDecimatingBolt)
                            if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                                return DecimatingBolt();

                        //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                        if (CanCastSoulRot)
                            if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                                return SoulRot();

                        //actions.covenant+=/scouring_tithe
                        if (CanCastScouringTithe)
                            return ScouringTithe();
                    }

                    //actions.darkglare_prep+=/summon_darkglare
                    if (CanCastSummonDarkglare)
                        return SummonDarkglare();
                }

                //actions.aoe+=/seed_of_corruption,if=talent.sow_the_seeds.enabled&can_seed
                if (CanCastSeedofCorruption)
                    if (TalentSowtheSeeds && SeedCount < EnemiesNearTarget - 1)
                        return SeedofCorruption();

                //actions.aoe+=/seed_of_corruption,if=!talent.sow_the_seeds.enabled&!dot.seed_of_corruption.ticking&!in_flight&dot.corruption.refreshable
                if (CanCastSeedofCorruption)
                    if (!TalentSowtheSeeds && !DotSeedofCorruptionUp && DotCorruptionRefreshable)
                        return SeedofCorruption();

                //actions.aoe+=/agony,cycle_targets=1,if=active_dot.agony<4,target_if=!dot.agony.ticking
                if (CanCastAgony)
                    if (!DotAgonyUp)
                        return Agony();

                //actions.aoe+=/agony,cycle_targets=1,if=active_dot.agony>=4,target_if=refreshable&dot.agony.ticking
                if (CanCastAgony)
                    if (DotAgonyRefreshable && DotAgonyUp)
                        return Agony();

                //actions.aoe+=/unstable_affliction,if=dot.unstable_affliction.refreshable
                if (CanCastUnstableAffliction)
                    if (DotUnstableAfflictionRefreshable)
                        return UnstableAffliction();

                //actions.aoe+=/vile_taint,if=soul_shard>1
                if (CanCastVileTaint)
                    if (SoulShard > 1)
                        return VileTaint();

                //actions.aoe+=/call_action_list,name=covenant,if=!covenant.necrolord
                if (!CovenantNecrolord)
                {
                    //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                    if (CanCastImpendingCatastrophe)
                        if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                            return ImpendingCatastrophe();

                    //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                    if (CanCastDecimatingBolt)
                        if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                            return DecimatingBolt();

                    //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                    if (CanCastSoulRot)
                        if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                            return SoulRot();

                    //actions.covenant+=/scouring_tithe
                    if (CanCastScouringTithe)
                        return ScouringTithe();
                }

                //actions.aoe+=/call_action_list,name=darkglare_prep,if=covenant.venthyr&(cooldown.impending_catastrophe.ready|dot.impending_catastrophe_dot.ticking)&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
                //actions.aoe+=/call_action_list,name=darkglare_prep,if=(covenant.necrolord|covenant.kyrian|covenant.none)&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
                //actions.aoe+=/call_action_list,name=darkglare_prep,if=covenant.night_fae&(cooldown.soul_rot.ready|dot.soul_rot.ticking)&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
                if ((CovenantVenthyr && (CDImpendingCatastropheUp || DotImpendingCatastropheUp) && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                    ((CovenantNecrolord || CovenantKyrian || CovenantNone) && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                    (CovenantNightFae && (CDSoulRotUp || DotSoulRotUp) && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)))
                {
                    //actions.darkglare_prep=vile_taint,if=cooldown.summon_darkglare.remains<2
                    if (CanCastVileTaint)
                        if (CDSummonDarkglareRemains < 2000)
                            return VileTaint();

                    //actions.darkglare_prep+=/dark_soul
                    if (CanCastDarkSoulMisery)
                        return DarkSoulMisery();

                    //actions.darkglare_prep+=/potion
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

                    //actions.darkglare_prep+=/call_action_list,name=covenant,if=!covenant.necrolord&cooldown.summon_darkglare.remains<2
                    if (!CovenantNecrolord && CDSummonDarkglareRemains < 2000)
                    {
                        //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                        if (CanCastImpendingCatastrophe)
                            if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                                return ImpendingCatastrophe();

                        //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                        if (CanCastDecimatingBolt)
                            if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                                return DecimatingBolt();

                        //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                        if (CanCastSoulRot)
                            if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                                return SoulRot();

                        //actions.covenant+=/scouring_tithe
                        if (CanCastScouringTithe)
                            return ScouringTithe();
                    }

                    //actions.darkglare_prep+=/summon_darkglare
                    if (CanCastSummonDarkglare)
                        return SummonDarkglare();
                }

                //actions.aoe+=/dark_soul,if=cooldown.summon_darkglare.remains>time_to_die
                if (CanCastDarkSoulMisery)
                    if (CDSummonDarkglareRemains > TargetTimeToDie)
                        return DarkSoulMisery();

                //actions.aoe+=/call_action_list,name=item
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

                //actions.aoe+=/malefic_rapture,if=dot.vile_taint.ticking
                //actions.aoe+=/malefic_rapture,if=dot.soul_rot.ticking&!talent.sow_the_seeds.enabled
                //actions.aoe +=/ malefic_rapture,if= !talent.vile_taint.enabled
                //actions.aoe+=/malefic_rapture,if=soul_shard>4
                if (CanCastMaleficRapture)
                    if (DotVileTaintUp || DotSoulRotUp && !TalentSowtheSeeds || !TalentVileTaint || SoulShard > 4)
                        return MaleficRapture();

                //actions.aoe+=/siphon_life,cycle_targets=1,if=active_dot.siphon_life<=3,target_if=!dot.siphon_life.ticking
                if (CanCastSiphonLife)
                    if (!DotSiphonLifeUp)
                        return SiphonLife();

                //actions.aoe+=/call_action_list,name=covenant
                //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                if (CanCastImpendingCatastrophe)
                    if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                        return ImpendingCatastrophe();

                //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                if (CanCastDecimatingBolt)
                    if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                        return DecimatingBolt();

                //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                if (CanCastSoulRot)
                    if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                        return SoulRot();

                //actions.covenant+=/scouring_tithe
                if (CanCastScouringTithe)
                    return ScouringTithe();

                //actions.aoe+=/drain_life,if=buff.inevitable_demise.stack>=50|buff.inevitable_demise.up&time_to_die<5|buff.inevitable_demise.stack>=35&dot.soul_rot.ticking
                if (CanCastDrainLife)
                    if (BuffInevitableDemiseStacks >= 50 || BuffInevitableDemiseUp && TargetTimeToDie < 5000 || BuffInevitableDemiseStacks >= 35 && DotSoulRotUp)
                        return DrainLife();

                //actions.aoe+=/drain_soul,interrupt=1
                if (CanCastDrainSoul)
                    return DrainSoul();

                //actions.aoe+=/shadow_bolt
                if (CanCastShadowBolt)
                    return ShadowBolt();
            }

            //actions+=/phantom_singularity,if=time>30
            if (CanCastPhantomSingularity)
                if (Time > 30000)
                    return PhantomSingularity();

            //actions+=/call_action_list,name=darkglare_prep,if=covenant.venthyr&dot.impending_catastrophe_dot.ticking&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
            //actions+=/call_action_list,name=darkglare_prep,if=covenant.night_fae&dot.soul_rot.ticking&cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
            //actions+=/call_action_list,name=darkglare_prep,if=(covenant.necrolord|covenant.kyrian|covenant.none)&dot.phantom_singularity.ticking&dot.phantom_singularity.remains<2
            if ((CovenantVenthyr && DotImpendingCatastropheUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                (CovenantNightFae && DotSoulRotUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                ((CovenantNecrolord || CovenantKyrian || CovenantNone) && DotPhantomSingularityUp && DotPhantomSingularityRemains < 2000))
            {

                //actions.darkglare_prep=vile_taint,if=cooldown.summon_darkglare.remains<2
                if (CanCastVileTaint)
                    if (CDSummonDarkglareRemains < 2000)
                        return VileTaint();

                //actions.darkglare_prep+=/dark_soul
                if (CanCastDarkSoulMisery)
                    return DarkSoulMisery();

                //actions.darkglare_prep+=/potion
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

                //actions.darkglare_prep+=/call_action_list,name=covenant,if=!covenant.necrolord&cooldown.summon_darkglare.remains<2
                if (!CovenantNecrolord && CDSummonDarkglareRemains < 2000)
                {
                    //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                    if (CanCastImpendingCatastrophe)
                        if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                            return ImpendingCatastrophe();

                    //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                    if (CanCastDecimatingBolt)
                        if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                            return DecimatingBolt();

                    //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                    if (CanCastSoulRot)
                        if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                            return SoulRot();

                    //actions.covenant+=/scouring_tithe
                    if (CanCastScouringTithe)
                        return ScouringTithe();
                }

                //actions.darkglare_prep+=/summon_darkglare
                if (CanCastSummonDarkglare)
                    return SummonDarkglare();
            }

            //actions+=/agony,if=dot.agony.remains<4
            if (CanCastAgony)
                if (DotAgonyRemains < 4000)
                    return Agony();

            //actions+=/haunt
            if (CanCastHaunt)
                return Haunt();

            if (EnemiesNearTarget>2)
            {
                if ((CovenantVenthyr && DotImpendingCatastropheUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                (CovenantNightFae && DotSoulRotUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                ((CovenantNecrolord || CovenantKyrian) && DotPhantomSingularityUp && DotPhantomSingularityRemains < 2000))
                {

                    //actions.darkglare_prep=vile_taint,if=cooldown.summon_darkglare.remains<2
                    if (CanCastVileTaint)
                        if (CDSummonDarkglareRemains < 2000)
                            return VileTaint();

                    //actions.darkglare_prep+=/dark_soul
                    if (CanCastDarkSoulMisery)
                        return DarkSoulMisery();

                    //actions.darkglare_prep+=/potion
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

                    //actions.darkglare_prep+=/call_action_list,name=covenant,if=!covenant.necrolord&cooldown.summon_darkglare.remains<2
                    if (!CovenantNecrolord && CDSummonDarkglareRemains < 2000)
                    {
                        //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                        if (CanCastImpendingCatastrophe)
                            if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                                return ImpendingCatastrophe();

                        //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                        if (CanCastDecimatingBolt)
                            if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                                return DecimatingBolt();

                        //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                        if (CanCastSoulRot)
                            if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                                return SoulRot();

                        //actions.covenant+=/scouring_tithe
                        if (CanCastScouringTithe)
                            return ScouringTithe();
                    }

                    //actions.darkglare_prep+=/summon_darkglare
                    if (CanCastSummonDarkglare)
                        return SummonDarkglare();
                }
            }

            //actions+=/seed_of_corruption,if=active_enemies>2&talent.sow_the_seeds.enabled&!dot.seed_of_corruption.ticking&!in_flight
            if (CanCastSeedofCorruption)
                if (EnemiesNearTarget > 2 && TalentSowtheSeeds && !DotSeedofCorruptionUp)
                    return SeedofCorruption();

            //actions+=/seed_of_corruption,if=active_enemies>2&talent.siphon_life.enabled&!dot.seed_of_corruption.ticking&!in_flight&dot.corruption.remains<4
            if (CanCastSeedofCorruption)
                if (EnemiesNearTarget > 2 && TalentSiphonLife && !DotSeedofCorruptionUp && DotCorruptionRemains < 4000)
                    return SeedofCorruption();

            //actions+=/vile_taint,if=(soul_shard>1|active_enemies>2)&cooldown.summon_darkglare.remains>12
            if (CanCastVileTaint)
                if ((SoulShard > 1 || EnemiesNearTarget > 2) && CDSummonDarkglareRemains > 12000)
                    return VileTaint();

            //actions+=/unstable_affliction,if=dot.unstable_affliction.remains<4
            if (CanCastUnstableAffliction)
                if (DotUnstableAfflictionRemains < 4000)
                    return UnstableAffliction();

            //actions+=/siphon_life,if=dot.siphon_life.remains<4
            if (CanCastSiphonLife)
                if (DotSiphonLifeRemains < 4000)
                    return SiphonLife();

            //actions+=/call_action_list,name=covenant,if=!covenant.necrolord
            if (!CovenantNecrolord)
            {
                //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                if (CanCastImpendingCatastrophe)
                    if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                        return ImpendingCatastrophe();

                //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                if (CanCastDecimatingBolt)
                    if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                        return DecimatingBolt();

                //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                if (CanCastSoulRot)
                    if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                        return SoulRot();

                //actions.covenant+=/scouring_tithe
                if (CanCastScouringTithe)
                    return ScouringTithe();
            }

            //actions+=/corruption,if=active_enemies<4-(talent.sow_the_seeds.enabled|talent.siphon_life.enabled)&dot.corruption.remains<2
            if (CanCastCorruption)
                if (EnemiesNearTarget < 4 - (TalentSowtheSeeds || TalentSiphonLife ? 1 : 0) && DotCorruptionRemains < 2000)               
                    return Corruption();

            //actions+=/phantom_singularity,if=covenant.necrolord|covenant.night_fae|covenant.kyrian|covenant.none
            if (CanCastPhantomSingularity)
                if (CovenantNecrolord || CovenantNightFae || CovenantKyrian || CovenantNone)
                    return PhantomSingularity();

            //actions+=/malefic_rapture,if=soul_shard>4
            if (CanCastMaleficRapture)
                if (SoulShard > 4)
                    return MaleficRapture();

            if ((CovenantVenthyr && DotImpendingCatastropheUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                (CovenantNightFae && DotSoulRotUp && CDSummonDarkglareRemains < 2000 && (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularity)) ||
                ((CovenantNecrolord || CovenantKyrian) && DotPhantomSingularityUp && DotPhantomSingularityRemains < 2000))
            {

                //actions.darkglare_prep=vile_taint,if=cooldown.summon_darkglare.remains<2
                if (CanCastVileTaint)
                    if (CDSummonDarkglareRemains < 2000)
                        return VileTaint();

                //actions.darkglare_prep+=/dark_soul
                if (CanCastDarkSoulMisery)
                    return DarkSoulMisery();

                //actions.darkglare_prep+=/potion
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

                //actions.darkglare_prep+=/call_action_list,name=covenant,if=!covenant.necrolord&cooldown.summon_darkglare.remains<2
                if (!CovenantNecrolord && CDSummonDarkglareRemains < 2000)
                {
                    //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
                    if (CanCastImpendingCatastrophe)
                        if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                            return ImpendingCatastrophe();

                    //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
                    if (CanCastDecimatingBolt)
                        if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                            return DecimatingBolt();

                    //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
                    if (CanCastSoulRot)
                        if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                            return SoulRot();

                    //actions.covenant+=/scouring_tithe
                    if (CanCastScouringTithe)
                        return ScouringTithe();
                }

                //actions.darkglare_prep+=/summon_darkglare
                if (CanCastSummonDarkglare)
                    return SummonDarkglare();
            }

            //actions+=/dark_soul,if=cooldown.summon_darkglare.remains>time_to_die
            if (CanCastDarkSoulMisery)
                if (CDSummonDarkglareRemains > TargetTimeToDie)
                    return DarkSoulMisery();

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

            //actions+=/call_action_list,name=se,if=debuff.shadow_embrace.stack<(2-action.shadow_bolt.in_flight)|debuff.shadow_embrace.remains<3
            if (DebuffShadowEmbraceStacks<(2-(LastCast == "Shadow Bolt" ? 1:0))||DebuffShadowEmbraceRemains<3000)
            {
                //actions.se=haunt
                if (CanCastHaunt)
                    return Haunt();

                //actions.se+=/drain_soul,interrupt_global=1,interrupt_if=debuff.shadow_embrace.stack>=3
                if (CanCastDrainSoul)
                    if (DebuffShadowEmbraceStacks >= 3)
                        return DrainSoul();

                //actions.se+=/shadow_bolt
                if (CanCastShadowBolt)
                    return ShadowBolt();
            }

            //actions+=/malefic_rapture,if=dot.vile_taint.ticking
            //actions+=/malefic_rapture,if=dot.impending_catastrophe_dot.ticking
            //actions+=/malefic_rapture,if=dot.soul_rot.ticking
            //actions+=/malefic_rapture,if=talent.phantom_singularity.enabled&(dot.phantom_singularity.ticking|soul_shard>3|time_to_die<cooldown.phantom_singularity.remains)
            //actions+=/malefic_rapture,if=talent.sow_the_seeds.enabled
            if (CanCastMaleficRapture)
                if (DotVileTaintUp || DotImpendingCatastropheUp || DotSoulRotUp || TalentPhantomSingularity && (DotPhantomSingularityUp || SoulShard > 3 || TargetTimeToDie < CDPhantomSingularityRemains) | TalentSowtheSeeds)
                    return MaleficRapture();

            //actions+=/drain_life,if=buff.inevitable_demise.stack>40|buff.inevitable_demise.up&time_to_die<4
            if (CanCastDrainLife)
                if (BuffInevitableDemiseStacks > 40 || BuffInevitableDemiseUp && TargetTimeToDie < 4000)
                    return DrainLife();

            //actions+=/call_action_list,name=covenant
            //actions.covenant=impending_catastrophe,if=cooldown.summon_darkglare.remains<10|cooldown.summon_darkglare.remains>50
            if (CanCastImpendingCatastrophe)
                if (CDSummonDarkglareRemains < 10000 || CDSummonDarkglareRemains > 50000)
                    return ImpendingCatastrophe();

            //actions.covenant+=/decimating_bolt,if=cooldown.summon_darkglare.remains>5&(debuff.haunt.remains>4|!talent.haunt.enabled)
            if (CanCastDecimatingBolt)
                if (CDSummonDarkglareRemains > 5000 && (DebuffHauntRemains > 4000 || !TalentHaunt))
                    return DecimatingBolt();

            //actions.covenant+=/soul_rot,if=cooldown.summon_darkglare.remains<5|cooldown.summon_darkglare.remains>50|cooldown.summon_darkglare.remains>25&conduit.corrupting_leer.enabled
            if (CanCastSoulRot)
                if (CDSummonDarkglareRemains < 5000 || CDSummonDarkglareRemains > 50000 || CDSummonDarkglareRemains > 25000 && ConduitCorruptingLeer)
                    return SoulRot();

            //actions.covenant+=/scouring_tithe
            if (CanCastScouringTithe)
                return ScouringTithe();

            //actions+=/agony,if=refreshable
            if (CanCastAgony)
                if (DotAgonyRefreshable)
                    return Agony();

            //actions+=/corruption,if=refreshable&active_enemies<4-(talent.sow_the_seeds.enabled|talent.siphon_life.enabled)
            if (CanCastCorruption)
                if (DotCorruptionRefreshable && EnemiesNearTarget < 4 - (TalentSowtheSeeds || TalentSiphonLife ? 1 : 0))
                    return Corruption();

            //actions+=/unstable_affliction,if=refreshable
            if (CanCastUnstableAffliction)
                if (DotUnstableAfflictionRefreshable)
                    return UnstableAffliction();

            //actions+=/siphon_life,if=refreshable
            if (CanCastSiphonLife)
                if (DotSiphonLifeRefreshable)
                    return SiphonLife();

            //actions+=/corruption,cycle_targets=1,if=active_enemies<4-(talent.sow_the_seeds.enabled|talent.siphon_life.enabled),target_if=refreshable
            if (CanCastCorruption)
                if (DotCorruptionRefreshable)
                    return Corruption();

            //actions+=/drain_soul,interrupt=1
            if (CanCastDrainSoul)
                return DrainSoul();

            if (CanCastShadowBolt)
                return ShadowBolt();

            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

        bool PhantomSingularity() { Aimsharp.Cast("Phantom Singularity"); return true; }
        bool Agony() { Aimsharp.Cast("Agony"); return true; }
        bool Haunt() { Aimsharp.Cast("Haunt"); return true; }
        bool SeedofCorruption() { Aimsharp.Cast("Seed of Corruption"); return true; }
        bool VileTaint() { Aimsharp.Cast("VileTaintC"); return true; }
        bool UnstableAffliction() { Aimsharp.Cast("Unstable Affliction"); return true; }
        bool SiphonLife() { Aimsharp.Cast("Siphon Life"); return true; }
        bool Corruption() { Aimsharp.Cast("Corruption"); return true; }
        bool MaleficRapture() { Aimsharp.Cast("Malefic Rapture"); return true; }
        bool DarkSoulMisery() { Aimsharp.Cast("Dark Soul: Misery"); return true; }
        bool DrainLife() { Aimsharp.Cast("Drain Life"); return true; }
        bool DrainSoul() { Aimsharp.Cast("Drain Soul"); return true; }
        bool ShadowBolt() { Aimsharp.Cast("Shadow Bolt"); return true; }
        bool DecimatingBolt() { Aimsharp.Cast("Decimating Bolt"); return true; }
        bool SoulRot() { Aimsharp.Cast("Soul Rot"); return true; }
        bool ScouringTithe() { Aimsharp.Cast("Scouring Tithe"); return true; }
        bool SummonDarkglare() { Aimsharp.Cast("Summon Darkglare"); return true; }
        bool ImpendingCatastrophe() { Aimsharp.Cast("Impending Catastrophe"); return true; }


    }
}
