using System;
using System.Collections.Generic;
//using System.Linq;


public enum LOG_QUEUE_MSG_TYPE
{
    INSERT = 0,     //eLOG_QMTYPE_INSERT = 0,
    eLOG_QMTYPE_FIRST = 0,
    UPDATE = 1,     //eLOG_QMTYPE_UPDATE = 1,
    DELETE = 2,     //eLOG_QMTYPE_DELETE = 2,
    EXECUTE = 3,    //eLOG_QMTYPE_EXECUTE = 3,
    eLOG_QMTYPE_LAST = 3
};

public enum LOG_IDS
{
      CallDetail = 11, //eLOG_ID_CALL_DETAIL           = 11,
      IAChangeLog = 7, //eLOG_ID_IA_CHANGE_NOTIF       = 7,
      ICDirChangeLog = 8, //eLOG_ID_EIC_CHANGE_NOTIF      = 8,
      IAgentQueueStats = 100, //eLOG_ID_IAGENT_QUEUE_STATS    = 100,
      IStatsGroup = 101, //eLOG_ID_ISTATS_GROUP          = 101,
      IWrkgrpQueueStats = 102, //eLOG_ID_IWRKGRP_QUEUE_STATS   = 102,
      IWrapUpStats = 93, //eLOG_ID_WRAPUP_STATS          = 93,
      UserWorkgroups = 70, //eLOG_ID_WRKGRP_TO_USER_MIRROR = 70,
      LineConfig = 71, //eLOG_ID_LINE_MIRROR                 = 71,
      LineGroupConfig = 72, //eLOG_ID_LINE_GROUP_MIRROR     = 72,
      LineGroupLines = 73, //eLOG_ID_LINE_TO_GROUP_MIRROR = 73,
      AccountCodeMirror = 74, //eLOG_ID_ACCOUNT_CODE_MIRROR = 74,
      AgentActivityLog = 80, //eLOG_ID_AGENT_ACTIVITY        = 80,//change reverted // modifed from 80 -> 100 to support Agent Queue Activation Report //23874
      AgentQueueActivationHist = 86, //eLOG_ID_AGENT_Q_ACTIVATION_HIST =86,
      ILineGroupStats = 81, //eLOG_ID_ILINE_GROUP_STATS     = 81,
      ILineStats = 82, //eLOG_ID_ILINE_STATS                 = 82,
      FaxEnvelopeHist = 83, //eLOG_ID_FAX_ENVELOPE_HIST     = 83,
      IVRHistory = 84, //eLOG_ID_IVR_HISTORY                 = 84,
      IVRInterval = 85, //eLOG_ID_IVR_INTERVAL          = 85,
      CustomLoggingPassthrough = 9999, //

      //Depricated log IDs
      ID_V1_Call_Detail = 1, //eLOG_ID_V1_CALL_DETAIL              = 1,
      ID_V2_Call_Detail = 10, //eLOG_ID_V2_CALL_DETAIL              = 10,
      ID_V1_IAgent_Queue_Stats = 57, //eLOG_ID_V1_IAGENT_QUEUE_STATS = 57,
      ID_V2_IAgent_Queue_Stats = 90, //eLOG_ID_V2_IAGENT_QUEUE_STATS = 90,
      ID_V1_IStats_Group = 58, //eLOG_ID_V1_ISTATS_GROUP             = 58,
      ID_V2_IStats_Group = 91, //eLOG_ID_V2_ISTATS_GROUP             = 91,
      ID_V1_IWorkgroup_Queue_Stats = 59, //eLOG_ID_V1_IWRKGRP_QUEUE_STATS      = 59,
      ID_V2_IWorkgroup_Queue_Stats = 92 //eLOG_ID_V2_IWRKGRP_QUEUE_STATS      = 92
};


namespace PMQViewer
{
    /**
     * Defines a Log Base message.
     */ 
    public class LogBaseMessage : Message
    {
        private int schema;
        private int log;
        private DateTime creationtime;
        private int hardcode;
        private LOG_QUEUE_MSG_TYPE msgtype;
        private String message1;
        private String message2;
        private List<LogBaseColumn> list;

        ~LogBaseMessage()
        {
            
                message1 = null;
                message2 = null;
                list = null;
        }


        public LogBaseMessage()
        {
            schema = 0;
            log = 0;
            creationtime = new DateTime();
            hardcode = 0;
            msgtype = 0;
            message1 = "";
            message2 = "";
        }

        public LogBaseMessage(int s, int l, DateTime c, int h, LOG_QUEUE_MSG_TYPE m, String m1, String m2, List<LogBaseColumn> lst)
        {
            schema = s;
            log = l;
            creationtime = c;
            hardcode = h;
            msgtype = m;
            message1 = m1;
            message2 = m2;
            list = lst;
        }

        public String getMessage1()
        {
            return message1;
        }

        public string getCompleteSQL()
        {

            return "";
        }

        public String getMessage2()
        {
            return message2;
        }

        public void setMessage1(string m1)
        {
            message1 = m1;
        }

        public void setMessage2(string m2)
        {
            message2 = m2;
        }

        public List<LogBaseColumn> getList()
        {
            return list;
        }

        public void setList(List<LogBaseColumn> lst)
        {
            list = new List<LogBaseColumn>(lst);
        }

        public int getSchema()
        {
            return schema;
        }

        public int getLog()
        {
            return log;
        }

        public DateTime getCreationTime()
        {
            return creationtime;
        }

        public int getHardCode()
        {
            return hardcode;
        }

        public LOG_QUEUE_MSG_TYPE getMsgType()
        {
            return msgtype;
        }

        public void setSchema(int s)
        {
            schema = s;
        }

        public void setLog(int l)
        {
            log = l;
        }

        public void setCreationTime(DateTime c)
        {
            creationtime = c;
        }

        public void setHardCode(int h)
        {
            hardcode = h;
        }

        public void setMsgType(LOG_QUEUE_MSG_TYPE m)
        {
            msgtype = m;
        }
    }
}
