using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// Watching Files in a Directory
/// </summary>
namespace Watcher {
  class FileObj {
    public string FileName { get; set; }
    public DateTime LastChanged { get; set; }

    public FileObj(string passedFileName, DateTime changed) {
      FileName = passedFileName;
      LastChanged = changed;
    }
  }

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
          OnFileAdded(EventArgs.Empty);
        }
      }


      // Deleted
      if (temp != null) {
        List<FileObj> result = temp.Where(n => !list.Select(n1 => n1.FileName).Contains(n.FileName)).ToList();
        foreach(FileObj item in result) {
          OnFileDeleted(EventArgs.Empty);
        }
      }

      // Changed
      if (temp != null) {
        for (int i = 0; i < list.Count; i++) {
          FileObj file = list[i];
          for (int k = 0; k < temp.Count; k++) {
            FileObj tempFile = temp[k];
            if (file.FileName == tempFile.FileName && file.LastChanged != tempFile.LastChanged) {
              OnFileChanged(EventArgs.Empty);
            }
          }
        }
      }

      temp = list;
    }

    protected virtual void OnFileAdded(EventArgs e) {
      EventHandler handler = Added;
      if (handler != null) {
        handler(this, e);
      }
    }

    protected virtual void OnFileChanged(EventArgs e) {
      EventHandler handler = Changed;
      if (handler != null) {
        handler(this, e);
      }
    }

    protected virtual void OnFileDeleted(EventArgs e) {
      EventHandler handler = Deleted;
      if (handler != null) {
        handler(this, e);
      }
    }

    /// <summary>
    /// Added Item EventHandler
    /// </summary>
    public event EventHandler Added;

    /// <summary>
    /// Changed Item EventHandler
    /// </summary>
    public event EventHandler Changed;

    /// <summary>
    /// Deleted Item EventHandler
    /// </summary>
    public event EventHandler Deleted;
  }
}
