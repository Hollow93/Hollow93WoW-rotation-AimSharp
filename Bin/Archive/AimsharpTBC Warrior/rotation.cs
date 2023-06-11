using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ClassicShadowPriest : Rotation
    {
        List<string> Racials = new List<string>
        {
            "Berserking"
        };


        List<string> GeneralBuffs = new List<string>
        {

        };

        List<string> GeneralDebuffs = new List<string>
        {

        };

        List<string> SpellsList = new List<string>
        {
    "Slam",
    "Execute",
    "Mortal Strike",
    "Whirlwind",
    "Heroic Strike",
        };

        List<string> BuffsList = new List<string>
        {

        };

        List<string> DebuffsList = new List<string>
        {

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
            Settings.Add(new Setting("Use Top Trinket:", true));
            Settings.Add(new Setting("Use Bottom Trinket:", true));

        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("TBC Arms Warrior", Color.Purple);
            Aimsharp.PrintMessage("Version 1.0 (WoW TBC Classic)", Color.Purple);

            /*  Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
              Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
              Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
              Aimsharp.PrintMessage(" ");
              Aimsharp.PrintMessage("/xxxxx AOE", Color.Blue);
              Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
              Aimsharp.PrintMessage(" ");
              Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue); */

            Aimsharp.Latency = 100;
            Aimsharp.QuickDelay = 150;
            Aimsharp.SlowDelay = 350;

            foreach (string Spell in SpellsList)
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


            Macros.Add("TopTrinket", "/use 13");
            Macros.Add("BottomTrinket", "/use 14");

            foreach (string MacroCommand in MacroCommands)
            {
                CustomCommands.Add(MacroCommand);
            }

            //CustomFunctions.Add("Cascading Calamity Rank", "local isSelected\nlocal count = 0\nfor _, itemLocation in AzeriteUtil.EnumerateEquipedAzeriteEmpoweredItems() do\nisSelected = C_AzeriteEmpoweredItem.IsPowerSelected(itemLocation, 230)\nif isSelected then count = count + 1 end\nend\nreturn count");
        }

        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick()
        {
            #region Simc Conditionals

            // Aimsharp Settings
            bool UseTopTrinket = GetCheckBox("Use Top Trinket:");
            bool UseBottomTrinket = GetCheckBox("Use Bottom Trinket:");


            // Custom commands
            bool SaveCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            bool AOE = Aimsharp.IsCustomCodeOn("AOE");

            // Base simc 
            int Time = Aimsharp.CombatTime();
            bool Fighting = Aimsharp.Range("target") <= 40 && Aimsharp.TargetIsEnemy();
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int RangeToTarget = Aimsharp.Range();

            if (!AOE)
            {
                EnemiesNearTarget = 1;
                EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            }
            float Haste = Aimsharp.Haste() / 100f;
            int GCD = Aimsharp.GCD();
            int GCDMAX = (int)(1500f / (Haste + 1f));
            int TargetHealthPct = Aimsharp.Health("target");
            int PlayerLevel = Aimsharp.GetPlayerLevel();
            bool Moving = Aimsharp.PlayerIsMoving();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            int PlayerCastingID = Aimsharp.CastingID("player");
            string LastCast = Aimsharp.LastCast();

            // General buffs

            // General Debuffs

            // General Cooldowns

            // Mage power
            int Rage = Aimsharp.Power("player");

            // Mage Talents

            // Mage buffs

            // Mage debuffs
            /*    "Vampiric Touch",
            "Shadow Word: Pain",
            "Mind Flay",
            "Vampiric Embrace",
            "Blackout",*/
          //  int VTRemaining = Aimsharp.DebuffRemaining("Vampiric Touch", "target");
          //  int SWPRemaining = Aimsharp.DebuffRemaining("Shadow Word: Pain", "target");

            // Mage cooldowns

            // Mage specific variables


            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

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

            //racials
            foreach (string Racial in Racials)
            {
                if (Aimsharp.CanCast(Racial, "player") && Fighting)
                {
                    Aimsharp.Cast(Racial, true);
                    return true;
                }
            }

            if (Aimsharp.CanCast("Execute") && TargetHealthPct < 20)
            {
                    Aimsharp.Cast("Execute");
                    return true;
            }

            if (Aimsharp.CanCast("Slam") && !Moving)
            {
                    Aimsharp.Cast("Slam");
                    return true;
            }

            if (Aimsharp.CanCast("Mortal Strike"))
            {
                Aimsharp.Cast("Mortal Strike");
                return true;
            }

            if (Aimsharp.CanCast("Whirlwind") && Aimsharp.SpellCooldown("Mortal Strike") >= 4000)
            {
                Aimsharp.Cast("Whirlwind");
                return true;
            }

            if (Aimsharp.CanCast("Heroic Strike") && Rage > 60)
            {
                Aimsharp.Cast("Heroic Strike");
                return true;
            }


            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

    }
}
