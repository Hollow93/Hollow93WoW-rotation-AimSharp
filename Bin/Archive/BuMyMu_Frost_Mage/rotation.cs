using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{
    public class BuMyMuFrostMage : Rotation
    {
        bool SaveCDs;
        bool AOE;
        string LastCast;
        bool Fighting;
        int TargetHealth;
        int Health;
        bool LastCasted;

        //Spells
        List<string> FrostSpells = new List<string>{
            "Flurry","Frozen Orb","Frostbolt","Ice Lance","Icy Veins","Rune of Power","Shimmer","Glacial Spike","Blizzard","Ice Barrier"
        };

        List<string> FrostBuffs = new List<string>{
            "Fingers of Frost","Brain Freeze","Icy Veins","Rune of Power","Winter's Chill","Ice Barrier"
        };

        List<string> FrostDebuffs = new List<string>{
            "Winter's Chill","Frozen"
        };

        //Talents
        bool TalentLonelyWinter = Aimsharp.Talent(1, 2);
        bool TalentShimmer = Aimsharp.Talent(2, 2);
        bool TalentRuneOfPower = Aimsharp.Talent(3, 3);
        bool TalentChainReaction = Aimsharp.Talent(4, 2);
        bool TalentSplittingIce = Aimsharp.Talent(6, 2);
        bool TalentThermalVoid = Aimsharp.Talent(7, 1);

        //Main CD's
        bool CDFrozenOrbReady;
        int CDFrozenOrbRemains;

        bool CDRuneOfPowerReady;
        int CDRuneOfPowerRemains;

        bool CDIcyVeinsReady;
        int CDIcyVeinsRemains;

        int CDBlizzardRemains;
        bool CDBlizzardReady;

        int CDIceBarrierRemains;
        bool CDIceBarrierUp;

        //Buff
        bool BuffRuneOfPowerUp;
        int BuffRuneOfPowerRemains;

        bool BuffIcyVeinsUp;
        int BuffIcyVeinsRemains;

        bool BuffFingersOfFrostUp;
        int BuffFingersOfFrostRemains;

        bool BuffBrainFreezeUp;
        int BuffBrainFreezeRemains;

        int BuffWintersChillRemains;
        bool BuffWintersChillUp;

        int BuffIceBarrierRemains;
        bool BuffIceBarrierUp;

        //Debuffs
        int DebuffFrozenRemains;
        bool DebuffFrozenUp;

        int DebuffWintersChillRemains;
        bool DebuffWintersChillUp;

        public override void LoadSettings()
        {
            List<string> Trinkets = new List<string>(new string[] { "Generic", "None" });
            Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
            Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
        }

        public override void Initialize()
        {
            Aimsharp.PrintMessage("ButterMyMuffin Pre-Patch Frost Mage", Color.Blue);
            Aimsharp.PrintMessage("Recommended Talents: 2232x21", Color.Blue);
            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

            Aimsharp.Latency = 50;
            Aimsharp.QuickDelay = 125;
            Aimsharp.SlowDelay = 250;

            //Main Skills
            foreach(string skill in FrostSpells){
                Spellbook.Add(skill);
            }

            //Buffs
            foreach(string buff in FrostBuffs){
                Buffs.Add(buff);
            }

            //Debuffs
            foreach(string debuff in FrostDebuffs){
                Debuffs.Add(debuff);
            }

            Macros.Add("TopTrink", "/use 13");
            Macros.Add("BotTrink", "/use 14");
            Macros.Add("Blizz", "/cast [@cursor] Blizzard");

            CustomCommands.Add("AOE");
            CustomCommands.Add("SaveCooldowns");
        }


        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick()
        {
            Fighting = Aimsharp.Range("target") <= 40 && Aimsharp.TargetIsEnemy();
            Health = Aimsharp.Health("player");
            bool Moving = Aimsharp.PlayerIsMoving();
            float Haste = Aimsharp.Haste() / 100f;
            int Time = Aimsharp.CombatTime();
            int Range = Aimsharp.Range("target");
            TargetHealth = Aimsharp.Health("target");
            LastCast = Aimsharp.LastCast();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            SaveCDs = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            AOE = Aimsharp.IsCustomCodeOn("AOE");
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int GCDMAX = (int)(1500f / (Haste + 1f));
            int GCD = Aimsharp.GCD();
            int Latency = Aimsharp.Latency;
            bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) || Aimsharp.HasBuff("Heroism", "player", false) || Aimsharp.HasBuff("Time Warp", "player", false) || Aimsharp.HasBuff("Ancient Hysteria", "player", false) || Aimsharp.HasBuff("Netherwinds", "player", false) || Aimsharp.HasBuff("Drums of Rage", "player", false);
            int FlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));

            CDFrozenOrbRemains = Aimsharp.SpellCooldown("Frozen Orb") - GCD;
            CDFrozenOrbReady = CDFrozenOrbRemains <= 10;

            CDRuneOfPowerRemains = Aimsharp.SpellCooldown("Rune of Power") - GCD;
            CDRuneOfPowerReady = CDRuneOfPowerRemains <= 10;

            CDIcyVeinsRemains = Aimsharp.SpellCooldown("Icy Veins") - GCD;
            CDIcyVeinsReady = CDIcyVeinsRemains <= 10;

            CDBlizzardRemains = Aimsharp.SpellCooldown("Blizzard") - GCD;
            CDBlizzardReady = CDBlizzardRemains <= 10;

            CDIceBarrierRemains = Aimsharp.SpellCooldown("Ice Barrier") - GCD;
            CDIceBarrierUp = CDIceBarrierRemains <= 10;

            BuffRuneOfPowerRemains = Aimsharp.BuffRemaining("Rune of Power");
            BuffRuneOfPowerUp = BuffRuneOfPowerRemains > 0;

            BuffIcyVeinsRemains = Aimsharp.BuffRemaining("Icy Veins");
            BuffIcyVeinsUp = Aimsharp.BuffRemaining("Icy Veins") > 0;

            BuffFingersOfFrostRemains = Aimsharp.BuffRemaining("Fingers of Frost");
            BuffFingersOfFrostUp = Aimsharp.BuffRemaining("Fingers of Frost") > 0;

            BuffBrainFreezeRemains = Aimsharp.BuffRemaining("Brain Freeze");
            BuffBrainFreezeUp = Aimsharp.BuffRemaining("Brain Freeze") > 0;

            BuffWintersChillRemains = Aimsharp.BuffRemaining("Winter's Chill");
            BuffWintersChillUp = BuffWintersChillRemains > 0;

            BuffIceBarrierRemains = Aimsharp.BuffRemaining("Ice Barrier");
            BuffIceBarrierUp = BuffIceBarrierRemains > 0;

            DebuffFrozenRemains = Aimsharp.DebuffRemaining("Frozen");
            DebuffFrozenUp = DebuffFrozenRemains > 0;

            DebuffWintersChillRemains = Aimsharp.DebuffRemaining("Winter's Chill");
            DebuffWintersChillUp = DebuffWintersChillRemains > 0;

            if (!AOE)
            {
                EnemiesNearTarget = 1;
                EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            } 
            
            if(Aimsharp.CastingID("player") == 116){
                LastCasted = true;
            }

            if (IsChanneling){
                return false;
            }

            if(UseBarrier()){
                return true;
            }

            if(!SaveCDs){
                if(UseCD()){
                    return true;
                }
            }

            if(UseSmallCD()){
                return true;
            } 

            if(EnemiesNearTarget < 5){
                if(UseST()){
                    return true;
                }
            }

            if(EnemiesNearTarget >= 5 && AOE){
                if(UseAOE()){
                    return true;
                }
            }

            return false;
        }

        public bool UseBarrier(){
            if(Fighting){
                if(CDIceBarrierUp && (!BuffIceBarrierUp || BuffIceBarrierRemains <= 5000)){
                    Aimsharp.Cast("Ice Barrier");
                    return true;
                }
            }
            return false;
        }

        public bool UseCD(){
            if(Fighting){
                if(!BuffRuneOfPowerUp && CDIcyVeinsReady){
                    Aimsharp.Cast("Icy Veins");
                    LastCasted = false;
                    return true;
                }
            }
            return false;
        }

        public bool UseSmallCD(){
            if(Fighting){
                if(((CDIcyVeinsRemains > 15000 || SaveCDs) && !BuffRuneOfPowerUp && CDRuneOfPowerReady)){
                    Aimsharp.Cast("Rune of Power");
                    LastCasted = false;
                    return true;
                }
            }
            return false;
        }

        public bool UseAOE(){
            if(Fighting){
                if(CDFrozenOrbReady){
                    Aimsharp.Cast("Frozen Orb");
                    LastCasted = false;
                    return true;
                }else if(CDBlizzardReady){
                    Aimsharp.Cast("Blizz");
                    LastCasted = false;
                    return true;
                }else if(BuffBrainFreezeUp && LastCasted){
                    LastCasted = false;
                    Aimsharp.Cast("Flurry");
                    return true;
                }else if((!BuffBrainFreezeUp || (BuffBrainFreezeUp && !LastCasted)) && BuffFingersOfFrostUp || DebuffFrozenRemains > ((Aimsharp.Range("target")/50)*1000) || DebuffWintersChillRemains > ((Aimsharp.Range("target")/50)*1000)){
                    LastCasted = false;
                    Aimsharp.Cast("Ice Lance");
                    return true;
                }else{
                    Aimsharp.Cast("Frostbolt");
                    return true;
                }
            }
            return false;
        }

        public bool UseST(){
            if(Fighting){
                if(BuffBrainFreezeUp && LastCasted){
                    LastCasted = false;
                    Aimsharp.Cast("Flurry");
                    return true;
                }else{
                    if(CDFrozenOrbReady){
                        LastCasted = false;
                        Aimsharp.Cast("Frozen Orb");
                        return true;
                    }else if(!BuffBrainFreezeUp && DebuffWintersChillUp && BuffFingersOfFrostUp && DebuffWintersChillRemains > ((Aimsharp.Range("target")/50)*1000)){
                        LastCasted = false;
                        Aimsharp.Cast("Ice Lance");
                        return true;
                    }else if(!BuffBrainFreezeUp && BuffFingersOfFrostUp || DebuffFrozenRemains > ((Aimsharp.Range("target")/50)*1000)){
                        LastCasted = false;
                        Aimsharp.Cast("Ice Lance");
                        return true;
                    }else{
                        Aimsharp.Cast("Frostbolt");
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
