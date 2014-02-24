using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace PMQViewer
{
    public partial class Form1
    {
        /**
 * Generic delete for file.  It ignores what the actual
 * type of message is and just deletes the file entry that
 * corresponds to the message.
 */

        /**
        * Test if current Message is an I3DBTran message. This
        * only gets called if the user opens file in another mode,
        * like Log Base, and then selects I3DBTran mode and then clicks
        * on a message.
        */
        public bool isI3DBTranMessage(Message m)
        {
            try
            {
                if (m.GetType().ToString().Equals("PMQViewer.I3DBTranMessage"))
                {
                    return true;
                }
                else
                {
                    //MessageBox.Show("Cannot switch to I3DBTran Mode! (Reopen in I3DBTran mode if you know that it is an I3DBTran file.)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   //rbGeneric.Checked = true;
                    return false;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return false;
            }
        }

        /**
         * Same as above except visa versa.
         */
        public bool isLogBaseMessage(Message m)
        {
            try
            {
                if (m.GetType().ToString().Equals("PMQViewer.LogBaseMessage"))
                {
                    return true;
                }
                else
                {
                    //MessageBox.Show("Cannot switch to Log Base Mode! (Reopen in Log Base mode if you know that it is a Log Base file.)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //rbGeneric.Checked = true;
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (rbGeneric.Checked)
            {
                while (lstOutput.SelectedItems.Count > 0)
                {
                    lstOutput.Items.Remove(lstOutput.SelectedItem);
                }
                modified = true;
                txtMessage.Text = "";
            }
            else if (rbI3DBTran.Checked)
            {
                while (lstOutput.SelectedItems.Count > 0)
                {
                    list.RemoveAt(lstOutput.SelectedIndex);
                    lstOutput.Items.RemoveAt(lstOutput.SelectedIndex);
                }

                lstOutput.SelectedIndex = -1;

                modified = true;
                lstvwParameters.Clear();
            }
            else if (rbLogBase.Checked)
            {
                while (lstOutput.SelectedItems.Count > 0)
                {
                    list.RemoveAt(lstOutput.SelectedIndex);
                    lstOutput.Items.RemoveAt(lstOutput.SelectedIndex);
                }
                lstOutput.SelectedIndex = -1;

                modified = true;
                lstvwLogBase.Clear();
            }
        }

        /**
         * Saves the file.
         */
        private void cmdSave_Click(object sender, EventArgs e)
        {
            saveFile(curFile);
            toolStripStatusLabel1.Text = "Saved " + curFile;
        }

        /**
         * Saves the current file.  Again it ignores the
         * message type and just writes each file entry back
         * to the file.
         */
        private void saveFile(String filename)
        {
            if (filename != null)
            {
                BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create));

                foreach (FileEntry temp in list)
                {
                    Int32 version = 1;
                    Int64 date = temp.getDate();
                    Int32 size = temp.getSize();
                    int end = temp.getEndByte();
                    bw.Write(version);
                    bw.Write(date);
                    bw.Write(size);

                    for (int i = 0; i < size; i++)
                    {
                        bw.Write(temp.getRaw()[i]);
                    }

                    bw.Write(end);
                    bw.Flush();
                }
                bw.Close();
                saved = true;
            }
        }

        /**
         * Reloads the file if it has been changed and you need to revert.
         */
        private void cmdReload_Click(object sender, EventArgs e)
        {
            if (curFile == null)
            {
                return;
            }

            if (!saved && modified)
            {
                DialogResult result = MessageBox.Show("Do you want to save?", "Save?", MessageBoxButtons.YesNoCancel);

                if (result.Equals(DialogResult.Yes))
                {
                    saveFile(curFile);
                    lstOutput.Items.Clear();
                }
                else if (result.Equals(DialogResult.No))
                {
                    lstOutput.Items.Clear();
                }
                else
                {
                    return;
                }
                loadFile(curFile);
            }
            toolStripStatusLabel1.Text = curFile;
        }

        /**
         * Prompts user for file and opens it.
         */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rbGeneric.Checked || rbI3DBTran.Checked || rbLogBase.Checked)
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.DefaultExt = "*.*";
                fd.Filter = "All PMQ Files(*.I3P;*.ERR)|*.I3P;*.ERR|Buffered Files(*.I3P)|*.I3P|Error Files(*.ERR)|*.ERR|All files (*.*)|*.*";
                fd.ShowDialog();

                curFile = fd.FileName;
                //MessageBox.Show(curFile);
                
                //fd.SafeFileName

                if (curFile != String.Empty)
                {
                    lblCurrentFile.Text = fd.SafeFileName;
                    txtCreationTime.Text = "";
                    txtDBOperation.Text = "";
                    txtDestination.Text = "";
                    txtReportLog.Text = "";
                    lstOutput.Items.Clear();
                    txtMessage.Text = "";
                    txtHexView.Text = "";
                    mr.Dispose();
                    list.Clear();
                    GC.Collect();
                    

                    loadFile(curFile);
                }
            }
            else
            {
                MessageBox.Show("Please select a mode!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }



        public void doOpen()
        {
           // rbGeneric.Checked = true;
                OpenFileDialog fd = new OpenFileDialog();
                fd.DefaultExt = "*.*";
                fd.Filter = "All PMQ Files(*.I3P;*.ERR)|*.I3P;*.ERR|Buffered Files(*.I3P)|*.I3P|Error Files(*.ERR)|*.ERR|All files (*.*)|*.*";
                fd.ShowDialog();

                curFile = fd.FileName;
                //MessageBox.Show(curFile);

                //fd.SafeFileName

                if (curFile != String.Empty)
                {
                    lblCurrentFile.Text = fd.SafeFileName;
                    txtCreationTime.Text = "";
                    txtDBOperation.Text = "";
                    txtDestination.Text = "";
                    txtReportLog.Text = "";
                    lstOutput.Items.Clear();
                    txtMessage.Text = "";
                    txtHexView.Text = "";
                    loadFile(curFile);
                }
           
        }

        /**
         * Saves file under a different name.
         */
        private void exportToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.DefaultExt = "*.*";
            fd.Filter = "All PMQ Files(*.I3P;*.ERR)|*.I3P;*.ERR|Buffered Files(*.I3P)|*.I3P|Error Files(*.ERR)|*.ERR|All files (*.*)|*.*";
            fd.ShowDialog();

            String filename = fd.FileName;

            if (filename != String.Empty)
            {
                saveFile(filename);
                toolStripStatusLabel1.Text = "Saved " + curFile;
            }
        }

        /**
         * Calls the menu item to open file.
         */
        private void cmdOpen_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        /**
         * Toggles tab view for different modes.
         */

        private void rbI3DBTran_CheckedChanged(object sender, EventArgs e)
        {
            if (rbI3DBTran.Checked)
            {
                tbView.SelectedIndex = 0;
            }
        }

        private void rbGeneric_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGeneric.Checked)
            {
                tbView.SelectedIndex = 2;
            }
        }

        private void rbLogBase_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLogBase.Checked)
            {
                tbView.SelectedIndex = 1;
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdReload_Click(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdSave_Click(sender, e);
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdDelete_Click(sender, e);
        }

        private void tbView_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (tbView.SelectedIndex == 0)
            {
                if (!rbI3DBTran.Checked)
                {
                    rbI3DBTran.Checked = true;
                }
            }
            else if (tbView.SelectedIndex == 1)
            {
                if (!rbLogBase.Checked)
                {
                    rbLogBase.Checked = true;
                }
            }
            else
            {
                rbGeneric.Checked = true;
            }
        }

        private bool determineColumnHeaders(int id)
        {
            switch (id)
            {
                case 100:
                    lstvwLogBase.Columns.Add("cName");
                    lstvwLogBase.Columns.Add("cReportGroup");
                    lstvwLogBase.Columns.Add("cHKey3");
                    lstvwLogBase.Columns.Add("cHKey4");
                    lstvwLogBase.Columns.Add("cType");
                    lstvwLogBase.Columns.Add("dIntervalStart");
                    lstvwLogBase.Columns.Add("nDuration");
                    lstvwLogBase.Columns.Add("nEnteredAcd");
                    lstvwLogBase.Columns.Add("nAbandonedAcd");
                    lstvwLogBase.Columns.Add("nGrabbedAcd");
                    lstvwLogBase.Columns.Add("nLocalDisconnectAcd");
                    lstvwLogBase.Columns.Add("nAlertedAcd");
                    lstvwLogBase.Columns.Add("nAnsweredAcd");
                    lstvwLogBase.Columns.Add("nAnswered");
                    lstvwLogBase.Columns.Add("nAcdSvcLvl");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl1");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl2");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl3");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl4");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl5");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl6");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl1");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl2");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl3");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl4");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl5");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl6");
                    lstvwLogBase.Columns.Add("tGrabbedAcd");
                    lstvwLogBase.Columns.Add("tAnsweredAcd");
                    lstvwLogBase.Columns.Add("mtAnsweredAcd");
                    lstvwLogBase.Columns.Add("tAbandonedAcd");
                    lstvwLogBase.Columns.Add("tTalkAcd");
                    lstvwLogBase.Columns.Add("tTalkCompleteAcd");
                    lstvwLogBase.Columns.Add("nHoldAcd");
                    lstvwLogBase.Columns.Add("tHoldAcd");
                    lstvwLogBase.Columns.Add("nAcw");
                    lstvwLogBase.Columns.Add("tAcw");
                    lstvwLogBase.Columns.Add("tAcwComplete");
                    lstvwLogBase.Columns.Add("nExternToInternCalls");
                    lstvwLogBase.Columns.Add("nExternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("nInternToExternCalls");
                    lstvwLogBase.Columns.Add("nInternToExternAcdCalls");
                    lstvwLogBase.Columns.Add("nInternToInternCalls");
                    lstvwLogBase.Columns.Add("nInternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("tExternToInternCalls");
                    lstvwLogBase.Columns.Add("tExternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("tInternToExternCalls");
                    lstvwLogBase.Columns.Add("tInternToExternAcdCalls");
                    lstvwLogBase.Columns.Add("tInternToInternCalls");
                    lstvwLogBase.Columns.Add("tInternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("nAcwCalls");
                    lstvwLogBase.Columns.Add("tAcwCalls");
                    lstvwLogBase.Columns.Add("nTransferedAcd");
                    lstvwLogBase.Columns.Add("nNotAnsweredAcd");
                    lstvwLogBase.Columns.Add("tAlertedAcd");
                    lstvwLogBase.Columns.Add("nFlowOutAcd");
                    lstvwLogBase.Columns.Add("tFlowOutAcd");
                    lstvwLogBase.Columns.Add("nStartWaitAlertAcdCalls");
                    lstvwLogBase.Columns.Add("nStartActiveAcdCalls");
                    lstvwLogBase.Columns.Add("nStartHeldAcdCalls");
                    lstvwLogBase.Columns.Add("nEndWaitAlertAcdCalls");
                    lstvwLogBase.Columns.Add("nEndActiveAcdCalls");
                    lstvwLogBase.Columns.Add("nEndHeldAcdCalls");
                    lstvwLogBase.Columns.Add("nTransferWithinAcdCalls");
                    lstvwLogBase.Columns.Add("nTransferOutAcdCalls");
                    lstvwLogBase.Columns.Add("nDisconnectAcd");
                    lstvwLogBase.Columns.Add("tAgentLoggedIn");
                    lstvwLogBase.Columns.Add("tAgentAvailable");
                    lstvwLogBase.Columns.Add("tAgentTalk");
                    lstvwLogBase.Columns.Add("tAgentOtherBusy");
                    lstvwLogBase.Columns.Add("tAgentOnAcdCall");
                    lstvwLogBase.Columns.Add("tAgentOnOtherAcdCall");
                    lstvwLogBase.Columns.Add("tAgentInAcw");
                    lstvwLogBase.Columns.Add("tAgentOnNonAcdCall");
                    lstvwLogBase.Columns.Add("tAgentDnd");
                    lstvwLogBase.Columns.Add("tAgentNotAvailable");
                    lstvwLogBase.Columns.Add("tAgentAcdLoggedIn");
                    lstvwLogBase.Columns.Add("tAgentStatusDnd");
                    lstvwLogBase.Columns.Add("tAgentStatusAcw");
                    lstvwLogBase.Columns.Add("tAgentLoggedInDiluted");
                    lstvwLogBase.Columns.Add("tStatusGroupFollowup");
                    lstvwLogBase.Columns.Add("tStatusGroupBreak");
                    lstvwLogBase.Columns.Add("tStatusGroupTraining");
                    lstvwLogBase.Columns.Add("CustomValue1");
                    lstvwLogBase.Columns.Add("CustomValue2");
                    lstvwLogBase.Columns.Add("CustomValue3");
                    lstvwLogBase.Columns.Add("CustomValue4");
                    lstvwLogBase.Columns.Add("CustomValue5");
                    lstvwLogBase.Columns.Add("CustomValue6");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    return true;
                case 101:
                    lstvwLogBase.Columns.Add("cName");
                    lstvwLogBase.Columns.Add("cReportGroup");
                    lstvwLogBase.Columns.Add("cHKey3");
                    lstvwLogBase.Columns.Add("cHKey4");
                    lstvwLogBase.Columns.Add("cType");
                    lstvwLogBase.Columns.Add("dIntervalStart");
                    lstvwLogBase.Columns.Add("nDuration");
                    lstvwLogBase.Columns.Add("nEnteredAcd");
                    lstvwLogBase.Columns.Add("nAbandonedAcd");
                    lstvwLogBase.Columns.Add("nGrabbedAcd");
                    lstvwLogBase.Columns.Add("nLocalDisconnectAcd");
                    lstvwLogBase.Columns.Add("nAlertedAcd");
                    lstvwLogBase.Columns.Add("nAnsweredAcd");
                    lstvwLogBase.Columns.Add("nAnswered");
                    lstvwLogBase.Columns.Add("nAcdSvcLvl");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl1");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl2");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl3");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl4");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl5");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl6");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl1");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl2");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl3");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl4");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl5");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl6");
                    lstvwLogBase.Columns.Add("tGrabbedAcd");
                    lstvwLogBase.Columns.Add("tAnsweredAcd");
                    lstvwLogBase.Columns.Add("mtAnsweredAcd");
                    lstvwLogBase.Columns.Add("tAbandonedAcd");
                    lstvwLogBase.Columns.Add("tTalkAcd");
                    lstvwLogBase.Columns.Add("tTalkCompleteAcd");
                    lstvwLogBase.Columns.Add("nHoldAcd");
                    lstvwLogBase.Columns.Add("tHoldAcd");
                    lstvwLogBase.Columns.Add("nAcw");
                    lstvwLogBase.Columns.Add("tAcw");
                    lstvwLogBase.Columns.Add("tAcwComplete");
                    lstvwLogBase.Columns.Add("nExternToInternCalls");
                    lstvwLogBase.Columns.Add("nExternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("nInternToExternCalls");
                    lstvwLogBase.Columns.Add("nInternToExternAcdCalls");
                    lstvwLogBase.Columns.Add("nInternToInternCalls");
                    lstvwLogBase.Columns.Add("nInternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("tExternToInternCalls");
                    lstvwLogBase.Columns.Add("tExternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("tInternToExternCalls");
                    lstvwLogBase.Columns.Add("tInternToExternAcdCalls");
                    lstvwLogBase.Columns.Add("tInternToInternCalls");
                    lstvwLogBase.Columns.Add("tInternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("nAcwCalls");
                    lstvwLogBase.Columns.Add("tAcwCalls");
                    lstvwLogBase.Columns.Add("nTransferedAcd");
                    lstvwLogBase.Columns.Add("nNotAnsweredAcd");
                    lstvwLogBase.Columns.Add("tAlertedAcd");
                    lstvwLogBase.Columns.Add("nFlowOutAcd");
                    lstvwLogBase.Columns.Add("tFlowOutAcd");
                    lstvwLogBase.Columns.Add("nStartWaitAlertAcdCalls");
                    lstvwLogBase.Columns.Add("nStartActiveAcdCalls");
                    lstvwLogBase.Columns.Add("nStartHeldAcdCalls");
                    lstvwLogBase.Columns.Add("nEndWaitAlertAcdCalls");
                    lstvwLogBase.Columns.Add("nEndActiveAcdCalls");
                    lstvwLogBase.Columns.Add("nEndHeldAcdCalls");
                    lstvwLogBase.Columns.Add("nTransferWithinAcdCalls");
                    lstvwLogBase.Columns.Add("nTransferOutAcdCalls");
                    lstvwLogBase.Columns.Add("nDisconnectAcd");
                    lstvwLogBase.Columns.Add("tAgentLoggedIn");
                    lstvwLogBase.Columns.Add("tAgentAvailable");
                    lstvwLogBase.Columns.Add("tAgentTalk");
                    lstvwLogBase.Columns.Add("tAgentOtherBusy");
                    lstvwLogBase.Columns.Add("tAgentOnAcdCall");
                    lstvwLogBase.Columns.Add("tAgentOnOtherAcdCall");
                    lstvwLogBase.Columns.Add("tAgentInAcw");
                    lstvwLogBase.Columns.Add("tAgentOnNonAcdCall");
                    lstvwLogBase.Columns.Add("tAgentDnd");
                    lstvwLogBase.Columns.Add("tAgentNotAvailable");
                    lstvwLogBase.Columns.Add("tAgentAcdLoggedIn");
                    lstvwLogBase.Columns.Add("tAgentStatusDnd");
                    lstvwLogBase.Columns.Add("tAgentStatusAcw");
                    lstvwLogBase.Columns.Add("tAgentLoggedInDiluted");
                    lstvwLogBase.Columns.Add("tStatusGroupFollowup");
                    lstvwLogBase.Columns.Add("tStatusGroupBreak");
                    lstvwLogBase.Columns.Add("tStatusGroupTraining");
                    lstvwLogBase.Columns.Add("CustomValue1");
                    lstvwLogBase.Columns.Add("CustomValue2");
                    lstvwLogBase.Columns.Add("CustomValue3");
                    lstvwLogBase.Columns.Add("CustomValue4");
                    lstvwLogBase.Columns.Add("CustomValue5");
                    lstvwLogBase.Columns.Add("CustomValue6");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    return true;
                case 102:
                    lstvwLogBase.Columns.Add("cName");
                    lstvwLogBase.Columns.Add("cReportGroup");
                    lstvwLogBase.Columns.Add("cHKey3");
                    lstvwLogBase.Columns.Add("cHKey4");
                    lstvwLogBase.Columns.Add("cType");
                    lstvwLogBase.Columns.Add("dIntervalStart");
                    lstvwLogBase.Columns.Add("nDuration");
                    lstvwLogBase.Columns.Add("nEnteredAcd");
                    lstvwLogBase.Columns.Add("nAbandonedAcd");
                    lstvwLogBase.Columns.Add("nGrabbedAcd");
                    lstvwLogBase.Columns.Add("nLocalDisconnectAcd");
                    lstvwLogBase.Columns.Add("nAlertedAcd");
                    lstvwLogBase.Columns.Add("nAnsweredAcd");
                    lstvwLogBase.Columns.Add("nAnswered");
                    lstvwLogBase.Columns.Add("nAcdSvcLvl");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl1");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl2");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl3");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl4");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl5");
                    lstvwLogBase.Columns.Add("nAnsweredAcdSvcLvl6");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl1");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl2");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl3");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl4");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl5");
                    lstvwLogBase.Columns.Add("nAbandonAcdSvcLvl6");
                    lstvwLogBase.Columns.Add("tGrabbedAcd");
                    lstvwLogBase.Columns.Add("tAnsweredAcd");
                    lstvwLogBase.Columns.Add("mtAnsweredAcd");
                    lstvwLogBase.Columns.Add("tAbandonedAcd");
                    lstvwLogBase.Columns.Add("tTalkAcd");
                    lstvwLogBase.Columns.Add("tTalkCompleteAcd");
                    lstvwLogBase.Columns.Add("nHoldAcd");
                    lstvwLogBase.Columns.Add("tHoldAcd");
                    lstvwLogBase.Columns.Add("nAcw");
                    lstvwLogBase.Columns.Add("tAcw");
                    lstvwLogBase.Columns.Add("tAcwComplete");
                    lstvwLogBase.Columns.Add("nExternToInternCalls");
                    lstvwLogBase.Columns.Add("nExternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("nInternToExternCalls");
                    lstvwLogBase.Columns.Add("nInternToExternAcdCalls");
                    lstvwLogBase.Columns.Add("nInternToInternCalls");
                    lstvwLogBase.Columns.Add("nInternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("tExternToInternCalls");
                    lstvwLogBase.Columns.Add("tExternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("tInternToExternCalls");
                    lstvwLogBase.Columns.Add("tInternToExternAcdCalls");
                    lstvwLogBase.Columns.Add("tInternToInternCalls");
                    lstvwLogBase.Columns.Add("tInternToInternAcdCalls");
                    lstvwLogBase.Columns.Add("nAcwCalls");
                    lstvwLogBase.Columns.Add("tAcwCalls");
                    lstvwLogBase.Columns.Add("nTransferedAcd");
                    lstvwLogBase.Columns.Add("nNotAnsweredAcd");
                    lstvwLogBase.Columns.Add("tAlertedAcd");
                    lstvwLogBase.Columns.Add("nFlowOutAcd");
                    lstvwLogBase.Columns.Add("tFlowOutAcd");
                    lstvwLogBase.Columns.Add("nStartWaitAlertAcdCalls");
                    lstvwLogBase.Columns.Add("nStartActiveAcdCalls");
                    lstvwLogBase.Columns.Add("nStartHeldAcdCalls");
                    lstvwLogBase.Columns.Add("nEndWaitAlertAcdCalls");
                    lstvwLogBase.Columns.Add("nEndActiveAcdCalls");
                    lstvwLogBase.Columns.Add("nEndHeldAcdCalls");
                    lstvwLogBase.Columns.Add("nTransferWithinAcdCalls");
                    lstvwLogBase.Columns.Add("nTransferOutAcdCalls");
                    lstvwLogBase.Columns.Add("nDisconnectAcd");
                    lstvwLogBase.Columns.Add("tAgentLoggedIn");
                    lstvwLogBase.Columns.Add("tAgentAvailable");
                    lstvwLogBase.Columns.Add("tAgentTalk");
                    lstvwLogBase.Columns.Add("tAgentOtherBusy");
                    lstvwLogBase.Columns.Add("tAgentOnAcdCall");
                    lstvwLogBase.Columns.Add("tAgentOnOtherAcdCall");
                    lstvwLogBase.Columns.Add("tAgentInAcw");
                    lstvwLogBase.Columns.Add("tAgentOnNonAcdCall");
                    lstvwLogBase.Columns.Add("tAgentDnd");
                    lstvwLogBase.Columns.Add("tAgentNotAvailable");
                    lstvwLogBase.Columns.Add("tAgentAcdLoggedIn");
                    lstvwLogBase.Columns.Add("tAgentStatusDnd");
                    lstvwLogBase.Columns.Add("tAgentStatusAcw");
                    lstvwLogBase.Columns.Add("tAgentLoggedInDiluted");
                    lstvwLogBase.Columns.Add("tStatusGroupFollowup");
                    lstvwLogBase.Columns.Add("tStatusGroupBreak");
                    lstvwLogBase.Columns.Add("tStatusGroupTraining");
                    lstvwLogBase.Columns.Add("CustomValue1");
                    lstvwLogBase.Columns.Add("CustomValue2");
                    lstvwLogBase.Columns.Add("CustomValue3");
                    lstvwLogBase.Columns.Add("CustomValue4");
                    lstvwLogBase.Columns.Add("CustomValue5");
                    lstvwLogBase.Columns.Add("CustomValue6");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    return true;
                case 11:
                    lstvwLogBase.Columns.Add("CallId");
                    lstvwLogBase.Columns.Add("CallType");
                    lstvwLogBase.Columns.Add("CallDirection");
                    lstvwLogBase.Columns.Add("LineId");
                    lstvwLogBase.Columns.Add("StationId");
                    lstvwLogBase.Columns.Add("LocalUserId");
                    lstvwLogBase.Columns.Add("LocalNumber");
                    lstvwLogBase.Columns.Add("LocalName");
                    lstvwLogBase.Columns.Add("RemoteNumber");
                    lstvwLogBase.Columns.Add("RemoteNumberCountry");
                    lstvwLogBase.Columns.Add("RemoteNumberLoComp1");
                    lstvwLogBase.Columns.Add("RemoteNumberLoComp2");
                    lstvwLogBase.Columns.Add("RemoteNumberFmt");
                    lstvwLogBase.Columns.Add("RemoteNumberCallId");
                    lstvwLogBase.Columns.Add("RemoteName");
                    lstvwLogBase.Columns.Add("InitiatedDate");
                    lstvwLogBase.Columns.Add("InitiatedDateTimeGmt");
                    lstvwLogBase.Columns.Add("ConnectedDate");
                    lstvwLogBase.Columns.Add("ConnectedDateTimeGmt");
                    lstvwLogBase.Columns.Add("TerminatedDate");
                    lstvwLogBase.Columns.Add("TerminatedDateTimeGmt");
                    lstvwLogBase.Columns.Add("CallDurationSeconds");
                    lstvwLogBase.Columns.Add("HoldDurationSeconds");
                    lstvwLogBase.Columns.Add("LineDurationSeconds");
                    lstvwLogBase.Columns.Add("DNIS");
                    lstvwLogBase.Columns.Add("CallEventLog");
                    lstvwLogBase.Columns.Add("CustomNum1");
                    lstvwLogBase.Columns.Add("CustomNum2");
                    lstvwLogBase.Columns.Add("CustomNum3");
                    lstvwLogBase.Columns.Add("CustomString1");
                    lstvwLogBase.Columns.Add("CustomString2");
                    lstvwLogBase.Columns.Add("CustomString3");
                    lstvwLogBase.Columns.Add("CustomDateTime");
                    lstvwLogBase.Columns.Add("CustomDateTimeGmt");
                    lstvwLogBase.Columns.Add("InteractionType");
                    lstvwLogBase.Columns.Add("AccountCode");
                    lstvwLogBase.Columns.Add("PurposeCode");
                    lstvwLogBase.Columns.Add("DispositionCode");
                    lstvwLogBase.Columns.Add("CallNote");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    lstvwLogBase.Columns.Add("WrapUpCode");
                    return true;
                case 80:
                    lstvwLogBase.Columns.Add("UserId");
                    lstvwLogBase.Columns.Add("StatusDateTime");
                    lstvwLogBase.Columns.Add("StatusDateTimeGMT");
                    lstvwLogBase.Columns.Add("ChangedStatus");
                    lstvwLogBase.Columns.Add("ChangedStatusGroup");
                    lstvwLogBase.Columns.Add("ChangedLoggedIn");
                    lstvwLogBase.Columns.Add("ChangedAcdLoggedIn");
                    lstvwLogBase.Columns.Add("StatusKey");
                    lstvwLogBase.Columns.Add("StatusGroup");
                    lstvwLogBase.Columns.Add("LoggedIn");
                    lstvwLogBase.Columns.Add("AcdLoggedIn");
                    lstvwLogBase.Columns.Add("StatusDnd");
                    lstvwLogBase.Columns.Add("StatusAcw");
                    lstvwLogBase.Columns.Add("EndDateTime");
                    lstvwLogBase.Columns.Add("EndDateTimeGMT");
                    lstvwLogBase.Columns.Add("StateDuration");
                    lstvwLogBase.Columns.Add("SeqNo");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    return true;
                case 81:
                    lstvwLogBase.Columns.Add("GroupId");
                    lstvwLogBase.Columns.Add("dIntervalStart");
                    lstvwLogBase.Columns.Add("nDuration");
                    lstvwLogBase.Columns.Add("nEntered");
                    lstvwLogBase.Columns.Add("mEntered");
                    lstvwLogBase.Columns.Add("tActiveLines");
                    lstvwLogBase.Columns.Add("tAllBusy");
                    lstvwLogBase.Columns.Add("tSeized");
                    lstvwLogBase.Columns.Add("nEnteredOutbound");
                    lstvwLogBase.Columns.Add("nOutboundBlocked");
                    lstvwLogBase.Columns.Add("tResourceAvailable");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    return true;
                case 82:
                    lstvwLogBase.Columns.Add("LineId");
                    lstvwLogBase.Columns.Add("dIntervalStart");
                    lstvwLogBase.Columns.Add("nDuration");
                    lstvwLogBase.Columns.Add("nEntered");
                    lstvwLogBase.Columns.Add("tSeized");
                    lstvwLogBase.Columns.Add("nEnteredOutbound");
                    lstvwLogBase.Columns.Add("nOutboundBlocked");
                    lstvwLogBase.Columns.Add("tResourceAvailable");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    return true;
                case 83:
                    lstvwLogBase.Columns.Add("EnvelopeId");
                    lstvwLogBase.Columns.Add("EnvelopeTimeStamp");
                    lstvwLogBase.Columns.Add("FaxId");
                    lstvwLogBase.Columns.Add("FaxTimestamp");
                    lstvwLogBase.Columns.Add("CallIdKey");
                    lstvwLogBase.Columns.Add("ProcessingDatetime");
                    lstvwLogBase.Columns.Add("ProcessingDateTimeGmt");
                    lstvwLogBase.Columns.Add("Direction");
                    lstvwLogBase.Columns.Add("SuccessFlag");
                    lstvwLogBase.Columns.Add("RemoteCSId");
                    lstvwLogBase.Columns.Add("RemoteNumber");
                    lstvwLogBase.Columns.Add("T30");
                    lstvwLogBase.Columns.Add("PortNumber");
                    lstvwLogBase.Columns.Add("Duration");
                    lstvwLogBase.Columns.Add("Speed");
                    lstvwLogBase.Columns.Add("PageCount");
                    lstvwLogBase.Columns.Add("ErrorInfo");
                    lstvwLogBase.Columns.Add("SignalQuality");
                    lstvwLogBase.Columns.Add("SignalStrength");
                    lstvwLogBase.Columns.Add("LineNoise");
                    lstvwLogBase.Columns.Add("Header");
                    lstvwLogBase.Columns.Add("SendWhen");
                    lstvwLogBase.Columns.Add("CheapBeginDateTime");
                    lstvwLogBase.Columns.Add("CheapEndDateTime");
                    lstvwLogBase.Columns.Add("ScheduledDateTime");
                    lstvwLogBase.Columns.Add("Retries");
                    lstvwLogBase.Columns.Add("RetryDelay");
                    lstvwLogBase.Columns.Add("SubmitDateTime");
                    lstvwLogBase.Columns.Add("SubmitDateTimeGMT");
                    lstvwLogBase.Columns.Add("SenderName");
                    lstvwLogBase.Columns.Add("NotifyOnSuccess");
                    lstvwLogBase.Columns.Add("SuccessAddress");
                    lstvwLogBase.Columns.Add("NotifyOnFailure");
                    lstvwLogBase.Columns.Add("FailureAddress");
                    lstvwLogBase.Columns.Add("FailureAttempts");
                    lstvwLogBase.Columns.Add("FailureType");
                    lstvwLogBase.Columns.Add("MaxBPS");
                    lstvwLogBase.Columns.Add("DeviceGroup");
                    lstvwLogBase.Columns.Add("CoverPageName");
                    lstvwLogBase.Columns.Add("ToCompany");
                    lstvwLogBase.Columns.Add("ToName");
                    lstvwLogBase.Columns.Add("ToVoicePhone");
                    lstvwLogBase.Columns.Add("FromName");
                    lstvwLogBase.Columns.Add("FromFaxPhone");
                    lstvwLogBase.Columns.Add("FromVoicePhone");
                    lstvwLogBase.Columns.Add("FromCompany");
                    lstvwLogBase.Columns.Add("FaxComment");
                    lstvwLogBase.Columns.Add("LocalCSId");
                    lstvwLogBase.Columns.Add("SiteId");
                    lstvwLogBase.Columns.Add("SubSiteId");
                    lstvwLogBase.Columns.Add("");
                    return true;
            }
            return false;
        }

       

    }
}
