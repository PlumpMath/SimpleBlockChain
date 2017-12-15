﻿using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Crypto;

namespace SimpleBlockChain.UnitTests.Stores
{
    public static class KeyStore
    {
        public static Key GetGenesisKey()
        {
            return Key.Deserialize(new BigInteger("66661394595692466950200829442443674598224300882267065208709422638481412972116609477112206002430829808784107536250360432119209033266013484787698545014625057"), new BigInteger("43102461949956883352376427470284148089747996528740865531180015053863743793176"));
        }

        public static Key GetClientKey()
        {
            return Key.Deserialize(new BigInteger("55821205064713516294703127430400616105539980828115464481216494737343536494861392791366661233462519462101585894103124424523076975002332234845254777599135465"), new BigInteger("12865140029298721655663530581243123640092469699773563307406591049514067995825"));
        }
    }
}
