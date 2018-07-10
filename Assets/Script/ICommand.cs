using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// コマンド
public interface ICommand
{
    string Tag { get; }
    void Command(Dictionary<string, string> command);
}

// 事前に呼ばれるコマンド
public interface IPreCommand
{
    string Tag { get; }
    void PreCommand(Dictionary<string, string> command);
}