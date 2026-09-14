namespace EventFlow.Identity.Api.Common
{
    public static class CookieExtensions
    {
        //set tokens in cookies 
        public static void SetAuthCookies(this HttpResponse response,string accessToken, string refreshToken)
        {
            response.Cookies.Append("accessToken", accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

            response.Cookies.Append("refreshToken", refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
        }

        public static void ClearAuthCookies(this HttpResponse response)
        {
            response.Cookies.Delete("accessToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            response.Cookies.Delete("refreshToken",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
        }
    }
}
