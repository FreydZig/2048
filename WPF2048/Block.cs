using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF2048
{
    class Block
    {
        public enum BlockStatus
        {
            NotMoved, Moved, Combined
        }
        public int num { set; get; }
        public int movesteps { set; get; } 
        public BlockStatus status { set; get; }  
        public int oldRow { set; get; }     
        public int oldColumn { set; get; } 
    }
}
