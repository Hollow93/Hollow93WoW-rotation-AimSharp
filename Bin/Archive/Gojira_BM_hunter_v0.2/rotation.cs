using System.Linq;
using System.Diagnostics;
using System;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API



namespace AimsharpWow.Modules
{

    public class BeastMastery : Rotation
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
           "Steady Shot","Arcane Shot","Light's Judgment","Barbed Shot","Multi-Shot","Tar Trap","Flare","Death Chakram","Wild Spirits","Bestial Wrath","Resonating Arrow","Stampede","Flayed Shot","Kill Shot","Chimaera Shot","Bloodshed","A Murder of Crows","Barrage","Kill Command","Dire Beast","Cobra Shot","Freezing Trap","Arcane Pulse","Aspect of the Wild","Revive Pet", "Call Pet","Mend Pet"
        };

        List<string> BuffsList = new List<string>
        {
            "Wild Spirits","Aspect of the Wild","Bestial Wrath","Flayer's Mark","Nesingwary's Trapping Apparatus","Frenzy","Beast Cleave","Flamewaker's Cobra Sting"
        };

        List<string> DebuffsList = new List<string>
        {
           "Barbed Shot","Tar Trap"
        };

        List<string> TotemsList = new List<string>
        {

        };

        List<string> MacroCommands = new List<string>
        {
            "AOE","SaveCooldowns","AutoTranq"
        };
        List<string> TranqList = new List<string>
        {
            "Dark Shroud","Seething Rage","Forsworn Doctrine","Motivational Clubbing","Loyal Beasts","Raging Tantrum","Unholy Fervor","Stimulate Resistance","Slime Coated","Additional Treads","Renew","Unholy Frenzy","Raging","Fallen Armanents", "Dark Fortress", "Undying Rage", "Angering Shriek", "Enrage"
        };
        int timer1 = -1;
        int timer2 = -1;
        int health1 = -1;
        int health2 = -1;

        public override void LoadSettings()
        {

            Settings.Add(new Setting("General Settings"));
            Settings.Add(new Setting("Use Top Trinket:", false));
            Settings.Add(new Setting("Use Bottom Trinket:", false));
            Settings.Add(new Setting("Use DPS Potion:", false));
            Settings.Add(new Setting("Potion name:", "Potion of Unbridled Fury"));

            Settings.Add(new Setting("Hunter Settings"));
            Settings.Add(new Setting("Auto Revive Pet?", true));
            Settings.Add(new Setting("Mend Pet %:", 1, 100, 50));
            Settings.Add(new Setting("Pet Number:", new List<string>() { "1", "2", "3", "4", "5"}, "1"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Sephuzs Proclamation", "Soulforge Embers", "Nessingwarys Trapping Apparatus", "Qapla Eredun War Order", }, "None"));
            Settings.Add(new Setting("If Nessingwarys Legendary cast FreezingTap:", false)); 
        }


        public override void Initialize()
        {

           
            string PetName = GetDropDown("Pet Number:");
            //Aimsharp.DebugMode(false);
            Aimsharp.PrintMessage("Gojira Beast Mastery", Color.Purple);
            Aimsharp.PrintMessage("Version 0.1", Color.Purple);

            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AutoTranq", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AutoTranq mode on/off.", Color.Blue);
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

            foreach (string Buff in TranqList)
            {
                Buffs.Add(Buff);
            }

            Spellbook.Add("Call Pet "+PetName);

            Items.Add(GetString("Potion name:"));

            Macros.Add("DPS Pot", "/use " + GetString("Potion name:"));
            Macros.Add("TopTrinket", "/use 13");
            Macros.Add("BottomTrinket", "/use 14");

            Macros.Add("TarTrapC", "/cast [@cursor] Tar Trap");
            Macros.Add("FlareC", "/cast [@cursor] Flare");
            Macros.Add("WildSpiritsC", "/cast [@cursor] Wild Spirits");
            Macros.Add("ResonatingArrowC", "/cast [@cursor] Resonating Arrow");
            Macros.Add("FreezingTrapC", "/cast [@cursor] Freezing Trap");
            Macros.Add("FreezingTrapP", "/cast [@player] Freezing Trap");


            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

        }

        private int timeToDie(){

            if (timer1 <= 0){
                this.timer1 = Aimsharp.CombatTime();
                this.health1 = Aimsharp.TargetCurrentHP()*1000;
                return 10000;
            }
            else
            {   
                if (Aimsharp.CombatTime() >= timer1+200) {
                    this.timer2 = Aimsharp.CombatTime();
                    this.health2 = Aimsharp.TargetCurrentHP()*1000;
                    int pdvSec = (health1 - health2) / ((timer1 - timer2)+1)*-1;

                    this.timer1 = Aimsharp.CombatTime();
                    this.health1 = Aimsharp.TargetCurrentHP()*1000;
                    if(pdvSec > 0){
                        return Aimsharp.TargetCurrentHP()*1000/pdvSec;
                    }

                    else
                    {
                        return 10000;
                    }
                }
                else
                {
                    return 10000;
                }

            }
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
            int MendPetLifePercent = GetSlider("Mend Pet %:");
            bool UseFreezingTrap = GetCheckBox("If Nessingwarys Legendary cast FreezingTap:");

            // Custom commands
            bool SaveCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            bool AOE = Aimsharp.IsCustomCodeOn("AOE");
            bool TranqBool = Aimsharp.IsCustomCodeOn("AutoTranq");


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
            //int TargetTimeToDie = 1000000; //need to implement time to die estimation
            int TargetTimeToDie = timeToDie();
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
            int Focus = Aimsharp.Power("player");
            int FocusMax = Aimsharp.PlayerMaxPower();
            int FocusDefecit = FocusMax - Focus;

            //Talents
            bool TalentKillerInstinct = Aimsharp.Talent(1, 1);
            bool TalentScentofBlood = Aimsharp.Talent(2, 1);
            bool TalentBloodShed = Aimsharp.Talent(7, 3);
            bool TalentChimaeraShot = Aimsharp.Talent(2, 3);
            bool TalentDireBeast = Aimsharp.Talent(1, 3);

            //buffs
            int BuffWildSpiritsRemains = Aimsharp.BuffRemaining("Wild Spirits") - GCD;
            bool BuffWildSpiritsUp = BuffWildSpiritsRemains > 0;
            int BuffAspectoftheWildRemains = Aimsharp.BuffRemaining("Aspect of the Wild") - GCD;
            bool BuffAspectoftheWildUp = BuffAspectoftheWildRemains > 0;
            int BuffBestialWrathRemains = Aimsharp.BuffRemaining("Bestial Wrath") - GCD;
            bool BuffBestialWrathUp = BuffBestialWrathRemains > 0;
            int BuffFlayersMarkRemains = Aimsharp.BuffRemaining("Flayer's Mark") - GCD;
            bool BuffFlayersMarkUp = BuffFlayersMarkRemains > 0;
            int BuffNesingwarysTrappingApparatusRemains = Aimsharp.BuffRemaining("Nesingwary's Trapping Apparatus") - GCD;
            bool BuffNesingwarysTrappingApparatusUp = BuffNesingwarysTrappingApparatusRemains > 0;
            int BuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy") - GCD;
            bool BuffFrenzyUp = BuffFrenzyRemains > 0;
            int BuffBeastCleaveRemains = Aimsharp.BuffRemaining("Beast Cleave") - GCD;
            bool BuffBeastCleaveUp = BuffBeastCleaveRemains > 0;
            int PetMainBuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy", "pet") - GCD;
            bool PetMainBuffFrenzyUp = PetMainBuffFrenzyRemains > 0;
            int BuffFlamewakersCobraStingRemains = Aimsharp.BuffRemaining("Flamewaker's Cobra Sting") - GCD;
            bool BuffFlamewakersCobraStingUp = BuffFlamewakersCobraStingRemains > 0;
            int PetMainBuffBeastCleaveRemains = Aimsharp.BuffRemaining("Beast Cleave", "pet") - GCD;
            bool PetMainBuffBeastCleaveUp = PetMainBuffBeastCleaveRemains > 0;


            //debuffs
            int DotBarbedShotRemains = Aimsharp.DebuffRemaining("Barbed Shot") - GCD;
            bool DotBarbedShotUp = DotBarbedShotRemains > 0;
            int DebuffTarTrapRemains = Aimsharp.DebuffRemaining("Tar Trap", "target", false) - GCD;
            bool DebuffTarTrapUp = DebuffTarTrapRemains > 0;


            //cooldowns
            int CDBarbedShotCharges = Aimsharp.SpellCharges("Barbed Shot");
            int CDBarbedShotFullRecharge = (int)(Aimsharp.RechargeTime("Barbed Shot") + 0f * (Aimsharp.MaxCharges("Barbed Shot") - CDBarbedShotCharges - 1) / (1f + Haste));
            float CDBarbedShotFractional = CDBarbedShotCharges + (1 - (Aimsharp.RechargeTime("Barbed Shot") - GCD) / (0f / (1f + Haste)));
            CDBarbedShotFractional = CDBarbedShotFractional > Aimsharp.MaxCharges("Barbed Shot") ? Aimsharp.MaxCharges("Barbed Shot") : CDBarbedShotFractional;
            int CDFlareRemains = Aimsharp.SpellCooldown("Flare");
            bool CDFlareUp = CDFlareRemains <= 0;
            int CDWildSpiritsRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Wild Spirits");
            bool CDWildSpiritsUp = CDWildSpiritsRemains <= 0;
            int CDBestialWrathRemains = Aimsharp.SpellCooldown("Bestial Wrath");
            bool CDBestialWrathUp = CDBestialWrathRemains <= 0;
            int CDKillCommandRemains = Aimsharp.SpellCooldown("Kill Command");
            bool CDKillCommandUp = CDKillCommandRemains <= 0;
            int CDChimaeraShotRemains = Aimsharp.SpellCooldown("Chimaera Shot");
            bool CDChimaeraShotUp = CDChimaeraShotRemains <= 0;


            //specific variables
            bool RuneforgeSephuzsProclamation = RuneforgePower == "Sephuzs Proclamation";
            bool RuneforgeSoulforgeEmbers = RuneforgePower == "Soulforge Embers";
            bool RuneforgeNessingwarysTrappingApparatus = RuneforgePower == "Nessingwarys Trapping Apparatus";
            bool RuneforgeQaplaEredunWarOrder = RuneforgePower == "Qapla Eredun War Order";
            bool ConduitNiyasToolsPoison = ActiveConduits.Contains(320660);
            bool ConduitReversalofFortune = ActiveConduits.Contains(339495);


            //CaNCasts
            bool CanCastLightsJudgment = Aimsharp.CanCast("Light's Judgment", "player") && !SaveCooldowns && Fighting;
            bool CanCastBarbedShot = Aimsharp.CanCast("Barbed Shot") && Fighting;
            bool CanCastMultiShot = Aimsharp.CanCast("Multi-Shot") && Fighting;
            bool CanCastTarTrap = Aimsharp.CanCast("Tar Trap", "player") && Fighting;
            bool CanCastFlare = Aimsharp.CanCast("Flare", "player") && Fighting;
            bool CanCastDeathChakram = Aimsharp.CanCast("Death Chakram") && Fighting;
            bool CanCastWildSpirits = Aimsharp.CanCast("Wild Spirits", "player") && !SaveCooldowns && Fighting;
            bool CanCastBestialWrath = Aimsharp.CanCast("Bestial Wrath", "player") && Fighting;
            bool CanCastResonatingArrow = Aimsharp.CanCast("Resonating Arrow", "player") && !SaveCooldowns && Fighting;
            bool CanCastStampede = Aimsharp.CanCast("Stampede", "player") && !SaveCooldowns && Fighting;
            bool CanCastFlayedShot = Aimsharp.CanCast("Flayed Shot") && Fighting;
            bool CanCastKillShot = Aimsharp.CanCast("Kill Shot") && Fighting;
            bool CanCastChimaeraShot = Aimsharp.CanCast("Chimaera Shot") && Fighting;
            bool CanCastBloodshed = Aimsharp.CanCast("Bloodshed") && Fighting;
            bool CanCastAMurderofCrows = Aimsharp.CanCast("A Murder of Crows") && Fighting;
            bool CanCastBarrage = Aimsharp.CanCast("Barrage", "player") && Fighting;
            bool CanCastKillCommand = Aimsharp.CanCast("Kill Command") && Fighting;
            bool CanCastDireBeast = Aimsharp.CanCast("Dire Beast") && Fighting;
            bool CanCastCobraShot = Aimsharp.CanCast("Cobra Shot") && Fighting;
            bool CanCastFreezingTrap = Aimsharp.CanCast("Freezing Trap", "player") && Fighting;
            bool CanCastArcanePulse = Aimsharp.CanCast("Arcane Pulse", "player") && !SaveCooldowns && Fighting;
            bool CanCastAspectoftheWild = Aimsharp.CanCast("Aspect of the Wild", "player") && !SaveCooldowns && Fighting;
            bool CanCastCallPet = Aimsharp.CanCast("Call Pet");
            bool CanCastMendPet = Aimsharp.CanCast("Mend Pet");
            bool CanCastTranquilizingShot = Aimsharp.CanCast("Tranquilizing Shot") && Fighting && TranqBool;
            bool CanCastRevivePet = Aimsharp.CanCast("Revive Pet");
            bool CanCastSteadyShot = Aimsharp.CanCast("Steady Shot") && Fighting;
            bool CanCastArcaneShot = Aimsharp.CanCast("Arcane Shot") && Fighting;



            bool RevivePetEnabled = GetCheckBox("Auto Revive Pet?");
            int BarbedShotBuffCount = Aimsharp.BuffStacks("Barbed Shot");
            int BarbedShotCountForGCD = 0;
            for (int i = 0; i < BarbedShotBuffCount; i++)
            {
                if (Aimsharp.BuffInfoDetailed("player", "Barbed Shot", true)[i]["Remaining"] - GCD > GCDMAX)
                    BarbedShotCountForGCD++;
            }

            float FocusRegen = 10f * (1f + Haste) + BarbedShotCountForGCD * 2.5f;
            float FocusTimeToMax = (FocusMax - Focus) * 1000f / FocusRegen;
            int PlayerCasting = Aimsharp.CastingID("player");
            int PlayerCastRemaining = Aimsharp.CastingRemaining("player");

            bool ReviveExecuting = PlayerCasting == 982 ? true : false && PlayerCastRemaining > 200;
            int PetHealth = Aimsharp.Health("pet");

            //Target Buff
            bool TargetBuffed = false;
            foreach (string tranq in TranqList)
            {
                if ( Aimsharp.HasBuff(tranq,"target",false,"enrage") || Aimsharp.HasBuff(tranq,"target",false,"magic")){
                    TargetBuffed = true;
                } 
            }


            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

            if (RevivePetEnabled)
            {
                if (CanCastRevivePet && PetHealth < 1)
                {
                    if (!ReviveExecuting)
                    {
                        
                        return RevivePet();
                    }
                }
            }

            //actions+=/use_items

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

            if(CanCastMendPet && Aimsharp.PlayerHasPet() && PetHealth <= MendPetLifePercent && PetHealth >=1)
                return MendPet();

            if (CanCastTranquilizingShot && TargetBuffed)
                return TranquilizingShot();

            //actions+=/call_action_list,name=cds
            //actions.cds+=/berserking,if=(buff.wild_spirits.up|!covenant.night_fae&buff.aspect_of_the_wild.up&buff.bestial_wrath.up)&(target.time_to_die>cooldown.berserking.duration+duration|(target.health.pct<35|!talent.killer_instinct))|target.time_to_die<13
            if ((BuffWildSpiritsUp || !CovenantNightFae && BuffAspectoftheWildUp && BuffBestialWrathUp) && (true || (TargetHealthPct < 35 || !TalentKillerInstinct)) || TargetTimeToDie < 13000)

                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }

            //actions.cds+=/lights_judgment
            if (CanCastLightsJudgment)
            {
                Aimsharp.Cast("Light's Judgment");
                return true;
            }

            //actions.cds+=/potion,if=buff.aspect_of_the_wild.up|target.time_to_die<26
            if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
            {
                if ((BuffAspectoftheWildUp && TargetTimeToDie < 26000) || (TargetTimeToDie < 26000 && BloodlustUp ))
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            #region ST
            if( !AOE || (AOE && Aimsharp.EnemiesNearTarget() <2)){
            

                if(CanCastAspectoftheWild && !SaveCooldowns)
                    return AspectOfTheWild();

                if(CanCastBarbedShot && PetMainBuffFrenzyRemains < 2500)
                    return BarbedShot();

                if (CanCastTarTrap && RuneforgeSoulforgeEmbers && DebuffTarTrapRemains < GCDMAX && CDFlareRemains < GCDMAX )
                    return TarTrap();

                if(CanCastFlare && DebuffTarTrapUp && RuneforgeSoulforgeEmbers)
                    return Flare();


                if(CanCastBloodshed && !SaveCooldowns && TalentBloodShed)
                    return BloodShed();

                if(CanCastWildSpirits && !SaveCooldowns)
                    return WildSpirits();

                if(CanCastFlayedShot && !SaveCooldowns)
                    return FlayedShot();

                if(CanCastKillShot && (Aimsharp.Health("target")< 20 || BuffFlayersMarkUp))
                    return KillShot();

                if(CanCastBarbedShot && (CDWildSpiritsRemains > CDBarbedShotFullRecharge || !CovenantNightFae) && (CDBestialWrathRemains < 12000 * CDBarbedShotFractional + GCDMAX && TalentScentofBlood || CDBarbedShotFullRecharge < GCDMAX && !CDBestialWrathUp) || TargetTimeToDie < 9000)
                    return BarbedShot();

                if(CanCastDeathChakram && Focus + 3 < FocusMax)
                    return DeathChakram();

                if(CanCastStampede && !SaveCooldowns && ( BuffAspectoftheWildUp || TargetTimeToDie < 15000))
                    return Stampede();

                if(CanCastAMurderofCrows)
                    return AMurderofCrows();
                
                if(CanCastResonatingArrow && (BuffBestialWrathUp || TargetTimeToDie < 10000))
                    return ResonatingArrow();

                if(CanCastBestialWrath)
                    return BestialWrath();

                if(CanCastChimaeraShot && TalentChimaeraShot)
                    return ChimaeraShot();
                
                if(CanCastKillCommand)
                    return KillCommand();

                if(CanCastDireBeast && TalentDireBeast)
                    return DireBeast();

                if(CanCastCobraShot && ((!CanCastKillCommand && CDKillCommandRemains > 1000) || (!CDChimaeraShotUp && CDChimaeraShotRemains > 1000)) && Focus >= 50 && (Focus - 35 + FocusRegen * ((CDKillCommandRemains - 1000) / 1000) > (BuffFlamewakersCobraStingUp ? 0 : 30) || CDKillCommandRemains > 1000 + GCDMAX) || (BuffBestialWrathUp || BuffNesingwarysTrappingApparatusUp) && !RuneforgeQaplaEredunWarOrder || TargetTimeToDie < 3000)
                {
                    return CobraShot();
                }
                    
                if(CanCastBarbedShot && PetMainBuffFrenzyRemains < 2500)
                    return BarbedShot();

                if (CanCastArcanePulse && (!BuffBestialWrathUp || TargetTimeToDie < 5000) && Aimsharp.EnemiesInMelee() >=1)
                    return ArcanePulse();

                if(CanCastTarTrap && (RuneforgeSoulforgeEmbers || RuneforgeNessingwarysTrappingApparatus))
                    return TarTrap();

                if(CanCastFreezingTrap && RuneforgeNessingwarysTrappingApparatus && UseFreezingTrap)
                    return FreezingTrapP();
                
                if(CanCastArcaneShot && Focus > 80)
                    return ArcaneShot();

                if(CanCastSteadyShot && Aimsharp.CastingID("player") != 56641)
                    return SteadyShot();
            }
            #endregion

            #region AOE
            else{   
                if(CanCastMultiShot && BuffBeastCleaveRemains < 1000)
                    return MultiShot();

                if(CanCastAspectoftheWild && !SaveCooldowns)
                    return AspectOfTheWild();

                if(CanCastBarbedShot && PetMainBuffFrenzyRemains < 2500)
                    return BarbedShot();

                if(CanCastKillCommand)
                    return KillCommand();



                if (CanCastTarTrap && RuneforgeSoulforgeEmbers && DebuffTarTrapRemains < GCDMAX && CDFlareRemains < GCDMAX)
                    return TarTrap();

                if(CanCastFlare && DebuffTarTrapUp && RuneforgeSoulforgeEmbers)
                    return Flare();

                if(CanCastDeathChakram && Focus + 3 < FocusMax)
                    return DeathChakram();

                if(CanCastWildSpirits && !SaveCooldowns)
                    return WildSpirits();

                if(CanCastBarbedShot && (CDBarbedShotFullRecharge < GCDMAX && CDBestialWrathRemains > 0 || CDBestialWrathRemains < 12000 + GCDMAX && TalentScentofBlood))
                    return BarbedShot();
                
                if(CanCastBestialWrath)
                    return BestialWrath();

                if(CanCastStampede && !SaveCooldowns && ( BuffAspectoftheWildUp || TargetTimeToDie < 15000))
                    return Stampede();
                
                if(CanCastResonatingArrow && (BuffBestialWrathUp || TargetTimeToDie < 10000))
                    return ResonatingArrow();

                if(CanCastFlayedShot && !SaveCooldowns)
                    return FlayedShot();

                if(CanCastKillShot && (Aimsharp.Health("target")< 20 || BuffFlayersMarkUp))
                    return KillShot();
                
                if(CanCastChimaeraShot && TalentChimaeraShot)
                    return ChimaeraShot();

                if(CanCastBloodshed && !SaveCooldowns && TalentBloodShed)
                    return BloodShed();
                    
                if(CanCastAMurderofCrows)
                    return AMurderofCrows();

                if(CanCastBarrage && (PetMainBuffFrenzyRemains > 3000/(1+Haste) && AOE))
                    return Barrage();

                if(CanCastKillCommand && Focus >= 30)
                    return KillCommand();

                if(CanCastDireBeast && TalentDireBeast)
                    return DireBeast();

                if(CanCastBarbedShot && PetMainBuffFrenzyRemains < 2500)
                    return BarbedShot();
                
                if(CanCastCobraShot && ((!CanCastKillCommand && CDKillCommandRemains > 1000) || (!CDChimaeraShotUp && CDChimaeraShotRemains > 1000)) && Focus >= 50 && (Focus - 35 + FocusRegen * ((CDKillCommandRemains - 1000) / 1000) > (BuffFlamewakersCobraStingUp ? 0 : 30) || CDKillCommandRemains > 1000 + GCDMAX) || (BuffBestialWrathUp || BuffNesingwarysTrappingApparatusUp) && !RuneforgeQaplaEredunWarOrder || TargetTimeToDie < 3000)
                    return CobraShot();

                if(CanCastTarTrap && (RuneforgeSoulforgeEmbers || RuneforgeNessingwarysTrappingApparatus))
                    return TarTrap();

                if(CanCastFreezingTrap && RuneforgeNessingwarysTrappingApparatus && UseFreezingTrap)
                    return FreezingTrapP();

                if(CanCastArcaneShot && Focus > 80)
                    return ArcaneShot();

                if(CanCastSteadyShot)
                    return SteadyShot();
 
                
            }
            #endregion

            return false;
        }


        public override bool OutOfCombatTick()
        {
            this.timer1 = -1;
            this.health1= -1;
            int PlayerCasting = Aimsharp.CastingID("player");
            int PlayerCastRemaining = Aimsharp.CastingRemaining("player");
            bool ReviveExecuting = PlayerCasting == 982 ? true : false && PlayerCastRemaining > 200;
            int PetHealth = Aimsharp.Health("pet");
            bool RevivePetEnabled = GetCheckBox("Auto Revive Pet?");
            string PetName = GetDropDown("Pet Number:");


            if (!Aimsharp.PlayerIsMounted() && !Aimsharp.PlayerHasPet())
            {
                if(Aimsharp.CanCast("Call Pet "+PetName))
                {   
                   Aimsharp.Cast("Call Pet "+PetName);
                   return true;  
                }
                
                if (RevivePetEnabled)
                {
                    if (Aimsharp.CanCast("Revive Pet") && PetHealth < 1)
                    {
                        if (!ReviveExecuting)
                        {
                            Aimsharp.Cast("Revive Pet");
                            return true;
                        }
                    }
                }
            }


            return false;
        }

        bool MendPet() { Aimsharp.Cast("Mend Pet"); return true; }
        bool TranquilizingShot() { Aimsharp.Cast("Tranquilizing Shot"); return true; }
        bool RevivePet() { Aimsharp.Cast("Revive Pet"); return true; }
        bool AspectOfTheWild() { Aimsharp.Cast("Aspect of the Wild"); return true; }
        bool BarbedShot() { Aimsharp.Cast("Barbed Shot"); return true; }
        bool BloodShed() { Aimsharp.Cast("Bloodshed"); return true; }
        bool WildSpirits() { Aimsharp.Cast("WildSpiritsC"); return true; }
        bool TarTrap() { Aimsharp.Cast("TarTrapC"); return true; }
        bool Flare() { Aimsharp.Cast("FlareC"); return true; }
        bool FlayedShot() { Aimsharp.Cast("Flayed Shot"); return true; }
        bool KillShot() { Aimsharp.Cast("Kill Shot"); return true; }
        bool DeathChakram() { Aimsharp.Cast("Death Chakram"); return true; }
        bool Stampede() { Aimsharp.Cast("Stampede"); return true; }
        bool AMurderofCrows() {Aimsharp.Cast("A Murder of Crows"); return true; }
        bool ResonatingArrow() {Aimsharp.Cast("ResonatingArrowC"); return true; }
        bool BestialWrath() {Aimsharp.Cast("Bestial Wrath"); return true; }
        bool ChimaeraShot() {Aimsharp.Cast("Chimaera Shot"); return true; }
        bool KillCommand() { Aimsharp.Cast("Kill Command"); return true; }
        bool DireBeast() { Aimsharp.Cast("Dire Beast"); return true; }
        bool CobraShot() { Aimsharp.Cast("Cobra Shot"); return true; }
        bool FreezingTrapP() { Aimsharp.Cast("FreezingTrapP"); return true; }
        bool MultiShot() { Aimsharp.Cast("Multi-Shot"); return true; }
        bool Barrage() { Aimsharp.Cast("Barrage"); return true; }
        bool ArcanePulse() {Aimsharp.Cast("Arcane Pulse"); return true;}
        bool ArcaneShot() { Aimsharp.Cast("Arcane Shot"); return true; }
        bool SteadyShot() { Aimsharp.Cast("Steady Shot"); return true; }

    }
}
