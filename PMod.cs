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
        typeof(Card_SeleneTest)
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

        /*
        MethodInfo? method = kokoroAssembly?.GetTypes().FirstOrDefault(t=>t.Name == "DroneShiftManager")?.GetMethod("GetHandlingHook");
        if (method != null)
        {
            Console.WriteLine("heyo");
            harmony.Patch(method, postfix: typeof(Patch_Selene).GetMethod(nameof(Patch_Selene.IsDroneShiftPossible)));
        }*/

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

        RegisterSprite("selene_constructorDrone", "constructorDrone.png", package);

        RegisterSprite("selene_border", "Characters/panel.png", package);
        RegisterSprite("selene_cardBorder", "Characters/border_default.png", package);
        RegisterSprite("selene_cardBackDefault", "Characters/back_default.png", package);
        RegisterSprite("selene_mini", "Characters/selene_mini.png", package);
        RegisterSprite("selene_neutral_0", "Characters/selene_neutral_0.png", package);
        RegisterSprite("selene_artifact", "Artifacts/artifact_selene.png", package);

        decks["selene"] = helper.Content.Decks.RegisterDeck("selene", 
            new DeckConfiguration() { 
            BorderSprite = sprites["selene_cardBorder"].Sprite, 
            DefaultCardArt = sprites["selene_cardBackDefault"].Sprite, 
            Definition = new DeckDef() { color = Colors.white, titleColor = Colors.black } });

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
                Frames = [sprites["selene_neutral_0"].Sprite],
                LoopTag = "neutral"
            });

        characters["selene"] = helper.Content.Characters.RegisterCharacter("Selene", 
            new CharacterConfiguration() { 
                BorderSprite = sprites["selene_border"].Sprite, 
                Deck = decks["selene"].Deck,
                MiniAnimation = animations["selene_mini"].Configuration,
                NeutralAnimation = animations["selene_neutral"].Configuration,
                Starters = new StarterDeck() { artifacts = [new Artifact_Selene()] }
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
