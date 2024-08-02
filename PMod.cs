using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using Shockah.Shared;
using Nanoray.EnumByNameSourceGenerator;
using System.Reflection;
using Microsoft.Xna.Framework;
using APurpleApple.Selene.Artifacts;
using APurpleApple.Selene.ExternalAPIs;
using System.Linq;
using System.Xml.Linq;
using APurpleApple.Selene.Patches;
using APurpleApple.Selene.Cards;
using Newtonsoft.Json;

namespace APurpleApple.Selene;

public sealed class PMod : SimpleMod
{
    internal static PMod Instance { get; private set; } = null!;
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    public static Dictionary<string, ISpriteEntry> sprites = new();
    public static Dictionary<string, IStatusEntry> statuses = new();
    public static Dictionary<string, IPartEntry> parts = new();
    public static Dictionary<string, TTGlossary> glossaries = new();
    public static Dictionary<string, ICharacterAnimationEntryV2> animations = new();
    public static Dictionary<string, IArtifactEntry> artifacts = new();
    public static Dictionary<string, ICardEntry> cards = new();
    public static Dictionary<string, ICharacterEntryV2> characters = new();
    public static Dictionary<string, IShipEntry> ships = new();
    public static Dictionary<string, IDeckEntry> decks = new();

    public static IKokoroApi? kokoroApi { get; private set; }
    public static IShipPartExpansionAPI? SPEApi { get; private set; }
    public static Assembly? kokoroAssembly { get; private set; }

    public static List<Tuple<Type, PType>> cardActionLooksForType = new();

    internal static IReadOnlyList<Type> Registered_Card_Types { get; } = [
        typeof(Card_SeleneAttachCannon),
        typeof(Card_SeleneAttachBay),
        typeof(Card_SeleneAttachShield),
        typeof(Card_SeleneAttachThruster),
        typeof(Card_SeleneAttachCloaking),
        typeof(Card_SeleneAttachBubble),
        typeof(Card_SeleneAttachDynamo),
        typeof(Card_SeleneAttachLauncher),
        typeof(Card_SeleneAttachReactor),
        typeof(Card_SeleneAlign),
        typeof(Card_SeleneEject),
        typeof(Card_SeleneMagnetize),
        typeof(Card_SeleneShuffle),
        typeof(Card_SeleneEjectAll),
        typeof(Card_SeleneWeld),
        typeof(Card_SeleneActivateCloak),
        typeof(Card_SeleneReinforce),
        typeof(Card_SelenePlating),
        typeof(Card_SeleneRecycle),
        typeof(Card_SeleneDelivery),
        typeof(Card_RandomAttach),
    ];



    internal static IReadOnlyList<Type> Registered_Artifact_Types { get; } = [
        typeof(Artifact_Selene),
        typeof(Artifact_BreakArmor),
        typeof(Artifact_Delivery),
        typeof(Artifact_Safety),
        typeof(Artifact_EjectBuff),
        typeof(Artifact_CheapRandom),
        //typeof(Artifact_SeleneV2),
    ];

    public override object? GetApi(IModManifest requestingMod) => new ApiImplementation();

    public void RegisterSprite(string key, string fileName, IPluginPackage<IModManifest> package)
    {
        sprites.Add(key, Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/" + fileName)));
    }

    private void Patch()
    {
        Harmony harmony = new("APurpleApple.Selene");
        harmony.PatchAll();

        CustomTTGlossary.ApplyPatches(harmony);
    }

    public PMod(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;

        this.AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
        );

        RegisterSprite("selene_cdrone_body", "Parts/cdrone_body.png", package);
        RegisterSprite("selene_cdrone_body_gun", "Parts/cdrone_body_gun.png", package);
        RegisterSprite("selene_cdrone_rail", "Parts/cdrone_rail.png", package);
        RegisterSprite("selene_cdrone_claw", "Parts/cdrone_claw.png", package);
        RegisterSprite("selene_cdrone_top", "Parts/cdrone_top.png", package);
        RegisterSprite("selene_cdrone_arm_small", "Parts/cdrone_arm_small.png", package);
        RegisterSprite("selene_cdrone_arm_big", "Parts/cdrone_arm_big.png", package);
        RegisterSprite("selene_cdrone_arm_med", "Parts/cdrone_arm_med.png", package);
        RegisterSprite("selene_hint_ship_widden", "Hints/hint_ship_widden.png", package);
        RegisterSprite("selene_hint_magnetize_pull", "Hints/hint_magnetize_cap_pull.png", package);
        RegisterSprite("selene_hint_magnetize_push", "Hints/hint_magnetize_cap_push.png", package);
        RegisterSprite("selene_part_cannon_temp_off", "Parts/cannon_temp_off.png", package);
        RegisterSprite("selene_fx_part_bit_0", "FX/fx_part_bit_0.png", package);
        RegisterSprite("FX_ShieldProj", "Parts/shield_projection.png", package);

        RegisterSprite("selene_part_cannon", "Parts/cannon_temp.png", package);
        RegisterSprite("selene_part_dynamo", "Parts/dynamocannon.png", package);
        RegisterSprite("selene_part_bay", "Parts/bay_temp.png", package);
        RegisterSprite("selene_part_shield", "Parts/shield_temp.png", package);
        RegisterSprite("selene_part_shieldV2", "Parts/shield_v2_temp.png", package);
        RegisterSprite("selene_part_thruster", "Parts/thruster_temp.png", package);
        RegisterSprite("selene_part_thrusterV2", "Parts/thruster_v2_temp.png", package);
        RegisterSprite("selene_part_cloak", "Parts/cloak_temp.png", package);
        RegisterSprite("selene_part_bubble", "Parts/bubble_bay.png", package);
        RegisterSprite("selene_part_launcher", "Parts/missilelauncher.png", package);
        RegisterSprite("selene_part_launcherHeavy", "Parts/missilelauncherheavy.png", package);
        RegisterSprite("selene_part_reactor", "Parts/reactor.png", package);

        RegisterSprite("selene_border", "Characters/panel.png", package);
        RegisterSprite("selene_cardBorder", "Cards/border_selene.png", package);
        RegisterSprite("selene_cardBackDefault", "Cards/back_default.png", package);
        RegisterSprite("selene_cardBackAttach", "Cards/back_attach.png", package);
        RegisterSprite("selene_cardBackAttach_tbot", "Cards/back_attach_toggle_bottom.png", package);
        RegisterSprite("selene_cardBackAttach_ttop", "Cards/back_attach_toggle_top.png", package);
        RegisterSprite("back_MagPull", "Cards/back_mag_pull.png", package);
        RegisterSprite("back_MagPush", "Cards/back_mag_push.png", package);
        RegisterSprite("selene_mini", "Characters/selene_mini.png", package);
        RegisterSprite("selene_neutral_0", "Characters/selene_neutral_0.png", package);
        RegisterSprite("selene_neutral_1", "Characters/selene_neutral_1.png", package);
        RegisterSprite("selene_neutral_2", "Characters/selene_neutral_2.png", package);
        RegisterSprite("selene_neutral_3", "Characters/selene_neutral_3.png", package);
        RegisterSprite("selene_neutral_4", "Characters/selene_neutral_4.png", package);
        RegisterSprite("selene_artifact", "Artifacts/artifact_selene.png", package);
        RegisterSprite("selene_artifact_gun", "Artifacts/artifact_selene_gun.png", package);
        RegisterSprite("artifact_cheap_random", "Artifacts/artifact_cheap_random.png", package);
        RegisterSprite("artifact_delivery", "Artifacts/artifact_delivery.png", package);
        RegisterSprite("artifact_safety", "Artifacts/artifact_safety.png", package);
        RegisterSprite("artifact_break_armor", "Artifacts/artifact_break_armor.png", package);
        RegisterSprite("artifact_eject_buff", "Artifacts/artifact_eject_buff.png", package);

        RegisterSprite("icon_attachPart", "Icons/Attach.png", package);
        RegisterSprite("icon_magPush", "Icons/magPush.png", package);
        RegisterSprite("icon_magPull", "Icons/magPull.png", package);
        RegisterSprite("icon_weldPart", "Icons/Weld.png", package);
        RegisterSprite("icon_ejectLeft", "Icons/EjectLeft.png", package);
        RegisterSprite("icon_ejectRight", "Icons/EjectRight.png", package);
        RegisterSprite("icon_alignDrone", "Icons/aligndrone.png", package);
        RegisterSprite("icon_recyclePart", "Icons/Recycle.png", package);
        RegisterSprite("icon_breakTemp", "Icons/BreakTemp.png", package);
        RegisterSprite("icon_breakTempSingle", "Icons/BreakTempSingle.png", package);
        RegisterSprite("icon_single", "Icons/Single.png", package);
        RegisterSprite("icon_singleTemp", "Icons/SingleTemp.png", package);
        RegisterSprite("status_reinforce", "Icons/reinforced.png", package);
        RegisterSprite("status_plating", "Icons/plating.png", package);
        RegisterSprite("cost_droneshift", "Icons/droneShiftCost.png", package);
        RegisterSprite("cost_droneshiftOff", "Icons/droneShiftCostOff.png", package);

        RegisterSprite("icon_part_cannon", "Icons/Cannon.png", package);
        RegisterSprite("icon_part_dynamo", "Icons/DynamoCannon.png", package);
        RegisterSprite("icon_part_shield", "Icons/Shield.png", package);
        RegisterSprite("icon_part_shield_v2", "Icons/ShieldV2.png", package);
        RegisterSprite("icon_part_thruster_left", "Icons/ThrusterLeft.png", package);
        RegisterSprite("icon_part_thruster_right", "Icons/ThrusterRight.png", package);
        RegisterSprite("icon_part_thruster_v2_left", "Icons/ThrusterV2Left.png", package);
        RegisterSprite("icon_part_thruster_v2_right", "Icons/ThrusterV2Right.png", package);
        RegisterSprite("icon_part_bay", "Icons/Bay.png", package);
        RegisterSprite("icon_part_drill", "Icons/Drill.png", package);
        RegisterSprite("icon_part_cloak", "Icons/Cloak.png", package);
        RegisterSprite("icon_part_bubble", "Icons/Bubble.png", package);
        RegisterSprite("icon_part_launcher", "Icons/Launcher.png", package);
        RegisterSprite("icon_part_launcherHeavy", "Icons/LauncherHeavy.png", package);
        RegisterSprite("icon_part_reactor", "Icons/Reactor.png", package);

        RegisterSprite("fx_shield_0", "FX/Shield/0.png", package);
        RegisterSprite("fx_shield_1", "FX/Shield/1.png", package);
        RegisterSprite("fx_shield_2", "FX/Shield/2.png", package);
        RegisterSprite("fx_shield_3", "FX/Shield/3.png", package);
        RegisterSprite("fx_shield_4", "FX/Shield/4.png", package);
        RegisterSprite("fx_shield_5", "FX/Shield/5.png", package);
        RegisterSprite("fx_shield_6", "FX/Shield/6.png", package);
        RegisterSprite("fx_shield_7", "FX/Shield/7.png", package);

        RegisterSprite("fx_shieldImpact_0", "FX/ShieldImpact/0.png", package);
        RegisterSprite("fx_shieldImpact_1", "FX/ShieldImpact/1.png", package);
        RegisterSprite("fx_shieldImpact_2", "FX/ShieldImpact/2.png", package);
        RegisterSprite("fx_shieldImpact_3", "FX/ShieldImpact/3.png", package);
        RegisterSprite("fx_shieldImpact_4", "FX/ShieldImpact/4.png", package);

        RegisterSprite("fx_bubble_0", "FX/Bubble/0.png", package);
        RegisterSprite("fx_bubble_1", "FX/Bubble/1.png", package);
        RegisterSprite("fx_bubble_2", "FX/Bubble/2.png", package);
        RegisterSprite("fx_bubble_3", "FX/Bubble/3.png", package);
        RegisterSprite("fx_bubble_4", "FX/Bubble/4.png", package);

        glossaries.Add("AttachPart", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_attachPart"].Sprite,
            () => Localizations.Localize(["action", "AttachPart", "name"]),
            () => Localizations.Localize(["action", "AttachPart", "description"])
            ));

        glossaries.Add("AlignDrone", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_alignDrone"].Sprite,
            () => Localizations.Localize(["action", "Align", "name"]),
            () => Localizations.Localize(["action", "Align", "description"])
            ));

        glossaries.Add("MagPush", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_magPush"].Sprite,
            () => Localizations.Localize(["action", "MagPush", "name"]),
            () => Localizations.Localize(["action", "MagPush", "description"])
            ));

        glossaries.Add("MagPull", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_magPull"].Sprite,
            () => Localizations.Localize(["action", "MagPull", "name"]),
            () => Localizations.Localize(["action", "MagPull", "description"])
            ));

        glossaries.Add("RecyclePart", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_recyclePart"].Sprite,
            () => Localizations.Localize(["action", "RecyclePart", "name"]),
            () => Localizations.Localize(["action", "RecyclePart", "description"])
            ));

        glossaries.Add("WeldPart", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_weldPart"].Sprite,
            () => Localizations.Localize(["action", "WeldPart", "name"]),
            () => Localizations.Localize(["action", "WeldPart", "description"])
            ));

        glossaries.Add("Part_Cannon", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_cannon"].Sprite,
            () => Localizations.Localize(["parts", "Cannon", "name"]),
            () => Localizations.Localize(["parts", "Cannon", "description"])
            ));

        glossaries.Add("Part_Reactor", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_reactor"].Sprite,
            () => Localizations.Localize(["parts", "Reactor", "name"]),
            () => Localizations.Localize(["parts", "Reactor", "description"])
            ));

        glossaries.Add("Part_Launcher", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_launcher"].Sprite,
            () => Localizations.Localize(["parts", "Launcher", "name"]),
            () => Localizations.Localize(["parts", "Launcher", "description"])
            ));

        glossaries.Add("Part_LauncherHeavy", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_launcherHeavy"].Sprite,
            () => Localizations.Localize(["parts", "Launcher", "name"]),
            () => Localizations.Localize(["parts", "Launcher", "descriptionA"])
            ));

        glossaries.Add("Part_Dynamo", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_dynamo"].Sprite,
            () => Localizations.Localize(["parts", "Dynamo", "name"]),
            () => Localizations.Localize(["parts", "Dynamo", "description"])
            ));

        glossaries.Add("Part_Bay", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_bay"].Sprite,
            () => Localizations.Localize(["parts", "Bay", "name"]),
            () => Localizations.Localize(["parts", "Bay", "description"])
            ));

        glossaries.Add("Part_ThrusterLeft", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_thruster_left"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterLeft", "name"]),
            () => Localizations.Localize(["parts", "ThrusterLeft", "description"])
            ));

        glossaries.Add("Part_ThrusterRight", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_thruster_right"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterRight", "name"]),
            () => Localizations.Localize(["parts", "ThrusterRight", "description"])
            ));

        glossaries.Add("Part_ThrusterV2Left", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_thruster_v2_left"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterV2Left", "name"]),
            () => Localizations.Localize(["parts", "ThrusterV2Left", "description"])
            ));

        glossaries.Add("Part_ThrusterV2Right", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_thruster_v2_right"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterV2Right", "name"]),
            () => Localizations.Localize(["parts", "ThrusterV2Right", "description"])
            ));

        glossaries.Add("Part_Shield", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_shield"].Sprite,
            () => Localizations.Localize(["parts", "Shield", "name"]),
            () => Localizations.Localize(["parts", "Shield", "description"])
            ));

        glossaries.Add("Part_ShieldV2", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_shield_v2"].Sprite,
            () => Localizations.Localize(["parts", "ShieldV2", "name"]),
            () => Localizations.Localize(["parts", "ShieldV2", "description"])
            ));

        glossaries.Add("Part_Cloak", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_cloak"].Sprite,
            () => Localizations.Localize(["parts", "Cloak", "name"]),
            () => Localizations.Localize(["parts", "Cloak", "description"])
            ));

        glossaries.Add("Part_Bubble", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.part,
            () => sprites["icon_part_bubble"].Sprite,
            () => Localizations.Localize(["parts", "Bubble", "name"]),
            () => Localizations.Localize(["parts", "Bubble", "description"])
            ));

        glossaries.Add("SingleUse", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.parttrait,
            () => sprites["icon_single"].Sprite,
            () => Localizations.Localize(["partTrait", "SingleUse", "name"]),
            () => Localizations.Localize(["partTrait", "SingleUse", "description"])
            ));

        glossaries.Add("Temp", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.parttrait,
            () => SSpr.icons_temporary,
            () => Localizations.Localize(["partTrait", "Temp", "name"]),
            () => Localizations.Localize(["partTrait", "Temp", "description"])
            ));

        glossaries.Add("Breakable", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.parttrait,
            () => SSpr.icons_breakable,
            () => Localizations.Localize(["partTrait", "Breakable", "name"]),
            () => Localizations.Localize(["partTrait", "Breakable", "description"])
            ));

        statuses["reinforce"] = helper.Content.Statuses.RegisterStatus("reinforce", new StatusConfiguration() {
            Definition = new StatusDef()
            {
                isGood = true,
                icon = sprites["status_reinforce"].Sprite,
                affectedByTimestop = false,
                border = new Color("a8a8a8"),
                color = new Color("a8a8a8")
            },
            Description = AnyLocalizations.Bind(["status", "Reinforce", "description"]).Localize,
            Name = AnyLocalizations.Bind(["status", "Reinforce", "name"]).Localize
        });

        statuses["plating"] = helper.Content.Statuses.RegisterStatus("plating", new StatusConfiguration()
        {
            Definition = new StatusDef()
            {
                isGood = true,
                icon = sprites["status_plating"].Sprite,
                affectedByTimestop = false,
                border = new Color("a8a8a8"),
                color = new Color("a8a8a8")
            },
            Description = AnyLocalizations.Bind(["status", "Plating", "description"]).Localize,
            Name = AnyLocalizations.Bind(["status", "Plating", "name"]).Localize
        });

        parts["selene_cannon"] = helper.Content.Ships.RegisterPart("selene_cannon", new PartConfiguration() { Sprite = sprites["selene_part_cannon"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_bay"] = helper.Content.Ships.RegisterPart("selene_bay", new PartConfiguration() { Sprite = sprites["selene_part_bay"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_shield"] = helper.Content.Ships.RegisterPart("selene_shield", new PartConfiguration() { Sprite = sprites["selene_part_shield"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_shieldV2"] = helper.Content.Ships.RegisterPart("selene_shieldV2", new PartConfiguration() { Sprite = sprites["selene_part_shieldV2"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_thruster"] = helper.Content.Ships.RegisterPart("selene_thruster", new PartConfiguration() { Sprite = sprites["selene_part_thruster"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_thrusterV2"] = helper.Content.Ships.RegisterPart("selene_thrusterV2", new PartConfiguration() { Sprite = sprites["selene_part_thrusterV2"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_cloak"] = helper.Content.Ships.RegisterPart("selene_cloak", new PartConfiguration() { Sprite = sprites["selene_part_cloak"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_bubble"] = helper.Content.Ships.RegisterPart("selene_bubble", new PartConfiguration() { Sprite = sprites["selene_part_bubble"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_dynamo"] = helper.Content.Ships.RegisterPart("selene_dynamo", new PartConfiguration() { Sprite = sprites["selene_part_dynamo"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_reactor"] = helper.Content.Ships.RegisterPart("selene_reactor", new PartConfiguration() { Sprite = sprites["selene_part_reactor"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_launcher"] = helper.Content.Ships.RegisterPart("selene_launcher", new PartConfiguration() { Sprite = sprites["selene_part_launcher"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_launcherHeavy"] = helper.Content.Ships.RegisterPart("selene_launcherHeavy", new PartConfiguration() { Sprite = sprites["selene_part_launcherHeavy"].Sprite, DisabledSprite = SSpr.parts_scaffolding });

        decks["selene"] = helper.Content.Decks.RegisterDeck("selene", 
            new DeckConfiguration() { 
            BorderSprite = sprites["selene_cardBorder"].Sprite, 
            DefaultCardArt = sprites["selene_cardBackDefault"].Sprite, 
            Name = AnyLocalizations.Bind(["characters", "Selene", "name"]).Localize,
            Definition = new DeckDef() { color = new Color("E77FDB"), titleColor = Colors.black } });

        animations["selene_mini"] = helper.Content.Characters.V2.RegisterCharacterAnimation(
            new CharacterAnimationConfigurationV2() {
                CharacterType = decks["selene"].UniqueName,
                Frames = [ sprites["selene_mini"].Sprite ],
                LoopTag = "mini"
            });

        animations["selene_neutral"] = helper.Content.Characters.V2.RegisterCharacterAnimation(
            new CharacterAnimationConfigurationV2()
            {
                CharacterType = decks["selene"].UniqueName,
                Frames = [
                    sprites["selene_neutral_0"].Sprite,
                    sprites["selene_neutral_1"].Sprite,
                    sprites["selene_neutral_2"].Sprite,
                    sprites["selene_neutral_3"].Sprite,
                    sprites["selene_neutral_4"].Sprite
                ],
                LoopTag = "neutral"
            });
        
        characters["selene"] = helper.Content.Characters.V2.RegisterPlayableCharacter("Selene", 
            new PlayableCharacterConfigurationV2() {
                Description = AnyLocalizations.Bind(["characters", "Selene", "description"]).Localize,
                BorderSprite = sprites["selene_border"].Sprite,
                Deck = decks["selene"].Deck,
                MiniAnimation = animations["selene_mini"].Configuration,
                NeutralAnimation = animations["selene_neutral"].Configuration,
                Starters = new StarterDeck() { artifacts = [new Artifact_Selene()], cards = [new Card_SeleneAlign(), new Card_SeleneAttachCannon()] }
            });



        foreach (var cardType in Registered_Card_Types)
            AccessTools.DeclaredMethod(cardType, nameof(IModCard.Register))?.Invoke(null, [helper]);

        foreach (var artifactType in Registered_Artifact_Types)
            AccessTools.DeclaredMethod(artifactType, nameof(IModArtifact.Register))?.Invoke(null, [helper]);

        helper.Events.OnModLoadPhaseFinished += (object? sender, ModLoadPhase e) => {
            if (e == ModLoadPhase.AfterDbInit)
            {
                kokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro");
                SPEApi = helper.ModRegistry.GetApi<IShipPartExpansionAPI>("APurpleApple.ShipPartExpansion");

                Patch();
            }
        };

    }
}
