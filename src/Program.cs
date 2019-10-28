using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using MyWatcher;


namespace Program {
  class Program {
    static void Main(string[] args) {
      string folderName = "watch";
      if (!Directory.Exists(folderName)) {
        Directory.CreateDirectory(folderName);
      }

      Watcher watcher = new Watcher(folderName);
      watcher.Added += Added;
      watcher.Changed += Changed;
      watcher.Deleted += Deleted;

      Thread t = new Thread(new ThreadStart(watcher.Watch));
      t.Start();
    }

    static void Added(object sender, EventArgs e) {
      Console.WriteLine("Something was added to the Directory");
    }
    static void Changed(object sender, EventArgs e) {
      Console.WriteLine("Something was changed in the Directory");
    }
    static void Deleted(object sender, EventArgs e) {
      Console.WriteLine("Something was Deleted in the Directory");
    }
  }
}

namespace MyWatcher {
  class MyFile {
    public string FileName { get; set; }
    public DateTime LastChanged { get; set; }

    public MyFile(string passedFileName, DateTime changed) {
      FileName = passedFileName;
      LastChanged = changed;
    }
  }

  class Watcher {
    private string folder;
    private List<MyFile> temp;

    public Watcher(string passedFolder) {
      folder = passedFolder;
    }

    public void Watch() {
      int sleepTime = 500;
      while (true) {
        DoWatch();
        Thread.Sleep(sleepTime);
      }
    }

    public void DoWatch() {
      List<MyFile> list = new List<MyFile>();
      string[] files = Directory.GetFiles(folder);

      if (files != null) {
        foreach (string fileName in files) {
          DateTime time = File.GetCreationTime(fileName);
          list.Add(new MyFile(fileName, time));
        }
      }

      // Created
      if (temp != null) {
        List<MyFile> result = list.Where(n => !temp.Select(n1 => n1.FileName).Contains(n.FileName)).ToList();
        foreach(MyFile item in result) {
          OnFileAdded(EventArgs.Empty);
        }
      }


      // Deleted
      if (temp != null) {
        List<MyFile> result = temp.Where(n => !list.Select(n1 => n1.FileName).Contains(n.FileName)).ToList();
        foreach(MyFile item in result) {
          OnFileDeleted(EventArgs.Empty);
        }
      }

      // Changed
      if (temp != null) {
        for (int i = 0; i < list.Count; i++) {
          MyFile file = list[i];
          for (int k = 0; k < temp.Count; k++) {
            MyFile tempFile = temp[k];
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

    public event EventHandler Added;
    public event EventHandler Changed;
    public event EventHandler Deleted;
  }
}
