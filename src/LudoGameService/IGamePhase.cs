using Ludo.API.Models;

namespace Ludo.API.Service
{
    // I call this a Type Bundling Interface. (Probably not the first to do this, so an official name probably exists.)
    // It's the best approach when you want to store disparate types in a single field with maximum performance:
    // faster than type checks and no casting:
    //  - trying to take action during a specific state > a single null check on the related property.
    //  - need to know what session you have > get the State property.
    // smaller memory footprint:
    //  - a single IGameStateSession field needed per game (as opposed to a separate field per type per game).
    //  - GameState value embedded in the code, i.e. stored once per type as opposed to once per instance.
    // easy thread-safety (a GameState change is a simple interlocked exchange of a single field).
    public interface IGamePhase
    {
        GamePhase Phase { get; }

        // WARNING: Only one of these three are non-null at any given time!
        SetupPhase Setup { get; }
        IngamePhase Ingame { get; }
        FinishedPhase Finished { get; }

        // An array of encoded userIds, in slot order, with null for empty slots.
        ISlotArray Slots { get; }
        // UserId of the winner, or null.
        string Winner { get; }
    }
    // A similar strategy can be used to avoid casting in other situations too.
    // E.g. when you have a child class that implements an interface but you're holding an instance of the base class.
    // It might look a bit odd in the implementing type, but once you see the code in heavy use it looks beutiful...
    // You are actually moving the responsibility of the cast from the would be caster to the castee,
    // and in the castee the type check is simply "return this" - which is a compile time verified.
}
