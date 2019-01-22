using System.Net.Sockets;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    // https://stackoverflow.com/a/46805801/7557790
    public static class SocketExtensions
    {
        private const int BytesPerLong = 4; // 32 / 8
        private const int BitsPerByte = 8;

        public static bool IsConnected(this Socket socket)
        {
            try
            {
                bool part1 = socket.Poll(5000, SelectMode.SelectRead);
                bool part2 = (socket.Available == 0);
                if (part1 && part2 || !socket.Connected)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}