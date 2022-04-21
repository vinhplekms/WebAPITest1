namespace WebAPITest2.Helpers
{
    public interface ICookieUtils
    {
        public void SetTookenCookie(string token, string nameToken, int expireDay, HttpResponse  response);
    }
}
