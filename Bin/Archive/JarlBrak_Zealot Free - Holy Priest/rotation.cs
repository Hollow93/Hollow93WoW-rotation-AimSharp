using System.Linq;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API;

namespace AimsharpWow.Modules
{
    public class Priest : Rotation
    {
        #region Static Variables
        private static int[] Talents = new int[8];
        public static int SpellQueueWindow = new int();
        public static int Latency = new int();
        public static int QuickDelay = new int();
        public static int SlowDelay = new int();
        public static int PlayerHealth = new int();
        public static int TargetHealth = new int();
        public static bool UseCooldowns = new bool();
        public static bool SingleTargetMode = new bool();
        public static bool AutoStartMode = new bool();
        public static bool Moving = new bool();
        public static bool Mounted = new bool();
        public static bool Channeling = new bool();
        public static bool Casting = new bool();
        public static bool InParty = new bool();
        public static bool InCombat = new bool();
        public static bool DebugMode = new bool();
        public static string WoWSpec;
        public static bool HasBuffFae = new bool();
        public static bool HasBuffBoon = new bool();
        private static int CovenantID = new int();
        public static Color Covenant = Color.FromArgb(255, 179, 0);
        public static Color Damage = Color.FromArgb(255, 103, 0);
        public static Color Healing = Color.FromArgb(0, 204, 255);
        public static Color Cooldown = Color.FromArgb(0, 255, 179);
        #endregion
        #region Lists
        public static List<string> BuffsList = new List<string>
        {
            "Power Word: Shield","Spirit of Redemption","Prayer of Mending","Renew","Angelic Feather","Power Word: Fortitude"
        };
        public static List<string> DebuffsList = new List<string>
        {
            "Shadow Word: Pain","Weakened Soul"
        };
        public static List<string> SpellsList = new List<string>
        {
            
            "Power Word: Shield","Shadow Word: Death","Shadow Word: Pain","Angelic Feather","Holy Nova","Flash Heal","Prayer of Mending","Guardian Spirit",
            "Heal","Holy Fire","Holy Word: Sanctify","Smite","Renew","Symbol of Hope","Binding Heal","Holy Word: Chastise"
        };
        public static List<string> CovenantAbilities = new List<string>
        {
            "Mindgames",
            "Ascended Blast",
            "Ascended Nova",
            "Unholy Nova",
            "Fae Guardians"
        };
        public static List<string> MacroCommands = new List<string>
        {
            "AutoStart","UseCDs","AngelicFeatherSelf","SanctifyCursor","BodyandSoulSelf"
        };
        #endregion
        #region General Methods
        // Logger
        public static void Logger(string lines)
        {
            if (DebugMode)
            {
                string path = Directory.GetCurrentDirectory() + "\\Rotations\\JarlBrak_Zealot Free - Holy Priest\\";
                VerifyDir(path);
                string fileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_ZealotLogs.txt";
                try
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(path + fileName, true);
                    file.WriteLine(DateTime.Now.ToString() + ": " + lines);
                    file.Close();
                }
                catch (Exception) { }
            }
        }

        public static void VerifyDir(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                }
            catch{}
        }

        private void GetTalents()
        {
            // Starting at 1, 0 element not used for easy reference
            for (int i = 1; i <= 7; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    if (Aimsharp.Talent(i, j))
                    {
                        Talents[i] = new int();
                        Talents[i] = j;
                    }
                }
            }
        }
        #endregion
        #region Covenant Methods
        private bool NecroUnholyNova()
        {
            if (Aimsharp.EnemiesInMelee() > 3 && Aimsharp.CanCast("Unholy Nova", "player", false, true))
            {
                Aimsharp.Cast("Unholy Nova");
                Aimsharp.PrintMessage("Covenant - Unholy Nova cast in melee", Covenant);
                return true;
            }

            if (Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0)
            {
                Aimsharp.Cast("Unholy Nova");
                Aimsharp.PrintMessage("Covenant - Unholy Nova on target", Covenant);
                return true;
            }
            return false;
        }
        private bool KyrianAscendedBlast()
        {
            if (Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0 && Aimsharp.CanCast("Ascended Blast", "target", true, true))
            {
                Aimsharp.Cast("Ascended Blast");
                Aimsharp.PrintMessage("Covenant - Ascended Blast on target", Covenant);
                return true;
            }
            return false;
        }

        private bool KyrianAscendedNova()
        {
            if (Aimsharp.EnemiesInMelee() > 3 && Aimsharp.CanCast("Ascended Nova", "player", false, true))
            {
                Aimsharp.Cast("Ascended Nova");
                Aimsharp.PrintMessage("Covenant - Ascended Nova cast", Covenant);
                return true;
            }
            
            return false;
        }

        private bool VenthyrMindgames()
        {
            if (!Moving && Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0 && Aimsharp.CanCast("Mindgames", "target", true, true))
            {
                Aimsharp.Cast("Mindgames");
                Aimsharp.PrintMessage("Covenant - Mindgames on enemy target", Covenant);
                return true;
            }
            return false;
        }

        private bool BoonoftheAscended()
        {
            if (!Moving && UseCooldowns && Aimsharp.CanCast("Boon of the Ascended", "player", false, true))
            {
                Aimsharp.Cast("Boon of the Ascended");
                Aimsharp.PrintMessage("Covenant - Boon of the Ascended cast", Covenant);
                return true;
            }
            return false;
        }        

        private bool FaeGuardians()
        {
            if (UseCooldowns && Aimsharp.CanCast("Fae Guardians", "player", false, true))
            {
                Aimsharp.Cast("Fae Guardians");
                Aimsharp.PrintMessage("Covenant - Fae Guardians cast", Covenant);
                return true;
            }
            return false;
        }
        #endregion
        #region DPS Spell Methods
        private bool ShadowWordPain ()
        {
            if (Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0  && Aimsharp.CanCast("Shadow Word: Pain", "target", true, true))
            {
                if (!Aimsharp.HasDebuff("Shadow Word: Pain", "target")
                    || Aimsharp.DebuffRemaining("Shadow Word: Pain", "target", true) < 1000)
                {
                    Aimsharp.Cast("Shadow Word: Pain");
                    Aimsharp.PrintMessage("DPS - SW:P on target", Damage);
                    return true;
                }
            }
            return false;
        }

        private bool ShadowWordDeath()
        {
            if (Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0  && Aimsharp.TargetCurrentHP() < 3 && Aimsharp.CanCast("Shadow Word: Death", "target", true, false))
            {
                Aimsharp.Cast("Shadow Word: Death");
                Aimsharp.PrintMessage("SOLO - DPS - Shadow Word: Death on Target", Damage);
                return true;
            }

            return false;
        }

        private bool HolyWordChastise()
        {
            if (Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0  && Aimsharp.CanCast("Holy Word: Chastise", "target", true, true))
            {
                Aimsharp.Cast("Holy Word: Chastise");
                Aimsharp.PrintMessage("DPS - Chastise on target", Damage);
                return true;
            }
            return false;
        }

        private bool HolyFire()
        {
            if (!Moving && Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0  && Aimsharp.CanCast("Holy Fire", "target", true, true))
            {
                Aimsharp.Cast("Holy Fire");
                Aimsharp.PrintMessage("DPS - Holy Fire on target", Damage);
                return true;
            }
            return false;
        }

        private bool Smite()
        {
            if (!Moving && Aimsharp.TargetIsEnemy() && Aimsharp.Health("target") > 0  && Aimsharp.CanCast("Smite", "target", true, true))
            {
                Aimsharp.Cast("Smite");
                Aimsharp.PrintMessage("DPS - Smite on target", Damage);
                return true;
            }
            return false;
        }
        #endregion
        #region Holy Spell Methods
        private bool HealBestSerenityTarget(int healthThreshold)
        {
            if (Aimsharp.Health("target") <= healthThreshold && Aimsharp.CanCast("Holy Word: Serenity", "target", true, false))
            {
                Aimsharp.Cast("Holy Word: Serenity");
                Aimsharp.PrintMessage("Holy Word: Serenity on target (ST MODE)", Healing);
                return true;
            }
            return false;
        }

        private bool HealFlash(int healthThreshold)
        {
            if (Aimsharp.Health("target") <= healthThreshold && Aimsharp.CanCast("Flash Heal", "target", true, true))
            {
                Aimsharp.Cast("Flash Heal");
                Aimsharp.PrintMessage("Flash Heal on target (ST MODE with cooldowns on)", Healing);
                return true;
            }
            return false;
        }

        private bool HealMain(int healthThreshold)
        {
            if (Aimsharp.Health("target") <= healthThreshold && Aimsharp.CanCast("Heal", "target", true, true))
            {
                Aimsharp.Cast("Heal");
                Aimsharp.PrintMessage("Heal on target", Healing);
                return true;
            }
            return false;
        }

        private bool HealRenew(int healthThreshold)
        {
            if (Aimsharp.Health("target") <= healthThreshold
                && Aimsharp.CanCast("Renew", "target", true, true)
                && !Aimsharp.HasBuff("Renew", "target"))
            {
                Aimsharp.Cast("Renew");
                Aimsharp.PrintMessage("Renew on target (ST MODE)", Healing);
                return true;
            }
            return false;
        }

        private bool HealPoM(int healthThreshold)
        {
            if (Aimsharp.Health("target") <= healthThreshold && Aimsharp.CanCast("Prayer of Mending", "target", false, true))
            {
                Aimsharp.Cast("Prayer of Mending");   
                Aimsharp.PrintMessage("Prayer of Mending on target", Healing);
                return true;
            }
            return false;
        }

        private bool HealSanctifyCursor(int healthThreshold)
        {
            if (InParty
                && Aimsharp.Health("target") <= healthThreshold
                && Aimsharp.AlliesNearTarget() >= 2
                && Aimsharp.CanCast("Holy Word: Sanctify", "player", false, true))
            {
                Aimsharp.Cast("SanctifyCursor");
                Aimsharp.PrintMessage("Sanctify on cursor", Healing);
                return true;
            }
            return false;
        }

        private bool GuardianSpiritCD(int healthThreshold)
        {
            if (Aimsharp.Health("target") <= healthThreshold && Aimsharp.CanCast("Guardian Spirit", "target", true, true) && !Aimsharp.HasBuff("Guardian Spirit", "target"))
            {
                Aimsharp.Cast("Guardian Spirit");
                Aimsharp.PrintMessage("CD - Guardian Spirit on target (ST MODE)", Cooldown);
                return true;
            }
            return false;
        }

        private bool AngelicFeatherSelf()
        {
            if (Aimsharp.CanCast("Angelic Feather", "player", false, true) && !Aimsharp.HasBuff("Angelic Feather", "player"))
            {
                Aimsharp.Cast("AngelicFeatherSelf");
                Aimsharp.PrintMessage("Angelic Feather speed boost used", Color.White);
                return true;
            }
            return false;
        }

        private bool BodyandSoulSelf()
        {
            if (Aimsharp.CanCast("Power Word: Shield", "player", false, true) && !Aimsharp.HasDebuff("Weakened Soul", "player"))
            {
                Aimsharp.Cast("BodyandSoulSelf");
                Aimsharp.PrintMessage("Body and Soul (PW:S) speed boost used", Color.White);
                return true;
            }
            return false;
        }
        #endregion
        #region Holy Rotation Methods
        private bool SingleTargetModeRotation()
        {
            if (!Aimsharp.TargetIsEnemy() || TargetHealth == 0)
            {
                if (UseCooldowns)
                {
                    if (Aimsharp.CanUseTrinket(0) && !Moving)
                    {
                        Aimsharp.Cast("TopTrinket", true);
                        Aimsharp.PrintMessage("CD - Top Trinket used (ST MODE)", Cooldown);
                        return true;
                    }
                    if (Aimsharp.CanUseTrinket(1) && !Moving)
                    {
                        Aimsharp.Cast("BottomTrinket", true);
                        Aimsharp.PrintMessage("CD - Bottom Trinket used (ST MODE)", Cooldown);
                        return true;
                    }
                    if (GuardianSpiritCD(20)) return true;
                }
                if (HealSanctifyCursor(85)) return true;
                if (HealRenew(100)) return true;
                if (HealPoM(100)) return true;
                if (HealBestSerenityTarget(30)) return true;
                if (!UseCooldowns) if (HealMain(85)) return true;
                if (HealFlash(60)) return true;
            }
            if (Aimsharp.TargetIsEnemy())
            {
                if (UseCooldowns && CovenantID == 3) if (FaeGuardians()) return true;
                if (UseCooldowns && CovenantID == 1) if (BoonoftheAscended()) return true;
                if (HasBuffFae && CovenantID == 3) if (ShadowWordPain()) return true;
                if (HasBuffBoon && CovenantID == 1) if (KyrianAscendedBlast()) return true;
                if (HasBuffBoon && CovenantID == 1) if (KyrianAscendedNova()) return true;
                if (CovenantID == 4) if (NecroUnholyNova()) return true;
                if (CovenantID == 2) if(VenthyrMindgames()) return true;
                if (ShadowWordDeath()) return true;
                if (HolyWordChastise()) return true;
                if (HolyFire()) return true;
                if (ShadowWordPain()) return true;
                if (Smite()) return true;
            }
            return false;
        }
        #endregion
        #region Aimsharp Methods
        public override void LoadSettings()
        {
            Settings.Add(new Setting("Zealot Free Edition - A Holy Priest rotation by JarlBrak"));
            Settings.Add(new Setting("Warning, log files fill up fast. Only use this if directed by JarlBrak!"));
            Settings.Add(new Setting("DEBUG MODE", false));
            Settings.Add(new Setting("Internet Lag Settings"));
            Settings.Add(new Setting("Spell Queue Window", 0, 400, 10));
            Settings.Add(new Setting("Latency", 0, 200, 60));
            Settings.Add(new Setting("Quick Delay", 50, 200, 125));
            Settings.Add(new Setting("Slow Delay", 200, 500, 350));
        }

        public override void Initialize()
        {
            Aimsharp.PrintMessage("-------------------------------------------------------------------------", Color.White);
            Aimsharp.PrintMessage("Welcome to Zealot Free Edition 1.3 - A Priest rotation by JarlBrak", Color.White);
            Aimsharp.PrintMessage("-------------------------------------------------------------------------", Color.White);
            Aimsharp.PrintMessage("Please refer to the Discord server for setup info as well", Color.White);
            Aimsharp.PrintMessage("as open issues/bugs!", Color.White);
            Aimsharp.PrintMessage("-------------------------------------------------------------------------", Color.White);

            DebugMode = GetCheckBox("DEBUG MODE");
            SpellQueueWindow = GetSlider("Spell Queue Window");
            Latency = GetSlider("Latency");
            QuickDelay = GetSlider("Quick Delay");
            SlowDelay= GetSlider("Slow Delay");

            Aimsharp.Latency = Latency;
            Aimsharp.QuickDelay = QuickDelay;
            Aimsharp.SlowDelay = SlowDelay;

            Macros.Add("AngelicFeatherSelf", "/cast [@player] Angelic Feather");
            Macros.Add("BodyandSoulSelf", "/cast [@player] Power Word: Shield");
            Macros.Add("SanctifyCursor", "/cast [@cursor] Holy Word: Sanctify");

            SpellsList.ForEach(spell => Spellbook.Add(spell));
            CovenantAbilities.ForEach(spell => Spellbook.Add(spell));
            BuffsList.ForEach(buff => Buffs.Add(buff));
            DebuffsList.ForEach(debuff => Debuffs.Add(debuff));
            MacroCommands.ForEach(macroCommand => CustomCommands.Add(macroCommand));
        }

        public override bool CombatTick()
        {
            // Variables
            UseCooldowns = Aimsharp.IsCustomCodeOn("UseCDs");
            Moving = Aimsharp.PlayerIsMoving();
            PlayerHealth = Aimsharp.Health("player");
            TargetHealth = Aimsharp.Health("target");
            Mounted = Aimsharp.PlayerIsMounted();
            Channeling = Aimsharp.IsChanneling("player");
            Casting = Aimsharp.CastingRemaining("player") - SpellQueueWindow > Latency;
            HasBuffFae = Aimsharp.HasBuff("Fae Guardians", "player", true);
            HasBuffBoon = Aimsharp.HasBuff("Boon of the Ascended", "player", true);
            InCombat = true;

            if (Mounted || Channeling) return false;
            if (Moving && Talents[2] == 3) if (AngelicFeatherSelf()) return true;
            if (Moving && Talents[2] == 2) if (BodyandSoulSelf()) return true;
            if (SingleTargetModeRotation()) return true;
            return true;
        }

        public override bool OutOfCombatTick()
        {
            // Variables
            UseCooldowns = Aimsharp.IsCustomCodeOn("UseCDs");
            Moving = Aimsharp.PlayerIsMoving();
            AutoStartMode = Aimsharp.IsCustomCodeOn("AutoStart");
            Mounted = Aimsharp.PlayerIsMounted();
            Channeling = Aimsharp.IsChanneling("player");
            Casting = Aimsharp.CastingRemaining("player") - SpellQueueWindow > Latency;
            CovenantID = Aimsharp.CovenantID();
            
            GetTalents();
            if (Mounted || Channeling) return false;
            if (Moving && Talents[2] == 3) if (AngelicFeatherSelf()) return true;
            if (Moving && Talents[2] == 2) if (BodyandSoulSelf()) return true;
            if (AutoStartMode) if (SingleTargetModeRotation()) return true;

            InCombat = false;
            return true;
        }
        #endregion
    }
}