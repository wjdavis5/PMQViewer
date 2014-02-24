using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.ComponentModel;
namespace PMQViewer
{
    delegate void setProgressBarDlgt();
    delegate void setTxtMessageTextDlgt(string c);
    delegate void setProgressBarMaxDlgt(int e);
    delegate void setProgressBarMinDlgt(int e);
    delegate void setProgressBarValueDlgt(int e);
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
            tbView.SelectedIndexChanged += new EventHandler(tbView_SelectedIndexChanged);
           

            //AllocConsole();
        }

        /**
         * A list for each entry in the file.
         */ 
        private List<FileEntry> list;
        private int lastSelect= -1;
        private Queue<string> genericMsgQ = new Queue<string>();
        /**
         * String to store current file.
         */ 
        private String curFile;
        //private string curFilePath;

        /**
         * Boolean to detect if file has been saved.
         */ 
        private bool saved;
        private ContextMenuStrip cms = new ContextMenuStrip();
        /**
         * Boolean to detect if file has been modified.
         */ 
        private bool modified;

        /**
         * Binary file reader to read and interpret the
         * bytes.
         */ 
        private BinaryReader br;

        private MemoryStream mr;

        /**
         * Debug console
         */
        /*
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern Int32 AllocConsole(); 
         */ 

        /**
         * Loads current file into list of FileEntries.
         */
        public void CopyListViewToClipboard(ListView lv)
        {
            String buffer = "";

            
            for (int i = 0; i < lv.Columns.Count; i++)
            {
                buffer += (lv.Columns[i].Text);
                buffer +=("\t");
            }

            buffer +=("\n");
            //MessageBox.Show(lv.Items.Count.ToString());
            for (int i = 0; i < lv.Items.Count; i++)
            {
              //  MessageBox.Show(lv.Items[i].SubItems.Count.ToString());
                for (int j = 0; j < lv.Columns.Count; j++)
                {
                    if (lv.Items[i].SubItems[j].Text != "" & lv.Items[i].SubItems[j].Text != null)
                    {
                        /**
                         * if (i == 2)
                        {
                            int ei = 0;
                        }
                         * */
                        buffer += (lv.Items[i].SubItems[j].Text);
                        buffer += ("\t");
                    }
                    else
                    {
                        buffer += ("NULL");
                        buffer += ("\t");
                    }
                }

                buffer += ("\n");
            }

            Clipboard.SetText(buffer);
        }  
        private static bool feNotNull(FileEntry l)
        {
            if (l != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private void loadFile(String filename)
        {
            /**
             * Initialize variables.
             */ 
            saved = false;
            modified = false;
            if (br != null || mr != null)
            {
                if (list.Count > 0)
                {
                    /*
                    foreach (FileEntry l in list)
                    {
                       
                    }
                     * */
                  //  MessageBox.Show(list.RemoveAll(feNotNull).ToString());
                    list = null;

                }

                try
                {
                    br.Close();
                }
                catch(Exception){}
                try
                {
                    br.Dispose();
                }
                catch (Exception) { }
                try
                {
                    mr.Close();
                }
                catch (Exception) { }
                try
                {
                    mr.Dispose();
                }
                catch (Exception) { }

                br = null;
                mr = null;

            }
           // lblCurrentFile.Text = filename;
            br = new BinaryReader(File.OpenRead(filename));
            mr = new MemoryStream(unchecked((int)br.BaseStream.Length));
            br.BaseStream.CopyTo(mr, 1024);
            br.Close();
            br = null;
            br = new BinaryReader(mr);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            list = new List<FileEntry>();
            lstvwLogBase.Clear();
            lstvwParameters.Clear();
            int count = 0;
            //int ThreadCount = 0;
            //int ThreadTimerCount = 0;

            /**
             * Loop through file until done.
             */
            progressBar1.Maximum = (int)br.BaseStream.Length;
            progressBar1.Minimum = 0;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                long mStart = br.BaseStream.Position;
                int status = (((int)br.BaseStream.Position) / ((int)br.BaseStream.Length)) * 100;

                progressBar1.Value = (int)br.BaseStream.Position;
                pb1ShowPercent();
               // progressBar1.ProgressBar.createGraphics
                    
                count++;
                Int32 version;

                /**
                 * Read in I3 header.
                 */ 

                /**
                 * Read in version.
                 */ 
                try
                {
                    version = br.ReadInt32();
                }
                catch (EndOfStreamException)
                {
                    br.Close();
                    return;
                }

                /**
                 * If version does not equal 1 then quit.
                 * May not be necessary.
                 */ 
                if (version != 1)
                {
                    br.Close();
                   // MessageBox.Show("I BROKE! version is wrong!!");
                    return;
                }

                /**
                 * Read in creation time.
                 * Creation time format is binary file time.
                 */ 
                long nCreateTime = br.ReadInt64();

                /**
                 * Read in the number of bytes the message is.
                 */ 
                Int32 nBytes = br.ReadInt32();

                /**
                 * Read into char array for parsing.
                 * This is the threading point.
                 */
                //MessageBox.Show(offEnd + " :END\n" + offStart + " :Start\n" + (offEnd - offStart) + " :Diff");
                long finalPos = br.BaseStream.Position + nBytes;
                //MessageBox.Show("Current Pos:" + br.BaseStream.Position + "\nNBytes:" + nBytes + "\nFinalPos:" + finalPos);
                char[] c;
                long offset = br.BaseStream.Position;
                c = br.ReadChars(nBytes);
                if (finalPos != br.BaseStream.Position)
                {
                    br.BaseStream.Seek(finalPos, SeekOrigin.Begin);
                    /*
                    try
                    {
                        byte[] tempC = br.ReadBytes(nBytes-1);
                        //c = br.ReadChars(nBytes);
                        MemoryStream tempMS = new MemoryStream(nBytes);
                        BinaryWriter tempBW = new BinaryWriter(tempMS);
                        BinaryReader tempBR = new BinaryReader(tempMS);
                        //tempBW.BaseStream.Write(tempC, 0, nBytes);
                        c = tempBR.ReadChars(nBytes);
                        
                        tempBW.Close();
                        tempBR.Close();
                        tempBW = null;
                        tempMS = null;
                        tempBR = null;
                        if (c.Length <= 0)
                        {
                            /*
                                 byte[] tempbyteArray = new byte[(length * 2)];
                                 tempbyteArray = br.ReadBytes((length * 2));
                                 System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                                 message1 = enc.GetString(tempbyteArray);
                                 message1.Replace("\0", "");
                             
                            br.BaseStream.Position = offset;
                            c = br.ReadChars(nBytes);
                        }
                    }
            
                    catch (ArgumentException e)
                    {
                        MessageBox.Show(e.Message);
                        
                        return;
                    }
                    catch (ObjectDisposedException e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    catch (IOException e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
            **/
                }
                
                /**
                 * Read termination bytes.
                 */
                //byte[] end = br.ReadBytes((int)(nBytes - br.BaseStream.Position));
                Int32 end;
                if ((br.BaseStream.Length - br.BaseStream.Position) < 4)
                {
                    end = 0;
                    br.BaseStream.Position = br.BaseStream.Length;
                }
                else
                {
                    end = br.ReadInt32();
                    if (end != 0)
                    {
                        br.BaseStream.Seek(finalPos, SeekOrigin.Begin);
                        end = br.ReadInt32();
                    }
                }

                /**
                 * Create new file entry and add it to list.
                 * Display message entry in left listbox.
                 */ 
              //  FileEntry fe = new FileEntry(nCreateTime,nBytes,end,c,m,hex);
                 FileEntry fe = new FileEntry(nCreateTime, nBytes, end, c, offset,mStart);

                list.Add(fe);
                //DateTime date = DateTime.FromBinary(fe.getDate());
                DateTime date = DateTime.FromBinary(nCreateTime);
                
                date = date.AddYears(1600);
                //FileEntry(string msgDateTime, int msgSize, int version, string filename,long msgStart)
                
                //FileEntry fe = new FileEntry(nCreateTime, nBytes, version, filename, offset);
                lstOutput.Items.Add(date);
            }
            toolStripStatusLabel1.Text = curFile;
            if (list.Count > 1 || list.Count == 0)
            {
                toolStripStatusLabel2.Text = list.Count + " messages: ";
            }
            else
            {
                toolStripStatusLabel2.Text = list.Count + " message: ";
            }
            progressBar1.Value = 0;
            pb1ShowPercent();
           //br.Close();
        }
        private void pb1ShowPercent()
        {
            int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
            progressBar1.ProgressBar.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)7.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
        }
        private void pb2ShowPercent()
        {
            int percent = (int)(((double)toolStripProgressBar1.Value / (double)toolStripProgressBar1.Maximum) * 100);
            toolStripProgressBar1.ProgressBar.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)7.25, FontStyle.Regular), Brushes.Black, new PointF(toolStripProgressBar1.Width / 2 - 10, toolStripProgressBar1.Height / 2 - 7));
        }

       private int newPMQFile(int start, int length)
       {
           try
           {
               FolderBrowserDialog fbd = new FolderBrowserDialog();
               fbd.ShowNewFolderButton = true;
               //fbd.Container.Components.
               fbd.ShowDialog();
               /*
               SaveFileDialog sfd = new SaveFileDialog();
                 sfd.AddExtension = true;
               sfd.DefaultExt = ".i3p";
               sfd.ShowDialog();
             **/
               //if (sfd.FileName.Length > 0 && sfd.CheckPathExists)
               if (fbd.SelectedPath != "") 
               {
                   BinaryWriter newBR = new BinaryWriter(File.Create(fbd.SelectedPath + "\\PMQSplit_" + DateAndTime.DateString + ".i3p"));
                   byte[] buffer = new byte[(length + 1)];
                   br.BaseStream.Position = start;
                   buffer = br.ReadBytes(length);
                   
                   newBR.BaseStream.Write(buffer, start, length);
                   newBR.BaseStream.Close();

               }
               Process.Start(@fbd.SelectedPath);
           }
           catch (Exception e)
           {
               //MessageBox.Show(e.Message);
               return -1;
           }

           return 0;
       }
         private int newPMQFile(int start, int length,List<FileEntry> l)
       {
           try
           {
               FolderBrowserDialog fbd = new FolderBrowserDialog();
               fbd.ShowNewFolderButton = true;
               fbd.ShowDialog();
              // messagebax.show(fbd.SelectedPath);
               /*
               SaveFileDialog sfd = new SaveFileDialog();
                 sfd.AddExtension = true;
               sfd.DefaultExt = ".i3p";
                
               sfd.ShowDialog();
                * */
               //if (sfd.FileName.Length > 0 && sfd.CheckPathExists)
               if(fbd.SelectedPath != "")
               {
                   int index = 0;
                   progressBar1.Maximum = l.Count();
                   progressBar1.Minimum = 0;
                   progressBar1.Step = 1;
                   foreach (FileEntry fl in l)
                   {
                       progressBar1.PerformStep();
                       start = (int)fl.getStart();
                       length = (int)fl.getLength();
                       //BinaryWriter newBR = new BinaryWriter(File.Create(sfd.FileName + index.ToString()));
                       
                       BinaryWriter newBR = new BinaryWriter(File.Create(fbd.SelectedPath + "\\PMQSplit_" + index.ToString() + "_" + lblCurrentFile.Text));
                       byte[] buffer = new byte[(length + 1)];
                       br.BaseStream.Position = start;
                       buffer = br.ReadBytes(length);

                       newBR.BaseStream.Write(buffer, 0, length);
                       newBR.BaseStream.Close();
                       index += 1;
                       newBR.Dispose();
                       buffer = null;
                   }
               }
               Process.Start(@fbd.SelectedPath);
           }
           catch (Exception e)
           {
               //MessageBox.Show(e.Message);
               return -1;
           }
           progressBar1.Value = 0;
           pb1ShowPercent();
           return 0;
       }
      
        /**
         * Read in I3DBTran message and interpret it.
         */ 
        private I3DBTranMessage readI3DBTranmessage(char[] c)
        {
            try
            {

                /**
                 * Create a list to store each part of the message.
                 */
                List<String> s = new List<String>();
                string temp = "";

                /**
                 * Parse char array manually in order to deal with
                 * quotes around entries that happen to include pipes.
                 */
                if (this.InvokeRequired == true)
                {
                }
                else
                {
                    progressBar1.Maximum = c.Length;
                    progressBar1.Minimum = 0;
                }
                /*
                for (int j = 0; j < c.Length; j++)
                {
                    progressBar1.Value = j;
                    pb1ShowPercent();
                
                    if (c[j].Equals('"'))
                    {
                        j++;

                        progressBar1.Value = j;
                        pb1ShowPercent();
                        //temp += "\"";
                        try
                        {
                            while (!c[j].Equals('"'))
                            {
                            
                                temp += c[j];
                                j++;
                                progressBar1.Value = j;
                                pb1ShowPercent();
                            }
                        }
                        catch(IndexOutOfRangeException)
                        {

                        }

                    }
                 * */

                for (int j = 0; j < c.Length; j++)
                {
                    int isGuid = 0;
                    if (this.InvokeRequired == true)
                    {
                        //this.Invoke(new setProgressBarDlgt(setProgressBar));
                    }
                    else
                    {
                        progressBar1.Value = j;
                        pb1ShowPercent();
                    }

                    if (c[j].Equals('\0'))
                    {
                        continue;
                    }
                    if (c[j].Equals('\''))
                    {

                        temp += "\'\'";
                        continue;
                    }
                    if (c[j].Equals('"'))
                    {
                        /*
                         * So we encountered a double quote if we have already identified this as a guid disable that flag
                         * 
                        */
                        if (isGuid == 1)    
                        {
                            isGuid = 0;
                            continue;
                        }
                        /*
                         * Next we want to look 2 ahead and make sure that there is not another double quote !c[(j+2)].Equals('"'))
                         * Then we want to look 2 behind and make sure there is not a pipe c[(j - 2)].Equals('|')
                         * We also want to look 2 behind and see if there is a double quote and 2 ahead to see if there is a pipe.
                         * (c[(j - 2)].Equals('"') && c[(j + 2)].Equals('|'))
                        */
                        if (((!c[(j + 2)].Equals('"')) && c[(j - 2)].Equals('|')) || (c[(j - 2)].Equals('"') && c[(j + 2)].Equals('|')))  

                        {
                            /*
                             * Because we have two criteria that can make the if true we want to evaluate the first one again here.
                             * This seems inefficient to me but it works for now
                             * If we do not find a double quote 2 ahead and a pipe 2 behind
                             * then we will increment the counter by 2 to get to that position
                             * If it is false then we will just continue the loop
                             * All of this is required to correctly parse out GUIDs
                             * GUIDs can contain any number of characters. The previous method
                             * would fail to parse them correctly and they would not show in the dispaly
                             * As a result GetSQL failed to grab the guids too.
                             * This was all because some GUIDs contain pips | and quotes ' / " 
                            */
                            if (((!c[(j + 2)].Equals('"')) && c[(j - 2)].Equals('|')))
                            {
                                j++;
                                j++;

                                while (!c[j].Equals('"'))
                                {
                                    if (!c[j].Equals('\0'))
                                    {
                                        temp += c[j].ToString();
                                    }
                                    j++;
                                }
                                isGuid = 1;
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {

                            temp += c[j].ToString();

                            continue;
                        }

                    }
                    /*
                    if (c[j].Equals('"'))
                    {
                        if (c[(j + 1)].Equals('|') || c[(j - 1)].Equals('|'))
                        {
                    
                            continue;
                        }
                        else
                        {
                            temp += c[j].ToString();
                    
                            continue;
                        }

                    }
                     */

                    //temp += c[j];
                    //j++;

                    if (c[j].Equals('|'))
                    {
                        if (temp == "\"")
                        {
                            temp = "NULL";
                        }
                        s.Add(temp);
                        temp = "";
                        continue;
                    }

                    if (!c[j].Equals('\0'))
                    {
                        temp += c[j].ToString();
                        // j++;
                        continue;
                    }
                    /*
                    if (c[j].Equals('\0'))
                    {
                        int hi = 1;
                        hi++;
                    }
                     * */
                }

                String v;

                /**
                 * If this fails, then the user has selected
                 * the wrong mode.
                 */
                try
                {
                    v = s[0];
                }
                catch (Exception)
                {
                    return null;
                }

                String tempstr = "";

                /**
                 * Get those anoying squares out of the version.
                 */

                if (this.InvokeRequired == true)
                {
                }
                else
                {
                    progressBar1.Value = 0;

                    pb1ShowPercent();
                    progressBar1.Maximum = v.Count();
                    progressBar1.Minimum = 0;
                    progressBar1.Step = 1;
                }
                foreach (char ch in v)
                {
                    if (this.InvokeRequired == true)
                    {
                    }
                    else
                    {
                        progressBar1.PerformStep();
                        pb1ShowPercent();
                    }
                    if (Char.IsLetterOrDigit(ch))
                    {
                        tempstr += ch;
                    }
                }

                v = tempstr;

                /**
                 * Assign parsed data to string variables.
                 */

                String sectoken = s[1];
                String uid = s[2];
                String tranName = s[3];
                String procName = s[4];
                String provider = s[5];
                String connInfo = s[6];
                String catalog = s[7];
                String schema = s[8];
                String timeout = s[9];
                String txnid = s[10];
                String txnreq = s[11];
                String txniso = s[12];
                String rdonly = s[13];
                String includeParameterMetaData = s[14];
                int paramCount = Convert.ToInt32(s[15]);

                /**
                 * Create a list for the parameters.
                 */
                List<I3DBTranParameter> parameters = new List<I3DBTranParameter>(paramCount);
                int i = 16;

                /**
                 * For each parameter, convert data to correct value and then add it to the list.
                 */
                if (paramCount > 0)
                {
                    I3DBTRAN_PARAM_TYPES paramType;
                    I3DBTRAN_DATA_TYPES dataType;
                    if (this.InvokeRequired == true)
                    { }
                    else
                    {
                        progressBar1.Value = 0;
                        progressBar1.Maximum = (paramCount * 3 + 61);
                        progressBar1.Minimum = 0;
                        pb1ShowPercent();
                    }
                    while (i < (paramCount * 3 + 16))
                    {
                        if (this.InvokeRequired == true)
                        { }
                        else
                        {
                            progressBar1.Value = i;
                            pb1ShowPercent();
                        }
                        paramType = 0;
                        dataType = 0;

                        if (Enum.IsDefined(typeof(I3DBTRAN_PARAM_TYPES), Convert.ToInt32(s[i])))
                        {
                            paramType = (I3DBTRAN_PARAM_TYPES)Enum.Parse(typeof(I3DBTRAN_PARAM_TYPES), s[i], true);
                        }

                        i++;
                        if (this.InvokeRequired == true)
                        { }
                        else
                        {
                            progressBar1.Value = i;
                            pb1ShowPercent();
                        }

                        if (Enum.IsDefined(typeof(I3DBTRAN_DATA_TYPES), Convert.ToInt32(s[i])))
                        {
                            dataType = (I3DBTRAN_DATA_TYPES)Enum.Parse(typeof(I3DBTRAN_DATA_TYPES), s[i], true);
                        }

                        i++;
                        if (this.InvokeRequired == true)
                        { }
                        else
                        {
                            progressBar1.Value = i;
                            pb1ShowPercent();
                        }

                        String value = s[i];
                        parameters.Add(new I3DBTranParameter(paramType, dataType, value));

                        i++;
                        if (this.InvokeRequired == true)
                        { }
                        else
                        {
                            progressBar1.Value = i;
                            pb1ShowPercent();
                        }
                    }
                    i++;
                    if (this.InvokeRequired == true)
                    { }
                    else
                    {
                        progressBar1.Value = i;
                        pb1ShowPercent();
                    }
                }

                String rowLimit = s[i];
                String includeRowSetMetaData = s[i++];
                String additionalInformation = s[i];
                String errormsg = "";

                /**
                 * If there is an error message at the end, store it.
                 */
                if (this.InvokeRequired == true)
                { }
                else
                {
                    progressBar1.Value = 0;
                    pb1ShowPercent();
                    progressBar1.Maximum = c.Length;
                }

                for (i = 0; i < c.Length; i++)
                {
                    if (this.InvokeRequired == true)
                    { }
                    else
                    {
                        progressBar1.Value = i;
                        pb1ShowPercent();
                    }
                    if (c[i].Equals('#') && c[i + 2].Equals('E'))
                    {
                        while (i < c.Length)
                        {
                            if (!c[i].Equals('\0'))
                            {
                                errormsg += c[i];
                            }
                            i++;
                            if (this.InvokeRequired == true)
                            { }
                            else
                            {
                                progressBar1.Value = i;
                                pb1ShowPercent();
                            }
                        }
                    }
                }

                return new I3DBTranMessage(v, sectoken, uid, tranName, procName, provider, connInfo, catalog,
                                        schema, timeout, txnid, txnreq, txniso, rdonly, includeParameterMetaData,
                                        paramCount, parameters, rowLimit, includeRowSetMetaData, additionalInformation, errormsg);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        /**
         * Read in Log Base message.
         */
        object o = new object();
        private void ExecuteSecure(Action a)
        {
            try
            {
                if (InvokeRequired)
                {
                    lock (o)
                        Invoke(a);
                }
                else a();
            }
            catch (Exception) { }
        }
       
        public LogBaseMessage readLogBaseMessage(long offset)
        {
            
            
            br.BaseStream.Position = offset;
            List<LogBaseColumn> lst = new List<LogBaseColumn>();
            int i = 0;
            int schema = br.ReadInt32();
            LOG_IDS log = (LOG_IDS)br.ReadInt32();
            int creationtime = br.ReadInt32();
            DateTime date = new DateTime(1970,1,1).AddSeconds((double)creationtime);
            int hardcode = br.ReadInt32();
            LOG_QUEUE_MSG_TYPE msgtype = (LOG_QUEUE_MSG_TYPE)br.ReadInt32();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            int length = br.ReadByte();
            string message1 = "";
            string message2 = "";

            progressBar1.Value = 0;
            ExecuteSecure(() => progressBar1.Maximum = length * 2);
            
            pb1ShowPercent();

            for(i=0; i<length*2; i++)
            {
                progressBar1.Value = i;
               //ExecuteSecure(() => pb1ShowPercent());
                char c;
                try
                {
                    c = br.ReadChar();
                }catch(Exception) {continue;}



                if(!c.Equals('\0'))
                {
                    message1 += c;
                }
            }
             
            /*
            byte[] tempbyteArray = new byte[(length*2)];
            tempbyteArray = br.ReadBytes((length * 2));
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            message1 = enc.GetString(tempbyteArray);
            message1.Replace("\0", "");
            **/
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            length = br.ReadByte();

            //int delete1 = 2;
            //LOG_QUEUE_MSG_TYPE delete = (LOG_QUEUE_MSG_TYPE)delete1;
            /*
             * INSERT = 0,     //eLOG_QMTYPE_INSERT = 0,
               eLOG_QMTYPE_FIRST = 0,
               UPDATE = 1,     //eLOG_QMTYPE_UPDATE = 1,
               DELETE = 2,      //eLOG_QMTYPE_DELETE = 2,
               EXECUTE = 3,    //eLOG_QMTYPE_EXECUTE = 3,
               eLOG_QMTYPE_LAST = 3
             * 
             * */

            switch(msgtype)
            {
                case (LOG_QUEUE_MSG_TYPE)2: //delete
                    for(i=0; i<length*2; i++)
                     {
                       byte[] bytes = br.ReadBytes(4);
//                       char c = br.ReadChar();
                       char[] c = ASCIIEncoding.ASCII.GetChars(bytes);
                       
                       foreach (char c2 in c)
                        {
                            if (!c2.Equals('\0')) message2 += c2;
                            else continue;
                        }

                      }
                    return new LogBaseMessage(schema,(int)log,date,hardcode,msgtype,message1,message2,null);
                    

                case (LOG_QUEUE_MSG_TYPE)3:

                    if (log == (LOG_IDS)9999)
                    {
                       return null;
                        //MessageBox.Show(length.ToString());
                        
                        
                    }
                    else
                    {
                        int j = 0;
                        j += 1;
                        goto default;
                    }
                    

                default:
                    br.ReadByte();
                    br.ReadByte();
                    br.ReadInt32();

                    int columnsize = br.ReadInt32();
                    progressBar1.Value = 0;
                    pb1ShowPercent();
                    progressBar1.Maximum = columnsize;
                    for(i=0; i<columnsize; i++)
                    {
                        progressBar1.Value = i;
                        pb1ShowPercent();
                        LogBaseColumn temp = new LogBaseColumn();
                        int t;
    
                        try
                        {
    
                            t = br.ReadInt32();
                            switch (t)
                            {
                                //Double
                                case 1:
                                    double d = br.ReadInt64();
                                    temp = new LogBaseColumn((LOG_COLIN_TYPE)t, d.ToString());
                                    break;
                                //Long
                                case 2:
                                    long l = br.ReadInt32();
                                    temp = new LogBaseColumn((LOG_COLIN_TYPE)t, l.ToString());
                                    break;
                            //String
                                case 3:
                                    br.ReadByte();
                                    br.ReadByte();
                                    br.ReadByte();
                                    int len = br.ReadByte();
                                    string message = "";
                                    //len = len * 2;
                                    char[] ch;
                                    byte[] b = br.ReadBytes(len*2);
                                    ch = ASCIIEncoding.ASCII.GetChars(b);
                                    
                                    
                                    foreach(char ch2 in ch)
                                    {
                                        
                                        if (!ch2.Equals('\0'))
                                        {
                                            message += ch2;
                                        }
                                    }
                                    
                                    /*
                                    for (int j = 0; j < len; j++)
                                    {
                                        char ch;
                                        try
                                        {
                                            ch = br.ReadChar();
                                        }
                                        catch (Exception) { continue; }
                                        if (!ch.Equals('\0'))
                                        {
                                            message += ch;
                                        }
                                    }*/
                                    temp = new LogBaseColumn((LOG_COLIN_TYPE)t, message);
                                    break;
                            //time_t
                                case 4:
                                    int time_t = br.ReadInt32();
                                    DateTime dte = new DateTime(1970, 1, 1).AddSeconds(time_t);
                                    temp = new LogBaseColumn((LOG_COLIN_TYPE)t, dte.ToString());
                                    break;
                            }
                        }
                        catch (EndOfStreamException)
                        {
                            return null;
                        }

                    
                        lst.Add(temp);
                    }
                        break;

                }
            
            /*
            if(msgtype == delete)
            {
                for(i=0; i<length*2; i++)
                {
                    char c = br.ReadChar();
                    if(!c.Equals('\0'))
                    {
                        message2 += c;
                    }
                }
                return new LogBaseMessage(schema,log,date,hardcode,msgtype,message1,message2,null);
            }
                */
            
            
            
            return new LogBaseMessage(schema, (int)log, date, hardcode, msgtype,message1,message2,lst);
        }

        /**
         * When user clicks on a different message, display it.
         */ 
        private void lstOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (lstOutput.SelectedIndex != -1)
            {
                if (lastSelect == -1)
                {
                    lastSelect = lstOutput.SelectedIndex;
                }
                else
                {
                    try
                    {
                        list[lastSelect].setMsg(null);
                        list[lastSelect].setHex(null);
                        lastSelect = lstOutput.SelectedIndex;
                    }
                    catch (Exception)
                    { }
                }
                //Message m;
                int identifier = 0;
                if (rbI3DBTran.Checked)
                {
                    identifier = 1;
                    list[lstOutput.SelectedIndex].setMsg(readI3DBTranmessage(list[lstOutput.SelectedIndex].getRaw()));
                    if (list[lstOutput.SelectedIndex].getMsg() == null)
                    {
                        //this means it is not a I3DBTran Message
                        //lets try a log base or generic
                        list[lstOutput.SelectedIndex].isI3DBTran = 0;

                        if (list[lstOutput.SelectedIndex].isLog == -1)
                        {
                            rbLogBase.Checked = true;
                            //list[lstOutput.SelectedIndex].setMsg(
                            lstOutput_SelectedIndexChanged(sender, e);
                        }
                        else
                        {
                            rbGeneric.Checked = true;
                            lstOutput_SelectedIndexChanged(sender, e);
                        }
                        
                    }
                }
                else if (rbLogBase.Checked)
                {
                    identifier = 2;
                    list[lstOutput.SelectedIndex].setMsg(readLogBaseMessage(list[lstOutput.SelectedIndex].getOffset()));
                    if (list[lstOutput.SelectedIndex].getMsg() == null)
                    {
                        //this means it is not a log base Message
                        //lets try a generic or i3dbtran
                        list[lstOutput.SelectedIndex].isLog = 0;

                        if (list[lstOutput.SelectedIndex].isI3DBTran == -1)
                        {
                            rbI3DBTran.Checked = true;
                            lstOutput_SelectedIndexChanged(sender, e);
                        }
                        else
                        {
                            rbGeneric.Checked = true;
                            lstOutput_SelectedIndexChanged(sender, e);
                        }
                    }
                }
                else
                {
                    txtMessage.Text = "";
                    list[lstOutput.SelectedIndex].setMsg(new Message());
                }

                /**
                 * If m is null, then the user selected the wrong format.
                 * This also tells the user it is there problem.
                 */
                if (list[lstOutput.SelectedIndex].getMsg() == null)
                {
                    if (identifier == 1)
                    {
                        MessageBox.Show("PMQ file not an I3DBTran file!  Please choose a different mode!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (identifier == 2)
                    {
                        MessageBox.Show("PMQ file not a Log Base file!  Please choose a different mode!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Mode!! Please select a different mode or choose Generic!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    rbI3DBTran.Checked = false;
                    rbLogBase.Checked = false;
                    rbGeneric.Checked = false;
                    return;
                }
                FileEntry temp = list[lstOutput.SelectedIndex];

                txtHexView.Text = temp.getHex();

                /**
                 * If user has selected generic, display it.
                 */ 
                if (rbGeneric.Checked)
                {
                    char[] raw = temp.getRaw();
                    /**
                    txtMessage.Text = "";
                    char[] raw = temp.getRaw();
                    string output = String.Empty;
                    Encoding enc8 = Encoding.Unicode;
                    Decoder decoder8 = enc8.GetDecoder();
                    Byte[] bytes = new Byte[temp.getSize()];
                    br.BaseStream.Seek(temp.getStart(), SeekOrigin.Begin);

                    while (br.BaseStream.Position < temp.getStart()+temp.getSize())
                    {
                        int nBytes = br.BaseStream.Read(bytes, 0, bytes.Length);
                        int nChars = decoder8.GetCharCount(bytes, 0, nBytes);
                        char[] chars = new char[nChars];
                        nChars = decoder8.GetChars(bytes, 0, nBytes, chars, 0);
                        output += new String(chars, 0, nChars);
                    }
                    txtMessage.Text = output;
                    MessageBox.Show(output);
                     * **/
                    //string s = new String(raw);
                  //  txtMessage.Text = s;
                    
                    progressBar1.Value = 0;
                    progressBar1.Maximum = raw.Length;
                    progressBar1.Step = 1;
                    pb1ShowPercent();
                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback(charToString), raw);
                    

                    /*
                    foreach (char c6 in raw)
                    {
                        progressBar1.PerformStep();
                        pb1ShowPercent();
                        
                        if (Char.IsLetterOrDigit(c6))
                        {
                            txtMessage.Text += c6;
                        }
                        else if (c6.Equals('\0'))
                        {
                            //txtMessage.Text += c6;
                            continue;
                        }
                        else
                        {
                            txtMessage.Text += c6;
                            //txtMessage.Text += ".";
                        }
                         * */
                    
                        
                    
                     
                     
                   // if (s == s)
                   // {
                  //  }

                    
                }
                /**
                 * If user has selected I3DBTran mode, check if message
                 * is an I3DBTran message and then display it.
                 */ 
                else if (rbI3DBTran.Checked && isI3DBTranMessage(temp.getMsg()))
                {
                    /**
                     * Intialize variables.
                     */
                    lstvwData.Columns.Clear();
                    lstvwData.Items.Clear();
                    lstvwParameters.Columns.Clear();
                    I3DBTranMessage message = (I3DBTranMessage)temp.getMsg();
                    List<I3DBTranParameter> parameters = message.getParameters();
                    ListViewItem lvi;
                    //List<String> row = new List<String>(19 + 3*message.getParamCount());                    
                    List<String> row = new List<String>(19);
                    //string[] row = new string[19 + 3*message.getParamCount()];
                    lstvwParameters.Items.Clear();

                    /**
                     * Create columns.
                     */ 
                    //ColumnHeader version = new ColumnHeader();
                    ColumnHeader sectoken = new ColumnHeader();
                    ColumnHeader uid = new ColumnHeader();
                    ColumnHeader tranname = new ColumnHeader();
                    ColumnHeader procname = new ColumnHeader();
                    ColumnHeader prov = new ColumnHeader();
                    ColumnHeader conn = new ColumnHeader();
                    ColumnHeader catalog = new ColumnHeader();
                    ColumnHeader schema = new ColumnHeader();
                    ColumnHeader timeout = new ColumnHeader();
                    ColumnHeader txnid = new ColumnHeader();
                    ColumnHeader txnreq = new ColumnHeader();
                    ColumnHeader txniso = new ColumnHeader();
                    ColumnHeader rdonly = new ColumnHeader();
                    ColumnHeader ipmd = new ColumnHeader();
                    ColumnHeader paramcnt = new ColumnHeader();

                    /**
                     * Format created columns and add them to
                     * row array list and output.
                     */ 
                    /*
                    version.Text = "Format Version";
                    lstvwOutput.Columns.Add(version);
                    row.Add(message.getVersion());
                     */ 

                    sectoken.Text = "Security Token";
                    lstvwData.Columns.Add(sectoken);
                    row.Add(message.getSecToken());

                    uid.Text = "UID";
                    lstvwData.Columns.Add(uid);
                    row.Add(message.getUID());

                    tranname.Text = "TranName";
                    lstvwData.Columns.Add(tranname);
                    row.Add(message.getTranName());

                    procname.Text = "ProcName";
                    lstvwData.Columns.Add(procname);
                    row.Add(message.getProcName());

                    prov.Text = "Provider";
                    lstvwData.Columns.Add(prov);
                    row.Add(message.getProvider());

                    conn.Text = "ConnInfo";
                    lstvwData.Columns.Add(conn);
                    row.Add(message.getConnInfo());

                    catalog.Text = "Catalog";
                    lstvwData.Columns.Add(catalog);
                    row.Add(message.getCatalog());

                    schema.Text = "Schema";
                    lstvwData.Columns.Add(schema);
                    row.Add(message.getSchema());

                    timeout.Text = "Timeout";
                    lstvwData.Columns.Add(timeout);
                    row.Add(message.getTimeout());

                    txnid.Text = "TXNID";
                    lstvwData.Columns.Add(txnid);
                    row.Add(message.getTxnid());

                    txnreq.Text = "TXNREQ";
                    lstvwData.Columns.Add(txnreq);
                    row.Add(message.getTxnreq());

                    txniso.Text = "TXNISO";
                    lstvwData.Columns.Add(txniso);
                    row.Add(message.getTxniso());

                    rdonly.Text = "Read Only";
                    lstvwData.Columns.Add(rdonly);
                    row.Add(message.getReadOnly());

                    ipmd.Text = "Include Param Meta Data";
                    lstvwData.Columns.Add(ipmd);
                    row.Add(message.getIncludeParamMetadata());

                    paramcnt.Text = "Param Count";
                    lstvwData.Columns.Add(paramcnt);
                    row.Add(message.getParamCount().ToString());
                    
                    ColumnHeader rowlimit = new ColumnHeader();
                    ColumnHeader irsm = new ColumnHeader();
                    ColumnHeader ai = new ColumnHeader();
                    ColumnHeader errormsg = new ColumnHeader();

                    rowlimit.Text = "Row Limit";
                    lstvwData.Columns.Add(rowlimit);
                    row.Add(message.getRowLimit());

                    irsm.Text = "Include Row Set Meta Data";
                    lstvwData.Columns.Add(irsm);
                    row.Add(message.getIncludeRowSetMetaData());

                    ai.Text = "Additional Information";
                    lstvwData.Columns.Add(ai);
                    row.Add(message.getAdditionalInformation());

                    errormsg.Text = "Error Message";
                    lstvwData.Columns.Add(errormsg);
                    row.Add(message.getErrorMsg());

                    //lvi = new ListViewItem(row.ToArray<String>());
                    lvi = new ListViewItem(row.ToArray());

                    /**
                     * Display data.
                     */
                    lstvwData.Items.Add(lvi);

                    int k = 0;

                    List<ColumnHeader> header = new List<ColumnHeader>(message.getParamCount());

                    for (k = 0; k < header.Count; k++)
                    {
                        
                        header.Add(new ColumnHeader());
                    }

                    k = 0;

                    ListViewItem paramtyperow;
                    ListViewItem datatyperow;
                    ListViewItem datarow;
                    List<String> paramtype = new List<String>();
                    List<String> datatype = new List<String>();
                    List<String> data = new List<String>();

                    paramtype.Add("Param Type");
                    datatype.Add("Data Type");
                    data.Add("Value");
                    lstvwParameters.Columns.Add("                     ");

                    foreach (I3DBTranParameter i3 in parameters)
                    {
                        lstvwParameters.Columns.Add("Parameter " + (k + 1));
                        paramtype.Add(i3.getParamType().ToString());
                        datatype.Add(i3.getDataType().ToString());
                        data.Add(i3.getValue());
                        k++;
                    }

                    paramtyperow = new ListViewItem(paramtype.ToArray());
                    datatyperow = new ListViewItem(datatype.ToArray());
                    datarow = new ListViewItem(data.ToArray());

                    lstvwParameters.Items.Add(paramtyperow);
                    lstvwParameters.Items.Add(datatyperow);
                    lstvwParameters.Items.Add(datarow);

                    /**
                     * If header is bigger than row data, then use header size
                     * for column.  If row is bigger than header, then use row
                     * size.
                     */

                    for (int i = 0; i < lstvwData.Columns.Count; i++)
                    {
                        if (ColumnHeaderAutoResizeStyle.HeaderSize.CompareTo(ColumnHeaderAutoResizeStyle.ColumnContent) >= 0)
                        {
                            lstvwData.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                        else
                        {
                            lstvwData.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                    }

                    for (int i = 0; i < lstvwParameters.Columns.Count; i++)
                    {
                        if (ColumnHeaderAutoResizeStyle.HeaderSize.CompareTo(ColumnHeaderAutoResizeStyle.ColumnContent) >= 0)
                        {
                            lstvwParameters.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                        else
                        {
                            lstvwParameters.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                    }
                }
                /**
                 * Same thing as above except for Log Base Message.
                 */ 
                else if (rbLogBase.Checked && isLogBaseMessage(temp.getMsg()))
                {
                    lstvwLogBase.Columns.Clear();
                    LogBaseMessage message = (LogBaseMessage)temp.getMsg();
                    ListViewItem lvi;
                    List<String> row = new List<String>();
                    lstvwLogBase.Items.Clear();
                    //ColumnHeader schema = new ColumnHeader();
                    ColumnHeader log = new ColumnHeader();
                    ColumnHeader creationtime = new ColumnHeader();
                    //ColumnHeader hardcode = new ColumnHeader();
                    ColumnHeader msgtype = new ColumnHeader();
                    ColumnHeader message1 = new ColumnHeader();                    

                    /*
                    schema.Text = "Schema";
                    lstvwLogBase.Columns.Add(schema);
                    row.Add(message.getSchema().ToString());
                     */ 

                    /*
                    log.Text = "Report Log";
                    lstvwLogBase.Columns.Add(log);
                    row.Add(message.getLog().ToString());
                    */

                    if (Enum.IsDefined(typeof(LOG_IDS), Convert.ToInt32(message.getLog().ToString())))
                    {
                        txtReportLog.Text = ((LOG_IDS)Enum.Parse(typeof(LOG_IDS), message.getLog().ToString(), true)).ToString();
                    }
                    else
                    {
                        txtReportLog.Text = message.getLog().ToString();
                    }

                    /*
                    creationtime.Text = "Creation Time";
                    lstvwLogBase.Columns.Add(creationtime);
                    row.Add(message.getCreationTime().ToString());
                    */
                    txtCreationTime.Text = message.getCreationTime().ToString();
                    
                    /*
                    hardcode.Text = "Hard Code";
                    lstvwLogBase.Columns.Add(hardcode);
                    row.Add(message.getHardCode().ToString());
                    */

                    /*
                    msgtype.Text = "DB Operation";
                    lstvwLogBase.Columns.Add(msgtype);
                    row.Add(message.getMsgType().ToString());
                    */
                    txtDBOperation.Text = message.getMsgType().ToString();

                    /*
                    message1.Text = "Destination";
                    lstvwLogBase.Columns.Add(message1);
                    row.Add(message.getMessage1());
                    */
                    txtDestination.Text = message.getMessage1();

                    if (message.getMsgType().ToString().Equals("DELETE"))
                    {
                        ColumnHeader message2 = new ColumnHeader();
                        message2.Text = "WHERE Clause";
                        lstvwLogBase.Columns.Add(message2);
                        row.Add(message.getMessage2());
                        lvi = new ListViewItem(row.ToArray());
                        lstvwLogBase.Items.Add(lvi);
                    }
                    else
                    {
                        List<LogBaseColumn> lst = message.getList();
                        ListViewItem typerow;
                        ListViewItem datarow;
                        List<String> type = new List<String>();
                        List<String> data = new List<String>();
                        List<ColumnHeader> columns = new List<ColumnHeader>();

                        int i = 0;

                        for (i = 0; i < lst.Count; i++)
                        {
                            columns.Add(new ColumnHeader());
                        }

                        i = 0;

                        type.Add("Type");
                        data.Add("Data");
                        lstvwLogBase.Columns.Add("         ");

                        bool determine = determineColumnHeaders(message.getLog());

                        foreach (LogBaseColumn lg in lst)
                        {
                            type.Add(lg.getType().ToString());
                            data.Add(lg.getStr());

                            if (!determine)
                            {
                                lstvwLogBase.Columns.Add("Column " + (i + 1));
                            }
                            i++;
                        }

                        typerow = new ListViewItem(type.ToArray());
                        datarow = new ListViewItem(data.ToArray());
                        lstvwLogBase.Items.Add(typerow);
                        lstvwLogBase.Items.Add(datarow);
                    }                    

                    for (int j = 0; j < lstvwLogBase.Columns.Count; j++)
                    {
                        if (ColumnHeaderAutoResizeStyle.HeaderSize.CompareTo(ColumnHeaderAutoResizeStyle.ColumnContent) >= 0)
                        {
                            lstvwLogBase.Columns[j].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                        else
                        {
                            lstvwLogBase.Columns[j].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                    }
                }
            }
        }

        private void charToString(Object state)
        {
            string tempS = "";
            char[] raw = (char[])state;
            foreach (char c6 in raw)
            {
                if (!this.InvokeRequired)
                {
                    setProgressBar();
                }
                else
                {
                    this.Invoke(new setProgressBarDlgt(setProgressBar));
                }
                //this.setProgressBar();
                if (!c6.Equals('\0'))
                {
                    tempS += c6;
                }
               
                /*
                if (Char.IsLetterOrDigit(c6))
                {
                    txtMessage.Text += c6;
                }
                else if (c6.Equals('\0'))
                {
                    //txtMessage.Text += c6;
                    continue;
                }
                else
                {
                    txtMessage.Text += c6;
                    //txtMessage.Text += ".";
                }
                 * */

            }
            if (!this.InvokeRequired)
            {

                txtMessage.Text += tempS;
            }
            else
            {
                lock (genericMsgQ)
                {
                    this.Invoke(new setTxtMessageTextDlgt(setTxtMessageText), tempS);
                }
            }
        }

        private void setTxtMessageText(string txt)
        {
            try
            {
                txtMessage.Text = txt;
            }
            catch
            {
                return;
            }
        }
        private void setProgressBar()
        {
                progressBar1.PerformStep();
                pb1ShowPercent();
            
        }
        private void setProgressBar(int e)
        {
            progressBar1.Value = e;
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        private void splitIntoFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           


        }

        private void selectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newPMQFile((int)list[lstOutput.SelectedIndex].getStart(), (int)list[lstOutput.SelectedIndex].getLength()) < 0)
            {
                MessageBox.Show("FAILED!");
            }
            else
            {
                MessageBox.Show("Success!");
            }
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {


            newPMQFile(0, 0, list);
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab1 = new AboutBox1();
            ab1.ShowDialog();
        }

        private void coToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            //MessageBox.Show(menuItem.Name);
            if (menuItem != null)
            {
                ContextMenuStrip calendarMenu = menuItem.Owner as ContextMenuStrip;
              //  MessageBox.Show(calendarMenu.Name);
                if (calendarMenu != null)
                {
                    Control controlSelected = calendarMenu.SourceControl;
                //    MessageBox.Show(controlSelected.Name);
                    
                    CopyListViewToClipboard((ListView)controlSelected);
                }
            }
        }

     

        private void GetAllSQL(int i)
        {
            int counter = 0;
            string mydocpath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            StreamWriter outfile = new StreamWriter(mydocpath2 + @"\PMQTEST.SQL");

            foreach (FileEntry e in list)
            {
                e.setMsg(readI3DBTranmessage(e.getRaw()));
                if (e.getMsg() == null)  e.setMsg(readLogBaseMessage(e.getOffset()));
                
                if (isI3DBTranMessage(e.getMsg()))
                {
                    I3DBTranMessage t22 = (I3DBTranMessage)e.getMsg();
                    //MessageBox.Show(t2.getCompleteSproc());
                    
                    //StringBuilder sb = new StringBuilder();

                    // sb.AppendLine(t2.getCompleteSproc());
                    
                    
                        outfile.WriteLine(t22.getCompleteSproc(i));
                        //outfile.Write(t2.getCompleteSproc());
                        //outfile.app
                    
                }
                else if (isLogBaseMessage(e.getMsg()))
                {
                    LogBaseMessage t22 = (LogBaseMessage)e.getMsg();
                    
                }
                backgroundWorker1.ReportProgress((int)((counter / list.Count())*100));
                counter++;
            }
            outfile.Close();
            Process.Start(mydocpath2 + @"\PMQTEST.SQL");
        }

        private void GetAllSQL(int i,string path)
        {
            int counter = 0;
            string mydocpath2 = path;
            //StreamWriter outfile = new StreamWriter(mydocpath2 + @"\PMQTEST.SQL");

            foreach (FileEntry e in list)
            {
                
                e.setMsg(readI3DBTranmessage(e.getRaw()));
                if (isI3DBTranMessage(e.getMsg()))
                {

                    I3DBTranMessage t22 = (I3DBTranMessage)e.getMsg();
                    StreamWriter outfile = new StreamWriter(mydocpath2 + @"\" + t22.getProcName() + ".SQL");
                    //MessageBox.Show(t2.getCompleteSproc());

                    StringBuilder sb = new StringBuilder();

                    // sb.AppendLine(t2.getCompleteSproc());


                    //outfile.WriteLine(t22.getCompleteSproc(i));
                    outfile.Write(t22.getCompleteSproc(i));
                    outfile.Close();
                    //outfile.app

                }
                backgroundWorker1.ReportProgress((int)((counter / list.Count()) * 100));
                counter++;
            }
            //outfile.Close();
            //Process.Start(mydocpath2 + @"\PMQTEST.SQL");
            Process.Start(@path);
        }
        private void GetAllSQL2(int i,string path)
        {
            int counter = 0;
            string mydocpath2 = path;
            //StreamWriter outfile = new StreamWriter(mydocpath2 + @"\PMQTEST.SQL");

            foreach (FileEntry e in list)
            {
                /*
                e.setMsg(readI3DBTranmessage(e.getRaw()));
                if (isI3DBTranMessage(e.getMsg()))
                {

                    I3DBTranMessage t22 = (I3DBTranMessage)e.getMsg();
                    StreamWriter outfile = new StreamWriter(mydocpath2 + @"\" + t22.getProcName() + "_" + counter + ".SQL");
                    StringBuilder sb = new StringBuilder();
                    outfile.Write(t22.getCompleteSproc(i));
                    outfile.Close();
                }
                 * */
                object[] paramsL = new object[4];
                paramsL[0] = e;
                paramsL[1] = path;
                paramsL[2] = counter;
                paramsL[3] = i;
                object paramList = (object)paramsL;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadGetAllSQL2), paramList);
                backgroundWorker1.ReportProgress((int)((counter / list.Count()) * 100));
                counter++;
                /*
                int workerT;
                int cpT;
                ThreadPool.GetAvailableThreads(out workerT, out cpT);
                if(counter % 100 == 0 && counter>200)
                {
                   // MessageBox.Show(workerT.ToString() + "\r\n" + cpT.ToString());
                }
                 * */
            }
            Process.Start(@mydocpath2);
        }

        private void ThreadGetAllSQL2(object e)
        {
            ///ParamList is a list of all the parameters
            ///1 =  the fileentry object
            ///2 = the path
            ///3 = the count
            ///4 = the type (1 = sql 2 = oracle)
            
            object[] paramList = (object[])e;
            FileEntry param1 = (FileEntry)paramList[0];
            String param2 = (String)paramList[1];
            int param3 = (int)paramList[2];
            int param4 = (int)paramList[3];

            param1.setMsg(readI3DBTranmessage(param1.getRaw()));
            if (isI3DBTranMessage(param1.getMsg()))
            {

                I3DBTranMessage t22 = (I3DBTranMessage)param1.getMsg();
                StreamWriter outfile = new StreamWriter(param2 + @"\" + t22.getProcName() + "_" + param3 + ".SQL");
                StringBuilder sb = new StringBuilder();
                outfile.Write(t22.getCompleteSproc(param4));
                outfile.Close();
            }

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            GetAllSQL((int)e.Argument);
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            //toolStripProgressBar1.Paint;
        }
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            //toolStripProgressBar1.Paint;
        }

        

        private void sQLServerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                if (isI3DBTranMessage(list[lstOutput.SelectedIndex].getMsg()))
                {
                    I3DBTranMessage t2 = (I3DBTranMessage)list[lstOutput.SelectedIndex].getMsg();
                    //MessageBox.Show(t2.getCompleteSproc());
                    
                    StringBuilder sb = new StringBuilder();

                    // sb.AppendLine(t2.getCompleteSproc());

                    using (StreamWriter outfile =
                        new StreamWriter(mydocpath + @"\PMQTEST.SQL"))
                    {
                        outfile.Write(t2.getCompleteSproc(1));
                        outfile.Close();
                        Process.Start(mydocpath + @"\PMQTEST.SQL");
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                lstOutput.SelectedIndex = 0;
                sQLServerToolStripMenuItem1_Click(sender,e);
            }
        }

        private void oracleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                if (isI3DBTranMessage(list[lstOutput.SelectedIndex].getMsg()))
                {
                    I3DBTranMessage t2 = (I3DBTranMessage)list[lstOutput.SelectedIndex].getMsg();
                    //MessageBox.Show(t2.getCompleteSproc());
                    
                    StringBuilder sb = new StringBuilder();

                    // sb.AppendLine(t2.getCompleteSproc());

                    using (StreamWriter outfile =
                        new StreamWriter(mydocpath + @"\PMQTEST.SQL"))
                    {
                        outfile.Write(t2.getCompleteSproc(2));
                        outfile.Close();
                        Process.Start(mydocpath + @"\PMQTEST.SQL");

                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                lstOutput.SelectedIndex = 0;
                sQLServerToolStripMenuItem1_Click(sender, e);
            }
        }

        private void seperateFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = 2;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            while (fbd.SelectedPath.Length < 1)
            {
                fbd.ShowDialog();
            }
            object[] e2 = new object[2];
            e2[0] = i;
            e2[1] = fbd.SelectedPath;

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync(e2);

                toolStripProgressBar1.Maximum = 100;

            }
            else
            {
                MessageBox.Show("That thread has already been started");
            }
        }

        private void singleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = 2;
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync(i);

                toolStripProgressBar1.Maximum = 100;

            }
            else
            {
                MessageBox.Show("That thread has already been started");
            }
        }
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lstOutput.Items.Count > 0)
            {
                lstOutput.Items.Clear();
                lstOutput = null;
            }

            if (list.Count > 0)
            {
                
                list = null;
            }
            try
            {
                br.Close();
            }
            catch (Exception) { }
            try
            {
                br.Dispose();
            }
            catch (Exception) { }
            try
            {
                mr.Dispose();
            }
            catch (Exception) { }
            //list.Clear();
            try
            {
                mr = null;
            }
            catch (Exception) { }
            try
            {
                br = null;
            }
            catch (Exception) { }
            //list = null;
           // GC.Collect();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] e2 = (object[])e.Argument;

            GetAllSQL2((int)e2[0],(string)e2[1]);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void seperateFilesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = 1;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            while (fbd.SelectedPath.Length < 1)
            {
                fbd.ShowDialog();
            }
            object[] e2 = new object[2];
            e2[0] = i;
            e2[1] = fbd.SelectedPath;

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync(e2);

                toolStripProgressBar1.Maximum = 100;

            }
            else
            {
                MessageBox.Show("That thread has already been started");
            }
        }

        private void singleFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = 1;
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync(i);

                toolStripProgressBar1.Maximum = 100;

            }
            else
            {
                MessageBox.Show("That thread has already been started");
            }
        }

   

       
    
        
        /*
        private void rbI3DBTran_CheckedChanged(object sender, EventArgs e)
        {
            if (rbI3DBTran.Checked)
            {
                tbI3DBTran.Show();
            }
        }

        private void rbGeneric_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGeneric.Checked)
            {
                tbGeneric.Show();
            }
        }

        private void rbLogBase_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLogBase.Checked)
            {
                tabPage1.Show();
            }
        }
        **/
        }
        }

       
    

