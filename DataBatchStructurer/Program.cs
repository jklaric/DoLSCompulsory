class  Program
{
    static void Main(string[] args)
    {
        // Path to the data directory
        string dataPath = "../Data/maildir";

        // Check if the data directory exists
        if (!Directory.Exists(dataPath))
        {
            Console.WriteLine("Data directory does not exist.");
        }
        
        // Get the subdirectories in the data directory
        string[] subdirectories = Directory.GetDirectories(dataPath);
        int totalSubdirectories = subdirectories.Length;
        int subdirectoriesPerDirectory = totalSubdirectories / 3;
        int remainder = totalSubdirectories % 3;
        
        // Create directories 1, 2, and 3
        string dir1 = Path.Combine(dataPath, "1");
        string dir2 = Path.Combine(dataPath, "2");
        string dir3 = Path.Combine(dataPath, "3");
        
        Directory.CreateDirectory(dir1);
        Directory.CreateDirectory(dir2);
        Directory.CreateDirectory(dir3);
        
        // Keep track of the current subdirectory
        int currentSubdirectory = 0;
        
        // Distribute the subdirectories into the 1, 2, and 3 directories
        for (int i = 0; i < 3; i++)
        {
            int targetCount = subdirectoriesPerDirectory + (i < remainder ? 1 : 0);

            for (int j = 0; j < targetCount; j++)
            {
                if (currentSubdirectory < totalSubdirectories)
                {
                    string subdirectory = subdirectories[currentSubdirectory];
                    string targetDirectory = i == 0 ? dir1 : i == 1 ? dir2 : dir3;

                    // Move the subdirectory to the target directory
                    string targetPath = Path.Combine(targetDirectory, Path.GetFileName(subdirectory));
                    Directory.Move(subdirectory, targetPath);
                    Console.WriteLine($"Moved {subdirectory} to {targetPath}");

                    currentSubdirectory++;
                }
            }
        }

        Console.WriteLine("Data split into directories 1, 2, and 3.");
    }
}
