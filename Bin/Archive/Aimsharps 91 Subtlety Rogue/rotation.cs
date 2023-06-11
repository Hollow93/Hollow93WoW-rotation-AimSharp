using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsSub : Rotation
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
           "Stealth","Marked for Death","Slice and Dice","Shadow Blades","Shuriken Storm","Echoing Reprimand","Serrated Bone Spike","Gloomblade","Backstab","Shadow Dance","Shuriken Tornado","Symbols of Death","Flagellation","Vanish","Sepsis","Rupture","Eviscerate","Shadowmeld","Shadowstrike","Shiv","Secret Technique","Black Powder"
        };

        List<string> BuffsList = new List<string>
        {
            "Slice and Dice","Stealth","Vanish","Subterfuge","Shadow Dance","Echoing Reprimand","Lead by Example","Shuriken Tornado","Master Assassin's Mark","Symbols of Death","Deathly Shadows","Master Assassin","Shadow Blades","The Rotten","Premeditation","Perforated Veins",
        };

        List<string> DebuffsList = new List<string>
        {
           "Marked for Death","Flagellation","Find Weakness","Serrated Bone Spike","Rupture",
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

            Settings.Add(new Setting("Rogue Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Mark of the Master Assassin", "Deathly Shadows", "Tiny Toxic Blade", "Akaaris Soul Fragment", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Sub", Color.Purple);
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
            int ComboPoints = Aimsharp.PlayerSecondaryPower();
            int EnergyMax = Aimsharp.PlayerMaxPower();
            int EnergyDefecit = EnergyMax - Energy;

            //Talents
            bool TalentVigor = Aimsharp.Talent(3, 1);
            bool TalentMasterofShadows = Aimsharp.Talent(7, 1);
            bool TalentShadowFocus = Aimsharp.Talent(2, 3);
            bool TalentAlacrity = Aimsharp.Talent(6, 2);
            bool TalentShurikenTornado = Aimsharp.Talent(7, 3);
            bool TalentEnvelopingShadows = Aimsharp.Talent(6, 3);
            bool TalentSubterfuge = Aimsharp.Talent(2, 2);
            bool TalentPremeditation = Aimsharp.Talent(1, 2);
            bool TalentNightstalker = Aimsharp.Talent(2, 1);
            bool TalentDarkShadow = Aimsharp.Talent(6, 1);
            bool TalentDeeperStratagem = Aimsharp.Talent(3, 2);
            bool TalentWeaponmaster = Aimsharp.Talent(1, 1);


            //buffs
            int BuffSliceandDiceRemains = Aimsharp.BuffRemaining("Slice and Dice") - GCD;
            bool BuffSliceandDiceUp = BuffSliceandDiceRemains > 0;
            bool BuffStealthUp = Aimsharp.HasBuff("Stealth");
            int BuffVanishRemains = Aimsharp.BuffRemaining("Vanish") - GCD;
            bool BuffVanishUp = BuffVanishRemains > 0;
            int BuffSubterfugeRemains = Aimsharp.BuffRemaining("Subterfuge") - GCD;
            bool BuffSubterfugeUp = BuffSubterfugeRemains > 0;
            int BuffShadowDanceRemains = Aimsharp.BuffRemaining("Shadow Dance") - GCD;
            bool BuffShadowDanceUp = BuffShadowDanceRemains > 0;
            int BuffEchoingReprimandRemains = Aimsharp.BuffRemaining("Echoing Reprimand") - GCD;
            bool BuffEchoingReprimandUp = BuffEchoingReprimandRemains > 0;
            int BuffEchoingReprimandStacks = Aimsharp.BuffStacks("Echoing Reprimand");
            int BuffLeadbyExampleRemains = Aimsharp.BuffRemaining("Lead by Example") - GCD;
            bool BuffLeadbyExampleUp = BuffLeadbyExampleRemains > 0;
            int BuffShurikenTornadoRemains = Aimsharp.BuffRemaining("Shuriken Tornado") - GCD;
            bool BuffShurikenTornadoUp = BuffShurikenTornadoRemains > 0;
            bool BuffMasterAssassinsMarkUp = Aimsharp.HasBuff("Master Assassin's Mark");
            int BuffSymbolsofDeathRemains = Aimsharp.BuffRemaining("Symbols of Death") - GCD;
            bool BuffSymbolsofDeathUp = BuffSymbolsofDeathRemains > 0;
            int BuffDeathlyShadowsRemains = Aimsharp.BuffRemaining("Deathly Shadows") - GCD;
            bool BuffDeathlyShadowsUp = BuffDeathlyShadowsRemains > 0;
            int BuffMasterAssassinRemains = Aimsharp.BuffRemaining("Master Assassin") - GCD;
            bool BuffMasterAssassinUp = BuffMasterAssassinRemains > 0;
            int BuffShadowBladesRemains = Aimsharp.BuffRemaining("Shadow Blades") - GCD;
            bool BuffShadowBladesUp = BuffShadowBladesRemains > 0;
            int BuffTheRottenRemains = Aimsharp.BuffRemaining("The Rotten") - GCD;
            bool BuffTheRottenUp = BuffTheRottenRemains > 0;
            int BuffPremeditationRemains = Aimsharp.BuffRemaining("Premeditation") - GCD;
            bool BuffPremeditationUp = BuffPremeditationRemains > 0;
            int BuffPerforatedVeinsRemains = Aimsharp.BuffRemaining("Perforated Veins") - GCD;
            bool BuffPerforatedVeinsUp = BuffPerforatedVeinsRemains > 0;
            int BuffPerforatedVeinsStacks = Aimsharp.BuffStacks("Perforated Veins");


            //debuffs
            int DebuffMarkedforDeathRemains = Aimsharp.DebuffRemaining("Marked for Death") - GCD;
            bool DebuffMarkedforDeathUp = DebuffMarkedforDeathRemains > 0;
            int DebuffFlagellationRemains = Aimsharp.DebuffRemaining("Flagellation") - GCD;
            bool DebuffFlagellationUp = DebuffFlagellationRemains > 0;
            int DebuffFlagellationStacks = Aimsharp.DebuffStacks("Flagellation");
            int DebuffFindWeaknessRemains = Aimsharp.DebuffRemaining("Find Weakness") - GCD;
            bool DebuffFindWeaknessUp = DebuffFindWeaknessRemains > 0;
            int DotSerratedBoneSpikeRemains = Aimsharp.DebuffRemaining("Serrated Bone Spike") - GCD;
            bool DotSerratedBoneSpikeUp = DotSerratedBoneSpikeRemains > 0;
            int DotRuptureRemains = Aimsharp.DebuffRemaining("Rupture") - GCD;
            bool DotRuptureUp = DotRuptureRemains > 0;
            bool DotRuptureRefreshable = DotRuptureRemains < 6000;


            //cooldowns
            int CDShadowBladesRemains = Aimsharp.SpellCooldown("Shadow Blades");
            bool CDShadowBladesUp = CDShadowBladesRemains <= 0;
            int CDSerratedBoneSpikeRemains = Aimsharp.SpellCooldown("Serrated Bone Spike");
            bool CDSerratedBoneSpikeUp = CDSerratedBoneSpikeRemains <= 0;
            int CDSerratedBoneSpikeCharges = Aimsharp.SpellCharges("Serrated Bone Spike");
            int CDSerratedBoneSpikeFullRecharge = Aimsharp.RechargeTime("Serrated Bone Spike") + 30000 * (Aimsharp.MaxCharges("Serrated Bone Spike") - CDSerratedBoneSpikeCharges - 1);
            float CDSerratedBoneSpikeFractional = CDSerratedBoneSpikeCharges + (1 - (Aimsharp.RechargeTime("Serrated Bone Spike") - GCD) / 30000f);
            CDSerratedBoneSpikeFractional = CDSerratedBoneSpikeFractional > Aimsharp.MaxCharges("Serrated Bone Spike") ? Aimsharp.MaxCharges("Serrated Bone Spike") : CDSerratedBoneSpikeFractional;
            int CDShadowDanceRemains = Aimsharp.SpellCooldown("Shadow Dance");
            bool CDShadowDanceUp = CDShadowDanceRemains <= 0;
            int CDShadowDanceCharges = Aimsharp.SpellCharges("Shadow Dance");
            int CDShadowDanceFullRecharge = Aimsharp.RechargeTime("Shadow Dance") + 60000 * (Aimsharp.MaxCharges("Shadow Dance") - CDShadowDanceCharges - 1);
            float CDShadowDanceFractional = CDShadowDanceCharges + (1 - (Aimsharp.RechargeTime("Shadow Dance") - GCD) / 60000f);
            CDShadowDanceFractional = CDShadowDanceFractional > Aimsharp.MaxCharges("Shadow Dance") ? Aimsharp.MaxCharges("Shadow Dance") : CDShadowDanceFractional;
            int CDShurikenTornadoRemains = Aimsharp.SpellCooldown("Shuriken Tornado");
            bool CDShurikenTornadoUp = CDShurikenTornadoRemains <= 0;
            int CDSymbolsofDeathRemains = Aimsharp.SpellCooldown("Symbols of Death");
            bool CDSymbolsofDeathUp = CDSymbolsofDeathRemains <= 0;


            //specific variables
            bool RuneforgeMarkoftheMasterAssassin = RuneforgePower == "Mark of the Master Assassin";
            bool RuneforgeDeathlyShadows = RuneforgePower == "Deathly Shadows";
            bool RuneforgeTinyToxicBlade = RuneforgePower == "Tiny Toxic Blade";
            bool RuneforgeAkaarisSoulFragment = RuneforgePower == "Akaaris Soul Fragment";
            bool ConduitLeadbyExample = ActiveConduits.Contains(342156);
            bool ConduitDeeperDaggers = ActiveConduits.Contains(341549);
            bool ConduitPerforatedVeins = ActiveConduits.Contains(341567);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastStealth = Aimsharp.CanCast("Stealth", "player") && Fighting;
            bool CanCastMarkedforDeath = Aimsharp.CanCast("Marked for Death") && Fighting;
            bool CanCastSliceandDice = Aimsharp.CanCast("Slice and Dice", "player") && Fighting;
            bool CanCastShadowBlades = Aimsharp.CanCast("Shadow Blades", "player") && Fighting;
            bool CanCastShurikenStorm = Aimsharp.CanCast("Shuriken Storm", "player") && Fighting;
            bool CanCastEchoingReprimand = Aimsharp.CanCast("Echoing Reprimand") && Fighting;
            bool CanCastSerratedBoneSpike = Aimsharp.CanCast("Serrated Bone Spike") && Fighting;
            bool CanCastGloomblade = Aimsharp.CanCast("Gloomblade") && Fighting;
            bool CanCastBackstab = Aimsharp.CanCast("Backstab") && Fighting;
            bool CanCastShadowDance = Aimsharp.CanCast("Shadow Dance", "player") && Fighting;
            bool CanCastShurikenTornado = Aimsharp.CanCast("Shuriken Tornado", "player") && Fighting;
            bool CanCastSymbolsofDeath = Aimsharp.CanCast("Symbols of Death", "player") && Fighting;
            bool CanCastFlagellation = Aimsharp.CanCast("Flagellation") && Fighting;
            bool CanCastVanish = false;
            bool CanCastSepsis = Aimsharp.CanCast("Sepsis") && Fighting;
            bool CanCastRupture = Aimsharp.CanCast("Rupture") && Fighting;
            bool CanCastBlackPowder = Aimsharp.CanCast("Eviscerate") && Fighting; 
            bool CanCastEviscerate = Aimsharp.CanCast("Eviscerate") && Fighting;
            bool CanCastShadowmeld = Aimsharp.CanCast("Shadowmeld", "player") && Fighting;
            bool CanCastShadowstrike = Aimsharp.CanCast("Shadowstrike") && Fighting;
            bool CanCastShiv = Aimsharp.CanCast("Shiv") && Fighting;
            bool CanCastSecretTechnique = Aimsharp.CanCast("Secret Technique") && Fighting;


            //actions+=/variable,name=snd_condition,value=buff.slice_and_dice.up|spell_targets.shuriken_storm>=6
            bool snd_condition = BuffSliceandDiceUp || EnemiesInMelee >= 6;

            //actions.finish=variable,name=premed_snd_condition,value=talent.premeditation.enabled&spell_targets<(5-covenant.necrolord)&!covenant.kyrian
            bool premed_snd_condition = TalentPremeditation && EnemiesInMelee < (5 - (CovenantNecrolord ? 1 : 0)) && !CovenantKyrian;

            //actions.finish+=/variable,name=skip_rupture,value=master_assassin_remains>0|!talent.nightstalker.enabled&talent.dark_shadow.enabled&buff.shadow_dance.up|spell_targets.shuriken_storm>=5
            bool skip_rupture = BuffMasterAssassinRemains > 0 || BuffMasterAssassinsMarkUp || !TalentNightstalker && TalentDarkShadow && BuffShadowDanceUp || EnemiesInMelee >= 5;

            //actions+=/variable,name=stealth_threshold,value=25+talent.vigor.enabled*20+talent.master_of_shadows.enabled*20+talent.shadow_focus.enabled*25+talent.alacrity.enabled*20+25*(spell_targets.shuriken_storm>=4)
            int stealth_threshold = 25 + (TalentVigor ? 20 : 0) + (TalentMasterofShadows ? 20 : 0) + (TalentShadowFocus ? 25 : 0) + (TalentAlacrity ? 20 : 0) + 25 * (EnemiesInMelee >= 4 ? 1 : 0);

            //actions.stealth_cds=variable,name=shd_threshold,value=cooldown.shadow_dance.charges_fractional>=1.75
            bool shd_threshold = CDShadowDanceFractional >= 1.75;

            bool Stealth = BuffStealthUp || BuffVanishUp || BuffSubterfugeUp || BuffShadowDanceUp;
            int MaxComboPoints = TalentDeeperStratagem ? 6 : 5;
            int ComboPointsDefecit = MaxComboPoints - ComboPoints;


            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;


            if (CanCastStealth)
            {
                Aimsharp.Cast("Stealth");
                return true;
            }

            //actions+=/call_action_list,name=cd
            if (!SaveCooldowns)
            {
                //actions.cds=shadow_dance,use_off_gcd=1,if=!buff.shadow_dance.up&buff.shuriken_tornado.up&buff.shuriken_tornado.remains<=3.5
                if (CanCastShadowDance)
                {
                    if (!BuffShadowDanceUp && BuffShurikenTornadoUp && BuffShurikenTornadoRemains <= 3500)
                    {
                        Aimsharp.Cast("Shadow Dance", true);
                        return true;
                    }
                }

                //actions.cds+=/symbols_of_death,use_off_gcd=1,if=buff.shuriken_tornado.up&buff.shuriken_tornado.remains<=3.5
                if (CanCastSymbolsofDeath)
                {
                    if (BuffShurikenTornadoUp && BuffShurikenTornadoRemains <= 3500)
                    {
                        Aimsharp.Cast("Symbols of Death", true);
                        return true;
                    }
                }

                //actions.cds+=/flagellation,if=variable.snd_condition&!stealthed.mantle
                if (CanCastFlagellation)
                {
                    if (!DebuffFlagellationUp && !(Stealth && BuffMasterAssassinsMarkUp))
                    {
                        Aimsharp.Cast("Flagellation");
                        return true;
                    }
                }

                //actions.cds+=/flagellation_cleanse,if=debuff.flagellation.remains<2
                if (CanCastFlagellation)
                {
                    if (DebuffFlagellationRemains < 2000)
                    {
                        Aimsharp.Cast("Flagellation");
                        return true;
                    }
                }

                //actions.cds+=/vanish,if=(runeforge.mark_of_the_master_assassin&combo_points.deficit<=3|runeforge.deathly_shadows&combo_points<1)&buff.symbols_of_death.up&buff.shadow_dance.up&master_assassin_remains=0&buff.deathly_shadows.down
                if (CanCastVanish)
                {
                    if ((RuneforgeMarkoftheMasterAssassin && ComboPointsDefecit <= 3 || RuneforgeDeathlyShadows && ComboPoints < 1) && BuffSymbolsofDeathUp && BuffShadowDanceUp && !BuffMasterAssassinsMarkUp && !BuffMasterAssassinUp && !BuffDeathlyShadowsUp)
                    {
                        Aimsharp.Cast("Vanish");
                        return true;
                    }
                }

                //actions.cds+=/pool_resource,for_next=1,if=talent.shuriken_tornado.enabled&!talent.shadow_focus.enabled
                if (TalentShurikenTornado && !TalentShadowFocus && CDShurikenTornadoUp && Energy < 60)
                {
                    return true;
                }

                //actions.cds+=/shuriken_tornado,if=energy>=60&variable.snd_condition&cooldown.symbols_of_death.up&cooldown.shadow_dance.charges>=1
                if (CanCastShurikenTornado)
                {
                    if (Energy >= 60 && snd_condition && CDSymbolsofDeathUp && CDShadowDanceCharges >= 1)
                    {
                        Aimsharp.Cast("Shuriken Tornado");
                        return true;
                    }
                }

                //actions.cds+=/serrated_bone_spike,cycle_targets=1,if=variable.snd_condition&!dot.serrated_bone_spike_dot.ticking&target.time_to_die>=21|fight_remains<=5&spell_targets.shuriken_storm<3
                if (CanCastSerratedBoneSpike)
                {
                    if (snd_condition && !DotSerratedBoneSpikeUp && TargetTimeToDie >= 21000 || TargetTimeToDie <= 5000 && EnemiesInMelee < 3)
                    {
                        Aimsharp.Cast("Serrated Bone Spike");
                        return true;
                    }
                }

                //actions.cds+=/sepsis,if=variable.snd_condition&combo_points.deficit>=1
                if (CanCastSepsis)
                {
                    if (snd_condition && ComboPointsDefecit >= 1)
                    {
                        Aimsharp.Cast("Sepsis");
                        return true;
                    }
                }
			//opener
		if (CanCastSliceandDice)
                {
                    if (!BuffSliceandDiceUp && ComboPoints == 2 && Time < 3000)
                    {
                        Aimsharp.Cast("Slice and Dice");
                        return true;
                    }
                }

                //actions.cds+=/symbols_of_death,if=variable.snd_condition&(talent.enveloping_shadows.enabled|cooldown.shadow_dance.charges>=1)&(!talent.shuriken_tornado.enabled|talent.shadow_focus.enabled|cooldown.shuriken_tornado.remains>2)
                if (CanCastSymbolsofDeath)
                {
                    if (snd_condition && (TalentEnvelopingShadows || CDShadowDanceCharges >= 1) && (!TalentShurikenTornado || TalentShadowFocus || CDShurikenTornadoRemains > 2000))
                    {
                        Aimsharp.Cast("Symbols of Death", true);
                        return true;
                    }
                }

                //actions.cds+=/marked_for_death,if=raid_event.adds.in>30-raid_event.adds.duration&combo_points.deficit>=cp_max_spend
                if (CanCastMarkedforDeath)
                {
                    if (ComboPoints == 0)
                    {
                        Aimsharp.Cast("Marked for Death");
                        return true;
                    }
                }

                //actions.cds+=/shadow_blades,if=variable.snd_condition&combo_points.deficit>=2
                if (CanCastShadowBlades)
                {
                    if (snd_condition && ComboPointsDefecit >= 2)
                    {
                        Aimsharp.Cast("Shadow Blades");
                        return true;
                    }
                }

                //actions.cds+=/echoing_reprimand,if=variable.snd_condition&combo_points.deficit>=2&(variable.use_priority_rotation|spell_targets.shuriken_storm<=4)
                if (CanCastEchoingReprimand)
                {
                    if (snd_condition && ComboPointsDefecit >= 2 && EnemiesInMelee <= 4)
                    {
                        Aimsharp.Cast("Echoing Reprimand");
                        return true;
                    }
                }

                //actions.cds+=/shuriken_tornado,if=talent.shadow_focus.enabled&variable.snd_condition&buff.symbols_of_death.up
                if (CanCastShurikenTornado)
                {
                    if (TalentShadowFocus && snd_condition && BuffSymbolsofDeathUp)
                    {
                        Aimsharp.Cast("Shuriken Tornado");
                        return true;
                    }
                }

                //actions.cds+=/shadow_dance,if=!buff.shadow_dance.up&fight_remains<=8+talent.subterfuge.enabled
                if (CanCastShadowDance)
                {
                    if (!BuffShadowDanceUp && TargetTimeToDie <= 8000 + (TalentSubterfuge ? 1000 : 0))
                    {
                        Aimsharp.Cast("Shadow Dance", true);
                        return true;
                    }
                }

                //actions.cds+=/potion,if=buff.bloodlust.react|buff.symbols_of_death.up&(buff.shadow_blades.up|cooldown.shadow_blades.remains<=10)
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false))
                {
                    if (BloodlustUp || BuffSymbolsofDeathUp && (BuffShadowBladesUp || CDShadowBladesRemains <= 10000))
                    {
                        Aimsharp.Cast("DPS Pot", true);
                        return true;
                    }
                }

                if (BuffSymbolsofDeathUp)
                {
                    foreach (string Racial in Racials)
                    {
                        if (Aimsharp.CanCast(Racial, "player") && Fighting)
                        {
                            Aimsharp.Cast(Racial, true);
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
                }
            }

            //actions+=/run_action_list,name=stealthed,if=stealthed.all
            if (Stealth)
            {
                //actions.stealthed=shadowstrike,if=(buff.stealth.up|buff.vanish.up)
                if (CanCastShadowstrike)
                {
                    if (BuffStealthUp || BuffVanishUp)
                    {
                        Aimsharp.Cast("Shadowstrike");
                        return true;
                    }
                }

                //actions.stealthed+=/call_action_list,name=finish,if=buff.shuriken_tornado.up&combo_points.deficit<=2
                //actions.stealthed+=/call_action_list,name=finish,if=spell_targets.shuriken_storm>=4&combo_points>=4
                //actions.stealthed+=/call_action_list,name=finish,if=combo_points.deficit<=1-(talent.deeper_stratagem.enabled&buff.vanish.up)
                if ((BuffShurikenTornadoUp && ComboPointsDefecit <= 2) || (EnemiesInMelee >= 4 && ComboPoints >= 4) || (ComboPointsDefecit <= 1 - (TalentDeeperStratagem && BuffVanishUp ? 1 : 0)))
                {
                    //actions.finish+=/slice_and_dice,if=!variable.premed_snd_condition&spell_targets.shuriken_storm<6&!buff.shadow_dance.up&buff.slice_and_dice.remains<fight_remains&refreshable
                    if (CanCastSliceandDice)
                    {
                        if (premed_snd_condition && EnemiesInMelee < 6 && !BuffShadowDanceUp && BuffSliceandDiceRemains < 3000)
                        {
                            Aimsharp.Cast("Slice and Dice");
                            return true;
                        }
                    }

                    //actions.finish+=/slice_and_dice,if=variable.premed_snd_condition&cooldown.shadow_dance.charges_fractional<1.75&buff.slice_and_dice.remains<cooldown.symbols_of_death.remains&(cooldown.shadow_dance.ready&buff.symbols_of_death.remains-buff.shadow_dance.remains<1.2)
                    if (CanCastSliceandDice)
                    {
                        if (premed_snd_condition && CDShadowDanceFractional < 1.75 && BuffSliceandDiceRemains < CDSymbolsofDeathRemains && (CDShadowDanceUp && BuffSymbolsofDeathRemains - BuffShadowDanceRemains < 1200))
                        {
                            Aimsharp.Cast("Slice and Dice");
                            return true;
                        }
                    }

                    //actions.finish+=/rupture,if=!variable.skip_rupture&target.time_to_die-remains>6&refreshable
                    if (CanCastRupture)
                    {
                        if (!skip_rupture && TargetTimeToDie - DotRuptureRemains > 6000 && DotRuptureRefreshable)
                        {
                            Aimsharp.Cast("Rupture");
                            return true;
                        }
                    }

                    //actions.finish+=/secret_technique
                    if (CanCastSecretTechnique)
                    {
                        Aimsharp.Cast("Secret Technique");
                        return true;
                    }

                    //actions.finish+=/rupture,cycle_targets=1,if=!variable.skip_rupture&!variable.use_priority_rotation&spell_targets.shuriken_storm>=2&target.time_to_die>=(5+(2*combo_points))&refreshable
                    if (CanCastRupture)
                    {
                        if (!skip_rupture && EnemiesInMelee >= 2 && TargetTimeToDie >= (5000 + (2000 * ComboPoints)) && DotRuptureRefreshable)
                        {
                            Aimsharp.Cast("Rupture");
                            return true;
                        }
                    }

                    //actions.finish+=/rupture,if=!variable.skip_rupture&remains<cooldown.symbols_of_death.remains+10&cooldown.symbols_of_death.remains<=5&target.time_to_die-remains>cooldown.symbols_of_death.remains+5
                    if (CanCastRupture)
                    {
                        if (!skip_rupture && DotRuptureRemains < CDSymbolsofDeathRemains + 10000 && CDSymbolsofDeathRemains <= 5000 && TargetTimeToDie - DotRuptureRemains > CDSymbolsofDeathRemains + 5000)
                        {
                            Aimsharp.Cast("Rupture");
                            return true;
                        }
                    }
			//AOE

		   if (CanCastBlackPowder)
		   {
		      if (EnemiesInMelee >= 3)
			 {
			    Aimsharp.Cast("Black Powder");
			    return true;
			 }
		    }

                    //actions.finish+=/eviscerate
                    if (CanCastEviscerate)
                    {
                        Aimsharp.Cast("Eviscerate");
                        return true;
                    }
                }

                //actions.stealthed+=/shiv,if=talent.nightstalker.enabled&runeforge.tiny_toxic_blade&spell_targets.shuriken_storm<5
                if (CanCastShiv)
                {
                    if (TalentNightstalker && RuneforgeTinyToxicBlade && EnemiesInMelee < 5)
                    {
                        Aimsharp.Cast("Shiv");
                        return true;
                    }
                }

                //actions.stealthed+=/shadowstrike,cycle_targets=1,if=debuff.find_weakness.remains<1&spell_targets.shuriken_storm<=3&target.time_to_die-remains>6
                if (CanCastShadowstrike)
                {
                    if (DebuffFindWeaknessRemains < 1000 && EnemiesInMelee <= 3 && TargetTimeToDie - DebuffFindWeaknessRemains > 6000)
                    {
                        Aimsharp.Cast("Shadowstrike");
                        return true;
                    }
                }

                //actions.stealthed+=/shuriken_storm,if=spell_targets>=3+(buff.the_rotten.up|runeforge.akaaris_soul_fragment&conduit.deeper_daggers.rank>=7)&(!buff.premeditation.up|spell_targets>=5)
                if (CanCastShurikenStorm)
                {
                    if (EnemiesInMelee >= 3 + ((BuffTheRottenUp || RuneforgeAkaarisSoulFragment && false ? 1 : 0)) && (!BuffPremeditationUp || EnemiesInMelee >= 5)) //need to implement conduit ranks
                    {
                        Aimsharp.Cast("Shuriken Storm");
                        return true;
                    }
                }

                //actions.stealthed+=/shadowstrike,if=debuff.find_weakness.remains<=1|cooldown.symbols_of_death.remains<18&debuff.find_weakness.remains<cooldown.symbols_of_death.remains
                if (CanCastShadowstrike)
                {
                    if (DebuffFindWeaknessRemains < 1000 || CDSymbolsofDeathRemains < 18000 && DebuffFindWeaknessRemains < CDSymbolsofDeathRemains)
                    {
                        Aimsharp.Cast("Shadowstrike");
                        return true;
                    }
                }

                //actions.stealthed+=/gloomblade,if=buff.perforated_veins.stack>=5&conduit.perforated_veins.rank>=13
                if (CanCastGloomblade)
                {
                    if (BuffPerforatedVeinsStacks >= 5 && false) //need to implement conduit ranks
                    {
                        Aimsharp.Cast("Gloomblade");
                        return true;
                    }
                }

                //actions.stealthed+=/shadowstrike
                if (CanCastShadowstrike)
                {
                    Aimsharp.Cast("Shadowstrike");
                    return true;
                }
                return false;
            }

            //actions+=/slice_and_dice,if=spell_targets.shuriken_storm<6&fight_remains>6&buff.slice_and_dice.remains<gcd.max&combo_points>=4-(time<10)*2
            if (CanCastSliceandDice)
            {
                if (EnemiesInMelee<6 && TargetTimeToDie>6000 && BuffSliceandDiceRemains<GCDMAX && ComboPoints>=4-(Time<10000?1:0)*2)
                {
                    Aimsharp.Cast("Slice and Dice");
                    return true;
                }
            }

            if (!SaveCooldowns)
            {
                //actions+=/call_action_list,name=stealth_cds,if=energy.deficit<=variable.stealth_threshold
                if (EnergyDefecit <= stealth_threshold)
                {
                    //actions.stealth_cds+=/vanish,if=(!variable.shd_threshold|!talent.nightstalker.enabled&talent.dark_shadow.enabled)&combo_points.deficit>1&!runeforge.mark_of_the_master_assassin
                    if (CanCastVanish)
                    {
                        if ((!shd_threshold || !TalentNightstalker && TalentDarkShadow) && ComboPointsDefecit > 1 && !RuneforgeMarkoftheMasterAssassin)
                        {
                            Aimsharp.Cast("Vanish");
                            return true;
                        }
                    }

                    //actions.stealth_cds+=/variable,name=shd_combo_points,value=combo_points.deficit>=2+buff.shadow_blades.up
                    //actions.stealth_cds+=/variable,name=shd_combo_points,value=combo_points.deficit>=3,if=covenant.kyrian
                    //actions.stealth_cds+=/variable,name=shd_combo_points,value=combo_points.deficit<=1,if=variable.use_priority_rotation&spell_targets.shuriken_storm>=4
                    bool shd_combo_points = ComboPointsDefecit >= 2 + (BuffShadowBladesUp ? 1 : 0) || ComboPointsDefecit >= 3 && CovenantKyrian;

                    //actions.stealth_cds+=/shadow_dance,if=variable.shd_combo_points&(variable.shd_threshold|buff.symbols_of_death.remains>=1.2|spell_targets.shuriken_storm>=4&cooldown.symbols_of_death.remains>10)
                    if (CanCastShadowDance)
                    {
                        if (shd_combo_points && (shd_threshold || BuffSymbolsofDeathRemains >= 1200 || EnemiesInMelee >= 4 && CDSymbolsofDeathRemains > 10000))
                        {
                            Aimsharp.Cast("Shadow Dance", true);
                            return true;
                        }
                    }

                    //actions.stealth_cds+=/shadow_dance,if=variable.shd_combo_points&fight_remains<cooldown.symbols_of_death.remains
                    if (CanCastShadowDance)
                    {
                        if (shd_combo_points && TargetTimeToDie < CDSymbolsofDeathRemains)
                        {
                            Aimsharp.Cast("Shadow Dance", true);
                            return true;
                        }
                    }
                }
            }

            //actions+=/call_action_list,name=finish,if=combo_points=animacharged_cp
            //actions+=/call_action_list,name=finish,if=combo_points.deficit<=1|fight_remains<=1&combo_points>=3
            //actions+=/call_action_list,name=finish,if=spell_targets.shuriken_storm>=4&combo_points>=4
            if (ComboPoints == BuffEchoingReprimandStacks || ComboPointsDefecit<=1 || TargetTimeToDie<=1000 && ComboPoints>=3 || EnemiesInMelee>=4 && ComboPoints>=4)
            {
                //actions.finish+=/slice_and_dice,if=!variable.premed_snd_condition&spell_targets.shuriken_storm<6&!buff.shadow_dance.up&buff.slice_and_dice.remains<fight_remains&refreshable
                if (CanCastSliceandDice)
                {
                    if (premed_snd_condition && EnemiesInMelee < 6 && !BuffShadowDanceUp && BuffSliceandDiceRemains < 3000)
                    {
                        Aimsharp.Cast("Slice and Dice");
                        return true;
                    }
                }

                //actions.finish+=/slice_and_dice,if=variable.premed_snd_condition&cooldown.shadow_dance.charges_fractional<1.75&buff.slice_and_dice.remains<cooldown.symbols_of_death.remains&(cooldown.shadow_dance.ready&buff.symbols_of_death.remains-buff.shadow_dance.remains<1.2)
                if (CanCastSliceandDice)
                {
                    if (premed_snd_condition && CDShadowDanceFractional < 1.75 && BuffSliceandDiceRemains < CDSymbolsofDeathRemains && (CDShadowDanceUp && BuffSymbolsofDeathRemains - BuffShadowDanceRemains < 1200))
                    {
                        Aimsharp.Cast("Slice and Dice");
                        return true;
                    }
                }

                //actions.finish+=/rupture,if=!variable.skip_rupture&target.time_to_die-remains>6&refreshable
                if (CanCastRupture)
                {
                    if (!skip_rupture && TargetTimeToDie - DotRuptureRemains > 6000 && DotRuptureRefreshable)
                    {
                        Aimsharp.Cast("Rupture");
                        return true;
                    }
                }

                //actions.finish+=/secret_technique
                if (CanCastSecretTechnique)
                {
                    Aimsharp.Cast("Secret Technique");
                    return true;
                }

                //actions.finish+=/rupture,cycle_targets=1,if=!variable.skip_rupture&!variable.use_priority_rotation&spell_targets.shuriken_storm>=2&target.time_to_die>=(5+(2*combo_points))&refreshable
                if (CanCastRupture)
                {
                    if (!skip_rupture && EnemiesInMelee >= 2 && TargetTimeToDie >= (5000 + (2000 * ComboPoints)) && DotRuptureRefreshable)
                    {
                        Aimsharp.Cast("Rupture");
                        return true;
                    }
                }

                //actions.finish+=/rupture,if=!variable.skip_rupture&remains<cooldown.symbols_of_death.remains+10&cooldown.symbols_of_death.remains<=5&target.time_to_die-remains>cooldown.symbols_of_death.remains+5
                if (CanCastRupture)
                {
                    if (!skip_rupture && DotRuptureRemains < CDSymbolsofDeathRemains + 10000 && CDSymbolsofDeathRemains <= 5000 && TargetTimeToDie - DotRuptureRemains > CDSymbolsofDeathRemains + 5000)
                    {
                        Aimsharp.Cast("Rupture");
                        return true;
                    }
                }
               //AOE
		if (CanCastBlackPowder)
		{
		      if (EnemiesInMelee >= 3)
			 {
			    Aimsharp.Cast("Black Powder");
			    return true;
			 }
		    }

                //actions.finish+=/eviscerate
                if (CanCastEviscerate)
                {
                    Aimsharp.Cast("Eviscerate");
                    return true;
                }
            }

            //actions+=/call_action_list,name=build,if=energy.deficit<=variable.stealth_threshold
            if (EnergyDefecit<=stealth_threshold)
            {
                //actions.build=shiv,if=!talent.nightstalker.enabled&runeforge.tiny_toxic_blade&spell_targets.shuriken_storm<5
                if (CanCastShiv)
                {
                    if (!TalentNightstalker && RuneforgeTinyToxicBlade && EnemiesInMelee<5)
                    {
                        Aimsharp.Cast("Shiv");
                        return true;
                    }
                }

                //actions.build+=/shuriken_storm,if=spell_targets>=2
                if (CanCastShurikenStorm)
                {
                    if (EnemiesInMelee>=2)
                    {
                        Aimsharp.Cast("Shuriken Storm");
                        return true;
                    }
                }

                //actions.build+=/serrated_bone_spike,if=cooldown.serrated_bone_spike.charges_fractional>=2.75|soulbind.lead_by_example.enabled&!buff.lead_by_example.up
                if (CanCastSerratedBoneSpike)
                {
                    if (CDSerratedBoneSpikeFractional>=2.75 || ConduitLeadbyExample && !BuffLeadbyExampleUp)
                    {
                        Aimsharp.Cast("Serrated Bone Spike");
                        return true;
                    }
                }

                //actions.build+=/gloomblade
                if (CanCastGloomblade)
                {
                    Aimsharp.Cast("Gloomblade");
                    return true;
                }

                if (CanCastBackstab)
                {
                    Aimsharp.Cast("Backstab");
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
