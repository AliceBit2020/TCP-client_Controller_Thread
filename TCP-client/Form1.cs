using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCP_client
{
    public partial class Form1 : Form
    {
        Controller contr;
        SynchronizationContext context;
        public Form1()
        {
            contr = new Controller();
            contr.reseive += AnswerToList;
            context = new SynchronizationContext();
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(contr.Connect));
            thread.IsBackground = true;
            thread.Start();          
        }

     
       
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                contr.CloseSock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент: " + ex.Message);
            }
        }

        private void Send_Click(object sender, EventArgs e)
        {
            string mas = textBox1.Text;
            Thread thread = new Thread(new ParameterizedThreadStart(contr.Send));///10c
            thread.IsBackground = true;
            thread.Start(mas);
         
            string unsw = "Клиент Ничего не получил";
            Thread threadRec = new Thread(() => {  contr.Recieve(); });/// ожидание   нужно оповестить форму о получении string
            threadRec.Start();


            /////  Controller  ------>  RecieveAsync
            ///  contr.RecieveAsync()
           
        }

        public void AnswerToList(string answ)
        {
            context.Send((m) =>  TextUnswer.Text = (string)m, answ) ; 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
