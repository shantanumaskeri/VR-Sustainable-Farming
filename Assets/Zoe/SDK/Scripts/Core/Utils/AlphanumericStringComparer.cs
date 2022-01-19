using System.Collections.Generic;

/// <summary>
/// The code you will find the int Compare(string, string) method has been entirely
/// taken from http://www.dotnetperls.com/alphanumeric-sorting with close to no alterations.
/// The only alterations made are miscallaneous variable renaming for clarity.
/// </summary>
class AlphanumericStringComparer : IComparer<string>
{
    public int Compare(string _string1, string _string2)
    {
        int string1Length = _string1.Length;
        int string2Length = _string2.Length;
        int string1Iterator = 0;
        int string2Iterator = 0;

        while (string1Iterator < string1Length && string2Iterator < string2Length)
        {
            char string1CurrentCharacter = _string1[string1Iterator];
            char string2CurrentCharacter = _string2[string2Iterator];

            // Some buffers we can build up characters in for each chunk.
            char[] string1CharacterChunk = new char[string1Length];
            int string1CharacterChunkIterator = 0;
            char[] string2CharacterChunk = new char[string2Length];
            int string2CharacterChunkIterator = 0;

            // Walk through all following characters that are digits or
            // characters in BOTH strings starting at the appropriate iterator.
            // Collect char arrays.
            do
            {
                string1CharacterChunk[string1CharacterChunkIterator++] = string1CurrentCharacter;
                string1Iterator++;

                if (string1Iterator < string1Length)
                {
                    string1CurrentCharacter = _string1[string1Iterator];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(string1CurrentCharacter) == char.IsDigit(string1CharacterChunk[0]));

            do
            {
                string2CharacterChunk[string2CharacterChunkIterator++] = string2CurrentCharacter;
                string2Iterator++;

                if (string2Iterator < string2Length)
                {
                    string2CurrentCharacter = _string2[string2Iterator];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(string2CurrentCharacter) == char.IsDigit(string2CharacterChunk[0]));

            // If we have collected numbers, compare them numerically.
            // Otherwise, if we have strings, compare them alphabetically.
            string string1Chunk = new string(string1CharacterChunk);
            string string2Chunk = new string(string2CharacterChunk);

            int result;

            if (char.IsDigit(string1CharacterChunk[0]) && char.IsDigit(string2CharacterChunk[0]))
            {
                int string1NumericChunk = int.Parse(string1Chunk);
                int string2NumericChunk = int.Parse(string2Chunk);
                result = string1NumericChunk.CompareTo(string2NumericChunk);
            }
            else
            {
                result = string1Chunk.CompareTo(string2Chunk);
            }

            if (result != 0)
            {
                return result;
            }
        }

        return string1Length - string2Length;
    }
}
