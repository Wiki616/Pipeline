using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Pipeline
{
    public partial class Form1 : Form
    {
        int lines; //y86代码行数
        string[] y86 = new string[50]; //存储y86指令
        string[] address = new string[20]; //存储指令地址
        int cycle = 0; //判断目前运行到第几个cycle
        bool state = false; //让程序能逐步执行
        //以下是判断目前各阶段应该读取第几行指令
        int F = 0;
        int D = -1;
        int E = -2;
        int M = -3;
        int W = -4;
        bool tst1 = false; //读取文件选择
        bool tst2 = false;
        bool tst3 = false;
        bool jmp1 = false;
        bool jmp2 = false;
        bool jmp3 = false;

        private void initial() //初始化
        {
            F = 0;
            D = -1;
            E = -2;
            M = -3;
            W = -4;
            cycle = 0;
            cycle_num.Text = "0";
            eax.Text = "0x00000000";
            ebx.Text = "0x00000000";
            ecx.Text = "0x00000000";
            edx.Text = "0x00000000";
            edi.Text = "0x00000000";
            esi.Text = "0x00000000";
            ebp.Text = "0x00000000";
            esp.Text = "0x00000000";

            F_predPC.Text = "0x00000000";

            D_icode.Text = "0";
            D_ifun.Text = "0";
            D_rA.Text = "8";
            D_rB.Text = "8";
            D_valC.Text = "0x00000000";
            D_valP.Text = "0x00000000";

            E_icode.Text = "0";
            E_ifun.Text = "0";
            E_valA.Text = "0x00000000";
            E_valB.Text = "0x00000000";
            E_valC.Text = "0x00000000";
            E_dstE.Text = "8";
            E_dstM.Text = "8";
            E_srcA.Text = "8";
            E_srcB.Text = "8";

            M_icode.Text = "0";
            M_Bch.Text = "0";
            M_valA.Text = "0x00000000";
            M_valE.Text = "0x00000000";
            M_dstE.Text = "8";
            M_dstM.Text = "8";

            W_icode.Text = "0";
            W_ifun.Text = "0";
            W_valE.Text = "0x00000000";
            W_valM.Text = "0x00000000";
            W_dstE.Text = "8";
            W_dstM.Text = "8";

            ZF.Text = "0";
            SF.Text = "0";
            OF.Text = "0";
            Step.Enabled = true;
        }

        public string transfer_from_registers(string reg) //将寄存器转化为对应的y86指令
        {
            string y86;
            if (reg == "%eax")
            {
                y86 = "0";
            }
            else if (reg == "%ecx")
            {
                y86 = "1";
            }
            else if (reg == "%edx")
            {
                y86 = "2";
            }
            else if (reg == "%ebx")
            {
                y86 = "3";
            }
            else if (reg == "%esp")
            {
                y86 = "4";
            }
            else if (reg == "%ebp")
            {
                y86 = "5";
            }
            else if (reg == "%esi")
            {
                y86 = "6";
            }
            else if (reg == "%edi")
            {
                y86 = "7";
            }
            else y86 = "8";
            return y86;
        }

        public int register_to_int(string reg) //将寄存器转化为相应的int型数字
        {
            int y86 = 0;
            if (reg == "%eax")
            {
                y86 = 0;
            }
            else if (reg == "%ecx")
            {
                y86 = 1;
            }
            else if (reg == "%edx")
            {
                y86 = 2;
            }
            else if (reg == "%ebx")
            {
                y86 = 3;
            }
            else if (reg == "%esp")
            {
                y86 = 4;
            }
            else if (reg == "%ebp")
            {
                y86 = 5;
            }
            else if (reg == "%esi")
            {
                y86 = 6;
            }
            else if (reg == "%edi")
            {
                y86 = 7;
            }
            else y86 = 8;
            return y86;
        }

        public string transfer_to_registers(int reg) //将y86的指令转化为对应的寄存器
        {
            string y86 = "";
            if (reg == 0)
            {
                y86 = "%eax";
            }
            else if (reg == 1)
            {
                y86 = "%ecx";
            }
            else if (reg == 2)
            {
                y86 = "%edx";
            }
            else if (reg == 3)
            {
                y86 = "%ebx";
            }
            else if (reg == 4)
            {
                y86 = "%esp";
            }
            else if (reg == 5)
            {
                y86 = "%ebp";
            }
            else if (reg == 6)
            {
                y86 = "%esi";
            }
            else if (reg == 7)
            {
                y86 = "%edi";
            }
            else if (reg == 8)
            {
                y86 = "";
            }
            return y86;
        }

        public void write_into_registerM(int reg) //写回阶段从dstM写入寄存器
        {
            if (reg == 0)
            {
                eax.Text = W_valM.Text;
            }
            else if (reg == 1)
            {
                ecx.Text = W_valM.Text;
            }
            else if (reg == 2)
            {
                edx.Text = W_valM.Text;
            }
            else if (reg == 3)
            {
                ebx.Text = W_valM.Text;
            }
            else if (reg == 4)
            {
                esp.Text = W_valM.Text;
            }
            else if (reg == 5)
            {
                ebp.Text = W_valM.Text;
            }
            else if (reg == 6)
            {
                esi.Text = W_valM.Text;
            }
            else if (reg == 7)
            {
                edi.Text = W_valM.Text;
            }
            else if (reg == 8) return;
        }

        public void write_into_registerE(int reg) //写回阶段从dstE写入寄存器
        {
            if (reg == 0)
            {
                eax.Text = W_valE.Text;
            }
            else if (reg == 1)
            {
                ecx.Text = W_valE.Text;
            }
            else if (reg == 2)
            {
                edx.Text = W_valE.Text;
            }
            else if (reg == 3)
            {
                ebx.Text = W_valE.Text;
            }
            else if (reg == 4)
            {
                esp.Text = W_valE.Text;
            }
            else if (reg == 5)
            {
                ebp.Text = W_valE.Text;
            }
            else if (reg == 6)
            {
                esi.Text = W_valE.Text;
            }
            else if (reg == 7)
            {
                edi.Text = W_valE.Text;
            }
            else if (reg == 8) return;
        }

        public void write_into_valA(int reg) //执行阶段从寄存器写入valA
        {
            if (reg == 0)
            {
                E_valA.Text = eax.Text;
            }
            else if (reg == 1)
            {
                E_valA.Text = ecx.Text;
            }
            else if (reg == 2)
            {
                E_valA.Text = edx.Text;
            }
            else if (reg == 3)
            {
                E_valA.Text = ebx.Text;
            }
            else if (reg == 4)
            {
                E_valA.Text = esp.Text;
            }
            else if (reg == 5)
            {
                E_valA.Text = ebp.Text;
            }
            else if (reg == 6)
            {
                E_valA.Text = esi.Text;
            }
            else if (reg == 7)
            {
                E_valA.Text = edi.Text;
            }
            else if (reg == 8) return;
        }

        public void write_into_valB(int reg) //执行阶段从寄存器写入valB
        {
            if (reg == 0)
            {
                E_valB.Text = eax.Text;
            }
            else if (reg == 1)
            {
                E_valB.Text = ecx.Text;
            }
            else if (reg == 2)
            {
                E_valB.Text = edx.Text;
            }
            else if (reg == 3)
            {
                E_valB.Text = ebx.Text;
            }
            else if (reg == 4)
            {
                E_valB.Text = esp.Text;
            }
            else if (reg == 5)
            {
                E_valB.Text = ebp.Text;
            }
            else if (reg == 6)
            {
                E_valB.Text = esi.Text;
            }
            else if (reg == 7)
            {
                E_valB.Text = edi.Text;
            }
            else if (reg == 8) return;
        }

        public void recognizing1(string head, string part1, string part2, int index) //将两个寄存器的操作转化为y86指令
        {
            if(head == "rrmovl")
            {
                string start = "20";
                string rA = transfer_from_registers(part1);
                string rB = transfer_from_registers(part2);
                y86[index] = string.Join("",start, rA, rB);
            }
            else if (head == "irmovl")
            {
                string start = "308";
                string V = part1.Substring(1, part1.Length - 1).PadLeft(8, '0');
                string rB = transfer_from_registers(part2);
                y86[index] = string.Join("", start, rB, V);
            }
            else if (head == "rmmovl")
            {
                string start = "40";
                string rA = transfer_from_registers(part1);
                int d_index = 0;
                for(int i = 0; i < part2.Length; i++)
                    if(part2[i] == '(')
                    {
                        d_index = i;
                        break;
                    }
                string D = part2.Substring(0, d_index);
                string rB = transfer_from_registers(part2.Substring(d_index + 1, 4));
                y86[index] = string.Join( "",start, rA, rB, D);
            }
            else if(head  == "mrmovl")
            {
                string start = "50";
                string rA = transfer_from_registers(part2);
                int d_index = 0;
                for(int i = 0; i < part1.Length; i++)
                    if(part1[i] == '(')
                    {
                        d_index = i;
                        break;
                    }
                string D = part1.Substring(0, d_index);
                string rB = transfer_from_registers(part1.Substring(d_index + 1, 4));
                y86[index] = string.Join( "",start, rA, rB, D);
            }
            else if(head == "addl")
            {
                string start = "60";
                string rA = transfer_from_registers(part1);
                string rB = transfer_from_registers(part2);
                y86[index] = string.Join("",start, rA, rB);
            }
            else if(head == "subl")
            {
                string start = "61";
                string rA = transfer_from_registers(part1);
                string rB = transfer_from_registers(part2);
                y86[index] = string.Join("",start, rA, rB);
            }
            else if (head == "andl")
            {
                string start = "62";
                string rA = transfer_from_registers(part1);
                string rB = transfer_from_registers(part2);
                y86[index] = string.Join("",start, rA, rB);
            }
            else if (head == "xorl")
            {
                string start = "63";
                string rA = transfer_from_registers(part1);
                string rB = transfer_from_registers(part2);
                y86[index] = string.Join("",start, rA, rB);
            }
        }

        public void recognizing2(string head, string part, int index) //将一个寄存器的操作转化为y86指令，这里选择pushl与popl
        {
            if (head == "pushl")
            {
                string start = "A0";
                string rA = transfer_from_registers(part);
                string end = "8";
                y86[index] = string.Join("", start, rA, end);
            }
            else if (head == "popl")
            {
                string start = "B0";
                string rA = transfer_from_registers(part);
                string end = "8";
                y86[index] = string.Join("", start, rA, end);
            }
            else
            {
                string start = null;
                string dest = null;
                for (int i = 0; i < lines; i++)
                {
                    if (address[i] != null) continue;
                    else
                    {
                        string cmp = Code.Lines[i].Substring(0, Code.Lines[i].Length - 1);
                        if (cmp == part)
                        {
                            dest = address[i + 1].Substring(2);
                            break;
                        }
                    }
                }
                if (head == "jmp") start = "70";
                else if (head == "jle") start = "71";
                else if (head == "jl") start = "72";
                else if (head == "je") start = "73";
                else if (head == "jne") start = "74";
                else if (head == "jge") start = "75";
                else if (head == "jg") start = "76";
                y86[index] = string.Join("", start, dest);
            }
        }

        public void recognizing3(string head, int index) //将不用寄存器的操作转化为y86指令
        {
            if (head == "halt")
            {
                y86[index] = "10";
            }
            else if (head == "ret")
            {
                y86[index] = "90";
            }
            else if (head == "nop")
            {
                y86[index] = "00";
            }
        }

        public void y86_coding(string com, int index) //将输入文件转化为y86指令
        {
            int space = 0;
            int intersect = 0;
            if (com[0] == '0')
            {
                for (int i = 11; i < com.Length; i++)
                {
                    if (com[i] == ' ')
                    {
                        space = i;
                        break;
                    }
                }
                if (space != 0)
                {
                    for (int j = space + 1; j < com.Length; j++)
                    {
                        if (com[j] == ',')
                        {
                            intersect = j;
                            break;
                        }
                    }
                }
                if (space != 0 && intersect != 0)
                {
                    string part1 = com.Substring(space + 1, intersect - space - 1);
                    string part2 = com.Substring(intersect + 1, com.Length - intersect - 1);
                    recognizing1(com.Substring(11, space - 11), part1, part2, index);
                }
                else if (space != 0 && intersect == 0)
                {
                    string part = com.Substring(space + 1, com.Length - space - 1);
                    recognizing2(com.Substring(11, space - 11), part, index);
                }
                else if (space == 0)
                {
                    recognizing3(com.Substring(11), index);
                }
            }
            else return;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initial();
            Step.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            initial();
            tst1 = false;
            tst2 = false;
            tst3 = false;
            jmp1 = false;
            jmp2 = false;
            jmp3 = false;
            //读取文件中的汇编代码
            OpenFileDialog code = new OpenFileDialog();
            code.InitialDirectory = "E:\\FDU\\ICS2013\\lab3\\Roy\\Pipeline\\Test";
            code.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            code.RestoreDirectory = true;
            code.FilterIndex = 1;
            if (code.ShowDialog(this) == DialogResult.OK)
            {
                Code.LoadFile(code.FileName, RichTextBoxStreamType.PlainText);
                if (code.FileName.Substring(code.FileName.Length-5,5) == "1.txt") tst1 = true;
                else if (code.FileName.Substring(code.FileName.Length - 5, 5) == "2.txt") tst2 = true;
                else if (code.FileName.Substring(code.FileName.Length - 5, 5) == "3.txt") tst3 = true;
                else if (code.FileName.Substring(code.FileName.Length-5,5) == "4.txt") jmp1 = true;
                else if (code.FileName.Substring(code.FileName.Length - 5, 5) == "4.txt") jmp2 = true;
                else if (code.FileName.Substring(code.FileName.Length - 5, 5) == "4.txt") jmp3 = true;
            }
            lines = Code.Lines.Length;
            //处理汇编代码，转化为y86指令
            for (int i = 0; i < lines; i++)
            {
                if (Code.Lines[i][0] == '0')
                {
                    address[i] = Code.Lines[i].Substring(0, 10);
                }
                else continue;
            }
            for (int i = 0; i < lines; i++) 
                y86_coding(Code.Lines[i], i);
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            initial();
            if (Code.Text == "")
            {
                Step.Enabled = false;
            }
        }

        public string register_conver(int number)
        {
            switch (number)
            {
                case 0: return eax.Text;
                case 1: return ecx.Text;
                case 2: return edx.Text;
                case 3: return ebx.Text;
                case 4: return esp.Text;
                case 5: return ebp.Text;
                case 6: return esi.Text;
                case 7: return edi.Text;
            }
            return "8";
        }

        public void fetch(int index)
        {
            if (F >= lines)
            {
                F_predPC.Text = "0x00000000";
            }
            else
            {
                F_predPC.Text = address[F];
            }
        }

        public void decode(int index)
        {
            if (D < 0 || D >= lines)
            {
                D_icode.Text = "0";
                D_ifun.Text = "0";
                D_rA.Text = "8";
                D_rB.Text = "8";
                D_valP.Text = "0x00000000";
                return;
            }
            if (D == lines - 1)
            {
                string add = address[D].Substring(address[D].Length-2,2);
                int tmp = (char)add[0];
                int end = (int)add[1] + 1;
                char last1 = (char)tmp;
                char last2 = (char)end;
                if (add[1] == 'f')
                {
                    tmp = (int)add[0];
                    tmp++;
                    last1 = (char)tmp;
                    last2 = '0';
                }
                string new_add = string.Join("", address[D].Substring(0, address[D].Length - 2), last1, last2);
                D_valP.Text = new_add;
                D_icode.Text = y86[D][0].ToString();
                D_ifun.Text = y86[D][1].ToString();
                D_rA.Text = "8";
                D_rB.Text = "8";
                D_valC.Text = "0x00000000";
            }
            else
            {
                D_valP.Text = address[D + 1];
                D_icode.Text = y86[D][0].ToString();
                D_ifun.Text = y86[D][1].ToString();
                D_rA.Text = "8";
                D_rB.Text = "8";
                D_valC.Text = "0x00000000";
                if (y86[D][0] =='2' || y86[D][0] == '3' || y86[D][0] == '4' || y86[D][0] == '5')
                {
                    D_rA.Text = y86[D][2].ToString();
                    D_rB.Text = y86[D][3].ToString();
                    if (y86[D][0] != '2')
                    {
                        D_valC.Text = y86[D].Substring(4).PadLeft(9, 'x').PadLeft(10, '0');
                    }
                }
                else if (y86[D][0] == '7')
                {
                    D_valC.Text = y86[D].Substring(2).PadLeft(9, 'x').PadLeft(10, '0');
                }
                else
                {
                    D_rA.Text = y86[D][2].ToString();
                    D_rB.Text = y86[D][3].ToString();
                }
            }
        }

        public void execute(int index)    
        {
            if (E < 0 || E >= lines)
            {
                E_icode.Text = "0";
                E_ifun.Text = "0";
                E_valC.Text = "0x00000000";
                E_valA.Text = "0x00000000";
                E_valB.Text = "0x00000000";
                E_dstE.Text = "8";
                E_dstM.Text = "8";
                E_srcA.Text = "8";
                E_srcB.Text = "8";
            }
            else
            {
                E_icode.Text = D_icode.Text;
                E_ifun.Text = D_ifun.Text;
                E_valC.Text = D_valC.Text;
                E_dstE.Text = D_rB.Text;
                E_srcA.Text = D_rA.Text;
                E_srcB.Text = "8";
                if (D_rA.Text != "8" && D_rB.Text != "8")
                {
                    E_srcA.Text = D_rA.Text;
                    E_srcB.Text = D_rB.Text;
                }
                if (E_icode.Text == "4")
                {
                    E_dstE.Text = "8";
                }
                //利用转发以避免冒险
                if (E_srcA.Text == M_dstE.Text)
                    {
                        E_valA.Text = M_valE.Text;
                        E_valB.Text = W_valE.Text;
                    }
                    else
                    {
                        E_valA.Text = W_valE.Text;
                        E_valB.Text = M_valE.Text;
                    }
            }
        }

        public void memory(int index)
        {
            if (M < 0 || M >= lines)
            {
                M_icode.Text = "0";
                M_Bch.Text = "0";
                M_valE.Text = "0x00000000";
                M_valA.Text = "0x00000000";
                M_dstE.Text = "8";
                M_dstM.Text = "8";
            }
            else
            {
                M_icode.Text = E_icode.Text;
                M_Bch.Text = "0";
                M_dstE.Text = E_dstE.Text;
                M_dstM.Text = E_dstM.Text;
                M_valA.Text = E_valA.Text;
                if (M_icode.Text == "2")    //需要修改
                {
                    M_valE.Text = E_valA.Text;
                }
                else if (M_icode.Text == "3")
                {
                    M_valE.Text = E_valC.Text;
                }
                else if (M_icode.Text == "4" || M_icode.Text == "5")
                {
                    int tmp1 = Convert.ToInt16(E_valC.Text, 16);
                    int tmp2 = Convert.ToInt16(E_valB.Text, 16);
                    int ans = tmp1 + tmp2;
                    string value = ans.ToString("x");
                    string res = value;
                    res = res.PadLeft(8, '0').PadLeft(9, 'x').PadLeft(10, '0');
                    M_valE.Text = res;
                }
                else if (M_icode.Text == "6")
                {
                    char tmp = E_ifun.Text[0];
                    int tmp1 = Convert.ToInt16(E_valB.Text, 16);
                    int tmp2 = Convert.ToInt16(E_valA.Text, 16);
                    switch (tmp)
                    {
                        case '0':
                            {
                                int ans = tmp1 + tmp2;
                                string value = ans.ToString("x");
                                string res = value;
                                res = res.PadLeft(8,'0').PadLeft(9, 'x').PadLeft(10, '0');
                                M_valE.Text = res;
                                break;
                            }
                        case '1': 
                            {
                                int ans = tmp1 - tmp2;
                                string value = ans.ToString("x");
                                string res = value;
                                res = res.PadLeft(8, '0').PadLeft(9, 'x').PadLeft(10, '0');
                                M_valE.Text = res;
                                //M_valE.Text = "hello world";
                                break;
                            }
                        case '2':
                            {
                                int ans = tmp1 & tmp2;
                                string value = ans.ToString("x");
                                string res = value;
                                res = res.PadLeft(8, '0').PadLeft(9, 'x').PadLeft(10, '0');
                                M_valE.Text = res;
                                break;
                            }
                        case '3':
                            {
                                int ans = tmp1 ^ tmp2;
                                string value = ans.ToString("x");
                                string res = value;
                                res = res.PadLeft(8, '0').PadLeft(9, 'x').PadLeft(10, '0');
                                M_valE.Text = res;
                                break;
                            }
                        default: break;
                    }
                }
                else if (M_icode.Text == "A" || M_icode.Text == "B")
                {
                    int tmp1 = Convert.ToInt16(E_valB.Text, 16);
                    if (M_icode.Text == "B")
                    {
                        tmp1 += 4;
                    }
                    else
                    {
                        tmp1 -= 4;
                    }
                    string value = tmp1.ToString("x");
                    string res = value;
                    res = res.PadLeft(8, '0').PadLeft(9, 'x').PadLeft(10, '0');
                    M_valE.Text = res;
                }
            }
        }

        public void write(int index)
        {
            if (W < 0)
            {
                W_icode.Text = "0";
                W_ifun.Text = "0";
                W_valE.Text = "0x00000000";
                W_valM.Text = "0x00000000";
                W_dstE.Text = "8";
                W_dstM.Text = "8";
            }
            else if (W == lines - 1)
            {
                W_icode.Text = "1";
                W_ifun.Text = "0";
                W_valE.Text = "0x00000000";
                W_valM.Text = "0x00000000";
                W_dstE.Text = "8";
                W_dstM.Text = "8";
            }
            else
            {
                write_into_registerM(int.Parse(W_dstM.Text));
                write_into_registerE(int.Parse(W_dstE.Text));
                W_icode.Text = M_icode.Text;
                W_valE.Text = M_valE.Text;
                W_dstE.Text = M_dstE.Text;
                W_dstM.Text = M_dstM.Text;
            }
        }

        private void Step_Click(object sender, EventArgs e)
        {
            if (Code.Text == "")
            {
                Step.Enabled = false;
                return;
            }
            if (tst1 == true || tst2 == true)
            {
                state = true;
                F++;
                D++;
                E++;
                M++;
                W++;
                write(W);
                memory(M);
                execute(E);
                decode(D);
                fetch(F);
                cycle++;
                cycle_num.Text = cycle.ToString();
                if (W == lines-1) Step.Enabled = false;
            }
        }
    }
}
