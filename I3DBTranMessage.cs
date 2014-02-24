using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using System.Linq;


namespace PMQViewer
{
    /**
     * Defines what an I3DBTran message is.  It extends Message.
     */ 
    public class I3DBTranMessage : Message
    {
        // ----------------------------- Request -------------------------------------
        //
        // The request format is:
        //   "formatVersion|securityToken|uid|tranName|procName|provider|connInfo
        //    |catalog|schema|timeout|txnid|txnreq|txniso|readonly|includeParameterMetaData
        //    |paramCount
        //    [  // for each parameter
        //        |paramType      // IN, IN_OUT, OUT, RET
        //        |dataType       // I3DataVariant type
        //        |value          // actual value
        //    ]
        //    |rowLimit|includeRowSetMetaData|additionalInformation|"
        //
        //
        // ----------------------------- Request ------------------------------------

        private String version;
        private String sectoken;
        private String uid;
        private String tranName;
        private String procName;
        private String provider;
        private String connInfo;
        private String catalog;
        private String schema;
        private String timeout;
        private String txnid;
        private String txnreq;
        private String txniso;
        private String rdonly;
        private String includeParameterMetaData;
        private int paramCount;
        private List<I3DBTranParameter> parameters;
        private String rowLimit;
        private String includeRowSetMetaData;
        private String additionalInformation;
        private String errormsg;

        ~I3DBTranMessage()
        {
            this.version = null;
            this.sectoken = null;
            this.uid = null;
            this.tranName = null;
            this.procName = null;
            this.provider = null;
            this.connInfo = null;
            this.catalog = null;
            this.schema = null;
            this.timeout = null;
            this.txnid = null;
            this.txnreq = null;
            this.txniso = null;
            this.rdonly = null;
            this.includeParameterMetaData = null;
            this.paramCount = 0;
            this.parameters = null;
            this.parameters = null;
            this.rowLimit = null;
            this.includeRowSetMetaData = null;
            this.additionalInformation = null;
            this.errormsg = null;
        }
        public I3DBTranMessage()
        {

        }

        public I3DBTranMessage(String version, String sectoken, String uid, String tranName, String procName,
                               String provider, String connInfo, String catalog, String schema, String timeout,
                               String txnid, String txnreq, String txniso, String rdonly, String includeParameterMetaData,
                               int paramCount, List<I3DBTranParameter> parameters, String rowLimit, String includeRowSetMetaData, 
                               String additionalInformation, String errormsg)
        {
            this.version = version;
            this.sectoken = sectoken;
            this.uid = uid;
            this.tranName = tranName;
            this.procName = procName;
            this.provider = provider;
            this.connInfo = connInfo;
            this.catalog = catalog;
            this.schema = schema;
            this.timeout = timeout;
            this.txnid = txnid;
            this.txnreq = txnreq;
            this.txniso = txniso;
            this.rdonly = rdonly;
            this.includeParameterMetaData = includeParameterMetaData;
            this.paramCount = paramCount;
            this.parameters = new List<I3DBTranParameter>();
            this.parameters.AddRange(parameters);
            this.rowLimit = rowLimit;
            this.includeRowSetMetaData = includeRowSetMetaData;
            this.additionalInformation = additionalInformation;
            this.errormsg = errormsg;
        }

        public String getErrorMsg()
        {
            return errormsg;
        }

        public void setErrorMsg(String emsg)
        {
            errormsg = emsg;
        }

        public String getVersion()
        {
            return version;
        }

        public String getSecToken()
        {
            return sectoken;
        }

        public String getUID()
        {
            return uid;
        }

        public String getTranName()
        {
            return tranName;
        }

        public String getProcName()
        {
            return procName;
        }

        public String getProvider()
        {
            return provider;
        }

        public String getConnInfo()
        {
            return connInfo;
        }

        public String getCatalog()
        {
            return catalog;
        }

        public String getSchema()
        {
            return schema;
        }

        public String getTimeout()
        {
            return timeout;
        }

        public String getTxnid()
        {
            return txnid;
        }

        public String getTxnreq()
        {
            return txnreq;
        }

        public String getTxniso()
        {
            return txniso;
        }

        public String getReadOnly()
        {
            return rdonly;
        }

        public String getIncludeParamMetadata()
        {
            return includeParameterMetaData;
        }

        public int getParamCount()
        {
            return paramCount;
        }

        public List<I3DBTranParameter> getParameters()
        {
            return parameters;
        }
        

        public String getRowLimit()
        {
            return rowLimit;
        }

        public String getIncludeRowSetMetaData()
        {
            return includeRowSetMetaData;
        }

        public String getAdditionalInformation()
        {
            return additionalInformation;
        }

        public void setVersion(String v)
        {
            version = v;
        }

        public void setSecToken(String stoken)
        {
            sectoken = stoken;
        }

        public void setUID(String u)
        {
            uid = u;
        }

        public void setTranName(String t)
        {
            tranName = t;
        }

        public void setProcName(String p)
        {
            procName = p;
        }

        public void setProvider(String p)
        {
            provider = p;
        }

        public void setConnInfo(String c)
        {
            connInfo = c;
        }

        public void setCatalog(String c)
        {
            catalog = c;
        }

        public void setSchema(String s)
        {
            schema = s;
        }

        public void setTimeout(String t)
        {
            timeout = t;
        }

        public void setTxnid(String t)
        {
            txnid = t;
        }

        public void setTxnreq(String t)
        {
            txnreq = t;
        }

        public void setTxniso(String t)
        {
            txniso = t;
        }

        public void setReadOnly(String r)
        {
            rdonly = r;
        }

        public void setIncludParameterMetaData(String i)
        {
            includeParameterMetaData = i;
        }

        public void setParamCount(int p)
        {
            paramCount = p;
        }

        public void setParameters(List<I3DBTranParameter> p)
        {
            parameters.Clear();
            foreach (I3DBTranParameter param in p)
            {
                parameters.Add(param);
            }
        }

        public void setRowLimit(String r)
        {
            rowLimit = r;
        }

        public void setIncludeRowSetMetaData(String i)
        {
            includeRowSetMetaData = i;
        }

        public void setAdditionalInformation(String a)
        {
            additionalInformation = a;
        }

        public override string ToString()
        {
            String p = "";
            foreach (I3DBTranParameter param in parameters)
            {
                p += (" | " + param.ToString());
            }

            String s = version.ToString() + " | " + sectoken.ToString() + " | " + uid + " | " +
                tranName + " | " + procName + " | " + provider + " | " + connInfo + " | " + catalog + " | " +
                schema + " | " + timeout + " | " + txnid + " | " + txnreq + " | " + txniso + " | " + rdonly + " | " +
                includeParameterMetaData + " | " + paramCount.ToString() + p + " | " + rowLimit + " | " +
                includeRowSetMetaData + " | " + additionalInformation;

            return s;
        }
        public string getCompleteSproc(int ServerType) //1 = SQL Server 2 = Oracle
        {
            String p = "";
            if (ServerType == 1)
            {
                p = "";
                p = "EXECUTE ";
                p += procName + " ";
                //p += "(";
                int count = 0;
                foreach (I3DBTranParameter param in parameters)
                {
                    if (count == 42)
                    {
                        int counter = 2;
                        counter++;
                    }
                    if (count == (parameters.Count - 1))
                    {

                        if (param.getValue().Length > 1)
                        {
                            if (param.GetType().ToString() == "10" || param.GetType().ToString() == "11")
                            {
                                p += ("{ts '" + param.getValue().Substring(0,19) + "'}");
                            }
                            else
                            {
                                p += ("'" + param.getValue() + "'");
                            }
                        }
                        else if (param.getValue() == "\0")
                        {
                            p += ("NULL");
                        }
                        else
                        {
                            string Str = param.getValue().Trim();
                            double Num;
                            bool isNum = double.TryParse(Str, out Num);
                            if (isNum)
                            {
                                p += (param.getValue());
                            }
                        }
                        //p += ");";
                        count++;
                    }
                    else
                    {
                        if (param.getValue().Length > 1)
                        {
                            if (param.getDataType1().ToString() == "10")
                            {
                                p += ("{ts '" + param.getValue().Substring(0, 19) + "'},");
                            }
                            else
                            {
                                p += ("'" + param.getValue() + "',");
                            }
                            
                        }
                        else if (param.getValue() == "\0")
                        {
                            p += ("NULL,");
                        }
                        else
                        {
                            string Str = param.getValue().Trim();
                            double Num;
                            bool isNum = double.TryParse(Str, out Num);
                            if (isNum)
                            {
                                p += (param.getValue() + ",");
                            }
                        }
                        count++;
                    }
                }
            }
            else if(ServerType == 2)
            {
                p = "";
                p = "call IC_ADMIN.";
                p += procName.ToUpper();
                p += "(";
                int count = 0;
                foreach (I3DBTranParameter param in parameters)
                {
                    if (count == 42)
                    {
                        int counter = 2;
                        counter++;
                    }
                    if (count == (parameters.Count - 1))
                    {

                        if (param.getValue().Length > 1)
                        {
                            
                            
                                p += ("'" + param.getValue() + "'");
                            
                        }
                        else if (param.getValue() == "\0")
                        {
                            p += ("NULL");
                        }
                        else
                        {
                            string Str = param.getValue().Trim();
                            double Num;
                            bool isNum = double.TryParse(Str, out Num);
                            if (isNum)
                            {
                                p += (param.getValue());
                            }
                        }
                        p += ");";
                        count++;
                    }
                    else
                    {
                        if (param.getValue().Length > 1)
                        {
                            if (param.getDataType1().ToString() == "10")
                            {
                                p += ("TIMESTAMP '" + param.getValue().Substring(0, 19) + "',");
                            }
                            else
                            {
                                p += ("'" + param.getValue() + "',");
                            }
                        }
                        else if (param.getValue() == "\0")
                        {
                            p += ("NULL,");
                        }
                        else
                        {
                            string Str = param.getValue().Trim();
                            double Num;
                            bool isNum = double.TryParse(Str, out Num);
                            if (isNum)
                            {
                                p += (param.getValue() + ",");
                            }
                        }
                        count++;
                    }
                }
            }
            return p;
        }
    }
}
