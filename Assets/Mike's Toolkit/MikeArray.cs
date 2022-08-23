namespace Mike
{
    class MikeArray
    {
        public static T[] Append<T>(T[] array, T element)
        {
            T[] temp = array;
            array = new T[array.Length + 1];
            temp.CopyTo(array, 0);
            array[array.Length - 1] = element;

            return array;
        }
    }
}