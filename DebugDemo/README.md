# Debugging .NET core on Linux

## Tools

### lldb

** For .NET Core version 1.x and 2.0.x, libsosplugin.so is built for and will only work with version 3.6 of lldb. For .NET Core 2.1, the plugin is built for 3.9 lldb and will work with 3.8 and 3.9 lldb. **

``` bash
sudo apt-get install lldb-3.6
```

## Debugging process on Linux(Ubuntu 16.04 )

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

## Analyzing a .NET Core Core Dump on Linux(Ubuntu 16.04)

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

## Refrence

* [https://github.com/dotnet/coreclr/blob/master/Documentation/building/debugging-instructions.md](https://github.com/dotnet/coreclr/blob/master/Documentation/building/debugging-instructions.md)
* [https://blogs.msdn.microsoft.com/premier_developer/2017/05/02/debugging-net-core-with-sos-everywhere/](https://blogs.msdn.microsoft.com/premier_developer/2017/05/02/debugging-net-core-with-sos-everywhere/)
* [https://coderwall.com/p/l0ajzg/practical-lldb-with-core-dump](https://coderwall.com/p/l0ajzg/practical-lldb-with-core-dump)
* [https://espider.github.io/NET-Core/lldb-dotnet-core-python/](https://espider.github.io/NET-Core/lldb-dotnet-core-python/)
* [http://blogs.microsoft.co.il/sasha/2017/02/26/analyzing-a-net-core-core-dump-on-linux/](http://blogs.microsoft.co.il/sasha/2017/02/26/analyzing-a-net-core-core-dump-on-linux/)
