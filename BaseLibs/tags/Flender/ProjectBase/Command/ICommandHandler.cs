using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectBase.Command
{
    //领域命令的处理程序
    public interface ICommandHandler<ResultT, CommandT> where CommandT : ICommand
    {
        ResultT Handle(CommandT commands);
    }

    public interface ICommandHandler<CommandT> where CommandT : ICommand
    {
        void Handle(CommandT commands);
    }

    //进程内同步方式查找命令的处理程序并执行
    public abstract class CommandHandler<ResultT, CommandT> : ICommandHandler<ResultT, CommandT> where CommandT : ICommand
    {  
        public abstract ResultT Handle(CommandT commands);
    }
    public abstract class CommandHandler<CommandT> : ICommandHandler<CommandT> where CommandT : ICommand
    {
        public abstract void Handle(CommandT commands);
    }


}