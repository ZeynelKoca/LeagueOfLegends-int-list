namespace IntList
{
    internal class LockFile
    {
        private string? _filePath;

        public string FilePath
        {
            get
            {
                return _filePath ??= $@"{TryGetFolderPath()}\lockfile";
            }
            set
            {
                _filePath = value;
            }
        }

        private static string TryGetFolderPath()
        {
            if (Directory.Exists(@"C:\Riot Games\League of Legends"))
            {
                return @"C:\Riot Games\League of Legends";
            }
            if (Directory.Exists(@"D:\Riot Games\League of Legends"))
            {
                return @"D:\Riot Games\League of Legends";
            }
            if (Directory.Exists(@"C:\Program Files\Riot Games\League of Legends"))
            {
                return @"C:\Program Files\Riot Games\League of Legends";
            }
            if (Directory.Exists(@"D:\Program Files\Riot Games\League of Legends"))
            {
                return @"D:\Program Files\Riot Games\League of Legends";
            }
            if (Directory.Exists(@"C:\Program Files (x86)\Riot Games\League of Legends"))
            {
                return @"C:\Program Files (x86)\Riot Games\League of Legends";
            }
            if (Directory.Exists(@"D:\Program Files (x86)\Riot Games\League of Legends"))
            {
                return @"D:\Program Files (x86)\Riot Games\League of Legends";
            }
            if (Directory.Exists(@"C:\Program Files\League of Legends"))
            {
                return @"C:\Program Files\League of Legends";
            }
            if (Directory.Exists(@"D:\Program Files\League of Legends"))
            {
                return @"D:\Program Files\League of Legends";
            }
            if (Directory.Exists(@"C:\Program Files (x86)\League of Legends"))
            {
                return @"C:\Program Files (x86)\League of Legends";
            }
            if (Directory.Exists(@"D:\Program Files (x86)\League of Legends"))
            {
                return @"D:\Program Files (x86)\League of Legends";
            }

            return string.Empty;
        }
    }

}
