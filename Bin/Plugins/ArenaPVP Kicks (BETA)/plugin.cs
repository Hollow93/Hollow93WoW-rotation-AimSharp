using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AimsharpWow.API; //needed to access Aimsharp API

namespace AimsharpWow.Modules
{
    public class PvPKicks : Plugin
    {
        public class Enemy
        {
            public string Unit = "";
            public int HP = 0;
            public bool IsInterruptable = false;
            public bool IsChanneling = false;
            public int CastingID = 0;
            public int CastingRemaining = 0;
            public int CastingElapsed = 0;
            public int Range = 0;
            public string Spec = "none";

        public Enemy(string unit)
            {
                Unit = unit;
            }

            public void Update()
            {
                HP = Aimsharp.Health(Unit);

                if (HP == 0)
                {
                    IsInterruptable = false;
                    IsChanneling = false;
                    CastingID = 0;
                    CastingRemaining = 0;
                    CastingElapsed = 0;
                    Range = 0;
                    Spec = "none";
                }
                else
                {
                    IsInterruptable = Aimsharp.IsInterruptable(Unit);
                    IsChanneling = Aimsharp.IsChanneling(Unit);
                    CastingID = Aimsharp.CastingID(Unit);
                    CastingRemaining = Aimsharp.CastingRemaining(Unit);
                    CastingElapsed = Aimsharp.CastingElapsed(Unit);
                    Range = Aimsharp.Range(Unit);
                    Spec = Aimsharp.GetSpec(Unit);
                } 
            }
        }

        string[] immunes = { "Divine Shield", "Aspect of the Turtle" };
        string[] physical_immunes = { "Blessing of Protection" };
        string[] spell_immunes = { "Nether Ward", "Grounding Totem Effect", "Spell Reflection", "Anti-Magic Shell" };

        Random rng = new Random();
        Stopwatch RngTimer = new Stopwatch();
        Stopwatch LassoTimer = new Stopwatch();


        public override void LoadSettings()
        {
            Settings.Add(new Setting("Kick at milliseconds remaining", 100, 1500, 200));
            Settings.Add(new Setting("Kick channels after milliseconds", 50, 2000, 500));
            Settings.Add(new Setting("Minimum delay", 50, 2000, 500));
            List<string> ClassList = new List<string>(new string[] { "Hunter", "Demon Hunter", "Priest", "Shaman", "Paladin", "Death Knight", "Druid", "Warlock", "Mage", "Rogue", "Monk", "Warrior" });
            Settings.Add(new Setting("Class", ClassList, "Hunter"));
            Settings.Add(new Setting("Kick Arena1-3?", true));
            Settings.Add(new Setting("Kick from OoC?", true));
        }

        List<string> Interrupts = new List<string>();
        string Class = "";
        bool KickArena = true;
        bool OOCKicks = true;
        int KickValue = 0;
        int KickChannelsAfter = 0;
        int MinimumDelay = 0;

        Enemy Target = new Enemy("target");
        Enemy Focus = new Enemy("focus");
        Enemy Arena1 = new Enemy("arena1");
        Enemy Arena2 = new Enemy("arena2");
        Enemy Arena3 = new Enemy("arena3");
        List<Enemy> Enemies = new List<Enemy>();

        public override void Initialize()
        {
            Class = GetDropDown("Class");
            KickArena = GetCheckBox("Kick Arena1-3?");
            OOCKicks = GetCheckBox("Kick from OoC?");

            Enemies.Add(Target);
            Enemies.Add(Focus);
            Enemies.Add(Arena1);
            Enemies.Add(Arena2);
            Enemies.Add(Arena3);

            Aimsharp.PrintMessage("PvP Kicks Plugin");
            Aimsharp.PrintMessage("Automated PvP kicking for your target,focus,arena1-3!");
            Aimsharp.PrintMessage("Do not use this together with any other Kicks plugin!");
            Aimsharp.PrintMessage("Use this macro to hold your kicks for a number of seconds: /xxxxx SaveKicks #");
            Aimsharp.PrintMessage("For example: /xxxxx SaveKicks 5");
            Aimsharp.PrintMessage("will make the bot not kick anything for the next 5 seconds.");

            if (Class == "Hunter")
            {
                Interrupts.Add("Counter Shot");
                Interrupts.Add("Muzzle");
            }

            if (Class == "Rogue")
            {
                Interrupts.Add("Kick");
            }

            if (Class == "Priest")
            {
                Interrupts.Add("Silence");
            }

            if (Class == "Demon Hunter")
            {
                Interrupts.Add("Disrupt");
            }

            if (Class == "Shaman")
            {
                Interrupts.Add("Wind Shear");
            }

            if (Class == "Paladin")
            {
                Interrupts.Add("Rebuke");
            }

            if (Class == "Death Knight")
            {
                Interrupts.Add("Mind Freeze");
            }

            if (Class == "Druid")
            {
                Interrupts.Add("Skull Bash");
            }

            if (Class == "Warlock")
            {
                Interrupts.Add("Spell Lock");
            }

            if (Class == "Mage")
            {
                Interrupts.Add("Counterspell");
            }

            if (Class == "Monk")
            {
                Interrupts.Add("Spear Hand Strike");
            }

            if (Class == "Warrior")
            {
                Interrupts.Add("Pummel");
            }

            foreach (string Interrupt in Interrupts)
            {
                Spellbook.Add(Interrupt);
                Macros.Add(Interrupt + "arena1", "/cast [@arena1] " + Interrupt);
                Macros.Add(Interrupt + "arena2", "/cast [@arena2] " + Interrupt);
                Macros.Add(Interrupt + "arena3", "/cast [@arena3] " + Interrupt);
                Macros.Add(Interrupt + "focus", "/cast [@focus] " + Interrupt);
            }

        
            foreach (string immune in immunes)
            {
                Buffs.Add(immune);
            }

            foreach (string spell_immune in spell_immunes)
            {
                Buffs.Add(spell_immune);
            }

            foreach (string physical_immune in physical_immunes)
            {
                Buffs.Add(physical_immune);
            }

            CustomCommands.Add("SaveKicks");
        }

        int[] BigCCSpells =
        {
            118, //polymorphs
            161355,
            161354,
            161353,
            126819,
            61780,
            161372,
            61721,
            61305,
            28272,
            28271,
            277792,
            277787,
            51514, // hexes
            211015,
            211010,
            211004,
            210873,
            269352,
            277778,
            277784,
            20066, // repentance
            113724, // Ring of Frost
            209753, // cyclones
            33786,
            605, // mind control
            198898, // song of chi-ji
        };

        int[] SmallCCSpells =
        {
            5782, // fear
            30283, //shadowfury
            2637, //hibernate
            710, //banish
        };

        int[] BigDamageSpells =
        {
            116858, // chaos bolt
            265187, // demonic tyrant
            234153, //drain life
            48181, //haunt
            264106, //deathbolt
            6353, //soul fire
            325289, //decimating bolt
            321792, //impending catastrophe
            325640, //soul rot

            320674, //chain harvest
            305485, //lightning lasso

            263165, //void torrent
            325013, // boon of the ascended

            316958, //ashen hallow

            113656, //fists of fury
            
            203286, // greater pyroblast
            314793, //mirrors of torment
            30451, // arcane blast
            205021, //ray of frost

            274283, // full moon
            274282, //halfmoon     
            
            258925, //fel barrage
        };

        int[] SmallDamageSpells =
        {
            30108, // unstable affliction
            198590, //drain soul
            324536, //malefic rapture
            105174, // hand of guldan

            8092, //mind blast
            34914, // vampiric touch
            15407, //mind flay
            47540, //penance
            214621, //schism

            117952, //crackling jade lightning

            198013, //eye beam                     
            
            194153, //starfire
            274281, //new moon
        };

        int[] SpecialSpells =
        {
            691, //summon felhunter
            688, // summon imp
            712, //summon succubus
            111771, //demonic gateway
            300728, //door of shadows
            324631, //fleshcraft

            32375, // mass dispel
            323673, //mind games
            64901, //symbol of hope
            228260, //void eruption

            324220, //deathborne
            314791, //shifting power

            982, // revive pet
            
            191634, // stormkeeper

            326434, //kindred spirits
        };

        int[] BigHeals =
        {
            77472, // healing wave
            188070, // healing surge
            186263, // shadow mend
            8004, // healing surge
            8936, // regrowth
            82326, // holy light
            227344, // surging mist
            289022, // nourish
            1064, // chain heal
            289666, //greater heal
            740, //tranquility
            115175, //soothing mist
            191837, //essence font
            47540, //penance
        };

        int[] SmallHeals =
        {          
            116670, // vivify
            19750, // flash of light
        };





        int RandomDelay = 0;
        public override bool CombatTick()
        {
            foreach (Enemy t in Enemies)
            {
                t.Update();
            }

            if (!RngTimer.IsRunning)
            {
                RngTimer.Restart();
                RandomDelay = rng.Next(0, 500);
            }
            if (RngTimer.ElapsedMilliseconds > 10000)
            {
                RngTimer.Restart();
                RandomDelay = rng.Next(0, 500);
            }

            KickValue = GetSlider("Kick at milliseconds remaining");
            KickChannelsAfter = GetSlider("Kick channels after milliseconds");
            MinimumDelay = GetSlider("Minimum delay");

            bool IAmChanneling = Aimsharp.IsChanneling("player");
            int GCD = Aimsharp.GCD();
            float Haste = Aimsharp.Haste() / 100f;
            bool LineOfSighted = Aimsharp.LineOfSighted();
            bool NoKicks = Aimsharp.IsCustomCodeOn("SaveKicks");

            KickValue = KickValue + RandomDelay;
            KickChannelsAfter = KickChannelsAfter + RandomDelay;

            List<string> EnemySpecs = new List<string>() { Arena1.Spec, Arena2.Spec, Arena3.Spec };

            bool NoPriorityCasters = !(EnemySpecs.Contains("Mage: Fire") || EnemySpecs.Contains("Mage: Frost") || EnemySpecs.Contains("Mage: Arcane") || EnemySpecs.Contains("Warlock: Destruction") || EnemySpecs.Contains("Warlock: Affliction") || EnemySpecs.Contains("Druid: Balance") || EnemySpecs.Contains("Shaman: Elemental"));

            int LowestAllyHP = 0;
            List<int> Healths = new List<int>() { Aimsharp.Health("player"), Aimsharp.Health("party1"), Aimsharp.Health("party2") };

            while (LowestAllyHP == 0 && Healths.Count > 0)
            {
                LowestAllyHP = Healths.Min();
                Healths.Remove(Healths.Min());
            }

            int LowestEnemyHP = 0;
            List<int> EnemyHealths = new List<int>() { Aimsharp.Health("arena1"), Aimsharp.Health("arena2"), Aimsharp.Health("arena3") };

            while (LowestEnemyHP == 0 && EnemyHealths.Count > 0)
            {
                LowestEnemyHP = EnemyHealths.Min();
                EnemyHealths.Remove(EnemyHealths.Min());
            }


            if (IAmChanneling || LineOfSighted)
                return false;

            if (!NoKicks)
            {
                foreach (string Interrupt in Interrupts)
                {

                    foreach (Enemy t in Enemies)
                    {
                        if (Aimsharp.CanCast(Interrupt, t.Unit))
                        {
                            //always kick big cc spells and special spells
                            if ((BigCCSpells.Contains(t.CastingID) || SpecialSpells.Contains(t.CastingID)) && (!t.IsChanneling && t.CastingRemaining < KickValue || t.IsChanneling && t.CastingElapsed > KickChannelsAfter) && t.IsInterruptable && t.CastingElapsed > MinimumDelay)
                            {
                                if (t.Unit == "target")
                                {
                                    Aimsharp.Cast(Interrupt, true);
                                    return true;
                                }
                                else
                                {
                                    Aimsharp.Cast(Interrupt + t.Unit, true);
                                    return true;
                                }
                            }

                            //kick big damage spells if lowest hp ally is medium-low hp
                            if (BigDamageSpells.Contains(t.CastingID) && LowestAllyHP <= (NoPriorityCasters ? 95 : 75) && (!t.IsChanneling && t.CastingRemaining < KickValue || t.IsChanneling && t.CastingElapsed > KickChannelsAfter) && t.IsInterruptable && t.CastingElapsed > MinimumDelay)
                            {
                                if (t.Unit == "target")
                                {
                                    Aimsharp.Cast(Interrupt, true);
                                    return true;
                                }
                                else
                                {
                                    Aimsharp.Cast(Interrupt + t.Unit, true);
                                    return true;
                                }
                            }

                            //kick small damage spells if lowest hp ally is very low hp
                            if (SmallDamageSpells.Contains(t.CastingID) && LowestAllyHP <= (NoPriorityCasters ? 25 : 5) && (!t.IsChanneling && t.CastingRemaining < KickValue || t.IsChanneling && t.CastingElapsed > KickChannelsAfter) && t.IsInterruptable && t.CastingElapsed > MinimumDelay)
                            {
                                if (t.Unit == "target")
                                {
                                    Aimsharp.Cast(Interrupt, true);
                                    return true;
                                }
                                else
                                {
                                    Aimsharp.Cast(Interrupt + t.Unit, true);
                                    return true;
                                }
                            }

                            //kick big heals if an enemy is medium-low hp
                            if (BigHeals.Contains(t.CastingID) && LowestEnemyHP <= (NoPriorityCasters ? 85 : 65) && (!t.IsChanneling && t.CastingRemaining < KickValue || t.IsChanneling && t.CastingElapsed > KickChannelsAfter) && t.IsInterruptable && t.CastingElapsed > MinimumDelay)
                            {
                                if (t.Unit == "target")
                                {
                                    Aimsharp.Cast(Interrupt, true);
                                    return true;
                                }
                                else
                                {
                                    Aimsharp.Cast(Interrupt + t.Unit, true);
                                    return true;
                                }
                            }

                            //kick small heals if an enemy is very low hp
                            if (SmallHeals.Contains(t.CastingID) && LowestEnemyHP <= (NoPriorityCasters ? 25 : 5) && (!t.IsChanneling && t.CastingRemaining < KickValue || t.IsChanneling && t.CastingElapsed > KickChannelsAfter) && t.IsInterruptable && t.CastingElapsed > MinimumDelay)
                            {
                                if (t.Unit == "target")
                                {
                                    Aimsharp.Cast(Interrupt, true);
                                    return true;
                                }
                                else
                                {
                                    Aimsharp.Cast(Interrupt + t.Unit, true);
                                    return true;
                                }
                            }

                            //kick small cc spells if an ally or enemy is very low hp
                            if (SmallCCSpells.Contains(t.CastingID) && (LowestEnemyHP <= (NoPriorityCasters ? 25 : 5) || LowestAllyHP <= (NoPriorityCasters ? 25 : 5)) && (!t.IsChanneling && t.CastingRemaining < KickValue || t.IsChanneling && t.CastingElapsed > KickChannelsAfter) && t.IsInterruptable && t.CastingElapsed > MinimumDelay)
                            {
                                if (t.Unit == "target")
                                {
                                    Aimsharp.Cast(Interrupt, true);
                                    return true;
                                }
                                else
                                {
                                    Aimsharp.Cast(Interrupt + t.Unit, true);
                                    return true;
                                }
                            }
                        }
                    }

                }
            }

            return false;
        }

        public override bool OutOfCombatTick()
        {
            if (OOCKicks)
                return CombatTick();

            return false;
        }

    }
}