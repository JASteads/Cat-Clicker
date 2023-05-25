public static class BitNotation
{
    const double MILLION = 1.000e6;

    /* To_Bit_Notation() : Condenses value into custom
     *                     variant of scientific notation */
    public static string ToBitNotation(double val)
    {
        if (val >= MILLION)
        {
            string numStr, suffixStr;
            float remainder;
            
            // Convert val into string with four decimal points
            numStr = val.ToString("e5");

            float.TryParse(numStr.Substring(0, 7),
                out float prefix); // Number
            int.TryParse(numStr.Substring(numStr.Length - 3),
                out int suffixNum); // Tail


            remainder = suffixNum * 0.333f;
            while (remainder > 1) --remainder;
            if ((int)(remainder + 0.2f) != 1)
                for (int i = 0; i < (int)((remainder * 3) + 0.5f); i++)
                    prefix *= 10;

            // Returns the appropriate suffix based on
            // suffixNum's value
            suffixStr = GetSuffix(suffixNum);

            // Concatenate both num_str and suffix_str
            numStr = $"{prefix:##0.000} {suffixStr}";
            return numStr;
        }
        return val.ToString("#,0.#");
    }

    /* Get_Suffix() : If exoponent would display a value over 1M,
     *                return custom suffix. */
    static string GetSuffix(int suffixNum)
    {
        if (suffixNum > 5)
        {
            if (suffixNum < 9)  return "million";
            if (suffixNum < 12) return "billion";
            if (suffixNum < 15) return "trillion";
            if (suffixNum < 18) return "quadillion";
            if (suffixNum < 21) return "quintillion";
            if (suffixNum < 24) return "sextillion";
            if (suffixNum < 27) return "septillion";
            if (suffixNum < 30) return "octillion";
            if (suffixNum < 33) return "nonillion";
            if (suffixNum < 36) return "decillion";
            if (suffixNum < 39) return "undecillion";
            if (suffixNum < 42) return "duodecillion";
            if (suffixNum < 45) return "tredecillion";
            if (suffixNum < 48) return "quattuordecillion";
            if (suffixNum < 51) return "quindecillion";
            if (suffixNum < 54) return "sexdecillion";
            if (suffixNum < 57) return "septendecillion";
            if (suffixNum < 60) return "octodecillion";
            if (suffixNum < 63) return "novemdecillion";
            if (suffixNum < 66) return "vigintillion";
            return "gigantillion";
        }

        return "";
    }
}