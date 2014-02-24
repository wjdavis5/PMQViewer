using System;

using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace PMQViewer
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*
            Thread nt = new Thread(newPMQWindow);
            nt.TrySetApartmentState(ApartmentState.STA);
            nt.Start();
             * */
            newPMQWindow();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Will be enabled in a future build");
        }

        private void newPMQWindow()
        {

            Form1 sForm = new Form1();
            sForm.Show();
            sForm.doOpen();
            
        }
        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
