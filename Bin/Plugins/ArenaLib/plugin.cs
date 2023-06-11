using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AimsharpWow.API; //needed to access Aimsharp API

namespace AimsharpWow.Modules
{
    public class UnitEntity
    {      
        public string Unit = "";
        public string Spec = "";
        public bool IsHealer = false;
        public bool IsEnemy = false;
        public int Range = 0;
        public int ImmuneRemaining = 0;
        public int PhysicalImmuneRemaining = 0;
        public int SpellImmuneRemaining = 0;
        public bool Immune = false;
        public bool PhysicalImmune = false;
        public bool SpellImmune = false;

        public int HP = 0;
        public bool IsInterruptable = false;
        public bool IsChanneling = false;
        public int CastingID = 0;
        public int CastingRemaining = 0;
        public int CastingElapsed = 0;

        public int DRDisorient = 0;
        public int DRIncapacitate = 0;
        public int DRStun = 0;

        public int CC_BreakableRemaining = 0;
        public int CC_NotBreakableRemaining = 0;
        public int BigBuffRemaining = 0;

        public UnitEntity(string unit)
        {
            Unit = unit;
        }

        public void Update()
        {
            HP = Aimsharp.Health(Unit);
            Spec = Aimsharp.GetSpec(Unit);
            IsEnemy = Unit == "arena1" || Unit == "arena2" || Unit == "arena3" || Unit == "target" && Aimsharp.TargetIsEnemy();
            Range = Aimsharp.Range(Unit);
            IsInterruptable = Aimsharp.IsInterruptable(Unit);
            IsChanneling = Aimsharp.IsChanneling(Unit);
            CastingID = Aimsharp.CastingID(Unit);
            CastingRemaining = Aimsharp.CastingRemaining(Unit);
            CastingElapsed = Aimsharp.CastingElapsed(Unit);
            DRDisorient = Aimsharp.EnemyDR(Unit, "Disorients");
            DRIncapacitate = Aimsharp.EnemyDR(Unit, "Incapacitates");
            DRStun = Aimsharp.EnemyDR(Unit, "Stuns");
        }
    }



    public class ArenaLib : Plugin
    {

        string[] immunes = { "Divine Shield", "Aspect of the Turtle", "Ice Block" };
        string[] physical_immunes = { "Blessing of Protection", "Ethereal Form" };
        string[] spell_immunes = { "Nether Ward", "Grounding Totem Effect", "Mass Spell Reflection", "Anti-Magic Shell", "Spell Reflection", "Cloak of Shadows" };
        string[] cc_notbreaks = { "Warstomp", "Storm Bolt", "Shockwave", "Hammer of Justice", "Intimidation", "Cheap Shot", "Kidney Shot", "Psychic Horror", "Holy Word: Chastise", "Asphyxiate", "Gnaw", "Static Charge", "Capacitor Totem", "Shadowfury", "Infernal Awakening", "Axe Toss", "Leg Sweep", "Mortal Coil", "Sin and Punishment", "Mighty Bash", "Fel Eruption", "Chaos Nova", "Lightning Lasso", "Zombie Explosion", "Maim" };
        string[] cc_silences = { "Silencing Shot", "Silence" };
        string[] cc_breaks = { "Intimidating Shout", "Repentance", "Blinding Light", "Freezing Shot", "Scatter Shot", "Gouge", "Sap", "Blind", "Psychic Scream", "Mind Bomb", "Hex", "Sundering", "Quaking Palm", "Polymorph", "Ring of Frost", "Dragon's Breath", "Fear", "Paralysis", "Imprison", "Seduction", "Howl of Terror", "Mind Bomb" };
        string[] cc_special = { "Cyclone" };

        string[] big_buffs = {
            "Heroism", "Bloodlust", //lust
            "Gladiator's Badge", "Gladiator's Medallion", //trinkets
            "Dark Soul: Instability","Dark Soul: Misery", //warlocks
            "Combustion", "Arcane Power", "Icy Veins", //mages
            "Avatar",  //warrior
            "Metamorphosis", "Rain from Above",//dh
            "Avenging Wrath", //pally
            "Coordinated Assault", "Trueshot", "Aspect of the Wild", //hunter
            "Pillar of Frost", "Unholy Assault", //dk
            "Celestial Alignment", "Incarnation: King of the Jungle", "Berserk", //druids
            "Voidform", "Power Infusion", //priest
            "Shadow Blades","Shadow Dance", "Adrenaline Rush", //rogue
            "Serenity", "Storm, Earth, and Fire", //monk
            "Stormkeeper", "Ascendance", //shaman
        };

        string[] big_debuffs = { "Touch of Death", "Vendetta", "Mindgames" };
        string[] fear_immunes = { "Lichborne", "Berserker Rage", "Bestial Wrath" };
        string[] stun_immunes = { "Icebound Fortitude", "Lichborne" };

        List<string> Healers = new List<string>() { "Priest: Holy", "Priest: Discipline", "Paladin: Holy", "Druid: Restoration", "Monk: Mistweaver", "Shaman: Restoration", "HEALER" };

        public override void LoadSettings()
        {

        }

        UnitEntity Target = new UnitEntity("target");
        UnitEntity Player = new UnitEntity("player");
        UnitEntity Party1 = new UnitEntity("party1");
        UnitEntity Party2 = new UnitEntity("party2");
        UnitEntity Party3 = new UnitEntity("party3");
        UnitEntity Party4 = new UnitEntity("party4");
        UnitEntity Arena1 = new UnitEntity("arena1");
        UnitEntity Arena2 = new UnitEntity("arena2");
        UnitEntity Arena3 = new UnitEntity("arena3");
        UnitEntity Focus = new UnitEntity("focus");
        List<UnitEntity> Units = new List<UnitEntity>();

        public override void Initialize()
        {
            Units.Add(Target);
            Units.Add(Player);
            Units.Add(Party1);
            Units.Add(Party2);
            Units.Add(Party3);
            Units.Add(Party4);
            Units.Add(Arena1);
            Units.Add(Arena2);
            Units.Add(Arena3);
            Units.Add(Focus);

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

            foreach (string cc in cc_breaks)
            {
                Debuffs.Add(cc);
            }

            foreach (string cc in cc_notbreaks)
            {
                Debuffs.Add(cc);
            }

            foreach (string cc in cc_silences)
            {
                Debuffs.Add(cc);
            }

            foreach (string cc in cc_special)
            {
                Debuffs.Add(cc);
            }

            foreach (string big_buff in big_buffs)
            {
                Buffs.Add(big_buff);
            }

            foreach (string big_debuff in big_debuffs)
            {
                Debuffs.Add(big_debuff);
            }

            foreach (string fear_immune in fear_immunes)
            {
                Buffs.Add(fear_immune);
            }

            foreach (string stun_immune in stun_immunes)
            {
                Buffs.Add(stun_immune);
            }


        }



        public override bool CombatTick()
        {
            foreach (UnitEntity t in Units)
            {
                t.Update();

                if (Healers.Contains(t.Spec))
                    t.IsHealer = true;

                int ImmuneRemaining = 0;
                foreach (string immune in immunes)
                {
                    int remains = Aimsharp.BuffRemaining(immune, t.Unit, false);
                    if (remains > ImmuneRemaining)
                    {
                        ImmuneRemaining = remains;
                    }
                }
                foreach (string cc in cc_special)
                {
                    int remains = Aimsharp.DebuffRemaining(cc, t.Unit, false);
                    if (remains > ImmuneRemaining)
                    {
                        ImmuneRemaining = remains;
                    }
                }

                t.ImmuneRemaining = ImmuneRemaining;

                if (ImmuneRemaining > 0)
                    t.Immune = true;

                int PhysicalImmuneRemaining = 0;
                foreach (string PhysicalImmune in physical_immunes)
                {
                    int remains = Aimsharp.BuffRemaining(PhysicalImmune, t.Unit, false);
                    if (remains > PhysicalImmuneRemaining)
                        PhysicalImmuneRemaining = remains;
                }

                t.PhysicalImmuneRemaining = PhysicalImmuneRemaining;

                if (PhysicalImmuneRemaining > 0)
                    t.PhysicalImmune = true;

                int SpellImmuneRemaining = 0;
                foreach (string SpellImmune in spell_immunes)
                {
                    int remains = Aimsharp.BuffRemaining(SpellImmune, t.Unit, false);
                    if (remains > SpellImmuneRemaining)
                    {
                        SpellImmuneRemaining = remains;
                    }
                    if (Aimsharp.HasBuff("Grounding Totem Effect", t.Unit, false))
                    {
                        SpellImmuneRemaining = 3000;
                    }
                }

                t.SpellImmuneRemaining = SpellImmuneRemaining;

                if (SpellImmuneRemaining > 0)
                    t.SpellImmune = true;

                int BigBuffRemaining = 0;
                foreach (string buff in big_buffs)
                {
                    int remains = Aimsharp.BuffRemaining(buff, t.Unit, false);
                    if (remains > BigBuffRemaining)
                        BigBuffRemaining = remains;
                }

                t.BigBuffRemaining = BigBuffRemaining;

                int CC_BreakableRemaining = 0;
                foreach (string cc in cc_breaks)
                {
                    int remains = Aimsharp.DebuffRemaining(cc, t.Unit, false);
                    if (remains > CC_BreakableRemaining)
                        CC_BreakableRemaining = remains;
                }

                t.CC_BreakableRemaining = CC_BreakableRemaining;

                int CC_NotBreakableRemaining = 0;
                foreach (string cc in cc_notbreaks)
                {
                    int remains = Aimsharp.DebuffRemaining(cc, t.Unit, false);
                    if (remains > CC_NotBreakableRemaining)
                        CC_NotBreakableRemaining = remains;
                }

                t.CC_NotBreakableRemaining = CC_NotBreakableRemaining;



                ExportObject(t.Unit + "Spec", t.Spec);
                ExportObject(t.Unit + "IsHealer", t.IsHealer);
                ExportObject(t.Unit + "IsEnemy", t.IsEnemy);
                ExportObject(t.Unit + "Range", t.Range);
                ExportObject(t.Unit + "ImmuneRemaining", t.ImmuneRemaining);
                ExportObject(t.Unit + "PhysicalImmuneRemaining", t.PhysicalImmuneRemaining);
                ExportObject(t.Unit + "SpellImmuneRemaining", t.SpellImmuneRemaining);
                ExportObject(t.Unit + "Immune", t.Immune);
                ExportObject(t.Unit + "PhysicalImmune", t.PhysicalImmune);
                ExportObject(t.Unit + "SpellImmune", t.SpellImmune);

                ExportObject(t.Unit + "HP", t.HP);
                ExportObject(t.Unit + "IsInterruptable", t.IsInterruptable);
                ExportObject(t.Unit + "IsChanneling", t.IsChanneling);
                ExportObject(t.Unit + "CastingID", t.CastingID);
                ExportObject(t.Unit + "CastingRemaining", t.CastingRemaining);
                ExportObject(t.Unit + "CastingElapsed", t.CastingElapsed);

                ExportObject(t.Unit + "DRDisorient", t.DRDisorient);
                ExportObject(t.Unit + "DRIncapacitate", t.DRIncapacitate);
                ExportObject(t.Unit + "DRStun", t.DRStun);

                ExportObject(t.Unit + "CC_BreakableRemaining", t.CC_BreakableRemaining);
                ExportObject(t.Unit + "CC_NotBreakableRemaining", t.CC_NotBreakableRemaining);
                ExportObject(t.Unit + "BigBuffRemaining", t.BigBuffRemaining);

                if (t.DRStun > 0)
                    Aimsharp.PrintMessage(t.Unit + " is DR stun: " + t.DRStun.ToString());
            }

            return false;
        }

        public override bool OutOfCombatTick()
        {
            return CombatTick();
        }

        public override bool MountedTick()
        {
            return CombatTick();
        }

    }
}