namespace Ludo.API.Service
{
    public static class Factory
    {
        public static ILudoService Create()
            => new LudoService();
    }
}
