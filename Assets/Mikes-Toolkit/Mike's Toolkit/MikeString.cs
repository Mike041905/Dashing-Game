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
            if(number < 1000 || number > (numberSuffixes.Length + 1) * 1000000) { return number.ToString("0.##"); }

            string text = number.ToString("G20");
            bool coma = false;

            if (text.Contains(",")) { text = text.Replace(',', '.'); coma = true; }

            for (int i = 1; i < numberSuffixes.Length + 1; i++)
            {

                string suffix = numberSuffixes[i - 1];

                if (number >= i * 1000 && number < i * 1000000)
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
