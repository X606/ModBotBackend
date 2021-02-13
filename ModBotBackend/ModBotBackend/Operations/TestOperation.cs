using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("test")]
    public class TestOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        static int i = 1;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "text/html";
            i++;
            //context.Response.Redirect("https://clonedronemodbot.com/error.html?error=test works&notError=true");
            return "<head><title>Test</title></head><body><div style=\"height: 100px; background-color: red; margin-left: auto; margin-right: auto; margin-top: auto; margin-bottom: auto; width:" + i + "em;\"></div></body>";
        }

    }
}
