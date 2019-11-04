# Watchdog

## Synopsis

A Program that runs a thread of a file watcher
to check if new files have been created, deleted or changed

## How

On every iteration a list is created of files in the directory
and on the next iteration this list is check against the old list
to determine if new files have been created or deleted or modified

### Library Functionality

The Filewatcher has dalegates to point to the event methods
which can be passed into the created object

- obj.Added += method
- obj.Deleted += method
- obj.Changed += method
