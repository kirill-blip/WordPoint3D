using System;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static void Shuffle<T>(this List<T> list, int times)
    {
        for (int i = 0; i < times; i++)
        {
            list.Shuffle();
        }
    }

    public static void Shuffle<T>(this List<T> list)
    {
        Random random = new Random();
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void SwapElements<T>(this List<T> list, int index1, int index2)
    {
        T temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
}
