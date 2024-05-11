using System.Collections;
using System.Net.NetworkInformation;
using System.Text;

namespace Utilities
{
    public static class Utilites
    {
        public static BitArray ToBits(this byte data)
        {
            return new BitArray(new byte[] { data });
        }
        public static BitArray ToBits(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            return new BitArray(data);
        }

        public static string ToHexString(this byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException("byteArray");

            return Convert.ToHexString(byteArray);
        }

        public static string FormattedMessaging(this byte[] byteArray, int messagesInOneLine)
        {
            if (byteArray == null)
                throw new ArgumentNullException("byteArray");
            int count = 0;
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                if (count == messagesInOneLine)
                {
                    hex.Append("\n    ");
                    count = 0;
                }
                hex.AppendFormat("{0:x2}", b);
                count++;
            }
            return hex.ToString().ToUpper();
        }

        public static bool CheckValidHexFormat(this string hexString)
        {
            return hexString.PadOddLengthHex(true).All(c => "0123456789abcdefABCDEF".Contains(c));
        }

        public static string StripBytesToString(this byte[] bytes, int stripAmountFromStart, int messagesInOneLine)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (bytes.Length <= stripAmountFromStart)
            {
                return bytes.FormattedMessaging(messagesInOneLine);
            }

            return bytes.Take(stripAmountFromStart).ToArray().FormattedMessaging(messagesInOneLine);
        }

        public static bool PortIsOccupied(int port, string ip)
        {
            var ips = IPGlobalProperties.GetIPGlobalProperties();
            var UdpListners = ips.GetActiveUdpListeners();
            if (UdpListners.Any(c => c.Port == port && c.Address.ToString() == ip))
            {
                return true;
            }
            return false;
        }

        public static string GetDescription(this Enum genericEnum)
        {
            Type genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var attribute = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribute).Description;
                }
            }
            return genericEnum.ToString();
        }

        public static ushort HexToUShort(this string hexString)
        {
            return Convert.ToUInt16(hexString.PadOddLengthHex(), 16);
        }

        public static string PadOddLengthHex(this string hexString, bool strip = false)
        {
            if (hexString == null)
                throw new ArgumentNullException(nameof(hexString));

            if (hexString.Length % 2 != 0)
            {
                if (hexString.StartsWith("0x") || hexString.StartsWith("0X"))
                {
                    if (strip)
                        hexString = "0" + hexString[2..];
                    else
                        hexString = "0x0" + hexString[2..];
                }
                else
                {
                    hexString = "0" + hexString;
                }
            }
            else
            {
                if ((hexString.StartsWith("0x") || hexString.StartsWith("0X")) && strip)
                    hexString = hexString[2..];
            }

            return hexString;
        }

        public static ushort ToUShort(this byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes[0..2].Reverse().ToArray(), 0);
        }

        public static uint ToUInt(this byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes[0..4].Reverse().ToArray(), 0);
        }

        public static uint HexToUInt(this string hexString)
        {
            return Convert.ToUInt32(hexString.PadOddLengthHex(), 16);
        }

        public static byte[] ToBytes(this uint number)
        {
            return BitConverter.GetBytes(number).Reverse().ToArray();
        }

        public static byte[] ToBytes(this ushort number)
        {
            return BitConverter.GetBytes(number).Reverse().ToArray();
        }

        public static byte[] HexToBytes(this string hexString)
        {
            return Convert.FromHexString(hexString.PadOddLengthHex(true));
        }
    }
}
