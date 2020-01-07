# ClrInvoke
演示c++创建Clr运行时并运行ClrDLL，使用PInvoke实现c++ c#互相调用

#ClrLoader
用于注入到其他程序上并加载ClrDLL的Native模块，通过运程执行Start导出函数加载指定ClrDLL

#DotNet
演示用C#类库，用于加载到c++程序

#InjectSharpLib
DLL注入器，将ClrLoader注入到目标进程，然后远程调用ClrLoader.Start函数执行指定的ClrDLL
