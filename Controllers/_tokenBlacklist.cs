    using System;
    using System.Collections.Concurrent;

    namespace JWTAuthenticationForProduct.Controllers
    {
    public interface IJwtBlacklistService
    {
        void BlacklistToken(string token);
        bool IsTokenBlacklisted(string token);
    }

    public class JwtBlacklistService : IJwtBlacklistService
    {
        private readonly HashSet<string> _blacklistedTokens = new();

        public void BlacklistToken(string token)
        {
            _blacklistedTokens.Add(token);
        }

        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.Contains(token);
        }
    }

}


