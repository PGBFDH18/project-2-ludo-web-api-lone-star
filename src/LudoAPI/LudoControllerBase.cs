namespace Ludo.WebAPI
{
    public abstract class LudoControllerBase : ExtendedControllerBase
    {
        protected LudoControllerBase() { }

        //TODO: refactor out to a dependency injected component
        protected bool IsValidGameId(string gameId)
        {
            //TODO
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        protected bool IsValidPlayerId(string player)
        {
            //TODO
            return player != "test"; // just for experimentation
        }
    }
}
