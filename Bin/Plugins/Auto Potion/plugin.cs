using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using AimsharpWow.API;

namespace AimsharpWow.Modules
{
    public class AutoHealPotions : Plugin
    {
        List<string> Potions = new List<string>
        {
            "Soulful Healing Potion","Spiritual Healing Potion","Abyssal Healing Potion"
        };



        public override void LoadSettings()
        {
            Settings.Add(new Setting("Healthstone HP %", 0, 100, 25));
            Settings.Add(new Setting("Healing Potion HP %", 0, 100, 15));
        }

        public override void Initialize()
        {
            Aimsharp.PrintMessage("Auto Heal Potions plugin loaded.");

            Macros.Add("hstone", "/use Healthstone");
            Macros.Add("hppot", "/use Soulful Healing Potion\\n/use Spiritual Healing Potion\\n/use Abyssal Healing Potion");


            foreach (string s in Potions)
            {
                Items.Add(s);
            }

            Items.Add("Healthstone");
        }

        //Timers to fix wow api bug with potions/healthstones and only using once per combat.
        Stopwatch HSTimer = new Stopwatch();

        public override bool CombatTick()
        {
            int Health = Aimsharp.Health("player");

            if (Health < GetSlider("Healthstone HP %"))
            {
                if (Aimsharp.CanUseItem("Healthstone", false))
                {
                    if (!HSTimer.IsRunning)
                    {
                        HSTimer.Restart();
                    }
                    if (HSTimer.ElapsedMilliseconds < 1500)
                    {
                        Aimsharp.Cast("hstone", true);
                        return true;
                    }
                }
            }

            if (Health < GetSlider("Healing Potion HP %"))
            {
                foreach (string s in Potions)
                {
                    if (Aimsharp.CanUseItem(s, false))
                    {
                            Aimsharp.Cast("hppot", true);
                            return true;
                    }
                }
            }

            return false;
        }

        public override bool OutOfCombatTick()
        {

            if (HSTimer.IsRunning)
                HSTimer.Reset();

            return false;
        }

    }
}
