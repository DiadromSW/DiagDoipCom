using DiagCom.Uds.Model.DiagnosticSequences;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DiagCom.Commands.Coordination
{
    public class LocalParser : ILocalParser
    {
        private readonly ILogger<LocalParser> _logger;

        public LocalParser(ILogger<LocalParser> logger)
        {
            _logger = logger;
        }

        public List<ServiceResult> ParseRawResults(IDiagnosticSequence sequence)
        {
            var results = new List<ServiceResult>();
            if (sequence is DiagnosticSequence diagSeq)
            {
                try
                {
                    foreach (var service in diagSeq.Sequence)
                    {
                        results.Add(ParseRawResult(service));
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Unexpected error parsing diagnostic sequence results.");
                }
            }
            return results;
        }

        public ServiceResult ParseRawResult(DiagnosticService service)
        {
            if (service.Service == "22" && service.ParsingData?.Parameter != null)
            {
                return Parse_Did(service.RawResult, service.ParsingData.Parameter);
            }
            else if (service.Service == "31" && service.ParsingData?.Parameter != null)
            {
                return Parse_control_routine(service.RawResult, service.ParsingData.Parameter);
            }

            _logger.LogDebug($"No parsing of service {service.Service}.");
            return new ServiceResult();
        }

        /*
        Parses a did that is defined in the sequence object. This is done by looping thru each parameter.
        Extracting the payload. And based on the defined data_type it will perform the parsing.

        Args:
        sequence_obj (DiagnosticSequenceObject): A DiagnosticSequenceObject.

        Returns:
        list[ParsedResult]: A list of results for each parameter in the DID.
        */
        private ServiceResult Parse_Did(List<string> rawResult, List<Parameter> parameters)
        {
            _logger.LogInformation($"Parsing raw result {string.Join(", ", rawResult)} ...");

            var ret = new ServiceResult();
            ret.RawData = rawResult;
            var payload = Extract_did_payload(rawResult, out var error);

            string binary_payload = ResultPayloadToBinaryPayload(payload);
            foreach (var parameter in parameters)
            {
                IParsedResult parsed_result = new ParsedParameterResult(parameterId: parameter.Identifier, parsedOk: false, value: null);

                if (error != null)
                {
                    SetError("Parse DID failure:", error, parsed_result);
                }
                else
                {
                    parsed_result = ParseParameter(payload, binary_payload, parameter, parsed_result);
                }

                ret.ParametersResult.Add(parsed_result);
            }
            return ret;
        }

        /*
        Parses a control_routine that is defined in the sequence object. This is done by looping thru each subroutine.
        Extracting the payload. And based on the defined data_type it will perform the parsing.

        Args:
        sequence_obj (DiagnosticSequenceObject): A DiagnosticSequenceObject.

        Returns:
        list[ParsedResult]: A list of results for each subroutine in the control routine.
        */
        private ServiceResult Parse_control_routine(List<string> rawResult, List<Parameter> subroutines)
        {

            _logger.LogInformation($"Parsing raw result {string.Join(", ", rawResult)} ...");

            var ret = new ServiceResult();
            ret.RawData = rawResult;
            var payload = Extract_control_routine_payload(rawResult, out var error);

            var binary_payload = ResultPayloadToBinaryPayload(payload);
            foreach (var subroutine in subroutines)
            {
                IParsedResult parsed_result = new ParsedParameterResult(parameterId: subroutine.Identifier, parsedOk: false, value: null);

                if (error != null)
                {
                    SetError("Parse RID failure:", error, parsed_result);
                }
                else
                {
                    parsed_result = ParseSubRoutine(payload, binary_payload, subroutine, parsed_result);
                }

                ret.ParametersResult.Add(parsed_result);
            }
            return ret;
        }
        private IParsedResult ParseParameter(string payload, string binary_payload, Parameter parameter, IParsedResult parsed_result)
        {
            var dataType = parameter.DataType.ToLower();
            var byteOffset = Convert.ToInt32(parameter.OffsetBits) / 4;
            var byteTotal = Convert.ToInt32(parameter.TotalBits) / 4;
            var bitOffset = Convert.ToInt32(parameter.OffsetBits);
            var bitTotal = Convert.ToInt32(parameter.TotalBits);
            var byteCount = Math.Min(byteTotal, payload.Length - byteOffset);

            var byteParamPayload = payload.Substring(byteOffset, byteCount);
            var bitCount = Math.Min(bitTotal, binary_payload.Length - bitOffset);
            var bitParamPayload = binary_payload.Substring(bitOffset, bitCount);

            parsed_result = Parse_Payload(dataType, byteParamPayload, bitParamPayload, parsed_result);
            _logger.LogInformation($"Parsed DID result. {parsed_result}.");
            return parsed_result;
        }
        private IParsedResult ParseSubRoutine(string payload, string binary_payload, Parameter subroutine, IParsedResult parsed_result)
        {
            var dataType = subroutine.DataType.ToLower();
            var byteOffset = Convert.ToInt32(subroutine.OffsetBits) / 4;
            var byteTotal = Convert.ToInt32(subroutine.TotalBits) / 4;
            var bitOffset = Convert.ToInt32(subroutine.OffsetBits);
            var bitTotal = Convert.ToInt32(subroutine.TotalBits);
            var byteCount = Math.Min(byteTotal, payload.Length - byteOffset);
            var byteRoutinePayload = payload.Substring(byteOffset, byteCount);
            var bitCount = Math.Min(bitTotal, binary_payload.Length - bitOffset);
            var bitRoutinePayload = binary_payload.Substring(bitOffset, bitCount);

            parsed_result = Parse_Payload(dataType, byteRoutinePayload, bitRoutinePayload, parsed_result);
            _logger.LogInformation($"Parsed RID result. {parsed_result}.");
            return parsed_result;
        }
        private void SetError(string message, string error, IParsedResult parsed_result)
        {
            parsed_result.Value = error;
            _logger.LogError($"{message} {error}");
        }
        private string ResultPayloadToBinaryPayload(string payload)
        {
            return string.Join(string.Empty, payload.Select(
                    c => Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0')
                  )
                );

        }

        private IParsedResult Parse_Payload(string data_type, string byte_payload, string bit_payload, IParsedResult parsed_result)
        {
            try
            {
                switch (data_type)
                {
                    case "ascii":
                        parsed_result.Value = ParseAscii(byte_payload);
                        parsed_result.ParsedOk = true;
                        break;
                    case "bcd+ascii":
                        parsed_result.Value = Parse_bcdascii(byte_payload);
                        parsed_result.ParsedOk = true;
                        break;
                    case "bcd":
                        parsed_result.Value = Validate_bcd(byte_payload);
                        parsed_result.ParsedOk = true;
                        break;
                    case "hex":
                        parsed_result.Value = Validate_hex(byte_payload);
                        parsed_result.ParsedOk = true;
                        break;
                    case "signed":
                    case "unsigned":
                        parsed_result.Value = Convert.ToString(Parse_decimal(bit_payload, data_type == "signed"));
                        parsed_result.ParsedOk = true;
                        break;
                    default:
                        _logger.LogError($"Parsing of data type {data_type} in sequence is not supported.");
                        break;
                }
            }
            catch (Exception e)
            {
                parsed_result.Value = e.Message;
            }

            return parsed_result;
        }

        private static string ParseAscii(string payload)
        {
            var bytes = Convert.FromHexString(payload);
            return Encoding.ASCII.GetString(bytes);
        }

        /*
        Extracts a DIDs payload. The first 5 bytes are response information:
        =>14010E806241F900004A0000
        ==>1401 = ECU Address
        ==>0E80 = TESTER Address
        ==>62 = service response (22 + 40)
        ==>41F9 = DID
        ==>00004A0000 = Payload

        Args:
            raw_result (list[str]): A list of all responses as hex strings.

        Returns:
            str: The first valid response found. (Larger then 5 bytes, otherwise it's an NRC)
        */
        private static string Extract_did_payload(List<string> raw_result, out string error)
        {
            error = null;
            string ret = "";
            byte nrc = 0;

            foreach (var res in raw_result)
            {
                var result = res.ToUpper();
                if (result.Length > 8 && result.Substring(4, 4) == "0E80")
                    result = result.Substring(8);
                else
                    // Ignore unrecognized message.
                    continue;

                if (result.Length > 6 && result.Substring(0, 2) == "62")
                    ret = result.Substring(6);
                else if (result.Length == 6 && result.Substring(0, 4) == "7F22")
                    nrc = byte.Parse(result.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }

            if (string.IsNullOrEmpty(ret))
            {
                if (nrc != 0)
                {
                    error = "Negative response";
                }
                else
                {
                    error = "Timeout";
                }
            }

            return ret;
        }

        /*
        Extracts a DIDs payload. The first 5 bytes are response information:
        =>14010E80710102061001
        ==>1401 = ECU Address
        ==>0E80 = TESTER Address
        ==>71 = service response (31 + 40)
        ==>01 = routine type
        ==>0206 = control routine
        ==>1001 = Payload

        Args:
            raw_result (list[str]): A list of all responses as hex strings.

        Returns:
        str: The first valid response found. (Larger then 5 bytes, otherwise it's an NRC)
        */
        private static string Extract_control_routine_payload(List<string> raw_result, out string error)
        {
            error = null;
            string ret = "";
            byte nrc = 0;

            foreach (var res in raw_result)
            {
                var result = res.ToUpper();
                if (result.Length > 8 && result.Substring(4, 4) == "0E80")
                    result = result.Substring(8);
                else
                    // Ignore unrecognized message.
                    continue;

                if (result.Length > 8 && result.Substring(0, 2) == "71")
                    ret = result.Substring(8);
                else if (result.Length == 6 && result.Substring(0, 4) == "7F31")
                    nrc = byte.Parse(result.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }

            if (string.IsNullOrEmpty(ret))
            {
                if (nrc != 0)
                {
                    error = "Negative response";
                }
                else
                {
                    error = "Timeout";
                }
            }

            return ret;
        }

        /*
        Parses decimals and performs twos complement if the input should be signed or not.

        Args:
        binary (bin): The response in binary
        signed (bool, optional): If the conversion should be signed. Defaults to False.

        Returns:
        int: binary value converted to integer (that might be signed based on input)
        */
        private static int Parse_decimal(string binary, bool signed = false)
        {
            var ret = Convert.ToInt32(binary, 2);
            if (signed)
                ret = Twos_complement(ret, binary.Length);
            return ret;
        }

        /*
        Twos complement helper method.

        Args:
        value (int): An unsigned integer
        bits (int): Number of bits.

        Returns:
        int: A signed in 
        */
        private static int Twos_complement(int value, int bits)
        {
            if (bits < 0)
            {
                throw new Exception("Negative shift count not allowed.");
            }
            if ((value & 1 << bits - 1) != 0)
            {
                value -= 1 << bits;
            }
            return value;
        }

        /*
        Validates the hex string

        If a character in the hex string is not valid (between 0-F) the conversion error will be thrown.

        Args:
        hex (str): A hex string that should be of hex format.

        Returns:
        str: The original hex string.
        */
        private static string Validate_hex(string hex)
        {
            Convert.ToInt64(hex, 16);
            return hex;
        }

        /*
        Validates the BCD code.

        If a character in the hex string is not a valid (between 0-9) the conversion error will be thrown.

        Args:
        hex (str): A hex string that should be of BCD format.

        Returns:
        str: The original hex string.
        */
        private static string Validate_bcd(string hex)
        {
            Convert.ToInt64(hex, 10);
            return hex;
        }

        /*
        Handles responses of type BCD+Ascii.

        Args:
        hex (str): The response as hexstring.

        Raises:
        Exception: If the length of the hex string is not as expected.

        Returns:
        str: The parsed hex string as BCD+ASCII
        */
        private static string Parse_bcdascii(string hex)
        {
            var ret = "";
            if (hex.Length % 2 > 0)
                throw new Exception("Invalid number of bytes");

            var i = 0;
            var split_found = false;
            while (i < hex.Length)
            {
                var temp = "";
                var byte_ = hex.Substring(i, 2);

                if (!split_found)
                    temp = Validate_bcd(byte_);
                if (temp == "20" || split_found)
                {
                    split_found = true;
                    temp = ParseAscii(byte_);
                }
                ret += temp;
                i += 2;
            }
            return ret;
        }
    }
}