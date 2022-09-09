using UnityEngine;

namespace Mike
{
    public class MikeString : MonoBehaviour
    {
        /// <summary>
        /// This has an error
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertNumberToString(float number = 0)
        {
            string text = number.ToString("F");
            bool coma = false;

            if (text.Contains(",")) { text.Replace(',', '.'); coma = true; }

            if (number >= 1000 && number < 1000000)
            {
                if (text.Contains(".")) text = text.Remove(text.IndexOf("."));
                return text.Remove(text.Length - 3) + "." + text.Remove(0, text.Length - 3).Remove(2) + "K";
            }
            else if (number >= 1000000 && number < 1000000000)
            {
                if (text.Contains(".")) text = text.Remove(text.IndexOf("."));
                return text.Remove(text.Length - 6) + "." + text.Remove(0, text.Length - 6).Remove(2) + "M";
            }
            else if (number >= 1000000000)
            {
                if (text.Contains(".")) text = text.Remove(text.IndexOf("."));
                return text.Remove(text.Length - 9) + "." + text.Remove(0, text.Length - 9).Remove(2) + "B";
            }

            if (coma && text.Contains(".")) { text.Replace('.', ','); }

            return number.ToString();
        }
    }
}
