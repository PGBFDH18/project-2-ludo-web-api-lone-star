using System;

namespace Ludo.GameService
{
    // TODO....
    public class LudoService : ILudoService
    {
        public UserStorage Users { get; } = new UserStorage();
        public GameStorage Games { get; } = new GameStorage();

        //public object Sessions
    }
}
