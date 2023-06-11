using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using AimsharpWow.API;

namespace AimsharpWow.Modules
{
    public class AutoDefense : Plugin
    {
        List<string> Specs = new List<string>
        {
            "Warrior: Fury","Warrior: Arms","Warrior: Protection","Death Knight: Frost","Death Knight: Unholy","Death Knight: Blood","Druid: Balance","Druid: Feral","Druid: Guardian","Druid: Restoration","Rogue: Assassination","Rogue: Outlaw","Rogue: Subtlety",
            "Hunter: Marksmanship","Hunter: Beast Mastery","Hunter: Survival","Monk: Windwalker","Monk: Brewmaster","Monk: Mistweaver"
        };

        Dictionary<string, List<string>> DefensiveSpells = new Dictionary<string, List<string>>();

        List<string> WarriorFurySpells = new List<string>
        {
            "Spell Reflection", "Ignore Pain", "Victory Rush", "Rallying Cry", "Enraged Regeneration"
        };
        List<string> WarriorArmsSpells = new List<string>
        {
            "Spell Reflection", "Ignore Pain", "Victory Rush", "Rallying Cry", "Die by the Sword"
        };
        List<string> WarriorProtectionSpells = new List<string>
        {
            "Spell Reflection", "Ignore Pain", "Victory Rush", "Rallying Cry", "Shield Wall", "Last Stand", "Demoralizing Shout"
        };

        List<string> DKFrostSpells = new List<string>
        {
            "Anti-Magic Shell", "Death Strike","Death Pact"
        };
        List<string> DKUnholySpells = new List<string>
        {
            "Anti-Magic Shell", "Death Strike","Death Pact"
        };
        List<string> DKBloodSpells = new List<string>
        {
            "Anti-Magic Shell", "Death Strike","Death Pact"
        };

        List<string> DruidBalanceSpells = new List<string>
        {
            "Barkskin",
        };
        List<string> DruidFeralSpells = new List<string>
        {
            "Barkskin","Regrowth","Survival Instincts"
        };
        List<string> DruidGuardianSpells = new List<string>
        {
            "Barkskin","Frenzied Regeneration","Survival Instincts"
        };
        List<string> DruidRestorationSpells = new List<string>
        {
            "Barkskin",
        };
        List<string> RogueAsnSpells = new List<string>
        {
            "Crimson Vial","Evasion","Cloak of Shadows"
        };
        List<string> RogueOutlawSpells = new List<string>
        {
            "Crimson Vial","Evasion","Cloak of Shadows"
        };
        List<string> RogueSubSpells = new List<string>
        {
            "Crimson Vial","Evasion","Cloak of Shadows"
        };
        List<string> HunterMarksSpells = new List<string>
        {
            "Exhilaration","Aspect of the Turtle","Survival of the Fittest"
        };
        List<string> HunterBMSpells = new List<string>
        {
            "Exhilaration","Aspect of the Turtle","Survival of the Fittest"
        };
        List<string> HunterSurvivalSpells = new List<string>
        {
            "Exhilaration","Aspect of the Turtle","Survival of the Fittest"
        };
        List<string> MonkWWSpells = new List<string>
        {
            "Fortifying Brew","Diffuse Magic","Touch of Karma"
        };
        List<string> MonkBrewSpells = new List<string>
        {
            "Fortifying Brew"
        };
        List<string> MonkMWSpells = new List<string>
        {
            "Fortifying Brew","Diffuse Magic"
        };


        public override void LoadSettings()
        {
            Settings.Add(new Setting("Class", Specs, "Warrior: Fury"));
            Settings.Add(new Setting("Immunities HP %", 0, 100, 5));
            Settings.Add(new Setting("Mitigation HP %", 0, 100, 35));
            Settings.Add(new Setting("Self Healing HP %", 0, 100, 55));
        }

        string Class = "";
        int ImmunityHP = 0;
        int MitigationHP = 0;
        int HealingHP = 0;
        public override void Initialize()
        {
            Class = GetDropDown("Class");
            ImmunityHP = GetSlider("Immunities HP %");
            MitigationHP = GetSlider("Mitigation HP %");
            HealingHP = GetSlider("Self Healing HP %");

            Aimsharp.PrintMessage("Auto Defensives plugin loaded.");

            Aimsharp.PrintMessage("Class selected: " + Class);

            DefensiveSpells.Add("Warrior: Fury", WarriorFurySpells);
            DefensiveSpells.Add("Warrior: Arms", WarriorArmsSpells);
            DefensiveSpells.Add("Warrior: Protection", WarriorProtectionSpells);
            DefensiveSpells.Add("Death Knight: Frost", DKFrostSpells);
            DefensiveSpells.Add("Death Knight: Unholy", DKUnholySpells);
            DefensiveSpells.Add("Death Knight: Blood", DKBloodSpells);
            DefensiveSpells.Add("Druid: Balance", DruidBalanceSpells);
            DefensiveSpells.Add("Druid: Restoration", DruidRestorationSpells);
            DefensiveSpells.Add("Druid: Guardian", DruidGuardianSpells);
            DefensiveSpells.Add("Druid: Feral", DruidFeralSpells);
            DefensiveSpells.Add("Rogue: Assassination", RogueAsnSpells);
            DefensiveSpells.Add("Rogue: Outlaw", RogueOutlawSpells);
            DefensiveSpells.Add("Rogue: Subtlety", RogueSubSpells);
            DefensiveSpells.Add("Hunter: Marksmanship", HunterMarksSpells);
            DefensiveSpells.Add("Hunter: Beast Mastery", HunterBMSpells);
            DefensiveSpells.Add("Hunter: Survival", HunterSurvivalSpells);
            DefensiveSpells.Add("Monk: Windwalker", MonkWWSpells);
            DefensiveSpells.Add("Monk: Brewmaster", MonkBrewSpells);
            DefensiveSpells.Add("Monk: Mistweaver", MonkMWSpells);

            if (Class.Contains("Druid"))
            {
                Buffs.Add("Predatory Swiftness");
                Buffs.Add("Survival Instincts");
            }



            Aimsharp.PrintMessage("Supported skills: " + String.Join(", ", DefensiveSpells[Class].ToArray()));
            foreach (string s in DefensiveSpells[Class])
            {
                Spellbook.Add(s);
            }

        }


        public override bool CombatTick()
        {
            int Health = Aimsharp.Health("player");

            switch (Class)
            {
                case "Warrior: Fury":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Victory Rush"))
                        {
                            Aimsharp.Cast("Victory Rush");
                            return true;
                        }
                        if (Aimsharp.CanCast("Enraged Regeneration", "player"))
                        {
                            Aimsharp.Cast("Enraged Regeneration", true);
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Ignore Pain", "player"))
                        {
                            Aimsharp.Cast("Ignore Pain", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Spell Reflection", "player"))
                        {
                            Aimsharp.Cast("Spell Reflection", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Rallying Cry", "player"))
                        {
                            Aimsharp.Cast("Rallying Cry");
                            return true;
                        }
                    }
                    break;
                case "Warrior: Arms":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Victory Rush"))
                        {
                            Aimsharp.Cast("Victory Rush");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Ignore Pain", "player"))
                        {
                            Aimsharp.Cast("Ignore Pain", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Spell Reflection", "player"))
                        {
                            Aimsharp.Cast("Spell Reflection", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Rallying Cry", "player"))
                        {
                            Aimsharp.Cast("Rallying Cry");
                            return true;
                        }
                        if (Aimsharp.CanCast("Die by the Sword", "player"))
                        {
                            Aimsharp.Cast("Die by the Sword", true);
                            return true;
                        }
                    }
                    break;
                case "Warrior: Protection":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Victory Rush"))
                        {
                            Aimsharp.Cast("Victory Rush");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Ignore Pain", "player"))
                        {
                            Aimsharp.Cast("Ignore Pain", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Spell Reflection", "player"))
                        {
                            Aimsharp.Cast("Spell Reflection", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Demoralizing Shout", "player"))
                        {
                            Aimsharp.Cast("Demoralizing Shout");
                            return true;
                        }
                        if (Aimsharp.CanCast("Shield Wall", "player"))
                        {
                            Aimsharp.Cast("Shield Wall", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Last Stand", "player"))
                        {
                            Aimsharp.Cast("Last Stand", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Rallying Cry", "player"))
                        {
                            Aimsharp.Cast("Rallying Cry");
                            return true;
                        }
                    }
                    break;
                case "Death Knight: Frost":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Death Strike"))
                        {
                            Aimsharp.Cast("Death Strike");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Death Pact", "player"))
                        {
                            Aimsharp.Cast("Death Pact");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Anti-Magic Shell", "player"))
                        {
                            Aimsharp.Cast("Anti-Magic Shell");
                            return true;
                        }
                    }
                    break;
                case "Death Knight: Unholy":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Death Strike"))
                        {
                            Aimsharp.Cast("Death Strike");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Death Pact", "player"))
                        {
                            Aimsharp.Cast("Death Pact");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Anti-Magic Shell", "player"))
                        {
                            Aimsharp.Cast("Anti-Magic Shell");
                            return true;
                        }
                    }
                    break;
                case "Death Knight: Blood":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Death Strike"))
                        {
                            Aimsharp.Cast("Death Strike");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Death Pact", "player"))
                        {
                            Aimsharp.Cast("Death Pact");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Anti-Magic Shell", "player"))
                        {
                            Aimsharp.Cast("Anti-Magic Shell");
                            return true;
                        }
                    }
                    break;
                case "Druid: Feral":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Regrowth", "player") && Aimsharp.HasBuff("Predatory Swiftness"))
                        {
                            Aimsharp.Cast("Regrowth");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Barkskin", "player"))
                        {
                            Aimsharp.Cast("Barkskin", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Survival Instincts", "player") && !Aimsharp.HasBuff("Survival Instincts"))
                        {
                            Aimsharp.Cast("Survival Instincts");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {

                    }
                    break;
                case "Druid: Guardian":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Frenzied Regeneration", "player"))
                        {
                            Aimsharp.Cast("Frenzied Regeneration");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Barkskin", "player"))
                        {
                            Aimsharp.Cast("Barkskin", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Survival Instincts", "player") && !Aimsharp.HasBuff("Survival Instincts"))
                        {
                            Aimsharp.Cast("Survival Instincts");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {

                    }
                    break;
                case "Druid: Balance":
                    if (Health < HealingHP)
                    {

                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Barkskin", "player"))
                        {
                            Aimsharp.Cast("Barkskin", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {

                    }
                    break;
                case "Druid: Restoration":
                    if (Health < HealingHP)
                    {

                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Barkskin", "player"))
                        {
                            Aimsharp.Cast("Barkskin", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {

                    }
                    break;
                case "Rogue: Assassination":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Crimson Vial", "player"))
                        {
                            Aimsharp.Cast("Crimson Vial");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Evasion", "player"))
                        {
                            Aimsharp.Cast("Evasion");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Cloak of Shadows", "player"))
                        {
                            Aimsharp.Cast("Cloak of Shadows");
                            return true;
                        }
                    }
                    break;
                case "Rogue: Outlaw":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Crimson Vial", "player"))
                        {
                            Aimsharp.Cast("Crimson Vial");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Evasion", "player"))
                        {
                            Aimsharp.Cast("Evasion");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Cloak of Shadows", "player"))
                        {
                            Aimsharp.Cast("Cloak of Shadows");
                            return true;
                        }
                    }
                    break;
                case "Rogue: Subtlety":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Crimson Vial", "player"))
                        {
                            Aimsharp.Cast("Crimson Vial");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Evasion", "player"))
                        {
                            Aimsharp.Cast("Evasion");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Cloak of Shadows", "player"))
                        {
                            Aimsharp.Cast("Cloak of Shadows");
                            return true;
                        }
                    }
                    break;
                case "Hunter: Marksmanship":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Exhilaration", "player"))
                        {
                            Aimsharp.Cast("Exhilaration");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Survival of the Fittest", "player"))
                        {
                            Aimsharp.Cast("Survival of the Fittest");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Aspect of the Turtle", "player"))
                        {
                            Aimsharp.Cast("Aspect of the Turtle");
                            return true;
                        }
                    }
                    break;
                case "Hunter: Survival":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Exhilaration", "player"))
                        {
                            Aimsharp.Cast("Exhilaration");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Survival of the Fittest", "player"))
                        {
                            Aimsharp.Cast("Survival of the Fittest");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Aspect of the Turtle", "player"))
                        {
                            Aimsharp.Cast("Aspect of the Turtle");
                            return true;
                        }
                    }
                    break;
                case "Hunter: Beast Mastery":
                    if (Health < HealingHP)
                    {
                        if (Aimsharp.CanCast("Exhilaration", "player"))
                        {
                            Aimsharp.Cast("Exhilaration");
                            return true;
                        }
                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Survival of the Fittest", "player"))
                        {
                            Aimsharp.Cast("Survival of the Fittest");
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Aspect of the Turtle", "player"))
                        {
                            Aimsharp.Cast("Aspect of the Turtle");
                            return true;
                        }
                    }
                    break;
                case "Monk: Windwalker":
                    if (Health < HealingHP)
                    {

                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Fortifying Brew", "player"))
                        {
                            Aimsharp.Cast("Fortifying Brew", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Diffuse Magic", "player"))
                        {
                            Aimsharp.Cast("Diffuse Magic", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {
                        if (Aimsharp.CanCast("Touch of Karma"))
                        {
                            Aimsharp.Cast("Touch of Karma", true);
                            return true;
                        }
                    }
                    break;
                case "Monk: Brewmaster":
                    if (Health < HealingHP)
                    {

                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Fortifying Brew", "player"))
                        {
                            Aimsharp.Cast("Fortifying Brew", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {

                    }
                    break;
                case "Monk: Mistweaver":
                    if (Health < HealingHP)
                    {

                    }
                    if (Health < MitigationHP)
                    {
                        if (Aimsharp.CanCast("Fortifying Brew", "player"))
                        {
                            Aimsharp.Cast("Fortifying Brew", true);
                            return true;
                        }
                        if (Aimsharp.CanCast("Diffuse Magic", "player"))
                        {
                            Aimsharp.Cast("Diffuse Magic", true);
                            return true;
                        }
                    }
                    if (Health < ImmunityHP)
                    {

                    }
                    break;
            }


            return false;
        }

        public override bool OutOfCombatTick()
        {


            return false;
        }

    }
}
