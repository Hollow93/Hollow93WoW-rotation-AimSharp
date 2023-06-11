using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{

    public class ShadowlandsMarksman : Rotation
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
           "Light's Judgment","Kill Shot","Double Tap","Flare","Tar Trap","Explosive Shot","Wild Spirits","Flayed Shot","Death Chakram","Volley","A Murder of Crows","Resonating Arrow","Trueshot","Aimed Shot","Rapid Fire","Chimaera Shot","Arcane Shot","Serpent Sting","Barrage","Steady Shot","Multi-Shot",
        };

        List<string> BuffsList = new List<string>
        {
            "Trueshot","Steady Focus","Precise Shots","Resonating Arrow","Wild Spirits","Volley","Trick Shots","Double Tap","Dead Eye","Lock and Load"
        };

        List<string> DebuffsList = new List<string>
        {
           "Serpent Sting","Tar Trap"
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

            Settings.Add(new Setting("Hunter Settings"));
            Settings.Add(new Setting("Legendary power equipped:", new List<string>() { "None", "Sephuz's Proclamation", "Soulforge Embers", "Eagletalon's True Focus", "Surging Shots", }, "None"));
            // Settings.Add(new Setting("Glaive Tempest desired targets:", 1, 5, 1));
        }


        public override void Initialize()
        {
            //Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Shadowlands Marksman", Color.Purple);
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

            Macros.Add("FlareC", "/cast [@cursor] Flare");
            Macros.Add("TarTrapC", "/cast [@cursor] Tar Trap");
            Macros.Add("WildSpiritsC", "/cast [@cursor] Wild Spirits");
            Macros.Add("VolleyC", "/cast [@cursor] Volley");
            Macros.Add("ResonatingArrowC", "/cast [@cursor] Resonating Arrow");


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
            bool Fighting = Aimsharp.Range("target") <= 50 && Aimsharp.TargetIsEnemy();
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


            //power
            int Focus = Aimsharp.Power("player");
            int FocusMax = Aimsharp.PlayerMaxPower();
            int FocusDefecit = FocusMax - Focus;

            //Talents
            bool TalentSteadyFocus = Aimsharp.Talent(4, 1);
            bool TalentChimaeraShot = Aimsharp.Talent(4, 3);
            bool TalentStreamline = Aimsharp.Talent(4, 2);


            bool CastingSteadyShot = PlayerCastingID == 56641;
            //buffs
            int BuffTrueshotRemains = Aimsharp.BuffRemaining("Trueshot") - GCD;
            bool BuffTrueshotUp = BuffTrueshotRemains > 0;
            int BuffSteadyFocusRemains = Aimsharp.BuffRemaining("Steady Focus") - GCD;
            if (LastCast == "Steady Shot" && CastingSteadyShot && TalentSteadyFocus)
            {
                BuffSteadyFocusRemains = 15000;
            }
            bool BuffSteadyFocusUp = BuffSteadyFocusRemains > 0;
            int BuffPreciseShotsRemains = Aimsharp.BuffRemaining("Precise Shots") - GCD;
            bool BuffPreciseShotsUp = BuffPreciseShotsRemains > 0;
            int BuffResonatingArrowRemains = Aimsharp.BuffRemaining("Resonating Arrow") - GCD;
            bool BuffResonatingArrowUp = BuffResonatingArrowRemains > 0;
            int BuffWildSpiritsRemains = Aimsharp.BuffRemaining("Wild Spirits") - GCD;
            bool BuffWildSpiritsUp = BuffWildSpiritsRemains > 0;
            int BuffVolleyRemains = Aimsharp.BuffRemaining("Volley") - GCD;
            bool BuffVolleyUp = BuffVolleyRemains > 0;
            int BuffTrickShotsRemains = Aimsharp.BuffRemaining("Trick Shots") - GCD;
            bool BuffTrickShotsUp = BuffTrickShotsRemains > 0;
            int BuffDoubleTapRemains = Aimsharp.BuffRemaining("Double Tap") - GCD;
            bool BuffDoubleTapUp = BuffDoubleTapRemains > 0;
            int BuffDeadEyeRemains = Aimsharp.BuffRemaining("Dead Eye") - GCD;
            bool BuffDeadEyeUp = BuffDeadEyeRemains > 0;
            int BuffLockandLoadRemains = Aimsharp.BuffRemaining("Lock and Load") - GCD;
            bool BuffLockandLoadUp = BuffLockandLoadRemains > 0;


            //debuffs
            int DotSerpentStingRemains = Aimsharp.DebuffRemaining("Serpent Sting") - GCD;
            bool DotSerpentStingUp = DotSerpentStingRemains > 0;
            bool DotSerpentStingRefreshable = DotSerpentStingRemains < 5400;
            int DebuffTarTrapRemains = Aimsharp.DebuffRemaining("Tar Trap", "target", false) - GCD;
            bool DebuffTarTrapUp = DebuffTarTrapRemains > 0;


            //cooldowns
            int CDFlareRemains = Aimsharp.SpellCooldown("Flare");
            bool CDFlareUp = CDFlareRemains <= 0;
            int CDResonatingArrowRemains = SaveCooldowns ? 600000 : Aimsharp.SpellCooldown("Resonating Arrow");
            bool CDResonatingArrowUp = CDResonatingArrowRemains <= 0;
            int CDAimedShotRemains = Aimsharp.SpellCooldown("Aimed Shot");
            bool CDAimedShotUp = CDAimedShotRemains <= 0;
            int CDAimedShotCharges = Aimsharp.SpellCharges("Aimed Shot");
            int CDAimedShotFullRecharge = (int)(Aimsharp.RechargeTime("Aimed Shot") + 12000f * (Aimsharp.MaxCharges("Aimed Shot") - CDAimedShotCharges - 1) / (1f + Haste));
            float CDAimedShotFractional = CDAimedShotCharges + (1 - (Aimsharp.RechargeTime("Aimed Shot") - GCD) / (12000f / (1f + Haste)));
            CDAimedShotFractional = CDAimedShotFractional > Aimsharp.MaxCharges("Aimed Shot") ? Aimsharp.MaxCharges("Aimed Shot") : CDAimedShotFractional;
            int CDRapidFireRemains = Aimsharp.SpellCooldown("Rapid Fire");
            bool CDRapidFireUp = CDRapidFireRemains <= 0;


            //specific variables
            bool RuneforgeSephuzsProclamation = RuneforgePower == "Sephuz's Proclamation";
            bool RuneforgeSoulforgeEmbers = RuneforgePower == "Soulforge Embers";
            bool RuneforgeEagletalonsTrueFocus = RuneforgePower == "Eagletalon's True Focus";
            bool RuneforgeSurgingShots = RuneforgePower == "Surging Shots";
            bool ConduitNiyasToolsPoison = ActiveConduits.Contains(320660);
            bool ConduitReversalofFortune = ActiveConduits.Contains(339495);


            //bool WeaponFallenCrusader = Aimsharp.CustomFunction("RuneforgeFallenCrusader") == 1;
            //bool WeaponRazorice = Aimsharp.CustomFunction("RuneforgeRazorice") == 1;
            // int ChaoticTransformationRank = Aimsharp.CustomFunction("Chaotic Transformation Rank");
            // int RevolvingBladesRank = Aimsharp.CustomFunction("Revolving Blades Rank");
            // int desired_targets = GetSlider("Glaive Tempest desired targets:");


            //CaNCasts
            bool CanCastLightsJudgment = Aimsharp.CanCast("Light's Judgment", "player") && !SaveCooldowns && Fighting;
            bool CanCastKillShot = Aimsharp.CanCast("Kill Shot") && Fighting;
            bool CanCastDoubleTap = Aimsharp.CanCast("Double Tap", "player") && Fighting;
            bool CanCastFlare = Aimsharp.CanCast("Flare", "player") && Fighting;
            bool CanCastTarTrap = Aimsharp.CanCast("Tar Trap", "player") && Fighting;
            bool CanCastExplosiveShot = Aimsharp.CanCast("Explosive Shot") && Fighting;
            bool CanCastWildSpirits = Aimsharp.CanCast("Wild Spirits", "player") && !SaveCooldowns && Fighting;
            bool CanCastFlayedShot = Aimsharp.CanCast("Flayed Shot") && Fighting;
            bool CanCastDeathChakram = Aimsharp.CanCast("Death Chakram") && Fighting;
            bool CanCastVolley = Aimsharp.CanCast("Volley", "player") && Fighting && AOE;
            bool CanCastAMurderofCrows = Aimsharp.CanCast("A Murder of Crows") && Fighting;
            bool CanCastResonatingArrow = Aimsharp.CanCast("Resonating Arrow", "player") && !SaveCooldowns && Fighting;
            bool CanCastTrueshot = Aimsharp.CanCast("Trueshot") && !SaveCooldowns && Fighting;
            bool CanCastAimedShot = Aimsharp.CanCast("Aimed Shot") && Fighting && (!Moving || BuffLockandLoadUp);
            bool CanCastRapidFire = Aimsharp.CanCast("Rapid Fire") && Fighting;
            bool CanCastChimaeraShot = Aimsharp.CanCast("Chimaera Shot") && Fighting;
            bool CanCastArcaneShot = Aimsharp.CanCast("Arcane Shot") && Fighting;
            bool CanCastSerpentSting = Aimsharp.CanCast("Serpent Sting") && Fighting;
            bool CanCastBarrage = Aimsharp.CanCast("Barrage", "player") && Fighting;
            bool CanCastSteadyShot = Aimsharp.CanCast("Steady Shot") && Fighting;
            bool CanCastMultiShot = Aimsharp.CanCast("Multi-Shot") && Fighting;

            int AimedShotCastTime = (int)(BuffLockandLoadUp ? 0 : 2500 / (1 + Haste));
            int RapidFireExecuteTime = (int)(2000 / (1 + Haste) + GCD);
            




            // end of Simc conditionals
            #endregion

            //never interrupt channels 
            if (IsChanneling)
                return false;

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

            //actions+=/call_action_list,name=cds
            //actions.cds=berserking,if=buff.trueshot.up|target.time_to_die<13
            if (BuffTrueshotUp||TargetTimeToDie<13000)
            {
                foreach (string Racial in Racials)
                {
                    if (Aimsharp.CanCast(Racial, "player") && Fighting && !SaveCooldowns)
                    {
                        Aimsharp.Cast(Racial, true);
                        return true;
                    }
                }
            }

            //actions.cds+=/lights_judgment,if=buff.trueshot.down
            if (CanCastLightsJudgment)
                if (!BuffTrueshotUp)
                    return LightsJudgment();

            //actions.cds+=/potion,if=buff.trueshot.up&buff.bloodlust.up|buff.trueshot.up&target.health.pct<20|target.time_to_die<26
            if (BuffTrueshotUp&&BloodlustUp||BuffTrueshotUp&&TargetHealthPct<20||TargetTimeToDie<26000)
            {
                if (UsePotion && Aimsharp.CanUseItem(PotionName, false) && !SaveCooldowns)
                {
                    Aimsharp.Cast("DPS Pot", true);
                    return true;
                }
            }

            //actions+=/call_action_list,name=st,if=active_enemies<3
            if (EnemiesNearTarget<3)
            {
                //actions.st=steady_shot,if=talent.steady_focus&(prev_gcd.1.steady_shot&buff.steady_focus.remains<5|buff.steady_focus.down)
                if (CanCastSteadyShot)
                    if (TalentSteadyFocus && (LastCast == "Steady Shot" && BuffSteadyFocusRemains < 5000 || !BuffSteadyFocusUp))
                        return SteadyShot();

                //actions.st+=/kill_shot
                if (CanCastKillShot)
                    return KillShot();

                //actions.st+=/double_tap,if=covenant.kyrian&cooldown.resonating_arrow.remains<gcd|!covenant.kyrian&(cooldown.aimed_shot.up|cooldown.rapid_fire.remains>cooldown.aimed_shot.remains)
                if (CanCastDoubleTap)
                    if (CovenantKyrian && CDResonatingArrowRemains < GCDMAX || !CovenantKyrian && (CDAimedShotUp || CDRapidFireRemains > CDAimedShotRemains))
                        return DoubleTap();

                //actions.st+=/flare,if=tar_trap.up&runeforge.soulforge_embers
                if (CanCastFlare)
                    if (DebuffTarTrapUp && RuneforgeSoulforgeEmbers)
                        return Flare();

                //actions.st+=/tar_trap,if=runeforge.soulforge_embers&tar_trap.remains<gcd&cooldown.flare.remains<gcd
                if (CanCastTarTrap)
                    if (RuneforgeSoulforgeEmbers && DebuffTarTrapRemains < GCDMAX && CDFlareRemains < GCDMAX)
                        return TarTrap();

                //actions.st+=/explosive_shot
                if (CanCastExplosiveShot)
                    return ExplosiveShot();

                //actions.st+=/wild_spirits
                if (CanCastWildSpirits)
                    return WildSpirits();

                //actions.st+=/flayed_shot
                if (CanCastFlayedShot)
                    return FlayedShot();

                //actions.st+=/death_chakram,if=focus+cast_regen<focus.max
                if (CanCastDeathChakram)
                    if (Focus + 3 < FocusMax)
                        return DeathChakram();

                //actions.st+=/volley,if=buff.precise_shots.down|!talent.chimaera_shot|active_enemies<2
                if (CanCastVolley)
                    if (!BuffPreciseShotsUp || !TalentChimaeraShot || EnemiesNearTarget < 2)
                        return Volley();

                //actions.st+=/a_murder_of_crows
                if (CanCastAMurderofCrows)
                    return AMurderofCrows();

                //actions.st+=/resonating_arrow
                if (CanCastResonatingArrow)
                    return ResonatingArrow();

                //actions.st+=/trueshot,if=buff.precise_shots.down|buff.resonating_arrow.up|buff.wild_spirits.up|buff.volley.up&active_enemies>1
                if (CanCastTrueshot)
                    if (!BuffPreciseShotsUp || BuffResonatingArrowUp || BuffWildSpiritsUp || BuffVolleyUp && EnemiesNearTarget > 1)
                        return Trueshot();

                //actions.st+=/aimed_shot,target_if=min:(dot.serpent_sting.remains<?action.serpent_sting.in_flight_to_target*dot.serpent_sting.duration),if=buff.precise_shots.down|(buff.trueshot.up|full_recharge_time<gcd+cast_time)&(!talent.chimaera_shot|active_enemies<2)|buff.trick_shots.remains>execute_time&active_enemies>1
                if (CanCastAimedShot)
                    if (!BuffPreciseShotsUp || (BuffTrueshotUp || CDAimedShotFullRecharge < GCDMAX + AimedShotCastTime) && (!TalentChimaeraShot || EnemiesNearTarget < 2) || BuffTrickShotsRemains > AimedShotCastTime + GCD && EnemiesNearTarget > 1)
                        return AimedShot();

                //actions.st+=/rapid_fire,if=focus+cast_regen<focus.max&(buff.trueshot.down|!runeforge.eagletalons_true_focus)&(buff.double_tap.down|talent.streamline)
                if (CanCastRapidFire)
                    if (Focus + 20 < FocusMax && (!BuffTrueshotUp || !RuneforgeEagletalonsTrueFocus) && (!BuffDoubleTapUp || TalentStreamline))
                        return RapidFire();

                //actions.st+=/chimaera_shot,if=buff.precise_shots.up|focus>cost+action.aimed_shot.cost
                if (CanCastChimaeraShot)
                    if (BuffPreciseShotsUp || Focus > 20 + (BuffLockandLoadUp ? 0 : 35))
                        return ChimaeraShot();

                //actions.st+=/arcane_shot,if=buff.precise_shots.up|focus>cost+action.aimed_shot.cost
                if (CanCastArcaneShot)
                    if (BuffPreciseShotsUp || Focus > 20 + (BuffLockandLoadUp ? 0 : 35))
                        return ArcaneShot();

                //actions.st+=/serpent_sting,target_if=min:remains,if=refreshable&target.time_to_die>duration
                if (CanCastSerpentSting)
                    if (DotSerpentStingRefreshable && TargetTimeToDie > 18000)
                        return SerpentSting();

                //actions.st+=/barrage,if=active_enemies>1
                if (CanCastBarrage)
                    if (EnemiesNearTarget > 1)
                        return Barrage();

                //actions.st+=/rapid_fire,if=focus+cast_regen<focus.max&(buff.double_tap.down|talent.streamline)
                if (CanCastRapidFire)
                    if (Focus + 20 < FocusMax && (!BuffDoubleTapUp || TalentStreamline))
                        return RapidFire();

                //actions.st+=/steady_shot
                if (CanCastSteadyShot)
                    return SteadyShot();
            }

            //actions+=/call_action_list,name=trickshots,if=active_enemies>2
            if (EnemiesNearTarget>2)
            {
                //actions.trickshots=steady_shot,if=talent.steady_focus&in_flight&buff.steady_focus.remains<5
                if (CanCastSteadyShot)
                    if (TalentSteadyFocus && LastCast == "Steady Shot" && BuffSteadyFocusRemains < 5000)
                        return SteadyShot();

                //actions.trickshots+=/double_tap,if=covenant.kyrian&cooldown.resonating_arrow.remains<gcd|cooldown.rapid_fire.remains<cooldown.aimed_shot.full_recharge_time|!(talent.streamline&runeforge.surging_shots)|!covenant.kyrian
                if (CanCastDoubleTap)
                    if (CovenantKyrian && CDResonatingArrowRemains < GCDMAX || CDRapidFireRemains < CDAimedShotFullRecharge || !(TalentStreamline && RuneforgeSurgingShots) || !CovenantKyrian)
                        return DoubleTap();

                //actions.st+=/flare,if=tar_trap.up&runeforge.soulforge_embers
                if (CanCastFlare)
                    if (DebuffTarTrapUp && RuneforgeSoulforgeEmbers)
                        return Flare();

                //actions.st+=/tar_trap,if=runeforge.soulforge_embers&tar_trap.remains<gcd&cooldown.flare.remains<gcd
                if (CanCastTarTrap)
                    if (RuneforgeSoulforgeEmbers && DebuffTarTrapRemains < GCDMAX && CDFlareRemains < GCDMAX)
                        return TarTrap();

                //actions.st+=/explosive_shot
                if (CanCastExplosiveShot)
                    return ExplosiveShot();

                //actions.st+=/wild_spirits
                if (CanCastWildSpirits)
                    return WildSpirits();

                //actions.trickshots+=/resonating_arrow
                if (CanCastResonatingArrow)
                    return ResonatingArrow();

                //actions.trickshots+=/volley
                if (CanCastVolley)
                    return Volley();

                //actions.trickshots+=/barrage
                if (CanCastBarrage)
                    return Barrage();

                //actions.trickshots+=/trueshot
                if (CanCastTrueshot)
                    return Trueshot();

                //actions.trickshots+=/rapid_fire,if=buff.trick_shots.remains>=execute_time&runeforge.surging_shots&buff.double_tap.down
                if (CanCastRapidFire)
                    if (BuffTrickShotsRemains>=RapidFireExecuteTime&&RuneforgeSurgingShots&&!BuffDoubleTapUp)
                        return RapidFire();

                //actions.trickshots+=/aimed_shot,target_if=min:(dot.serpent_sting.remains<?action.serpent_sting.in_flight_to_target*dot.serpent_sting.duration),if=buff.trick_shots.remains>=execute_time&(buff.precise_shots.down|full_recharge_time<cast_time+gcd|buff.trueshot.up)
                if (CanCastAimedShot)
                    if ((BuffTrickShotsRemains >= (EnemiesNearTarget>3?AimedShotCastTime + GCD:0)) && (!BuffPreciseShotsUp || CDAimedShotFullRecharge < AimedShotCastTime + GCDMAX || BuffTrueshotUp))
                        return AimedShot();

                //actions.trickshots+=/death_chakram,if=focus+cast_regen<focus.max
                if (CanCastDeathChakram)
                    if (Focus + 21 < FocusMax)
                        return DeathChakram();

                //actions.trickshots+=/rapid_fire,if=buff.trick_shots.remains>=execute_time
                if (CanCastRapidFire)
                    if (BuffTrickShotsRemains >= RapidFireExecuteTime)
                        return RapidFire();

                //actions.trickshots+=/multishot,if=buff.trick_shots.down|buff.precise_shots.up&focus>cost+action.aimed_shot.cost&(!talent.chimaera_shot|active_enemies>3)
                if (CanCastMultiShot)
                    if ((!BuffTrickShotsUp && EnemiesNearTarget > 3) || BuffPreciseShotsUp && Focus > 15 + (BuffLockandLoadUp ? 0 : 35) && (!TalentChimaeraShot || EnemiesNearTarget > 3))
                        return MultiShot();

                //actions.trickshots+=/chimaera_shot,if=buff.precise_shots.up&focus>cost+action.aimed_shot.cost&active_enemies<4
                if (CanCastChimaeraShot)
                    if (BuffPreciseShotsUp && Focus > 20 + (BuffLockandLoadUp ? 0 : 35) && EnemiesNearTarget < 4)
                        return ChimaeraShot();

                //actions.trickshots+=/kill_shot,if=buff.dead_eye.down
                if (CanCastKillShot)
                    if (!BuffDeadEyeUp)
                        return KillShot();

                //actions.trickshots+=/a_murder_of_crows
                if (CanCastAMurderofCrows)
                    return AMurderofCrows();

                //actions.trickshots+=/flayed_shot
                if (CanCastFlayedShot)
                    return FlayedShot();

                //actions.trickshots+=/serpent_sting,target_if=min:dot.serpent_sting.remains,if=refreshable
                if (CanCastSerpentSting)
                    if (DotSerpentStingRefreshable)
                        return SerpentSting();

                //actions.trickshots+=/multishot,if=focus>cost+action.aimed_shot.cost
                if (CanCastMultiShot)
                    if (Focus > 15 + (BuffLockandLoadUp ? 0 : 35))
                        return MultiShot();

                //actions.trickshots+=/steady_shot
                if (CanCastSteadyShot)
                    return SteadyShot();

            }



            return false;
        }


        public override bool OutOfCombatTick()
        {


            return false;
        }

        bool LightsJudgment() { Aimsharp.Cast("Light's Judgment"); return true; }
        bool KillShot() { Aimsharp.Cast("Kill Shot"); return true; }
        bool DoubleTap() { Aimsharp.Cast("Double Tap"); return true; }
        bool Flare() { Aimsharp.Cast("FlareC"); return true; }
        bool TarTrap() { Aimsharp.Cast("TarTrapC"); return true; }
        bool ExplosiveShot() { Aimsharp.Cast("Explosive Shot"); return true; }
        bool WildSpirits() { Aimsharp.Cast("WildSpiritsC"); return true; }
        bool FlayedShot() { Aimsharp.Cast("Flayed Shot"); return true; }
        bool DeathChakram() { Aimsharp.Cast("Death Chakram"); return true; }
        bool Volley() { Aimsharp.Cast("VolleyC"); return true; }
        bool AMurderofCrows() { Aimsharp.Cast("A Murder of Crows"); return true; }
        bool ResonatingArrow() { Aimsharp.Cast("ResonatingArrowC"); return true; }
        bool Trueshot() { Aimsharp.Cast("Trueshot"); return true; }
        bool AimedShot() { Aimsharp.Cast("Aimed Shot"); return true; }
        bool RapidFire() { Aimsharp.Cast("Rapid Fire"); return true; }
        bool ChimaeraShot() { Aimsharp.Cast("Chimaera Shot"); return true; }
        bool ArcaneShot() { Aimsharp.Cast("Arcane Shot"); return true; }
        bool SerpentSting() { Aimsharp.Cast("Serpent Sting"); return true; }
        bool Barrage() { Aimsharp.Cast("Barrage"); return true; }
        bool SteadyShot() { Aimsharp.Cast("Steady Shot"); return true; }
        bool MultiShot() { Aimsharp.Cast("Multi-Shot"); return true; }


    }
}
