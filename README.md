ShellOut
========

An integrated shell syntax for C#.

Somewhat (mis)inspired by: http://julialang.org/blog/2012/03/shelling-out-sucks/

You can run a process using `Shell.Run(executableName, arg1, arg2, arg3, etc)`.

You can connect processes using pipes with: `process1 | process2 | process3 | etc`.

You can pipe input/output using `<`/`>`. These operators support the following types:

- a string is assumed to be a filename to read from/write to
- any `Stream` type is also supported as a sink source/destination



*Note that error reporting is not yet implemented.*

For a full example, see [the example program](src/Example/Program.cs).

Unanswered questions
--------------------

Maybe `await` should perform the same as `await (...).Execute()`?
