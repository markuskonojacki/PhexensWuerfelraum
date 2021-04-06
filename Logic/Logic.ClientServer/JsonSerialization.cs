﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SimpleSockets.Messaging.Serialization;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public class JsonSerialization : IObjectSerializer
    {
        public T DeserializeJson<T>(byte[] bytes)
        {
            if (bytes.Length == 0 || bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public object DeserializeBytesToObject(byte[] bytes, Type objType)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), objType);
        }

        public byte[] SerializeObjectToBytes(object anySerializableObject)
        {
            string jsonObject = Regex.Unescape(JsonConvert.SerializeObject(anySerializableObject, Formatting.Indented));
            return Encoding.UTF8.GetBytes(jsonObject);
        }
    }
}