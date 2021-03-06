﻿using System.Reflection;
using UnityEngine;

namespace ModComponentMapper
{
    public class Implementation
    {
        public static void OnLoad()
        {
            Debug.Log("[ModComponentMapper]: Version " + Assembly.GetExecutingAssembly().GetName().Version);

            AutoMapper.Initialize();
            ModHealthManager.Initialize();
            GearSpawner.Initialize();
            BlueprintReader.Initialize();
        }
    }
}