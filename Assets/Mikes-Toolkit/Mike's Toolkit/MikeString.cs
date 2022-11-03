using UnityEngine;

namespace Mike
{
    public class MikeString : MonoBehaviour
    {
        readonly static string[] numberSuffixes = { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "De", "Ud", "Dd", "Td", "Sxd", "Spd", "Ocd", "Nod", "Vg" };

        /// <summary>
        /// Uses short scale and goes up vigintillion (Vg [10^63])
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertNumberToString(double number = 0)
        {
            if(number < 1000) { return number.ToString("0.##"); }
            if(number > Mathf.Pow(1000000, numberSuffixes.Length)) { return number.ToString("E"); }

            string text = number.ToString("G99");
            bool coma = false;

            if (text.Contains(",")) { text = text.Replace(',', '.'); coma = true; }

            for (int i = 1; i < numberSuffixes.Length + 1; i++)
            {
                string suffix = numberSuffixes[i - 1];

                if (number >= 1000 * Mathf.Pow(1000, i - 1) && number < 1000000 * Mathf.Pow(1000, i - 1))
                {
                    if (text.Contains(".")) text = text.Remove(text.IndexOf("."));
                    return text.Remove(text.Length - i * 3) + "." + text.Remove(0, text.Length - i * 3).Remove(2) + suffix;
                }
            }

            if (coma && text.Contains(".")) { text.Replace('.', ','); }

            return text.ToString();
        }
    }
}
