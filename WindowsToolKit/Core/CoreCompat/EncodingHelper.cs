#if !NET40 && !NET45
using System.Text;
#endif

namespace WindowsToolKit.CoreCompat
{
    internal static class EncodingHelper
    {
        private static bool _registered;

        public static void RegisterEncodings()
        {
            if (_registered)
                return;

            _registered = true;

#if !NET40 && !NET45
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }
    }
}