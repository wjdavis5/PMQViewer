using System;

using System.Linq;
using Microsoft.VisualBasic;
using System.Threading;
using System.IO;
using System.Windows.Forms;
namespace PMQViewer
{
    /**
     * Stores each message in the file.
     */ 
    
    public class FileEntry
    {
        
        private long filetime;
        private int size;
        private int endbyte;
        private char[] raw;
        private Message msg;
        private String hex;
        private long offset;
        long msgstart;
        public int isLog = -1;
        public int isI3DBTran = -1;
        public int isGeneric = -1;
        public int isBstrError = -1;
        const string bstrRequestError = "01 3F 05 40 23 00 45 00 52 00 52 00 4F 00 \r\n52 00 23 00 55 00 6E 00 70 00 61 00 63 00 \r\n6B 00 42 00 53 00 54 00 52 00 52 00 65 00 \r\n71 00 75 00 65 00 73 00 74 00 3A 00 20 00 \r\n";
        public FileEntry()
        {
            filetime = 0;
            size = -1;
            endbyte = -1;
            hex = "";
        }
        public long getOffset()
        { return offset; }
        public void setType(int e, int o)
        {
            // o 1 = set type 
            // o 0 = set not type
            //0 = generic
            //1 = log
            //2 = i3dbtran
            if (o == 1)
            {
                switch (e)
                {
                    case 0:
                        isGeneric = 1;
                        break;
                    case 1:
                        isLog = 1;
                        break;
                    case 2:
                        isI3DBTran = 1;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e)
                {
                    case 0:
                        isGeneric = 0;
                        break;
                    case 1:
                        isLog = 0;
                        break;
                    case 2:
                        isI3DBTran = 0;
                        break;
                    default:
                        break;
                }
            }


        }
        public FileEntry(long d, int sze, int ebyte, char[] r, Message m, String h)
        {
            filetime = d;
            size = sze;
            endbyte = ebyte;
            raw = r;
            msg = m;
            hex = h;
        }
        public FileEntry(long d, int sze, int ebyte, char[] r, Message m)
        {
            filetime = d;
            size = sze;
            endbyte = ebyte;
            raw = r;
            msg = m;
            //hex = h;
            getHexConvert();
        }
        public FileEntry(long d, int sze, int ebyte, char[] r, long o)
        {
            filetime = d;
            size = sze;
            endbyte = ebyte;
            raw = r;
            //msg = m;
            //hex = h;
            offset = o;
            //getHexConvert();
        }
        public FileEntry(long d, int sze, int ebyte, char[] r, long o,long m)
        {
            filetime = d;
            size = sze;
            endbyte = ebyte;
            raw = r;
            //msg = m;
            //hex = h;
            offset = o;
            //getHexConvert();
            msgstart = m;

           
        }
        public FileEntry(long msgDateTime, int msgSize, int version, string filename,long msgStart)
        {
            String indexEntry = msgDateTime + "|";
            indexEntry += msgStart + "|";
            indexEntry += msgSize + "\r\n";
            //File.Open(,FileMode.OpenOrCreate);
            
            File.AppendAllText(filename + ".idx", indexEntry);

        }
        public long getStart()
        {
            return msgstart;
        }
        public long getLength()        {
            return (20 + size);
        }

        public Message getMsg()
        {
            return msg;
        }

        public void setMsg(Message m)
        {
            msg = m;
        }

        public String getHex()
        {
            convertHex();
            if (isBstrError == -1)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(checkForbstrError), hex);
            }
            return hex;
        }

        public void setHex(String h)
        {
            //we should check for the security token error here
            /*
              01 3F 05 40 23 00 45 00 52 00 52 00 4F 00 \r\n
              52 00 23 00 55 00 6E 00 70 00 61 00 63 00 \r\n
              6B 00 42 00 53 00 54 00 52 00 52 00 65 00 \r\n
              71 00 75 00 65 00 73 00 74 00 3A 00 20 00 \r\n
            */
            if (h != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(checkForbstrError), h);
            }
            //isBstrError = checkForbstrError(h);
            hex = h;
        }

        public char[] getRaw()
        {
            return raw;
        }

        public void setRaw(char[] r)
        {
            raw = r;
        }

        public int getEndByte()
        {
            return endbyte;
        }

        public void setEndByte(int ebyte)
        {
            endbyte = ebyte;
        }

        public int getSize()
        {
            return size;
        }

        public void setSize(int sze)
        {
            size = sze;
        }

        public long getDate()
        {
            return filetime;
        }

        public void setDate(long d)
        {
            filetime = d;
        }

        public override string ToString()
        {
            return base.ToString();
        }
        private void convertHex(object j)
        {
            if (hex != "" && hex != null)
            { return; }

            for (int i = 1; i < raw.Length + 1; i++)
            {
                int t = Strings.Asc(raw[i - 1]);
                string t2 = Conversion.Hex(t);

                if (t2.Length == 1)
                {
                    t2 = "0" + t2;
                }

                hex += (t2 + " ");

                if ((i % 14) == 0)
                {
                    hex += "\r\n";
                }
            }
            return;
           // Thread.CurrentThread.Abort();
        }
        private void convertHex()
        {
            if (hex != "" && hex != null)
            { return; }

            for (int i = 1; i < raw.Length + 1; i++)
            {
                int t = Strings.Asc(raw[i - 1]);
                string t2 = Conversion.Hex(t);

                if (t2.Length == 1)
                {
                    t2 = "0" + t2;
                }

                hex += (t2 + " ");

                if ((i % 14) == 0)
                {
                    hex += "\r\n";
                }
            }
            return;
           // Thread.CurrentThread.Abort();
        }

        public void getHexConvert()
        {
            //BackgroundWorker bw = new BackgroundWorker();
            //Thread td = new Thread(new ThreadStart(this.convertHex));
            //td.Start();
            
            ThreadPool.QueueUserWorkItem(new WaitCallback(convertHex));
        }
        public void checkForbstrError(object h)
        {

            try
            {
                if (h.ToString().Substring(0, bstrRequestError.Length) == bstrRequestError)
                {
                    isBstrError = 1;
                    return;
                }
                else
                {
                    isBstrError = 0;
                    return;
                }
            }
            catch (NullReferenceException)
            { }
        }


    }
}
