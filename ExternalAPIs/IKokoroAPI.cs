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
    }

    public enum DroneShiftHookContext
    {
        Rendering, Action
    }

    public interface IDroneShiftHook
    {
        bool? IsDroneShiftPossible(State state, Combat combat, DroneShiftHookContext context) => null;
        void AfterDroneShift(State state, Combat combat, int direction, IDroneShiftHook hook) { }
    }
}
