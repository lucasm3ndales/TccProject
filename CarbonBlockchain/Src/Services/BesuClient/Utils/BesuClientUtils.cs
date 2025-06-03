using System.Numerics;
using System.Text;
using Nethereum.Util;

namespace CarbonBlockchain.Services.BesuClient.Utils;

public static class BesuClientUtils
{
    public static BigInteger GenerateTokenIdFromCreditCode(string creditCode)
    {
        var encoded = Encoding.UTF8.GetBytes(creditCode);
        var sha3 = new Sha3Keccack();
        var hash = sha3.CalculateHash(encoded);
        return new BigInteger(hash, isUnsigned: true, isBigEndian: true);
    }
}