using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    [Serializable]
    public class CharacterDataPacket
    {
        /// <summary>
        /// character data exchange
        /// </summary>
        /// <param name="data">base64 encoded character JSON</param>
        public CharacterDataPacket(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
}