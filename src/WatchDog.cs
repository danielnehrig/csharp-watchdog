using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// WatchDog namespace Watcher
/// By Daniel Nehrig
/// </summary>
namespace Watcher {
  /// <summary>
  /// File Object Buffer for use in List to compare
  /// and extract data from
  /// </summary>
  class FileObj {
    public string FileName { get; set; }
    public DateTime LastChanged { get; set; }

    public FileObj(string passedFileName, DateTime changed) {
      FileName = passedFileName;
      LastChanged = changed;
    }
  }

  /// <summary>
  /// Watching Files in a Directory
  /// </summary>
  class WatchDog {
    private string folder;
    private int sleepTime;
    private bool keyPressed = false;
    private List<FileObj> temp;

    public WatchDog(string passedFolder, int passedSleepTime) {
      folder = passedFolder;
      sleepTime = passedSleepTime;
    }

    /// <summary>
    /// Start the Watching
    /// </summary>
    public void Watch() {
      // Main Watch Loop
      while (!keyPressed) {
        DoWatch();
        Thread.Sleep(sleepTime);
      }
    }

    /// <summary>
    /// Do Watching Logic
    /// </summary>
    private void DoWatch() {
      List<FileObj> list = new List<FileObj>();
      string[] files = Directory.GetFiles(folder);

      // Check if files can be found in folder
      if (files != null) {
        foreach (string fileName in files) {
          DateTime time = File.GetCreationTime(fileName);
          list.Add(new FileObj(fileName, time));
        }
      }

      // Created
      if (temp != null) {
        List<FileObj> result = list.Where(n => !temp.Select(n1 => n1.FileName).Contains(n.FileName)).ToList();
        foreach(FileObj item in result) {
          FileEventArgs createdArgs = new FileEventArgs();
          createdArgs.FileName = item.FileName;
          createdArgs.LastChanged = item.LastChanged;
          OnFileAdded(createdArgs);
        }
      }


      // Deleted
      if (temp != null) {
        List<FileObj> result = temp.Where(n => !list.Select(n1 => n1.FileName).Contains(n.FileName)).ToList();
        foreach(FileObj item in result) {
          FileEventArgs deletedArgs = new FileEventArgs();
          deletedArgs.FileName = item.FileName;
          deletedArgs.LastChanged = item.LastChanged;
          OnFileDeleted(deletedArgs);
        }
      }

      // Changed
      if (temp != null) {
        for (int i = 0; i < list.Count; i++) {
          FileObj file = list[i];
          for (int k = 0; k < temp.Count; k++) {
            FileObj tempFile = temp[k];
            if (file.FileName == tempFile.FileName && file.LastChanged != tempFile.LastChanged) {
              FileEventArgs changedArgs = new FileEventArgs();
              changedArgs.FileName = file.FileName;
              changedArgs.LastChanged = file.LastChanged;
              OnFileChanged(changedArgs);
            }
          }
        }
      }

      temp = list;
    }

    protected virtual void OnFileAdded(FileEventArgs e) {
      EventHandler<FileEventArgs> handler = Added;
      if (handler != null) {
        handler(this, e);
      }
    }

    protected virtual void OnFileChanged(FileEventArgs e) {
      EventHandler<FileEventArgs> handler = Changed;
      if (handler != null) {
        handler(this, e);
      }
    }

    protected virtual void OnFileDeleted(FileEventArgs e) {
      EventHandler<FileEventArgs> handler = Deleted;
      if (handler != null) {
        handler(this, e);
      }
    }

    /// <summary>
    /// Added Item EventHandler
    /// </summary>
    public event EventHandler<FileEventArgs> Added;

    /// <summary>
    /// Changed Item EventHandler
    /// </summary>
    public event EventHandler<FileEventArgs> Changed;

    /// <summary>
    /// Deleted Item EventHandler
    /// </summary>
    public event EventHandler<FileEventArgs> Deleted;
  }

  /// <summary>
  /// File Event Arguments extends EventArgs
  /// </summary>
  public class FileEventArgs : EventArgs {
    public string FileName { get; set; }
    public DateTime LastChanged { get; set; }
  }
}
