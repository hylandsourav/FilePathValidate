using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FilePathValidate;

public static class FileAndPathNameIllegalCharUtility
{
    ////OnBase defined illegal characters and replacements
    private static readonly Dictionary<char, char> OB_SWAP_CHARS = new Dictionary<char, char>{
                {'\\', '-'},
                {'/', '-'},
                {':', ';'},
                {'*', '+'},
                {'?', '!'},
                {'\"', '\''},
                {'<', '['},
                {'>', ']'},
                {'|', '!'},
            };



    //URI illegal characters.
    private static readonly char[] URI_ILLEGAL_CHARS = { '@', '"', '$', '&', ':', '<', '>', '{', '}', '[', ']', '#', '%', '/', ';', '=', '?', '\\', '^', '|', '~', '\'', '\"' };


    /// <summary>
    /// Remove illegal characters from a file name or any string.
    /// </summary>
    /// <param name="fileNameToScrub"></param>
    /// <param name="fileName"></param>
    /// <param name="scrubOptions"></param>
    /// <returns></returns>
    static public string CreateSafeFileName(string fileNameToScrub, out string fileName, IllegalCharacterScrubOptions scrubOptions)
    {

        string _tempFileName = fileNameToScrub;

        if (FlagUtility.IsFlagSet(scrubOptions, IllegalCharacterScrubOptions.ScrubOBIllegalChars))
        {
            _tempFileName = ReplaceOBReservedCharacters(fileNameToScrub);
        }
        if (FlagUtility.IsFlagSet(scrubOptions, IllegalCharacterScrubOptions.ScrubWindowsIllegalChars))
        {
            _tempFileName = ReplaceWindowsIllegalCharacters(_tempFileName);
        }
        if (FlagUtility.IsFlagSet(scrubOptions, IllegalCharacterScrubOptions.ScrubURIIllegalCharacters))
        {
            _tempFileName = ReplaceURIIllegalCharacters(_tempFileName);
        }

        fileName = _tempFileName;

        return fileName;
    }

    static public string CreateSafeFilePath(string filePathToScrub)
    {
        if (string.IsNullOrEmpty(filePathToScrub))
        {
            throw new ArgumentNullException("The provided file path was null or empty.");
        }

        // Separate the file path and filename
        string directory = Path.GetDirectoryName(filePathToScrub);
        string filename = Path.GetFileName(filePathToScrub);
        
        // Ensure directory and filename are not null
        if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException("The provided file path is not valid.");
        }

        // Replace invalid characters in the file path
        string canonicalDirectory = ReplaceInvalidCharactersFromFilePath(directory);

        // Replace URI illegal characters in the filename
        string sanitizedFileName = CreateSafeFileName(filename, out string safeFileName, IllegalCharacterScrubOptions.All);
        
        string sanitizedFilePath = Path.Combine(canonicalDirectory, sanitizedFileName);
        
        // Ensure there are no path traversal sequences
        if (ContainsPathTraversalSequence(sanitizedFilePath))
        {
            throw new ArgumentException("The provided file path contains path traversal sequences.");
        }

        return sanitizedFilePath;
    }


    static private string ReplaceOBReservedCharacters(string nameToScrub)
    {
        StringBuilder safeName = new StringBuilder(nameToScrub);

        foreach (char c in OB_SWAP_CHARS.Keys)
        {
            safeName.Replace(c, OB_SWAP_CHARS[c]);
        }
        return safeName.ToString();
    }

    static private string ReplaceWindowsIllegalCharacters(string nameToScrub)
    {
        string safeName = nameToScrub;
        int i;
        i = safeName.IndexOfAny(Path.GetInvalidPathChars());
        while (i != -1)
        {
            char toRemoveChar = safeName[i];
            safeName = safeName.Replace(toRemoveChar, '_');
            i = safeName.IndexOfAny(Path.GetInvalidPathChars(), i);
        }
        return safeName;
    }

    static private string ReplaceURIIllegalCharacters(string nameToScrub)
    {
        string safeName = nameToScrub;

        int i = -1;
        i = safeName.IndexOfAny(URI_ILLEGAL_CHARS);
        while (i != -1)
        {
            char toRemoveChar = safeName[i];
            safeName = safeName.Replace(toRemoveChar, '_');
            i = safeName.IndexOfAny(URI_ILLEGAL_CHARS, i);
        }
        return safeName;
    }

    static private string ReplaceInvalidCharactersFromFilePath(string input)
    {
        string sanitizedInput = input;

        // Replace invalid characters with underscores
        foreach (char c in Path.GetInvalidPathChars())
        {
            sanitizedInput = sanitizedInput.Replace(c, '_');
        }

        return sanitizedInput;
    }
    static private bool ContainsPathTraversalSequence(string path)
    {
        // Check for common path traversal sequences
        return path.Contains("..") || path.Contains("%2e%2e") || path.Contains("/..") || path.Contains("\\..");
    }
}

[Flags]
public enum IllegalCharacterScrubOptions
{
    ScrubOBIllegalChars = 1,
    ScrubWindowsIllegalChars = 2,
    ScrubURIIllegalCharacters = 4,
    All = ScrubOBIllegalChars | ScrubWindowsIllegalChars | ScrubURIIllegalCharacters,
}

public class Program
{
    public static void Main()
    {
        // Test cases
        string filePath = @"C:\Users\somaji\Desktop\web.config";

        try
        {
            string safeFileName = FileAndPathNameIllegalCharUtility.CreateSafeFileName(filePath, out string sanitizedFileName, IllegalCharacterScrubOptions.All);
            
            // Create a safe file path
            string safeFilePath = FileAndPathNameIllegalCharUtility.CreateSafeFilePath(filePath);
            Console.WriteLine($"Safe File Path: {safeFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
