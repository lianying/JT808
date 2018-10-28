﻿using JT808.Protocol.Extensions;
using JT808.Protocol.JT808Properties;
using JT808.Protocol.MessageBody;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace JT808.Protocol.JT808Formatters.MessageBodyFormatters
{
    public class JT808_0x8301Formatter : IJT808Formatter<JT808_0x8301>
    {
        public JT808_0x8301 Deserialize(ReadOnlySpan<byte> bytes, out int readSize)
        {
            int offset = 0;
            JT808_0x8301 jT808_0X8301 = new JT808_0x8301();
            jT808_0X8301.SettingType = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
            jT808_0X8301.SettingCount = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
            jT808_0X8301.EventItems = new List<JT808EventProperty>();
            for(var i=0;i< jT808_0X8301.SettingCount; i++)
            {
                JT808EventProperty jT808EventProperty = new JT808EventProperty();
                jT808EventProperty.EventId= JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808EventProperty.EventContentLength = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808EventProperty.EventContent = JT808BinaryExtensions.ReadStringLittle(bytes, ref offset, jT808EventProperty.EventContentLength);
                jT808_0X8301.EventItems.Add(jT808EventProperty);
            }
            readSize = offset;
            return jT808_0X8301;
        }

        public int Serialize(IMemoryOwner<byte> memoryOwner, int offset, JT808_0x8301 value)
        {
            offset += JT808BinaryExtensions.WriteByteLittle(memoryOwner, offset, value.SettingType);
            if(value.EventItems!=null && value.EventItems.Count > 0)
            {
                offset += JT808BinaryExtensions.WriteByteLittle(memoryOwner, offset, (byte)value.EventItems.Count);
                foreach(var item in value.EventItems)
                {
                    offset += JT808BinaryExtensions.WriteByteLittle(memoryOwner, offset, item.EventId);
                    // 先计算内容长度（汉字为两个字节）
                    offset += 1;
                    int byteLength = JT808BinaryExtensions.WriteStringLittle(memoryOwner, offset, item.EventContent);
                    JT808BinaryExtensions.WriteByteLittle(memoryOwner, offset - 1, (byte)byteLength);
                    offset += byteLength;
                }
            }
            return offset;
        }
    }
}