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
    public static Dictionary<string, ICharacterAnimationEntry> animations = new();
    public static Dictionary<string, IArtifactEntry> artifacts = new();
    public static Dictionary<string, ICardEntry> cards = new();
    public static Dictionary<string, ICharacterEntry> characters = new();
    public static Dictionary<string, IShipEntry> ships = new();
    public static Dictionary<string, IDeckEntry> decks = new();

    public static IKokoroApi? kokoroApi { get; private set; }
    public static Assembly? kokoroAssembly { get; private set; }

    public static List<Tuple<Type, PType>> cardActionLooksForType = new();

    internal static IReadOnlyList<Type> Registered_Card_Types { get; } = [
        typeof(Card_SeleneAttachCannon),
        typeof(Card_SeleneAttachBay),
        typeof(Card_SeleneAttachShield),
        typeof(Card_SeleneAttachThruster),
        typeof(Card_SeleneAlign)
    ];

    internal static IReadOnlyList<Type> Registered_Artifact_Types { get; } = [
        typeof(Artifact_Selene)
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
        RegisterSprite("selene_cdrone_rail", "Parts/cdrone_rail.png", package);
        RegisterSprite("selene_cdrone_claw", "Parts/cdrone_claw.png", package);
        RegisterSprite("selene_cdrone_top", "Parts/cdrone_top.png", package);
        RegisterSprite("selene_cdrone_arm_small", "Parts/cdrone_arm_small.png", package);
        RegisterSprite("selene_cdrone_arm_big", "Parts/cdrone_arm_big.png", package);
        RegisterSprite("selene_cdrone_arm_med", "Parts/cdrone_arm_med.png", package);
        RegisterSprite("selene_hint_ship_widden", "Hints/hint_ship_widden.png", package);
        RegisterSprite("selene_part_cannon_temp_off", "Parts/cannon_temp_off.png", package);
        RegisterSprite("selene_fx_part_bit_0", "FX/fx_part_bit_0.png", package);
        RegisterSprite("FX_ShieldProj", "Parts/shield_projection.png", package);

        RegisterSprite("selene_part_cannon", "Parts/cannon_temp.png", package);
        RegisterSprite("selene_part_bay", "Parts/bay_temp.png", package);
        RegisterSprite("selene_part_shield", "Parts/shield_temp.png", package);
        RegisterSprite("selene_part_thruster", "Parts/thruster_temp.png", package);
        RegisterSprite("selene_part_thrusterV2", "Parts/thruster_v2_temp.png", package);
        RegisterSprite("selene_part_cloak", "Parts/cloak_temp.png", package);


        RegisterSprite("selene_border", "Characters/panel.png", package);
        RegisterSprite("selene_cardBorder", "Cards/border_selene.png", package);
        RegisterSprite("selene_cardBackDefault", "Cards/back_default.png", package);
        RegisterSprite("selene_cardBackAttach", "Cards/back_attach.png", package);
        RegisterSprite("selene_mini", "Characters/selene_mini.png", package);
        RegisterSprite("selene_neutral_0", "Characters/selene_neutral_0.png", package);
        RegisterSprite("selene_neutral_1", "Characters/selene_neutral_1.png", package);
        RegisterSprite("selene_neutral_2", "Characters/selene_neutral_2.png", package);
        RegisterSprite("selene_neutral_3", "Characters/selene_neutral_3.png", package);
        RegisterSprite("selene_neutral_4", "Characters/selene_neutral_4.png", package);
        RegisterSprite("selene_artifact", "Artifacts/artifact_selene.png", package);

        RegisterSprite("icon_attachPart", "Icons/Attach.png", package);
        RegisterSprite("icon_breakTemp", "Icons/BreakTemp.png", package);
        RegisterSprite("icon_breakTempSingle", "Icons/BreakTempSingle.png", package);
        RegisterSprite("icon_single", "Icons/Single.png", package);
        RegisterSprite("icon_singleTemp", "Icons/SingleTemp.png", package);

        RegisterSprite("icon_part_cannon", "Icons/Cannon.png", package);
        RegisterSprite("icon_part_shield", "Icons/Shield.png", package);
        RegisterSprite("icon_part_thruster_left", "Icons/ThrusterLeft.png", package);
        RegisterSprite("icon_part_thruster_right", "Icons/ThrusterRight.png", package);
        RegisterSprite("icon_part_thruster_v2_left", "Icons/ThrusterV2Left.png", package);
        RegisterSprite("icon_part_thruster_v2_right", "Icons/ThrusterV2Right.png", package);
        RegisterSprite("icon_part_bay", "Icons/Bay.png", package);
        RegisterSprite("icon_part_drill", "Icons/Drill.png", package);
        RegisterSprite("icon_part_cloak", "Icons/Cloak.png", package);

        glossaries.Add("AttachPart", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_attachPart"].Sprite,
            () => Localizations.Localize(["action", "AttachPart", "name"]),
            () => Localizations.Localize(["action", "AttachPart", "description"])
            ));

        glossaries.Add("Part_Cannon", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_cannon"].Sprite,
            () => Localizations.Localize(["parts", "Cannon", "name"]),
            () => Localizations.Localize(["parts", "Cannon", "description"])
            ));

        glossaries.Add("Part_Bay", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_bay"].Sprite,
            () => Localizations.Localize(["parts", "Bay", "name"]),
            () => Localizations.Localize(["parts", "Bay", "description"])
            ));

        glossaries.Add("Part_ThrusterLeft", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_thruster_left"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterLeft", "name"]),
            () => Localizations.Localize(["parts", "ThrusterLeft", "description"])
            ));

        glossaries.Add("Part_ThrusterRight", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_thruster_right"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterRight", "name"]),
            () => Localizations.Localize(["parts", "ThrusterRight", "description"])
            ));

        glossaries.Add("Part_ThrusterV2Left", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_thruster_v2_left"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterV2Left", "name"]),
            () => Localizations.Localize(["parts", "ThrusterV2Left", "description"])
            ));

        glossaries.Add("Part_ThrusterV2Right", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_thruster_v2_right"].Sprite,
            () => Localizations.Localize(["parts", "ThrusterV2Right", "name"]),
            () => Localizations.Localize(["parts", "ThrusterV2Right", "description"])
            ));

        glossaries.Add("Part_Shield", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_shield"].Sprite,
            () => Localizations.Localize(["parts", "Shield", "name"]),
            () => Localizations.Localize(["parts", "Shield", "description"])
            ));

        glossaries.Add("Part_Cloak", new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => sprites["icon_part_cloak"].Sprite,
            () => Localizations.Localize(["parts", "Cloak", "name"]),
            () => Localizations.Localize(["parts", "Cloak", "description"])
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

        parts["selene_cannon"] = helper.Content.Ships.RegisterPart("selene_cannon", new PartConfiguration() { Sprite = sprites["selene_part_cannon"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_bay"] = helper.Content.Ships.RegisterPart("selene_bay", new PartConfiguration() { Sprite = sprites["selene_part_bay"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_shield"] = helper.Content.Ships.RegisterPart("selene_shield", new PartConfiguration() { Sprite = sprites["selene_part_shield"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_thruster"] = helper.Content.Ships.RegisterPart("selene_thruster", new PartConfiguration() { Sprite = sprites["selene_part_thruster"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_thrusterV2"] = helper.Content.Ships.RegisterPart("selene_thrusterV2", new PartConfiguration() { Sprite = sprites["selene_part_thrusterV2"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        parts["selene_cloak"] = helper.Content.Ships.RegisterPart("selene_cloak", new PartConfiguration() { Sprite = sprites["selene_part_cloak"].Sprite, DisabledSprite = SSpr.parts_scaffolding });

        decks["selene"] = helper.Content.Decks.RegisterDeck("selene", 
            new DeckConfiguration() { 
            BorderSprite = sprites["selene_cardBorder"].Sprite, 
            DefaultCardArt = sprites["selene_cardBackDefault"].Sprite, 
            Name = AnyLocalizations.Bind(["characters", "Selene", "name"]).Localize,
            Definition = new DeckDef() { color = new Color("E77FDB"), titleColor = Colors.black } });

        animations["selene_mini"] = helper.Content.Characters.RegisterCharacterAnimation(
            new CharacterAnimationConfiguration() { 
                Deck = decks["selene"].Deck,
                Frames = [ sprites["selene_mini"].Sprite ],
                LoopTag = "mini"
            });

        animations["selene_neutral"] = helper.Content.Characters.RegisterCharacterAnimation(
            new CharacterAnimationConfiguration()
            {
                Deck = decks["selene"].Deck,
                Frames = [
                    sprites["selene_neutral_0"].Sprite,
                    sprites["selene_neutral_1"].Sprite,
                    sprites["selene_neutral_2"].Sprite,
                    sprites["selene_neutral_3"].Sprite,
                    sprites["selene_neutral_4"].Sprite
                ],
                LoopTag = "neutral"
            });

        characters["selene"] = helper.Content.Characters.RegisterCharacter("Selene", 
            new CharacterConfiguration() { 
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
                kokoroAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Kokoro");

                Patch();
            }
        };
    }

}
