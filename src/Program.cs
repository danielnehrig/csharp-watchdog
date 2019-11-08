using System;
using System.IO;
using System.Threading;
using Watcher;

/// <summary>
/// By Daniel Nehrig
/// </summary>
namespace Program {
  class Program {
    static int Main(string[] args) {
      string folderName = "watch";
      int waitTime = 500;


      // Check if Directory Exists if not add
      if (!Directory.Exists(folderName)) {
        Directory.CreateDirectory(folderName);
      }

      // Create Watcher and attach events
      WatchDog watcher = new WatchDog(folderName, waitTime);
      watcher.Added += Added;
      watcher.Changed += Changed;
      watcher.Deleted += Deleted;

      // Start watcher as a Thread
      Thread t = new Thread(new ThreadStart(watcher.Watch));
      t.Start();

      // Exit Gracefully
      return 0;
    }

    static void Added(object sender, FileEventArgs e) {
      // Do Add Logic
      Console.WriteLine(String.Format("Something was Added in the Directory : {0}", e.FileName));
    }

    static void Changed(object sender, FileEventArgs e) {
      // Do Changed Logic
      Console.WriteLine(String.Format("Something was Changed in the Directory : {0}", e.FileName));
    }

    static void Deleted(object sender, FileEventArgs e) {
      // Do Deleted Logic
      Console.WriteLine(String.Format("Something was Deleted in the Directory : {0}", e.FileName));
    }
  }
}
