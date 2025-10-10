/********************************************************************
created:    2023-06-12
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

using System;
using System.Text;
using UnityEngine;

namespace Unicorn.Road
{
    public class JsonSerde : ISerde
    {
        public Chunk<byte> Serialize(object input)
        {
            if (null != input)
            {
                if (input is byte[] buffer)
                {
                    return new Chunk<byte>
                    {
                        Items = buffer,
                        Size = buffer.Length
                    };
                }

                var text = JsonUtility.ToJson(input);
                var bytes = Encoding.UTF8.GetBytes(text);
                return new Chunk<byte>
                {
                    Items = bytes,
                    Size = bytes.Length
                };
            }

            return new Chunk<byte> { Items = Array.Empty<byte>() };
        }

        public T Deserialize<T>(Chunk<byte> input) where T : new()
        {
            return (T)Deserialize(input, typeof(T));
        }

        public object Deserialize(Chunk<byte> input, Type type)
        {
            if (input.Size > 0 && type != null)
            {
                var text = Encoding.UTF8.GetString(input.Items);
                return JsonUtility.FromJson(text, type);
            }

            return null;
        }

        public string GetName()
        {
            return "json";
        }
    }
}