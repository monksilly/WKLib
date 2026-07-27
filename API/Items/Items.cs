using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WKLib.API.Items
{
    public static class Items
    {
        public static void AddItemIntoBag(Item item) {
            Traverse.Create(ENT_Player.GetInventory()).Method("LoadItemsIntoBag", new List<Item> { item }).GetValue();
        }
        public static void AddItemIntoBag(string itemId)
        {
            Item item = GetItemByItemId(itemId);
            AddItemIntoBag(item);
        }

        public static void AddItemsIntoBag(IEnumerable<Item> items) {
            foreach (Item item in items)
            {
                AddItemIntoBag(item);
            }
        }
        public static void AddItemsIntoBag(IEnumerable<string> itemIds)
        {
            foreach (string itemId in itemIds)
            {
                AddItemIntoBag(itemId);
            }
        }

        public static Item GetItemByItemId(string itemId)
        {
            GameObject assetGameObject = CL_AssetManager.GetAssetGameObject(itemId, "");
            Item clone = assetGameObject.GetComponent<Item_Object>().itemData.GetClone(null, false);
            clone.GetDropObject(true);
            clone.bagPosition = new Vector3(0f, 0f, 1f) + UnityEngine.Random.insideUnitSphere * 0.01f;
            clone.bagRotation = Quaternion.LookRotation(clone.upDirection);
            return clone;
        }

        public static Item_Object _LoadItemObject(string itemId) {
            return CL_AssetManager.GetItemObjectPrefab(itemId, "");
         }

        public static class ItemIds
        {
            //public const string DenizenRat = "Denizen_Rat";
            public const string DenizenRoachGold = "Denizen_Roach_Gold";
            //public const string DenizenRoachGoldNavmesh = "Denizen_Roach_Gold_Navmesh";
            public const string DenizenRoachLemon = "Denizen_Roach_Lemon";
            public const string DenizenRoachPlatinum = "Denizen_Roach_Platinum";
            //public const string DenizenRoachPlatinumNavmesh = "Denizen_Roach_Platinum_Navmesh";
            //public const string DenizenSpider = "Denizen_Spider";
            public const string DenizenRoachFlyingRuby = "Denizen_Roach_Flying_Ruby";
            public const string ItemArtifactEVAGlove = "Item_Artifact_EVAGlove";
            public const string ItemArtifactRapier = "Item_Artifact_Rapier";
            public const string ItemArtifactRemote = "Item_Artifact_Remote";
            public const string ItemArtifactRebarReturn = "Item_Artifact_Rebar_Return";
            public const string WorldItemScissors = "World_Item_Scissors";
            public const string ItemArtifactTranslocator = "Item_Artifact_Translocator";
            public const string ItemArtifactTimepiece = "Item_Artifact_Timepiece";
            public const string ItemBlinkEye = "Item_BlinkEye";
            public const string ItemBlinkEyeMarionette = "Item_BlinkEye_Marionette";
            public const string ItemCleaver = "Item_Cleaver";
            public const string ItemRhoStone = "Item_RhoStone";
            public const string ItemBandage = "Item_Bandage";
            public const string ItemBeans = "Item_Beans";
            public const string ItemBeansEaten = "Item_Beans_Eaten";
            public const string ItemBeansPeriphery = "Item_Beans_Periphery";
            public const string ItemCandyCauldron = "Item_CandyCauldron";
            public const string ItemCandyCauldronEmpty = "Item_CandyCauldron_Empty";
            public const string ItemFoodCookie = "Item_Food_Cookie";
            public const string ItemFoodBar = "Item_Food_Bar";
            public const string ItemFoodFruit = "Item_Food_Fruit";
            public const string ItemCocoaEmpty = "Item_Cocoa_Empty";
            public const string ItemCocoaFull = "Item_Cocoa_Full";
            public const string ItemFoodMeat = "Item_Food_Meat";
            public const string ItemMilk = "Item_Milk";
            public const string ItemMilkEmpty = "Item_Milk_Empty";
            public const string ItemMilkRho = "Item_Milk_Rho";
            public const string ItemWine = "Item_Wine";
            public const string ItemWineEmpty = "Item_Wine_Empty";
            public const string ItemInjector = "Item_Injector";
            public const string ItemInoculator = "Item_Inoculator";
            public const string ItemPillbottle = "Item_Pillbottle";
            public const string DenizenSlugGrub = "Denizen_SlugGrub";
            public const string DenizenSlugGrubChristmas = "Denizen_SlugGrub_Christmas";
            public const string DenizenSlugGrubSam = "Denizen_SlugGrub_Sam";
            public const string ItemFloppyT1 = "Item_Floppy_T1";
            public const string ItemFloppyT1Notes = "Item_Floppy_T1_Notes";
            public const string ItemFloppyT1RandomItem = "Item_Floppy_T1_RandomItem";
            public const string ItemFloppyT2 = "Item_Floppy_T2";
            public const string ItemFloppyT3 = "Item_Floppy_T3";
            public const string ItemFloppyTest = "Item_Floppy_Test";
            public const string ItemFloppyVariant = "Item_Floppy_Variant";
            public const string ItemNote01 = "Item_Note_01";
            public const string ItemNote02Bloody = "Item_Note_02_Bloody";
            public const string ItemNote03Torn = "Item_Note_03_Torn";
            public const string ItemRadio = "Item_Radio";
            public const string ItemAutoPiton = "Item_AutoPiton";
            public const string ItemBarnacleHook = "Item_BarnacleHook";
            public const string ItemBarnacleHookInfinite = "Item_BarnacleHook_Infinite";
            public const string ItemCryogun = "Item_Cryogun";
            public const string ItemEntityScanner = "Item_EntityScanner";
            public const string ItemFlaregun = "Item_Flaregun";
            public const string ItemFlaregunAmmo = "Item_Flaregun_Ammo";
            public const string ItemFlashlight = "Item_Flashlight";
            public const string ANullitemPiton = "A_NullItem_Piton";
            public const string ItemPiton = "Item_Piton";
            public const string ItemPitonHoliday = "Item_Piton_Holiday";
            public const string ItemPowercell = "Item_Powercell";
            public const string ItemRebar = "Item_Rebar";
            public const string ItemRebarBone = "Item_Rebar_Bone";
            public const string ItemRebarExplosive = "Item_Rebar_Explosive";
            public const string ItemRebarHoliday = "Item_Rebar_Holiday";
            public const string ItemRebarRope = "Item_RebarRope";
            public const string ItemRebarRopeHoliday = "Item_RebarRope_Holiday";
            public const string ItemRope = "Item_Rope";
            public const string ItemRubble = "Item_Rubble";
            public const string ItemHammerCosmeticWrench = "Item_Hammer_Cosmetic_Wrench";
            public const string ItemBanHammer = "Item_BanHammer";
            public const string ItemHammer = "Item_Hammer";
            public const string Item10mmAmmo = "Item_10mm_Ammo";
            public const string ItemHandgun = "Item_Handgun";
            public const string ItemHandgunDebug = "Item_Handgun_Debug";
            public const string ItemPipewrench = "Item_Pipewrench";
            public const string ItemComponent = "Item_Component";
            public const string ItemTemp = "Item_Temp";
            public const string ItemTrinketBase = "Item_Trinket_Base";
            public const string ItemTrinketBeta = "Item_Trinket_Beta";
            public const string ItemTrinketCalmingBuddy = "Item_Trinket_CalmingBuddy";
            public const string ItemTrinketCarabiner = "Item_Trinket_Carabiner";
            public const string ItemTrinketChalk = "Item_Trinket_Chalk";
            public const string ItemTrinketClimbingShoes = "Item_Trinket_ClimbingShoes";
            public const string ItemTrinketEmployeeID = "Item_Trinket_EmployeeID";
            public const string ItemTrinketHeadlamp = "Item_Trinket_Headlamp";
            public const string ItemTrinketHelmet = "Item_Trinket_Helmet";
            public const string ItemTrinketMassDamper = "Item_Trinket_MassDamper";
            public const string ItemTrinketMoonRock = "Item_Trinket_MoonRock";
            public const string ItemTrinketNugget = "Item_Trinket_Nugget";
            public const string ItemTrinketPhotoOfHome = "Item_Trinket_PhotoOfHome";
            public const string ItemTrinketPouch = "Item_Trinket_Pouch";
        }

    }
}
