using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using log4net.Config;

namespace ValueAddDemo
{
    public static class Logger
    {
        private static string LoggerName = "IALogger";
        private static readonly ILog log = LogManager.GetLogger(LoggerName);

        /// <summary>
        /// Logger constructor
        /// </summary>
        static Logger()
        {
            XmlConfigurator.Configure();
        }

        public static ILog GetLogger 
        {
            get { return log;}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void LogMessage(string msg)
        {
            log.Info(msg);
        }      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        public static void LogException(Exception exp)
        {
            log.Error(exp.Message, exp);
        }
    }
}