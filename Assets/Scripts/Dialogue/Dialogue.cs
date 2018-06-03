using System;
using System.Collections.Generic;

class Dialogue
{
    public int Id { get; set; }
    public IEnumerable<string> Lines { get; set; }
    public IEnumerable<Dialogue> Children { get; set; }
    public IEnumerable<Action> Actions { get; set; }
}