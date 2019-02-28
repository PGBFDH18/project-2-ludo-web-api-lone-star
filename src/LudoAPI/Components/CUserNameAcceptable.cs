using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CUserNameAcceptable : IUserNameAcceptable
    {
        //TODO: refactor out local methods to dependency injected components
        public ErrorCode IsUserNameAcceptable(string userName)
        {
            var desc = IsLengthError() ?? HasIllegalCharError() ?? IsReservedError();
            return desc == null ? default : new ErrorCode(ErrorCodes.E06BadUserName, desc);

            string IsLengthError()
            {
                const int MIN_LEN = 3;
                const uint MAX_LEN = 15;
                return unchecked((uint)(userName.Length - MIN_LEN) > (MAX_LEN - MIN_LEN))
                    ? $"Min length {MIN_LEN}; Max length {MAX_LEN}"
                    : null;
            }

            string HasIllegalCharError()
            {
                //FIXME/TODO
                return userName.Contains(' ')
                    ? "Contains illegal character." //TODO: describe legal characters!
                    : null;
            }

            string IsReservedError()
            {
                //TODO
                return userName == "test" // reserved for testing
                    ? "Username reserved. Please pick another."
                    : null;
            }
        }
    }
}
