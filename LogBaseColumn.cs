using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

/**
 * Column type enum.  Predefined in
 * a C++ library somewhere.
 */ 
public enum LOG_COLIN_TYPE
{
    eLOG_COLIN_TYPE_FIRST = 0,
    eLOG_COLIN_TYPE_UNKNOWN = 0,
    Integer64 = 1,  //eLOG_COLIN_TYPE_DOUBLE = 1,
    Integer32 = 2,  //eLOG_COLIN_TYPE_LONG = 2,
    String = 3,     //eLOG_COLIN_TYPE_STRING = 3,
    DateTime = 4,   //eLOG_COLIN_TYPE_DATETIME = 4,
    eLOG_COLIN_TYPE_LAST = 4
};

namespace PMQViewer
{
    /**
     * Defines what a Log Base Column is.
     * Since the data is going to be displayed as
     * a string anyway, then I chose to leave it as
     * a string in the class.
     */ 
    public class LogBaseColumn
    {
        private LOG_COLIN_TYPE type;
        private String str;

        ~LogBaseColumn()
        {
            str = null;
        }

        public LogBaseColumn()
        {
            type = 0;
            str = "";
        }

        public LogBaseColumn(LOG_COLIN_TYPE t, String s)
        {
            type = t;
            str = s;
        }

        public LOG_COLIN_TYPE getType()
        {
            return type;
        }

        public void setType(LOG_COLIN_TYPE t)
        {
            type = t;
        }

        public String getStr()
        {
            return str;
        }

        public void setStr(String s)
        {
            str = s;
        }

        public override string ToString()
        {
            return type + " " + str;
        }
    }
}
