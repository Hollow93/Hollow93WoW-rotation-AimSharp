//DiscPriest -- Mythic+
using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Management;
using System.Threading;
using System.Globalization;
using AimsharpWow.API; //needed to access Aimsharp API
using AimsharpWow.Modules;

namespace AimsharpWow.Modules
{
    #region Inferno Framework Classes
    public class Enemy
    {
        public enum Range
        {
            RANGE5,
            RANGE8,
            RANGE15,
            RANGE25,
            RANGE40,
        }

        public int Enemies5 { get; private set; }
        public int Enemies8 { get; private set; }
        public int Enemies15 { get; private set; }
        public int Enemies25 { get; private set; }
        public int Enemies40 { get; private set; }
        public int EnemiesTotal { get; private set; }

        public Enemy(List<string> variables)
        {
            Enemies5 = int.Parse(variables[0]);
            Enemies8 = int.Parse(variables[1]);
            Enemies15 = int.Parse(variables[2]);
            Enemies25 = int.Parse(variables[3]);
            Enemies40 = int.Parse(variables[4]);
            EnemiesTotal = int.Parse(variables[5]);
        }

        public int GetValue(Range range)
        {
            switch (range)
            {
                case Range.RANGE5: return Enemies5;
                case Range.RANGE8: return Enemies8;
                case Range.RANGE15: return Enemies15;
                case Range.RANGE25: return Enemies25;
                case Range.RANGE40: return Enemies40;
                default: return 0;
            }
        }

        public string DebugString
        {
            get
            {
                return "[" + Enemies5 + "][" + Enemies8 + "][" + Enemies15 + "][" + Enemies25 + "][" + Enemies40 + "][" + EnemiesTotal + "]";
            }
        }

        public bool CheckEnemies(Range rangeMin, Range rangeThreshold, int maxCount = 0)
        {
            bool result = false;
            int rMin = GetValue(rangeMin);
            int rThreshold = GetValue(rangeThreshold);

            if(maxCount > 0)
            {
                if (rMin >= maxCount || rMin >= EnemiesTotal)
                    result = true;
            }
            else
            {
                if (rMin >= EnemiesTotal || rMin >= rThreshold)
                    result = true;
            }
            return result;
        }
    }
    public class Unit
    {
        // Properties
        #region Properties
        public int ID { get; private set; }
        public string UnitType { get; private set; }
        public string TargetString { get; private set; }
        public bool InCombat { get; private set; }
        public string Role { get; private set; }
        public int Health { get; private set; }
        public int HealthDefeceit { get; private set; }
        public int Mana { get; private set; }
        public int Power { get; private set; }
        public int Range { get; private set; }
        public int CastingID { get; private set; }
        public bool IsInterruptable { get; private set; }
        public bool IsChanneling { get; private set; }
        public int CastingElapsed { get; private set; }
        public int CastingRemaining { get; private set; }
        public bool Dispellable { get; set; }

        public int MaxHP 
        { 
            get
            {
                int hp = 0;
                if(UnitType.Equals("UNIT_RAID") || UnitType.Equals("UNIT_PARTY"))
                {
                    hp = UnitHPManager.Instance.GetUnitMaxHP(TargetString);
                }
                return hp;
            }
        }
        public int CurrentHP 
        { 
            get
            {
                return (MaxHP * Health) / 100;
            }
        }
        public int HPDefeceit 
        { 
            get
            {
                return MaxHP - CurrentHP;
            }
        }

        public bool IsTank
        {
            get { return Role == "TANK"; }
        }
        public bool IsHealer
        {
            get { return Role == "HEALER" || TargetString == "player"; }
        }
        #endregion

        // Helper Functions
        #region Helper Functions
        public bool SpellInRange(string spellName)
        {
            return Aimsharp.SpellInRange(spellName, TargetString);
        }
        public bool CheckRange(int range)
        {
            return Range <= range;
        }
        public bool HasBuff(string buffName, bool byPlayer = true, string type = "")
        {
            return Aimsharp.HasBuff(buffName, TargetString, byPlayer, type);
        }
        public int BuffStacks(string buffName, bool byPlayer = true)
        {
            return Aimsharp.BuffStacks(buffName, TargetString, byPlayer);
        }
        public int BuffRemaining(string buffName, bool byPlayer = true, string type = "")
        {
            return Aimsharp.BuffRemaining(buffName, TargetString, byPlayer, type);
        }
        public bool HasDebuff(string debuffName, bool byPlayer = true, string type = "")
        {
            return Aimsharp.HasDebuff(debuffName, TargetString, byPlayer, type);
        }
        public int DebuffStacks(string debuffName, bool byPlayer = true)
        {
            return Aimsharp.DebuffStacks(debuffName, TargetString, byPlayer);
        }
        public int DebuffRemaining(string debuffName, bool byPlayer = true, string type = "")
        {
            return Aimsharp.DebuffRemaining(debuffName, TargetString, byPlayer, type);
        }
        public string DebugString
        {
            get
            {
                return "[" + ID + "][" + UnitType + "][" + TargetString + "][combat: " + InCombat + "][" + Role + "][range: " + Range + "][health: " + Health + "][missing: " + HealthDefeceit + "][mana: " + Mana + "][" + Power +  "][" + CastingID + "][" + IsInterruptable + "][" + IsChanneling + "][" + CastingElapsed + "][" + CastingRemaining + "][" + MaxHP + "][" + CurrentHP + "][" + HPDefeceit + "]";
            }
        }
        #endregion

        #region Constructor
        public Unit(List<string> variables)
        {
            ID = int.Parse(variables[0]);
            UnitType = variables[1];
            TargetString = variables[2];
            InCombat = bool.Parse(variables[3]);
            Role = variables[4];
            Health = int.Parse(variables[5]);
            HealthDefeceit = int.Parse(variables[6]);
           // HPDefeceit = int.Parse(variables[7]);
           // CurrentHP = int.Parse(variables[8]);
           // MaxHP = int.Parse(variables[9]);
            Mana = int.Parse(variables[10]);
            Power = int.Parse(variables[11]);
            Range = int.Parse(variables[12]);
            CastingID = int.Parse(variables[13]);
            IsInterruptable = bool.Parse(variables[14]);
            IsChanneling = bool.Parse(variables[15]);
            CastingElapsed = int.Parse(variables[16]);
            CastingRemaining = int.Parse(variables[17]);
            Dispellable = false;
        }
        #endregion
    }

    public class UnitHPManager
    {
        public int GetUnitMaxHP(string unitID)
        {
            int result = 0;

            if (UnitMaxHPDict.ContainsKey(unitID))
            {
                result = UnitMaxHPDict[unitID];
            }

            return result;
        }

        public void AddUnitMaxHP(string unitID, int HP)
        {
            //Aimsharp.PrintMessage(unitID + " " + HP.ToString(), Color.Orange);
            if (UnitMaxHPDict.ContainsKey(unitID))
            {
                UnitMaxHPDict[unitID] = HP;
            }
            else
            {
                UnitMaxHPDict.Add(unitID, HP);
            }
        }


        private Dictionary<string, int> UnitMaxHPDict;

        public void Update(int updateInterval = 5)
        {
            bool updateTime = false;
            TimeSpan ts = DateTime.Now.Subtract(nextUpdate);

            TimeSpan ts2 = DateTime.Now.Subtract(prevUpdate);
            if (ts2.Milliseconds > 400)
            {
                prevUpdate = DateTime.Now;
            }
            else
            {
                return;
            }

            //Aimsharp.PrintMessage(ts.Seconds.ToString(), Color.Orange);
            if (ts.Seconds > updateInterval)
            {
                updateTime = true;
                nextUpdate = DateTime.Now;
            }

            if (!UpdateStarted && updateTime)
            {
                Aimsharp.PrintMessage("UPDATE STARTED", Color.Orange);
                Raid = Aimsharp.InRaid();
                Party = Aimsharp.InParty();
                Size = Aimsharp.GroupSize();

                i = 0;

                if (Raid || Party)
                {
                    UpdateStarted = true;

                    if (Raid)
                    {
                        Aimsharp.Cast("FOC_raid1");
                        i = 1;
                    }
                    else if(Party)
                    {
                        Aimsharp.Cast("FOC_party1");
                        i = 1;
                    }
                }
            }
            else if (UpdateStarted)
            {
                string unit = "";

                Aimsharp.PrintMessage("UPDATE CONTINUING", Color.Orange);                

                if (Raid)
                {
                    if (i > Size)
                    {
                        UpdateStarted = false;
                        nextUpdate = DateTime.Now;
                        Aimsharp.PrintMessage("UPDATE FINISHED", Color.Orange);
                    }
                    else
                    {
                        unit = "raid" + i;
                        ++i;
                        Aimsharp.PrintMessage(unit, Color.Orange);
                        if (Utility.UnitFocus(unit))
                        {
                            Aimsharp.PrintMessage("Get HP", Color.Orange);
                            AddUnitMaxHP(unit, Aimsharp.UnitMaxHP("focus"));
                        }

                        unit = "raid" + i;
                        Aimsharp.Cast("FOC_" + unit);
                    }
                }
                else if (Party)
                {
                    if (i > Size - 1)
                    {
                        UpdateStarted = false;
                        nextUpdate = DateTime.Now;
                        Aimsharp.PrintMessage("UPDATE FINISHED", Color.Orange);
                    }

                    unit = "party" + i;
                    ++i;
                    Aimsharp.PrintMessage(unit, Color.Orange);
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.PrintMessage("M+ Get HP", Color.Orange);
                        AddUnitMaxHP(unit, Aimsharp.UnitMaxHP("focus"));
                    }

                    unit = "party" + i;
                    Aimsharp.Cast("FOC_" + unit);
                }
            }
        }

        private bool Raid = false;
        private bool Party = false;
        private int Size = 0;

        private bool UpdateStarted = false;
        private int i = 0;
        private DateTime nextUpdate;
        private DateTime prevUpdate;

        #region Singleton
        private UnitHPManager() 
        {
            UnitMaxHPDict = new Dictionary<string, int>();
            nextUpdate = DateTime.Now;
            prevUpdate = DateTime.Now;
        }
        private static UnitHPManager instance = null;
        public static UnitHPManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnitHPManager();
                }
                return instance;
            }
        }
        #endregion
    }

    public class DebugLog
    {
        static string filename = "";
        static string PreviousSpell = "";
        static string PreviousMessage = "";
        static DateTime PreviousTime = DateTime.Now.ToLocalTime();
        static int SpamProtection = 1000;
        public static void Initialize(string RotationPath, int spamProtection = 0)
        {
            filename = "Debug.txt";
            SpamProtection = spamProtection;
            System.IO.File.Create(filename);
        }

        public static void Log(string Spell, string CustomMessage, int CombatTime = 0)
        {
            DateTime currTime = DateTime.Now.ToLocalTime();
            if (PreviousSpell != "")
            {
                if (PreviousMessage.Equals(CustomMessage))
                {
                    if ((currTime - PreviousTime).TotalMilliseconds <= SpamProtection)
                        return;// CustomMessage = "SPAM";
                }
            }

            PreviousTime = currTime;
            PreviousSpell = Spell;
            PreviousMessage = CustomMessage;


            string debugLine = "";
            debugLine += "[" + currTime.ToString("HH:mm:ss") + "] ";
            debugLine += "[" + CombatTime.ToString() + "] ";
            debugLine += "[" + Spell.ToString() + "] ";
            debugLine += "[" + CustomMessage.ToString() + "] ";
            debugLine += "  [" + Aimsharp.LastCast().ToString() + "] ";
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true);
            file.WriteLine(debugLine);
            //Aimsharp.PrintMessage(debugLine);
            file.Flush();
            file.Close();
        }
    }

    static class Utility
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => (!Char.IsWhiteSpace(c) && c != ','))
                .ToArray());
        }

        public static float TimeToDeath(int HealthMax, int HealthCurrent, int CombatTimer)
        {
            if (HealthMax <= 0 || HealthCurrent <= 0) return 10000;
            float DPS = ((float)(HealthMax - HealthCurrent) * 1000) / (float)(CombatTimer * 0.001);
            if (DPS <= 0.1f) DPS = 0.1f;
            float time = (float)HealthCurrent * 1000 / DPS;
            if (time >= 10000) time = 10000;
            return time;
        }

        public static bool Check()
        {
            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/json";
            Random rnd = new Random();
            int myRandomNo = rnd.Next(10000000, 100000000);
            int ReqID = myRandomNo;
            bool Result = false;

             string json_data = "{\"hwid\": \"" + Aimsharp.GetHWID() + "\", \"rotation_id\": \"420\", \"request_id\": \"" + ReqID + "\"}";
             Aimsharp.PrintMessage(json_data, Color.Orange);

            string response = wc.UploadString("https://hook.integromat.com/ral2bkwyrjk1u3f455d5ivx5egoai4da", json_data);
            //Aimsharp.PrintMessage(response, Color.Orange);

            int r1 = (int)Math.Floor(ReqID * 0.0001f);
            int r2 = ReqID - r1 * 10000;
            int R = 84961584 + r2 * 10000 + r1;
            //Aimsharp.PrintMessage("r1: " + r1.ToString() + " r2: " + r2.ToString(), Color.Orange);
            //Aimsharp.PrintMessage("Request: " + ReqID.ToString() + " Expected Response: " + R.ToString(), Color.Orange);
            //Aimsharp.PrintMessage("ServerResp: " + response + " Expected Response: " + R.ToString(), Color.Orange);

            if (string.Equals(R.ToString(), response))
            {
                Aimsharp.PrintMessage("***************** Valid license *****************", Color.Green);
                Aimsharp.PrintMessage("You have a valid License, Thank you for your support .", Color.Pink);
                Result = true;
            }
            else
            {
                Aimsharp.PrintMessage("***************** Exipired / Invalid license *****************", Color.Red);
                Aimsharp.PrintMessage("Contact PizzaHelper for more info", Color.Red);
            }
            return Result;
        }
        public static bool CheckFree()
        {
           
                Aimsharp.PrintMessage("***************** FREE PIZZAA !!! RADICAL!!  *****************", Color.Green);
                //Aimsharp.PrintMessage("You have a valid License, Thank you for your support .", Color.Pink);
    

            return true;
        }

        public static Unit GetPlayer()
        {
            var temp = (List<string>)Aimsharp.ImportObject("PizzaHelper", "Player");

            return new Unit(temp);
        }

        public static Unit GetTarget()
        {
            var temp = (List<string>)Aimsharp.ImportObject("PizzaHelper", "Target");

            return new Unit(temp);
        }

        public static List<Unit> GetUnitList()
        {
            var temp = (List<List<string>>)Aimsharp.ImportObject("PizzaHelper", "UnitList");

            var UnitList = new List<Unit>();

            int Player = Aimsharp.CustomFunction("CheckPlayerPurify");
            int Party = Aimsharp.CustomFunction("CheckPartyPurifyGrouped");
            int Raid = Aimsharp.CustomFunction("CheckRaidPurifyGrouped");


            foreach (var tempUnit in temp)
            {
                var U = new Unit(tempUnit);

                if (U.UnitType.Equals("UNIT_PLAYER"))
                {
                    if (U.ID == 0 && Player == 1) 
                    {
                         //DebugLog.Log("[Dispell_Manager]  Player Debuff Detected!!" , " -- ID: " + U.ID);
                         U.Dispellable = true;
                    }
                }
                //DebugLog.Log("[Dispell_Manager]  Debugging info:" , " -- ID: " + U.ID);
                //DebugLog.Log("[Dispell_Manager]  Debugging info:" , " -- Unit Type: " + U.UnitType);
                //DebugLog.Log("[Dispell_Manager]  Debugging info: " , " -- Party: " + Party);
                if (U.UnitType.Equals("UNIT_PARTY"))
                {
                    if ((Party & (int)Math.Pow(2, U.ID)) == Math.Pow(2, U.ID))
                    {
                         //DebugLog.Log("[Dispell_Manager]  Party Debuff Detected!!" , " -- ID: " + U.ID);
                         U.Dispellable = true;
                    }
                }

                if (U.UnitType.Equals("UNIT_RAID"))
                {
                    if ((Raid & (int)Math.Pow(2, U.ID)) == Math.Pow(2, U.ID))
                    {
                        //DebugLog.Log("[Dispell_Manager]  Raid Debuff Detected!!" , " -- ID: " + U.ID);
                        U.Dispellable = true;
                    }
                }
                
                UnitList.Add(U);
            }

            return UnitList;
        }

        public static Enemy GetEnemyList()
        {
            var temp = (List<string>)Aimsharp.ImportObject("PizzaHelper", "EnemyList");

            return new Enemy(temp);
        }

        #region Focus Check M+
        public static bool UnitFocus(string unit)
        {
            int FocusUnit = Aimsharp.CustomFunction("UnitIsFocus");

         
            if ((FocusUnit == 1 && unit == "party1") ||
                (FocusUnit == 2 && unit == "party2") ||
                (FocusUnit == 3 && unit == "party3") || 
                (FocusUnit == 4 && unit == "party4") || 
                (FocusUnit == 99 && unit == "player"))
                return true;

           //int PlayerParty = Aimsharp.CustomFunction("PlayerisParty");
           //  if ((FocusUnit == 1 && unit == "party1") ||
           //    (FocusUnit == 2 && unit == "party2") ||
           //    (FocusUnit == 3 && unit == "party3") ||
           //    (FocusUnit == 4 && unit == "party4") ||
           //    (FocusUnit == 99 && unit == "player") ||
           //    (FocusUnit == 99 && unit == "party1" && PlayerParty == 1) ||
           //    (FocusUnit == 99 && unit == "party2" && PlayerParty == 2) ||
           //    (FocusUnit == 99 && unit == "party3" && PlayerParty == 3) ||
           //    (FocusUnit == 99 && unit == "party4" && PlayerParty == 4))
           //    return true;

            /*
            int PlayerRaid = Aimsharp.CustomFunction("PlayerIsRaid");
            if ((FocusUnit == 1 && unit == "raid1") ||
                (FocusUnit == 2 && unit == "raid2") ||
                (FocusUnit == 3 && unit == "raid3") ||
                (FocusUnit == 4 && unit == "raid4") ||
                (FocusUnit == 5 && unit == "raid5") ||
                (FocusUnit == 6 && unit == "raid6") ||
                (FocusUnit == 7 && unit == "raid7") ||
                (FocusUnit == 8 && unit == "raid8") ||
                (FocusUnit == 9 && unit == "raid9") ||
                (FocusUnit == 10 && unit == "raid10") ||
                (FocusUnit == 11 && unit == "raid11") ||
                (FocusUnit == 12 && unit == "raid12") ||
                (FocusUnit == 13 && unit == "raid13") ||
                (FocusUnit == 14 && unit == "raid14") ||
                (FocusUnit == 15 && unit == "raid15") ||
                (FocusUnit == 16 && unit == "raid16") ||
                (FocusUnit == 17 && unit == "raid17") ||
                (FocusUnit == 18 && unit == "raid18") ||
                (FocusUnit == 19 && unit == "raid19") ||
                (FocusUnit == 20 && unit == "raid20") ||
                (FocusUnit == 21 && unit == "raid21") ||
                (FocusUnit == 22 && unit == "raid22") ||
                (FocusUnit == 23 && unit == "raid23") ||
                (FocusUnit == 24 && unit == "raid24") ||
                (FocusUnit == 25 && unit == "raid25") ||
                (FocusUnit == 26 && unit == "raid26") ||
                (FocusUnit == 27 && unit == "raid27") ||
                (FocusUnit == 28 && unit == "raid28") ||
                (FocusUnit == 29 && unit == "raid29") ||
                (FocusUnit == 30 && unit == "raid30") ||
                (FocusUnit == 99 && unit == "player") ||
                (FocusUnit == 99 && unit == "raid1" && PlayerRaid == 1) ||
                (FocusUnit == 99 && unit == "raid2" && PlayerRaid == 2) ||
                (FocusUnit == 99 && unit == "raid3" && PlayerRaid == 3) ||
                (FocusUnit == 99 && unit == "raid4" && PlayerRaid == 4) ||
                (FocusUnit == 99 && unit == "raid5" && PlayerRaid == 5) ||
                (FocusUnit == 99 && unit == "raid6" && PlayerRaid == 6) ||
                (FocusUnit == 99 && unit == "raid7" && PlayerRaid == 7) ||
                (FocusUnit == 99 && unit == "raid8" && PlayerRaid == 8) ||
                (FocusUnit == 99 && unit == "raid9" && PlayerRaid == 9) ||
                (FocusUnit == 99 && unit == "raid10" && PlayerRaid == 10) ||
                (FocusUnit == 99 && unit == "raid11" && PlayerRaid == 11) ||
                (FocusUnit == 99 && unit == "raid12" && PlayerRaid == 12) ||
                (FocusUnit == 99 && unit == "raid13" && PlayerRaid == 13) ||
                (FocusUnit == 99 && unit == "raid14" && PlayerRaid == 14) ||
                (FocusUnit == 99 && unit == "raid15" && PlayerRaid == 15) ||
                (FocusUnit == 99 && unit == "raid16" && PlayerRaid == 16) ||
                (FocusUnit == 99 && unit == "raid17" && PlayerRaid == 17) ||
                (FocusUnit == 99 && unit == "raid18" && PlayerRaid == 18) ||
                (FocusUnit == 99 && unit == "raid19" && PlayerRaid == 19) ||
                (FocusUnit == 99 && unit == "raid20" && PlayerRaid == 20) ||
                (FocusUnit == 99 && unit == "raid21" && PlayerRaid == 21) ||
                (FocusUnit == 99 && unit == "raid22" && PlayerRaid == 22) ||
                (FocusUnit == 99 && unit == "raid23" && PlayerRaid == 23) ||
                (FocusUnit == 99 && unit == "raid24" && PlayerRaid == 24) ||
                (FocusUnit == 99 && unit == "raid25" && PlayerRaid == 25) ||
                (FocusUnit == 99 && unit == "raid26" && PlayerRaid == 26) ||
                (FocusUnit == 99 && unit == "raid27" && PlayerRaid == 27) ||
                (FocusUnit == 99 && unit == "raid28" && PlayerRaid == 28) ||
                (FocusUnit == 99 && unit == "raid29" && PlayerRaid == 29) ||
                (FocusUnit == 99 && unit == "raid30" && PlayerRaid == 30))
                return true;

            return false;
            */
            return false;
        }
        #endregion
    }

    public class CombatTimer
    {
        int lastTargetID = 0;
        int lastTargetMaxHP = 0;

        public float TimeToDeath(int targetID, int targetHP, int Time)
        {
            if (lastTargetID == 0 || lastTargetID != targetID)
            {
                lastTargetID = targetID;
                lastTargetMaxHP = targetHP;
            }

            return Utility.TimeToDeath(lastTargetMaxHP, targetHP, Time);
        }
    }
    #endregion

    //Healing Logic 
    class SimC
    {
        // Aimsharp Variables
        #region Aimsharp Varables 
        public Dictionary<string, int> PartyDict = new Dictionary<string, int>(){ };
        public Dictionary<string, int> BlacklistDict = new Dictionary<string, int>() { };
        public Dictionary<string, int> UnitHealthDict = new Dictionary<string, int>() { };
        public enum CleansePlayers
        {
            player = 1,
            raid1 = 2,
            raid2 = 4,
            raid3 = 8,
            raid4 = 16,
            raid5 = 32,
            raid6 = 64,
            raid7 = 128,
            raid8 = 256,
            raid9 = 512,
            raid10 = 1024,
            raid11 = 2048,
            raid12 = 4096,
            raid13 = 8192,
            raid14 = 16384,
            raid15 = 32768,
            raid16 = 65536,
            raid17 = 131072,
            raid18 = 262144,
            raid19 = 524288,
            raid20 = 1048576,
            raid21 = 2097152,
            raid22 = 4194304,
            raid23 = 8388608,
            raid24 = 16777216,
            raid25 = 33554432,
            raid26 = 67108864,
            raid27 = 134217728,
            raid28 = 268435456,
            raid29 = 536870912,
            raid30 = 1073741824,


        }
        public List<string> CleansePriorityList = new List<string>
        {
            "Above Healing",
            "Below Healing",
        };
        public List<string> CastingPrefrence = new List<string>
        {
            "Cursor",
            "Manual",
            "Player",
        };
         List<int> InstanceIDList = new List<int>
                {
            2296,
            1735,
            1744,
            1745,
            1746,
            1747,
            1748,
            1750,
            1755,
            1989,
            1990,
            1991,
            1992,
            1993,
            1994,
            1995,
            1996,
            1997,
            1998,
            1999,
            2000,
            2001,
            2002,
            2003,
            2004, 
            2441, 
            2450,
                };
        List<int> BattlegroundIDList = new List<int>
        {
            401,
            443,
            461,
            482,
            512,
            540,
            626,
            860,
            736,
            813,
            856,
            935,
            1010,
            1139,
            1140,
        };
        
        // Aimsharp Settings
        public bool UsePotion;
        public bool UseTopTrinket;
        public bool UseBottomTrinket;
        public bool TopTrinketDefensive;
        public bool BottomTrinketDefensive;
        public string TopTrinket;
        public string BotTrinket;
        public string RacialPower;
        public string PotionName;
        public Color col = Color.FromArgb(255, 135,206,235);
        public Color Error = Color.Red;
        public Color white = Color.White;
        public static bool DebugPrints = true;
        public int Latency;
        #endregion
        // Custom commands
        #region Custom Commands
        public bool pause;
        public bool dpsonly;
        public bool healonly;
        public bool oocheal;
        public bool SaveCooldowns;
        public bool nopurify;
        public bool nocds;
        public bool solo;
        public bool nofeather;
        public bool dktank;
        #endregion
        // Inferno Framework 
        #region Inferno Healing Variables 
        public int Time;
        public int Counter;
        public bool Fighting;
        public bool RangedFighting_20y;
        public bool RangedFighting_40y;
        public bool RangedFighting_30y;
        public bool InMelee;
        public bool Enemy;
        public int EnemiesInMelee;
        public int EnemiesNearTarget;
        public bool isCasting;
        public bool isCastingWaitSpells;
        public bool isCastingDPSSpells;
        public int RangeToTarget;
        public List<int> ActiveConduits;
        public float Haste;
        public int GCD;
        public int GCDMAX;
        public int TargetTimeToDie;
        public int TargetHealthPct;
        public int TargetMaxHealth;
        public int TargetCurrentHP;
        public int PlayerLevel;
        public int PlayerCurrentHP;
        public int PlayerCurrentHP_P;
        public float PlayerMaxHP;
        public bool Moving;
        public bool IsChanneling;
        public bool PlayerAlive;
        public bool TargetInCombat;
        public bool PlayerInCombat;
        public bool PartyInCombat;
        public int PlayerCastingID;
        public bool Channeling_SoothingMist;
        public bool Channeling_EF;
        public bool ThunderFocusActive;
        public string LastCast;
        public int CovenantID;
        public bool CovenantKyrian;
        public bool CovenantVenthyr;
        public bool CovenantNightFae;
        public bool CovenantNecrolord;
        public bool TargetIsEnemy;
        public bool PlayerDead;
        public string PlayerSpec;
        public bool PlayerMounted;
        public bool PlayerVehicle;
        // Healing Targets
        public string lowestHP1;
        public int AveragePartyHealth;
        public int AverageRaidHealth;
        public int lowestHP1_Range;
        public int HP_lowest1;
        public int HP_player;
        public string _tank;
        public int HP_tank;
        public float GCDfloat;
        public int PlayerIsRaid;
        public int PlayerIsParty;
        public int HPMomentum;
        public int HPRate;
        // Boss Logic Variables 
        public bool LostSoulActive;
        public bool BefouledBarrierActive;
        public bool CastingKingsMourne;
        public bool CastingHopeBreaker;
        public bool HasBefouldBarrier;
        public bool TargetIsAnduinsHope;
        public bool TargetIsWitheringSeed;
        // General buffs
        public bool BloodlustUp;
        public bool BeingSneaky;
        // Power
        public int Energy;
        public int HolyPower;
        public int MaxEnergy = 100;
        public int ManaPct;
        public int Mana;
        public int MaxMana;
        public int ManaDefeceit;
        public int SPELLS_CD = 0;
        public int SPELLS_CUSTOMIZATION = 0;
        public int GCD_DIFF = 100;
        public float SpellPower;
        public float Mastery;
        // Interrupt
        public bool Interruptable = false;
        public int CastingElapsed = 0;
        public bool AutoInterrupt = false;
        public bool BlindingLightAsInterupt = false;
        public bool HammerofJusticeAsInterupt = false;
        // Group Type
        public int PlayerGroupType = 0; // 0 = Player, 1 = Party, 2 = Raid
        public int DetoxUnitNumber = 0;
        // Weakaura Support
        public bool WeakauraSupport = false;
        //Movement Timer 
        public int MovementTimer = 0;
        public int StandingTimer = 0;
        public DateTime mvtick = DateTime.Now;
        public DateTime sttick = DateTime.Now;
        public static List<string> CastingLocation = new List<string>
        {
            "Manual",
            "Cursor",
            "Player",
        };
        #endregion
        #region Inferno Framework
        public int SpellCDRemains(String spell, bool CD = false)
        {
           return (CD && SaveCooldowns) ? 600000 : Aimsharp.SpellCooldown(spell) - GCD;
        }

        public int SpellCharges(String spell)
        {
            return Aimsharp.SpellCharges(spell);
        }

        public bool ASRotationAuto(String spell, string value = "Auto")
        {
            bool result = true;
            int i = 1;
            foreach (string s in SpellsList)
            {
                if (i > SPELLS_CUSTOMIZATION)
                    break;

                if (s.Equals(spell))
                {
                    result = false;
                    break;
                }
                ++i;
            }

            if (!result)
            {
                string automated = Settings.GetDropDown(spell);
                result = automated.Equals(value);
            }

            return result;
        }

        public bool ASSaveCooldownSpell(String spell)
        {
            bool result = true;

            if (SaveCooldowns)
            {
                int i = 1;
                foreach (string s in SpellsList)
                {
                    if (i > SPELLS_CD)
                        break;

                    if (s.Equals(spell))
                    {
                        result = !Settings.GetCheckBox(spell + " ");
                        break;
                    }
                    ++i;
                }
            }

            return result;
        }

        public bool ASCanCast(String spell, String unit = "target", bool CheckRange = true, bool CheckCasting = false, bool OffGCD = false)
        {
            return (Aimsharp.CanCast(spell, unit, CheckRange, CheckCasting) || (SpellCDRemains(spell) - GCD < GCD_DIFF)) && (OffGCD || GCD <= GCD_DIFF) &&
                ASRotationAuto(spell) && ASSaveCooldownSpell(spell);
        }

        public bool HasBuff(String buff, String unit = "player", bool byplayer = true)
        {
            return Aimsharp.HasBuff(buff, unit, byplayer);
        }
        public int HasBuffStacks(String buff, String unit = "player")
        {
            return Aimsharp.BuffStacks(buff, unit);
        }

        public bool HasDebuff(String debuff, String unit = "target", bool byplayer = true)
        {
            return Aimsharp.HasDebuff(debuff, unit, byplayer);
        }

        public int BuffRemaining(String buff, String unit = "player")
        {
            return Aimsharp.BuffRemaining(buff, unit) - GCD;
        }

        public int DebuffRemaining(String debuff, String unit = "target")
        {
            return Aimsharp.DebuffRemaining(debuff, unit) - GCD;
        }

        public bool ASCheckQueue()
        {
            bool Queued = false;
            // Check for Queued Spells from List
            for (int i = 0; i < SPELLS_CUSTOMIZATION; ++i)
            {
                string Spell = SpellsList[i];
                if (ASRotationAuto(Spell, "Queue"))
                {
                    Queued = Aimsharp.IsCustomCodeOn(Utility.RemoveWhitespace(Spell));
                    if (Queued)
                    {
                        Aimsharp.PrintMessage(Spell, Color.Green);
                    }

                    if (Queued && (SpellCDRemains(Spell) > 2000 || !ValidateSpell(Spell)))
                    {
                        Cast(Utility.RemoveWhitespace(Spell) + "Off");
                        return true;
                    }

                    if (Queued && (Aimsharp.CanCast(Spell) || (SpellCDRemains(Spell) - GCD < GCD_DIFF)) && CheckSpellTarget(Spell))
                    {
                        Aimsharp.PrintMessage("Queued " + Spell);
                        Cast(Spell);
                        return true;
                    }
                }
            }

            // Check for Queued Spells from Custom Spells
            {   
                
              
                
                // Mind Sooth
                Queued = Aimsharp.IsCustomCodeOn(Utility.RemoveWhitespace(MindSooth));
                if (Queued)
                {
                    Aimsharp.PrintMessage(MindSooth, Color.Green);
                }

                if (SpellCDRemains(MindSooth) > 2000 && Queued)
                {
                    Cast(MindSooth + "Off");
                    return true;
                }

                if (Queued && (Aimsharp.CanCast(MindSooth, "target") || (SpellCDRemains(MindSooth) - GCD < GCD_DIFF)))
                {
                    Aimsharp.PrintMessage("Queued " + MindSooth);
                    Cast(MindSooth);
                    return true;
                }

                //Mass Dispell
                Queued = Aimsharp.IsCustomCodeOn(Utility.RemoveWhitespace(MassDispel));
                if (Queued)
                {
                    Aimsharp.PrintMessage(MassDispel, Color.Green);
                }

                if (SpellCDRemains(MassDispel) > 2000 && Queued)
                {
                    Cast(MassDispel + "Off");
                    return true;
                }

                if (Queued && (Aimsharp.CanCast(MassDispel, "player") || (SpellCDRemains(MassDispel) - GCD < GCD_DIFF)))
                {
                    Aimsharp.PrintMessage("Queued " + MassDispel);
                    Cast(MassDispel);
                    return true;
                }

                //Mass Dispell @cursor 
                Queued = Aimsharp.IsCustomCodeOn(Utility.RemoveWhitespace(MassDispel+"C"));
                if (Queued)
                {
                    Aimsharp.PrintMessage(MassDispel+"C", Color.Green);
                }

                if (SpellCDRemains(MassDispel) > 2000 && Queued)
                {
                    Cast(MassDispel+"C" + "Off");
                    return true;
                }

                if (Queued && (Aimsharp.CanCast(MassDispel, "player") || (SpellCDRemains(MassDispel) - GCD < GCD_DIFF)))
                {
                    Aimsharp.PrintMessage("Queued " + MassDispel+"C");
                    Cast("MD_cursor");
                    return true;
                }

                

                
            } 

            return false;
        }

        public bool CanUseItem(String item, bool equipped = true)
        {
            return Aimsharp.CanUseItem(item, equipped);
        }

        public bool Cast(String spell)
        {
            Aimsharp.Cast(spell, true);
            return true;
        }

        public bool CastPreFight(List<string> Racials)
        {
            if (Fighting && !SaveCooldowns)
            {
                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player"))
                    {
                        return Cast(Racial);
                    }
                }
            }
            return false;
        }

        public bool CheckInterrupt()
        {
            /*
            AutoInterrupt = RotationMonk.GetCheckBox("Auto Interrupt:");
            ParalysisAsInterupt = RotationMonk.GetCheckBox("Use Paralysis as Interrupt:");

            if (AutoInterrupt && Interruptable && CastingElapsed > 500)
            {
                if (ASCanCast(SpearHandStrike, "target", true, false, true))
                {
                    Cast(SpearHandStrike);
                    Interruptable = false;
                    return false;
                }

                if (ParalysisAsInterupt && ASCanCast(Paralysis))
                {
                    return Cast(Paralysis);
                }
            }
            */
            return false;
        }
        // DF Talent
        public void CheckTalents(bool print = false)
        {
            // DF Talent
            ActiveTalents = new List<string>();
            int TalentCount = AllTalents.Count;
            for (int i = 0; i < (TalentCount / 20) + 1; ++i)
            {
                int talent = Aimsharp.CustomFunction("GetDFTalents" + (i+1).ToString());
                int startIndex = i * 20;

                for(int j = 0; j < 20; ++j)
                {
                    int p = (int)Math.Pow(2, j+1);
                    if ((talent & p) == p) ActiveTalents.Add(AllTalents.ElementAt(startIndex + j).Key);
                }
            }

            if (print)
            {
                foreach (string t in ActiveTalents)
                {
                    //Aimsharp.PrintMessage(t);
                    DebugLog.Log( "CheckTalents: ", t + "..... Found");
                }
            }
        }
        #endregion
        #region Healing Framework 
        
        public List<Unit> GetValidHealTargets()
        {
            List<Unit> healList = new List<Unit>();
            bool PlayerLostSoul = HasLostSoul("player");
            if (PlayerLostSoul)
            {
                //bool PlayerLostSoul = HasLostSoul("player");
                healList = UnitList.Where(x => (HasLostSoul(x.TargetString) == PlayerLostSoul)).ToList();

                // Get Heal units in Range, Alive, with MaxHP for smart heals
                var SmartHealUnits_Anduin = healList.Where(x => x.Health > 0 && x.MaxHP != 0 && x.Range <= 40 );
                if (SmartHealUnits_Anduin.Count() == 0)
                {
                    SmartHealUnits_Anduin = healList.Where(x => x.Health > 0 && x.Range <= 40);
                }

                healList = SmartHealUnits_Anduin.ToList();
            }
            else
            {
                // Get Heal units in Range, Alive, with MaxHP for smart heals
                healList = UnitList.Where(x => x.Health > 0 && x.MaxHP != 0 
                                            && x.Range <= 40).ToList();
                //Fall back to usingx.Health if we do not have a list with MaxpHPs   
                if (healList.Count() == 0)
                {
                    healList = UnitList.Where(x => x.Health > 0 && x.Range <= 40).ToList();
                }
            }

            return healList;
        }

        public bool HasLostSoul(string unit)
        {
            if(HasDebuff("Lost Soul", unit, false)) return true;
            return false;

        }
        public bool HasGloom(string unit)
        {
            if(HasDebuff("Gloom", unit, false) || HasBuff("Gloom", unit, false)) return true;
            return false;

        }
        
        public bool HasBefouledBarrier(string unit)
        {
            if(HasDebuff("Befouled Barrier", unit, false)) return true;
            return false;

        }

        public int AverageHP_RaidArea()
        {
            var Healing_Range = 40;
            
            //Get all Raid units in Range 
            var Raid_Units = UnitList.Where(x => x.Range <= Healing_Range);
            if(Raid_Units.Count() > 0)
            {
                return (int)Raid_Units.Average(x => x.Health);
            }
            return 0;
        }

        public DateTime HpMomentumTimer = DateTime.Now;
        public int PrevHealth = 0;
        public int AverageHP_PartyArea(ref int HP, ref int Rate)
        {
            var Healing_Range = 40;
            int PartyHealth = 100;

            if(Aimsharp.GroupSize() < 2)
            {
                PartyHealth = 100;

                return PartyHealth;
            }
            
            //Get all Raid units in Range 
            var Party_units = UnitList.Where(x => x.Health != 0 && x.Range <= Healing_Range && x.Role != "TANK");
            if(Party_units.Count() > 1)
            {
                PartyHealth = (int)Party_units.Average(x => x.Health);
            }

            if (PrevHealth == 0) PrevHealth = PartyHealth;

            TimeSpan ts = DateTime.Now.Subtract(HpMomentumTimer);
            if(ts.Seconds >= 2)
            {
                HP = PrevHealth - PartyHealth;
                Rate = ((PartyHealth - PrevHealth) * 100) / PrevHealth;
            }

            return PartyHealth;
        }
        public int AverageHP_RaidDefeceit()
        {
            var Healing_Range = 40;
            
            //Get all Raid units in Range 
            var Raid_Units = UnitList.Where(x => x.Range <= Healing_Range);
            if(Raid_Units.Count() > 0)
            {
                var avg = (int)Raid_Units.Average(x => x.Health);
                int avgdef = (100 - avg);

                return avgdef;
            }
            return 0;
        }
       
        public bool CriticalHealingNeeded()
        {
            var HealUnits = GetValidHealTargets();
            // Flash heal Near Death Tanks 

            int criticalhealing_pct = 0;
            int criticalhealing_pct_tank = 0;
            SettingsDictionary.TryGetValue(CriticalHealingP, out criticalhealing_pct);
            SettingsDictionary.TryGetValue(CriticalHealingPT, out criticalhealing_pct_tank);
            var NearDeathTanks = HealUnits.Where(x => x.Health != 0 && x.Health <= criticalhealing_pct_tank && x.Role == "TANK").OrderBy(x => x.Health);
            var NearDeathUnits = HealUnits.Where(x => x.Health != 0 && x.Health <= criticalhealing_pct && x.Role != "TANK").OrderBy(x => x.Health);
           
           if(NearDeathUnits.Count() > 0)
            {
                DebugLog.Log( "CriticalHealingNeeded", "NearDeathUnits Detected!!!!! ");
                DebugLog.Log( "CriticalHealingNeeded", "Low Unit:  " + NearDeathUnits.First().TargetString);
                // If Casting DPS spell Cancel Spell 
                // Need IsCastingDPS
                if(isCastingDPSSpells)
                {
                        DebugLog.Log("[CriticalHealingNeeded]", " Cancelling isCastingDPSSpells due to focus HP:  " + NearDeathUnits.First().Health);
                        Aimsharp.StopCasting();
                }
                // If current Heal target is not Critical Unit Stop and cast on Critical unit 
                if(CriticalHealing(NearDeathUnits.First().TargetString)) return true;
            }

           if(NearDeathTanks.Count() > 0)
            {
                DebugLog.Log( "CriticalHealingNeeded", "NearDeathTanks Detected!!!!! ");
                DebugLog.Log( "CriticalHealingNeeded", "Low Tank Unit:  " + NearDeathTanks.First().TargetString);
                // If Casting DPS spell Cancel Spell 
                // Need IsCastingDPS
                if(isCastingDPSSpells)
                {
                      DebugLog.Log("[CriticalHealingNeeded]", " Cancelling isCastingDPSSpells due to focus HP:  " + NearDeathTanks.First().Health);
                      Aimsharp.StopCasting();
                }
                // If current Heal target is not Critical Unit Stop and cast on Critical unit 
                if(CriticalHealing(NearDeathTanks.First().TargetString)) return true;
            }

            
            return false;
        }
        
        public bool CheckFocusDPS()
        {
            var HealUnits = GetValidHealTargets();
            int HP_player = Aimsharp.Health("player");
            //Damage If We are healthy above setting 
            int focusdps = 0;
            SettingsDictionary.TryGetValue(DPS_Focus_HP, out focusdps);
            var BelowFocusDPS = HealUnits.Where(x =>  x.Health != 0 && x.Health <= focusdps && x.Role != "TANK").OrderBy(x => x.Health);

            if(HP_player < 75)
            {
                DebugLog.Log( "CheckFocusDPS", "Low Unit Player :  " + HP_player);
                return false;
            }
            
           if(BelowFocusDPS.Count() > 0)
            {
                DebugLog.Log( "CheckFocusDPS", "Units below Focus DPS threshold  ");
                DebugLog.Log( "CheckFocusDPS", "Low Unit:  " + BelowFocusDPS.First().TargetString);
                // If Casting DPS spell Cancel Spell 
                // Need IsCastingDPS
                if(isCastingDPSSpells)
                {
                      DebugLog.Log("[CheckFocusDPS]", " Cancelling isCastingDPSSpells due to focus HP:  " + BelowFocusDPS.First().Health);
                      Aimsharp.StopCasting();
                }
              
              return false;
            }

            return true;
        }

        public bool CheckBursting(ref bool BurstingActive, ref int BurstingStacks)
        {
            string burst = "Burst";
            BurstingActive = false;
            BurstingStacks = 0;

            for(int i = 0; i < Aimsharp.GroupSize(); ++i)
            {
                string Target = "player";
                if(i != 0)
                {
                    Target = "party" + i.ToString();
                }

                int stacks = Aimsharp.DebuffStacks(burst, Target, false);
                if(stacks > BurstingStacks)
                {
                    BurstingActive = true;
                    BurstingStacks = stacks;
                }
            }

            return BurstingActive;
        }
        public bool TargetisSpitefull()
        {
            return false;
        }
        public bool TargetisExplosive()
        {
            int ExplosiveCheck = Aimsharp.CustomFunction("CheckMOExplosive");

            if(ExplosiveCheck == 1)
            {
                if(HandleExplosives())
                {
                    DebugLog.Log("[TargetisExplosive]", " HandleExplosives Complete ");
                    return true;
                }
            }
            
            return false;
        }
        public bool CheckQuaking(ref bool QuakingActive, ref int QuakingRemaining)
        {
          
            QuakingActive = false;
            QuakingRemaining = 0;


            QuakingActive = HasDebuff(Quake, "player", false);
            QuakingRemaining = DebuffRemaining(Quake,"player");
            
            return QuakingActive;
        }
        public bool TimetoFeed_Manager()
        {
            var HealUnits = GetValidHealTargets();
            var FeedUnits = HealUnits.Where(x => HasDebuff("Time to Feed", x.TargetString, false) ).OrderBy(x => x.Health);

             if(FeedUnits.Count() > 0)
                {
                    var unit = FeedUnits.First().TargetString;
                    if(CriticalHealing(unit))
                    {
                        DebugLog.Log( "[TimetoFeed_Manager] :  Critical Healing Unit with Time to Feed  ", unit);
                        return true;
                    }
                }

                return false;
        }
        public bool ArcaneTorrent_Manager()
        {
            int arcanetorrent_pct = 0;
            SettingsDictionary.TryGetValue(ArcaneTorrentP, out arcanetorrent_pct);

            if(ManaPct <= arcanetorrent_pct)
            {
                if(CanCast(ArcaneTorrent, "player"))
                {
                    Cast(ArcaneTorrent);
                    return true;
                }
            }

            return false;
        }
        public bool AutoEnterCombat()
        {
            var HealUnits = GetValidHealTargets();
            var AutoCombatUnits = HealUnits.Where(x => x.InCombat).OrderBy(x => x.Health);

            if(AutoCombatUnits.Count() > 1 && !PlayerInCombat)
            {
                DebugLog.Log("[AutoEnterCombat]", " Player not in Combat:  " + PlayerInCombat );
                DebugLog.Log("[AutoEnterCombat]", " Party Memebers in Combat Detected:  " + AutoCombatUnits.Count() );
                Cast("StartAttack");
                return true;
            }
           
            return false;
        }
        public void HealingIndicator(int MA_HP, int MA_HR)
        {
            Aimsharp.PrintMessage("HealingIndicator  ==> Values : " + MA_HP + "  Rate: " + MA_HR, Color.White);
             DebugLog.Log( "==============================    HealingIndicator  " +  MA_HP + "    " + MA_HR +"       ", "==============================");
        }

        public int DamagedUnits()
        {
            var HealUnits = GetValidHealTargets();
            var LowestParty_Heal = HealUnits.Where(x => x.Health < 100).OrderBy(x => x.Health);

            return LowestParty_Heal.Count();
        }
        #endregion
        #region Common Spells / Varaibles / Settings
        // Common Spells
        public const string ArcaneTorrent = "Arcane Torrent";
        public const string Quake = "Quake";
        // Common Constants 

        // Common Debuffs
        public int DiseasePoisonMagicCheck;
        // M+ Logic 
        public bool BurstingActive;
        public int BurstingStacks;
        public bool QuakingActive;
        public int QuakingRemaining;

        //Common Settings 
        
        //Items
        //public const string TopTrinketThreshold = "Top Trinket Health %";
        //public const string BottomTrinketThreshold = "Bottom Trinket Health %";
        public const string HealthStoneThreshold = "Healthstone Health %";
        public const string ManaPot = "Mana Pot (Instant) Mana %";
        public const string HealthPot = "Cosmic Healing Potion Health %";
        // Urgent Healing Thresholds 
        public const string URGENT_TANK = "Heal Tanks ASAP      || HP%";
        public const string URGENT_RAID = "Heal Raid Unit ASAP      || HP%";
        public const string CriticalHealingPT   = "Heal Critical Tanks  | HP%";
        public const string CriticalHealingP    = "Heal Critical Units  | HP%";

        //Racials 
        public const string ArcaneTorrentP      = "Use Arcane Torrent   | Mana%";
        /*
        public const string TTKHP = "Save CDs if Target below HP(in thousands)";
        public const string TTKT = "Save CDs if TTK(Target) below seconds";
        public const string TTKPackHP = "Save CDs if Pack below HP(in thousands)";
        public const string TTKPackT = "Save CDs if TTK(Pack) below seconds";
        */
      
        #endregion
        //// Update Function ////
        #region UPDATE FUNCTION        
        public void update(bool ooc = false)
        {
            /// <summary>
            ///  This class performs an important function.
            /// </summary>
            #region InfernoCore Updates
            // Aimsharp Settings
            //UseTopTrinket = Settings.GetCheckBox("Use Top Trinket:");
            //UseBottomTrinket = Settings.GetCheckBox("Use Bottom Trinket:");
            //TopTrinketDefensive = Settings.GetCheckBox("Top Trinket Defensive:");
            //BottomTrinketDefensive = Settings.GetCheckBox("Bottom Trinket Defensive:");
            //UsePotion = Settings.GetCheckBox("Use DPS Potion:");
            //PotionName = Settings.GetString("Potion name:");

            // Plugin
            Player = Utility.GetPlayer();
            Target = Utility.GetTarget();
            UnitList = Utility.GetUnitList();
            // Should we throw an error When these fail to init? 
            EnemyList = Utility.GetEnemyList();


            #region Update Debug
            // Aimsharp.PrintMessage("Unit List DEBUG" + UnitList, Color.Orange);
            if((Aimsharp.CombatTime() % 1000) == 0)
            {
                UnitList.ToList().ForEach(p => DebugLog.Log("UnitDEBUG","Entry :" + p.DebugString));
            }
            #endregion
            // Custom commands
            #region WeakAuras Support  
            
           
            if (WeakauraSupport)
            {
                int i = Aimsharp.CustomFunction("GetCVars");
                pause = ((i & 1) == 1);
                dpsonly = ((i & 2) == 2);
                healonly = ((i & 4) == 4);
                //fcooc  = ((i & 8) == 8);
                oocheal = ((i & 16) == 16);
                nopurify = ((i & 32) == 32);
                nocds = ((i & 64) == 64);

            }
            else
            {
                pause = Aimsharp.IsCustomCodeOn("pause");
                dpsonly = Aimsharp.IsCustomCodeOn("dpsonly");
                healonly = Aimsharp.IsCustomCodeOn("healonly");
                //fcooc = Aimsharp.IsCustomCodeOn("fcooc");
                oocheal = Aimsharp.IsCustomCodeOn("oocheal");
                nopurify = Aimsharp.IsCustomCodeOn("nopurify");
                nocds = Aimsharp.IsCustomCodeOn("nocds");
            
            }
            nofeather = Aimsharp.IsCustomCodeOn("nofeather");
            dktank = Aimsharp.IsCustomCodeOn("dktank");
           
            
            #endregion


            // Base simc 
            Time = Aimsharp.CombatTime();
            int TARGETRANGE = Aimsharp.Range("target");
            RangedFighting_40y = TARGETRANGE <= 40 && Aimsharp.TargetIsEnemy();
            RangedFighting_30y = TARGETRANGE <= 30 && Aimsharp.TargetIsEnemy();
            Fighting = TARGETRANGE <= 10 && Aimsharp.TargetIsEnemy();
            InMelee = TARGETRANGE <= 5 && Aimsharp.TargetIsEnemy();
            Enemy = Aimsharp.TargetIsEnemy();
            EnemiesInMelee = Aimsharp.EnemiesInMelee();
            EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            RangeToTarget = TARGETRANGE;
            isCasting = Aimsharp.CastingRemaining("player") > 0 ? true : false;
            ActiveConduits = Aimsharp.GetActiveConduits();
            PlayerDead = Aimsharp.PlayerIsDead();
            PlayerMounted = Aimsharp.PlayerIsMounted();
            PlayerVehicle = Aimsharp.PlayerInVehicle();
            PlayerSpec = Aimsharp.GetSpec();
            CheckTalents();

            // Player Buffs / Player Debuffs
            
            //if (NoAOE)
            //{
            //    EnemiesNearTarget = 1;
            //    EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            //}
            
            Haste = Aimsharp.Haste() / 100f;
            GCD = Aimsharp.GCD();
            GCD_DIFF = Aimsharp.CustomFunction("GetSpellQueueWindow");
            GCDMAX = (int)(1500f / (Haste + 1f));
            TargetTimeToDie = 1000000; //need to implement time to die estimation
            TargetHealthPct = Aimsharp.Health("target");
            TargetMaxHealth = Aimsharp.TargetMaxHP();
            TargetCurrentHP = Aimsharp.TargetCurrentHP();
            PlayerLevel = Aimsharp.GetPlayerLevel();
            PlayerCurrentHP = Aimsharp.UnitCurrentHP();
            PlayerMaxHP = Aimsharp.UnitMaxHP() * 1000;
            PlayerCurrentHP_P = Aimsharp.Health("player");
            ManaPct = Aimsharp.Mana("player");
            Moving = Aimsharp.PlayerIsMoving();
            IsChanneling = Aimsharp.IsChanneling("player");
            PlayerAlive = Aimsharp.Health("player") > 0;
            PlayerCastingID = Aimsharp.CastingID("player");
            // RAID -- PlayerIsRaid = Aimsharp.CustomFunction("PlayerIsRaid");

            //Aimsharp 
            Latency = Aimsharp.Latency;
            LastCast = Aimsharp.LastCast();
            //Coventants
            CovenantID = Aimsharp.CovenantID();
            CovenantKyrian = CovenantID == 1;
            CovenantVenthyr = CovenantID == 2;
            CovenantNightFae = CovenantID == 3;
            CovenantNecrolord = CovenantID == 4;
            TargetIsEnemy = Aimsharp.TargetIsEnemy();

            #region Combat Timer
            // Combat Timer
            dbg -= 334; // 3 ticks in 1 second
            int CurrentTargetID = Aimsharp.UnitID("target");
            if (dbg <= 0)
            {
                PackMaxHP = Aimsharp.CustomFunction("PackMaxHP") / 1000;
                PackCurrentHP = Aimsharp.CustomFunction("PackHP") / 1000;
                TimeToDeath = (int)cbt.TimeToDeath(Aimsharp.UnitID("target"), TargetCurrentHP, Time);
                TimeToDeathPack = (int)Utility.TimeToDeath(PackMaxHP, PackCurrentHP, Time);
                if (TimeToDeath > TimeToDeathPack) TimeToDeath = TimeToDeathPack;
                //Aimsharp.PrintMessage("TTD - Target: " + TimeToDeath.ToString() + " Pack: " + TimeToDeathPack.ToString() + "HP: " + PackCurrentHP.ToString() + "/" + PackMaxHP.ToString(), Color.Blue);
                dbg = 1000;
            }
            #endregion

            // Interupt
            Interruptable = Aimsharp.IsInterruptable("target");
            CastingElapsed = Aimsharp.CastingElapsed("target");

           

            #region Bloodlust Detection
            BloodlustUp = false;
            foreach (string BloodlustEffect in BloodlustEffects)
            {
                if (Aimsharp.HasBuff(BloodlustEffect))
                {
                    BloodlustUp = true;
                    break;
                }
            }
            #endregion

            // Enable TTK for SaveCooldowns
            // General Debuffs
            //HP's
            //HealingCore
            SpellPower = Aimsharp.CustomFunction("SpellPowerValue");
            Mastery = Aimsharp.CustomFunction("MasteryValue");
           
            int HPMomentum = 0;
            int HPRate = 0;
            AverageRaidHealth = AverageHP_RaidArea();
            AveragePartyHealth = AverageHP_PartyArea(ref HPMomentum, ref HPRate);

            #region Focus Variables
            #endregion
            TargetInCombat = (Aimsharp.InCombat("target") && Aimsharp.TargetIsEnemy()) || !InstanceIDList.Contains(Aimsharp.GetMapID());
            PartyInCombat = (Aimsharp.InCombat("party1") || Aimsharp.InCombat("party2") || Aimsharp.InCombat("party3") || Aimsharp.InCombat("party4")) ? true : false;
            PlayerInCombat = Aimsharp.CombatTime() > 0 || Aimsharp.InCombat("player");
            // General Debuffs
            #region Dispells 
            DiseasePoisonMagicCheck = Aimsharp.CustomFunction("DiseasePoisonMagicCheck");
            #endregion
            #region Tank Unit Variables
            //  Tank = findTank();
            // Offtank = findTank();
            //Tank_HP = Aimsharp.Health(Tank);
            //Offtank_HP = Aimsharp.Health(Offtank);
            //ActiveTank = findActiveTank();
            #endregion
            #region AOE Heal Variables
            // AOE Heals 
                //AutoAuraMastery = AOEHealUnits(GetSlider("Aura Mastery Units HP%")).Count;
                //LightofDawn_HPSValue = LightofDawn_HPSValue();
                //LightsHammer_HPSValue = LightsHammer_HPSValue();
            #endregion
           
            #region Boss Logics
            // Boss Logics 
            LostSoulActive = HasLostSoul("player");
            BefouledBarrierActive = HasBefouledBarrier("player");
            // https://www.wowhead.com/spell=362405/kingsmourne-hungers
            // If Andiun is casting KingsMourne Pool HP 
            CastingKingsMourne = Aimsharp.CastingID("target") == 362405;
            //https://www.wowhead.com/spell=365805/empowered-hopebreaker
            //https://www.wowhead.com/spell=361817/hopebreaker
            //https://www.wowhead.com/spell=361815/hopebreaker
            CastingHopeBreaker = Aimsharp.CastingID("target") == 361815 || Aimsharp.CastingID("target") == 361817 || Aimsharp.CastingID("target") == 365805;
            #endregion
            #region LOS Black List System
            // LOS Blacklist system 
            List<string> removals = new List<string>();
            foreach (KeyValuePair<string, int> entry in BlacklistDict)
            {
               if(Aimsharp.CombatTime() >= (entry.Value + GCDMAX))
                {
                    removals.Add(entry.Key);
                    Aimsharp.PrintMessage("Removed " + entry.Key + " from Blacklist - Then: " + entry.Value + " - Now: " + Aimsharp.CombatTime());
                }
            }
            foreach(string player in removals)
            {
                BlacklistDict.Remove(player);
            }
            removals.Clear();
             #endregion

            #region Movement + Standing Timers
            //Movement Timer 
            if(Aimsharp.PlayerIsMoving())
            {
                TimeSpan mvts = DateTime.Now.Subtract(mvtick);
                MovementTimer = mvts.Seconds;
            }
            else
            {
                mvtick = DateTime.Now;
                MovementTimer = 0;
            }

            //Standing Timer 
            if(!Moving)
            {
                TimeSpan stts = DateTime.Now.Subtract(sttick);
                StandingTimer = stts.Seconds;
            }
            else
            {
                sttick = DateTime.Now;
                StandingTimer = 0;
            }
            #endregion

            #region M+ Affix Logics
            // M+ Logic updates
            if(!Aimsharp.InRaid())
            {
                CheckBursting(ref BurstingActive, ref BurstingStacks);
                CheckQuaking( ref QuakingActive, ref QuakingRemaining);
                BeingSneaky = HasBuff("Invisible") || HasBuff("Shroud of Concealment", "player", false);
            }
            
            #endregion
            #endregion
            //////////////////////////////////
            /////    CLASS UPDATES      //////
            //////////////////////////////////
            #region Class Specfic Updates
            isCastingWaitSpells = Aimsharp.CastingID("player") == SmiteID || Aimsharp.CastingID("player") ==HolyFireID || Aimsharp.CastingID("player") ==HealID || Aimsharp.CastingID("player") ==FlashHealID ;
            //Get Spec
            SpecIsDisc = Aimsharp.GetSpec() == "Priest: Discipline";
            SpecIsHoly = Aimsharp.GetSpec() == "Priest: Holy";
          
            // Class Specific Updates 
            FlashConcentration_Status(ref HasFlashConcentration, ref FlashConcentrationRemaining, ref FlashConcentrationStacks);
            Lightweaver_Status(ref HasLightweaver, ref LightweaverRemaining, ref LightweaverStacks);
            Atonement_Status(ref AtonementCount);

            int SerenityCD = Aimsharp.CustomFunction("CheckSerenityCD");
            //bool ArcaneTorrentEnabled = Aimsharp.CustomFunction("CheckArcaneTorrent") > 0;
            //Aimsharp.PrintMessage(SerenityCD.ToString(), Color.Orange);
            isCastingDPSSpells = Aimsharp.CastingID("player") == SmiteID || Aimsharp.CastingID("player") == HolyFireID; 
            #endregion
          
           
        }
        #endregion
        //// Class / Spec Logic ////
        #region Class Spells / Varaibles / Settings 
        //Class Constants 

       
       
        // Class Vars 
        public bool SpecIsHoly;
        public bool SpecIsDisc;
        public bool HasFlashConcentration;
        public int FlashConcentrationStacks;
        public int FlashConcentrationRemaining;
        public bool HasLightweaver;
        public int LightweaverStacks;
        public int LightweaverRemaining;
        public int AtonementCount;
        

        //Class Spells

        //Common Priest Spells
        public const string AngelicFeather      = "Angelic Feather";
        public const string DesperatePrayer     = "Desperate Prayer";
        public const string DispelMagic         = "Dispel Magic";
        public const string Fade                = "Fade";
        public const string FlashHeal           = "Flash Heal";
        public const string Heal                = "Heal";
        public const string Renew               = "Renew";
        public const string HolyNova            = "Holy Nova";    
        public const string MassDispel          = "Mass Dispel";  
        public const string MindBlast           = "Mind Blast"; 
        public const string MindSooth           = "Mind Soothe";
        public const string PowerInfusion       = "Power Infusion";
        public const string PowerWordFortitude  = "Power Word: Fortitude";
        public const string PowerWordShield     = "Power Word: Shield";
        public const string PsychicScream       = "Pscyhic Scream";
        public const string ShadowWordDeath     = "Shadow Word: Death";
        public const string ShadowWordPain      = "Shadow Word: Pain";
        public const string Shadowfiend         = "Shadowfiend";
        public const string Smite               = "Smite";
        public const string HolyFire            = "Holy Fire";
        public const string SpiritofRedemption  = "Spirit of Redemption";
        public const string MindSear            = "Mind Sear";
        public const string MindControl         = "Mind Control";

        //Class Talents
        public const string FaeGuardians = "Fae Guardians";
        public const string GuardianFaerie = "Guardian Faerie";
        public const string BenevolentFaerie = "Benevolent Faerie";
        public const string WrathfulFaerie = "Wrathful Faerie";
        public const string Soulshape = "Soulshape";
        public const string CircleofHealing     = "Circle of Healing";
        public const string HolyWordSalvation   = "Holy Word: Salvation";
        public const string PrayerofMending     = "Prayer of Mending";
        public const string PrayerofHealing     = "Prayer of Healing";
        public const string HolyWordSanctify    = "Holy Word: Sanctify";
        public const string HolyWordSerenity    = "Holy Word: Serenity";
        public const string HolyWordChastise    = "Holy Word: Chastise";
        public const string GuardianAngel       = "Guardian Angel";
        public const string DivineStar          = "Divine Star";
        public const string PowerWordBarrier    = "Power Word: Barrier";
        public const string PurgetheWicked      = "Purge the Wicked";
        public const string Rapture             = "Rapture";
        public const string Schism              = "Schism";
        public const string Halo                = "Halo";
        public const string Lightwell           = "Lightwell";
        public const string PrayerCircle        = "Prayer Circle";
        public const string DivineWord          = "Divine Word";
        public const string ShadowCovenant      = "Shadow Covenant";
        public const string HarshDiscipline     = "Harsh Discipline";
        
        public const string TalentBodyandSoul   = "Body and Soul";
        public const string TalentCensure       = "Censure";
        // Spec Talents 
        
        public const string DivineHymn          = "Divine Hymn";
        public const string GuardianSpirit      = "Guardian Spirit";
        public const string Mindgames           = "Mindgames";
        public const string PowerWordLife       = "Power Word: Life";
        public const string Lightweaver         = "Lightweaver";
        public const string Apotheosis          = "Apotheosis";
        public const string SymbolofHope        = "Symbol of Hope";
        // Disc Priest Spells 
        public const string PainSuppression     = "Pain Suppression";
        public const string Penance            = "Penance";
        public const string PowerWordRadiance   = "Power Word: Radiance";
        public const string Purify              = "Purify";
        public const string EmpyrealBlaze       = "Empyreal Blaze";
        public const string TalentBorrowedTime  = "Borrowed Time";
        public const string Evangelism          = "Evangelism";
        public const string TalentMindbender    = "Mindbender";
        public const string TalentTwilightEquilibrium = "Twilight Equilibrium";
        public const string LightsWrath         = "Light's Wrath";


       
       

        // Class Buffs
        public const string Atonement = "Atonement";
        public const string FlashConcentration = "Flash Concentration";
        public const string SurgeofLight = "Surge of Light";
        public const string ResonantWords = "Resonant Words";
        public const string TrailofLight = "Trail of Light";
        public const string DivineConversation = "Divine Conversation";
        public const string WealandWoe = "Weal and Woe";
        // Class Debuffs
        #endregion
        


        #region Class UI Settings
         // Settings
        public const string PWR_Count                   = "PW:Radiance Count    |    ";
        public const string PWR_HP                      = "PW:Radiance          | HP%";
        public const string FH_HP                       = "Flash Heal           | HP%";

       // PW:L
        public const string PowerWordLife_Tank_HP       = "PW:Life Tank         | HP%";
        public const string PowerWordLife_DKTank_HP     = "PW:Life DK           | HP%";
        public const string PowerWordLife_Party_HP      = "PW:Life Party        | HP%";
        // Renew 
        public const string Renew_Tank                 = "Renew Tank           | HP%";
        public const string Renew_Party                = "Renew Party          | HP%";
        public const string Renew_OOC                  = "Renew OOC            | HP%";
        // Cooldowns 
        public const string PainSuppression_Tank        = "Pain Supp Tank       | HP%";
        public const string PainSuppression_DK          = "Pain Supp DK Tank    | HP%";
        public const string PainSuppression_Party       = "Pain Supp Party      | HP%";
        public const string Rapture_HP                  = "Auto Rapture         | AHP%";

        // Utility
        public const string SymbolofHopeP       = "Use Symbol of Hope   | Mana%";
        //Defensives
        public const string DesperatePrayerP    = "Use Desperate Prayer | HP%";
        public const string FadeP               = "Use Fade             | HP%";
        public const string DPS_Focus_HP        = "Focus DPS above %    | AHP%";
        public const string TurnOFF_AT          = "Disable Arcane Torrent for Mana";
        public const string Renew_HP            = "Renew                | HP%";
        public const string Renew_THP           = "Renew Tank           | HP%";
        public const string FlashHeal_HP        = "Flash Heal           | HP%";
        public const string FlashHeal_THP       = "Flash Heal Tank      | HP%";

        //public const string Sanctify_Placement  = "Sanctify placement Option";

        // Cooldowns 
        // Power word Life 

        //Holy Settings 
       

        

      


        #endregion
        #region Spell Lists & Spell Ingest
        public List<string> SpellsList;
        public List<string> Racials;
        public List<string> CovenantAbilities;
        public List<string> BloodlustEffects;
        public List<string> GeneralBuffs;
        public List<string> Trinkets;
        public List<string> Races;
        public List<string> GeneralDebuffs;
        public List<string> BuffsList;
        public List<string> DebuffsList;
        public List<string> TotemsList;
        public List<string> MacroCommands;
        public List<string> Items;
        public List<string> CVarList;
        public List<KeyValuePair<string, int>> Legendaries;
        // DF Talents
        public List<KeyValuePair<string, int>> AllTalents;
        public List<string> ActiveTalents;

        // Plugin
        Unit Player = null;
        Unit Target = null;
        List<Unit> UnitList = new List<Unit>();
        Enemy EnemyList = null;

        Dictionary<string, int> SettingsDictionary;

        RaidHealing Settings;

        // Combat Timer
        int dbg = 0;
        int PackMaxHP = 0;
        int PackCurrentHP = 0;
        CombatTimer cbt = new CombatTimer();

        // Time to Death
        int TimeToDeath = 10000;
        int TimeToDeathPack = 10000;

        public SimC(RaidHealing instance)
        {
            Settings = instance;

            SpellsList = new List<string>()
            {
                // Class Spells
                DesperatePrayer, Fade, FlashHeal,         
                HolyNova,HolyFire,MindBlast,MindSooth,
                PowerWordFortitude,PowerWordShield,PsychicScream,
                ShadowWordPain,Smite,Penance,
                //Class Talents
                Renew,DispelMagic, Shadowfiend, ShadowWordDeath,
                AngelicFeather,HolyNova, MassDispel, PowerInfusion, Fade,
                DivineStar, PowerWordLife, Mindgames, FaeGuardians,
                // Spec Talents
                PainSuppression,PowerWordRadiance,Schism,PowerWordBarrier,
                Rapture, PurgetheWicked,Evangelism, LightsWrath, TalentMindbender,ShadowCovenant,
              
                     
              

            };

            Racials = new List<string>
            {
                "Blood Fury","Berserking","Fireblood","Ancestral Call","Bag of Tricks", "Arcane Torrent",
            };


            BloodlustEffects = new List<string>
            {
                "Bloodlust","Heroism","Time Warp","Primal Rage","Drums of Rage"
            };

            GeneralBuffs = new List<string>
            {
                 "Refreshment", "Eat", "Drink", "Food & Drink", "Alter Time", "Forgeborne Reveries", 
                 "Lost Soul" /* Anduin */, "Befouled Barrier"/* Anduin */, "Gloom"/* Anduin */,
                 "Domination" /* Jailer */,
                 //M+ 
                 "Invisible", "Shroud of Concealment", // Shroud and invisible
            };

            GeneralDebuffs = new List<string>
            {
                "Frozen Binds","Frost Blast","Glacial Winds","Gripping Infection","Grasping Hands","Rapid Growth","Bramblethorn Entanglement","Glacial Spike","Web Wrap","Freezing Blast","Chains of Ice","Time Out","Rat Traps","Grasp of the Dead","Creeping Tendrils","Buzz-Saw","Shadow Surge","Wretched Phlegm","Wretched Poison","Constricting Memories","Calcify","Mind Flay","Sedative Dust","Ravenous Swarm","Two Left Feet","Spirit Shackles","Bind Soul","Cripple","Snaring Gore","Charged Anima","Torment Soul","Wracking Pain","Allergen Cloud","Mass Debilitate","Chilled","Sticky Muck","Wracking Torment","Crystal Eruption","Petrifying Breath","Tethering Shot Stacking Slow","Soulbreaker Trap","Screech From Beyond","Mass Slow","Crushing Stomp","Staggered","Seed of Doubt","Debilitating Beam","Snaring Blast","Tethering Shot","Noxious Cloud","Dark Binding","Hampering Strike","Hurled Charge","Shin Kick","Tethered","Curse of Lethargy","Keeper's Protection","Burst","Infectious Rain","Polymorph","Mortal Coil","Freezing Trap","Blinding Light","Dragon's Breath","Mindgames","Hammer of Justice","Repentance","Deep Freeze","Seduction","Shadowfury","Capacitor Totem","Lightning Lasso","Ring of Frost","Entangling Roots","Feral Charge","Entrapment","Frost Nova","Disable","Void Tendrils","Earthgrab Totem","Mass Entanglement","Narrow Escape","Pin","Freeze","Void Tendril's Grasp","Earthgrab","Chains of Ice","Concussive Shot","Ice Trap","Frostbolt","Slow","Cone of Cold","Mind Flay","Deadly Throw","Crippling Poison","Frost Shock","Earthbind Totem","Piercing Howl","Hamstring","Hand of Hindrance","Disable","Flying Serpent Kick","Curse of Exhaustion","Sacrolash's Dark Strike","Cyclone","Belligerent Boast","Necrotic Wound","Quake","Possession","Mind Control","Grievous Wound","Fate Fragment","Curse of Lethargy","Wracking Torture","Rift Stare","Wracking Interrogation","Lumbering Stomp","Lumbering Roar","Chains of Damnation","Freezing Blast","Glacial Winds","Barbed Shackles","Bindings of Misery","Spiteful Fixation", "Befouled Barrier",
                "Lost Soul" /* Anduin */, "Befouled Barrier"/* Anduin */,"Gloom"/* Anduin */,
                "Domination" /* Jailer */, 
                // M+ 
                "Time to Feed", 
            };

            BuffsList = new List<string>
            {
                SurgeofLight, PrayerofMending, TrailofLight, Renew, FlashConcentration, Lightweaver,
                //Utility
                AngelicFeather, PowerWordFortitude, Atonement, Rapture,PowerWordShield,HarshDiscipline,WealandWoe,

            };

            DebuffsList = new List<string>
            {
              ShadowWordPain,ShadowWordDeath, PurgetheWicked,
              
            };

            MacroCommands = new List<string>
            {
               
                Utility.RemoveWhitespace(MindSooth), Utility.RemoveWhitespace(MindSooth) + "Off",
                Utility.RemoveWhitespace(MassDispel),Utility.RemoveWhitespace(MassDispel) + "Off", 
                Utility.RemoveWhitespace(MassDispel+"C"),Utility.RemoveWhitespace(MassDispel+"C") + "Off",
               
    
            };

            Items = new List<string>
            {
                "Healthstone", "Spiritual Healing Potion", "Phial of Serenity", "Cosmic Healing Potion",
            };

            CVarList = new List<string>
            {
                 "HolyPrPause", "HolyPrDPSOnly", "HolyPrHealOnly", "HolyPrFCOOC", "HolyPrOOCHeal", "HolyNoPurify","HolyNoCDs",
            };

           
            // DF Talents
            AllTalents = new List<KeyValuePair<string, int>>()
            {
                // Priest Class
                new KeyValuePair<string, int>(Renew, 139),
                new KeyValuePair<string, int>(DispelMagic, 528),
                new KeyValuePair<string, int>(Shadowfiend, 34433),
                new KeyValuePair<string, int>(ShadowWordDeath, 32379),
                new KeyValuePair<string, int>(AngelicFeather, 121536),
                new KeyValuePair<string, int>(HolyNova, 132157),
                new KeyValuePair<string, int>(MindControl, 605),
                new KeyValuePair<string, int>(MassDispel, 32375),
                new KeyValuePair<string, int>(PowerInfusion, 10060),
                new KeyValuePair<string, int>(TalentBodyandSoul, 64129),
                new KeyValuePair<string, int>(DivineStar, 110744),
                new KeyValuePair<string, int>(Mindgames, 375901),
                new KeyValuePair<string, int>(PowerWordLife, 373481),

                // Spec 
                new KeyValuePair<string, int>(Atonement, 81749 ),
                new KeyValuePair<string, int>(PainSuppression, 33206),
                new KeyValuePair<string, int>(PowerWordRadiance, 194509),
                new KeyValuePair<string, int>(Schism, 214621),
                new KeyValuePair<string, int>(PowerWordBarrier, 62618),
                new KeyValuePair<string, int>(Rapture, 47536),
                new KeyValuePair<string, int>(PurgetheWicked, 204197),
                new KeyValuePair<string, int>(TalentBorrowedTime, 200209),
                new KeyValuePair<string, int>(TalentTwilightEquilibrium, 390705),
                new KeyValuePair<string, int>(Evangelism, 246287),
                new KeyValuePair<string, int>(LightsWrath, 373178),
                new KeyValuePair<string, int>(TalentMindbender, 265202),
                new KeyValuePair<string, int>(ShadowCovenant, 314867),
                new KeyValuePair<string, int>(HarshDiscipline, 373180),
                
            };

            SettingsDictionary = new Dictionary<string, int>();
        }

        public void AddSetting(string settingName, int settingValue)
        {
            if (SettingsDictionary.ContainsKey(settingName))
            {
                SettingsDictionary.Remove(settingName);
            }

            SettingsDictionary.Add(settingName, settingValue);
        }
        #endregion
        #region CanCasts / Vaidate Spells / CheckTarget
        public bool CanCast(String spell, String unit = "target", bool CheckRange = true, bool CheckCasting = false)
        {
            bool result = false;
            if ((Aimsharp.LineOfSighted() || Aimsharp.NotFacing()) && LastCast.Equals(spell))
            {
                return result;
            }
            switch (spell)
            {   
                case MindSear:
                case MindBlast:
                case Penance:
                
                case ShadowWordPain: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && RangedFighting_40y; break;
                case PowerWordFortitude:
                case DesperatePrayer:
                case MindSooth:
                case Purify:
                case PowerWordShield:
                case ArcaneTorrent: result = ASCanCast(spell, unit, CheckRange, CheckCasting); break;
                
                // Casted Spells 
                case Smite: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && RangedFighting_40y && Aimsharp.CastingID("player") != SmiteID && !QuakingActive; break;
                case HolyFire: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && RangedFighting_40y && Aimsharp.CastingID("player") != HolyFireID&& !QuakingActive; break;
                case PrayerofHealing:result = ASCanCast(spell, unit, CheckRange, CheckCasting) && Aimsharp.CastingID("player") != PrayerofHealingID&& !QuakingActive; break;
                case Heal:result = ASCanCast(spell, unit, CheckRange, CheckCasting) && Aimsharp.CastingID("player") != HealID&& !QuakingActive; break;
                case FlashHeal: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && Aimsharp.CastingID("player") != FlashHealID && (!QuakingActive || HasBuff(SurgeofLight)); break;
                case PowerWordRadiance: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && Aimsharp.CastingID("player") != 194509 && !QuakingActive; break;
                // Covenants and Other Validated
                case HolyWordChastise:
                case EmpyrealBlaze:
                case Shadowfiend: 
                case ShadowWordDeath:
                case Mindgames:
                case Schism:
                case PurgetheWicked: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && ValidateSpell(spell) && RangedFighting_40y; break;
                case PainSuppression:
                case Renew:
                case PowerWordLife: 
                case DivineStar: 
                case AngelicFeather: 
                case SymbolofHope: 
                case Apotheosis:
                case Lightweaver:
                case FlashConcentration:
                case DispelMagic: 
                case MassDispel: 
                case HolyNova:
                case DivineHymn:
                case CircleofHealing:
                case HolyWordSalvation:
                case GuardianAngel:
                case PrayerofMending:
                case HolyWordSanctify:
                case LightsWrath:
                case ShadowCovenant: result = ASCanCast(spell, unit, CheckRange, CheckCasting) && ValidateSpell(spell); break;
            }
            return result;
        }

        public bool ValidateSpell(String spell)
        {
            bool Disable_ArcaneTorrent = Settings.GetCheckBox(TurnOFF_AT);
            bool result = true;
            result = ActiveTalents.Contains(spell);
            
            switch (spell)
            {
                case Schism: result = !Moving; break;
                case FlashHeal: result = !Moving; break;
                case PowerWordRadiance: result = !Moving; break;
                case Smite: result = !Moving; break;
                case LightsWrath: result = !Moving; break;
            }
            return result;
        }

        // Spell Que Condtions
        public bool CheckSpellTarget(String spell)
        {
            bool result = true;
            switch (spell)
            {   

                //case HammerofWrath: result = (HasBuff(AvengingWrath) || TargetHealthPct <= 20 || (Aimsharp.SpellCooldown("Ashen Hallow")/1000f >= 195) ) && (SpellCDRemains(HammerofWrath) - GCD < GCD_DIFF); break;
                //case BeaconofFaith:
                //case BeaconofLight: result =  !HasBuff(spell); break;
                default: break;
            }
            return result;
        }
        #endregion     
        #region Class Healing Constants
        // Holy Priest Fucntions
        #region Casting IDs
        public int FlashHealID = 2061;
        public int HealID = 2060;
        public int SmiteID = 585;
        public int HolyFireID = 14914;
        public int PrayerofHealingID = 596;
        public int BoonID = 325013;
        
        #endregion
        #endregion
        #region Class Functions
        public bool Bursting_Manager()
        {    
            DebugLog.Log("[Bursting_Manager]", "CHecking Bursting " + BurstingActive);
            DebugLog.Log("[Bursting_Manager]", "CHecking Stacks of Bursting " + BurstingStacks);
            //MD at 5 stacks 
            if(BurstingStacks >= 4)
            {
                

            }
            return false;
        }
        public bool CriticalHealing(string unit)
        {
            //PWS
            if(CanCast(PowerWordShield, unit) && PowerWordShield_Manager_Disc())
            {
                DebugLog.Log( "[CriticalHealing PowerWordShield_Manager_Disc]", " == Complete == ");
                return true;
            }
            // PWL 
            // use PowerWordLife instead of PS if up
            if(CanCast(PowerWordLife, unit))
            {
                if(PowerWordLife_Manager(true))
                {
                    DebugLog.Log("[PainSupression_Manager]  Using PowerWordLife_Manager to optmize PainSupression_Manager: " ,"== COMPLETE ==");
                    return true;
                }
            }
            // PainSuppression
            if(CanCast(PainSuppression, unit) && PainSupression_Manager())
            {
                DebugLog.Log( "[CriticalHealing PainSupression_Manager]", " == Complete == ");
                return true;
            }
            // Defensive Pennance 
            if(CanCast(Penance, unit))
            {
                DebugLog.Log( "[CriticalHealing Penance]", " == Complete == ");
                return CastPenance(unit);
            }
            if(CanCast(ShadowCovenant, unit))
            {
                DebugLog.Log( "[CriticalHealing ShadowCovenant]", " == Complete == ");
                return CastShadowCovenant(unit);
            }
            if(CanCast(FlashHeal, unit))
            {
                DebugLog.Log( "[CriticalHealing FlashHeal]", " == Complete == ");
                return CastFlashHeal(unit);
            }
            


            return false;
        }
        public bool UrgentSelf_Healing()
        {
            
            if(PlayerCurrentHP_P <= 35)
            {

                // Run Defensives 
                if(CanCast(DesperatePrayer, "player"))
                {
                    Cast(DesperatePrayer);
                    return true;
                }
                // Guardian if below setting 
                if(CanCast(GuardianSpirit, "player"))
                {
                    CastGuardianSpirit("player");
                    return true;
                }
                // HW Serenity
                if(CanCast(HolyWordSerenity, "player"))
                {
                    CastHolyWordSerenity("player");
                    return true;
                }
                // Circle of Healing
                if(CanCast(CircleofHealing, "player"))
                {
                    CastCircleofHealing("player");
                    return true;
                }

                
            }
            
            return false;
        }
        public bool UrgentTank_Healing(string Spell)
        {
            var HealUnits = GetValidHealTargets();
            // Flash heal Near Death Tanks 
            var NearDeathTanks = HealUnits.Where(x => x.Health <= 30 && x.Role == "TANK").OrderBy(x => x.Health);
            if(NearDeathTanks.Count() > 0)
            {
                if(CanCast(Spell, NearDeathTanks.First().TargetString))
                {
                    
                    switch(Spell)
                    {
                        //case FlashHeal: return CastFlashHeal(NearDeathTanks.First().TargetString);
                        //case Heal: return CastHeal(NearDeathTanks.First().TargetString);
                        //case HolyWordSerenity: return CastHolyWordSerenity(NearDeathTanks.First().TargetString);
                        default: return false;
                    }
                    
                }
            }
            return false;
        }
        public bool Quaking_Manager()
        {
             //DebugLog.Log("[Quaking_Manager] =====  Monitoring ====", "");
            if(QuakingActive)
            {
                // DebugLog.Log("[Quaking_Manager] =====  QuakingActive Start ====", " "+ QuakingActive);

                // Fisnish Cast if i think we can 
                if(isCasting)
                {
                    //DebugLog.Log("[Quaking_Manager] =====  isCasting Start ====", " "+ isCasting);
                    if(Aimsharp.CastingRemaining("player") > QuakingRemaining + 100 )
                    {
                        //stopcasting
                        DebugLog.Log("[Quaking_Manager] =====  QuakingRemaining ====", " "+ QuakingRemaining);
                        DebugLog.Log("[Quaking_Manager] =====  Stop Casting Cast Longer then Quake!! ====", " "+ Aimsharp.CastingRemaining("player"));
                        Aimsharp.Jump();
                        return true;
                    }
                }

                if(QuakingRemaining < 500 && isCasting)
                {
                    DebugLog.Log("[Quaking_Manager] =====  QuakingRemaining ====", " "+ QuakingRemaining);
                    DebugLog.Log("[Quaking_Manager] =====  isCasting ====", " "+ isCasting);
                    DebugLog.Log("[Quaking_Manager] =====  Stop Casting QUAKE ENDS SOON!!! ====", " "+ QuakingRemaining);
                    Aimsharp.Jump();
                    return true;
                }
                
            

                // Use SOL procs if poeple are low 
                if(HasBuff(SurgeofLight))
                {
                    if(SurgeofLight_Manager())
                    {
                        DebugLog.Log( "[OOC] SurgeofLight_Manager", "== Complete ==");
                        return true;
                    
                    }
                }
              

                // SWP during Qualing 
                if((!HasDebuff(ShadowWordPain) || DebuffRemaining(ShadowWordPain) < 5000) && CanCast(ShadowWordPain)) 
                {
                    Cast(ShadowWordPain);
                    return true;
                }
                //DebugLog.Log("[Quaking_Manager] =====   Quaking Detected -- No Action Needed ====", "");
                return false;
            }
            //DebugLog.Log("[Quaking_Manager] =====  No Quaking Detected ====", "");
            return false;
        }
        public bool UsePrimaryHealInstead()
        {
            var HealUnits = GetValidHealTargets();
            
            int fhhp = 0;
            SettingsDictionary.TryGetValue(FH_HP, out fhhp);
            var Lowest = HealUnits.Where(x =>  x.Role != "TANK" && x.Health <= fhhp && !HasBuff(Atonement, x.TargetString) ).OrderBy(x => x.Health);

            if(Lowest.Count() > 0)
            {
                if(CanCast(Heal, Lowest.First().TargetString))
                {
                    DebugLog.Log( "[UsePrimaryHealInstead] :  We could Use heal instead ", Lowest.ToList().First().DebugString);
                    return CastHeal(Lowest.First().TargetString);
                }
            }
            return false;
        }
        public bool HandleExplosives()
        {
            if(CanCast(PurgetheWicked) ) 
                {
                    DebugLog.Log("[TargetisExplosive]", " Casting PTW on Target for explosive:  ");
                    Cast("PTWMO");
                    return true;
                }
            return false;
        }

        
        #endregion
        #region Class Healing Functions
        // Holy Priest Fucntions
        // AOE Heals 
        
        public bool DPSUnderHeal_Manager()
        {
           
            if(isCastingDPSSpells && AveragePartyHealth <= 50)
            {
                DebugLog.Log("[DPSUnderHeal_Manager]", " Cancelling isCastingDPSSpells due to focus HP:  " + AveragePartyHealth );
                Aimsharp.StopCasting();
                return true;
            }
            return false;
        }
        // Buff Detection 
        public void FlashConcentration_Status(ref bool FCBuff_Status, ref int FCBuff_Remaining, ref int FCBuff_Stacks )
        {
            FCBuff_Status = false;
            FCBuff_Stacks = 0;
            if(HasBuff(FlashConcentration))
            {
                FCBuff_Status = true;
            }
            FCBuff_Remaining = (int) BuffRemaining(FlashConcentration)/1000;

            FCBuff_Stacks = HasBuffStacks(FlashConcentration);
        }
        public void Lightweaver_Status(ref bool LWBuff_Status, ref int LWBuff_Remaining, ref int LWBuff_Stacks )
        {
            LWBuff_Status = false;
            LWBuff_Stacks = 0;
            if(HasBuff(Lightweaver))
            {
                LWBuff_Status = true;
            }
            LWBuff_Remaining = (int) BuffRemaining(Lightweaver)/1000;

            LWBuff_Stacks = HasBuffStacks(Lightweaver);
        }

        // Holy Pirest heal Estimates 
        #region Heal Estimates
        public int Estimate_FlashHeal()
        {
            //  A fast spell that heals an ally for (203% of Spell power).
            double Estimate_FlashHeal = (SpellPower * 2.03f); // LOTM is 210% of SpellPower
            if(HasBuff(ResonantWords))
            {
                Estimate_FlashHeal = (Estimate_FlashHeal * 1.79);
            }
           return (int)Estimate_FlashHeal/1000 ;
        }
        public float Estimate_Heal()
        {
           float Estimate_Heal = (float)(SpellPower * 2.95); // LOTM is 210% of SpellPower
           if(HasFlashConcentration)
           {
               Estimate_Heal = (float)(Estimate_Heal * 1.15);
           }
           if(HasBuff(ResonantWords))
           {
                Estimate_Heal = (float)(Estimate_Heal * 1.79);
           }
           return  (Estimate_Heal/1000);
        }
        public float Estimate_CircleofHealing()
        {
           float Estimate_CircleofHealing = (float)(SpellPower * 1.05); // LOTM is 210% of SpellPower
           return Estimate_CircleofHealing/1000 ;
        }
        public float Estimate_Halo()
        {
           float Estimate_Halo = (float)(SpellPower * 1.15); 
           return Estimate_Halo/1000 ;
        }
        public float Estimate_HolyWordSerentiy()
        {
           float Estimate_HolyWordSerentiy = (float)(SpellPower * 7.00); // LOTM is 210% of SpellPower
           return Estimate_HolyWordSerentiy/1000 ;
        }
        public float Estimate_HolyWordSanctify()
        {
           float Estimate_HolyWordSanctify = (float)(SpellPower * 2.45); // LOTM is 210% of SpellPower
           return Estimate_HolyWordSanctify/1000 ;
        }
        public float Estimate_PrayerofHealing()
        {
           float Estimate_PrayerofHealing = (float)(SpellPower * .875); // LOTM is 210% of SpellPower
           return Estimate_PrayerofHealing/1000 ;
        }
        #endregion
        
        #region Cast Hanndlers
        // Holy Preist 
        public bool CastFlashHeal(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("FH_FOC");
                        DebugLog.Log(FlashHeal, "Casting " + FlashHeal + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(FlashHeal) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(FlashHeal) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(FlashHeal, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastHeal(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("H_FOC");
                        DebugLog.Log(Heal, "Casting " + Heal + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(Heal) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(Heal) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(Heal, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastHolyWordSerenity(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("HWS_FOC");
                        DebugLog.Log(HolyWordSerenity, "Casting " + HolyWordSerenity + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(HolyWordSerenity) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(HolyWordSerenity) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(HolyWordSerenity, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastPrayerofHealing(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PH_FOC");
                        DebugLog.Log(PrayerofHealing, "Casting " + PrayerofHealing + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                        if ( Aimsharp.CanCast(PrayerofHealing) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(PrayerofHealing) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(PrayerofHealing, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastCircleofHealing(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("CH_FOC");
                        DebugLog.Log(CircleofHealing, "Casting " + CircleofHealing + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(CircleofHealing) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(CircleofHealing) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(CircleofHealing, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastPrayerofMending(String unit)
        {
            //.Log(PrayerofMending, "Unit is focus  " + Utility.UnitFocus(unit));
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PM_FOC");
                        DebugLog.Log(CircleofHealing, "Casting " + CircleofHealing + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                        if ( Aimsharp.CanCast(PrayerofMending) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(PrayerofMending) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(PrayerofMending, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastGuardianSpirit(String unit)
        {
            //DebugLog.Log(GuardianSpirit, "Unit is focus  " + Utility.UnitFocus(unit));
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("GS_FOC");
                        DebugLog.Log(GuardianSpirit, "Casting " + GuardianSpirit + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit));
                        if ( Aimsharp.CanCast(GuardianSpirit) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(GuardianSpirit) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(GuardianSpirit, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastRenew(String unit)
        {
            //DebugLog.Log(Renew, "Unit is focus  " + Utility.UnitFocus(unit));
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("R_FOC");
                        DebugLog.Log(Renew, "Casting " + Renew + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit));
                        if ( Aimsharp.CanCast(Renew) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(GuardianSpirit) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(Renew, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastPurify(String unit)
        {
            
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("DISPEL_FOC");
                        DebugLog.Log(Purify, "Casting " + Purify + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit));
                        if ( Aimsharp.CanCast(Purify) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(Purify) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(Purify, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastFaeGuardians(String unit)
        {
            
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("FAE_FOC");
                        DebugLog.Log(FaeGuardians, "Casting " + FaeGuardians + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit));
                        if ( Aimsharp.CanCast(FaeGuardians) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(FaeGuardians) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(FaeGuardians, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastHolyWordSanctify(String CastingLocation)
        {
            
            switch (CastingLocation)
            {
                case "Manual": return Cast(HolyWordSanctify); 
                case "Cursor": return Cast("HWSanc_cursor"); 
                case "Player": return Cast("HWSanc_player"); 
                default: return false;
        
            }
           
        }
        public bool CastPowerWordLife(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PWL_FOC");
                        DebugLog.Log(PowerWordLife, "Casting " + PowerWordLife + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(PowerWordLife) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(PowerWordLife) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(PowerWordLife, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        #region Disc
        public bool CastPowerWordShield(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PWS_FOC");
                        DebugLog.Log(PowerWordShield, "Casting " + PowerWordShield + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(PowerWordShield) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(PowerWordShield) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(PowerWordShield, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastPowerWordRadiance(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PWR_FOC");
                        DebugLog.Log(PowerWordShield, "Casting " + PowerWordShield + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(PowerWordShield) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(PowerWordShield) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(PowerWordShield, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastPainSupression(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PSUP_FOC");
                        DebugLog.Log(PainSuppression, "Casting " + PainSuppression + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(PainSuppression) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(PainSuppression) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(PainSuppression, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        public bool CastShadowCovenant(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("SCV_FOC");
                        DebugLog.Log(ShadowCovenant, "Casting " + ShadowCovenant + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(ShadowCovenant) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(ShadowCovenant) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(ShadowCovenant, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
         public bool CastPenance(String unit)
        {
             if (!Utility.UnitFocus(unit))
                {
                    Aimsharp.Cast("FOC_" + unit, true);
                    return true;
                }
                else
                {
                    if (Utility.UnitFocus(unit))
                    {
                        Aimsharp.Cast("PN_FOC");
                        DebugLog.Log(Penance, "Casting " + Penance + " on Unit: " + unit + " | HP: " + Aimsharp.Health(unit) + " | AvgPartyHP: " + AveragePartyHealth );
                         if ( Aimsharp.CanCast(Penance) && Aimsharp.LineOfSighted() && !Aimsharp.LastCast().Equals(Penance) && !BlacklistDict.ContainsKey(unit))
                            {
                                BlacklistDict.Add(unit, Aimsharp.CombatTime());
                                DebugLog.Log(Penance, "Added to Blacklist  " + unit);
                                return true;
                            }
                        return true;
                    }
                }
            return false;
        }
        #endregion
        #endregion
        
        #region Class Functions 
        public bool Renew_Tank_Manager()
        {
            var HealUnits = GetValidHealTargets();
            var Tanks = HealUnits.Where(x => x.Role == "TANK" && !HasBuff(Renew, x.TargetString)).OrderBy(x => x.Health);
            //Keep up renew Tanks all the time 
              if(Tanks.Count() > 0)
                {
                    if(!HasBuff(Renew, Tanks.First().TargetString))
                    {
                        if(CanCast(Renew, Tanks.First().TargetString))
                        {
                            DebugLog.Log("[Renew_Tank_Manager]  Using Renew on Tank does not have Renew: " + Tanks.First().Health , Tanks.First().TargetString);
                            return CastRenew(Tanks.First().TargetString);
                        }
                    }
                    
                }
            return false;
        }
        public bool Renew_Moving_Manager()
        {
            int renewtank = 0;
            int renewparty = 0;
            int renewooc = 0;
            SettingsDictionary.TryGetValue(Renew_Tank , out renewtank);
            SettingsDictionary.TryGetValue(Renew_Party, out renewparty);
            SettingsDictionary.TryGetValue(Renew_OOC, out renewooc);
            
            var HealUnits = GetValidHealTargets();
            var Tanks = HealUnits.Where(x => x.Role == "TANK" && x.Health <= renewtank && !HasBuff(Renew, x.TargetString)).OrderBy(x => x.Health);
            var Party = HealUnits.Where(x => x.Role != "TANK" && x.Health <= renewparty && !HasBuff(Renew, x.TargetString)).OrderBy(x => x.Health);
            var OOC = HealUnits.Where(x => x.Health <= renewooc && !HasBuff(Renew, x.TargetString)).OrderBy(x => x.Health);

            if(!PlayerInCombat)
            {
                //Keep up renew Tanks all the time 
                if(OOC.Count() > 0)
                {
                    if(!HasBuff(Renew, OOC.First().TargetString))
                    {
                        if(CanCast(Renew, OOC.First().TargetString))
                        {
                            DebugLog.Log("[Renew_Manager]  Using Renew on OOC Unit does not have Renew: " + OOC.First().Health , OOC.First().TargetString);
                            return CastRenew(OOC.First().TargetString);
                        }
                    }
                    
                }
            }
            //Keep up renew Tanks all the time 
            if(Tanks.Count() > 0)
            {
                if(!HasBuff(Renew, Tanks.First().TargetString))
                {
                    if(CanCast(Renew, Tanks.First().TargetString))
                    {
                        DebugLog.Log("[Renew_Manager]  Using Renew on Tank does not have Renew: " + Tanks.First().Health , Tanks.First().TargetString);
                        return CastRenew(Tanks.First().TargetString);
                    }
                }
                
            }
            if(Party.Count() > 0)
            {
                if(!HasBuff(Renew, Party.First().TargetString))
                {
                    if(CanCast(Renew, Party.First().TargetString))
                    {
                        DebugLog.Log("[Renew_Manager]  Using Renew on Party does not have Renew: " + Party.First().Health , Party.First().TargetString);
                        return CastRenew(Party.First().TargetString);
                    }
                }
                
            }
            return false;
        }
        
        public bool SymbolofHope_Manager()
        {
            int symbolofhope_pct = 0;
            SettingsDictionary.TryGetValue(SymbolofHopeP, out symbolofhope_pct);

            if(ManaPct <= symbolofhope_pct)
            {
                // only use symbol if party is healthy 
                if( AveragePartyHealth >= 90)
                {
                    if(CanCast(SymbolofHope, "player"))
                    {
                        //DebugLog.Log("[SymbolofHope_Manager]  Using Symbol of Hope Mana / AvgHp: " + symbolofhope_pct, AveragePartyHealth);
                        Cast(SymbolofHope);
                        return true;
                    }
                }
                
            }

            return false;
        }
        /*
        public bool  Halo_Manager()
        {
            // Heal anyone on the bring of death 
            //DebugLog.Log("[PrayerofMending_Manager] Status:  Stacks: " + FlashConcentrationStacks," Seconds Remaining: " + FlashConcentrationRemaining);
            // Get Base Heal Units w/ Anduin Logic 
            int halo_hp = 0;
            SettingsDictionary.TryGetValue(Halo_HP, out halo_hp);
            
            var HealUnits = GetValidHealTargets();
            var LowestClosest = HealUnits.Where(x => x.Health <= halo_hp).OrderBy(x => x.Health);
            var LowestClosest_Smart = HealUnits.Where(x =>  x.HPDefeceit >= (int) Estimate_Halo()).OrderByDescending(x => x.HPDefeceit);


            if(LowestClosest_Smart.Count() >= 6)
            {
                if(CanCast(Halo))
                {
                    DebugLog.Log("[Halo_Manager]  Uasing Halo Smart Count:  " + LowestClosest_Smart.Count()  , "  Estimate:  " + (int)Estimate_Halo());
                    return Cast(Halo);
                }
            }

            if(LowestClosest.Count() >= 6)
            {
                if(CanCast(Halo))
                {
                    DebugLog.Log("[Halo_Manager]  Uasing Halo Smart Settings:  " + LowestClosest.Count()  , "  Setting:  " + halo_hp);
                    return Cast(Halo);
                }
            }
            
            return false; 
        }
        */
        public bool  Fortitude_Manager()
        {
            // Heal anyone on the bring of death 
            //DebugLog.Log("[PrayerofMending_Manager] Status:  Stacks: " + FlashConcentrationStacks," Seconds Remaining: " + FlashConcentrationRemaining);
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var Fort_Units = HealUnits.Where(x => !HasBuff(PowerWordFortitude, x.TargetString)).OrderBy(x => x.Health);

           if(LastCast == PowerWordFortitude)
           {
                DebugLog.Log("[Fortitude_Manager]  Spam Protection"  , " ");
                return false;
           }
           if(Fort_Units.Count() == 0)
           {
                DebugLog.Log("[Fortitude_Manager]  Everyone has Fortitude"  , " ");
                return false;
           }


            if(Fort_Units.Count() >= 0)
            {
                if(CanCast(PowerWordFortitude, "player"))
                {
                    DebugLog.Log("[Fortitude_Manager] Using Forton the group:  " + Fort_Units.Count()  , " ");
                    DebugLog.Log("[Fortitude_Manager] Waiting..........  "  , " ");
                    System.Threading.Thread.Sleep(GCD+300);
                    return Cast(PowerWordFortitude);
                }
            }

            return false; 
        }
        
        public bool SurgeofLight_Manager(bool UseNow = false)
        {
            var HealUnits = GetValidHealTargets();
            var Lowest = HealUnits.Where(x => x.Health < 100 && !HasBuff(Atonement, x.TargetString) ).OrderBy(x => x.Health);
            var CriticalUnits = HealUnits.Where(x => x.Health < 80 && x.Role != "TANK" ).OrderBy(x => x.Health);

             // On Demands Use from other functions 
            if(UseNow)
            {
                var UseNow_Units = HealUnits.OrderBy(x => x.Health);
                if(UseNow_Units.Count() > 0)
                {
                    if(CanCast(FlashHeal, UseNow_Units.First().TargetString))
                    {
                        DebugLog.Log( "[SurgeofLight_Manager] UseNow_Units Casting on:  ", UseNow_Units.ToList().First().DebugString);
                        return CastFlashHeal(UseNow_Units.First().TargetString);
                    }
                }

            }

            // Use at 2 Charges 
            if(HasBuffStacks(SurgeofLight) == 2)
            {
                // Flash Heal Lowest HP %
                if(Lowest.Count() > 0)
                {
                    DebugLog.Log("[SurgeofLight_Manager]  Using a Surge of Light Proc at 2 Stacks : " + HasBuffStacks(SurgeofLight) , Lowest.First().TargetString);
                    if(CanCast(FlashHeal, Lowest.First().TargetString))
                    {
                        return CastFlashHeal(Lowest.First().TargetString);
                    }
                }
            }

            // Heal Ciritcal Units wit h SOL 
            if(CriticalUnits.Count() > 0)
            {
                DebugLog.Log("[SurgeofLight_Manager]  Using a Surge of Light Proc On critical Units  : " + CriticalUnits.First().Health , CriticalUnits.First().TargetString);
                if(CanCast(FlashHeal, CriticalUnits.First().TargetString))
                {
                    return CastFlashHeal(CriticalUnits.First().TargetString);
                }
            }

            if(Moving)
            {
               
            }

            return false;
        }
        
        public bool  PowerWordLife_Manager(bool UseNow = false)
        {
            // Heal anyone on the bring of death 
           
            // Grab UI Setting for Heal
            int pwltankhp = 0;
            int pwldktankhp = 0;
            int pwlpartyhp = 0;
            SettingsDictionary.TryGetValue(PowerWordLife_Tank_HP, out pwltankhp);
            SettingsDictionary.TryGetValue(PowerWordLife_DKTank_HP, out pwldktankhp);
            SettingsDictionary.TryGetValue(PowerWordLife_Party_HP, out pwlpartyhp);
            
            // Dk Tank logic 
            if(dktank)
            {
                pwltankhp = pwldktankhp;
            }
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var LowestTank = HealUnits.Where(x => x.Role == "TANK" && x.Health <= pwltankhp ).OrderBy(x => x.Health);
            var Lowest = HealUnits.Where(x =>  x.Role != "TANK" && x.Health <= pwlpartyhp ).OrderBy(x => x.Health);
            

            // Use DP instead of Srenity if up if we want to heal self 
            if(Lowest.Count() > 0)
            {
                if(Lowest.First().TargetString == "player")
                {
                    if(CanCast(DesperatePrayer, "player"))
                    {
                        Cast(DesperatePrayer);
                        return true;
                    }
                }
            }
            // On Demands Use from other functions 
            if(UseNow)
            {
                var UseNow_Units = HealUnits.OrderBy(x => x.Health);
                if(UseNow_Units.Count() > 0)
                {
                    if(CanCast(HolyWordSerenity, UseNow_Units.First().TargetString))
                    {
                        DebugLog.Log( "[PowerWordLife_Manager] UseNow_Units Casting on:  ", UseNow_Units.ToList().First().DebugString);
                        return CastPowerWordLife(UseNow_Units.First().TargetString);
                    }
                }

            }

            
            // Flash heal Near Death Tanks 
            if(UrgentTank_Healing(PowerWordLife))
            {
                DebugLog.Log("[PowerWordLife_Manager]", "UrgentTank_Healing Complete:  " + HolyWordSerenity);
                return true;
            }


            if(LowestTank.Count() > 0)
            {
                if(CanCast(PowerWordLife, LowestTank.First().TargetString))
                {
                     DebugLog.Log( "[PowerWordLife_Manager] LowestTank Casting on:  ", LowestTank.ToList().First().DebugString);
                    return CastPowerWordLife(LowestTank.First().TargetString);
                }
            }
          
            if(Lowest.Count() > 0)
            {
                if(CanCast(PowerWordLife, Lowest.First().TargetString))
                {
                     DebugLog.Log( "[PowerWordLife_Manager] Lowest Casting on:  ", Lowest.ToList().First().DebugString);
                    return CastPowerWordLife(Lowest.First().TargetString);
                }
            }
            
            return false; 
        }
       
       
        public bool Mindgames_Disc_Manager()
        {
            if(CanCast(Mindgames) && AveragePartyHealth > 75 && AtonementCount > 3)
            {
                DebugLog.Log( "[Mindgames_Disc_Manager] Casting on Target:  ", "cancast:" + CanCast(Mindgames));
                return Cast(Mindgames);

            }
            return false; 
        }
       
        #region Disc Priest Functions
        public bool PowerWordShield_Manager_Disc()
        {
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var LowestTank = HealUnits.Where(x => x.Role == "TANK" && !HasBuff(PowerWordShield, x.TargetString)).OrderBy(x => x.Health);
            var LowestParty = HealUnits.Where(x => x.Role != "TANK" && !HasBuff(PowerWordShield, x.TargetString)).OrderBy(x => x.Health);

            // Heal Party First if Tank is more healthy 
            if(LowestTank.Count() > 0 && LowestParty.Count() > 0)
            {
                if( LowestTank.First().Health > LowestParty.First().Health)
                {
                    if(CanCast(PowerWordShield, LowestParty.First().TargetString))
                    {
                        DebugLog.Log( "[PowerWordShield_Manager_Disc] LowestParty Casting on:  ", LowestParty.ToList().First().DebugString);
                        return CastPowerWordShield(LowestParty.First().TargetString);
                    }
                }
            }
            // Heal Tank With PWS Preffered 
            if(LowestTank.Count() > 0)
            {
                if(CanCast(PowerWordShield, LowestTank.First().TargetString))
                {
                     DebugLog.Log( "[PowerWordShield_Manager_Disc] LowestTank Casting on:  ", LowestTank.ToList().First().DebugString);
                    return CastPowerWordShield(LowestTank.First().TargetString);
                }
            }
            // Heal Party If tank has PWS 
            if(LowestParty.Count() > 0)
            {
                if(CanCast(PowerWordShield, LowestParty.First().TargetString))
                {
                     DebugLog.Log( "[PowerWordShield_Manager_Disc] LowestParty Casting on:  ", LowestParty.ToList().First().DebugString);
                    return CastPowerWordShield(LowestParty.First().TargetString);
                }
            }
            return false; 
        }
        public bool PowerWordRadiance_Manager_Disc_Mplus()
        {
            int pwrcount = 0;
            int pwrhp = 0;
            SettingsDictionary.TryGetValue(PWR_Count, out pwrcount);
            SettingsDictionary.TryGetValue(PWR_HP, out pwrhp);
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var PWRUnits = HealUnits.Where(x =>  x.Health <= pwrhp).OrderBy(x => x.Health);
            var PWRBlanketUnits = HealUnits.Where(x =>  x.Health < 100).OrderBy(x => x.Health);

            if(PWRUnits.Count() >= pwrcount)
            {
                if(CanCast(PowerWordRadiance, PWRUnits.First().TargetString))
                {
                    DebugLog.Log( "[PowerWordRadiance_Manager_Disc_Mplus] PWRUnits Casting on:  ", PWRUnits.ToList().First().DebugString);
                    return CastPowerWordRadiance(PWRUnits.First().TargetString);
                }
            }

            if(SpellCharges(PowerWordRadiance) == 2 && ManaPct >= 75 && AveragePartyHealth < 99)
            {
                if(PWRBlanketUnits.Count() > 0)
                {
                    if(CanCast(PowerWordRadiance, PWRBlanketUnits.First().TargetString))
                    {
                        DebugLog.Log( "[PowerWordRadiance_Manager_Disc_Mplus] Capped PWR and Mana healthy Casting on:  ", PWRBlanketUnits.ToList().First().DebugString);
                        return CastPowerWordRadiance(PWRBlanketUnits.First().TargetString);
                    }
                }
            }

            // If Target is Boss we should keep Atonements up all the time 
            // Try to just Blanket Atonement with PWR
            if(PWRBlanketUnits.Count() > AtonementCount )
            {
                if(CanCast(PowerWordRadiance, PWRBlanketUnits.First().TargetString))
                {
                    DebugLog.Log( "[PowerWordRadiance_Manager_Disc_Mplus] PWRBlanketUnits Casting on:  ", PWRBlanketUnits.ToList().First().DebugString);
                    return CastPowerWordRadiance(PWRBlanketUnits.First().TargetString);
                }
            }


            return false; 
        }
        public bool FlashHeal_Manager()
        {
            int fhhp = 0;
            SettingsDictionary.TryGetValue(FH_HP, out fhhp);
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var FlashHealUnits = HealUnits.Where(x => x.Health <= fhhp).OrderBy(x => x.Health);

            if(FlashHealUnits.Count() > 0)
            {
                if(CanCast(FlashHeal, FlashHealUnits.First().TargetString))
                {
                    DebugLog.Log( "[FlashHeal_Manager] FlashHealUnits Casting on:  ", FlashHealUnits.ToList().First().DebugString);
                    return CastFlashHeal(FlashHealUnits.First().TargetString);
                }
            }
            

            return false;
        }
        public bool OOCFlashHeal_Manager()
        {
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var FlashHealUnits = HealUnits.Where(x => x.Health <= 95).OrderBy(x => x.Health);

            if(FlashHealUnits.Count() > 0)
            {
                if(CanCast(FlashHeal, FlashHealUnits.First().TargetString))
                {
                    DebugLog.Log( "[FlashHeal_Manager] FlashHealUnits Casting on:  ", FlashHealUnits.ToList().First().DebugString);
                    return CastFlashHeal(FlashHealUnits.First().TargetString);
                }
            }
            

            return false;
        }
        public bool Renew_Atonement_Manager()
        {
            int rnhp = 0;
            SettingsDictionary.TryGetValue(Renew_HP, out rnhp);
            var HealUnits = GetValidHealTargets();
            var Tanks = HealUnits.Where(x => x.Role == "TANK" && !HasBuff(Atonement, x.TargetString) && !HasBuff(Renew, x.TargetString) && x.Health <= rnhp).OrderBy(x => x.Health);
            var Party = HealUnits.Where(x => x.Role != "TANK" && !HasBuff(Atonement, x.TargetString) && !HasBuff(Renew, x.TargetString) && x.Health <= rnhp).OrderBy(x => x.Health);
            var PWRBlanketUnits = HealUnits.Where(x =>  x.Health < 88).OrderBy(x => x.Health);
            //Keep up renew Tanks all the time 
            if(Tanks.Count() > 0)
            {
                if(!HasBuff(Renew, Party.First().TargetString) && !HasBuff(Atonement, Party.First().TargetString))
                {
                    if(CanCast(Renew, Tanks.First().TargetString))
                    {
                        DebugLog.Log("[Renew_Atonement_Manager]  Using Renew on Tank does not have Renew: " + Tanks.First().Health , Tanks.First().TargetString);
                        return CastRenew(Tanks.First().TargetString);
                    }
                }
                
            }

            // Check to see if we should PWR instead 
            if(PWRBlanketUnits.Count() >= 3 && CanCast(PowerWordRadiance))
            {
                if(CanCast(PowerWordRadiance, PWRBlanketUnits.First().TargetString))
                {
                    DebugLog.Log( "[Renew_Atonement_Manager] [PowerWordRadiance_Manager_Disc_Mplus] PWRBlanketUnits Casting on:  ", PWRBlanketUnits.ToList().First().DebugString);
                    return CastPowerWordRadiance(PWRBlanketUnits.First().TargetString);
                }
            }

            if(Party.Count() > 0)
            {
                if(!HasBuff(Renew, Party.First().TargetString) && !HasBuff(Atonement, Party.First().TargetString))
                {
                    if(CanCast(Renew, Party.First().TargetString))
                    {
                        DebugLog.Log("[Renew_Atonement_Manager]  Using Renew on Party does not have Renew: " + Party.First().Health , Party.First().TargetString);
                        return CastRenew(Party.First().TargetString);
                    }
                }
                
            }
            return false;
        }
        public void Atonement_Status( ref int atonementcount )
        {
            var HealUnits = GetValidHealTargets();

            var Party = HealUnits.Where(x => HasBuff(Atonement, x.TargetString) && BuffRemaining(Atonement,x.TargetString) > 3000).OrderBy(x => x.Health);
            atonementcount = 0;
            if(Party.Count() > 0)
            {
                atonementcount = Party.Count();
            }
        }
        public bool Rapture_Manager_Mplus()
        {
            // Get Base Heal Units w/ Anduin Logic 
            var HealUnits = GetValidHealTargets();
            var LowestTank = HealUnits.Where(x => x.Role == "TANK" && !HasBuff(PowerWordShield, x.TargetString)).OrderBy(x => x.Health);
            var LowestParty = HealUnits.Where(x => x.Role != "TANK" && !HasBuff(PowerWordShield, x.TargetString)).OrderBy(x => x.Health);

            if(HasBuff(Rapture))
            {
                if(LowestTank.Count() > 0)
                {
                    if(CanCast(PowerWordShield, LowestTank.First().TargetString))
                    {
                        DebugLog.Log( "[PowerWordShield_Manager_Disc] LowestTank Casting on:  ", LowestTank.ToList().First().DebugString);
                        return CastPowerWordShield(LowestTank.First().TargetString);
                    }
                }
                
                if(LowestParty.Count() > 0)
                {
                    if(CanCast(PowerWordShield, LowestParty.First().TargetString))
                    {
                        DebugLog.Log( "[PowerWordShield_Manager_Disc] LowestParty Casting on:  ", LowestParty.ToList().First().DebugString);
                        return CastPowerWordShield(LowestParty.First().TargetString);
                    }
                }
            }
            
            return false; 
        }
        public bool PainSupression_Manager()
        {

           int psuptankhp = 0;
           int psupdkhp = 0;
           int psuppartyhp = 0;
            SettingsDictionary.TryGetValue(PainSuppression_Party, out psuppartyhp);
            SettingsDictionary.TryGetValue(PainSuppression_DK, out psupdkhp);
            SettingsDictionary.TryGetValue(PainSuppression_Tank, out psuptankhp);

            if(dktank)
            {
                    psuptankhp = psupdkhp;
                
            }

            var HealUnits = GetValidHealTargets();
            
            var PainSupTank = HealUnits.Where(x => x.Health <= psuptankhp && x.Role == "TANK").OrderBy(x => x.Health);
            var PainSupParty = HealUnits.Where(x => x.Health <= psuppartyhp && x.Role != "TANK").OrderBy(x => x.Health);
           
         
           
          
            // PS self if below Value
            if(PlayerCurrentHP_P <= psuppartyhp)
            {
                // use PowerWordLife instead of PS if up
                if(CanCast(PowerWordLife, "player"))
                {
                    if(PowerWordLife_Manager(true))
                    {
                        DebugLog.Log("[PainSupression_Manager]  Using PowerWordLife_Manager to optmize PainSupression_Manager: " ,"== COMPLETE ==");
                        return true;
                    }
                }

                if(CanCast(PainSuppression, "player"))
                {
                    DebugLog.Log( "[PainSupression_Manager] PainSuppression -- Self", " ------------- ");
                    return CastPainSupression("player");
                }   
            }


            if(PainSupTank.Count() > 0)
            {

                if(CanCast(PainSuppression, PainSupTank.First().TargetString))
                {
                    DebugLog.Log( "[PainSupression_Manager] Casting on PainSupTank:  ", PainSupTank.ToList().First().DebugString);
                    return CastPainSupression(PainSupTank.First().TargetString);
                }   
            }

            if(PainSupParty.Count() > 0)
            {
                // use PowerWordLife instead of PS if up
                if(CanCast(PowerWordLife, PainSupParty.First().TargetString))
                {
                    if(PowerWordLife_Manager(true))
                    {
                        DebugLog.Log("[PainSupression_Manager]  Using PowerWordLife_Manager to optmize PainSupression_Manager: " ,"== COMPLETE ==");
                        return true;
                    }
                }

                if(CanCast(PainSuppression, PainSupParty.First().TargetString))
                {
                    DebugLog.Log( "[PainSupression_Manager] Casting on PainSupParty:  ", PainSupParty.ToList().First().DebugString);
                    return CastPainSupression(PainSupParty.First().TargetString);
                }   
            }
        
        
        

            return false;

        }
        public bool ShadowCovenant_Manager(bool usenow = false)
        {

            var HealUnits = GetValidHealTargets();
        
            if(usenow)
            {
                var SCUnitis = HealUnits.OrderBy(x => x.Health);
                if(SCUnitis.Count() > 0)
                {
                    // use ShadowCovenant if its up 
                    if(CanCast(ShadowCovenant, "player"))
                    {
                        DebugLog.Log( "[ShadowCovenant_Manager] Use Now Casting on ShadowCovenant:  ", SCUnitis.ToList().First().DebugString);
                        return CastShadowCovenant(SCUnitis.First().TargetString);
                    }
                }
            }
            
            return false;

        }

        public bool AtonementRotation(int AtonementCount)
        {
            if(AtonementCount >= 4)
            {
                // Shadow Convenant 
                if(CanCast(ShadowCovenant) && ShadowCovenant_Manager(true) && AveragePartyHealth < 80)
                {
                    DebugLog.Log( "[AtonementRotation] ShadowCovenant_Manager", "== Complete ==");
                    return true;
                }
                // Schism 
                if(CanCast(Schism) &&  TimeToDeath >= 5)
                {
                        DebugLog.Log( "[AtonementRotation]", " == Schism == ");
                        return Cast(Schism);
                }
                // Shadow Word Death 
                // MindGames 
                // Divine Star
                // Penance 
                // Smite 

            }

            return false;
        }
        #endregion

        #endregion
        
        #endregion
        //// Rotations ////
        #region Rotations
        #region Core Rotations
        // Utility Rotations 
        public bool UseConsumables()
        {
            if (CanUseItem("Healthstone", false))
            {
                int healthThreshold = 0;
                SettingsDictionary.TryGetValue(HealthStoneThreshold, out healthThreshold);
                if (PlayerCurrentHP_P <= healthThreshold)
                {
                    DebugLog.Log( "Health Stone", "===== Using Defensive Health Stone Under Setting ======="); 
                    Cast("Healthstone");
                }
            }
            
            if (Aimsharp.CanUseItem("Cosmic Healing Potion", false))
            {
                int healthThreshold = 0;
                SettingsDictionary.TryGetValue(HealthPot, out healthThreshold);
                if (PlayerCurrentHP_P <= healthThreshold)
                {
                    DebugLog.Log( "Health Pot", "===== Using Defensive Cosmic Healing Potion Under Setting =======");  
                    Cast("CosmicHPPotion");
                }
            }

            // Instant Mana Pots according to Setting recommended 85%
            if (Aimsharp.CanUseItem("Spiritual Mana Potion", false) && Aimsharp.GetMapID() != 0)
            {
                int manaThreshold = 0;
                SettingsDictionary.TryGetValue(ManaPot, out manaThreshold);
                if (ManaPct <= manaThreshold)
                {
                    DebugLog.Log( "Mana Pot", "===== Using Mana Potion Under Setting ======="); 
                    Cast("ManaPotion");
                }
            }

            return false;
        }
        public bool TrinketManager()
        {
            if (UseTopTrinket && Aimsharp.CanUseTrinket(0))
            {
                if (!SaveCooldowns)
                {
                    Cast("TopTrinket");
                }
            }

            if (UseBottomTrinket && Aimsharp.CanUseTrinket(1))
            {
                if (!SaveCooldowns)
                {
                    Cast("BottomTrinket");
                }   
            }
            

            if (UsePotion && CanUseItem(PotionName, false))
            {
                Cast("DPS Pot");
            }
           
    
            return true;
        }
        public bool Solo()
        {
            

            

           // Aimsharp.PrintMessage(Aimsharp.LastCast(), Color.Blue);
            return true;
        }
        public bool OOC()
        {
           
            // Out of Combat Healing 
            // Fortitude Manager
            if(CanCast(PowerWordFortitude))
            {
                if(Fortitude_Manager())
                {
                    return true;
                }
            }
            
            if(HasBuff(SurgeofLight) && CanCast(FlashHeal))
            {
                if(SurgeofLight_Manager())
                {
                    DebugLog.Log( "[OOC] SurgeofLight_Manager", "== Complete ==");
                    return true;
                
                }
            }

            //Heal Bursting OOC Logic
            if(BurstingActive)
            {
                DebugLog.Log( "[OOC] BurstingActive", "== Complete == BurstingActive : " + BurstingActive );
                if(CanCast(Renew, "player") && Renew_Tank_Manager())
                {
                    DebugLog.Log( "Renew_Tank_Manager", "== Complete ==");
                }
                // Run OOC Healing Rotation 
                
            }

            // Out of Combat Healing 
            if(oocheal)
            {
                if(OOCHealing())
                {
                    DebugLog.Log( "[oocheal] OOCHealing", "== Complete == oocheal : " + oocheal );
                    return true;
                }
            }


            return false;
        }
        public bool OnlyDPS()
        {
            // Aimsharp.PrintMessage("DPS Only  ==> OnlyDPS() ==>  " + dpsonly, Color.Red);
            DebugLog.Log( "DPS Only", "Running DPSOnly Rotation");
            var HealUnits = GetValidHealTargets();
            int criticalhealing_pct = 0;
            int criticalhealing_pct_tank = 0;
            SettingsDictionary.TryGetValue(CriticalHealingP, out criticalhealing_pct);
            SettingsDictionary.TryGetValue(CriticalHealingPT, out criticalhealing_pct_tank);
            var NearDeathTanks = HealUnits.Where(x => x.Health <= criticalhealing_pct_tank && x.Role == "TANK").OrderBy(x => x.Health);

            var LowestHP = HealUnits.Where(x=> x.Health <= criticalhealing_pct).OrderBy(x => x.Health);
            if(LowestHP.Count() > 0)
            {
                //Aimsharp.PrintMessage("DPS Only  ==> OnlyDPS() ==>  TOO LOW" + dpsonly, Color.Red);
                DebugLog.Log( "DPS Only", "ABORT not Safe To DPS only Low UNIT!!");
                DebugLog.Log( "DPS Only", "Low Unit:  " + LowestHP.First().TargetString);
                if(CriticalHealing(LowestHP.First().TargetString)) return true;
    
                return false;
            }

          
            //Aimsharp.PrintMessage("DPS Only  ==> OnlyDPS() ==> Entering DPS_Rotation" + dpsonly, Color.Red);
            if(DPS_Rotation_Disc()) return true;
            // Aimsharp.PrintMessage("DPS Only  ==> OnlyDPS() ==>  DPS_Rotation ==> FALSE" + dpsonly, Color.Red);
            return false;
        }
        public bool AutoTarget()
        {
            // Need to know if our target is a Party Frame for some reason 
            bool TargetIsParty = Aimsharp.CustomFunction("TargetIsParty") > 0;
            bool TargetIsEmpty = Aimsharp.CustomFunction("TargetIsEmpty") == 0;

            if(TargetIsEnemy)
            {
                // If target is Valid we are chilling 
                return false;
            }

            // Keep Enemy target
            if( (!TargetIsEnemy || TargetIsEmpty) && PlayerInCombat)
            {
                Aimsharp.Cast("TargetFocusTarget");
                return true;
            }

           
           
            return false;
        }
        public bool Eating()
        {
            //Do nothing if eating
            if (Aimsharp.HasBuff("Refreshment", "player") || Aimsharp.HasBuff("Drink", "player") || Aimsharp.HasBuff("Eat", "player") || Aimsharp.HasBuff("Food & Drink", "player"))
            {
                return true;
            }
            return false;
        }
        public bool TargetHealing()
        {
            int HP_target = Aimsharp.Health("target");
            int HP_player = Aimsharp.Health("player");

            return false;
        }
        public bool TargetIsSpecial()
        {
            // Withering Seed == NPCID = 182822
            TargetIsWitheringSeed = false;
            // Anduins Hope == NPCID = 184494
            TargetIsAnduinsHope = false;
            int CurrentTargetID = Aimsharp.UnitID("target");

            if(CurrentTargetID == 182822) // Withering Seed == NPCID = 182822
            {
                TargetIsWitheringSeed = true;
                DebugLog.Log( "TargetIsSpecial", "== Special Target Detected == " + CurrentTargetID + "  TargetIsWitheringSeed: " + TargetIsWitheringSeed);
                return true;
            }
            else if(CurrentTargetID == 184494)// Anduins Hope == NPCID = 184494
            {
                TargetIsAnduinsHope = true;
                DebugLog.Log( "TargetIsSpecial", "== Special Target Detected == " + CurrentTargetID + "  TargetIsAnduinsHope: " + TargetIsAnduinsHope);
                return true;
            }

            return false;
        }
        #endregion 
        #region Raid Boss Rotations
        public bool WitherSeed_Manager()
        {
            // If Boss is casting Kingsmourne Hungers 
            // Hold Procs of Divine Purpose 
            // Anduin's Hope ** npc=184494
            // Put with No Miracles toggle 
            // need to ebacon targets ASAP 
          
            DebugLog.Log( "[****************   Withering Seed  ******************", " Entering -------------------------------------------------");
            int HP_target = Aimsharp.Health("target");
            int HP_player = Aimsharp.Health("player");
            // If Boss is casting Kingsmourne Hungers 
            // Hold Procs of Divine Purpose 012345

            // Beacon of Virtue on ads 

            if(Enemy)
            {   
                DebugLog.Log( "WitherSeed_Manager", "== Targetting Enemy == Running Core Healing");
                return false;
            }

         
           
            return false;
        }
        public bool AnduinsHope()
        {
            // If Boss is casting Kingsmourne Hungers 
            // Hold Procs of Divine Purpose 
            // Anduin's Hope ** npc=184494
            // Put with No Miracles toggle 
            // need to ebacon targets ASAP 
          
             DebugLog.Log( "[****************   ANDUINS HOPE ******************", " Entering -------------------------------------------------");
            int HP_target = Aimsharp.Health("target");
            int HP_player = Aimsharp.Health("player");
            // If Boss is casting Kingsmourne Hungers 
            // Hold Procs of Divine Purpose 012345

            // Beacon of Virtue on ads 

            if(Enemy)
            {   
                DebugLog.Log( "AnduinsHope", "== Targetting Enemy == Running Core Healing");
                return false;
            }

          
            return false;
        }
        public bool BefouledBarrier()
        {
            
            
            return false;
        }
        
        #endregion
        #region Class Rotations 
        public bool DebugRotation()
        {
            //Aimsharp.PrintMessage("CanCast handler ----> ");
            //if(CanCast(BeaconofVirtue)) return Cast(BeaconofVirtue);
            
            //Aimsharp.PrintMessage("Cast handler ----> ");
            // CastBeaconofVirtue("player");

       

             return false;
        }
        public bool cooldowns()
        {
            if(nocds)
            {
                DebugLog.Log( "[cooldowns]", " nocds is ON Skipping CDs -- " + nocds);
                return false;
            }
            // Run Defensives

            // Pain Supression
            if(CanCast(PainSuppression) && PainSupression_Manager())
            {
                 DebugLog.Log( "[cooldowns PainSupression_Manager]", " == Complete == ");
                    return true;
            }

            // Rapture Handler
            if(HasBuff(Rapture))
            {
                return Rapture_Manager_Mplus();
            }
            // Auto Rapture 
            int rapturehp = 0;
            SettingsDictionary.TryGetValue(Rapture_HP , out rapturehp);
            if(AveragePartyHealth <= rapturehp)
            {
                return Cast(Rapture);
            }




            bool Disable_ArcaneTorrent = Settings.GetCheckBox(TurnOFF_AT);
            if( CanCast(ArcaneTorrent,"player") && Disable_ArcaneTorrent && ArcaneTorrent_Manager())
            {
                    DebugLog.Log( "[cooldowns ArcaneTorrent_Manager]", " == Complete == ");
                    return true;
            }
            
            if(CanCast(Shadowfiend) && AtonementCount > 1)
            {
                    DebugLog.Log( "[cooldowns]", " == Shadowfiend == ");
                    return Cast(Shadowfiend);
            }
            
            // Schism 
            if(CanCast(Schism) &&  TimeToDeath >= 10 && AtonementCount >= 4)
            {
                    DebugLog.Log( "[cooldowns]", " == Schism == ");
                    return Cast(Schism);
            }

            //Mindgames
            if(CanCast(Mindgames) && TimeToDeath >= 10 && Mindgames_Disc_Manager() && AtonementCount >= 4 )
            {
                DebugLog.Log( "[Mindgames_Holy_Manager]", " == Complete == ");
                return true;
            }
            // Lights Wrath
            if(ValidateSpell(LightsWrath) && TimeToDeath >= 10 && AtonementCount >= 4 && CanCast(LightsWrath) )
            {
                  DebugLog.Log( "[cooldowns]", " == LightsWrath == ");
                    return Cast(LightsWrath);
            }

           
            
            return false;
        }      
       
        public bool DPS_Rotation_Disc()
        {
            int focusdps = 0;
            SettingsDictionary.TryGetValue(DPS_Focus_HP, out focusdps);
            if(healonly)
            {
                DebugLog.Log( "DPS_Rotation_Disc", "healonly ON Skipping Rotation");
                return false;
            }
            if(CriticalHealingNeeded())
            {
                DebugLog.Log( "DPS_Rotation_Disc", "CriticalHealingNeeded Detected == Bailing ");
                return false;
            }
            
            if((!HasDebuff(PurgetheWicked) || DebuffRemaining(PurgetheWicked) < 5000) && CanCast(PurgetheWicked))
            {
                Cast(PurgetheWicked);
                return true;
            }
            if(CanCast(Penance))
            {
                Cast(Penance);
                return true;
            }
             // Divine Star
            if(CanCast(DivineStar) && TargetIsEnemy )
            {
                
                if(UsePrimaryHealInstead())
                {
                    DebugLog.Log("[DivineStar]", "UsePrimaryHealInstead == Complete ==" );
                    return true;
                }
                Cast(DivineStar);
                DebugLog.Log( "DivineStar", "== Complete ==");
                return true;
            }

            if(CanCast(HolyNova) && (EnemyList.GetValue(Modules.Enemy.Range.RANGE15) > 4))
            {
                Cast(HolyNova);
            }
            if(CanCast(ShadowWordDeath) && TargetIsEnemy )
            {
                DebugLog.Log( "DPS_Rotation_Holy", "== ShadowWordDeath ==");
                return Cast(ShadowWordDeath);
            }
            
            if(!HasBuff(HarshDiscipline) && ValidateSpell(HarshDiscipline) && CanCast(Smite))
            {
                return Cast(Smite);
            }

            // MindBlast
            if(CanCast(MindBlast) && TargetIsEnemy )
            {
                
               
                Cast(MindBlast);
                DebugLog.Log( "MindBlast", "== Complete ==");
                return true;
            }
            if(CanCast(Smite))
            {
                Cast(Smite);
                return true;
            }

            return false;
        }
    
        public DateTime dispellTimer = DateTime.Now;
        public bool Dispell_Manager()
        {
            // Dispell Handler 
            //Auto Purify
            var HealUnits = GetValidHealTargets();
            var LowestHP = HealUnits.Where( x => x.Health <= 100 && x.Dispellable == true).OrderBy(x => x.Health); // TRY THIS

            int Player = Aimsharp.CustomFunction("CheckPlayerPurify");
            int Party = Aimsharp.CustomFunction("CheckPartyPurifyGrouped");
            int Raid = Aimsharp.CustomFunction("CheckRaidPurifyGrouped");

            //DebugLog.Log("[Dispell_Manager]  Spell Debugging :   Player: " + Player  , "Party: " + Party +" Raid: "+Raid );

            if (LowestHP.Count() > 0 && LastCast != Purify && !Aimsharp.LineOfSighted())
            {
                //DebugLog.Log("[Dispell_Manager]  Units to Dispell LowestHP.Count() : " + LowestHP.Count() , " Lowest Unit:  " + LowestHP.First().TargetString + " HP: " + LowestHP.First().Health);
                TimeSpan ts = DateTime.Now.Subtract(dispellTimer);
                //DebugLog.Log("[Dispell_Manager]  Timer Debugging: " + ts.Seconds  , "  ");
                if (ts.Seconds >= 1)
                {
                    //DebugLog.Log("[Dispell_Manager]  Checkpoint: > 1500 Timer Debugging: " + ts.Seconds  , "  ");
                    if (ts.Seconds >= 2)
                    {
                        //DebugLog.Log("[Dispell_Manager]  Checkpoint: > 2500 Timer Debugging: " + ts.Seconds  , "  ");
                        dispellTimer = DateTime.Now;
                    }
                    
                    // Try and Cast the Dispell
                    if(CanCast(Purify, LowestHP.First().TargetString))
                    {
                        //DebugLog.Log("[Dispell_Manager]  Spell Cast: Dispelling LowestHP : " + LowestHP.First().Health , LowestHP.First().TargetString);
                        return CastPurify(LowestHP.First().TargetString);
                    }
                   

                }

                return false;
            }
            else
            {
                //DebugLog.Log("[Dispell_Manager]  Timer Reset Timer = Now: ", " ");
                dispellTimer = DateTime.Now;
            }

            return false;
        }
        public bool Utility_Rotation()
        {
            
            // Auto Angelic Feather
           if(MovementTimer > 1)
           {
               if(CanCast(AngelicFeather) && !HasBuff(AngelicFeather) && !nofeather )
                {
                    Aimsharp.Cast("AF_player");
                    return true;
                }
           }

           // Bursting Handler 
           if(BurstingActive)
           {
               if(Bursting_Manager())
               {
                   DebugLog.Log("[Bursting_Manager]", "== Complete " + BurstingStacks);
                   return true;
               }               
           }
            
            return false;
        }

        // Core Healing Functions 
        public bool CoreHealing_Disc(bool LostSoulPhase = false)
        {  
            
            
            #region Auto Target
            if(AutoEnterCombat())
            {
                 DebugLog.Log( "CoreHealing", "== Complete == PlayerInCombat Function");
            }


            if(AutoTarget())
            {
                DebugLog.Log( "CoreHealing", "== complete == Auto Target Function / Target was not Special:  " + TargetIsSpecial());
            }
            
            #endregion
            #region Boss Logics 
            // BOSS LOGICS 
            // WitherSeed 
            if(TargetIsWitheringSeed && WitherSeed_Manager())
            {
                DebugLog.Log( "TargetIsWitheringSeed", "== Complete == WitherSeed_Manager");
                return true;
            }

            // Anduins Hope
            if(LostSoulActive && !LostSoulPhase)
            {
                 DebugLog.Log( "CoreHealing", "== Test == Entering Lost Soul Rotation");
                // Run Anduin's Hope Rotation
                if(TargetIsAnduinsHope && AnduinsHope())
                {
                    DebugLog.Log( "AnduinsHope", "== PASS == Lost Soul Rotation Coplete");
                    return true;
                }
            }

            //Anduin - If in Barrier Drop hammer if we can 
            if(BefouledBarrierActive)
            {
                DebugLog.Log( "CoreHealing", "== Test == Entering Befouled Barrier Rotation");
                // Run Anduin's Hope Rotation
                if(BefouledBarrier())
                {
                    DebugLog.Log( "CoreHealing", "== PASS == Befouled Barrier Rotation Coplete");
                    return true;
                }
            }

            // https://www.wowhead.com/spell=362405/kingsmourne-hungers
            // If Andiun is casting KingsMourne Pool HP 
            CastingKingsMourne = Aimsharp.CastingID("target") == 362405;
            if(CastingKingsMourne)
            {
                DebugLog.Log( "CastingKingsMourne", "== Test == CastingKingsMourne HP: =>  " + HolyPower);
                DebugLog.Log( "CastingKingsMourne", "Pooling HP  =>  " + HolyPower);
                DebugLog.Log( "CastingKingsMourne", "Casting Function Complete  Looping =>  " + HolyPower);
                return true;
                
            }

            //https://www.wowhead.com/spell=365805/empowered-hopebreaker
            //https://www.wowhead.com/spell=361817/hopebreaker
            //https://www.wowhead.com/spell=361815/hopebreaker
            CastingHopeBreaker = Aimsharp.CastingID("target") == 361815 || Aimsharp.CastingID("target") == 361817 || Aimsharp.CastingID("target") == 365805;

            #endregion
            #region Urgent - Self Healing
            // Emergency Rotations 
            if(UrgentSelf_Healing())
            {
                DebugLog.Log( "UrgentSelf_Healing", "== Activated and Ran ==");
                return true;
            }
            #endregion


           // =======   MO Manual Stuff   =======================
            /*
            int SWMODPS = Aimsharp.CustomFunction("SWMO");
            DebugLog.Log("[Dotting With Manual MO]", " START -----------  Casting SWP on Target for FOR SWP MO  " + SWMODPS);
            if( SWMODPS == 1 )
            {
                if(CanCast(ShadowWordPain) && !HasBuff(BoonoftheAscended, "player")) 
                {
                    DebugLog.Log("[Dotting With Manual MO]", " Casting SWP on Target for FOR SWP MO  " + SWMODPS);
                    Cast("SWPMO");
                    return true;
                }
            }
            */
           
            // Use PowerWordLife if people are low
            if(CanCast(PowerWordLife, "player") && PowerWordLife_Manager())
            {
                 DebugLog.Log( "PowerWordLife_Manager", "== Complete ==");
                return true;
            }
            //buffed PW:S with Weal and Woe
            if(CanCast(PowerWordShield) && PowerWordShield_Manager_Disc() && HasBuff(WealandWoe) )
            {
                DebugLog.Log( "[PowerWordShield_Manager_Disc]", " == Complete == ");
                return true;
            }

            // Purge the Wicked 
            if(CanCast(PurgetheWicked) && !HasDebuff(PurgetheWicked))
            {
                DebugLog.Log( "[CoreHealing_Disc]", " == PurgetheWicked == ");
                Cast(PurgetheWicked);
            }
            
            if(CanCast(Renew, "player") && Renew_Atonement_Manager())
            {
                DebugLog.Log( "Renew_Atonement_Manager", "== Complete ==");
                return true;
            }

            // Check for Healing Needed 
            if(CanCast(PowerWordRadiance) && LastCast != PowerWordRadiance && PowerWordRadiance_Manager_Disc_Mplus())
            {
                DebugLog.Log( "[PowerWordRadiance_Manager_Disc_Mplus]", " == Complete == ");
                return true;
            }
           

            if(CanCast(FlashHeal) && FlashHeal_Manager())
            {
                DebugLog.Log( "[FlashHeal_Manager]", " == Complete == ");
                return true;
            }

           
            
            if(AtonementCount >= 4 && AtonementRotation(AtonementCount) && !CriticalHealingNeeded())
            {
                DebugLog.Log( "[AtonementRotation] Atonement Count:  " + AtonementCount, " == Complete == ");
                return true;
            }
           

            if(Moving)
            {
                if(CanCast(Renew, "player") && Renew_Moving_Manager())
                {
                    DebugLog.Log( "Renew_Moving_Manager", "== Complete ==");
                    return true;
                }
            }
           
             // DPS if we are safe to to DPS 
            if(TargetIsEnemy && !CriticalHealingNeeded())
            {
                if(DPS_Rotation_Disc() && !healonly)
                {
                    //Cast Smite or Holy Nova if over 3 enemies
                    DebugLog.Log( "DPS_Rotation", "== Complete ==  DPS_Focus_HP AHP%:  "+ AveragePartyHealth);
                    return true;
                }
            }

            
            return true;
        }
    
        public bool OOCHealing()
        {   
            //Aimsharp.PrintMessage("OOC Movement Timer  " + MovementTimer, Color.White);
            //
            // Force Combat when Party or target in combat 
         
           
            //Aimsharp.PrintMessage("Running OOC Healing  -- Option ON" , Color.White);
            // Keep renew on the Tank
            if(CanCast(Renew, "player") && Renew_Tank_Manager())
            {
                DebugLog.Log( "OOCHealing Renew_Manager", "== Complete ==");
                return true;
            }

             //Keep up Flash Concentration
            if(CanCast(FlashHeal, "player") && FlashHeal_Manager() )
            {
                DebugLog.Log( "OOCHealing FlashConcentration_Manager", "== Complete ==");
                return true;
            }
        
            if(Moving)
            {
                DebugLog.Log( "======    OOC Moving  ", "==========");
             
                if(CanCast(Renew, "player") && Renew_Moving_Manager())
                {
                    DebugLog.Log( "OOCHealing Renew_Moving_Manager", "== Complete ==");
                    return true;
                }                      
            }

            

            return false;
        }
        #endregion
        
        #endregion
    }

    //Aimsharp System
    public class RaidHealing : Rotation
    {
        static SimC simc;
        string FiveLetters;

        static bool InitializeSettings = true;
        bool Authenticated = true;
        

        public override void LoadSettings()
        {
            //Authenticated = Utility.Check();
            Authenticated = Utility.CheckFree();
            simc = new SimC(this);

            FiveLetters = Aimsharp.GetAddonName().Substring(0, 5);

            //Settings.Add(new Setting("General Settings"));
            //Settings.Add(new Setting("Trinkets"));
            /*
            Settings.Add(new Setting("Use Top Trinket:", false));
            Settings.Add(new Setting("Top Trinket Defensive:", false));

            Settings.Add(new Setting("Use Bottom Trinket:", false));
            Settings.Add(new Setting("Bottom Trinket Defensive:", false));
            Settings.Add(new Setting(SimC.BottomTrinketThreshold, 1, 90, 50));
            */

            //Settings.Add(new Setting("Interrupt Settings"));
            //Settings.Add(new Setting("Auto Interrupt:", false));
            //Settings.Add(new Setting("Use Blinding Light as Interrupt:", false));
            //Settings.Add(new Setting("Use Hammer if Justice as Interrupt:", false));
            

            //Settings.Add(new Setting("|| Emergency Healing || || ||"));
            //Settings.Add(new Setting(SimC.URGENT_TANK, 0, 100, 35));
            //Settings.Add(new Setting(SimC.URGENT_RAID, 0, 100, 35));
            Settings.Add(new Setting("||||||    Defensives      ||||||"));
            Settings.Add(new Setting(SimC.DesperatePrayerP, 0, 90, 55));
            Settings.Add(new Setting(SimC.FadeP, 0, 90, 45));
            Settings.Add(new Setting("||||||    Core Spells     ||||||"));
            Settings.Add(new Setting(SimC.PWR_Count, 0, 6, 3));
            Settings.Add(new Setting(SimC.PWR_HP, 0, 100, 97));
            Settings.Add(new Setting(SimC.Renew_HP, 0, 100, 95));
            Settings.Add(new Setting(SimC.FH_HP, 0, 100, 75));
            
            Settings.Add(new Setting(SimC.PowerWordLife_Tank_HP, 0, 100, 35));
            Settings.Add(new Setting(SimC.PowerWordLife_DKTank_HP, 0, 100, 25));
            Settings.Add(new Setting(SimC.PowerWordLife_Party_HP, 0, 100, 45));
           
            Settings.Add(new Setting(SimC.Renew_OOC, 0, 100, 100));
            Settings.Add(new Setting(SimC.Renew_Tank, 0, 100, 100));
            Settings.Add(new Setting(SimC.Renew_Party, 0, 100, 98));
            Settings.Add(new Setting("||||||      Cooldowns       ||||||"));
            Settings.Add(new Setting(SimC.Rapture_HP, 0, 100, 60));
            
            Settings.Add(new Setting("||||||      Utility       ||||||"));
            Settings.Add(new Setting(SimC.PainSuppression_Tank, 0, 100, 50));
            Settings.Add(new Setting(SimC.PainSuppression_DK, 0, 100, 35));
            Settings.Add(new Setting(SimC.PainSuppression_Party, 0, 100, 45));

            //Settings.Add(new Setting(SimC.Guardian_Tank_HP, 0, 100, 35));
            //Settings.Add(new Setting(SimC.Guardian_Party_HP, 0, 100, 45));
            //Settings.Add(new Setting(SimC.GuardianA_Tank_HP, 0, 100, 65));
            //Settings.Add(new Setting(SimC.GuardianA_Party_HP, 0, 100, 55));
            //Settings.Add(new Setting(SimC.Apotheosis_AHP, 0, 100, 75));
            //Settings.Add(new Setting(SimC.Apotheosis_Optimize, false));
            Settings.Add(new Setting("||||||    Mana Options    ||||||"));
            Settings.Add(new Setting(SimC.ManaPot, 0, 100, 35));
            Settings.Add(new Setting(SimC.SymbolofHopeP, 0, 100, 55));
            Settings.Add(new Setting(SimC.TurnOFF_AT, false));
            Settings.Add(new Setting(SimC.ArcaneTorrentP, 0, 100, 65));

            Settings.Add(new Setting("||||||    DPS Options     ||||||"));
            Settings.Add(new Setting(SimC.DPS_Focus_HP, 0, 100, 88));
            Settings.Add(new Setting("||||||    Special Options     ||||||"));
            Settings.Add(new Setting(SimC.CriticalHealingPT, 0, 100, 45));
            Settings.Add(new Setting(SimC.CriticalHealingP, 0, 100, 25));

            Settings.Add(new Setting("||||||    Consumables     ||||||"));
            Settings.Add(new Setting(SimC.HealthStoneThreshold, 0, 90, 35));
            Settings.Add(new Setting(SimC.HealthPot, 0, 90, 60));
            

            Settings.Add(new Setting("||||||  Weakaura Options   ||||||"));
            Settings.Add(new Setting("Weakaura Support", false));
        }

        public override void Initialize()
        {
            Aimsharp.DebugMode(true);
            
            Aimsharp.PrintMessage("--- ----- ----- ---- --- ----- ----- ----- ", simc.white);
            Aimsharp.PrintMessage("     +++   Disc Preist  +++          ", simc.col);
            Aimsharp.PrintMessage("             Mythic +                 ", simc.col);
            Aimsharp.PrintMessage("            DF v1.0              ", simc.white);
            Aimsharp.PrintMessage("--- ----- ----- ---- --- ----- ----- -----  ", simc.white);


            if (Authenticated)
            {
                simc.WeakauraSupport = GetCheckBox("Weakaura Support");
               
                if (simc.WeakauraSupport)
                {
                    Aimsharp.PrintMessage("WEAKAURA SUPPORT ENABLED", Color.Yellow);
                    Aimsharp.PrintMessage("Use the Weakaura Buttons for Toggles", Color.Yellow);
                    Aimsharp.PrintMessage("You can still use these macros to control the Toggles", Color.Yellow);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyPrPause' SetCVar(cvar, 1-GetCVar(cvar),cvar)  -- Toggles Rotaion On/Off", simc.col);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyPrDPSOnly' SetCVar(cvar, 1-GetCVar(cvar),cvar)  -- Toggles DPS only rotation", simc.col);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyPrHealOnly' SetCVar(cvar, 1-GetCVar(cvar),cvar)  -- Toggles Healing only rotation", simc.col);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyPrFCOOC' SetCVar(cvar, 1-GetCVar(cvar),cvar)  -- Toggles Keeping FlashCon up out of combat!!", simc.col);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyPrOOCHeal' SetCVar(cvar, 1-GetCVar(cvar),cvar)  -- Toggles Out of Combat Healing On/Off.", simc.col);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyNoPurify' SetCVar(cvar, 1-GetCVar(cvar),cvar)  --Toggles Cleanse On/Off", simc.col);
                    Aimsharp.PrintMessage("/run local cvar = 'HolyNoCDs' SetCVar(cvar, 1-GetCVar(cvar),cvar)  --Toggles Using Major CDs in any auto logic", simc.col);
                    
                }
                else
                {
                    Aimsharp.PrintMessage("TOGGLES", Color.Yellow);
                    Aimsharp.PrintMessage("/" + FiveLetters + " pause -- Toggles Rotaion On/Off.", simc.col);
                    Aimsharp.PrintMessage("/" + FiveLetters + " dpsonly -- Toggles Healing On/Off (except Serenity, GS)", simc.col);
                    Aimsharp.PrintMessage("/" + FiveLetters + " healonly --- Toggles Healing only rotation", simc.col);
                    Aimsharp.PrintMessage("/" + FiveLetters + " oocheal -- Toggles Out of Combat Healing On/Off.", simc.col);
                    Aimsharp.PrintMessage("/" + FiveLetters + " nopurify ---Toggles Cleanse On/Off", simc.col);
                    Aimsharp.PrintMessage("/" + FiveLetters + " nocds --Toggles Using Major CDs in any auto logic", simc.col);
                   
                }

                // Aimsharp.PrintMessage("/" + FiveLetters + " SoloDolo -- Toggles Solo Questing On/Off.", Color.Blue);

                Aimsharp.PrintMessage("--- ----- ----- ---- --- ----- ----- ---- --- ----- -----", simc.white);
                Aimsharp.PrintMessage("              Queues           ", simc.col);
                Aimsharp.PrintMessage("--- ----- ----- ---- --- ----- ----- ---- --- ----- -----", simc.white);

                Aimsharp.PrintMessage("These Queue macros can be used for manual control:",  simc.col);

                //Aimsharp.PrintMessage("/" + FiveLetters + " SaveCooldowns", simc.col);
                //Aimsharp.PrintMessage("   --- Toggles the use of big cooldowns on/off.", simc.white);
                //Aimsharp.PrintMessage("/" + FiveLetters + " SaveMana", simc.col);
                //Aimsharp.PrintMessage("   --- Toggles the use of EF and Vivify to save mana on/off.", simc.white);
                //Aimsharp.PrintMessage("/" + FiveLetters + " OOCHeal", simc.col);
                Aimsharp.PrintMessage("--- ----- ----- ---- --- --- -----", simc.col);
                Aimsharp.PrintMessage("--- ----- ----- ---- --- ----- ----- ---- --- ----- -----", simc.white);
                Aimsharp.PrintMessage("              Modes           ", simc.col);
                Aimsharp.PrintMessage("--- ----- ----- ---- --- ----- ----- ---- --- ----- -----", simc.white);
                // Aimsharp.PrintMessage("/" + FiveLetters + " SoloDolo", simc.col);
                //Aimsharp.PrintMessage("   --- Toggles the use of Solo questing and torghast rotation on/off.", simc.white);


                Aimsharp.Latency = 50;
                Aimsharp.QuickDelay = 150;
                Aimsharp.SlowDelay = 350;

                // Start the DebugLog
                DebugLog.Initialize("", 1000);

                int i = 1;
                foreach (string Spell in simc.SpellsList)
                {
                    Spellbook.Add(Spell);

                    if (i <= simc.SPELLS_CUSTOMIZATION)
                    {
                        if (simc.ASRotationAuto(Spell, "Queue"))
                        {
                            Aimsharp.PrintMessage(FiveLetters + " " + Utility.RemoveWhitespace(Spell) + " - Queue the specified spell", Color.Blue);
                            Macros.Add(Utility.RemoveWhitespace(Spell) + "Off", "/" + FiveLetters + " " + Utility.RemoveWhitespace(Spell));
                            CustomCommands.Add(Utility.RemoveWhitespace(Spell));
                        }
                    }
                    ++i;
                }


                foreach (string Spell in simc.Racials)
                {
                    Spellbook.Add(Spell);
                }

                foreach (string Buff in simc.GeneralBuffs)
                {
                    Buffs.Add(Buff);
                }

                foreach (string Buff in simc.BuffsList)
                {
                    Buffs.Add(Buff);
                }

                foreach (string Buff in simc.BloodlustEffects)
                {
                    Buffs.Add(Buff);
                }

                foreach (string Debuff in simc.DebuffsList)
                {
                    Debuffs.Add(Debuff);
                }

                foreach (string Debuff in simc.GeneralDebuffs)
                {
                    Debuffs.Add(Debuff);
                }

                foreach (string Item in simc.Items)
                {
                    Items.Add(Item);
                }

                // Custom Macros
                {

                  
                    Macros.Add(SimC.MindSooth + "Off", "/" + FiveLetters + " " + Utility.RemoveWhitespace(SimC.MindSooth));
                 
                    Macros.Add(SimC.MassDispel + "Off", "/" + FiveLetters + " " + Utility.RemoveWhitespace(SimC.MassDispel));
                    Macros.Add(SimC.MassDispel+"C" + "Off", "/" + FiveLetters + " " + Utility.RemoveWhitespace(SimC.MassDispel+"C"));
                }

                #region Common Macros 
                //Items.Add(GetString("Potion name:"));
                //Macros.Add("DPS Pot", "/use " + GetString("Potion name:"));
                Macros.Add("TopTrinket", "/use 13");
                Macros.Add("BottomTrinket", "/use 14");
                Macros.Add("Wait", "/" + FiveLetters + " wait 0.25");
                Macros.Add("Healthstone", "/use Healthstone");
                Macros.Add("CosmicHPPotion", "/use Cosmic Healing Potion");
                Macros.Add("ManaPotion", "/use Spiritual Mana Potion");
                Macros.Add("SpiritualHPPotion", "/use Spiritual Healing Potion");
                Macros.Add("StopAttack", "/stopattack");
                Macros.Add("StartAttack", "/startattack");

                // Targeting
                Macros.Add("FOC_player", "/focus player");
                Macros.Add("TargetEnemy", "/targetenemy");
                Macros.Add("TargetFocusTarget", "/target focustarget");

                Macros.Add("FOC_party1", "/focus party1");
                Macros.Add("FOC_party2", "/focus party2");
                Macros.Add("FOC_party3", "/focus party3");
                Macros.Add("FOC_party4", "/focus party4");
               

                #endregion

                #region Class Macros
                // Holy Palaldin Macros
                Macros.Add("DISPEL_FOC", "/cast [@focus] Purify"); // formely Macros.Add("CL_FOC", "/cast [@focus] Cleanse");
                Macros.Add("FH_FOC", "/cast [@focus] Flash Heal");
                Macros.Add("H_FOC", "/cast [@focus] Heal");
                Macros.Add("R_FOC", "/cast [@focus] Renew");
                Macros.Add("PH_FOC", "/cast [@focus] Prayer of Healing");
                Macros.Add("CH_FOC", "/cast [@focus] Circle of Healing");
                Macros.Add("PM_FOC", "/cast [@focus] Prayer of Mending");
                Macros.Add("HWS_FOC", "/cast [@focus] Holy Word: Serenity");
                Macros.Add("GS_FOC", "/cast [@focus] Guardian Spirit");
                Macros.Add("FAE_FOC", "/cast [@focus] Fae Guardians");
                Macros.Add("DM_FOC", "/cast [@focus] Direct Mask");
                Macros.Add("PWL_FOC", "/cast [@focus] Power Word: Life");
                Macros.Add("PWS_FOC", "/cast [@focus] Power Word: Shield");
                Macros.Add("PWR_FOC", "/cast [@focus] Power Word: Radiance");
                Macros.Add("PSUP_FOC", "/cast [@focus] Pain Suppression");
                Macros.Add("SCV_FOC", "/cast [@focus] Shadow Covenant");
                Macros.Add("PN_FOC", "/cast [@focus] Penance");

                // Utility
                Macros.Add("AF_player", "/cast [@player] Angelic Feather");
                Macros.Add("PurifyPlayer", "/cast [@player] Purify");
                Macros.Add("PurifyParty1", "/cast [@party1] Purify");
                Macros.Add("PurifyParty2", "/cast [@party2] Purify");
                Macros.Add("PurifyParty3", "/cast [@party3] Purify");
                Macros.Add("PurifyParty4", "/cast [@party4] Purify");
                Macros.Add("MD_cursor", "/cast [@cursor] Mass Dispel");
                Macros.Add("MD_player", "/cast [@player] Mass Dispel");
                Macros.Add("HWSanc_cursor", "/cast [@cursor] Holy Word: Sanctify");
                Macros.Add("HWSanc_player", "/cast [@player] Holy Word: Sanctify");


                // MouseOver 
                Macros.Add("SWPMO", "/cast [@mouseover, harm, nodead] Shadow Word: Pain");
                Macros.Add("PTWMO", "/cast [@mouseover, harm, nodead] Purge The Wicked");
                Macros.Add("MCMO", "/cast [@mouseover, harm, nodead] Mind Control");
               
               // Macros.Add("BlessingofProtectionMO", "/cast [@mouseover,exists] Blessing of Protection");
               #endregion
            

                foreach (string MacroCommand in simc.MacroCommands)
                {
                    CustomCommands.Add(MacroCommand);
                }

                if (!simc.WeakauraSupport)
                {
                    CustomCommands.Add("pause");
                    CustomCommands.Add("dpsonly");
                    CustomCommands.Add("healonly");
                    CustomCommands.Add("oocheal");
                    CustomCommands.Add("nopurify");
                    CustomCommands.Add("nocds");
                }
                CustomCommands.Add("solo");
                CustomCommands.Add("nofeather");
                CustomCommands.Add("dktank");
               

                // Common Custom Functions 
                #region Inferno Custom Functions
                
                CustomFunctions.Add("CheckMOExplosive",
                "local result = 0" +
                "\n if(GetUnitName(\"mouseover\") == \"Explosives\") then result = 1 end" +
                "\nreturn result");
                

                CustomFunctions.Add("SpellPowerValue", "return GetSpellBonusHealing();");
                CustomFunctions.Add("MasteryValue", "local mastery, coefficient = GetMasteryEffect(); return mastery;");
                CustomFunctions.Add("StaggerValue", "if(UnitStagger(\"player\"))\nthen\nreturn UnitStagger(\"player\");\nend\n return 0;");
                CustomFunctions.Add("TimeToDeathPack",
                    "local UnitMaxHP = 0" +
                    "\nlocal UnitCurrentHP = 0" +
                    "\nlocal Time = " + (float)Aimsharp.CombatTime() * 0.001f +
                    "\nfor i=1,20 do" +
                    "\n   local unit = \"nameplate\" .. i" +
                    "\n   if UnitExists(unit) then" +
                    "\n      if UnitCanAttack(\"player\", unit) and UnitAffectingCombat(unit)" +
                    "\n      then" +
                    "\n         UnitCurrentHP = UnitCurrentHP + UnitHealth(unit)" +
                    "\n         UnitMaxHP = UnitMaxHP + UnitHealthMax(unit)" +
                    "\n      end" +
                    "\n   end" +
                    "\nend" +
                    "\nlocal dps = (UnitMaxHP - UnitCurrentHP) / (Time + 0.001)" +
                    "\nif dps < 10 then dps = 10 end" +
                    "\nreturn (UnitCurrentHP / dps)");

                CustomFunctions.Add("PackMaxHP",
                    "local UnitMaxHP = 0" +
                    "\nfor i=1,20 do" +
                    "\n   local unit = \"nameplate\" .. i" +
                    "\n   if UnitExists(unit) then" +
                    "\n      if UnitCanAttack(\"player\", unit) and UnitAffectingCombat(unit)" +
                    "\n      then" +
                    "\n         UnitMaxHP = UnitMaxHP + UnitHealthMax(unit)" +
                    "\n      end" +
                    "\n   end" +
                    "\nend" +
                    "\nreturn UnitMaxHP");

                CustomFunctions.Add("PackHP",
                    "local UnitCurrentHP = 0" +
                    "\nfor i=1,20 do" +
                    "\n   local unit = \"nameplate\" .. i" +
                    "\n   if UnitExists(unit) then" +
                    "\n      if UnitCanAttack(\"player\", unit) and UnitAffectingCombat(unit)" +
                    "\n      then" +
                    "\n         UnitCurrentHP = UnitCurrentHP + UnitHealth(unit)" +
                    "\n      end" +
                    "\n   end" +
                    "\nend" +
                    "\nreturn UnitCurrentHP");

                CustomFunctions.Add("CheckPlayerGroup",
                    "local Members = GetNumGroupMembers()" +
                    "\nlocal IsRaid = IsInRaid()" +
                    "\nlocal result = 0;" +
                    "\nif(Members > 0)" +
                    "\nthen if IsRaid" +
                    "\n   then result = 2" +
                    "\n   else" +
                    "\n      result = 1" +
                    "\n   end" +
                    "\nend" +
                    "\nreturn result");

                CustomFunctions.Add("CheckPlayerPurify",
                    "local result = 0" +
                    "\nfor i=1,25 do " +
                    "\n   local name,_,_,type=UnitDebuff(\"player\",i,\"RAID\")" +
                    "\n   if type ~= nil and " +
                    "\n      name ~= \"Frozen Binds\" and " +
                    "\n      name ~= \"Burst\" and " +
                    "\n      name ~= \"Intangible Presence\" and " +
                    "\n      name ~= \"Chaotic Shadows\" and " +
                    "\n   (type == \"Disease\" or type == \"Magic\") " +
                    "\n   then result = 1 " +
                    "\n   end " +
                    "\nend" +
                    "\nreturn result");

                CustomFunctions.Add("CheckPartyPurify",
                "local Members = GetNumGroupMembers()" +
                "\nlocal result = 0" +
                "\nfor i = 1,Members-1 do" +
                "\n   for p=1,25 do " +
                "\n      local name,_,_,type=UnitDebuff(\"party\" ..i,p,\"RAID\")" +
                "\n      if type ~= nil and " +
                "\n      name ~= \"Frozen Binds\" and " +
                "\n      name ~= \"Burst\" and " +
                "\n      name ~= \"Intangible Presence\" and " +
                "\n      name ~= \"Chaotic Shadows\" and " +
                "\n      (type == \"Disease\" or type == \"Magic\") " +
                "\n      then result = i" +
                "\n      end" +
                "\n   end" +
                "\nend" +
                "\nreturn result");

                CustomFunctions.Add("CheckPartyPurifyGrouped",
                    "local Members = GetNumGroupMembers()" +
                    "\nlocal result = 0" +
                    "\nfor i = 1,Members-1 do" +
                    "\n   for p=1,25 do " +
                    "\n      local name,_,_,type=UnitDebuff(\"party\" ..i,p,\"RAID\")" +
                    "\n      if type ~= nil and " +
                    "\n      name ~= \"Frozen Binds\" and " +
                    "\n      name ~= \"Burst\" and " +
                    "\n      name ~= \"Intangible Presence\" and " +
                    "\n      name ~= \"Chaotic Shadows\" and " +
                    "\n      (type == \"Disease\" or type == \"Magic\") " +
                    "\n      then result = result + 2^i" +
                    "\n      end" +
                    "\n   end" +
                    "\nend" +
                    "\nreturn result");

                CustomFunctions.Add("CheckRaidPurify",
                    "local Members = GetNumGroupMembers()" +
                    "\nlocal result = 0" +
                    "\nfor i = 1,Members do" +
                    "\n   for p=1,10 do " +
                    "\n      local name,_,_,type=UnitDebuff(\"raid\" ..Members,p,1)" +
                    "\n      if type ~= nil and " +
                    "\n      name ~= \"Banshee's Bane\" and" +
                    "\n      name ~= \"Crushing Dread\" and" +
                    "\n      name ~= \"Fragments of Destiny\" and" +
                    "\n      name ~= \"Malevolence\" and" +
                    "\n      (type == \"Disease\" or type == \"Magic\") " +
                    "\n      then result = i" +
                    "\n      end" +
                    "\n   end" +
                    "\nend" +
                    "\nreturn result");

                CustomFunctions.Add("CheckRaidPurifyGrouped",
                "local Members = GetNumGroupMembers()" +
                "\nlocal result = 0" +
                "\nfor i = 1,Members do" +
                "\n   for p=1,10 do " +
                "\n      local name,_,_,type=UnitDebuff(\"raid\" ..Members,p,1)" +
                "\n      if type ~= nil and " +
                "\n      name ~= \"Banshee's Bane\" and" +
                "\n      name ~= \"Crushing Dread\" and" +
                "\n      name ~= \"Fragments of Destiny\" and" +
                "\n      name ~= \"Malevolence\" and" +
                "\n      (type == \"Disease\" or type == \"Magic\") " +
                "\n      then result = result + 2^i" +
                "\n      end" +
                "\n   end" +
                "\nend" +
                "\nreturn result");

                CustomFunctions.Add("TargetTapped", "return UnitIsTapDenied(\"target\") and 1 or 0");
                #endregion

                // Healing Custom Functions 
                #region Inferno Healing Custom Functions
                

                CustomFunctions.Add("UnitIsFocus", "local foc=0; " +
                "\nif UnitExists('focus') and UnitIsUnit('party1','focus') then foc = 1; end" +
                "\nif UnitExists('focus') and UnitIsUnit('party2','focus') then foc = 2; end" +
                "\nif UnitExists('focus') and UnitIsUnit('party3','focus') then foc = 3; end" +
                "\nif UnitExists('focus') and UnitIsUnit('party4','focus') then foc = 4; end" +
                "\nif UnitExists('focus') and UnitIsUnit('player','focus') then foc = 99; end" +
                "\nreturn foc");

                CustomFunctions.Add("DiseasePoisonMagicCheck", "local y=0; " +
                "for i=1,25 do local name,_,_,type=UnitDebuff(\"player\",i,\"RAID\"); " +
                    "if type ~= nil and name ~= \"Frozen Binds\" and name ~= \"Burst\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +1; end end " +
                "for i=1,25 do local name,_,_,type=UnitDebuff(\"party1\",i,\"RAID\"); " +
                    "if type ~= nil and name ~= \"Frozen Binds\" and name ~= \"Burst\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^1; end end " +
                "for i=1,25 do local name,_,_,type=UnitDebuff(\"party2\",i,\"RAID\"); " +
                    "if type ~= nil and name ~= \"Frozen Binds\" and name ~= \"Burst\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^2; end end " +
                "for i=1,25 do local name,_,_,type=UnitDebuff(\"party3\",i,\"RAID\"); " +
                    "if type ~= nil and name ~= \"Frozen Binds\" and name ~= \"Burst\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^3; end end " +
                "for i=1,25 do local name,_,_,type=UnitDebuff(\"party4\",i,\"RAID\"); " +
                    "if type ~= nil and name ~= \"Frozen Binds\" and name ~= \"Burst\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^4; end end " +
                "return y");

                 CustomFunctions.Add("TargetingParty", "local result = 0" +
                "\nif UnitExists('target') and UnitIsUnit('targettarget','party1') then result = 1 end" +
                "\nif UnitExists('target') and UnitIsUnit('targettarget','party2') then result = 2 end" +
                "\nif UnitExists('target') and UnitIsUnit('targettarget','party3') then result = 3 end" +
                "\nif UnitExists('target') and UnitIsUnit('targettarget','party4') then result = 4 end" +
                "\nif UnitExists('target') and UnitIsUnit('targettarget','player') then result = 5 end" +
                "\nreturn result");

                CustomFunctions.Add("TargetIsParty", "local result = 0" +
                "\nif UnitExists('target') and UnitIsUnit('target','party1') then result = 1 end" +
                "\nif UnitExists('target') and UnitIsUnit('target','party2') then result = 2 end" +
                "\nif UnitExists('target') and UnitIsUnit('target','party3') then result = 3 end" +
                "\nif UnitExists('target') and UnitIsUnit('target','party4') then result = 4 end" +
                "\nif UnitExists('target') and UnitIsUnit('target','player') then result = 5 end" +
                "\nreturn result");
                
                CustomFunctions.Add("TargetIsEmpty", "local result = 0" +
                "\nif UnitExists('target') then result = 1 end" +
                "\nreturn result");

                CustomFunctions.Add("PlayerisParty", "local foc=0; " +
                "\nif UnitIsUnit('party1','player') then foc = 1; end" +
                "\nif UnitIsUnit('party2','player') then foc = 2; end" +
                "\nif UnitIsUnit('party3','player') then foc = 3; end" +
                "\nif UnitIsUnit('party4','player') then foc = 4; end" +
                "\nif UnitIsUnit('party5','player') then foc = 5; end" +
                "\nreturn foc");

                /* 
                ============  Raid Custom Functions ====================
                Raid Units is focus 
                CustomFunctions.Add("UnitIsFocus", "local foc=0; " +
                "\nif UnitExists('focus') and UnitIsUnit('raid1','focus') then foc = 1; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid2','focus') then foc = 2; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid3','focus') then foc = 3; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid4','focus') then foc = 4; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid5','focus') then foc = 5; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid6','focus') then foc = 6; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid7','focus') then foc = 7; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid8','focus') then foc = 8; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid9','focus') then foc = 9; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid10','focus') then foc = 10; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid11','focus') then foc = 11; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid12','focus') then foc = 12; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid13','focus') then foc = 13; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid14','focus') then foc = 14; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid15','focus') then foc = 15; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid16','focus') then foc = 16; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid17','focus') then foc = 17; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid18','focus') then foc = 18; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid19','focus') then foc = 19; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid20','focus') then foc = 20; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid21','focus') then foc = 21; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid22','focus') then foc = 22; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid23','focus') then foc = 23; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid24','focus') then foc = 24; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid25','focus') then foc = 25; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid26','focus') then foc = 26; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid27','focus') then foc = 27; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid28','focus') then foc = 28; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid29','focus') then foc = 29; end" +
                "\nif UnitExists('focus') and UnitIsUnit('raid30','focus') then foc = 30; end" +
                "\nif UnitExists('focus') and UnitIsUnit('player','focus') then foc = 99; end" +
                "\nreturn foc");

                CustomFunctions.Add("DiseasePoisonMagicCheck", "local y=0; " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"player\",i,\"RAID\"); " +
                    "if type ~= nil and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +1; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid1\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid1','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^1; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid2\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid2','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^2; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid3\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid3','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^3; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid4\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid4','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^4; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid5\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid5','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^5; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid6\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid6','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^6; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid7\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid7','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^7; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid8\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid8','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^8; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid9\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid9','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^9; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid10\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid10','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^10; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid11\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid11','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^11; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid12\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid12','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^12; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid13\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid13','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^13; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid14\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid14','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^14; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid15\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid15','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^15; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid16\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid16','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^16; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid17\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid17','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^17; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid18\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid18','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^18; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid19\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid19','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^19; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid20\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid20','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^20; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid21\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid21','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^21; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid22\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid22','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^22; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid23\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid23','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^23; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid24\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid24','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^24; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid25\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid25','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^25; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid26\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid26','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^26; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid27\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid27','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^27; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid28\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid28','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^28; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid29\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid29','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^29; end end " +
                "for i=1,30 do local name,_,count,type=UnitDebuff(\"raid30\",i,\"RAID\"); " +
                    "if type ~= nil and not UnitIsUnit('raid30','player') and name ~= \"Banshee's Bane\" and (name ~= \"Crushing Dread\" or name == \"Crushing Dread\" and count >= 5) and name ~= \"Fragments of Destiny\" and name ~= \"Malevolence\" and (type == \"Disease\" or type == \"Poison\" or type == \"Magic\") then y = y +2^30; end end " +
                "return y");

                 CustomFunctions.Add("PlayerIsRaid", "local foc=0; " +
                "\nif UnitIsUnit('raid1','player') then foc = 1; end" +
                "\nif UnitIsUnit('raid2','player') then foc = 2; end" +
                "\nif UnitIsUnit('raid3','player') then foc = 3; end" +
                "\nif UnitIsUnit('raid4','player') then foc = 4; end" +
                "\nif UnitIsUnit('raid5','player') then foc = 5; end" +
                "\nif UnitIsUnit('raid6','player') then foc = 6; end" +
                "\nif UnitIsUnit('raid7','player') then foc = 7; end" +
                "\nif UnitIsUnit('raid8','player') then foc = 8; end" +
                "\nif UnitIsUnit('raid9','player') then foc = 9; end" +
                "\nif UnitIsUnit('raid10','player') then foc = 10; end" +
                "\nif UnitIsUnit('raid11','player') then foc = 11; end" +
                "\nif UnitIsUnit('raid12','player') then foc = 12; end" +
                "\nif UnitIsUnit('raid13','player') then foc = 13; end" +
                "\nif UnitIsUnit('raid14','player') then foc = 14; end" +
                "\nif UnitIsUnit('raid15','player') then foc = 15; end" +
                "\nif UnitIsUnit('raid16','player') then foc = 16; end" +
                "\nif UnitIsUnit('raid17','player') then foc = 17; end" +
                "\nif UnitIsUnit('raid18','player') then foc = 18; end" +
                "\nif UnitIsUnit('raid19','player') then foc = 19; end" +
                "\nif UnitIsUnit('raid20','player') then foc = 20; end" +
                "\nif UnitIsUnit('raid21','player') then foc = 21; end" +
                "\nif UnitIsUnit('raid22','player') then foc = 22; end" +
                "\nif UnitIsUnit('raid23','player') then foc = 23; end" +
                "\nif UnitIsUnit('raid24','player') then foc = 24; end" +
                "\nif UnitIsUnit('raid25','player') then foc = 25; end" +
                "\nif UnitIsUnit('raid26','player') then foc = 26; end" +
                "\nif UnitIsUnit('raid27','player') then foc = 27; end" +
                "\nif UnitIsUnit('raid28','player') then foc = 28; end" +
                "\nif UnitIsUnit('raid29','player') then foc = 29; end" +
                "\nif UnitIsUnit('raid30','player') then foc = 30; end" +
                "\nreturn foc");

                CustomFunctions.Add("UnitIsTanking", "local foc=0; " +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid1','targettarget') and UnitGroupRolesAssigned('raid1') == 'TANK' then foc = 1; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid2','targettarget') and UnitGroupRolesAssigned('raid2') == 'TANK' then foc = 2; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid3','targettarget') and UnitGroupRolesAssigned('raid3') == 'TANK' then foc = 3; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid4','targettarget') and UnitGroupRolesAssigned('raid4') == 'TANK' then foc = 4; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid5','targettarget') and UnitGroupRolesAssigned('raid5') == 'TANK' then foc = 5; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid6','targettarget') and UnitGroupRolesAssigned('raid6') == 'TANK' then foc = 6; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid7','targettarget') and UnitGroupRolesAssigned('raid7') == 'TANK' then foc = 7; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid8','targettarget') and UnitGroupRolesAssigned('raid8') == 'TANK' then foc = 8; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid9','targettarget') and UnitGroupRolesAssigned('raid9') == 'TANK' then foc = 9; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid10','targettarget') and UnitGroupRolesAssigned('raid10') == 'TANK' then foc = 10; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid11','targettarget') and UnitGroupRolesAssigned('raid11') == 'TANK' then foc = 11; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid12','targettarget') and UnitGroupRolesAssigned('raid12') == 'TANK' then foc = 12; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid13','targettarget') and UnitGroupRolesAssigned('raid13') == 'TANK' then foc = 13; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid14','targettarget') and UnitGroupRolesAssigned('raid14') == 'TANK' then foc = 14; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid15','targettarget') and UnitGroupRolesAssigned('raid15') == 'TANK' then foc = 15; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid16','targettarget') and UnitGroupRolesAssigned('raid16') == 'TANK' then foc = 16; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid17','targettarget') and UnitGroupRolesAssigned('raid17') == 'TANK' then foc = 17; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid18','targettarget') and UnitGroupRolesAssigned('raid18') == 'TANK' then foc = 18; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid19','targettarget') and UnitGroupRolesAssigned('raid19') == 'TANK' then foc = 19; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid20','targettarget') and UnitGroupRolesAssigned('raid20') == 'TANK' then foc = 20; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid21','targettarget') and UnitGroupRolesAssigned('raid21') == 'TANK' then foc = 21; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid22','targettarget') and UnitGroupRolesAssigned('raid22') == 'TANK' then foc = 22; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid23','targettarget') and UnitGroupRolesAssigned('raid23') == 'TANK' then foc = 23; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid24','targettarget') and UnitGroupRolesAssigned('raid24') == 'TANK' then foc = 24; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid25','targettarget') and UnitGroupRolesAssigned('raid25') == 'TANK' then foc = 25; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid26','targettarget') and UnitGroupRolesAssigned('raid26') == 'TANK' then foc = 26; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid27','targettarget') and UnitGroupRolesAssigned('raid27') == 'TANK' then foc = 27; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid28','targettarget') and UnitGroupRolesAssigned('raid28') == 'TANK' then foc = 28; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid29','targettarget') and UnitGroupRolesAssigned('raid29') == 'TANK' then foc = 29; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('raid30','targettarget') and UnitGroupRolesAssigned('raid30') == 'TANK' then foc = 30; end" +
                "\nif UnitExists('targettarget') and UnitIsUnit('player','targettarget') and UnitGroupRolesAssigned('player') == 'TANK' then foc = 99; end" +
                "\nreturn foc");
                */

            
                CustomFunctions.Add("SpellHaste", "if GetHaste() ~= nil and GetHaste() * 1000 > 0 then return math.floor(GetHaste() * 1000) end return 0");

                #endregion


                // Get CVars
                int k = 0;
                string cfunc = "";
                foreach (string s in simc.CVarList)
                {
                    cfunc += "val = GetCVarBool(\"" + s + "\")\n";
                    cfunc += "if val == true then result = result + 2^" + k.ToString() + " end\n";
                    ++k;
                }

                CustomFunctions.Add("GetCVars",
                    "local result = 0" +
                    "\nlocal val = nil" +
                    "\n" + cfunc +
                    "\nreturn result");

       
                CustomFunctions.Add("GetSpellQueueWindow", 
                    "local cvar = \"SpellQueueWindow\"" +
                    "\nlocal sqw = GetCVar(cvar)" +
                    "\nlocal sqwn = 0" +
                    "\nlocal _, _, home, world = GetNetStats()" +
                    "\nlocal latency = home + world" +
                    "\nif sqw == nil" +
                    "\nthen" +
                    "\n   sqwn = latency + 50" +
                    "\nelse if tonumber(sqw) < latency" +
                    "\n   then" +
                    "\n      sqwn = latency + 50" +
                    "\n   else" +
                    "\n      sqwn = tonumber(sqw);" +
                    "\n      if(sqwn > latency + 50)" +
                    "\n      then" +
                    "\n         sqwn = latency + 50" +
                    "\n      end" +
                    "\n   end " +
                    "\nend" +
                    "\nif(sqwn > 400) then sqwn = 400 end" +
                    "\nif(sqwn < 100) then sqwn = 100 end" +
                    "\nSetCVar(cvar, sqwn, cvar)" +
                    "\nreturn sqwn");

                    
                    // DF Talent
                    int TalentCount = simc.AllTalents.Count;
                    for (int z = 0; z < (TalentCount / 20) + 1; ++z)
                    {
                        int startIndex = (z * 20);
                        int endIndex = (TalentCount - startIndex);
                        if (endIndex >= 20) endIndex = startIndex + 20;
                        else endIndex += startIndex;

                        cfunc = "";
                        k = 0;
                        for (int j = startIndex; j < endIndex; ++j)
                        {
                            ++k;
                            cfunc += "if(IsPlayerSpell(" + simc.AllTalents.ElementAt(j).Value.ToString() + ")) then result = result + 2^" + k.ToString() + " end\n";
                        }

                        CustomFunctions.Add("GetDFTalents" + (z + 1).ToString(),
                            "local result = 0" +
                            "\n" + cfunc +
                            "\n return result");
                    }

                    CustomFunctions.Add("CheckSerenityCD",
                    "usable, nomana = IsUsableSpell(\"Holy Word: Serenity\")" +
                    "\nif(usable) then" +
                    "\nlocal start, duration, enabled = GetSpellCooldown(\"Holy Word: Serenity\")" +
                    "\nif ( start > 0 and duration > 0) then return 0 end end" +
                    "\nreturn 1;");

                    // Arcane torrent or Racial custome fucntions being developed
                    
                    /*
                    CustomFunctions.Add("CheckArcaneTorrent",
                    "usable, nomana = IsUsableSpell(\"Arcane Torrent\")" +
                    "\nif(usable) then return 1 end" +
                    "\nreturn 0;");
                    */

                    /*
                    CustomFunctions.Add("Racial", "local race = UnitRace(\"player\")" +
                    "\nlocal raceInt = 0;" +
                    "\nif(race == \"BloodElf\") then raceInt = 1; end" +
                    "\nif(race == \"Draenei\") then raceInt = 2; end" +
                    "\nif(race == \"LightforgedDraenei\") then raceInt = 3; end" +
                    "\nif(race == \"Tauren\") then raceInt = 4; end" +
                    "\nif(race == \"Pandaren\") then raceInt = 5; end" +
                    "\nreturn raceInt;")
                    */

                    
                   CustomFunctions.Add("SWMO",
                    "local SWPMO = 0" +
                    "\nif UnitExists(\"mouseover\") then" +
                    "\n     if UnitCanAttack(\"mouseover\", \"target\") and UnitAffectingCombat(\"mouseover\") and UnitIsDead(\"mouseover\") ~= true then" +   
                    "\n     SWPMO = 1" +        
                    "\n     for j = 1, 40 do" +
                    "\n         local name,_,_,_,_,endt,source = UnitDebuff(\"mouseover\", j)" +
                    "\n         if name == \"Shadow Word: Pain\" and source == \"player\" and endt > 3 then" +
                    "\n             SWPMO = 2" +
                    "\n         end" +
                    "\n     end" +
                    "\nend" +
                    "\nend" +
                    "\nreturn SWPMO;"

                    );
                

            
            }
            else
            {
                Aimsharp.PrintMessage("NOT AUTHENTICATED!!!!!", Color.Yellow);
            }

            //CustomFunctions.Add("ExpelHarmCharges",
            //    "local _, _, auraCount, _, _, _, _, _, _, _, _, _, _, _, _, = UnitAura(\"player\", \"Expel Harm\");" +
            //    "\n if (currentCharges ~= nil) then return currentCharges; \nend \n return 0;");0

            InitializeSettings = false;
        }

        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick()
        {
            if (!Authenticated) return true;

            //Aimsharp.PrintMessage("CombatTick Start", Color.Blue);
            if (!InitializeSettings)
            {
                //Heal Settings 
                //simc.AddSetting(SimC.URGENT_RAID, GetSlider(SimC.URGENT_RAID));
                //simc.AddSetting(SimC.URGENT_TANK, GetSlider(SimC.URGENT_TANK));
                //Defensives
                simc.AddSetting(SimC.DesperatePrayerP, GetSlider(SimC.DesperatePrayerP));
                simc.AddSetting(SimC.FadeP, GetSlider(SimC.FadeP));
                //Core Spells 
                simc.AddSetting(SimC.PWR_Count , GetSlider(SimC.PWR_Count));
                simc.AddSetting(SimC.PWR_HP, GetSlider(SimC.PWR_HP));
                simc.AddSetting(SimC.Renew_HP, GetSlider(SimC.Renew_HP));
                simc.AddSetting(SimC.FH_HP, GetSlider(SimC.FH_HP));
                simc.AddSetting(SimC.PowerWordLife_Tank_HP , GetSlider(SimC.PowerWordLife_Tank_HP));
                simc.AddSetting(SimC.PowerWordLife_DKTank_HP, GetSlider(SimC.PowerWordLife_DKTank_HP));
                simc.AddSetting(SimC.PowerWordLife_Party_HP, GetSlider(SimC.PowerWordLife_Party_HP));
                simc.AddSetting(SimC.Renew_Tank , GetSlider(SimC.Renew_Tank));
                simc.AddSetting(SimC.Renew_Party, GetSlider(SimC.Renew_Party));
                simc.AddSetting(SimC.Renew_OOC, GetSlider(SimC.Renew_OOC));
                simc.AddSetting(SimC.Rapture_HP, GetSlider(SimC.Rapture_HP));
                //simc.AddSetting(SimC.Sanctify_AHP, GetSlider(SimC.Sanctify_AHP));
                //simc.AddSetting(SimC.Circle_HP, GetSlider(SimC.Circle_HP));
                //simc.AddSetting(SimC.Halo_HP, GetSlider(SimC.Halo_HP));
               
                // Utility 
                //simc.AddSetting(SimC.Guardian_Tank_HP, GetSlider(SimC.Guardian_Tank_HP));
                //simc.AddSetting(SimC.Guardian_Party_HP, GetSlider(SimC.Guardian_Party_HP));
                //simc.AddSetting(SimC.GuardianA_Tank_HP, GetSlider(SimC.GuardianA_Tank_HP));
                //simc.AddSetting(SimC.GuardianA_Party_HP, GetSlider(SimC.GuardianA_Party_HP));
                //simc.AddSetting(SimC.Apotheosis_AHP, GetSlider(SimC.Apotheosis_AHP));
                // Mana options 
                simc.AddSetting(SimC.ManaPot, GetSlider(SimC.ManaPot));
                simc.AddSetting(SimC.SymbolofHopeP, GetSlider(SimC.SymbolofHopeP));
                simc.AddSetting(SimC.ArcaneTorrentP, GetSlider(SimC.ArcaneTorrentP));
                // DPS Options 
                simc.AddSetting(SimC.DPS_Focus_HP, GetSlider(SimC.DPS_Focus_HP));
                simc.AddSetting(SimC.CriticalHealingP, GetSlider(SimC.CriticalHealingP));
                simc.AddSetting(SimC.CriticalHealingPT, GetSlider(SimC.CriticalHealingPT));
                // Consumables
                simc.AddSetting(SimC.HealthStoneThreshold, GetSlider(SimC.HealthStoneThreshold));
                simc.AddSetting(SimC.HealthPot, GetSlider(SimC.HealthPot));
                simc.CheckTalents();
            }

           
            

            simc.update();

            //simc.ASCheckCvars();

           
            // Pause Toggle
            if(simc.pause) return true;
            if(simc.BeingSneaky) return true;

            // Quaking manager 
            if (!Aimsharp.InRaid() && simc.Quaking_Manager()) return true;
            // Explosive Logic 
            if (!Aimsharp.InRaid() && simc.TargetisExplosive()) return true;
            // never interrupt channels 
            if (simc.IsChanneling) return true;
            //DPS Cast watching to cancel if Party is unhealthy
            if (simc.DPSUnderHeal_Manager()) return true;
            // never interupt a casting
            if (simc.isCastingWaitSpells) return true;

            // Pause for Eating
            if(simc.Eating()) return true;

            // check queue
            if (simc.ASCheckQueue()) return true;

            //Dispells 
            if(!simc.nopurify && simc.Dispell_Manager()) return true;
           
            // use cooldowns
            simc.cooldowns();
            // First Casts
            //simc.CastPreFight(simc.Racials);
            simc.UseConsumables();

            // DPS Only if toggled on
            if(simc.dpsonly && !simc.healonly ) return simc.OnlyDPS();
            //if(simc.SoloDolo) return simc.Solo();
            //if(simc.HealTarget) return simc.TargetHealing();
            // Utility Rotation
            if(simc.Utility_Rotation()) return true;

            // DebugLog.Log(simc.PlayerSpec, "Spec Check ", Aimsharp.CombatTime());
            
            if(simc.SpecIsHoly)
            {
                Aimsharp.PrintMessage("Spec is Holy Error  End  ==>  ", Color.Red);
               return false;
            }
            if(simc.SpecIsDisc)
            {
               return simc.CoreHealing_Disc();
            }
            //simc.DebugRotation();
            //DebugLog.Log(Aimsharp.LastCast(), "Custom message 1", Aimsharp.CombatTime());


            //Aimsharp.PrintMessage("CombatTick End  ==>  " + simc.EstimateLOTMHeal(10), Color.Red);
            return true;
        }

        public override bool OutOfCombatTick()
        {
            if (!Authenticated) return true;

            int i = Aimsharp.CustomFunction("GetCVars");
            //Aimsharp.PrintMessage(i.ToString(), Color.Orange);

            if (InitializeSettings)
            {
               

                //Heal Settings 
                //Urgent Healing 
                // simc.AddSetting(SimC.URGENT_RAID, GetSlider(SimC.URGENT_RAID));
                //simc.AddSetting(SimC.URGENT_TANK, GetSlider(SimC.URGENT_TANK));
                //Defensives
                simc.AddSetting(SimC.DesperatePrayerP, GetSlider(SimC.DesperatePrayerP));
                simc.AddSetting(SimC.FadeP, GetSlider(SimC.FadeP));
                //Core Spells 
                simc.AddSetting(SimC.PWR_Count , GetSlider(SimC.PWR_Count));
                simc.AddSetting(SimC.PWR_HP, GetSlider(SimC.PWR_HP));
                simc.AddSetting(SimC.Renew_HP, GetSlider(SimC.Renew_HP));
                simc.AddSetting(SimC.FH_HP, GetSlider(SimC.FH_HP));
                simc.AddSetting(SimC.PowerWordLife_Tank_HP , GetSlider(SimC.PowerWordLife_Tank_HP));
                simc.AddSetting(SimC.PowerWordLife_DKTank_HP, GetSlider(SimC.PowerWordLife_DKTank_HP));
                simc.AddSetting(SimC.PowerWordLife_Party_HP, GetSlider(SimC.PowerWordLife_Party_HP));
                simc.AddSetting(SimC.Renew_Tank , GetSlider(SimC.Renew_Tank));
                simc.AddSetting(SimC.Renew_Party, GetSlider(SimC.Renew_Party));
                simc.AddSetting(SimC.Renew_OOC, GetSlider(SimC.Renew_OOC));
                simc.AddSetting(SimC.Rapture_HP, GetSlider(SimC.Rapture_HP));
                //simc.AddSetting(SimC.Sanctify_AHP, GetSlider(SimC.Sanctify_AHP));
                //simc.AddSetting(SimC.Circle_HP, GetSlider(SimC.Circle_HP));
                //simc.AddSetting(SimC.Halo_HP, GetSlider(SimC.Halo_HP));
                // Utility
                //simc.AddSetting(SimC.Guardian_Tank_HP, GetSlider(SimC.Guardian_Tank_HP));
                //simc.AddSetting(SimC.Guardian_Party_HP, GetSlider(SimC.Guardian_Party_HP));
                //simc.AddSetting(SimC.GuardianA_Tank_HP, GetSlider(SimC.GuardianA_Tank_HP));
                //simc.AddSetting(SimC.GuardianA_Party_HP, GetSlider(SimC.GuardianA_Party_HP));
                //simc.AddSetting(SimC.Apotheosis_AHP, GetSlider(SimC.Apotheosis_AHP));
                // Mana options 
                simc.AddSetting(SimC.ManaPot, GetSlider(SimC.ManaPot));
                simc.AddSetting(SimC.SymbolofHopeP, GetSlider(SimC.SymbolofHopeP));
                simc.AddSetting(SimC.ArcaneTorrentP, GetSlider(SimC.ArcaneTorrentP));
                //DPS Options 
                simc.AddSetting(SimC.DPS_Focus_HP, GetSlider(SimC.DPS_Focus_HP));
                simc.AddSetting(SimC.CriticalHealingP, GetSlider(SimC.CriticalHealingP));
                simc.AddSetting(SimC.CriticalHealingPT, GetSlider(SimC.CriticalHealingPT));
                // Consumables
                simc.AddSetting(SimC.HealthStoneThreshold, GetSlider(SimC.HealthStoneThreshold));
                simc.AddSetting(SimC.HealthPot, GetSlider(SimC.HealthPot));
                simc.CheckTalents();
                InitializeSettings = false;
            }
             

           
            
            // Update OOC
            simc.update(true);
            // Pause Toggle
            if(simc.pause) return true;
            // Sneaky Handler 
            if(simc.BeingSneaky) return true;
            
            // never interrupt channels 
            if (simc.IsChanneling) return true;
            // never interupt a casting
            if (simc.isCastingWaitSpells) return true;

            //Update units 

            if (Aimsharp.InRaid())
            {
                if(simc.PlayerAlive)
                {
                    UnitHPManager.Instance.Update();
                }
              
            }

            // never interrupt channels 
            if (simc.IsChanneling || simc.PlayerDead || simc.PlayerMounted || simc.PlayerVehicle) return true;

            // check queue
            if (simc.ASCheckQueue()) return true;

            //Dispells 
            if(!simc.nopurify && simc.Dispell_Manager()) return true;

            // Utility Rotation
            if(simc.Utility_Rotation()) return true;


            return simc.OOC();
        }

        

    }
}

