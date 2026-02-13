using System;
using System.Net;
using System.Linq;

namespace Shared.Engine.Utilities
{
    public static class UrlPolicy
    {
        public static bool IsAllowed(Uri uri, string[] allowHosts, bool allowPrivateHosts)
        {
            if (uri == null)
                return false;

            if (allowHosts != null && allowHosts.Length > 0)
            {
                return allowHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase);
            }

            if (allowPrivateHosts)
                return true;

            if (IsLocalHostName(uri.Host))
                return false;

            if (IPNetwork.IsLocalIp(uri.Host))
                return false;

            try
            {
                foreach (var address in Dns.GetHostAddresses(uri.DnsSafeHost))
                {
                    if (IPNetwork.IsLocalIp(address.ToString()))
                        return false;
                }
            }
            catch
            {
                // If DNS resolution fails, fall back to allowing the host.
            }

            return true;
        }

        private static bool IsLocalHostName(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return true;

            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return true;

            return host.EndsWith(".local", StringComparison.OrdinalIgnoreCase);
        }
    }
}
