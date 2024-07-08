using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.ExternalAPIs
{
    public partial interface IKokoroApi
    {
        IDroneShiftHook VanillaDroneShiftHook { get; }
        void RegisterDroneShiftHook(IDroneShiftHook hook, double priority);
        IActionApi Actions { get; }
        IConditionalActionApi ConditionalActions { get; }
        IActionCostApi ActionCosts { get; }

        public partial interface IConditionalActionApi
        {
            CardAction Make(IBoolExpression expression, CardAction action, bool fadeUnsatisfied = true);
            IIntExpression Constant(int value);
            IIntExpression HandConstant(int value);
            IIntExpression XConstant(int value);
            IIntExpression ScalarMultiplier(IIntExpression expression, int scalar);
            IBoolExpression HasStatus(Status status, bool targetPlayer = true, bool countNegative = false);
            IIntExpression Status(Status status, bool targetPlayer = true);
            IBoolExpression Equation(IIntExpression lhs, EquationOperator @operator, IIntExpression rhs, EquationStyle style, bool hideOperator = false);

            public enum EquationOperator
            {
                Equal, NotEqual, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual
            }

            public enum EquationStyle
            {
                Formal, State, Possession, PossessionComparison
            }

            public interface IExpression
            {
                void Render(G g, ref Vec position, bool isDisabled, bool dontRender);
                string GetTooltipDescription(State state, Combat? combat);
                List<Tooltip> GetTooltips(State state, Combat? combat) => [];
            }

            public interface IBoolExpression : IExpression
            {
                bool GetValue(State state, Combat combat);
                bool ShouldRenderQuestionMark(State state, Combat? combat) => true;
                IEnumerable<Tooltip> OverrideConditionalTooltip(State state, Combat? combat, Tooltip defaultTooltip, string defaultTooltipDescription) => [defaultTooltip];
            }

            public interface IIntExpression : IExpression
            {
                int GetValue(State state, Combat combat);
            }
        }

        public partial interface IActionCostApi
        {
            CardAction Make(IActionCost cost, CardAction action);
            CardAction Make(IReadOnlyList<IActionCost> costs, CardAction action);

            IActionCost Cost(IReadOnlyList<IResource> potentialResources, int amount = 1, int? iconOverlap = null, Spr? costUnsatisfiedIcon = null, Spr? costSatisfiedIcon = null, int? iconWidth = null, CustomCostTooltipProvider? customTooltipProvider = null);
            IActionCost Cost(IResource resource, int amount = 1, int? iconOverlap = null, CustomCostTooltipProvider? customTooltipProvider = null);

            IResource StatusResource(Status status, Spr costUnsatisfiedIcon, Spr costSatisfiedIcon, int? iconWidth = null);
            IResource StatusResource(Status status, StatusResourceTarget target, Spr costUnsatisfiedIcon, Spr costSatisfiedIcon, int? iconWidth = null);
            IResource EnergyResource();

            public delegate List<Tooltip> CustomCostTooltipProvider(State state, Combat? combat, IReadOnlyList<IResource> potentialResources, int amount);

            public interface IActionCost
            {
                IReadOnlyList<IResource> PotentialResources { get; }
                int ResourceAmount { get; }
                Spr? CostUnsatisfiedIcon { get; }
                Spr? CostSatisfiedIcon { get; }

                void RenderPrefix(G g, ref Vec position, bool isDisabled, bool dontRender)
                    => PotentialResources.FirstOrDefault()?.RenderPrefix(g, ref position, isDisabled, dontRender);

                void RenderSuffix(G g, ref Vec position, bool isDisabled, bool dontRender)
                    => PotentialResources.FirstOrDefault()?.RenderSuffix(g, ref position, isDisabled, dontRender);

                void RenderSingle(G g, ref Vec position, IResource? satisfiedResource, bool isDisabled, bool dontRender);
                List<Tooltip> GetTooltips(State state, Combat? combat) => new();
            }

            public interface IResource
            {
                string ResourceKey { get; }
                Spr? CostUnsatisfiedIcon { get; }
                Spr? CostSatisfiedIcon { get; }

                int GetCurrentResourceAmount(State state, Combat combat);
                void PayResource(State state, Combat combat, int amount);
                void RenderPrefix(G g, ref Vec position, bool isDisabled, bool dontRender) { }
                void RenderSuffix(G g, ref Vec position, bool isDisabled, bool dontRender) { }
                void Render(G g, ref Vec position, bool isSatisfied, bool isDisabled, bool dontRender);
                List<Tooltip> GetTooltips(State state, Combat? combat, int amount) => [];
            }

            public enum StatusResourceTarget
            {
                Player,
                Enemy,
                EnemyWithOutgoingArrow
            }
        }

        public interface IActionApi
        {
            CardAction MakeExhaustEntireHandImmediate();
            CardAction MakePlaySpecificCardFromAnywhere(int cardId, bool showTheCardIfNotInHand = true);
            CardAction MakePlayRandomCardsFromAnywhere(IEnumerable<int> cardIds, int amount = 1, bool showTheCardIfNotInHand = true);

            CardAction MakeContinue(out Guid id);
            CardAction MakeContinued(Guid id, CardAction action);
            IEnumerable<CardAction> MakeContinued(Guid id, IEnumerable<CardAction> action);
            CardAction MakeStop(out Guid id);
            CardAction MakeStopped(Guid id, CardAction action);
            IEnumerable<CardAction> MakeStopped(Guid id, IEnumerable<CardAction> action);

            CardAction MakeSpoofed(CardAction renderAction, CardAction realAction);
            CardAction MakeHidden(CardAction action, bool showTooltips = false);
            AVariableHint SetTargetPlayer(AVariableHint action, bool targetPlayer);
            AVariableHint MakeEnergyX(AVariableHint? action = null, bool energy = true, int? tooltipOverride = null);
            AStatus MakeEnergy(AStatus action, bool energy = true);

            ACardOffering WithDestination(ACardOffering action, CardDestination? destination, bool? insertRandomly = null);
            CardReward WithDestination(CardReward route, CardDestination? destination, bool? insertRandomly = null);

            List<CardAction> GetWrappedCardActions(CardAction action);
            List<CardAction> GetWrappedCardActionsRecursively(CardAction action);
            List<CardAction> GetWrappedCardActionsRecursively(CardAction action, bool includingWrapperActions);

            void RegisterWrappedActionHook(IWrappedActionHook hook, double priority);
            void UnregisterWrappedActionHook(IWrappedActionHook hook);
        }
    }

    public interface IWrappedActionHook
    {
        List<CardAction>? GetWrappedCardActions(CardAction action);
    }

    public enum DroneShiftHookContext
    {
        Rendering, Action
    }

    public interface IDroneShiftHook
    {
        bool? IsDroneShiftPossible(State state, Combat combat, int direction, DroneShiftHookContext context) => IsDroneShiftPossible(state, combat, context);
        bool? IsDroneShiftPossible(State state, Combat combat, DroneShiftHookContext context) => null;
        void PayForDroneShift(State state, Combat combat, int direction) { }
        void AfterDroneShift(State state, Combat combat, int direction, IDroneShiftHook hook) { }
        List<CardAction>? ProvideDroneShiftActions(State state, Combat combat, int direction) => null;
    }

    public enum EvadeHookContext
    {
        Rendering, Action
    }

    public interface IEvadeHook
    {
        bool? IsEvadePossible(State state, Combat combat, int direction, EvadeHookContext context) => IsEvadePossible(state, combat, context);
        bool? IsEvadePossible(State state, Combat combat, EvadeHookContext context) => null;
        void PayForEvade(State state, Combat combat, int direction) { }
        void AfterEvade(State state, Combat combat, int direction, IEvadeHook hook) { }
        List<CardAction>? ProvideEvadeActions(State state, Combat combat, int direction) => null;
    }
}
