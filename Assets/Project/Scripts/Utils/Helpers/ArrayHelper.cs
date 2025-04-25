using System;
using UnityEngine;

namespace Project.Utils
{
    public static class ArrayHelper
    {
        public static void AddToArray<t>(ref t[] array, t element)
        {
            int arrayLength = array.Length;
            Array.Resize(ref array, arrayLength + 1);
            array[arrayLength] = element;
        }

        public static void RemoveFromArray<t>(ref t[] array, t element)
        {
            int i, arrayLength = array.Length, lastElement = arrayLength - 1;
            for (i = 0; i < arrayLength; ++i) if (array[i].Equals(element)) break;
            if (i < lastElement) Array.Copy(array, i + 1, array, i, lastElement - i);
            if (i < arrayLength) Array.Resize(ref array, lastElement);
        }

        public static void InsertIntoArray<t>(ref t[] array, t element, int position)
        {
            if (array.Length > 0) {
                int clampedPosition = Mathf.Clamp(position, 0, array.Length);
                Array.Resize(ref array, array.Length + 1);
                Array.Copy(array, clampedPosition, array, clampedPosition + 1, array.Length - clampedPosition - 1);
                array[clampedPosition] = element;
            } else AddToArray(ref array, element);
        }

        public static void CopyArray<t>(in t[] original, out t[] target) where t : ICloneable
        {
            target = new t[original.Length];
            for (int i = 0; i < original.Length; ++i) {
                if (original[i] == null) continue;
                target[i] = (t)original[i].Clone();
            }
        }

        public static t[] ArrayFilled<t>(int count, t element) where t : ICloneable
        {
            t[] array = new t[count];
            if (element == null) Array.Fill(array, element);
            else for (int i = 0; i < count; ++i) array[i] = (t)element.Clone();
            return array;
        }
    }
}