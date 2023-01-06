using System;
using System.Collections.Generic;
using System.Linq;

public class RecursiveFileSearch
{
    static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
    public static void WalkDirectoryTree(System.IO.DirectoryInfo root)
    {
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        // First, process all the files directly under this folder
        try
        {
            subDirs = root.GetDirectories();
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (subDirs != null && subDirs.Count() != 0)
        {
            foreach (System.IO.DirectoryInfo di in subDirs)
            {
                WalkDirectoryTree(di);
            }
        }
        else {
            files = root.GetFiles();
            foreach (System.IO.FileInfo fi in files)
            {
                System.Console.WriteLine(fi.FullName);
            }
        }
    }

    public static void WalkDirectoryTree(System.IO.DirectoryInfo root, string search_pattern)
    {
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        // First, process all the files directly under this folder
        try
        {
            subDirs = root.GetDirectories();
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (subDirs != null && subDirs.Count() != 0)
        {
            foreach (System.IO.DirectoryInfo di in subDirs)
            {
                WalkDirectoryTree(di,search_pattern);
            }
        }
        else {
            files = root.GetFiles("*" + search_pattern + "*");
            foreach (System.IO.FileInfo fi in files)
            {
                System.Console.WriteLine(fi.Name);
            }
        }
    }

    public static void WalkDirectoryTreeString(System.IO.DirectoryInfo root, string search_pattern, List<string> results)
    {
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;
        List<string> new_results = new List<string>();

        // First, process all the files directly under this folder
        try
        {
            subDirs = root.GetDirectories();
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (subDirs != null && subDirs.Count() != 0)
        {
            foreach (System.IO.DirectoryInfo di in subDirs)
            {
                WalkDirectoryTreeString(di,search_pattern, results);
            }
        }
        else {
            files = root.GetFiles("*" + search_pattern + "*");
            foreach(System.IO.FileInfo fi in files)
            {
                if(!fi.FullName.Contains("jpg.import") && !fi.FullName.Contains("jpeg.import"))
                    results.Add(fi.FullName);
            }
        }
    }
}