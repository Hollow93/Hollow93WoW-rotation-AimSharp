using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API

namespace AimsharpWow.Modules
{

    public class Aimsharp9_1Havoc : Rotation
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
           "Throw Glaive","Metamorphosis","Sinful Brand","The Hunt","Elysian Decree","Death Sweep","Glaive Tempest","Eye Beam","Blade Dance","Immolation Aura","Annihilation","Felblade","Essence Break","Chaos Strike","Fel Rush","Demon's Bite","Vengeful Retreat","Fel Barrage",
        };

        List<string> BuffsList = new List<string>
        {
            "Burning Wound","Chaos Theory","Metamorphosis","Essence Break","Momentum","Fel Bombardment","Immolation Aura","Furious Gaze","Unbound Chaos","Exposed Wound","Prepared",
        };

        List<string> DebuffsList = new List<string>
        {
           "Burning Wound","Essence Break","Exposed Wound","Sinful Brand",
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

            Settings.Add(new Setting("DemonHunter Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Burning Wound", "Chaos Theory", "Darkglare Medallion", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Aimsharp 9_1Havoc", Color.Purple);
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

            Macros.Add("ElysianDecreeC", "/cast [@cursor] Elysian Decree");
            Macros.Add("MetamorphosisC", "/cast [@cursor] Metamorphosis");


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
            int Fury = Aimsharp.Power("player");
            int Runes = Aimsharp.PlayerSecondaryPower();
            int MaxFury = Aimsharp.PlayerMaxPower();
            int FuryDefecit = MaxFury - Fury;

            //Talents
            bool TalentDemonBlades = Aimsharp.Talent(2, 3);
            bool TalentFirstBlood = Aimsharp.Talent(5, 2);
            bool TalentTrailofRuin = Aimsharp.Talent(3, 1);
            bool TalentCycleofHatred = Aimsharp.Talent(5, 1);
            bool TalentDemonic = Aimsharp.Talent(7, 1);
            bool TalentEssenceBreak = Aimsharp.Talent(5, 3);
            bool TalentBlindFury = Aimsharp.Talent(1, 1);
            bool TalentMomentum = Aimsharp.Talent(7, 2);
            bool TalentUnboundChaos = Aimsharp.Talent(3, 2);


            //buffs
            int BuffBurningWoundRemains = Aimsharp.BuffRemaining("Burning Wound") - GCD;
            bool BuffBurningWoundUp = BuffBurningWoundRemains > 0;
            int BuffChaosTheoryRemains = Aimsharp.BuffRemaining("Chaos Theory") - GCD;
            bool BuffChaosTheoryUp = BuffChaosTheoryRemains > 0;
            int BuffMetamorphosisRemains = Aimsharp.BuffRemaining("Metamorphosis") - GCD;
            bool BuffMetamorphosisUp = BuffMetamorphosisRemains > 0;
            int BuffEssenceBreakRemains = Aimsharp.BuffRemaining("Essence Break") - GCD;
            bool BuffEssenceBreakUp = BuffEssenceBreakRemains > 0;
            int BuffMomentumRemains = Aimsharp.BuffRemaining("Momentum") - GCD;
            bool BuffMomentumUp = BuffMomentumRemains > 0;
            int BuffFelBombardmentRemains = Aimsharp.BuffRemaining("Fel Bombardment") - GCD;
            bool BuffFelBombardmentUp = BuffFelBombardmentRemains > 0;
            int BuffFelBombardmentStacks = Aimsharp.BuffStacks("Fel Bombardment");
            int BuffImmolationAuraRemains = Aimsharp.BuffRemaining("Immolation Aura") - GCD;
            bool BuffImmolationAuraUp = BuffImmolationAuraRemains > 0;
            int BuffFuriousGazeRemains = Aimsharp.BuffRemaining("Furious Gaze") - GCD;
            bool BuffFuriousGazeUp = BuffFuriousGazeRemains > 0;
            int BuffUnboundChaosRemains = Aimsharp.BuffRemaining("Unbound Chaos") - GCD;
            bool BuffUnboundChaosUp = BuffUnboundChaosRemains > 0;
            int BuffExposedWoundRemains = Aimsharp.BuffRemaining("Exposed Wound") - GCD;
            bool BuffExposedWoundUp = BuffExposedWoundRemains > 0;
            int BuffPreparedRemains = Aimsharp.BuffRemaining("Prepared") - GCD;
            bool BuffPreparedUp = BuffPreparedRemains > 0;


            //debuffs
            int DebuffBurningWoundRemains = Aimsharp.DebuffRemaining("Burning Wound") - GCD;
            bool DebuffBurningWoundUp = DebuffBurningWoundRemains > 0;
            int DebuffEssenceBreakRemains = Aimsharp.DebuffRemaining("Essence Break") - GCD;
            bool DebuffEssenceBreakUp = DebuffEssenceBreakRemains > 0;
            int DebuffExposedWoundRemains = Aimsharp.DebuffRemaining("Exposed Wound") - GCD;
            bool DebuffExposedWoundUp = DebuffExposedWoundRemains > 0;
            int DotSinfulBrandRemains = Aimsharp.DebuffRemaining("Sinful Brand") - GCD;
            bool DotSinfulBrandUp = DotSinfulBrandRemains > 0;


            //cooldowns
            int CDMetamorphosisRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Metamorphosis");
            bool CDMetamorphosisUp = CDMetamorphosisRemains <= 0;
            int CDEyeBeamRemains = Aimsharp.SpellCooldown("Eye Beam");
            bool CDEyeBeamUp = CDEyeBeamRemains <= 0;
            int CDBladeDanceRemains = Aimsharp.SpellCooldown("Blade Dance");
            bool CDBladeDanceUp = CDBladeDanceRemains <= 0;
            int CDImmolationAuraRemains = Aimsharp.SpellCooldown("Immolation Aura");
            bool CDImmolationAuraUp = CDImmolationAuraRemains <= 0;
            int CDEssenceBreakRemains = Aimsharp.SpellCooldown("Essence Break");
            bool CDEssenceBreakUp = CDEssenceBreakRemains <= 0;
            int CDFelRushCharges = Aimsharp.SpellCharges("Fel Rush");


            //specific variables
            bool RuneforgeBurningWound = RuneforgePower == "Burning Wound";
            bool RuneforgeChaosTheory = RuneforgePower == "Chaos Theory";
            bool RuneforgeDarkglareMedallion = RuneforgePower == "Darkglare Medallion";
            bool ConduitSerratedGlaive = ActiveConduits.Contains(339230);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastThrowGlaive = Aimsharp.CanCast("Throw Glaive") && Fighting;
            bool CanCastMetamorphosis = Aimsharp.CanCast("Metamorphosis", "player") && !SaveCooldowns && Fighting;
            bool CanCastSinfulBrand = Aimsharp.CanCast("Sinful Brand") && !SaveCooldowns && Fighting;
            bool CanCastTheHunt = Aimsharp.CanCast("The Hunt") && !SaveCooldowns && Fighting;
            bool CanCastElysianDecree = Aimsharp.CanCast("Elysian Decree", "player") && !SaveCooldowns && Fighting;
            bool CanCastDeathSweep = Aimsharp.CanCast("Death Sweep", "player") && Fighting && BuffMetamorphosisUp;
            bool CanCastGlaiveTempest = Aimsharp.CanCast("Glaive Tempest", "player") && Fighting;
            bool CanCastEyeBeam = Aimsharp.CanCast("Eye Beam", "player") && !Moving && Fighting;
            bool CanCastBladeDance = Aimsharp.CanCast("Blade Dance", "player") && Fighting;
            bool CanCastImmolationAura = Aimsharp.CanCast("Immolation Aura", "player") && Fighting;
            bool CanCastAnnihilation = Aimsharp.CanCast("Annihilation") && Fighting && BuffMetamorphosisUp;
            bool CanCastFelblade = Aimsharp.CanCast("Felblade") && Fighting;
            bool CanCastEssenceBreak = Aimsharp.CanCast("Essence Break", "player") && Fighting;
            bool CanCastChaosStrike = Aimsharp.CanCast("Chaos Strike") && Fighting;
            bool CanCastFelRush = Aimsharp.CanCast("Fel Rush", "player") && Fighting;
            bool CanCastDemonsBite = Aimsharp.CanCast("Demon's Bite") && Fighting;
            bool CanCastVengefulRetreat = Aimsharp.CanCast("Vengeful Retreat", "player") && Fighting;
            bool CanCastFelBarrage = Aimsharp.CanCast("Fel Barrage", "player") && !Moving && Fighting;





            // end of Simc conditionals
            #endregion

            //actions.precombat+=/variable,name=use_eye_beam_fury_condition,value=talent.blind_fury.enabled&(runeforge.darkglare_medallion|talent.demon_blades.enabled)
            bool use_eye_beam_fury_condition = TalentBlindFury && (RuneforgeDarkglareMedallion || TalentDemonBlades);
            


            //never interrupt channels 
            if (IsChanneling)
                return false;

            //actions+=/retarget_auto_attack,line_cd=1,target_if=min:debuff.burning_wound.remains,if=runeforge.burning_wound&talent.demon_blades.enabled

            //actions+=/variable,name=blade_dance,if=!runeforge.chaos_theory&!runeforge.darkglare_medallion,value=talent.first_blood.enabled|spell_targets.blade_dance1>=(3-talent.trail_of_ruin.enabled)
            bool blade_dance = (!RuneforgeChaosTheory && !RuneforgeDarkglareMedallion && (TalentFirstBlood || EnemiesInMelee >= (3 - (TalentTrailofRuin ? 1 : 0)))) || (RuneforgeChaosTheory && (!BuffChaosTheoryUp || TalentFirstBlood && EnemiesInMelee >= (2 - (TalentTrailofRuin ? 1 : 0) ) || !TalentCycleofHatred && EnemiesInMelee >= (4 - (TalentTrailofRuin ? 1 : 0)))) || (RuneforgeDarkglareMedallion && (TalentFirstBlood || (BuffMetamorphosisUp || TalentTrailofRuin || DebuffEssenceBreakUp) && EnemiesInMelee >= (3 - (TalentTrailofRuin ? 1 : 0)) || !TalentDemonic && EnemiesInMelee >= 4)) || (!(TalentEssenceBreak && CDEssenceBreakUp));

            //actions+=/variable,name=blade_dance,if=runeforge.chaos_theory,value=buff.chaos_theory.down|talent.first_blood.enabled&spell_targets.blade_dance1>=(2-talent.trail_of_ruin.enabled)|!talent.cycle_of_hatred.enabled&spell_targets.blade_dance1>=(4-talent.trail_of_ruin.enabled)


            //actions+=/variable,name=blade_dance,if=runeforge.darkglare_medallion,value=talent.first_blood.enabled|(buff.metamorphosis.up|talent.trail_of_ruin.enabled|debuff.essence_break.up)&spell_targets.blade_dance1>=(3-talent.trail_of_ruin.enabled)|!talent.demonic.enabled&spell_targets.blade_dance1>=4


            //actions+=/variable,name=blade_dance,op=reset,if=talent.essence_break.enabled&cooldown.essence_break.ready

            //actions+=/variable,name=pooling_for_meta,value=!talent.demonic.enabled&cooldown.metamorphosis.remains<6&fury.deficit>30
            bool pooling_for_meta = !TalentDemonic && CDMetamorphosisRemains > 6000 && FuryDefecit > 30;

            //actions+=/variable,name=pooling_for_blade_dance,value=variable.blade_dance&(fury<75-talent.first_blood.enabled*20)
            bool pooling_for_blade_dance = blade_dance && (Fury < 75 - (TalentFirstBlood ? 20 : 0));

            //actions+=/variable,name=pooling_for_eye_beam,value=talent.demonic.enabled&!talent.blind_fury.enabled&cooldown.eye_beam.remains<(gcd.max*2)&fury.deficit>20
            bool pooling_for_eye_beam = TalentDemonic && !TalentBlindFury && CDEyeBeamRemains < (GCDMAX * 2) && FuryDefecit > 20;

            //actions+=/variable,name=waiting_for_momentum,value=talent.momentum.enabled&!buff.momentum.up
            bool waiting_for_momentum = TalentMomentum && !BuffMomentumUp;  


            //actions+=/call_action_list,name=cooldown,if=gcd.remains=0
            //actions.cooldown=metamorphosis,if=!talent.demonic.enabled&cooldown.eye_beam.remains>20&(!covenant.venthyr.enabled|!dot.sinful_brand.ticking)|fight_remains<25
            if (CanCastMetamorphosis)
            {
                if (!TalentDemonic && CDEyeBeamRemains > 20000 && (!CovenantVenthyr || !DotSinfulBrandUp) || TargetTimeToDie < 25000)
                {
                    return Metamorphosis();
                }
            }

            //actions.cooldown+=/metamorphosis,if=talent.demonic.enabled&(cooldown.eye_beam.remains>20&(!variable.blade_dance|cooldown.blade_dance.remains>gcd.max))&(!covenant.venthyr.enabled|!dot.sinful_brand.ticking)|fight_remains<25
            if (CanCastMetamorphosis)
            {
                if (TalentDemonic && (CDEyeBeamRemains > 20000 && (!blade_dance || CDBladeDanceRemains > GCDMAX)) && (!CovenantVenthyr || !DotSinfulBrandUp) || TargetTimeToDie < 25000)
                {
                    return Metamorphosis();
                }
            }

            foreach (string Racial in Racials)
            {
                if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns && BuffMetamorphosisRemains > 15000)
                {
                    Aimsharp.Cast(Racial, true);
                    return true;
                }
            }

            //actions.cooldown+=/potion,if=buff.metamorphosis.remains>25|fight_remains<60
            if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns && BuffMetamorphosisRemains > 25000)
            {
                Aimsharp.Cast("DPS Pot", true);
                return true;
            }

            //actions.cooldown+=/use_items,slots=trinket1,if=variable.trinket_sync_slot=1&(buff.metamorphosis.up|(!talent.demonic.enabled&cooldown.metamorphosis.remains>(fight_remains>?trinket.1.cooldown.duration%2))|fight_remains<=20)|(variable.trinket_sync_slot=2&!trinket.2.cooldown.ready)|!variable.trinket_sync_slot
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

            //actions.cooldown+=/sinful_brand,if=!dot.sinful_brand.ticking
            if (CanCastSinfulBrand)
            {
                if (!DotSinfulBrandUp)
                {
                    return SinfulBrand();
                }
            }

            //actions.cooldown+=/the_hunt,if=!talent.demonic.enabled&!variable.waiting_for_momentum&!variable.pooling_for_meta|buff.furious_gaze.up
            if (CanCastTheHunt)
            {
                if (!TalentDemonic && !waiting_for_momentum && !pooling_for_meta || BuffFuriousGazeUp)
                {
                    return TheHunt();
                }
            }

            //actions.cooldown+=/elysian_decree,if=(active_enemies>desired_targets|raid_event.adds.in>30)
            if (CanCastElysianDecree)
            {
                if (AOE ? EnemiesInMelee > 1 : true)
                    return ElysianDecree();
            }

            //actions+=/throw_glaive,if=buff.fel_bombardment.stack=5&(buff.immolation_aura.up|!buff.metamorphosis.up)
            if (CanCastThrowGlaive)
            {
                if (BuffFelBombardmentStacks == 5 && (BuffImmolationAuraUp || !BuffMetamorphosisUp))
                    return ThrowGlaive();
            }

            //actions+=/run_action_list,name=demonic,if=talent.demonic.enabled
            //actions.demonic=fel_rush,if=talent.unbound_chaos.enabled&buff.unbound_chaos.up&(charges=2|(raid_event.movement.in>10&raid_event.adds.in>10))
            if (CanCastFelRush)
            {
                if (TalentUnboundChaos && BuffUnboundChaosUp && (CDFelRushCharges == 2))
                    return FelRush();
            }

            //actions.demonic+=/death_sweep,if=variable.blade_dance
            if (CanCastDeathSweep)
            {
                if (blade_dance)
                    return DeathSweep();
            }

            //actions.demonic+=/glaive_tempest,if=active_enemies>desired_targets|raid_event.adds.in>10
            if (CanCastGlaiveTempest)
            {
                if (AOE ? EnemiesInMelee > 1 : true)
                {
                    return GlaiveTempest();
                }
            }

            //actions.demonic+=/throw_glaive,if=conduit.serrated_glaive.enabled&cooldown.eye_beam.remains<6&!buff.metamorphosis.up&!debuff.exposed_wound.up
            if (CanCastThrowGlaive)
            {
                if (ConduitSerratedGlaive && CDEyeBeamRemains < 6000 && !BuffMetamorphosisUp && !DebuffExposedWoundUp)
                    return ThrowGlaive();
            }

            //actions.demonic+=/eye_beam,if=active_enemies>desired_targets|raid_event.adds.in>25&(!variable.use_eye_beam_fury_condition|spell_targets>1|fury<70)
            if (CanCastEyeBeam)
            {
                if (AOE ? EnemiesInMelee > 1 : true && (!use_eye_beam_fury_condition || EnemiesInMelee > 1 || Fury < 70))
                    return EyeBeam();
            }

            //actions.demonic+=/blade_dance,if=variable.blade_dance&!cooldown.metamorphosis.ready&(cooldown.eye_beam.remains>5|(raid_event.adds.in>cooldown&raid_event.adds.in<25))
            if (CanCastBladeDance)
            {
                if (blade_dance && !CDMetamorphosisUp && (CDEyeBeamRemains > 5000))
                    return BladeDance();
            }

            //actions.demonic+=/immolation_aura,if=!buff.immolation_aura.up
            if (CanCastImmolationAura)
            {
                if (!BuffImmolationAuraUp)
                    return ImmolationAura();
            }

            //actions.demonic+=/annihilation,if=!variable.pooling_for_blade_dance
            if (CanCastAnnihilation)
            {
                if (!pooling_for_blade_dance)
                    return Annihilation();
            }

            //actions.demonic+=/felblade,if=fury.deficit>=40
            if (CanCastFelblade)
            {
                if (FuryDefecit >= 40)
                    return Felblade();
            }

            //actions.demonic+=/essence_break
            if (CanCastEssenceBreak)
            {
                return EssenceBreak();
            }

            //actions.demonic+=/chaos_strike,if=!variable.pooling_for_blade_dance&!variable.pooling_for_eye_beam
            if (CanCastChaosStrike)
            {
                if (!pooling_for_blade_dance && !pooling_for_eye_beam)
                    return ChaosStrike();
            }

            //actions.demonic+=/fel_rush,if=talent.demon_blades.enabled&!cooldown.eye_beam.ready&(charges=2|(raid_event.movement.in>10&raid_event.adds.in>10))
            if (CanCastFelRush)
            {
                if (TalentDemonBlades && !CDEyeBeamUp && (CDFelRushCharges == 2))
                    return FelRush();
            }

            //actions.demonic+=/demons_bite,target_if=min:debuff.burning_wound.remains,if=runeforge.burning_wound&debuff.burning_wound.remains<4
            if (CanCastDemonsBite)
            {
                if (RuneforgeBurningWound && DebuffBurningWoundRemains < 4000)
                    return DemonsBite();
            }

            //actions.demonic+=/fel_rush,if=!talent.demon_blades.enabled&spell_targets>1&(charges=2|(raid_event.movement.in>10&raid_event.adds.in>10))
            if (CanCastFelRush)
            {
                if (!TalentDemonBlades && EnemiesInMelee > 1 && (CDFelRushCharges == 2))
                    return FelRush();
            }

            //actions.demonic+=/demons_bite
            if (CanCastDemonsBite)
                return DemonsBite();

            //actions.demonic+=/throw_glaive,if=buff.out_of_range.up
            if (RangeToTarget > 10)
            {
                if (CanCastThrowGlaive)
                    return ThrowGlaive();
            }

            //actions.demonic+=/throw_glaive,if=talent.demon_blades.enabled
            if (CanCastThrowGlaive && TalentDemonBlades)
                return ThrowGlaive();

            //actions+=/run_action_list,name=normal
            //actions.normal+=/fel_rush,if=(buff.unbound_chaos.up|variable.waiting_for_momentum&(!talent.unbound_chaos.enabled|!cooldown.immolation_aura.ready))&(charges=2|(raid_event.movement.in>10&raid_event.adds.in>10))
            if (CanCastFelRush)
            {
                if ((BuffUnboundChaosUp || waiting_for_momentum && (!TalentUnboundChaos || !CDImmolationAuraUp)) && (CDFelRushCharges == 2))
                    return FelRush();
            }

            //actions.normal+=/fel_barrage,if=active_enemies>desired_targets|raid_event.adds.in>30
            if (CanCastFelBarrage)
            {
                if (AOE ? EnemiesInMelee > 1 : true)
                    return FelBarrage();
            }

            //actions.normal+=/death_sweep,if=variable.blade_dance
            if (CanCastDeathSweep && blade_dance)
                return DeathSweep();

            //actions.normal+=/immolation_aura,if=!buff.immolation_aura.up
            if (CanCastImmolationAura && !BuffImmolationAuraUp)
                return ImmolationAura();

            //actions.normal+=/glaive_tempest,if=!variable.waiting_for_momentum&(active_enemies>desired_targets|raid_event.adds.in>10)
            if (CanCastGlaiveTempest)
            {
                if (!waiting_for_momentum && AOE ? EnemiesInMelee > 1 : true)
                    return GlaiveTempest();
            }

            //actions.normal+=/throw_glaive,if=conduit.serrated_glaive.enabled&cooldown.eye_beam.remains<6&!buff.metamorphosis.up&!debuff.exposed_wound.up
            if (CanCastThrowGlaive)
            {
                if (ConduitSerratedGlaive && CDEyeBeamRemains < 6000 && !BuffMetamorphosisUp && !DebuffExposedWoundUp)
                    return ThrowGlaive();
            }

            //actions.normal+=/eye_beam,if=!variable.waiting_for_momentum&(active_enemies>desired_targets|raid_event.adds.in>15&(!variable.use_eye_beam_fury_condition|spell_targets>1|fury<70))
            if (CanCastEyeBeam)
            {
                if (!waiting_for_momentum && (AOE ? EnemiesInMelee > 1 : true && (!use_eye_beam_fury_condition || EnemiesInMelee > 1 || Fury < 70)))
                    return EyeBeam();
            }

            //actions.normal+=/blade_dance,if=variable.blade_dance
            if (CanCastBladeDance && blade_dance)
            {
                return BladeDance();
            }

            //actions.normal+=/felblade,if=fury.deficit>=40
            if (CanCastFelblade && FuryDefecit >= 40)
                return Felblade();

            //actions.normal+=/essence_break
            if (CanCastEssenceBreak)
                return EssenceBreak();

            //actions.normal+=/annihilation,if=(talent.demon_blades.enabled|!variable.waiting_for_momentum|fury.deficit<30|buff.metamorphosis.remains<5)&!variable.pooling_for_blade_dance
            if (CanCastAnnihilation)
            {
                if ((TalentDemonBlades || !waiting_for_momentum || FuryDefecit < 30 || BuffMetamorphosisRemains < 5000) && !pooling_for_blade_dance)
                    return Annihilation();
            }

            //actions.normal+=/chaos_strike,if=(talent.demon_blades.enabled|!variable.waiting_for_momentum|fury.deficit<30)&!variable.pooling_for_meta&!variable.pooling_for_blade_dance
            if (CanCastChaosStrike)
            {
                if ((TalentDemonBlades || !waiting_for_momentum || FuryDefecit < 30) && !pooling_for_meta && !pooling_for_blade_dance)
                    return ChaosStrike();
            }

            //actions.normal+=/eye_beam,if=talent.blind_fury.enabled&raid_event.adds.in>cooldown
            if (CanCastEyeBeam)
            {
                if (TalentBlindFury)
                    return EyeBeam();
            }

            //actions.normal+=/demons_bite,target_if=min:debuff.burning_wound.remains,if=runeforge.burning_wound&debuff.burning_wound.remains<4
            if (CanCastDemonsBite)
            {
                if (RuneforgeBurningWound && DebuffBurningWoundRemains < 4000)
                {
                    return DemonsBite();
                }
            }

            //actions.normal+=/demons_bite
            if (CanCastDemonsBite)
                return DemonsBite();

            //actions.normal+=/fel_rush,if=!talent.momentum.enabled&raid_event.movement.in>charges*10&talent.demon_blades.enabled
            if (CanCastFelRush)
            {
                if (!TalentMomentum && TalentDemonBlades)
                    return FelRush();
            }

            if (CanCastThrowGlaive)
                return ThrowGlaive();


            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

        bool ThrowGlaive() { Aimsharp.Cast("Throw Glaive"); return true; }
        bool Metamorphosis() { Aimsharp.Cast("MetamorphosisC"); return true; }
        bool SinfulBrand() { Aimsharp.Cast("Sinful Brand"); return true; }
        bool TheHunt() { Aimsharp.Cast("The Hunt"); return true; }
        bool ElysianDecree() { Aimsharp.Cast("ElysianDecreeC"); return true; }
        bool DeathSweep() { Aimsharp.Cast("Death Sweep"); return true; }
        bool GlaiveTempest() { Aimsharp.Cast("Glaive Tempest"); return true; }
        bool EyeBeam() { Aimsharp.Cast("Eye Beam"); return true; }
        bool BladeDance() { Aimsharp.Cast("Blade Dance"); return true; }
        bool ImmolationAura() { Aimsharp.Cast("Immolation Aura"); return true; }
        bool Annihilation() { Aimsharp.Cast("Annihilation"); return true; }
        bool Felblade() { Aimsharp.Cast("Felblade"); return true; }
        bool EssenceBreak() { Aimsharp.Cast("Essence Break"); return true; }
        bool ChaosStrike() { Aimsharp.Cast("Chaos Strike"); return true; }
        bool FelRush() { Aimsharp.Cast("Fel Rush"); return true; }
        bool DemonsBite() { Aimsharp.Cast("Demon's Bite"); return true; }
        bool VengefulRetreat() { Aimsharp.Cast("Vengeful Retreat"); return true; }
        bool FelBarrage() { Aimsharp.Cast("Fel Barrage"); return true; }


    }
}
