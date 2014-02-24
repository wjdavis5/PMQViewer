using System;

public enum I3DBTRAN_PARAM_TYPES
{	IN = 0,     //eI3DBTRAN_PARAM_TYPE_IN
    OUT = 1,    //eI3DBTRAN_PARAM_TYPE_OUT
    INOUT = 2,   //eI3DBTRAN_PARAM_TYPE_INOUT
    RET = 3,     //eI3DBTRAN_PARAM_TYPE_RET
    
};

public enum I3DBTRAN_DATA_TYPES
{	INT = 0,                //eI3DBTRAN_DATA_TYPE_INT
	INT_NULL = 1,           //eI3DBTRAN_DATA_TYPE_INT_NULL
	ROWCOUNT_INT = 2,       //eI3DBTRAN_DATA_TYPE_ROWCOUNT_INT
	ROWCOUNT_INT_NULL = 3,  //eI3DBTRAN_DATA_TYPE_ROWCOUNT_INT_NULL
	BOOL = 4,               //eI3DBTRAN_DATA_TYPE_BOOL
	BOOL_NULL = 5,          //eI3DBTRAN_DATA_TYPE_BOOL_NULL
	STRING = 6,             //eI3DBTRAN_DATA_TYPE_STRING
	STRING_NULL = 7,        //eI3DBTRAN_DATA_TYPE_STRING_NULL
	REAL = 8,               //eI3DBTRAN_DATA_TYPE_REAL
	REAL_NULL = 9,          //eI3DBTRAN_DATA_TYPE_REAL_NULL
	TIMESTAMP = 10,         //eI3DBTRAN_DATA_TYPE_TIMESTAMP
	TIMESTAMP_NULL = 11,     //eI3DBTRAN_DATA_TYPE_TIMESTAMP_NULL
    
};

namespace PMQViewer
{
    /**
     * Defines an I3DBTran parameter.
     */ 
    
    public class I3DBTranParameter
    {
        //    [  // for each parameter
        //        |paramType      // IN, IN_OUT, OUT, RET
        //        |dataType       // I3DataVariant type
        //        |value          // actual value
        //    ]
        
        private I3DBTRAN_PARAM_TYPES paramType;
        private I3DBTRAN_DATA_TYPES dataType;
        private String value;

        ~I3DBTranParameter()
        {
            value = null;
            
        }
        public I3DBTranParameter(I3DBTRAN_PARAM_TYPES p, I3DBTRAN_DATA_TYPES d, String v)
        {
            paramType = p;
            dataType = d;
            value = v;
        }

        public I3DBTRAN_PARAM_TYPES getParamType()
        {
            return paramType;
        }
        public int getDataType1()
        {
            return (int)dataType;
        }

        public I3DBTRAN_DATA_TYPES getDataType()
        {
            return dataType;
        }

        public String getValue()
        {
            return value;
        }

        public void setParamType(I3DBTRAN_PARAM_TYPES p)
        {
            paramType = p;
        }

        public void setDataType(I3DBTRAN_DATA_TYPES d)
        {
            dataType = d;
        }

        public void setValue(String v)
        {
            value = v;
        }

        public override string ToString()
        {
            return (paramType + "|" + dataType + "|" + value);
        }

        public string[] getCollection()
        {
            String[] str = new String[3];
            str[0] = paramType.ToString();
            str[1] = dataType.ToString();
            str[2] = value;
            return str;
        }
    }
}
