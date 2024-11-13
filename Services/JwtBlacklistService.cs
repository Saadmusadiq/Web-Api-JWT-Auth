namespace webapic_.Services
{
    public class JwtBlacklistService
    {
        private readonly HashSet<string> _blacklist = new HashSet<string>();

        public void BlacklistToken(string jti)
        {
            _blacklist.Add(jti);
        }

        public bool IsTokenBlacklisted(string jti)
        {
            return _blacklist.Contains(jti);
        }
    }
}
