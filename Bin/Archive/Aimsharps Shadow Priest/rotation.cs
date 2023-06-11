using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class SLShadowPriest : Rotation
    {
        List<string> Racials = new List<string>
        {
            "Blood Fury","Berserking","Fireblood","Ancestral Call","Bag of Tricks"
        };

        List<string> CovenantAbilities = new List<string>
        {

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
            "Shadowform","Vampiric Touch","Searing Nightmare","Mind Blast","Void Eruption","Shadow Word: Pain","Mind Sear","Damnation","Void Bolt","Devouring Plague",
            "Shadow Word: Death","Surrender to Madness","Mindbender","Shadowfiend","Void Torrent","Shadow Crash","Mind Flay","Power Infusion","Power Word: Shield"
        };

        List<string> BuffsList = new List<string>
        {
            "Voidform","Dark Thoughts","Unfurling Darkness","Shadowform","Power Word: Shield","Fae Guardians"
        };

        List<string> DebuffsList = new List<string>
        {
            "Shadow Word: Pain","Vampiric Touch","Devouring Plague","Shadow Crash","Wrathful Faerie"
        };

        List<string> TotemsList = new List<string>
        {
            "Shadowfiend","Mindbender"
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

            Settings.Add(new Setting("Priest Settings"));
            Settings.Add(new Setting("Shield self:", true));
            Settings.Add(new Setting("Power Infusion only self:", true));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Twins of the Sun Priestess", "Painbreaker Psalm", "Talbadar's Stratagem", "Shadowflame Prism" }, "None"));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Shadow Priest", Color.Purple);
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

            Macros.Add("Crash Cursor", "/cast [@cursor] Shadow Crash");
            Macros.Add("PWS Self", "/cast [@player] Power Word: Shield");
            Macros.Add("PI Self", "/cast [@player] Power Infusion");

            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            //counts number of SW:Ps ticking on nearby enemies
            //CustomFunctions.Add("SWP Count", "local SWPCount = 0\nfor i=1,20 do\nlocal unit = \"nameplate\" .. i\nif UnitExists(unit) then\nif UnitCanAttack(\"player\", unit) then\nfor j = 1, 40 do\nlocal name,_,_,_,_,_,source = UnitDebuff(unit, j)\nif name == \"Shadow Word: Pain\" and source == \"player\" then\nSWPCount = SWPCount + 1\nend\nend\nend\nend\nend\nreturn SWPCount");

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

            // Priest power
            int Insanity = Aimsharp.Power("player");

            // Priest Talents
            bool TalentSearingNightmare = Aimsharp.Talent(3, 3);
            bool TalentHungeringVoid = Aimsharp.Talent(7, 2);
            bool TalentTwistOfFate = Aimsharp.Talent(3, 1);
            bool TalentMisery = Aimsharp.Talent(3, 2);
            bool TalentPsychicLink = Aimsharp.Talent(5, 2);

            // Priest buffs
            int BuffVoidformRemaining = Aimsharp.BuffRemaining("Voidform") - GCD;
            bool BuffVoidformUp = BuffVoidformRemaining > 0;
            int BuffDarkThoughtsRemaining = Aimsharp.BuffRemaining("Dark Thoughts") - GCD;
            bool BuffDarkThoughtsUp = BuffDarkThoughtsRemaining > 0;
            int BuffUnfurlingDarknessRemaining = Aimsharp.BuffRemaining("Unfurling Darkness") - GCD;
            bool BuffUnfurlingDarknessUp = BuffUnfurlingDarknessRemaining > 0;
            int BuffFaeGuardiansRemaining = Aimsharp.BuffRemaining("Fae Guardians") - GCD;
            bool BuffFaeGuardiansUp = BuffFaeGuardiansRemaining > 0;
            int BuffPowerWordShieldRemaining = Aimsharp.BuffRemaining("Power Word: Shield") - Aimsharp.GCD();
            bool BuffPowerWordShieldUp = BuffPowerWordShieldRemaining > 0;

            // Priest debuffs
            int DotShadowWordPainRemaining = Aimsharp.DebuffRemaining("Shadow Word: Pain") - GCD;
            bool DotShadowWordPainTicking = DotShadowWordPainRemaining > 0;
            bool DotShadowWordPainRefreshable = DotShadowWordPainRemaining <= 3600;
            int DotVampiricTouchRemaining = Aimsharp.DebuffRemaining("Vampiric Touch") - GCD;
            bool DotVampiricTouchTicking = DotVampiricTouchRemaining > 0;
            bool DotVampiricTouchRefreshable = DotVampiricTouchRemaining <= 6300;
            int DotDevouringPlagueRemaining = Aimsharp.DebuffRemaining("Devouring Plague") - GCD;
            bool DotDevouringPlagueTicking = DotDevouringPlagueRemaining > 0;
            bool DotDevouringPlagueRefreshable = DotDevouringPlagueRemaining <= 1800;
            int DebuffShadowCrashRemaining = Aimsharp.DebuffRemaining("Shadow Crash") - GCD;
            bool DebuffShadowCrashUp = DebuffShadowCrashRemaining > 0;
            int DebuffWrathfulFaerieRemaining = Aimsharp.DebuffRemaining("Wrathful Faerie") - GCD;
            bool DebuffWrathfulFaerieUp = DebuffWrathfulFaerieRemaining > 0;


            // Priest cooldowns
            int CDPowerInfusionRemaining = Aimsharp.SpellCooldown("Power Infusion");
            bool CDPowerInfusionUp = CDPowerInfusionRemaining <= 0;
            int CDVoidEruptionRemaining = Aimsharp.SpellCooldown("Void Eruption") - GCD;
            bool CDVoidEruptionUp = CDVoidEruptionRemaining <= 0;
            int CDShadowfiendRemaining = Aimsharp.SpellCooldown("Shadowfiend");
            bool CDShadowfiendUp = CDShadowfiendRemaining <= 0;

            // Priest specific variables
            bool ChannelingMindFlay = Aimsharp.CastingID("player") == 15407;
            bool ChannelingMindSear = Aimsharp.CastingID("player") == 48045;
            bool CastingVT = Aimsharp.CastingID("player") == 34914;
            bool CastingMindBlast = Aimsharp.CastingID("player") == 8092;
            bool PetFiendActive = Aimsharp.TotemRemaining("Shadowfiend") > GCD || Aimsharp.TotemRemaining("Mindbender") > GCD;
            int CDShadowCrashCharges = Aimsharp.SpellCharges("Shadow Crash");
            bool PowerInfusionSelf = GetCheckBox("Power Infusion only self:");
            bool ShieldSelf = GetCheckBox("Shield self:");

            int CastingRemaining = Aimsharp.CastingRemaining("player");
            //predictions
            if (CastingVT && CastingRemaining < 500)
            {
                DotVampiricTouchRefreshable = false;
                DotVampiricTouchRemaining = 20000;
                DotVampiricTouchTicking = true;
                Insanity = Insanity + 5;
            }
            if (CastingMindBlast && CastingRemaining < 500)
            {
                Insanity = Insanity + 8;
            }

            // end of Simc conditionals
            #endregion

            //never interrupt channels except Mind Flay and Mind Sear
            if (IsChanneling && !ChannelingMindFlay && !ChannelingMindSear)
                return false;

            //actions=potion,if=buff.bloodlust.react|target.time_to_die<=80|target.health.pct<35
            if (BloodlustUp || TargetTimeToDie <= 80000 || TargetHealthPct < 35)
            {
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false))
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //actions+=/variable,name=dots_up,op=set,value=dot.shadow_word_pain.ticking&dot.vampiric_touch.ticking
            bool dots_up = DotShadowWordPainTicking && DotVampiricTouchTicking;

            //actions+=/variable,name=all_dots_up,op=set,value=dot.shadow_word_pain.ticking&dot.vampiric_touch.ticking&dot.devouring_plague.ticking
            bool all_dots_up = dots_up && DotDevouringPlagueTicking;

            //actions+=/variable,name=searing_nightmare_cutoff,op=set,value=spell_targets.mind_sear>3
            bool searing_nightmare_cutoff = EnemiesNearTarget > 3;

            //actions+=/variable,name=pi_or_vf_sync_condition,op=set,value=(priest.self_power_infusion|runeforge.twins_of_the_sun_priestess.equipped)&level>=58&cooldown.power_infusion.up|(level<58|!priest.self_power_infusion&!runeforge.twins_of_the_sun_priestess.equipped)&cooldown.void_eruption.up
            bool pi_or_vf_sync_condition = (PowerInfusionSelf || RuneforgePower == "Twins of the Sun Priestess") && PlayerLevel >= 58 && CDPowerInfusionUp || (PlayerLevel < 58 || !PowerInfusionSelf && RuneforgePower != "Twins of the Sun Priestess") && CDVoidEruptionUp;

            //actions+=/call_action_list,name=cwc
            //actions.cwc=searing_nightmare,use_while_casting=1,target_if=(variable.searing_nightmare_cutoff&!variable.pi_or_vf_sync_condition)|(dot.shadow_word_pain.refreshable&spell_targets.mind_sear>1)
            if (Aimsharp.CanCast("Searing Nightmare", "player") && ChannelingMindSear && !Moving)
            {
                if ((searing_nightmare_cutoff && !pi_or_vf_sync_condition) || (DotShadowWordPainRefreshable && EnemiesNearTarget > 1))
                {
                    Aimsharp.Cast("Searing Nightmare");
                    return true;
                }
            }

            //actions.cwc+=/searing_nightmare,use_while_casting=1,target_if=talent.searing_nightmare.enabled&dot.shadow_word_pain.refreshable&spell_targets.mind_sear>2
            if (Aimsharp.CanCast("Searing Nightmare", "player") && ChannelingMindSear && !Moving)
            {
                if (TalentSearingNightmare && DotShadowWordPainRefreshable && EnemiesNearTarget > 2)
                {
                    Aimsharp.Cast("Searing Nightmare");
                    return true;
                }
            }

            //actions.cwc+=/mind_blast,only_cwc=1
            if (Aimsharp.CanCast("Mind Blast") && (ChannelingMindFlay || ChannelingMindSear) && !Moving)
            {
                Aimsharp.Cast("Mind Blast");
                return true;
            }

            //actions+=/run_action_list,name=main
            //actions.main=void_eruption,if=variable.pi_or_vf_sync_condition&insanity>=40
            if (Aimsharp.CanCast("Void Eruption") && !Moving)
            {
                if (pi_or_vf_sync_condition && Insanity >= 40)
                {
                    Aimsharp.Cast("Void Eruption");
                    return true;
                }
            }

            //pi
            if (PowerInfusionSelf && Aimsharp.CanCast("Power Infusion", "player") && BuffVoidformUp)
            {
                Aimsharp.Cast("PI Self");
                return true;
            }

            //actions.main+=/shadow_word_pain,if=buff.fae_guardians.up&!debuff.wrathful_faerie.up
            if (Aimsharp.CanCast("Shadow Word: Pain"))
            {
                if (BuffFaeGuardiansUp && !DebuffWrathfulFaerieUp)
                {
                    Aimsharp.Cast("Shadow Word: Pain");
                    return true;
                }
            }

            //actions.main+=/call_action_list,name=cds
            if (!SaveCooldowns)
            {
                //actions.cds=silence,target_if=runeforge.sephuzs_proclamation.equipped&(target.is_add|target.debuff.casting.react)
                //Rotation does not auto silence

                //actions.cds+=/call_action_list,name=essences
                foreach (string Essence in Essences)
                {
                    if (Aimsharp.CanCast(Essence, "player") && !Moving && Fighting)
                    {
                        Aimsharp.Cast(Essence);
                        return true;
                    }
                }

                foreach (string Essence in EssencesTargeted)
                {
                    if (Essence == "Concentrated Flame")
                    {
                        if (Aimsharp.CanCast(Essence) && Fighting && !Moving && ConcentratedFlameFullRecharge < GCDMAX)
                        {
                            Aimsharp.Cast(Essence);
                            return true;
                        }
                    }
                    else if (Aimsharp.CanCast(Essence) && Fighting && !Moving)
                    {
                        Aimsharp.Cast(Essence);
                        return true;
                    }
                }

                //actions.cds+=/use_items
                if (Aimsharp.CanUseTrinket(0) && UseTopTrinket && Fighting && !Moving)
                {
                    Aimsharp.Cast("TopTrinket", true);
                    return true;
                }

                if (Aimsharp.CanUseTrinket(1) && UseBottomTrinket && Fighting && !Moving)
                {
                    Aimsharp.Cast("BottomTrinket", true);
                    return true;
                }

                //actions.cds+=/use_racials
                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && !Moving && Fighting)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }
            }

            //actions.main+=/mind_sear,target_if=talent.searing_nightmare.enabled&spell_targets.mind_sear>(variable.mind_sear_cutoff+1)&!dot.shadow_word_pain.ticking&!cooldown.mindbender.up
            if (Aimsharp.CanCast("Mind Sear") && !Moving && Fighting)
            {
                if (TalentSearingNightmare && EnemiesNearTarget > (2) && !DotShadowWordPainTicking && !CDShadowfiendUp)
                {
                    Aimsharp.Cast("Mind Sear");
                    return true;
                }
            }

            //actions.main+=/damnation,target_if=!variable.all_dots_up
            if (Aimsharp.CanCast("Damnation") && Fighting)
            {
                if (!all_dots_up)
                {
                    Aimsharp.Cast("Damnation");
                    return true;
                }
            }

            //actions.main+=/void_bolt,if=insanity<=85&((talent.hungering_void.enabled&spell_targets.mind_sear<5)|spell_targets.mind_sear=1)
            if (Aimsharp.CanCast("Void Bolt") && BuffVoidformUp && Fighting)
            {
                if (Insanity <= 85 && ((TalentHungeringVoid && EnemiesNearTarget < 5) || EnemiesNearTarget == 1))
                {
                    Aimsharp.Cast("Void Bolt");
                    return true;
                }
            }

            //actions.main+=/devouring_plague,target_if=(refreshable|insanity>75)&!variable.pi_or_vf_sync_condition&(!talent.searing_nightmare.enabled|(talent.searing_nightmare.enabled&!variable.searing_nightmare_cutoff))
            if (Aimsharp.CanCast("Devouring Plague") && Fighting)
            {
                if ((DotDevouringPlagueRefreshable || Insanity > 75) && !pi_or_vf_sync_condition && (!TalentSearingNightmare || (TalentSearingNightmare && !searing_nightmare_cutoff)))
                {
                    Aimsharp.Cast("Devouring Plague");
                    return true;
                }
            }

            //actions.main+=/void_bolt,if=spell_targets.mind_sear<(4+conduit.dissonant_echoes.enabled)&insanity<=85
            if (Aimsharp.CanCast("Void Bolt") && BuffVoidformUp && Fighting)
            {
                if (EnemiesNearTarget < (4) && Insanity <= 85)
                {
                    Aimsharp.Cast("Void Bolt");
                    return true;
                }
            }

            //actions.main+=/shadow_word_death,target_if=(target.health.pct<20&spell_targets.mind_sear<4)|(pet.fiend.active&runeforge.shadowflame_prism.equipped)
            if (Aimsharp.CanCast("Shadow Word: Death") && Fighting)
            {
                if ((TargetHealthPct < 20 && EnemiesNearTarget < 4) || (PetFiendActive && RuneforgePower == "Shadowflame Prism"))
                {
                    Aimsharp.Cast("Shadow Word: Death");
                    return true;
                }
            }

            //actions.main+=/surrender_to_madness,target_if=target.time_to_die<25&buff.voidform.down
            if (Aimsharp.CanCast("Surrender to Madness") && !Moving && Fighting)
            {
                if (TargetTimeToDie < 25000 && !BuffVoidformUp)
                {
                    Aimsharp.Cast("Surrender to Madness");
                    return true;
                }
            }

            //actions.main+=/mindbender,if=dot.vampiric_touch.ticking&((talent.searing_nightmare.enabled&spell_targets.mind_sear>(variable.mind_sear_cutoff+1))|dot.shadow_word_pain.ticking)
            if (Aimsharp.CanCast("Shadowfiend") && !Moving && Fighting)
            {
                if (DotVampiricTouchTicking && ((TalentSearingNightmare && EnemiesNearTarget > (2)) || DotShadowWordPainTicking))
                {
                    Aimsharp.Cast("Shadowfiend");
                    return true;
                }
            }

            //actions.main+=/void_torrent,target_if=variable.dots_up&target.time_to_die>4&buff.voidform.down&spell_targets.mind_sear<(5+(6*talent.twist_of_fate.enabled))
            if (Aimsharp.CanCast("Void Torrent") && !Moving && Fighting)
            {
                if (dots_up && TargetTimeToDie > 4000 && !BuffVoidformUp && EnemiesNearTarget < (5 + (6 * (TalentTwistOfFate ? 1 : 0))))
                {
                    Aimsharp.Cast("Void Torrent");
                    return true;
                }
            }

            //actions.main+=/shadow_word_death,if=runeforge.painbreaker_psalm.equipped&variable.dots_up&target.time_to_pct_20>(cooldown.shadow_word_death.duration+gcd)
            if (Aimsharp.CanCast("Shadow Word: Death") && Fighting)
            {
                if (RuneforgePower == "Painbreaker Psalm" && dots_up)
                {
                    Aimsharp.Cast("Shadow Word: Death");
                    return true;
                }
            }

            //actions.main+=/shadow_crash,if=spell_targets.shadow_crash=1&(cooldown.shadow_crash.charges=3|debuff.shadow_crash_debuff.up|action.shadow_crash.in_flight|target.time_to_die<cooldown.shadow_crash.full_recharge_time)&raid_event.adds.in>30
            if (Aimsharp.CanCast("Shadow Crash", "player") && Fighting)
            {
                if (EnemiesNearTarget == 1 && (CDShadowCrashCharges == 3 || DebuffShadowCrashUp))
                {
                    Aimsharp.Cast("Crash Cursor");
                    return true;
                }
            }

            //actions.main+=/shadow_crash,if=raid_event.adds.in>30&spell_targets.shadow_crash>1
            if (Aimsharp.CanCast("Shadow Crash", "player") && Fighting)
            {
                if (EnemiesNearTarget > 1)
                {
                    Aimsharp.Cast("Crash Cursor");
                    return true;
                }
            }

            //actions.main+=/mind_flay,if=buff.dark_thoughts.up&variable.dots_up,chain=1,interrupt_immediate=1,interrupt_if=ticks>=2&cooldown.void_bolt.up
            if (Aimsharp.CanCast("Mind Flay") && !Moving && Fighting)
            {
                if (BuffDarkThoughtsUp)
                {
                    Aimsharp.Cast("Mind Flay");
                    return true;
                }
            }

            //actions.main+=/mind_blast,if=variable.dots_up&raid_event.movement.in>cast_time+0.5&spell_targets.mind_sear<4
            if (Aimsharp.CanCast("Mind Blast") && !Moving && Fighting)
            {
                if (dots_up && EnemiesNearTarget < 4)
                {
                    Aimsharp.Cast("Mind Blast");
                    return true;
                }
            }

            //actions.main+=/vampiric_touch,target_if=refreshable&target.time_to_die>6|(talent.misery.enabled&dot.shadow_word_pain.refreshable)|buff.unfurling_darkness.up
            if (Aimsharp.CanCast("Vampiric Touch") && (BuffUnfurlingDarknessUp || !CastingVT && !Moving) && Fighting) //!CastingVT to prevent double casting VT
            {
                if (DotVampiricTouchRefreshable && TargetTimeToDie > 6000 || (TalentMisery && DotShadowWordPainRefreshable) || BuffUnfurlingDarknessUp)
                {
                    Aimsharp.Cast("Vampiric Touch");
                    return true;
                }
            }

            //actions.main+=/shadow_word_pain,if=refreshable&target.time_to_die>4&!talent.misery.enabled&talent.psychic_link.enabled&spell_targets.mind_sear>2
            if (Aimsharp.CanCast("Shadow Word: Pain") && Fighting)
            {
                if (DotShadowWordPainRefreshable && TargetTimeToDie > 4000 && !TalentMisery && TalentPsychicLink && EnemiesNearTarget > 2)
                {
                    Aimsharp.Cast("Shadow Word: Pain");
                    return true;
                }
            }

            //actions.main+=/shadow_word_pain,target_if=refreshable&target.time_to_die>4&!talent.misery.enabled&!(talent.searing_nightmare.enabled&spell_targets.mind_sear>(variable.mind_sear_cutoff+1))&(!talent.psychic_link.enabled|(talent.psychic_link.enabled&spell_targets.mind_sear<=2))
            if (Aimsharp.CanCast("Shadow Word: Pain") && Fighting)
            {
                if (DotShadowWordPainRefreshable && TargetTimeToDie > 4000 && !TalentMisery && !(TalentSearingNightmare && EnemiesNearTarget > 2) && (!TalentPsychicLink || (TalentPsychicLink && EnemiesNearTarget <= 2)))
                {
                    Aimsharp.Cast("Shadow Word: Pain");
                    return true;
                }
            }

            //actions.main+=/mind_sear,target_if=spell_targets.mind_sear>variable.mind_sear_cutoff,chain=1,interrupt_immediate=1,interrupt_if=ticks>=2
            if (Aimsharp.CanCast("Mind Sear", "target", true, true) && !Moving && Fighting)
            {
                if (EnemiesNearTarget > 1)
                {
                    Aimsharp.Cast("Mind Sear");
                    return true;
                }
            }

            //actions.main+=/mind_flay,chain=1,interrupt_immediate=1,interrupt_if=ticks>=2&cooldown.void_bolt.up
            if (Aimsharp.CanCast("Mind Flay", "target", true, true) && !Moving && Fighting)
            {
                Aimsharp.Cast("Mind Flay");
                return true;
            }

            //actions.main+=/shadow_word_death
            if (Aimsharp.CanCast("Shadow Word: Death", "target", true, true) && Fighting)
            {
                Aimsharp.Cast("Shadow Word: Death");
                return true;
            }

            if (Aimsharp.CanCast("Power Word: Shield", "player") && !BuffPowerWordShieldUp && ShieldSelf)
            {
                Aimsharp.Cast("PWS Self");
                return true;
            }

            //actions.main+=/shadow_word_pain
            if (Aimsharp.CanCast("Shadow Word: Pain", "target", true, true) && Fighting)
            {
                Aimsharp.Cast("Shadow Word: Pain");
                return true;
            }

            return false;
        }


        public override bool OutOfCombatTick()
        {
            bool BuffShadowformUp = Aimsharp.HasBuff("Shadowform");
            int BuffPowerWordShieldRemaining = Aimsharp.BuffRemaining("Power Word: Shield") - Aimsharp.GCD();
            bool BuffPowerWordShieldUp = BuffPowerWordShieldRemaining > 0;

            bool ShieldSelf = GetCheckBox("Shield self:");

            if (Aimsharp.CanCast("Shadowform", "player") && !BuffShadowformUp)
            {
                Aimsharp.Cast("Shadowform");
                return true;
            }

            if (Aimsharp.CanCast("Power Word: Shield", "player") && !BuffPowerWordShieldUp && ShieldSelf)
            {
                Aimsharp.Cast("PWS Self");
                return true;
            }

            return false;
        }

    }
}
