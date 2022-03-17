using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

class Program
{

    static string fileName = "TotemPack.zip";
    static string newDirName = "totemPlush";
    static string playerName = "";
    
    static void Main(string[] args)
    {
        FileChecks();
        Console.Title = new string("TrxshScripted: Totem Plush Creator");
        Console.WriteLine("Please Put Name Of Player You Want To Turn To Totem Plush! JAVA EDITION ONLY!");
        init();
    }

    static void init()
    {
        //wait for read name
        string name = Console.ReadLine();

        if (name != null && name.Length > 3)
        {
            playerName = name;

            if(IsValidUsername())
            {
                Console.WriteLine("Creating Files For Prequisite");

                //register event
                WebClient client = new WebClient();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(onPackDownloaded);
                client.DownloadFileAsync(new Uri("https://download1496.mediafire.com/wj3gd2jl349g/9s2yyb66z7mw86b/TotemPack.zip"), fileName);
                while (true) ;
            }
            else
            {
                Console.WriteLine("Username Is Not Valid In Database!");
                init();
            }
        }
        else
        {
            Console.WriteLine("Name Must Have A Length Longer Than 3!");
            init();
        }
    }

    static void ExtractPack()
    { 
        try
        {
            Console.WriteLine("Extracting File...");
            ZipFile.ExtractToDirectory(fileName, newDirName + "/");
            Console.WriteLine("Extracted!");
            a();
        }catch(IOException)
        {
            Console.WriteLine("Failed To Extract! Retrying In 5 Seconds...");
            retryextract();
        }
    }

    static void a()
    {
        Console.WriteLine("Waiting...");
        Task.Delay(2000).ContinueWith(t => GetUUID());
    }

    static void b()
    {
        Console.WriteLine("Waiting...");
        Task.Delay(2000).ContinueWith(t => CreateZIP());
    }

    static void c()
    {
        Console.WriteLine("Waiting...");
        Task.Delay(1000).ContinueWith(t => BeginMCMove());
    }

    static void retryextract()
    {
        Console.WriteLine("Wating...");
        Task.Delay(5000).ContinueWith(t => ExtractPack());
    }

    static void GetUUID()
    {
        WebClient client = new WebClient();
        string ret = client.DownloadString("https://mcprofile.net/username/" + playerName + "/");
        string uuid = ret.Split("profile/")[1].Split("/")[0];
        Console.WriteLine("UUID: " + uuid);

        client.DownloadFileCompleted += new AsyncCompletedEventHandler(onPNGDownloaded);
        client.DownloadFileAsync(new Uri("https://crafatar.com/skins/" + uuid), @"totemPlush\assets\minecraft\textures\totem.png");
    }

    static void CreateZIP()
    {
        Console.WriteLine("Creating Zip File...");
        ZipFile.CreateFromDirectory(newDirName, "TrxshScriptedTotem.zip");
        Console.WriteLine("Zip FIle Created!");

        c();
    }

    static void BeginMCMove()
    {
        try
        {
            Console.WriteLine("Moving ZIP To Mc Folder...");
            File.Move("TrxshScriptedTotem.zip", GetMC() + "TrxshScriptedTotem.zip");
            Console.WriteLine("Moved!");
            finish();
        }
        catch(IOException e)
        {
            failed(e);
        }
    }

    static void finish()
    {
        Console.Clear();
        Console.Beep();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!!");
        Console.WriteLine("Run Minecraft And Set The Texture Pack At Highest Priority 'TrxshScriptedTotem'");
    }

    static void failed(object stacktrace)
    {
        Console.Clear();
        Console.Beep();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("The Operation Failed. Please Try Again." + "\n" + stacktrace);
        while (true) ;
    }

    static void FileChecks()
    {
        if (Directory.Exists(newDirName))
        {
            Directory.Delete(newDirName, true);
        }

        if (!Directory.Exists(newDirName))
        {
            Directory.CreateDirectory(newDirName);
        }

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        if(File.Exists("TrxshScriptedTotem.zip"))
        {
            File.Delete("TrxshScriptedTotem.zip");
        }

        if(File.Exists(GetMC() + "TrxshScriptedTotem.zip"))
        {
            try
            {
                File.Delete(GetMC() + "TrxshScriptedTotem.zip");
                Console.WriteLine("Deleted MC Pack.");
            }catch(IOException e)
            {
                failed(new IOException("Failed To Hook To Minecraft Folder! Is Minecraft Currently Running?" + "\n" + "The Application Will Not Run Properly If Minecraft Is Not Closed.") + "\n" + e.Message + "\n" + e.StackTrace);
            }
        }
    }

    static void onPackDownloaded(object obj, AsyncCompletedEventArgs args)
    {
        Console.WriteLine("File Downloaded Sucessfully!");
        retryextract();
    }

    static void onPNGDownloaded(object obj, AsyncCompletedEventArgs args)
    {
        Console.WriteLine("Player Skin Downloaded!");
        b();
    }

    static string GetMC()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\resourcepacks\";
    }

    static bool IsValidUsername()
    {
        WebClient client = new WebClient();
        string ret = client.DownloadString("https://mcprofile.net/username/" + playerName + "/");

        if(ret.Contains("not available"))
        {
            return true;
        }

        return false;
    }
}
