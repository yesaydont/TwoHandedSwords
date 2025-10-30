using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.GameModes;
using HarmonyLib;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp")]

namespace TwoHandedSwordMod
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class TwoHandedSwordMod : BaseUnityPlugin
    {
        private const string ModId = "com.yourname.rounds.twohandedsword";
        private const string ModName = "Two Handed Sword Mod";
        public const string Version = "1.0.0";
        public const string ModInitials = "THS";
        
        public static TwoHandedSwordMod instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
            
            instance = this;
            
            UnityEngine.Debug.Log($"[{ModInitials}] {ModName} v{Version} loading...");
        }

        void Start()
        {
            UnityEngine.Debug.Log($"[{ModInitials}] Registering cards...");
            
            // Register cards with callbacks
            CustomCard.BuildCard<MightyCleaverCard>((card) => {
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
                UnityEngine.Debug.Log($"[{ModInitials}] Registered: Mighty Cleaver");
            });
            
            CustomCard.BuildCard<SwordMasterCard>((card) => {
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
                UnityEngine.Debug.Log($"[{ModInitials}] Registered: Sword Master");
            });
            
            CustomCard.BuildCard<HeavyStrikeCard>((card) => {
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
                UnityEngine.Debug.Log($"[{ModInitials}] Registered: Heavy Strike");
            });
            
            CustomCard.BuildCard<WhirlwindSlashCard>((card) => {
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
                UnityEngine.Debug.Log($"[{ModInitials}] Registered: Whirlwind Slash");
            });
            
            CustomCard.BuildCard<BerserkersRageCard>((card) => {
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
                UnityEngine.Debug.Log($"[{ModInitials}] Registered: Berserkers Rage");
            });
            
            // Hook into game mode to show cards
            GameModeManager.AddHook(GameModeHooks.HookGameStart, OnGameStart);
            
            UnityEngine.Debug.Log($"[{ModInitials}] {ModName} loaded successfully!");
        }
        
        IEnumerator OnGameStart(IGameModeHandler gm)
        {
            // Show all hidden cards when game starts
            // Using the correct API method
            foreach (var cardInfo in ModdingUtils.Utils.Cards.instance.GetHiddenCards())
            {
                if (cardInfo != null && cardInfo.cardName != null)
                {
                    // Check if it's one of our cards by checking the source
                    var customCard = cardInfo.gameObject?.GetComponent<CustomCard>();
                    if (customCard != null && customCard.GetModName() == ModInitials)
                    {
                        ModdingUtils.Utils.Cards.instance.ShowCard(cardInfo);
                        UnityEngine.Debug.Log($"[{ModInitials}] Showing card: {cardInfo.cardName}");
                    }
                }
            }
            yield break;
        }
    }

    // Base class for sword cards
    public abstract class BaseSwordCard : CustomCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[THS] Player {player.playerID} equipped {GetTitle()}");
        }
        
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[THS] Player {player.playerID} unequipped {GetTitle()}");
        }
        
        protected override GameObject GetCardArt() => null;
        public override string GetModName() => TwoHandedSwordMod.ModInitials;
    }

    // Mighty Cleaver - Basic two-handed sword card
    public class MightyCleaverCard : BaseSwordCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            
            gun.damage = 2.5f;
            gun.attackSpeed = 0.4f;
            gun.reloadTime = 1.3f;
            gun.knockback = 3f;
            gun.projectileSpeed = 0.8f;
            gun.bulletDamageMultiplier = 1f;
            gun.projectielSimulatonSpeed = 0.7f;
            gun.gravity = 1.5f;
            gun.damageAfterDistanceMultiplier = 1.2f;
            
            statModifiers.health = 1.5f;
            statModifiers.movementSpeed = 0.7f;
            
            block.cdMultiplier = 0.8f;
            block.additionalBlocks = 1;
        }

        protected override string GetTitle() => "Mighty Cleaver";
        protected override string GetDescription() => "Wield a massive two-handed sword!\nSlow but devastating attacks.";
        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Common;
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat() { positive = true, stat = "Damage", amount = "+150%", simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf },
                new CardInfoStat() { positive = true, stat = "Knockback", amount = "+200%", simepleAmount = CardInfoStat.SimpleAmount.aLotOf },
                new CardInfoStat() { positive = true, stat = "Health", amount = "+50%", simepleAmount = CardInfoStat.SimpleAmount.Some },
                new CardInfoStat() { positive = false, stat = "Attack Speed", amount = "-60%", simepleAmount = CardInfoStat.SimpleAmount.aLotLower },
                new CardInfoStat() { positive = false, stat = "Move Speed", amount = "-30%", simepleAmount = CardInfoStat.SimpleAmount.lower }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme() => CardThemeColor.CardThemeColorType.DefensiveBlue;
    }

    // Sword Master - Advanced sword techniques
    public class SwordMasterCard : BaseSwordCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            
            gun.damage = 1.8f;
            gun.attackSpeed = 0.7f;
            gun.reflects = 2;
            gun.bulletDamageMultiplier = 1.3f;
            
            statModifiers.movementSpeed = 0.9f;
            statModifiers.jump = 1.2f;
            
            block.cdMultiplier = 0.6f;
            block.healing = 0.2f;
        }

        protected override string GetTitle() => "Sword Master";
        protected override string GetDescription() => "Years of training with the blade.\nPerfect your technique.";
        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Rare;
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat() { positive = true, stat = "Damage", amount = "+80%", simepleAmount = CardInfoStat.SimpleAmount.aLotOf },
                new CardInfoStat() { positive = true, stat = "Bullet Reflects", amount = "+2", simepleAmount = CardInfoStat.SimpleAmount.Some },
                new CardInfoStat() { positive = true, stat = "Block Cooldown", amount = "-40%", simepleAmount = CardInfoStat.SimpleAmount.lower },
                new CardInfoStat() { positive = false, stat = "Attack Speed", amount = "-30%", simepleAmount = CardInfoStat.SimpleAmount.lower }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme() => CardThemeColor.CardThemeColorType.TechWhite;
    }

    // Heavy Strike - Charged attack
    public class HeavyStrikeCard : BaseSwordCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 3f;
            gun.attackSpeed = 0.3f;
            gun.chargeSpeedTo = 0.5f;
            gun.knockback = 5f;
            gun.destroyBulletAfter = 0.5f;
            gun.speedMOnBounce = 2f;
            gun.dmgMOnBounce = 1.5f;
            
            statModifiers.health = 1.2f;
        }

        protected override string GetTitle() => "Heavy Strike";
        protected override string GetDescription() => "Charge up for a devastating blow!";
        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Uncommon;
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat() { positive = true, stat = "Damage", amount = "+200%", simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf },
                new CardInfoStat() { positive = true, stat = "Knockback", amount = "+400%", simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf },
                new CardInfoStat() { positive = true, stat = "Charge Attack", amount = "Enabled", simepleAmount = CardInfoStat.SimpleAmount.notAssigned },
                new CardInfoStat() { positive = false, stat = "Attack Speed", amount = "-70%", simepleAmount = CardInfoStat.SimpleAmount.aLotLower }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme() => CardThemeColor.CardThemeColorType.DestructiveRed;
    }

    // Whirlwind Slash - Area attack
    public class WhirlwindSlashCard : BaseSwordCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 0.7f;
            gun.attackSpeed = 1.5f;
            gun.numberOfProjectiles = 8;
            gun.spread = 0.5f;
            gun.evenSpread = 1f;
            gun.bulletDamageMultiplier = 0.8f;
            
            statModifiers.movementSpeed = 1.1f;
        }

        protected override string GetTitle() => "Whirlwind Slash";
        protected override string GetDescription() => "Spin your blade in all directions!";
        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Uncommon;
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat() { positive = true, stat = "Projectiles", amount = "+8", simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf },
                new CardInfoStat() { positive = true, stat = "Attack Speed", amount = "+50%", simepleAmount = CardInfoStat.SimpleAmount.Some },
                new CardInfoStat() { positive = true, stat = "360° Attack", amount = "Yes", simepleAmount = CardInfoStat.SimpleAmount.notAssigned },
                new CardInfoStat() { positive = false, stat = "Damage", amount = "-30%", simepleAmount = CardInfoStat.SimpleAmount.lower }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme() => CardThemeColor.CardThemeColorType.PoisonGreen;
    }

    // Berserker's Rage - High risk high reward
    public class BerserkersRageCard : BaseSwordCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            
            gun.damage = 2f;
            gun.attackSpeed = 1.8f;
            gun.reloadTime = 0.5f;
            gun.bulletDamageMultiplier = 1.5f;
            
            statModifiers.health = 0.6f;
            statModifiers.movementSpeed = 1.3f;
            statModifiers.lifeSteal = 0.3f;
            
            block.cdMultiplier = 2f;
        }

        protected override string GetTitle() => "Berserker's Rage";
        protected override string GetDescription() => "Abandon defense for raw power!";
        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Rare;
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat() { positive = true, stat = "Damage", amount = "+100%", simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf },
                new CardInfoStat() { positive = true, stat = "Attack Speed", amount = "+80%", simepleAmount = CardInfoStat.SimpleAmount.aLotOf },
                new CardInfoStat() { positive = true, stat = "Life Steal", amount = "+30%", simepleAmount = CardInfoStat.SimpleAmount.Some },
                new CardInfoStat() { positive = false, stat = "Health", amount = "-40%", simepleAmount = CardInfoStat.SimpleAmount.aLotLower },
                new CardInfoStat() { positive = false, stat = "Block Cooldown", amount = "+100%", simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf }  // FIXED: Changed from aLotHigher
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme() => CardThemeColor.CardThemeColorType.EvilPurple;
    }
}
