/********************************************************************
created:    2025-07-31
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;

namespace Unicorn.Web
{
    public static class JpegTools
    {
        // COM (Comment) Marker
        private static readonly byte[] _comMarker = { 0xFF, 0xFE };

        // COM 段的最大数据负载容量 (65535 - 2字节长度字段)
        private const int MaxCommentPayloadSize = 65533;

        /// <summary>
        /// 将一个自定义的字节数组嵌入到 JPEG 的 COM 注释段中。
        /// </summary>
        /// <param name="originalJpegBytes">原始的 JPEG 字节数组</param>
        /// <param name="payloadBytes">要嵌入的自定义数据字节数组 (例如 JSON 序列化后的数据)</param>
        /// <returns>修改后的 JPEG 字节数组，如果数据过大则返回 null。</returns>
        public static byte[] EmbedData(byte[] originalJpegBytes, byte[] payloadBytes)
        {
            // --- 输入验证 ---
            if (originalJpegBytes == null || originalJpegBytes.Length < 2)
            {
                Logo.Warn("原始 JPEG 数据无效。");
                return null;
            }

            if (payloadBytes == null || payloadBytes.Length == 0)
            {
                Logo.Warn("要嵌入的数据为空，将返回原始 JPEG 数据。");
                return originalJpegBytes;
            }

            if (payloadBytes.Length > MaxCommentPayloadSize)
            {
                Logo.Warn($"数据过大！COM 段最大支持 {MaxCommentPayloadSize} 字节，而你的数据有 {payloadBytes.Length} 字节。");
                return null;
            }

            // --- 构建 COM 数据段 ---
            // COM 段结构: Marker (2b) + Length (2b) + Payload (N b)

            // 1. 计算段长度 = payload 长度 + 2 (用于长度字段本身)
            var segmentLength = payloadBytes.Length + 2;

            using var ms = new MemoryStream();
            // 2. 写入 Marker
            ms.Write(_comMarker, 0, _comMarker.Length);

            // 3. 写入 Length (大端序 Big-endian)
            ms.WriteByte((byte)(segmentLength >> 8)); // 高位字节
            ms.WriteByte((byte)(segmentLength & 0xFF)); // 低位字节

            // 4. 写入我们的数据负载 (Payload)
            ms.Write(payloadBytes, 0, payloadBytes.Length);

            var comSegment = ms.ToArray();

            // --- 将 COM 段注入到原始 JPEG 数据中 ---
            using var finalMs = new MemoryStream();
            // 1. 写入 JPEG 的 SOI 标记 (前2个字节)
            finalMs.Write(originalJpegBytes, 0, 2);

            // 2. 写入我们构建的 COM 段
            finalMs.Write(comSegment, 0, comSegment.Length);

            // 3. 写入原始 JPEG 剩下的部分 (从第2个字节之后开始)
            finalMs.Write(originalJpegBytes, 2, originalJpegBytes.Length - 2);

            // Logo.Info($"成功将 {payloadBytes.Length} 字节的数据嵌入到 COM 段中。");
            return finalMs.ToArray();
        }

        /// <summary>
        /// 从 JPEG 的 COM 段中提取自定义字节数组。
        /// 注意：此方法会返回它找到的第一个符合规范的 COM 段的数据。
        /// </summary>
        /// <param name="modifiedJpegBytes">包含嵌入数据的 JPEG 字节数组</param>
        /// <returns>提取出的数据字节数组，如果未找到则返回 null。</returns>
        public static byte[] ExtractEmbedded(byte[] modifiedJpegBytes)
        {
            if (modifiedJpegBytes == null)
            {
                return null;
            }

            // 从 SOI (FF D8) 之后开始搜索
            // 循环边界减4是为了确保有足够的空间读取 Marker 和 Length
            for (var i = 2; i < modifiedJpegBytes.Length - 4; i++)
            {
                // 寻找 COM Marker (FF FE)
                if (modifiedJpegBytes[i] == _comMarker[0] && modifiedJpegBytes[i + 1] == _comMarker[1])
                {
                    // 读取段长度 (大端序)
                    int segmentLength = (modifiedJpegBytes[i + 2] << 8) | modifiedJpegBytes[i + 3];

                    // 计算数据负载的长度
                    int payloadLength = segmentLength - 2;

                    // --- 合法性检查 ---
                    if (payloadLength < 0) continue; // 长度字段有误，跳过
                    if (i + 4 + payloadLength > modifiedJpegBytes.Length) continue; // 数据段声称的长度超出了文件边界，跳过

                    // --- 提取数据 ---
                    var extractedPayload = new byte[payloadLength];
                    Buffer.BlockCopy(modifiedJpegBytes, i + 4, extractedPayload, 0, payloadLength);

                    // Logo.Info($"成功从 COM 段中提取出 {extractedPayload.Length} 字节的数据。");
                    return extractedPayload;
                }
            }

            Logo.Warn("未在 JPEG 数据中找到包含数据的 COM 段。");
            return null;
        }
    }
}