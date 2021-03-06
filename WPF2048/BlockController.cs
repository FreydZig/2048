using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WPF2048
{
    class BlockController
    {
        class MRandom
        {
            private static Random random;
            public static int getRandomNumber(int from, int to)
            {
                if (random == null)
                {
                    random = new Random();
                }
                int len = to - from; 
                int n = random.Next(len); 
                n = n + from;
                return n;
            }
        }
        public enum Direction { Up = 1, Down = 2, Right = 3, Left = 4 };
        public Block[,] blocks = new Block[4, 4];
        public Block[,] oldBlocks = new Block[4, 4];

        public void initBlocks()
        {
            initBlockArray(ref blocks);
            initBlockArray(ref oldBlocks);
            generateABlock();
            generateABlock();
        }

        public bool canMove(Direction dir)
        {
            Block[,] tBlocks = transformToLeft(dir);
            //printBlocks(tBlocks, "Can move: after trans");
            return canMoveLeft(tBlocks);
        }

        public int move(Direction dir)
        {
            Block[,] tBlocks = transformToLeft(dir);

            Block[,] tOldBlocks = new Block[4, 4];
            initBlockArray(ref tOldBlocks);
            copyBlocks(ref tBlocks, ref tOldBlocks);
            int score = moveLeft(ref tBlocks, ref tOldBlocks);
            oldBlocks = transformBack(dir, tOldBlocks);
            blocks = transformBack(dir, tBlocks);
            generateABlock();
            return score;
        }
        private void generateABlock()
        {
            List<Block> emptyBlockList = new List<Block>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (blocks[i,j].num == 0)
                    {
                        emptyBlockList.Add(blocks[i, j]);
                    }
                }
            }
            int ranNum = MRandom.getRandomNumber(0, emptyBlockList.Count);
            emptyBlockList[ranNum].num = MRandom.getRandomNumber(1, 3) * 2;
        }

        private void initBlockArray(ref Block[,] blks) {
           for (int Row = 0; Row < 4; Row++)
                for (int Column = 0; Column < 4; Column++)
                {
                    blks[Row, Column] = new Block();
                    blks[Row, Column].num = 0;
                }
        }
        private void copyBlocks(ref Block[,] fromBlocks, ref Block[,] toBlocks)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    assignBlocks(ref fromBlocks[i, j], ref toBlocks[i, j]);
                }
            }
        }
        private void assignBlocks(ref Block fromBlock, ref Block toBlock)
        {
            toBlock.movesteps = fromBlock.movesteps;
            toBlock.status = fromBlock.status;
            toBlock.num = fromBlock.num;
            toBlock.oldColumn = fromBlock.oldColumn;
            toBlock.oldRow = fromBlock.oldRow;
        }

        private void clearBlocks(ref Block a)
        {
            a.movesteps = 0;
            a.status = 0;
            a.num = 0;
            a.oldColumn = 0;
            a.oldRow = 0;
        }

        private Block[,] transformToLeft(Direction dir)
        {
            Block[,] tBlocks = new Block[4, 4];
            initBlockArray(ref tBlocks);
            switch (dir)
            {
                case Direction.Up:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref blocks[i, j],ref tBlocks[3-j, i]);
                            }
                        }
                    }
                    break;
                case Direction.Down:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref blocks[i, j],ref tBlocks[j, 3 - i]);
                            }
                        }
                    }
                    break;
                case Direction.Right:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref blocks[i, 3 - j],ref tBlocks[i, j]);
                            }
                        }
                    }
                    break;
                case Direction.Left:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks( ref blocks[i, j],ref tBlocks[i, j]);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return tBlocks;
        }

        private Block[,] transformBack(Direction dir,Block[,] tBlocks)
        {
            Block[,] nBlocks = new Block[4, 4];
            initBlockArray(ref nBlocks);
            switch (dir)
            {
                case Direction.Down:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref tBlocks[i, j], ref nBlocks[3 - j, i]);
                            }
                        }
                    }
                    break;
                case Direction.Up:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref tBlocks[i, j], ref nBlocks[j, 3 - i]);
                            }
                        }
                    }
                    break;
                case Direction.Right:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref tBlocks[i, 3 - j],ref nBlocks[i, j]);
                            }
                        }
                    }
                    break;
                case Direction.Left:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                assignBlocks(ref tBlocks[i, j], ref nBlocks[i, j]);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return nBlocks;
        }

        private bool canMoveLeft(Block[,] blks)
        {
            for (int row = 0; row < 4; row++)
            {
                int startCol = 0; 
                int endCol = 1; 
                while (startCol < 4 && endCol < 4)
                {
                    while (endCol < 4 && blks[row, endCol].num == 0)
                    {
                        endCol++;
                    }
                    if (endCol >= 4)
                    {
                        continue;
                    }
                    if (blks[row, startCol].num == 0)
                    {
                        return true; 
                    }
                    if (endCol - startCol > 1)
                    {
                        return true;
                    }
                    if (blks[row, startCol].num == blks[row, endCol].num)
                    {
                        return true;
                    }
                    startCol++;
                    endCol++;
                }
            }
            return false;
        }

        private void clearBlocksStatus(ref Block[,] blks)
        {
            for (int i = 0; i < 4; i++)
			{
                for (int j = 0; j < 4; j++)
                {
                    blks[i, j].status = Block.BlockStatus.NotMoved;
                }
			}
        }

        private int moveLeft(ref Block[,] blks, ref Block[,] oldBlks)
        {
            clearBlocksStatus(ref oldBlks);
            int score = 0;
            for (int row = 0; row < 4; row++)
            {
                int emptyCol = 0;
                int startCol = 0;
                int endCol = 1;
                while (startCol < 4 && emptyCol < 4)
                {
                    while (endCol < 4 && blks[row, endCol].num == 0)
                    {
                        endCol++;
                    }
                    while (startCol < 4 && blks[row, startCol].num == 0 )
                    {
                        startCol++;
                    }
                    if (startCol == endCol)
                    {
                        endCol++;
                    }
                    while (endCol < 4 && blks[row, endCol].num == 0)
                    {
                        endCol++;
                    }
                    if (startCol >= 4 && endCol >= 4)
                    {
                        break; 
                    }

                    if (blks[row, emptyCol].num == 0)
                    {
                        if (endCol < 4) 
                        {
                            if (blks[row, endCol].num == blks[row, startCol].num) 
                            {
                                blks[row, emptyCol].num = blks[row, startCol].num * 2; 
                                score += blks[row, emptyCol].num;

                                clearBlocks(ref blks[row, startCol]);
                                clearBlocks(ref blks[row, endCol]);

                                oldBlks[row, emptyCol].status = Block.BlockStatus.Combined;
                                oldBlks[row, startCol].status = Block.BlockStatus.Moved;
                                oldBlks[row, startCol].movesteps = startCol - emptyCol;
                                oldBlks[row, endCol].status = Block.BlockStatus.Moved;
                                oldBlks[row, endCol].movesteps = endCol - emptyCol;



                                emptyCol = emptyCol + 1;
                                startCol = endCol + 1;
                                endCol = startCol + 1;
                            }
                            else 
                            {

                                int firstNum = blks[row, startCol].num;
                                int secondNum = blks[row, endCol].num;


                                clearBlocks(ref blks[row, startCol]);
                                clearBlocks(ref blks[row, endCol]);

                                blks[row, emptyCol].num = firstNum;
                                blks[row, emptyCol + 1].num = secondNum;


                                oldBlks[row, startCol].status = Block.BlockStatus.Moved;
                                oldBlks[row, startCol].movesteps = startCol - emptyCol;
                                oldBlks[row, endCol].status = Block.BlockStatus.Moved;
                                oldBlks[row, endCol].movesteps = endCol - emptyCol - 1;

                                emptyCol = emptyCol + 2;
                                startCol = endCol + 1;
                                endCol = startCol + 1;

                            }
                        }
                        else 
                        {

                            blks[row, emptyCol].num = blks[row, startCol].num;

                            clearBlocks(ref blks[row, startCol]);

                            oldBlks[row, startCol].status = Block.BlockStatus.Moved;
                            oldBlks[row, startCol].movesteps = startCol - emptyCol;
                            
                            continue;
                        }

                    }
                    else 
                    {

                        if (endCol < 4 && blks[row,startCol].num == blks[row,endCol].num)
                        {
                            blks[row, emptyCol].num = blks[row, startCol].num * 2;
                            score += blks[row, emptyCol].num;

                            clearBlocks(ref blks[row, endCol]);

                            oldBlks[row, endCol].movesteps = endCol - startCol;
                            oldBlks[row, endCol].status = Block.BlockStatus.Moved;
                            oldBlks[row, startCol].status = Block.BlockStatus.Combined;

                            emptyCol = emptyCol + 1;
                            startCol = endCol + 1;
                            endCol = startCol + 1;
                        }
                        else
                        {
                            emptyCol++;
                            startCol++;
                            endCol++;
                        }
                    }
                }
            }
            return score;
        }
        public void printBlocks(Block[,] blks,String msg)
        {
            Debug.WriteLine("--------- " + msg + "  -----------");
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Debug.Write(blks[row, col].status.ToString() + " ");
                }
                Debug.Write("\n");
            }
        }
    }
}
