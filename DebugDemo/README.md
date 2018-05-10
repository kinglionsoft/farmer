# Debugging .NET core on Linux

## Ubuntu 16.04

### Tools

#### lldb

** For .NET Core version 1.x and 2.0.x, libsosplugin.so is built for and will only work with version 3.6 of lldb. For .NET Core 2.1, the plugin is built for 3.9 lldb and will work with 3.8 and 3.9 lldb. **

``` bash
sudo apt-get install lldb-3.6
```

### Debugging process on Linux

* Start the application and find the Pid.

``` bash
ps -ejH|grep <app name>
```

* Start lldb and attach the target process:

``` bash

root@administrator-Virtual-Machine:/home/yangchao/test/out# lldb-3.6
(lldb) attach -p 5526
Process 5526 stopped
* thread #1: tid = 5526, 0x00007f86a697151d libpthread.so.0, name = 'dotnet', stop reason = trace
    frame #0: 0x00007f86a697151d libpthread.so.0
-> 0x7f86a697151d <???+45>: movq   (%rsp), %rdi
   0x7f86a6971521 <???+49>: movq   %rax, %rdx
   0x7f86a6971524 <???+52>: callq  0x7f86a69711c0            ; __pthread_disable_asynccancel
   0x7f86a6971529 <???+57>: movq   %rdx, %rax

Executable module set to "/usr/share/dotnet/dotnet".
Architecture set to: x86_64-pc-linux.
```

* Load plugin

``` bash
(lldb) plugin load /usr/share/dotnet/shared/Microsoft.NETCore.App/2.0.6/libsosplugin.so
```

* Map OS Tid to LLDB Tid, and use sos command(e.g. clrstack) as below.

### Analyzing a .NET Core Core Dump on Linux(Ubuntu 16.04)

* Enable core dump

``` bash
ulimit -c unlimited
```

* Load core file    

``` bash

lldb-3.6 -O "settings set target.exec-search-paths /usr/share/dotnet/shared/Microsoft.NETCore.App/2.0.6/" -o "plugin load /usr/share/dotnet/shared/Microsoft.NETCore.App/2.0.6/libsosplugin.so" --core /home/yangchao/test/out/core /usr/bin/dotnet

```

* Find the thread that raised an exception

``` bash
(lldb) sos Threads
ThreadCount:      2
UnstartedThread:  0
BackgroundThread: 1
PendingThread:    0
DeadThread:       0
Hosted Runtime:   no
                                                                                                        Lock  
       ID OSID ThreadOBJ           State GC Mode     GC Alloc Context                  Domain           Count Apt Exception
XXXX    1  c6d 000000000199E310    20020 Preemptive  00007FD82C0341F8:00007FD82C035FD0 0000000001986030 0     Ukn System.Exception 00007fd82c028e38
XXXX    2  c73 00000000019C1050    21220 Preemptive  0000000000000000:0000000000000000 0000000001986030 0     Ukn (Finalizer) 
```


* Get LLDB thread list

``` bash
(lldb) thread list
Process 0 stopped
* thread #1: tid = 0, 0x00007fd8cce6b428 libc.so.6`gsignal + 56, name = 'dotnet', stop reason = signal SIGABRT
  thread #2: tid = 1, 0x00007fd8cdab194d libpthread.so.0, stop reason = signal SIGABRT
  thread #3: tid = 2, 0x00007fd8ccf374d9 libc.so.6`syscall + 25, stop reason = signal SIGABRT
  thread #4: tid = 3, 0x00007fd8cdab1c7d libpthread.so.0, stop reason = signal SIGABRT
  thread #5: tid = 4, 0x00007fd8cdaae360 libpthread.so.0`__pthread_cond_wait + 192, stop reason = signal SIGABRT
  thread #6: tid = 5, 0x00007fd8cdaae709 libpthread.so.0`__pthread_cond_timedwait + 297, stop reason = signal SIGABRT
  thread #7: tid = 6, 0x00007fd8ccf3174d libc.so.6`__poll + 45, stop reason = signal SIGABRT
```

* Map OS Tid to LLDB Tid

``` bash
(lldb) setsostid c6d 1
Mapped sos OS tid 0xc6d to lldb thread index 1
```

* Print Exception

``` bash
(lldb) pe
Exception object: 00007fd82c028e38
Exception type:   System.Exception
Message:          crash now
InnerException:   <none>
StackTrace (generated):
    SP               IP               Function
    00007FFC1B8845D0 00007FD8532004EE test.dll!test.Program.Main(System.String[])+0x6e

StackTraceString: <none>
HResult: 80131500

```

### Analyzing a .NET Core Core Dump on Linux(Self-containded, Ubuntu 16.04)

* Load executable file with core dump
``` bash 
root@administrator-Virtual-Machine:/home/yangchao/test/self# lldb-3.6  test -c core
(lldb) target create "test" --core "core"
Core file '/home/yangchao/test/self/core' (x86_64) was loaded.
Process 0 stopped
* thread #1: tid = 0, 0x00007f6ead6bd428 libc.so.6`gsignal + 56, name = 'test', stop reason = signal SIGABRT
    frame #0: 0x00007f6ead6bd428 libc.so.6`gsignal + 56
-> 0x7f6ead6bd428 <gsignal+56>: addb   %al, (%rax)
   0x7f6ead6bd42a <gsignal+58>: addb   %al, (%rax)
   0x7f6ead6bd42c <gsignal+60>: addb   %al, (%rax)
   0x7f6ead6bd42e <gsignal+62>: addb   %al, (%rax)
  thread #2: tid = 1, 0x00007f6eae30394d libpthread.so.0, stop reason = signal SIGABRT
    frame #0: 0x00007f6eae30394d libpthread.so.0
-> 0x7f6eae30394d <???+45>: addb   %al, (%rax)
   0x7f6eae30394f <???+47>: addb   %al, (%rax)
   0x7f6eae303951 <???+49>: addb   %al, (%rax)
   0x7f6eae303953 <???+51>: addb   %al, (%rax)
  thread #3: tid = 2, 0x00007f6ead7894d9 libc.so.6`syscall + 25, stop reason = signal SIGABRT
    frame #0: 0x00007f6ead7894d9 libc.so.6`syscall + 25
-> 0x7f6ead7894d9 <syscall+25>: addb   %al, (%rax)
   0x7f6ead7894db <syscall+27>: addb   %al, (%rax)
   0x7f6ead7894dd <syscall+29>: addb   %al, (%rax)
   0x7f6ead7894df <syscall+31>: addb   %al, (%rax)
  thread #4: tid = 3, 0x00007f6eae300709 libpthread.so.0`__pthread_cond_timedwait + 297, stop reason = signal SIGABRT
    frame #0: 0x00007f6eae300709 libpthread.so.0`__pthread_cond_timedwait + 297
-> 0x7f6eae300709 <__pthread_cond_timedwait+297>: addb   %al, (%rax)
   0x7f6eae30070b <__pthread_cond_timedwait+299>: addb   %al, (%rax)
   0x7f6eae30070d <__pthread_cond_timedwait+301>: addb   %al, (%rax)
   0x7f6eae30070f <__pthread_cond_timedwait+303>: addb   %al, (%rax)
  thread #5: tid = 4, 0x00007f6eae303c7d libpthread.so.0, stop reason = signal SIGABRT
    frame #0: 0x00007f6eae303c7d libpthread.so.0
-> 0x7f6eae303c7d <???+45>: addb   %al, (%rax)
   0x7f6eae303c7f <???+47>: addb   %al, (%rax)
   0x7f6eae303c81 <???+49>: addb   %al, (%rax)
   0x7f6eae303c83 <???+51>: addb   %al, (%rax)
  thread #6: tid = 5, 0x00007f6eae300360 libpthread.so.0`__pthread_cond_wait + 192, stop reason = signal SIGABRT
    frame #0: 0x00007f6eae300360 libpthread.so.0`__pthread_cond_wait + 192
-> 0x7f6eae300360 <__pthread_cond_wait+192>: addb   %al, (%rax)
   0x7f6eae300362 <__pthread_cond_wait+194>: addb   %al, (%rax)
   0x7f6eae300364 <__pthread_cond_wait+196>: addb   %al, (%rax)
   0x7f6eae300366 <__pthread_cond_wait+198>: addb   %al, (%rax)
  thread #7: tid = 6, 0x00007f6eae30351d libpthread.so.0, stop reason = signal SIGABRT
    frame #0: 0x00007f6eae30351d libpthread.so.0
-> 0x7f6eae30351d <???+45>: addb   %al, (%rax)
   0x7f6eae30351f <???+47>: addb   %al, (%rax)
   0x7f6eae303521 <???+49>: addb   %al, (%rax)
   0x7f6eae303523 <???+51>: addb   %al, (%rax)
  thread #8: tid = 7, 0x00007f6ead78374d libc.so.6`__poll + 45, stop reason = signal SIGABRT
    frame #0: 0x00007f6ead78374d libc.so.6`__poll + 45
-> 0x7f6ead78374d <__poll+45>: addb   %al, (%rax)
   0x7f6ead78374f <__poll+47>: addb   %al, (%rax)
   0x7f6ead783751 <__poll+49>: addb   %al, (%rax)
   0x7f6ead783753 <__poll+51>: addb   %al, (%rax)
```

* Load sos plugin

``` bash
(lldb) plugin load libsosplugin.so
```

* Get SOS threads and find the thread that raised an exception.

``` bash
(lldb) sos Threads
ThreadCount:      2
UnstartedThread:  0
BackgroundThread: 1
PendingThread:    0
DeadThread:       0
Hosted Runtime:   no
                                                                                                        Lock  
       ID OSID ThreadOBJ           State GC Mode     GC Alloc Context                  Domain           Count Apt Exception
XXXX    1 70c7 000000000199F750    21220 Preemptive  0000000000000000:0000000000000000 0000000001997690 0     Ukn (Finalizer) 
XXXX    2 70c3 00000000019B6CD0    20020 Preemptive  00007F6E0C033DB0:00007F6E0C033FD0 0000000001997690 0     Ukn System.Exception 00007f6e0c028cd0
```
* Get the lldb threads

``` bash
(lldb) thread list
Process 28867 stopped
  thread #1: tid = 28867, 0x00007f6ead6bd428 libc.so.6`__GI_raise(sig=6) + 56 at raise.c:54, name = 'test', stop reason = signal SIGABRT
  thread #2: tid = 28868, 0x00007f6eae30394d libpthread.so.0`__GI_recvmsg + 45, stop reason = signal SIGABRT
  thread #3: tid = 28869, 0x00007f6ead7894d9 libc.so.6`syscall + 25 at syscall.S:38, stop reason = signal SIGABRT
  thread #4: tid = 28871, 0x00007f6eae300709 libpthread.so.0`__pthread_cond_timedwait + 297, stop reason = signal SIGABRT
  thread #5: tid = 28872, 0x00007f6eae303c7d libpthread.so.0`__GI_open64 + 45, stop reason = signal SIGABRT
  thread #6: tid = 28873, 0x00007f6eae300360 libpthread.so.0`__pthread_cond_wait + 192, stop reason = signal SIGABRT
  thread #7: tid = 28874, 0x00007f6eae30351d libpthread.so.0`__libc_read + 45, stop reason = signal SIGABRT
  thread #8: tid = 28870, 0x00007f6ead78374d libc.so.6`__GI___poll + 45 at syscall-template.S:84, stop reason = signal SIGABR
```

* Map OS Tid to LLDB Tid

``` bash
(lldb) setsostid 70c3 1
Mapped sos OS tid 0x70c3 to lldb thread index 1
``` 

* Print Exception

``` bash
(lldb) pe
Exception object: 00007f6e0c028cd0
Exception type:   System.Exception
Message:          crash now
InnerException:   <none>
StackTrace (generated):
    SP               IP               Function
    00007FFCAE04B070 00007F6E33A60503 test.dll!test.Program.Main(System.String[])+0x83

StackTraceString: <none>
HResult: 80131500
```






## Raspberry Pi 3

### Tools

#### lldb

Only **lldb** is supported by the SOS plugin. gdb can be used to debug the coreclr code but with no SOS support.

* No lldb-3.6 available for Raspbian!!!
* Try building lldb-3.6 for Raspbian, but failed. Look forward to the release of .Net Core 2.1.

## Refrence

* [https://github.com/dotnet/coreclr/blob/master/Documentation/building/debugging-instructions.md](https://github.com/dotnet/coreclr/blob/master/Documentation/building/debugging-instructions.md)
* [https://blogs.msdn.microsoft.com/premier_developer/2017/05/02/debugging-net-core-with-sos-everywhere/](https://blogs.msdn.microsoft.com/premier_developer/2017/05/02/debugging-net-core-with-sos-everywhere/)
* [https://coderwall.com/p/l0ajzg/practical-lldb-with-core-dump](https://coderwall.com/p/l0ajzg/practical-lldb-with-core-dump)
* [https://espider.github.io/NET-Core/lldb-dotnet-core-python/](https://espider.github.io/NET-Core/lldb-dotnet-core-python/)
* [http://blogs.microsoft.co.il/sasha/2017/02/26/analyzing-a-net-core-core-dump-on-linux/](http://blogs.microsoft.co.il/sasha/2017/02/26/analyzing-a-net-core-core-dump-on-linux/)
