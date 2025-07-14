using System;
using System.IO;
using Microsoft.Extensions.Logging;

public enum FileOperation
{
    Copy,
    Move,
    Delete
}

public class FileCommand
{
    public FileOperation Operation { get; }
    public string SourcePath { get; }
    public string DestinationPath { get; }

    public FileCommand(string sourcePath, string destinationPath, FileOperation operation)
    {
        SourcePath = sourcePath;
        DestinationPath = destinationPath;
        Operation = operation;
    }

    public void Execute(ILogger logger)
    {
        try
        {
            switch (Operation)
            {
                case FileOperation.Copy:
                    File.Copy(SourcePath, DestinationPath, overwrite: true);
                    logger.LogInformation($"Copied {SourcePath} to {DestinationPath}");
                    break;

                case FileOperation.Move:
                    File.Move(SourcePath, DestinationPath, overwrite: true);
                    logger.LogInformation($"Moved {SourcePath} to {DestinationPath}");
                    break;

                case FileOperation.Delete:
                    File.Delete(SourcePath);
                    logger.LogInformation($"Deleted {SourcePath}");
                    break;

                default:
                    logger.LogWarning($"Invalid operation for {SourcePath}");
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error executing {Operation} on {SourcePath}");
        }
    }
}
