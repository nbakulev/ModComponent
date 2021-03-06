﻿using ModComponentAPI;
using System.Collections.Generic;
using UnityEngine;

namespace ModComponentMapper.ComponentMapper
{
    internal class ToolMapper
    {
        internal static void Configure(ModComponent modComponent)
        {
            ModToolComponent modToolComponent = modComponent as ModToolComponent;
            if (modToolComponent == null)
            {
                return;
            }

            ToolsItem toolsItem = ModUtils.GetOrCreateComponent<ToolsItem>(modToolComponent);

            toolsItem.m_ToolType = ModUtils.TranslateEnumValue<ToolsItem.ToolType, Usage>(modToolComponent.Usage);
            toolsItem.m_CuttingToolType = ModUtils.TranslateEnumValue<ToolsItem.CuttingToolType, ToolType>(modToolComponent.ToolType);

            toolsItem.m_CraftingAndRepairSkillModifier = modToolComponent.SkillBonus;
            toolsItem.m_CraftingAndRepairTimeModifier = modToolComponent.CraftingTimeMultiplier;
            toolsItem.m_DegradePerHourCrafting = modToolComponent.DegradePerHourCrafting;

            toolsItem.m_CanOnlyCraftAndRepairClothes = true;
            toolsItem.m_AppearInStoryOnly = false;

            ConfigureBodyHarvest(modToolComponent);
            ConfigureBreakDown(modToolComponent);
            ConfigureDegradeOnUse(modToolComponent);
            ConfigureForceLock(modToolComponent);
            ConfigureIceFishingHole(modToolComponent);
            ConfigureStruggleBonus(modToolComponent);
        }

        private static void AddAlternativeTool(ModToolComponent modToolComponent, string templateName)
        {
            GameObject original = Resources.Load(templateName) as GameObject;
            if (original == null)
            {
                return;
            }

            AlternateTools alternateTools = ModUtils.GetOrCreateComponent<AlternateTools>(original);
            List<GameObject> list = new List<GameObject>();
            if (alternateTools.m_AlternateToolsList != null)
            {
                list.AddRange(alternateTools.m_AlternateToolsList);
            }
            list.Add(modToolComponent.gameObject);
            alternateTools.m_AlternateToolsList = list.ToArray();
        }

        private static void ConfigureBodyHarvest(ModToolComponent modToolComponent)
        {
            if (!modToolComponent.CarcassHarvesting)
            {
                return;
            }

            BodyHarvestItem bodyHarvestItem = ModUtils.GetOrCreateComponent<BodyHarvestItem>(modToolComponent);
            bodyHarvestItem.m_HarvestMeatMinutesPerKG = modToolComponent.MinutesPerKgMeat;
            bodyHarvestItem.m_HarvestFrozenMeatMinutesPerKG = modToolComponent.MinutesPerKgFrozenMeat;
            bodyHarvestItem.m_HarvestGutMinutesPerUnit = modToolComponent.MinutesPerGut;
            bodyHarvestItem.m_HarvestHideMinutesPerUnit = modToolComponent.MinutesPerHide;
            bodyHarvestItem.m_HPDecreasePerHourUse = modToolComponent.DegradePerHourHarvesting;
        }

        private static void ConfigureBreakDown(ModToolComponent modToolComponent)
        {
            BreakDownItem breakDownItem = ModUtils.GetOrCreateComponent<BreakDownItem>(modToolComponent);
            breakDownItem.m_BreakDownTimeModifier = modToolComponent.BreakDownTimeMultiplier;

            string templateName = GetTemplateToolName(modToolComponent);
            if (templateName != null)
            {
                AddAlternativeTool(modToolComponent, templateName);
            }
        }

        private static void ConfigureDegradeOnUse(ModToolComponent modToolComponent)
        {
            DegradeOnUse degradeOnUse = ModUtils.GetOrCreateComponent<DegradeOnUse>(modToolComponent);
            degradeOnUse.m_DegradeHP = Mathf.Max(degradeOnUse.m_DegradeHP, modToolComponent.DegradeOnUse);
        }

        private static void ConfigureForceLock(ModToolComponent modToolComponent)
        {
            if (!modToolComponent.ForceLocks)
            {
                return;
            }

            ForceLockItem forceLockItem = ModUtils.GetOrCreateComponent<ForceLockItem>(modToolComponent);
            forceLockItem.m_ForceLockAudio = ModUtils.DefaultIfEmpty(modToolComponent.ForceLockAudio, "PLAY_LOCKERPRYOPEN1");
            forceLockItem.m_LocalizedProgressText = new LocalizedString() { m_LocalizationID = "GAMEPLAY_Forcing" };

            AddAlternativeTool(modToolComponent, "GEAR_Prybar");
        }

        private static void ConfigureIceFishingHole(ModToolComponent modToolComponent)
        {
            if (!modToolComponent.IceFishingHole)
            {
                return;
            }

            IceFishingHoleClearItem iceFishingHoleClearItem = ModUtils.GetOrCreateComponent<IceFishingHoleClearItem>(modToolComponent);
            iceFishingHoleClearItem.m_BreakIceAudio = ModUtils.DefaultIfEmpty(modToolComponent.IceFishingHoleAudio, "Play_IceBreakingChopping");
            iceFishingHoleClearItem.m_HPDecreaseToClear = modToolComponent.IceFishingHoleDegradeOnUse;
            iceFishingHoleClearItem.m_NumGameMinutesToClear = modToolComponent.IceFishingHoleMinutes;
        }

        private static void ConfigureStruggleBonus(ModToolComponent modToolComponent)
        {
            if (!modToolComponent.StruggleBonus)
            {
                return;
            }

            StruggleBonus struggleBonus = ModUtils.GetOrCreateComponent<StruggleBonus>(modToolComponent);
            struggleBonus.m_BleedoutMinutesScale = modToolComponent.BleedoutMultiplier;
            struggleBonus.m_CanPuncture = modToolComponent.CanPuncture;
            struggleBonus.m_DamageScalePercent = modToolComponent.DamageMultiplier;
            struggleBonus.m_FleeChanceScale = modToolComponent.FleeChanceMultiplier;
            struggleBonus.m_TapIncrementScale = modToolComponent.TapMultiplier;
            struggleBonus.m_StruggleWeaponType = GetStruggleWeaponType(modToolComponent);
        }

        private static StruggleBonus.StruggleWeaponType GetStruggleWeaponType(ModToolComponent modToolComponent)
        {
            switch (modToolComponent.ToolType)
            {
                case ToolType.Hatchet:
                    return StruggleBonus.StruggleWeaponType.Hatchet;

                case ToolType.Hammer:
                    return StruggleBonus.StruggleWeaponType.Hammer;

                case ToolType.Knife:
                    return StruggleBonus.StruggleWeaponType.Knife;

                default:
                    return StruggleBonus.StruggleWeaponType.BareHands;
            }
        }

        private static string GetTemplateToolName(ModToolComponent modToolComponent)
        {
            switch (modToolComponent.ToolType)
            {
                case ToolType.HackSaw:
                    return "GEAR_Hacksaw";

                case ToolType.Hatchet:
                    return "GEAR_Hatchet";

                case ToolType.Hammer:
                    return "GEAR_Hammer";

                case ToolType.Knife:
                    return "GEAR_Knife";

                default:
                    return null;
            }
        }
    }
}