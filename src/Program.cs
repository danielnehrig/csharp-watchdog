using System;
using System.IO;
using System.Threading;
using Watcher;

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

      return 0;
    }

    static void Added(object sender, EventArgs e) {
      // Do Add Logic
      Console.WriteLine("Something was added to the Directory");
    }

    static void Changed(object sender, EventArgs e) {
      // Do Changed Logic
      Console.WriteLine("Something was changed in the Directory");
    }

    static void Deleted(object sender, EventArgs e) {
      // Do Deleted Logic
      Console.WriteLine("Something was Deleted in the Directory");
    }
  }
}
