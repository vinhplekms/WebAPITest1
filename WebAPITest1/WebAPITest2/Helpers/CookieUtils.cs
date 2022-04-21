namespace WebAPITest2.Helpers
{
    public class CookieUtils : ICookieUtils
    {

        public void SetTookenCookie(string token, string nameToken, int expireDay, HttpResponse response)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(expireDay)
            };
            response.Cookies.Append(nameToken, token, cookieOptions);
        }
    }
}
